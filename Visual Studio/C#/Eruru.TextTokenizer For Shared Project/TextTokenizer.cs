using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Eruru.TextTokenizer {

	public class TextTokenizer<T> : IDisposable, IEnumerator<TextTokenizerToken<T>>, IEnumerable<TextTokenizerToken<T>> where T : Enum {

		public T EndType { get; set; }
		public T UnknownType { get; set; }
		public T IntegerType { get; set; }
		public T DecimalType { get; set; }
		public T StringType { get; set; }
		public bool AllowNumber { get; set; } = true;
		public bool AllowString { get; set; } = true;
		public bool AllowSingleQuotString { get; set; } = true;
		public List<char> KeywordEnds { get; private set; } = new List<char> ();
		public TextReader TextReader {

			private get {
				if (_TextReader is null) {
					throw new Exception ($"未指定{nameof (TextReader)}");
				}
				return _TextReader;
			}

			set {
				Index = 0;
				_TextReader = value;
			}

		}
		public Queue<char> Buffer { get; } = new Queue<char> ();
		public int BufferLength { get; set; } = 500;
		public char Character { get; private set; }
		public int Index { get; private set; }

		static readonly char[] NumberCharacters = { '+', '-' };

		readonly Dictionary<char, T> Characters = new Dictionary<char, T> ();
		readonly Dictionary<string, T> Keywords = new Dictionary<string, T> ();

		TextReader _TextReader;
		TextTokenizerToken<T> _Current;
		bool NeedMoveNext = true;

		public TextTokenizer (T endType, T unknownType, T integerType, T decimalType, T stringType) {
			EndType = endType;
			UnknownType = unknownType;
			IntegerType = integerType;
			DecimalType = decimalType;
			StringType = stringType;
		}
		public TextTokenizer (TextReader textReader, T endType, T unknownType, T integerType, T decimalType, T stringType) : this (
			endType,
			unknownType,
			integerType,
			decimalType,
			stringType
		) {
			TextReader = textReader ?? throw new ArgumentNullException (nameof (textReader));
		}

		public void Add (char character, T type) {
			Characters.Add (character, type);
		}
		public void Add (string keyword, T type) {
			if (keyword is null) {
				throw new ArgumentNullException (nameof (keyword));
			}
			Keywords.Add (keyword, type);
		}

		public string ReadTo (string end, bool allowNoEnd = false) {
			if (string.IsNullOrEmpty (end)) {
				throw new ArgumentException ($"“{nameof (end)}”不能是 Null 或为空", nameof (end));
			}
			StringBuilder stringBuilder = new StringBuilder ();
			while (TextReader.Peek () > -1) {
				PeekCharacter ();
				if (Match (stringBuilder, end)) {
					return stringBuilder.ToString ();
				}
				stringBuilder.Append (Character);
				Read ();
			}
			if (allowNoEnd) {
				return stringBuilder.ToString ();
			}
			throw new TextTokenizerException<T> ($"没有遇到{end}结束", this);
		}

		public string ReadNumber (out bool isFloat) {
			StringBuilder stringBuilder = new StringBuilder ();
			isFloat = false;
			while (TextReader.Peek () > -1) {
				PeekCharacter ();
				if (!char.IsDigit (Character) && Array.IndexOf (NumberCharacters, Character) == -1) {
					if (!isFloat && Character == '.') {
						isFloat = true;
					} else {
						break;
					}
				}
				stringBuilder.Append (Character);
				Read ();
			}
			return stringBuilder.ToString ();
		}

		public string ReadString () {
			StringBuilder stringBuilder = new StringBuilder ();
			char end = Character;
			int index = Index;
			int error = 0;
			Read ();
			while (TextReader.Peek () > -1) {
				PeekCharacter ();
				if (Character == '\\') {
					stringBuilder.Append (Character);
					Read ();
					if (TextReader.Peek () == -1) {
						error = 1;
						break;
					}
					stringBuilder.Append (PeekCharacter ());
					Read ();
					continue;
				}
				if (Character == end) {
					Read ();
					return stringBuilder.ToString ();
				}
				stringBuilder.Append (Character);
				Read ();
			}
			Current = new TextTokenizerToken<T> () {
				Type = StringType,
				Index = index,
				Length = stringBuilder.Length + 1,
				Value = stringBuilder.ToString ()
			};
			throw new TextTokenizerException<T> (error == 0 ? "字符串没有结束" : "转义符遇到流结尾", this);
		}

		public string ReadKeyword () {
			StringBuilder stringBuilder = new StringBuilder ();
			while (TextReader.Peek () > -1) {
				PeekCharacter ();
				if (char.IsWhiteSpace (Character) || KeywordEnds.Contains (Character)) {
					if (stringBuilder.Length == 0) {
						stringBuilder.Append (Character);
						Read ();
					}
					break;
				}
				stringBuilder.Append (Character);
				Read ();
			}
			return stringBuilder.ToString ();
		}

		public void SkipWhiteSpace () {
			while (TextReader.Peek () > -1) {
				PeekCharacter ();
				if (!char.IsWhiteSpace (Character)) {
					break;
				}
				Read ();
			}
		}

		public int Read () {
			Index++;
			if (Buffer.Count > BufferLength) {
				Buffer.Dequeue ();
			}
			Buffer.Enqueue (Character);
			return TextReader.Read ();
		}

		public int Peek () {
			return TextReader.Peek ();
		}

		char PeekCharacter () {
			Character = (char)TextReader.Peek ();
			return Character;
		}

		bool Match (StringBuilder stringBuilder, string end) {
			if (stringBuilder is null) {
				throw new ArgumentNullException (nameof (stringBuilder));
			}
			if (string.IsNullOrEmpty (end)) {
				throw new ArgumentException ($"“{nameof (end)}”不能是 Null 或为空", nameof (end));
			}
			if (Character != end[end.Length - 1]) {
				return false;
			}
			if (stringBuilder.Length + 1 < end.Length) {
				return false;
			}
			int start = stringBuilder.Length + 1 - end.Length;
			int length = end.Length - 1;
			for (int i = 0; i < length; i++) {
				if (stringBuilder[start + i] != end[i]) {
					return false;
				}
			}
			stringBuilder.Remove (start, length);
			return true;
		}

		#region IDisposable

		public void Dispose () {
			TextReader.Dispose ();
		}

		#endregion

		#region IEnumerator<TextTokenizerToken<T>>

		public TextTokenizerToken<T> Current {

			get {
				if (NeedMoveNext) {
					MoveNext ();
				}
				return _Current;
			}

			private set => _Current = value;

		}

		object IEnumerator.Current => Current;

		public bool MoveNext () {
			NeedMoveNext = false;
			SkipWhiteSpace ();
			TextTokenizerToken<T> token = new TextTokenizerToken<T> {
				Index = Index
			};
			if (TextReader.Peek () == -1) {
				token.Type = EndType;
				Current = token;
				return false;
			}
			if (Characters.TryGetValue (Character, out T Type)) {
				token.Type = Type;
				token.Length = 1;
				token.Value = Character;
				Read ();
				Current = token;
				return true;
			}
			string text;
			if (AllowNumber && (char.IsDigit (Character) || Array.IndexOf (NumberCharacters, Character) > -1)) {
				text = ReadNumber (out bool isFloat);
				token.Length = text.Length;
				if (isFloat) {
					if (decimal.TryParse (text, out decimal result)) {
						token.Type = DecimalType;
						token.Value = result;
						Current = token;
						return true;
					}
				} else {
					if (long.TryParse (text, out long result)) {
						token.Type = IntegerType;
						token.Value = result;
						Current = token;
						return true;
					}
				}
				token.Type = UnknownType;
				token.Value = text;
				Current = token;
				return true;
			}
			if ((AllowSingleQuotString && Character == '\'') || (AllowSingleQuotString && Character == '"')) {
				text = ReadString ();
				token.Type = StringType;
				token.Length = text.Length + 2;
				token.Value = text;
				Current = token;
				return true;
			}
			text = ReadKeyword ();
			token.Length = text.Length;
			token.Value = text;
			if (Keywords.TryGetValue (text, out Type)) {
				token.Type = Type;
			} else {
				token.Type = UnknownType;
			}
			Current = token;
			return true;
		}

		public void Reset () {
			throw new Exception ($"{nameof (TextReader)}无法{nameof (Reset)}");
		}

		#endregion

		#region IEnumerable<TextTokenizerToken<T>>

		public IEnumerator<TextTokenizerToken<T>> GetEnumerator () {
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator () {
			return this;
		}

		#endregion

	}

}
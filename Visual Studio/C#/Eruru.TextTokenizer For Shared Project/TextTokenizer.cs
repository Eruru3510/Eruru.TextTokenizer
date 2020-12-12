using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Eruru.TextTokenizer {

	public class TextTokenizer : IDisposable, IEnumerator<TextTokenizerToken>, IEnumerable<TextTokenizerToken> {

		public bool AllowNumber { get; set; } = true;
		public bool AllowString { get; set; } = true;
		public bool AllowSingleQuotString { get; set; } = true;
		public TextReader TextReader {

			get {
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

		static readonly char[] NumberCharacters = { '+', '-' };

		readonly List<char> Characters = new List<char> ();
		readonly List<string> Keywords = new List<string> ();

		TextReader _TextReader;
		TextTokenizerToken _Current;
		int Index;
		char Character;
		bool NeedMoveNext = true;

		public TextTokenizer () {

		}
		public TextTokenizer (TextReader textReader) {
			TextReader = textReader ?? throw new ArgumentNullException (nameof (textReader));
		}

		public void Add (char character) {
			Characters.Add (character);
		}
		public void Add (params char[] characters) {
			if (characters is null) {
				throw new ArgumentNullException (nameof (characters));
			}
			Characters.AddRange (characters);
		}
		public void Add (string keyword) {
			if (TextTokenizerAPI.IsNullOrWhiteSpace (keyword)) {
				throw new ArgumentException ($"“{nameof (keyword)}”不能为 Null 或空白", nameof (keyword));
			}
			Keywords.Add (keyword);
		}

		string ReadNumber (out bool isInt) {
			StringBuilder stringBuilder = new StringBuilder ();

			isInt = true;
			while (TextReader.Peek () > -1) {
				Peek ();
				if (!char.IsDigit (Character) && Array.IndexOf (NumberCharacters, Character) == -1) {
					if (isInt && Character == '.') {
						isInt = false;
					} else {
						break;
					}
				}
				stringBuilder.Append (Character);
				Read ();
			}
			return stringBuilder.ToString ();
		}

		string ReadString (char end) {
			StringBuilder stringBuilder = new StringBuilder ();

			Read ();
			while (TextReader.Peek () > -1) {
				Peek ();
				if (Character == '\\') {
					stringBuilder.Append (Character);
					Read ();
					if (TextReader.Peek () < 0) {
						throw new Exception ("转义符遇到流结尾");
					}
					stringBuilder.Append (Peek ());
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
			throw new Exception ("字符串没有结束");
		}

		string ReadKeyword () {
			StringBuilder stringBuilder = new StringBuilder ();
			while (TextReader.Peek () > -1) {
				Peek ();
				if (!char.IsLetter (Character)) {
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

		int Read () {
			Index++;
			if (Buffer.Count > BufferLength) {
				Buffer.Dequeue ();
			}
			Buffer.Enqueue (Character);
			return TextReader.Read ();
		}

		char Peek () {
			Character = (char)TextReader.Peek ();
			return Character;
		}

		#region IDisposable

		public void Dispose () {
			TextReader.Dispose ();
		}

		#endregion

		#region IEnumerator<TextTokenizerToken>

		public TextTokenizerToken Current {

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

			TextTokenizerToken token = new TextTokenizerToken ();
			while (TextReader.Peek () > -1) {
				Peek ();
				if (char.IsWhiteSpace (Character)) {
					Read ();
					continue;
				}
				token.Index = Index;
				if (Characters.Contains (Character)) {
					token.Type = TextTokenizerTokenType.Character;
					token.Length = 1;
					token.Value = Character;
					Current = token;
					Read ();
					return true;
				}
				string text;
				if (AllowNumber && (char.IsDigit (Character) || Array.IndexOf (NumberCharacters, Character) > -1)) {
					text = ReadNumber (out bool isInt);
					token.Length = text.Length;
					if (isInt) {
						if (long.TryParse (text, out long result)) {
							token.Type = TextTokenizerTokenType.Integer;
							token.Value = result;
							Current = token;
							return true;
						}
					} else {
						if (decimal.TryParse (text, out decimal result)) {
							token.Type = TextTokenizerTokenType.Decimal;
							token.Value = result;
							Current = token;
							return true;
						}
					}
					token.Type = TextTokenizerTokenType.Unknown;
					token.Value = text;
					Current = token;
					return true;
				}
				if ((AllowSingleQuotString && Character == '\'') || (AllowSingleQuotString && Character == '"')) {
					text = ReadString (Character);
					token.Type = TextTokenizerTokenType.String;
					token.Length = text.Length + 2;
					token.Value = text;
					Current = token;
					return true;
				}
				text = ReadKeyword ();
				token.Length = text.Length;
				token.Value = text;
				if (Keywords.Contains (text)) {
					token.Type = TextTokenizerTokenType.Keyword;
				} else {
					token.Type = TextTokenizerTokenType.Unknown;
				}
				Current = token;
				return true;
			}
			Current = token;
			return false;
		}

		public void Reset () {
			throw new Exception ($"{nameof (TextReader)}无法{nameof (Reset)}");
		}

		#endregion

		#region IEnumerable<TextTokenizerToken>

		public IEnumerator<TextTokenizerToken> GetEnumerator () {
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator () {
			return this;
		}

		#endregion

	}

}
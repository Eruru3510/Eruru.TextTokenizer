using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
		public bool AllowSymbolsBreakKeyword { get; set; } = true;
		public Queue<char> Buffer { get; } = new Queue<char> ();
		public int BufferLength { get; set; } = 500;
		public int Index {

			get => _Index;

		}
		public TextReader TextReader {

			set {
				_TextReader = value;
				_Index = 0;
				Buffer.Clear ();
			}

		}

		static readonly char[] NumberStartCharacters = { '+', '-' };
		static readonly char[] DecimalCharacters = { '.', 'E', 'e' };

		readonly Dictionary<char, T> Symbols = new Dictionary<char, T> ();
		readonly Dictionary<string, KeyValuePair<T, object>> StringSymbols = new Dictionary<string, KeyValuePair<T, object>> ();
		readonly List<TextTokenizerBlock<T>> Blocks = new List<TextTokenizerBlock<T>> ();
		readonly Dictionary<string, KeyValuePair<T, object>> Keywords = new Dictionary<string, KeyValuePair<T, object>> ();
		readonly List<char> BreakKeywordCharacters = new List<char> ();
		readonly List<KeyValuePair<char, int>> TempBuffer = new List<KeyValuePair<char, int>> ();

		TextReader _TextReader;
		TextTokenizerToken<T> _Current;
		bool NeedMoveNext = true;
		int _Index;

		public TextTokenizer (T endType, T unknownType, T integerType, T decimalType, T stringType) {
			EndType = endType;
			UnknownType = unknownType;
			IntegerType = integerType;
			DecimalType = decimalType;
			StringType = stringType;
		}
		public TextTokenizer (TextReader textReader, T endType, T unknownType, T integerType, T decimalType, T stringType) :
			this (endType, unknownType, integerType, decimalType, stringType) {
			_TextReader = textReader ?? throw new ArgumentNullException (nameof (textReader));
		}

		public void AddSymbol (char symbol, T type) {
			Symbols.Add (symbol, type);
		}
		public void AddStringSymbol (string symbol, T type) {
			if (symbol is null) {
				throw new ArgumentNullException (nameof (symbol));
			}
			StringSymbols.Add (symbol, new KeyValuePair<T, object> (type, symbol));
		}
		public void AddStringSymbol (string symbol, T type, object value) {
			if (symbol is null) {
				throw new ArgumentNullException (nameof (symbol));
			}
			StringSymbols.Add (symbol, new KeyValuePair<T, object> (type, value));
		}
		public void AddKeyword (string keyword, T type) {
			if (keyword is null) {
				throw new ArgumentNullException (nameof (keyword));
			}
			Keywords.Add (keyword, new KeyValuePair<T, object> (type, keyword));
		}
		public void AddKeyword (string keyword, T type, object value) {
			if (keyword is null) {
				throw new ArgumentNullException (nameof (keyword));
			}
			Keywords.Add (keyword, new KeyValuePair<T, object> (type, value));
		}
		public void AddBlock (T type, string head, string tail) {
			if (head is null) {
				throw new ArgumentNullException (nameof (head));
			}
			if (tail is null) {
				throw new ArgumentNullException (nameof (tail));
			}
			Blocks.Add (new TextTokenizerBlock<T> (type, head, tail));
		}
		public void AddBreakKeywordCharacter (char character) {
			BreakKeywordCharacters.Add (character);
		}

		public void SkipWhiteSpace () {
			while (Peek () != -1) {
				char character = PeekCharacter ();
				if (!char.IsWhiteSpace (character)) {
					break;
				}
				Read ();
			}
		}

		public string ReadTo (string end, bool eatLastCharacter = true, bool allowNotFoundEnd = false) {
			if (end is null) {
				throw new ArgumentNullException (nameof (end));
			}
			StringBuilder stringBuilder = new StringBuilder ();
			while (Peek () != -1) {
				char character = PeekCharacter ();
				if (Match (character, end, eatLastCharacter)) {
					return stringBuilder.ToString ();
				}
				stringBuilder.Append (character);
				Read ();
			}
			if (allowNotFoundEnd) {
				return stringBuilder.ToString ();
			}
			throw new TextTokenizerException<T> (this, $"没有遇到{end}结束");
		}

		public void Read () {
			_Index++;
			if (Buffer.Count == BufferLength) {
				Buffer.Dequeue ();
			}
			Buffer.Enqueue (PeekCharacter ());
			if (TempBuffer.Count == 0) {
				_TextReader.Read ();
				return;
			}
			TempBuffer.RemoveAt (0);
		}

		public int Peek () {
			if (TempBuffer.Count == 0) {
				return _TextReader.Peek ();
			}
			return TempBuffer[0].Value;
		}

		public char PeekCharacter () {
			if (TempBuffer.Count == 0) {
				return (char)_TextReader.Peek ();
			}
			return TempBuffer[0].Key;
		}

		string ReadNumber (char character, out bool isDecimal) {
			StringBuilder stringBuilder = new StringBuilder ();
			stringBuilder.Append (character);
			isDecimal = false;
			Read ();
			while (Peek () != -1) {
				character = PeekCharacter ();
				if (!char.IsDigit (character) && Array.IndexOf (NumberStartCharacters, character) == -1) {
					if (Array.IndexOf (DecimalCharacters, character) == -1) {
						break;
					} else {
						isDecimal = true;
					}
				}
				stringBuilder.Append (character);
				Read ();
			}
			return stringBuilder.ToString ();
		}

		string ReadString (int startIndex, char end) {
			StringBuilder stringBuilder = new StringBuilder ();
			Read ();
			while (Peek () != -1) {
				char character = PeekCharacter ();
				if (character == '\\') {
					stringBuilder.Append (character);
					Read ();
					if (Peek () == -1) {
						break;
					}
					character = PeekCharacter ();
				} else if (character == end) {
					Read ();
					return stringBuilder.ToString ();
				}
				stringBuilder.Append (character);
				Read ();
			}
			_Current = new TextTokenizerToken<T> (StringType, startIndex, stringBuilder.Length + 1, stringBuilder.ToString ());
			throw new TextTokenizerException<T> (this, "字符串没有结束");
		}

		string ReadKeyword (char character) {
			StringBuilder stringBuilder = new StringBuilder ();
			stringBuilder.Append (character);
			Read ();
			while (Peek () != -1) {
				character = PeekCharacter ();
				if (char.IsWhiteSpace (character) || BreakKeywordCharacters.Contains (character) || (AllowSymbolsBreakKeyword && Symbols.ContainsKey (character))) {
					if (stringBuilder.Length == 0) {
						stringBuilder.Append (character);
						Read ();
					}
					break;
				}
				stringBuilder.Append (character);
				Read ();
			}
			return stringBuilder.ToString ();
		}

		bool Match (StringBuilder stringBuilder, string end) {
			if (stringBuilder is null) {
				throw new ArgumentNullException (nameof (stringBuilder));
			}
			if (end is null) {
				throw new ArgumentNullException (nameof (end));
			}
			char character = PeekCharacter ();
			if (character != end[end.Length - 1]) {
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

		bool Match (char character, string value, bool eatLastCharacter = true) {
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			if (value.Length == 0 || character != value[0]) {
				return false;
			}
			if (value.Length == 1) {
				return true;
			}
			while (_TextReader.Peek () != -1 && TempBuffer.Count < value.Length) {
				TempBuffer.Add (new KeyValuePair<char, int> ((char)_TextReader.Peek (), _TextReader.Read ()));
			}
			if (TextTokenizerApi.StartsWith (TempBuffer, value)) {
				int length = eatLastCharacter ? value.Length : value.Length - 1;
				for (int i = 0; i < length; i++) {
					Read ();
				}
				return true;
			}
			return false;
		}

		#region IDisposable

		public void Dispose () {
			_TextReader.Dispose ();
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

		}

		object IEnumerator.Current => Current;

		public bool MoveNext () {
			NeedMoveNext = false;
			SkipWhiteSpace ();
			if (Peek () == -1) {
				_Current = new TextTokenizerToken<T> (EndType, Index, 0, null);
				return false;
			}
			char character = PeekCharacter ();
			int startIndex = Index;
			string text;
			foreach (TextTokenizerBlock<T> block in Blocks) {
				if (Match (character, block.Head)) {
					text = ReadTo (block.Tail);
					_Current = new TextTokenizerToken<T> (block.Type, startIndex, text.Length, text);
					return true;
				}
			}
			foreach (KeyValuePair<string, KeyValuePair<T, object>> stringSymbol in StringSymbols) {
				if (Match (character, stringSymbol.Key)) {
					_Current = new TextTokenizerToken<T> (stringSymbol.Value.Key, startIndex, stringSymbol.Key.Length, stringSymbol.Value.Value);
					return true;
				}
			}
			if (Symbols.TryGetValue (character, out T type)) {
				_Current = new TextTokenizerToken<T> (type, startIndex, 1, character);
				Read ();
				return true;
			}
			if (AllowString && (character == '"' || (AllowSingleQuotString && character == '\''))) {
				text = ReadString (startIndex, character);
				_Current = new TextTokenizerToken<T> (StringType, startIndex, text.Length + 2, text);
				return true;
			}
			if (AllowNumber && (char.IsDigit (character) || Array.IndexOf (NumberStartCharacters, character) != -1)) {
				text = ReadNumber (character, out bool isDecimal);
				if (isDecimal) {
					if (decimal.TryParse (text, NumberStyles.Float, null, out decimal result)) {
						_Current = new TextTokenizerToken<T> (DecimalType, startIndex, text.Length, result);
						return true;
					}
				} else {
					if (long.TryParse (text, out long result)) {
						_Current = new TextTokenizerToken<T> (IntegerType, startIndex, text.Length, result);
						return true;
					}
				}
				_Current = new TextTokenizerToken<T> (UnknownType, startIndex, text.Length, text);
				return true;
			}
			text = ReadKeyword (character);
			if (Keywords.TryGetValue (text, out KeyValuePair<T, object> keyValuePair)) {
				_Current = new TextTokenizerToken<T> (keyValuePair.Key, startIndex, text.Length, keyValuePair.Value);
				return true;
			}
			_Current = new TextTokenizerToken<T> (UnknownType, startIndex, text.Length, text);
			return true;
		}

		public void Reset () {
			throw new Exception ($"{nameof (TextReader)}无法{nameof (Reset)}，但你可以为{nameof (TextReader)}属性赋予一个新的{nameof (TextReader)}实现{nameof (Reset)}");
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
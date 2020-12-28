using System;

namespace Eruru.TextTokenizer {

	public struct TextTokenizerToken<T> where T : Enum {

		public T Type { get; }
		public int StartIndex { get; }
		public int Length { get; }
		public object Value { get; }
		public int Int {

			get => ToInt ();

		}
		public long Long {

			get => ToLong ();

		}
		public float Float {

			get => ToFloat ();

		}
		public decimal Decimal {

			get => Decimal;

		}
		public char Char {

			get => ToChar ();

		}
		public string String {

			get => ToString ();

		}

		public TextTokenizerToken (T type, int startIndex, int length, object value) {
			Type = type;
			StartIndex = startIndex;
			Length = length;
			Value = value;
		}

		public int ToInt (int defaultValue = default) {
			try {
				return Convert.ToInt32 (Value);
			} catch {
				return defaultValue;
			}
		}
		public long ToLong (long defaultValue = default) {
			try {
				return Convert.ToInt64 (Value);
			} catch {
				return defaultValue;
			}
		}
		public float ToFloat (float defaultValue = default) {
			try {
				return Convert.ToSingle (Value);
			} catch {
				return defaultValue;
			}
		}
		public decimal ToDecimal (decimal defaultValue = default) {
			try {
				return Convert.ToDecimal (Value);
			} catch {
				return defaultValue;
			}
		}
		public char ToChar (char defaultValue = default) {
			try {
				return Convert.ToChar (Value);
			} catch {
				return defaultValue;
			}
		}
		public string ToString (string defaultValue = default) {
			try {
				return Convert.ToString (Value);
			} catch {
				return defaultValue;
			}
		}

		public static implicit operator int (TextTokenizerToken<T> token) {
			return token.ToInt ();
		}
		public static implicit operator long (TextTokenizerToken<T> token) {
			return token.ToLong ();
		}
		public static implicit operator float (TextTokenizerToken<T> token) {
			return token.ToFloat ();
		}
		public static implicit operator decimal (TextTokenizerToken<T> token) {
			return token.ToDecimal ();
		}
		public static implicit operator char (TextTokenizerToken<T> token) {
			return token.ToChar ();
		}
		public static implicit operator string (TextTokenizerToken<T> token) {
			return token.ToString ();
		}

	}

}
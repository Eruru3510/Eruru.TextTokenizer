using System;

namespace Eruru.TextTokenizer {

	public struct TextTokenizerToken<T> where T : Enum {

		public T Type { get; }
		public int StartIndex { get; }
		public int Length { get; }
		public object Value { get; }
		public byte Byte {

			get => ToByte ();

		}
		public ushort UShort {

			get => ToUShort ();

		}
		public uint UInt {

			get => ToUInt ();

		}
		public ulong ULong {

			get => ToULong ();

		}
		public sbyte SByte {

			get => ToSByte ();

		}
		public short Short {

			get => ToShort ();

		}
		public int Int {

			get => ToInt ();

		}
		public long Long {

			get => ToLong ();

		}
		public float Float {

			get => ToFloat ();

		}
		public double Double {

			get => ToDouble ();

		}
		public decimal Decimal {

			get => Decimal;

		}
		public bool Bool {

			get => ToBool ();

		}
		public char Char {

			get => ToChar ();

		}
		public string String {

			get => ToString ();

		}
		public DateTime DateTime {

			get => ToDateTime ();

		}

		public TextTokenizerToken (T type, int startIndex, int length, object value) {
			Type = type;
			StartIndex = startIndex;
			Length = length;
			Value = value;
		}

		public byte ToByte (byte defaultValue = default) {
			try {
				return Convert.ToByte (Value);
			} catch {
				return defaultValue;
			}
		}
		public ushort ToUShort (ushort defaultValue = default) {
			try {
				return Convert.ToUInt16 (Value);
			} catch {
				return defaultValue;
			}
		}
		public uint ToUInt (uint defaultValue = default) {
			try {
				return Convert.ToUInt32 (Value);
			} catch {
				return defaultValue;
			}
		}
		public ulong ToULong (ulong defaultValue = default) {
			try {
				return Convert.ToUInt64 (Value);
			} catch {
				return defaultValue;
			}
		}
		public sbyte ToSByte (sbyte defaultValue = default) {
			try {
				return Convert.ToSByte (Value);
			} catch {
				return defaultValue;
			}
		}
		public short ToShort (short defaultValue = default) {
			try {
				return Convert.ToInt16 (Value);
			} catch {
				return defaultValue;
			}
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
		public double ToDouble (double defaultValue = default) {
			try {
				return Convert.ToDouble (Value);
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
		public bool ToBool (bool defaultValue = default) {
			try {
				return Convert.ToBoolean (Value);
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
		public DateTime ToDateTime (DateTime defaultValue = default) {
			try {
				return Convert.ToDateTime (Value);
			} catch {
				return defaultValue;
			}
		}

		public static implicit operator byte (TextTokenizerToken<T> token) {
			return token.ToByte ();
		}
		public static implicit operator ushort (TextTokenizerToken<T> token) {
			return token.ToUShort ();
		}
		public static implicit operator uint (TextTokenizerToken<T> token) {
			return token.ToUInt ();
		}
		public static implicit operator ulong (TextTokenizerToken<T> token) {
			return token.ToULong ();
		}
		public static implicit operator sbyte (TextTokenizerToken<T> token) {
			return token.ToSByte ();
		}
		public static implicit operator short (TextTokenizerToken<T> token) {
			return token.ToShort ();
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
		public static implicit operator double (TextTokenizerToken<T> token) {
			return token.ToDouble ();
		}
		public static implicit operator decimal (TextTokenizerToken<T> token) {
			return token.ToDecimal ();
		}
		public static implicit operator bool (TextTokenizerToken<T> token) {
			return token.ToBool ();
		}
		public static implicit operator char (TextTokenizerToken<T> token) {
			return token.ToChar ();
		}
		public static implicit operator string (TextTokenizerToken<T> token) {
			return token.ToString ();
		}
		public static implicit operator DateTime (TextTokenizerToken<T> token) {
			return token.ToDateTime ();
		}

	}

}
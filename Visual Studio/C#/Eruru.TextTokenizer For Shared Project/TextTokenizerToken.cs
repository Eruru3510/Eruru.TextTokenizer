using System;

namespace Eruru.TextTokenizer {

	public struct TextTokenizerToken {

		public TextTokenizerTokenType Type;
		public int Index;
		public int Length;
		public object Value;

		public char Char {

			get {
				try {
					return Convert.ToChar (Value);
				} catch {
					return default;
				}
			}

		}
		public int Int {

			get {
				try {
					return Convert.ToInt32 (Value);
				} catch {
					return default;
				}
			}

		}
		public long Long {

			get {
				try {
					return Convert.ToInt64 (Value);
				} catch {
					return default;
				}
			}

		}
		public decimal Decimal {

			get {
				try {
					return Convert.ToDecimal (Value);
				} catch {
					return default;
				}
			}

		}
		public string String {

			get {
				try {
					return Convert.ToString (Value);
				} catch {
					return default;
				}
			}

		}

	}

}
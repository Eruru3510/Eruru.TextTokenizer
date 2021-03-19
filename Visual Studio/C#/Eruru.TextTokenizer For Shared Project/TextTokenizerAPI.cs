using System;
using System.Collections.Generic;
using System.Reflection;

namespace Eruru.TextTokenizer {

	static class TextTokenizerApi {

		static readonly BindingFlags BindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		public static void SetExceptionMessage (object instance, string message) {
			if (instance is null) {
				throw new ArgumentNullException (nameof (instance));
			}
			if (message is null) {
				throw new ArgumentNullException (nameof (message));
			}
			typeof (Exception).GetField ("_message", BindingFlags).SetValue (instance, message);
		}

		public static bool StartsWith (List<KeyValuePair<char, int>> buffer, string value, bool ignoreCase = false) {
			if (buffer is null) {
				throw new ArgumentNullException (nameof (buffer));
			}
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			if (buffer.Count < value.Length) {
				return false;
			}
			for (int i = 0; i < value.Length; i++) {
				if (!Equals (buffer[i].Key, value[i], ignoreCase)) {
					return false;
				}
			}
			return true;
		}

		public static bool Equals (char a, char b, bool ignoreCase = false) {
			if (ignoreCase) {
				if (char.ToUpperInvariant (a) == char.ToUpperInvariant (b)) {
					return true;
				}
			} else {
				if (a == b) {
					return true;
				}
			}
			return false;
		}

	}

}
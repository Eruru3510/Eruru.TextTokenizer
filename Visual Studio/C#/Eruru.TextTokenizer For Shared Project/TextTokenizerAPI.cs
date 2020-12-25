using System;
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

		public static bool IsNullOrWhiteSpace (string text) {
			if (text is null) {
				return true;
			}
			if (text.Length == 0) {
				return true;
			}
			foreach (char character in text) {
				if (!char.IsWhiteSpace (character)) {
					return false;
				}
			}
			return true;
		}

	}

}
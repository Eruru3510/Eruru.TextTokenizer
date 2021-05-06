using System;
using System.Reflection;

namespace Eruru.TextTokenizer {

	static class ExtensionMethods {

		static readonly FieldInfo ExceptionMessageFieldInfo = typeof (Exception).GetField ("_message", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		public static void SetMessage (this Exception exception, string message) {
			if (exception is null) {
				throw new ArgumentNullException (nameof (exception));
			}
			if (message is null) {
				throw new ArgumentNullException (nameof (message));
			}
			ExceptionMessageFieldInfo.SetValue (exception, message);
		}

	}

}
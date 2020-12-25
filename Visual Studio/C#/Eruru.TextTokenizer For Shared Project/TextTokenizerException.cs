using System;
using System.Text;

namespace Eruru.TextTokenizer {

	public class TextTokenizerException<T> : Exception where T : Enum {

		public TextTokenizerException (string message, TextTokenizer<T> textTokenizer) {
			if (message is null) {
				throw new ArgumentNullException (nameof (message));
			}
			StringBuilder stringBuilder = new StringBuilder ();
			stringBuilder.AppendLine (message);
			stringBuilder.AppendLine (
				$"类型：{textTokenizer.Current.Type} " +
				$"位置：{textTokenizer.Current.Index} " +
				$"长度：{textTokenizer.Current.Length} " +
				$"值：{textTokenizer.Current.Value}"
			);
			stringBuilder.AppendLine (new string (textTokenizer.Buffer.ToArray ()));
			TextTokenizerApi.SetExceptionMessage (this, stringBuilder.ToString ());
		}

	}

}
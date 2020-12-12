using System;
using System.IO;
using Eruru.TextTokenizer;

namespace ConsoleApp1 {

	class Program {

		static void Main (string[] args) {
			Console.Title = nameof (ConsoleApp1);
			TextTokenizer tokenizer = new TextTokenizer (new StringReader ("{'key': \"\\\"Hello, World!\\\"\", [null, true, false, +1, -1, 1.0, 1.1, '\"\"']}")) {
				{ '{', '}', '[', ']', ',', ':'},
				{ "null" },
				{ "true" },
				{ "false" }
			};
			while (tokenizer.MoveNext ()) {
				Console.WriteLine ($"{PadRight (tokenizer.Current.Index)} " +
					$"{PadRight (tokenizer.Current.Length)} " +
					$"{PadRight (tokenizer.Current.Type)} " +
					$"{PadRight (tokenizer.Current.Value)}"
				);
			}
			Console.WriteLine (tokenizer.Buffer.ToArray ());
			Console.ReadLine ();
		}

		static string PadRight (string value) {
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			return value.PadRight (20);
		}
		static string PadRight (object value) {
			return PadRight (value?.ToString () ?? "null");
		}

	}

}
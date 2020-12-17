using System;
using System.IO;
using Eruru.TextTokenizer;

namespace ConsoleApp1 {

	class Program {

		static void Main (string[] args) {
			Console.Title = nameof (ConsoleApp1);
			TestHtml ();
			TestJson ();
			Console.ReadLine ();
		}

		static void TestHtml () {
			TextTokenizer<HtmlTokenType> textTokenizer = new TextTokenizer<HtmlTokenType> (
				new StringReader (@"
						<!doctype html>
						<html>
							<head>
								<meta charset='utf-8' a a/>
								<title>网页</title>
								<style>
									* {}
								</style>
							</head>
						</html>"
				),
				HtmlTokenType.End,
				HtmlTokenType.String,
				HtmlTokenType.Integer,
				HtmlTokenType.Decimal,
				HtmlTokenType.String
			) {
				{ '<', HtmlTokenType.LeftAngleBracket },
				{ '>', HtmlTokenType.RightAngleBracket },
				{ '!', HtmlTokenType.ExclamationMark },
				{ '/', HtmlTokenType.ForwardSlash },
				{ '=', HtmlTokenType.SignOfEquality }
			};
			textTokenizer.KeywordEnds.Add ('=');
			textTokenizer.KeywordEnds.Add ('/');
			textTokenizer.KeywordEnds.Add ('>');
			string name = null;
			while (textTokenizer.MoveNext ()) {
				Console.WriteLine ($"{PadRight (textTokenizer.Current.Index)} " +
					$"{PadRight (textTokenizer.Current.Length)} " +
					$"{PadRight (textTokenizer.Current.Type)} " +
					$"{PadRight (textTokenizer.Current.Value)}"
				);
				if (textTokenizer.Current.Type == HtmlTokenType.String) {
					name = (string)textTokenizer.Current.Value;
				}
				if (textTokenizer.Current.Type == HtmlTokenType.RightAngleBracket && name == "style") {
					name = string.Empty;
					Console.WriteLine (textTokenizer.ReadTo ("<"));
					continue;
				}
			}
			Console.WriteLine (textTokenizer.Buffer.ToArray ());
		}

		static void TestJson () {
			TextTokenizer<JsonTokenType> textTokenizer = new TextTokenizer<JsonTokenType> (
				new StringReader ("{'key': \"\\\"Hello, World!\\\"\", [null, true, false, +1, -1, 1.0, 1.1, '']}"),
				JsonTokenType.End,
				JsonTokenType.Unknown,
				JsonTokenType.Integer,
				JsonTokenType.Decimal,
				JsonTokenType.String
			) {
				{ '{', JsonTokenType.LeftBrace },
				{ '}', JsonTokenType.RightBrace },
				{'[', JsonTokenType.LeftBracket },
				{ ']', JsonTokenType.RightBracket },
				{ ',', JsonTokenType.Comma },
				{ ':',  JsonTokenType.Semicolon },
				{ "null", JsonTokenType.Null },
				{ "true" , JsonTokenType.Bool},
				{ "false" , JsonTokenType.Bool}
			};
			textTokenizer.KeywordEnds.Add (',');
			textTokenizer.KeywordEnds.Add (']');
			textTokenizer.KeywordEnds.Add ('}');
			Print (textTokenizer);
		}

		static void Print<T> (TextTokenizer<T> textTokenizer) where T : Enum {
			while (textTokenizer.MoveNext ()) {
				Console.WriteLine ($"{PadRight (textTokenizer.Current.Index)} " +
					$"{PadRight (textTokenizer.Current.Length)} " +
					$"{PadRight (textTokenizer.Current.Type)} " +
					$"{PadRight (textTokenizer.Current.Value)}"
				);
			}
			Console.WriteLine (textTokenizer.Buffer.ToArray ());
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
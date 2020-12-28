using System;
using System.IO;
using Eruru.TextTokenizer;

namespace ConsoleApp1 {

	class Program {

		const string AssetsPath = @"..\..\..\Assets";

		static void Main (string[] args) {
			Console.Title = nameof (ConsoleApp1);
			TestHtml ();
			Console.ReadLine ();
		}

		static void TestHtml () {
			TextReader textReader = new StringReader (
				@"<!doctype html 'public'>
				<html>
					<head>
						<meta charset='utf-8'/>
						<title>网页</title>
						<style>
							* {
								font-size: 14px;
							}
						</style>
					</head>
					<body>
						<!--注释-->
					</body>//行注释
					<script>
						document.getElementById ('a').innerHTML = '<div></div>';
					</script>
				</html>"
			);
			TextTokenizer<HtmlTokenType> textTokenizer = new TextTokenizer<HtmlTokenType> (
				textReader,
				HtmlTokenType.End,
				HtmlTokenType.String,
				HtmlTokenType.Integer,
				HtmlTokenType.Decimal,
				HtmlTokenType.String
			);
			textTokenizer.AddSymbol ('<', HtmlTokenType.LeftAngleBracket);
			textTokenizer.AddSymbol ('>', HtmlTokenType.RightAngleBracket);
			textTokenizer.AddSymbol ('!', HtmlTokenType.ExclamationMark);
			textTokenizer.AddSymbol ('/', HtmlTokenType.Slash);
			textTokenizer.AddSymbol ('=', HtmlTokenType.EqualSing);
			textTokenizer.AddStringSymbol ("<!", HtmlTokenType.DefineTag);
			textTokenizer.AddStringSymbol ("</", HtmlTokenType.EndTag);
			textTokenizer.AddStringSymbol ("/>", HtmlTokenType.SingleTag);
			textTokenizer.AddBlock (HtmlTokenType.BlockComment, "<!--", "-->");
			textTokenizer.AddBlock (HtmlTokenType.BlockComment, "//", "\n");
			textTokenizer.AllowSymbolsBreakKeyword = false;
			textTokenizer.AddBreakKeywordCharacter ('=');
			textTokenizer.AddBreakKeywordCharacter ('/');
			textTokenizer.AddBreakKeywordCharacter ('>');
			string name = null;
			while (textTokenizer.MoveNext ()) {
				Console.WriteLine (
					$"{PadRight (textTokenizer.Current.StartIndex)} " +
					$"{PadRight (textTokenizer.Current.Length)} " +
					$"{PadRight (textTokenizer.Current.Type)} " +
					$"{PadRight (textTokenizer.Current.Value)}"
				);
				switch (textTokenizer.Current.Type) {
					case HtmlTokenType.String:
						name = textTokenizer.Current;
						break;
					case HtmlTokenType.RightAngleBracket:
						switch (name) {
							case "style":
							case "script":
								Console.WriteLine (textTokenizer.ReadTo ($"</{name}>"));
								name = string.Empty;
								break;
						}
						break;
				}
			}
		}

		static void TestJson () {
			TextTokenizer<JsonTokenType> textTokenizer = new TextTokenizer<JsonTokenType> (
				new StringReader (
					@"{
						'key': 'value',
						'array': [
							null,
							true,
							false,
							1,
							1.0
						]
					}"
				),
				JsonTokenType.End,
				JsonTokenType.Unknown,
				JsonTokenType.Integer,
				JsonTokenType.Decimal,
				JsonTokenType.String
			);
			textTokenizer.AddSymbol ('{', JsonTokenType.LeftBrace);
			textTokenizer.AddSymbol ('}', JsonTokenType.RightBrace);
			textTokenizer.AddSymbol ('[', JsonTokenType.LeftBracket);
			textTokenizer.AddSymbol (']', JsonTokenType.RightBrace);
			textTokenizer.AddSymbol (',', JsonTokenType.Comma);
			textTokenizer.AddSymbol (':', JsonTokenType.Semicolon);
			textTokenizer.AddKeyword ("null", JsonTokenType.Null);
			textTokenizer.AddKeyword ("true", JsonTokenType.Bool);
			textTokenizer.AddKeyword ("false", JsonTokenType.Bool);
			Print (textTokenizer);
		}

		static void Print<T> (TextTokenizer<T> textTokenizer) where T : Enum {
			while (textTokenizer.MoveNext ()) {
				Console.WriteLine ($"{PadRight (textTokenizer.Current.StartIndex)} " +
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
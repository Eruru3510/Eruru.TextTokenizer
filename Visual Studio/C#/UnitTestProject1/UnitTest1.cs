using System;
using System.Collections.Generic;
using System.IO;
using Eruru.TextTokenizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1 {

	[TestClass]
	public class UnitTest1 {

		[TestMethod]
		public void C () {
			string c = @"
				#include<stdio.h>
				int main (int argc, char argv[]) {
					printf ('Hello, World!');//单行注释
					/*块
注
释*/
					return 0;
				}
			";
			using (TextTokenizer<TokenType> tokenizer = new TextTokenizer<TokenType> (
				new StringReader (c),
				TokenType.End,
				TokenType.Unknown,
				TokenType.Integer,
				TokenType.Decimal,
				TokenType.String
			)) {
				tokenizer.AddSymbol ('#', TokenType.NumberSign);
				tokenizer.AddSymbol ('<', TokenType.LessThanSign);
				tokenizer.AddSymbol ('>', TokenType.GreaterThanSign);
				tokenizer.AddSymbol ('(', TokenType.LeftParenthesis);
				tokenizer.AddSymbol (')', TokenType.RightParenthesis);
				tokenizer.AddSymbol ('[', TokenType.LeftBracket);
				tokenizer.AddSymbol (']', TokenType.RightBracket);
				tokenizer.AddSymbol ('{', TokenType.LeftBrace);
				tokenizer.AddSymbol ('}', TokenType.RightBrace);
				tokenizer.AddSymbol (';', TokenType.Semicolon);
				tokenizer.AddSymbol (',', TokenType.Comma);
				tokenizer.AddKeyword ("include", TokenType.Include);
				tokenizer.AddKeyword ("int", TokenType.Int);
				tokenizer.AddKeyword ("char", TokenType.Char);
				tokenizer.AddKeyword ("return", TokenType.Return);
				tokenizer.AddBlock ("//", "\r\n", TokenType.LineComment);
				tokenizer.AddBlock ("//", "\n", TokenType.LineComment);
				tokenizer.AddBlock ("/*", "*/", TokenType.BlockComment);
				KeyValuePair<TokenType, object>[] array = {
					new KeyValuePair<TokenType, object> (TokenType.NumberSign, '#'),
					new KeyValuePair<TokenType, object> (TokenType.Include, "include"),
					new KeyValuePair<TokenType, object> (TokenType.LessThanSign, '<'),
					new KeyValuePair<TokenType, object> (TokenType.Unknown, "stdio.h"),
					new KeyValuePair<TokenType, object> (TokenType.GreaterThanSign, '>'),
					new KeyValuePair<TokenType, object> (TokenType.Int, "int"),
					new KeyValuePair<TokenType, object> (TokenType.Unknown, "main"),
					new KeyValuePair<TokenType, object> (TokenType.LeftParenthesis, '('),
					new KeyValuePair<TokenType, object> (TokenType.Int, "int"),
					new KeyValuePair<TokenType, object> (TokenType.Unknown, "argc"),
					new KeyValuePair<TokenType, object> (TokenType.Comma, ','),
					new KeyValuePair<TokenType, object> (TokenType.Char, "char"),
					new KeyValuePair<TokenType, object> (TokenType.Unknown, "argv"),
					new KeyValuePair<TokenType, object> (TokenType.LeftBracket, '['),
					new KeyValuePair<TokenType, object> (TokenType.RightBracket, ']'),
					new KeyValuePair<TokenType, object> (TokenType.RightParenthesis, ')'),
					new KeyValuePair<TokenType, object> (TokenType.LeftBrace, '{'),
					new KeyValuePair<TokenType, object> (TokenType.Unknown, "printf"),
					new KeyValuePair<TokenType, object> (TokenType.LeftParenthesis, '('),
					new KeyValuePair<TokenType, object> (TokenType.String, "Hello, World!"),
					new KeyValuePair<TokenType, object> (TokenType.RightParenthesis, ')'),
					new KeyValuePair<TokenType, object> (TokenType.Semicolon, ';'),
					new KeyValuePair<TokenType, object> (TokenType.LineComment, "单行注释"),
					new KeyValuePair<TokenType, object> (TokenType.BlockComment, $"块{Environment.NewLine}注{Environment.NewLine}释"),
					new KeyValuePair<TokenType, object> (TokenType.Return, "return"),
					new KeyValuePair<TokenType, object> (TokenType.Integer, 0L),
					new KeyValuePair<TokenType, object> (TokenType.Semicolon, ';'),
					new KeyValuePair<TokenType, object> (TokenType.RightBrace, '}')
				};
				int i = 0;
				while (tokenizer.MoveNext ()) {
					Console.WriteLine ($"索引：{i} 预期：{array[i].Key} 实际：{tokenizer.Current.Type} 预期：{array[i].Value} 实际：{tokenizer.Current.String}");
					Assert.AreEqual (array[i].Key, tokenizer.Current.Type);
					Assert.AreEqual (array[i].Value, tokenizer.Current.Value);
					i++;
				}
			}
		}

		[TestMethod]
		public void Html () {
			string html = @"
				<!DOCTYPE HTML PUBLIC '-//W3C//DTD HTML 4.01 Frameset//EN' 'http://www.w3.org/TR/html4/frameset.dtd'>
				<html>
					<head>
						<meta charset=utf-8/>
						<title>Hello, World!</title>
						<style type='text/css'>h1{color: red;}</style>
					</head>
					<body>
						<h1>Hello, World!</h1>
					</body>
					<Script type='text/javascript'>console.log ('Hello, World!');</script><!--注释-->
				  </html>
			";
			using (TextTokenizer<TokenType> tokenizer = new TextTokenizer<TokenType> (
				new StringReader (html),
				TokenType.End,
				TokenType.Unknown,
				TokenType.Integer,
				TokenType.Decimal,
				TokenType.String,
				true
			)) {
				tokenizer.AllowNumber = false;
				tokenizer.AllowSymbolsBreakKeyword = false;
				tokenizer.AddBreakKeywordCharacter ('<', '>', '=', '/');
				tokenizer.AddSymbol ('<', TokenType.LessThanSign);
				tokenizer.AddSymbol ('>', TokenType.GreaterThanSign);
				tokenizer.AddSymbol ('=', TokenType.EqualSign);
				tokenizer.AddStringSymbol ("<!", TokenType.DefineTag);
				tokenizer.AddStringSymbol ("/>", TokenType.SingleTag);
				tokenizer.AddStringSymbol ("</", TokenType.EndTag);
				tokenizer.AddKeyword ("doctype", TokenType.Doctype);
				tokenizer.AddBlock ("<!--", "-->", TokenType.BlockComment);
				KeyValuePair<TokenType, string>[] array = {
					new KeyValuePair<TokenType, string> (TokenType.DefineTag, "<!"),
					new KeyValuePair<TokenType, string> (TokenType.Doctype, "doctype"),
					new KeyValuePair<TokenType, string> (TokenType.Unknown, "html"),
					new KeyValuePair<TokenType, string> (TokenType.Unknown, "public"),
					new KeyValuePair<TokenType, string> (TokenType.String, "-//W3C//DTD HTML 4.01 Frameset//EN"),
					new KeyValuePair<TokenType, string> (TokenType.String, "http://www.w3.org/TR/html4/frameset.dtd"),
					new KeyValuePair<TokenType, string> (TokenType.GreaterThanSign, ">"),
					new KeyValuePair<TokenType, string> (TokenType.LessThanSign, "<"),
					new KeyValuePair<TokenType, string> (TokenType.Unknown, "html"),
					new KeyValuePair<TokenType, string> (TokenType.GreaterThanSign, ">"),
					new KeyValuePair<TokenType, string> (TokenType.LessThanSign, "<"),
					new KeyValuePair<TokenType, string> (TokenType.Unknown, "head"),
					new KeyValuePair<TokenType, string> (TokenType.GreaterThanSign, ">"),
					new KeyValuePair<TokenType, string> (TokenType.LessThanSign, "<"),
					new KeyValuePair<TokenType, string> (TokenType.Unknown, "meta"),
					new KeyValuePair<TokenType, string> (TokenType.Unknown, "charset"),
					new KeyValuePair<TokenType, string> (TokenType.EqualSign, "="),
					new KeyValuePair<TokenType, string> (TokenType.Unknown, "utf-8"),
					new KeyValuePair<TokenType, string> (TokenType.SingleTag, "/>"),
					new KeyValuePair<TokenType, string> (TokenType.LessThanSign, "<"),
					new KeyValuePair<TokenType, string> (TokenType.Unknown, "title"),
					new KeyValuePair<TokenType, string> (TokenType.GreaterThanSign, ">"),
					new KeyValuePair<TokenType, string> (TokenType.Content, "Hello, World!"),
					new KeyValuePair<TokenType, string> (TokenType.EndTag, "</"),
					new KeyValuePair<TokenType, string> (TokenType.Unknown, "title"),
					new KeyValuePair<TokenType, string> (TokenType.GreaterThanSign, ">"),
					new KeyValuePair<TokenType, string> (TokenType.LessThanSign, "<"),
					new KeyValuePair<TokenType, string> (TokenType.Unknown, "style"),
					new KeyValuePair<TokenType, string> (TokenType.Unknown, "type"),
					new KeyValuePair<TokenType, string> (TokenType.EqualSign, "="),
					new KeyValuePair<TokenType, string> (TokenType.String, "text/css"),
					new KeyValuePair<TokenType, string> (TokenType.GreaterThanSign, ">"),
					new KeyValuePair<TokenType, string> (TokenType.Content, "h1{color: red;}"),
					new KeyValuePair<TokenType, string> (TokenType.EndTag, "</"),
					new KeyValuePair<TokenType, string> (TokenType.Unknown, "style"),
					new KeyValuePair<TokenType, string> (TokenType.GreaterThanSign, ">"),
					new KeyValuePair<TokenType, string> (TokenType.EndTag, "</"),
					new KeyValuePair<TokenType, string> (TokenType.Unknown, "head"),
					new KeyValuePair<TokenType, string> (TokenType.GreaterThanSign, ">"),
					new KeyValuePair<TokenType, string> (TokenType.LessThanSign, "<"),
					new KeyValuePair<TokenType, string> (TokenType.Unknown, "body"),
					new KeyValuePair<TokenType, string> (TokenType.GreaterThanSign, ">"),
					new KeyValuePair<TokenType, string> (TokenType.LessThanSign, "<"),
					new KeyValuePair<TokenType, string> (TokenType.Unknown, "h1"),
					new KeyValuePair<TokenType, string> (TokenType.GreaterThanSign, ">"),
					new KeyValuePair<TokenType, string> (TokenType.Content, "Hello, World!"),
					new KeyValuePair<TokenType, string> (TokenType.EndTag, "</"),
					new KeyValuePair<TokenType, string> (TokenType.Unknown, "h1"),
					new KeyValuePair<TokenType, string> (TokenType.GreaterThanSign, ">"),
					new KeyValuePair<TokenType, string> (TokenType.EndTag, "</"),
					new KeyValuePair<TokenType, string> (TokenType.Unknown, "body"),
					new KeyValuePair<TokenType, string> (TokenType.GreaterThanSign, ">"),
					new KeyValuePair<TokenType, string> (TokenType.LessThanSign, "<"),
					new KeyValuePair<TokenType, string> (TokenType.Unknown, "script"),
					new KeyValuePair<TokenType, string> (TokenType.Unknown, "type"),
					new KeyValuePair<TokenType, string> (TokenType.EqualSign, "="),
					new KeyValuePair<TokenType, string> (TokenType.String, "text/javascript"),
					new KeyValuePair<TokenType, string> (TokenType.GreaterThanSign, ">"),
					new KeyValuePair<TokenType, string> (TokenType.Content, "console.log ('Hello, World!');"),
					new KeyValuePair<TokenType, string> (TokenType.EndTag, "</"),
					new KeyValuePair<TokenType, string> (TokenType.Unknown, "script"),
					new KeyValuePair<TokenType, string> (TokenType.GreaterThanSign, ">"),
					new KeyValuePair<TokenType, string> (TokenType.BlockComment, "注释"),
					new KeyValuePair<TokenType, string> (TokenType.EndTag, "</"),
					new KeyValuePair<TokenType, string> (TokenType.Unknown, "html"),
					new KeyValuePair<TokenType, string> (TokenType.GreaterThanSign, ">"),
				};
				int i = 0;
				bool getTagName = false;
				string tagName = null;
				while (tokenizer.MoveNext ()) {
					if (getTagName) {
						getTagName = false;
						tagName = tokenizer.Current.String;
					} else {
						switch (tokenizer.Current.Type) {
							case TokenType.GreaterThanSign:
							case TokenType.DefineTag:
							case TokenType.EndTag:
								getTagName = true;
								break;
						}
					}
					switch (tokenizer.Current.Type) {
						case TokenType.GreaterThanSign:
							Console.WriteLine ($"索引：{i} 预期：{array[i].Key} 实际：{tokenizer.Current.Type} 预期：{array[i].Value} 实际：{tokenizer.Current.String}");
							Assert.AreEqual (array[i].Key, tokenizer.Current.Type);
							Assert.AreEqual (array[i].Value, tokenizer.Current.String);
							switch (tagName) {
								case "style":
								case "script": {
									string content = tokenizer.ReadTo ($"</{tagName}>");
									i++;
									Console.WriteLine ($"索引：{i} 预期：{array[i].Key} 实际：{TokenType.Content} 预期：{array[i].Value} 实际：{content}");
									Assert.AreEqual (array[i].Key, TokenType.Content);
									Assert.AreEqual (array[i].Value, content);
									break;
								}
								default: {
									string content = tokenizer.ReadTo ("<", false, true);
									if (string.IsNullOrWhiteSpace (content)) {
										break;
									}
									i++;
									Console.WriteLine ($"索引：{i} 预期：{array[i].Key} 实际：{TokenType.Content} 预期：{array[i].Value} 实际：{content}");
									Assert.AreEqual (array[i].Key, TokenType.Content);
									Assert.AreEqual (array[i].Value, content);
									break;
								}
							}
							i++;
							continue;
					}
					Console.WriteLine ($"索引：{i} 预期：{array[i].Key} 实际：{tokenizer.Current.Type} 预期：{array[i].Value} 实际：{tokenizer.Current.String}");
					Assert.AreEqual (array[i].Key, tokenizer.Current.Type);
					Assert.AreEqual (array[i].Value, tokenizer.Current.String, true);
					i++;
				}
			}
		}

		[TestMethod]
		public void Json () {
			string json = @"
				{
					'key': 'Hello, World!',
					'array': [
						null,
						true,
						false,
						0,
						0.1
					]
				}
			";
			using (TextTokenizer<TokenType> tokenizer = new TextTokenizer<TokenType> (
				new StringReader (json),
				TokenType.End,
				TokenType.Unknown,
				TokenType.Integer,
				TokenType.Decimal,
				TokenType.String
			)) {
				tokenizer.AddSymbol ('[', TokenType.LeftBracket);
				tokenizer.AddSymbol (']', TokenType.RightBracket);
				tokenizer.AddSymbol ('{', TokenType.LeftBrace);
				tokenizer.AddSymbol ('}', TokenType.RightBrace);
				tokenizer.AddSymbol (':', TokenType.Colon);
				tokenizer.AddSymbol (',', TokenType.Comma);
				tokenizer.AddKeyword ("null", TokenType.Null);
				tokenizer.AddKeyword ("true", TokenType.True);
				tokenizer.AddKeyword ("false", TokenType.False);
				KeyValuePair<TokenType, object>[] array = {
					new KeyValuePair<TokenType, object> (TokenType.LeftBrace, '{'),
					new KeyValuePair<TokenType, object> (TokenType.String, "key"),
					new KeyValuePair<TokenType, object> (TokenType.Colon, ':'),
					new KeyValuePair<TokenType, object> (TokenType.String, "Hello, World!"),
					new KeyValuePair<TokenType, object> (TokenType.Comma, ','),
					new KeyValuePair<TokenType, object> (TokenType.String, "array"),
					new KeyValuePair<TokenType, object> (TokenType.Colon, ':'),
					new KeyValuePair<TokenType, object> (TokenType.LeftBracket, '['),
					new KeyValuePair<TokenType, object> (TokenType.Null, "null"),
					new KeyValuePair<TokenType, object> (TokenType.Comma, ','),
					new KeyValuePair<TokenType, object> (TokenType.True, "true"),
					new KeyValuePair<TokenType, object> (TokenType.Comma, ','),
					new KeyValuePair<TokenType, object> (TokenType.False, "false"),
					new KeyValuePair<TokenType, object> (TokenType.Comma, ','),
					new KeyValuePair<TokenType, object> (TokenType.Integer, 0L),
					new KeyValuePair<TokenType, object> (TokenType.Comma, ','),
					new KeyValuePair<TokenType, object> (TokenType.Decimal, 0.1M),
					new KeyValuePair<TokenType, object> (TokenType.RightBracket, ']'),
					new KeyValuePair<TokenType, object> (TokenType.RightBrace, '}')
				};
				int i = 0;
				while (tokenizer.MoveNext ()) {
					Console.WriteLine ($"索引：{i} 预期：{array[i].Key} 实际：{tokenizer.Current.Type} 预期：{array[i].Value} 实际：{tokenizer.Current.String}");
					Assert.AreEqual (array[i].Key, tokenizer.Current.Type);
					Assert.AreEqual (array[i].Value, tokenizer.Current.Value);
					i++;
				}
			}
		}

	}

}
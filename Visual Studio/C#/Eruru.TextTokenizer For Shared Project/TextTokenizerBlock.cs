using System;
using System.Collections.Generic;
using System.Text;

namespace Eruru.TextTokenizer {

	public struct TextTokenizerBlock<T> where T : Enum {

		public T Type { get; }
		public string Head { get; }
		public string Tail { get; }

		public TextTokenizerBlock (T type, string head, string tail) {
			Type = type;
			Head = head ?? throw new ArgumentNullException (nameof (head));
			Tail = tail ?? throw new ArgumentNullException (nameof (tail));
		}

	}

}
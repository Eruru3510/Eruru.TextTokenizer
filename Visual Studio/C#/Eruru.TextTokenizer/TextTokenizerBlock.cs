using System;

namespace Eruru.TextTokenizer {

	struct TextTokenizerBlock<T> {

		public T Type { get; set; }
		public string Head { get; set; }
		public string Tail { get; set; }

		public TextTokenizerBlock (string head, string tail, T type) {
			Head = head ?? throw new ArgumentNullException (nameof (head));
			Tail = tail ?? throw new ArgumentNullException (nameof (tail));
			Type = type;
		}

	}

}
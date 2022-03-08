namespace Qrakhen.Sqript {

	internal abstract class Digester<T> {

		protected T[] stack;
		protected int position;


		public Digester(T[] stack) {
			this.stack = stack;
		}


		protected virtual void Reset() {
			position = 0;
		}

		protected virtual void Shift(int shift) {
			position += shift;
			position = (position < 0 ?
				0 : (position < stack.GetUpperBound(0) ?
				position : stack.GetUpperBound(0)));
		}

		protected virtual bool EndOfStack() {
			return (position > stack.Length - 1);
		}

		protected virtual T Peek(int shift) {
			shift += position;
			return stack[(shift < 0 ?
				0 : (shift < stack.GetUpperBound(0) ?
				shift : stack.GetUpperBound(0)))];
		}

		protected virtual T Peek() {
			return stack[(position < stack.GetUpperBound(0) ? position : stack.GetUpperBound(0))];
		}

		protected virtual T Digest() {
			return !EndOfStack() ? stack[position++] : Peek();
		}


		public class Sweeper {

			public Digester<T> Source { get; private set; }


			public Sweeper(Digester<T> source) {
				this.Source = source;
			}


			public T Peek() {
				return Source.Peek();
			}

			public T Digest() {
				return Source.Digest();
			}
		}
	}
}

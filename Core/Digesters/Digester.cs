namespace Qrakhen.Sqript {
	internal abstract class Digester<T> {
		protected T[] stack;
		protected int position;

		public Digester(T[] stack) {
			this.stack = stack;
		}

		protected virtual void reset() {
			position = 0;
		}

		protected virtual void shift(int shift) {
			position += shift;
			position = (position < 0 ?
				0 : (position < stack.GetUpperBound(0) ?
				position : stack.GetUpperBound(0)));
		}

		protected virtual bool endOfStack() {
			return (position > stack.Length - 1);
		}

		protected virtual T peek(int shift) {
			shift = shift + position;
			return stack[(shift < 0 ?
				0 : (shift < stack.GetUpperBound(0) ?
				shift : stack.GetUpperBound(0)))];
		}

		protected virtual T peek() {
			return stack[(position < stack.GetUpperBound(0) ? position : stack.GetUpperBound(0))];
		}

		protected virtual T digest() {
			if(!endOfStack())
				return stack[position++];
			else
				return peek();
		}

		public class Sweeper {
			public Digester<T> source { get; private set; }

			public Sweeper(Digester<T> source) {
				this.source = source;
			}

			public T peek() {
				return source.peek();
			}

			public T digest() {
				return source.digest();
			}
		}
	}
}

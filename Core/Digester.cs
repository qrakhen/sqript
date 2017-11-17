using System;
using System.Collections.Generic;
using System.Text;

namespace Qrakhen.Sqript
{
    public abstract class Digester<T>
    {
        protected T[] stack;
        protected int position;

        public Digester(T[] stack) {
            this.stack = stack;
        }

        protected virtual bool endOfStack() {
            return (position > stack.Length - 1);
        }

        protected virtual T peek() {
            return stack[(position < stack.GetUpperBound(0) ? position : stack.GetUpperBound(0))];
        }

        protected virtual T digest() {
            if (!endOfStack()) return stack[position++];
            else return peek();
        }
    }
}

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

        protected virtual T peek() {
            if (position >= stack.Length) return default(T);
            else return stack[position];
        }

        protected virtual T digest() {
            if (position >= stack.Length) return default(T);
            else return stack[position++];
        }
    }
}

using System.Collections.Generic;

namespace Serialize.Linq.Internals
{
    internal class PairStack<TItem1, TItem2>
    {
        private readonly Stack<KeyValuePair<TItem1, TItem2>> _stack;

        public PairStack()
        {
            _stack = new Stack<KeyValuePair<TItem1, TItem2>>();
        }

        public void Push(TItem1 item1, TItem2 item2)
        {
            _stack.Push(new KeyValuePair<TItem1, TItem2>(item1, item2));
        }

        public bool TryPeek(out TItem1 item1, out TItem2 item2)
        {
            item1 = default(TItem1);
            item2 = default(TItem2);

            if (_stack.Count == 0)
                return false;

            var pair = _stack.Peek();
            item1 = pair.Key;
            item2 = pair.Value;
            return true;
        }

        public bool TryPop(out TItem1 item1, out TItem2 item2)
        {
            item1 = default (TItem1);
            item2 = default (TItem2);

            if (_stack.Count == 0)
                return false;

            var pair = _stack.Pop();
            item1 = pair.Key;
            item2 = pair.Value;
            return true;
        }
    }
}

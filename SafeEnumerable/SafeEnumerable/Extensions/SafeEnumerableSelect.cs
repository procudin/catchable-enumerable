using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SafeEnumerable
{
    public static partial class SafeEnumerable
    {
        public static ISafeEnumerable<TResult> Select<TValue, TResult>(this ISafeEnumerable<TValue> enumerable, Func<TValue, TResult> selector)
            => new SafeEnumerableForSelect<TValue, TResult>(enumerable, selector);
        
        public static ISafeEnumerable<TResult> Select<TValue, TResult>(this ISafeEnumerable<TValue> enumerable, Func<TValue, int, TResult> selector)
            => new SafeEnumerableForSelectWithIdx<TValue, TResult>(enumerable, selector);
    }

    internal class SafeEnumerableForSelect<TValue, TResult> : ISafeEnumerable<TResult>
    {
        private readonly IEnumerable<TValue> _enumerable;

        private readonly Func<TValue, TResult> _selector;
        internal SafeEnumerableForSelect(IEnumerable<TValue> enumerable, Func<TValue, TResult> selector)
        {
            this._enumerable = enumerable;
            this._selector = selector;
        }

        public IEnumerator<TResult> GetEnumerator() => new SafeEnumeratorForSelect<TValue, TResult>(this._enumerable.GetEnumerator(), this._selector);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        internal class SafeEnumeratorForSelect<TValue, TResult> : IEnumerator<TResult>
        {
            private readonly Func<TValue, TResult> _selector;
            private readonly IEnumerator<TValue> _enumerator;
            public SafeEnumeratorForSelect(IEnumerator<TValue> enumerator, Func<TValue, TResult> selector)
            {
                this._enumerator = enumerator;
                this._selector = selector;
            }

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                if (!_enumerator.MoveNext()) return false;

                var current = _selector(this._enumerator.Current);
                this.Current = current;
                return true;
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public TResult Current { get; private set; }

            object IEnumerator.Current => this.Current;
        }
    }

    internal class SafeEnumerableForSelectWithIdx<TValue, TResult> : ISafeEnumerable<TResult>
    {
        private readonly IEnumerable<TValue> _enumerable;

        private readonly Func<TValue, int, TResult> _selector;
        internal SafeEnumerableForSelectWithIdx(IEnumerable<TValue> enumerable, Func<TValue, int, TResult> selector)
        {
            this._enumerable = enumerable;
            this._selector = selector;
        }

        public IEnumerator<TResult> GetEnumerator() => new SafeEnumeratorForSelect<TValue, TResult>(this._enumerable.GetEnumerator(), this._selector);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        internal class SafeEnumeratorForSelect<T, R> : IEnumerator<R>
        {
            private int _idx;
            private readonly Func<T, int, R> _selector;
            private readonly IEnumerator<T> _enumerator;
            public SafeEnumeratorForSelect(IEnumerator<T> enumerator, Func<T, int, R> selector)
            {
                this._enumerator = enumerator;
                this._selector = selector;
                this._idx = 0;
            }

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                if (!_enumerator.MoveNext()) return false;

                var current = _selector(this._enumerator.Current, this._idx);
                this.Current = current;
                ++this._idx;
                return true;
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public R Current { get; private set; }

            object IEnumerator.Current => this.Current;
        }
    }
}

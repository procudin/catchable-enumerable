using System;
using System.Collections;
using System.Collections.Generic;

namespace CatchableEnumerable
{
    public static partial class CatchableEnumerable
    {
        /// <summary>
        /// Extends Enumerable.Select for exception handling
        /// </summary>
        /// <typeparam name="TValue">Source type of objects to enumerate</typeparam>
        /// <typeparam name="TResult">Target type of objects to enumerate</typeparam>
        /// <param name="source">Source enumerable</param>
        /// <param name="selector">Projection</param>
        /// <returns>Target enumerable</returns>
        public static ICatchableEnumerable<TResult> Select<TValue, TResult>(this ICatchableEnumerable<TValue> source, Func<TValue, TResult> selector)
            => new CatchableEnumerableForSelect<TValue, TResult>(source, selector);

        /// <summary>
        /// Extends Enumerable.Select for exception handling
        /// </summary>
        /// <typeparam name="TValue">Source type of objects to enumerate</typeparam>
        /// <typeparam name="TResult">Target type of objects to enumerate</typeparam>
        /// <param name="source">Source enumerable</param>
        /// <param name="selector">Projection</param>
        /// <returns>Target enumerable</returns>
        public static ICatchableEnumerable<TResult> Select<TValue, TResult>(this ICatchableEnumerable<TValue> source, Func<TValue, int, TResult> selector)
            => new CatchableEnumerableForSelectWithIdx<TValue, TResult>(source, selector);
    }

    internal class CatchableEnumerableForSelect<TValue, TResult> : ICatchableEnumerable<TResult>
    {
        private readonly IEnumerable<TValue> _source;

        private readonly Func<TValue, TResult> _selector;

        internal CatchableEnumerableForSelect(IEnumerable<TValue> source, Func<TValue, TResult> selector)
        {
            _source = source;
            _selector = selector;
        }

        public IEnumerator<TResult> GetEnumerator() => new CatchableEnumeratorForSelect<TValue, TResult>(_source.GetEnumerator(), _selector);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class CatchableEnumeratorForSelect<TValue, TResult> : IEnumerator<TResult>
        {
            private readonly Func<TValue, TResult> _selector;

            private readonly IEnumerator<TValue> _enumerator;

            public CatchableEnumeratorForSelect(IEnumerator<TValue> enumerator, Func<TValue, TResult> selector)
            {
                _enumerator = enumerator;
                _selector = selector;
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (!_enumerator.MoveNext()) return false;

                var current = _selector(_enumerator.Current);
                Current = current;
                return true;
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public TResult Current { get; private set; }

            object IEnumerator.Current => Current;
        }
    }

    internal class CatchableEnumerableForSelectWithIdx<TValue, TResult> : ICatchableEnumerable<TResult>
    {
        private readonly IEnumerable<TValue> _source;

        private readonly Func<TValue, int, TResult> _selector;

        internal CatchableEnumerableForSelectWithIdx(IEnumerable<TValue> source, Func<TValue, int, TResult> selector)
        {
            _source = source;
            _selector = selector;
        }

        public IEnumerator<TResult> GetEnumerator() => new CatchableEnumeratorForSelect<TValue, TResult>(_source.GetEnumerator(), _selector);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class CatchableEnumeratorForSelect<T, R> : IEnumerator<R>
        {
            private int _idx;

            private readonly Func<T, int, R> _selector;

            private readonly IEnumerator<T> _enumerator;

            public CatchableEnumeratorForSelect(IEnumerator<T> enumerator, Func<T, int, R> selector)
            {
                _enumerator = enumerator;
                _selector = selector;
                _idx = 0;
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (!_enumerator.MoveNext()) return false;

                var current = _selector(_enumerator.Current, _idx++);
                Current = current;
                return true;
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public R Current { get; private set; }

            object IEnumerator.Current => Current;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;

namespace CatchableEnumerable
{
    public static partial class CatchableEnumerable
    {
        /// <summary>
        /// Extends Enumerable.Where for exception handling
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="source">Enumerable to filter</param>
        /// <param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element</param>
        /// <returns>An <see cref="ICatchableEnumerable{T}"/> that contains elements from the input sequence that satisfy the condition</returns>
        public static ICatchableEnumerable<T> Where<T>(this ICatchableEnumerable<T> source, Func<T, bool> predicate) => 
            new CatchableEnumerableForWhere<T>(source, predicate);

        /// <summary>
        /// Extends Enumerable.Where for exception handling
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="source">Enumerable to filter</param>
        /// <param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element</param>
        /// <returns>An <see cref="ICatchableEnumerable{T}"/> that contains elements from the input sequence that satisfy the condition</returns>
        public static ICatchableEnumerable<T> Where<T>(this ICatchableEnumerable<T> source, Func<T, int, bool> predicate) => 
            new CatchableEnumerableForWhereWithIdx<T>(source, predicate);
    }

    internal class CatchableEnumerableForWhere<T> : ICatchableEnumerable<T>
    {
        private readonly IEnumerable<T> _source;

        private readonly Func<T, bool> _predicate;

        internal CatchableEnumerableForWhere(IEnumerable<T> source, Func<T, bool> predicate)
        {
            _source = source;
            _predicate = predicate;
        }

        public IEnumerator<T> GetEnumerator() => new CatchableEnumeratorForWhere<T>(_source.GetEnumerator(), _predicate);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal class CatchableEnumeratorForWhere<T> : IEnumerator<T>
        {
            private readonly Func<T, bool> _predicate;

            private readonly IEnumerator<T> _enumerator;

            public CatchableEnumeratorForWhere(IEnumerator<T> enumerator, Func<T, bool> predicate)
            {
                _enumerator = enumerator;
                _predicate = predicate;
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public bool MoveNext()
            {
                while (_enumerator.MoveNext())
                {
                    if (_predicate(_enumerator.Current))
                    {
                        Current = _enumerator.Current;
                        return true;
                    }
                }

                return false;
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public T Current { get; private set; }

            object IEnumerator.Current => Current;
        }
    }

    internal class CatchableEnumerableForWhereWithIdx<T> : ICatchableEnumerable<T>
    {
        private readonly IEnumerable<T> _source;

        private readonly Func<T, int, bool> _predicate;

        internal CatchableEnumerableForWhereWithIdx(IEnumerable<T> source, Func<T, int, bool> predicate)
        {
            _source = source;
            _predicate = predicate;
        }

        public IEnumerator<T> GetEnumerator() => new CatchableEnumeratorForWhereWithIdx<T>(_source.GetEnumerator(), _predicate);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal class CatchableEnumeratorForWhereWithIdx<T> : IEnumerator<T>
        {
            private readonly Func<T, int, bool> _predicate;

            private readonly IEnumerator<T> _enumerator;

            private int _idx;

            public CatchableEnumeratorForWhereWithIdx(IEnumerator<T> enumerator, Func<T, int, bool> predicate)
            {
                _enumerator = enumerator;
                _predicate = predicate;
                _idx = 0;
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public bool MoveNext()
            {
                while (_enumerator.MoveNext())
                {
                    if (_predicate(_enumerator.Current, _idx++))
                    {
                        Current = _enumerator.Current;
                        return true;
                    }
                }

                return false;
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public T Current { get; private set; }

            object IEnumerator.Current => Current;
        }
    }
}

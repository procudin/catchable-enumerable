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
        public static ICatchableEnumerable<T> Where<T>(
            this ICatchableEnumerable<T> source, 
            Func<T, bool> predicate)
            => new CatchableEnumerableForWhere<T>(source, predicate);

        /// <summary>
        /// Extends Enumerable.Where for exception handling
        /// </summary>
        /// <typeparam name="T">The type of the elements of source</typeparam>
        /// <param name="source">Enumerable to filter</param>
        /// <param name="predicate">A function to test each source element for a condition; the second parameter of the function represents the index of the source element</param>
        /// <returns>An <see cref="ICatchableEnumerable{T}"/> that contains elements from the input sequence that satisfy the condition</returns>

        public static ICatchableEnumerable<T> Where<T>(
            this ICatchableEnumerable<T> enumerable, 
            Func<T, int, bool> predicate)
            => new CatchableEnumerableForWhereWithIdx<T>(enumerable, predicate);
    }

    internal class CatchableEnumerableForWhere<T> : ICatchableEnumerable<T>
    {
        private readonly IEnumerable<T> enumerable;

        private readonly Func<T, bool> predicate;

        internal CatchableEnumerableForWhere(IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            this.enumerable = enumerable;
            this.predicate = predicate;
        }

        public IEnumerator<T> GetEnumerator() => new CatchableEnumeratorForWhere<T>(this.enumerable.GetEnumerator(), this.predicate);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        internal class CatchableEnumeratorForWhere<T> : IEnumerator<T>
        {
            private readonly Func<T, bool> predicate;

            private readonly IEnumerator<T> enumerator;

            public CatchableEnumeratorForWhere(IEnumerator<T> enumerator, Func<T, bool> predicate)
            {
                this.enumerator = enumerator;
                this.predicate = predicate;
            }

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                while (this.enumerator.MoveNext())
                {
                    if (this.predicate(this.enumerator.Current))
                    {
                        this.Current = this.enumerator.Current;
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

            object IEnumerator.Current => this.Current;
        }
    }

    internal class CatchableEnumerableForWhereWithIdx<T> : ICatchableEnumerable<T>
    {
        private readonly IEnumerable<T> enumerable;

        private readonly Func<T, int, bool> predicate;
        internal CatchableEnumerableForWhereWithIdx(IEnumerable<T> enumerable, Func<T, int, bool> predicate)
        {
            this.enumerable = enumerable;
            this.predicate = predicate;
        }

        public IEnumerator<T> GetEnumerator() => new CatchableEnumeratorForWhereWithIdx<T>(this.enumerable.GetEnumerator(), this.predicate);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        internal class CatchableEnumeratorForWhereWithIdx<T> : IEnumerator<T>
        {
            private readonly Func<T, int, bool> predicate;

            private readonly IEnumerator<T> enumerator;

            private int idx;

            public CatchableEnumeratorForWhereWithIdx(IEnumerator<T> enumerator, Func<T, int, bool> predicate)
            {
                this.enumerator = enumerator;
                this.predicate = predicate;
                this.idx = 0;
            }

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                while (this.enumerator.MoveNext())
                {
                    if (this.predicate(this.enumerator.Current, this.idx++))
                    {
                        this.Current = this.enumerator.Current;
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

            object IEnumerator.Current => this.Current;
        }
    }
}

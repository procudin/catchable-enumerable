using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SafeEnumerable
{
    public static partial class SafeEnumerable
    {
        public static ISafeEnumerable<T> Where<T>(this ISafeEnumerable<T> enumerable, Func<T, bool> predicate)
            => new SafeEnumerableForWhere<T>(enumerable, predicate);

        public static ISafeEnumerable<T> Where<T>(this ISafeEnumerable<T> enumerable, Func<T, int, bool> predicate)
            => new SafeEnumerableForWhereWithIdx<T>(enumerable, predicate);
    }

    internal class SafeEnumerableForWhere<T> : ISafeEnumerable<T>
    {
        private readonly IEnumerable<T> _enumerable;

        private readonly Func<T, bool> _predicate;
        internal SafeEnumerableForWhere(IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            this._enumerable = enumerable;
            this._predicate = predicate;
        }

        public IEnumerator<T> GetEnumerator() => new SafeEnumeratorForWhere<T>(this._enumerable.GetEnumerator(), this._predicate);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        internal class SafeEnumeratorForWhere<T> : IEnumerator<T>
        {
            private readonly Func<T, bool> _predicate;
            private readonly IEnumerator<T> _enumerator;
            public SafeEnumeratorForWhere(IEnumerator<T> enumerator, Func<T, bool> predicate)
            {
                this._enumerator = enumerator;
                this._predicate = predicate;
            }

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                while (this._enumerator.MoveNext())
                {
                    if (this._predicate(this._enumerator.Current))
                    {
                        this.Current = this._enumerator.Current;
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

    internal class SafeEnumerableForWhereWithIdx<T> : ISafeEnumerable<T>
    {
        private readonly IEnumerable<T> _enumerable;

        private readonly Func<T, int, bool> _predicate;
        internal SafeEnumerableForWhereWithIdx(IEnumerable<T> enumerable, Func<T, int, bool> predicate)
        {
            this._enumerable = enumerable;
            this._predicate = predicate;
        }

        public IEnumerator<T> GetEnumerator() => new SafeEnumeratorForWhereWithIdx<T>(this._enumerable.GetEnumerator(), this._predicate);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        internal class SafeEnumeratorForWhereWithIdx<T> : IEnumerator<T>
        {
            private readonly Func<T, int, bool> _predicate;
            private readonly IEnumerator<T> _enumerator;
            private int _idx;
            public SafeEnumeratorForWhereWithIdx(IEnumerator<T> enumerator, Func<T, int, bool> predicate)
            {
                this._enumerator = enumerator;
                this._predicate = predicate;
                this._idx = 0;
            }

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                while (this._enumerator.MoveNext())
                {
                    if (this._predicate(this._enumerator.Current, this._idx++))
                    {
                        this.Current = this._enumerator.Current;
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

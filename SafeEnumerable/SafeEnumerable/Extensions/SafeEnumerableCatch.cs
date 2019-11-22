using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SafeEnumerable
{
    public static partial class SafeEnumerable
    {
        public static ISafeEnumerable<TValue> Catch<TValue, TException>(this ISafeEnumerable<TValue> enumerable, Action<TException> handler)
            where TException : Exception
                => new SafeEnumerableForCatch<TValue, TException>(enumerable, handler, null);

        public static ISafeEnumerable<TValue> Catch<TValue, TException>(this ISafeEnumerable<TValue> enumerable, Action<TException> handler, Func<TException, TValue> defaultValueOnException)
            where TException : Exception
            => new SafeEnumerableForCatch<TValue, TException>(enumerable, handler, defaultValueOnException);

        public static ISafeEnumerable<TValue> Catch<TValue, TException>(this ISafeEnumerable<TValue> enumerable, Action<TException> handler, Func<TValue> defaultValueOnException)
            where TException : Exception
            => new SafeEnumerableForCatch<TValue, TException>(enumerable, handler, _ => defaultValueOnException());
        
        public static void RunSafely<TValue>(this IEnumerable<TValue> enumerable, Action<AggregateException> handler)
        {
            var errors = new List<Exception>();
            var safeEnumerable = enumerable.AsSafe().Catch((Exception e) => errors.Add(e));

            foreach (var _ in safeEnumerable) { }

            if (errors.Count > 0)
            {
                handler(new AggregateException(errors));
            }
        }

        public static void RunSafely<TValue>(this IEnumerable<TValue> enumerable)
        {
            var safeEnumerable = enumerable.AsSafe().Catch((Exception e) => { });
            foreach (var _ in safeEnumerable) { }
        }
    }


    internal class SafeEnumerableForCatch<TValue, TException> : ISafeEnumerable<TValue>
        where TException : Exception
    {
        private readonly IEnumerable<TValue> _enumerable;

        private readonly Action<TException> _handler;

        private readonly Func<TException, TValue> _defaultValueOnException;
        internal SafeEnumerableForCatch(IEnumerable<TValue> enumerable, Action<TException> handler, Func<TException, TValue> defaultValueOnException = null)
        {
            this._enumerable = enumerable;
            this._handler = handler;
            this._defaultValueOnException = defaultValueOnException;
        }

        public IEnumerator<TValue> GetEnumerator() => new SafeEnumeratorForCatch<TValue, TException>(this._enumerable.GetEnumerator(), this._handler, this._defaultValueOnException);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();


        internal class SafeEnumeratorForCatch<TValue, TException> : IEnumerator<TValue>
            where TException : Exception
        {
            private readonly IEnumerator<TValue> _enumerator;

            private readonly Action<TException> _handler;
            private readonly Func<TException, TValue> _defaultValueOnException;
            public SafeEnumeratorForCatch(IEnumerator<TValue> enumerator, Action<TException> handler, Func<TException, TValue> defaultValueOnException = null)
            {
                this._enumerator = enumerator;
                this._handler = handler;
                this._defaultValueOnException = defaultValueOnException;
            }

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                while (true)
                {
                    try
                    {
                        if (this._enumerator.MoveNext())
                        {
                            this.Current = this._enumerator.Current;
                            return true;
                        }

                        return false;
                    }
                    catch (TException e)
                    {
                        this._handler(e);
                        if (this._defaultValueOnException != null)
                        {
                            this.Current = this._defaultValueOnException(e);
                            return true;
                        }
                    }
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public TValue Current { get; private set; }

            object IEnumerator.Current => this.Current;
        }
    }
}

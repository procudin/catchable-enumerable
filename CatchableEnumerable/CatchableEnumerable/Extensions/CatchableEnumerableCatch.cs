using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CatchableEnumerable
{
    public static partial class CatchableEnumerable
    {
        public static ICatchableEnumerable<TValue> Catch<TValue, TException>(
            this ICatchableEnumerable<TValue> enumerable, 
            Action<TException> handler)
            where TException : Exception
                => new CatchableEnumerableForCatch<TValue, TException>(enumerable, handler, null);

        public static ICatchableEnumerable<TValue> Catch<TValue, TException>(
            this ICatchableEnumerable<TValue> enumerable, 
            Action<TException> handler, 
            Func<TException, TValue> defaultValueOnException)
            where TException : Exception
            => new CatchableEnumerableForCatch<TValue, TException>(enumerable, handler, defaultValueOnException);

        public static ICatchableEnumerable<TValue> Catch<TValue, TException>(
            this ICatchableEnumerable<TValue> enumerable, 
            Action<TException> handler, 
            Func<TValue> defaultValueOnException)
            where TException : Exception
            => new CatchableEnumerableForCatch<TValue, TException>(enumerable, handler, _ => defaultValueOnException());
    }


    internal class CatchableEnumerableForCatch<TValue, TException> : ICatchableEnumerable<TValue>
        where TException : Exception
    {
        private readonly IEnumerable<TValue> enumerable;

        private readonly Action<TException> handler;

        private readonly Func<TException, TValue> _defaultValueOnException;
        internal CatchableEnumerableForCatch(IEnumerable<TValue> enumerable, Action<TException> handler, Func<TException, TValue> defaultValueOnException = null)
        {
            this.enumerable = enumerable;
            this.handler = handler;
            this._defaultValueOnException = defaultValueOnException;
        }

        public IEnumerator<TValue> GetEnumerator() => new CatchableEnumeratorForCatch<TValue, TException>(this.enumerable.GetEnumerator(), this.handler, this._defaultValueOnException);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();


        internal class CatchableEnumeratorForCatch<TValue, TException> : IEnumerator<TValue>
            where TException : Exception
        {
            private readonly IEnumerator<TValue> enumerator;

            private readonly Action<TException> handler;

            private readonly Func<TException, TValue> defaultValueOnException;

            public CatchableEnumeratorForCatch(IEnumerator<TValue> enumerator, Action<TException> handler, Func<TException, TValue> defaultValueOnException = null)
            {
                this.enumerator = enumerator;
                this.handler = handler;
                this.defaultValueOnException = defaultValueOnException;
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
                        if (this.enumerator.MoveNext())
                        {
                            this.Current = this.enumerator.Current;
                            return true;
                        }

                        return false;
                    }
                    catch (TException e)
                    {
                        this.handler(e);
                        if (this.defaultValueOnException != null)
                        {
                            this.Current = this.defaultValueOnException(e);
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

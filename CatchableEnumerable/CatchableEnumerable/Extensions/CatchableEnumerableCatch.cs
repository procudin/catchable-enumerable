using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CatchableEnumerable
{
    public static partial class CatchableEnumerable
    {
        /// <summary>
        /// Provides catching exceptions and returns enumerable without values that raises an exception
        /// </summary>
        /// <typeparam name="TValue">The type of objects to enumerate</typeparam>
        /// <typeparam name="TException">Type of Exception to be catched</typeparam>
        /// <param name="enumerable">Source enumerable</param>
        /// <param name="handler">Exception handler</param>
        /// <returns>Enumerable without values that raises an exception</returns>
        public static ICatchableEnumerable<TValue> Catch<TValue, TException>(
            this ICatchableEnumerable<TValue> enumerable, 
            Action<TException> handler)
            where TException : Exception
                => new CatchableEnumerableForCatch<TValue, TException>(enumerable, handler, null);

        /// <summary>
        /// Provides catching exceptions and returns enumerable with user-defined values for failure elements
        /// </summary>
        /// <typeparam name="TValue">The type of objects to enumerate</typeparam>
        /// <typeparam name="TException">Type of Exception to be catched</typeparam>
        /// <param name="enumerable">Source enumerable</param>
        /// <param name="handler">Exception handler</param>
        /// <param name="defaultValueOnException">Value selector for exception state</param>
        /// <returns>Enumerable with user-defined values for failure elements</returns>
        public static ICatchableEnumerable<TValue> Catch<TValue, TException>(
            this ICatchableEnumerable<TValue> enumerable, 
            Action<TException> handler, 
            Func<TException, TValue> defaultValueOnException)
            where TException : Exception
            => new CatchableEnumerableForCatch<TValue, TException>(enumerable, handler, defaultValueOnException);

        /// <summary>
        /// Provides catching exceptions and returns enumerable with user-defined values for failure elements
        /// </summary>
        /// <typeparam name="TValue">The type of objects to enumerate</typeparam>
        /// <typeparam name="TException">Type of Exception to be catched</typeparam>
        /// <param name="enumerable">Source enumerable</param>
        /// <param name="handler">Exception handler</param>
        /// <param name="defaultValueOnException">Value selector for exception state</param>
        /// <returns>Enumerable with user-defined values for failure elements</returns>
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

        private readonly Func<TException, TValue> defaultValueOnException;

        internal CatchableEnumerableForCatch(IEnumerable<TValue> enumerable, Action<TException> handler, Func<TException, TValue> defaultValueOnException = null)
        {
            this.enumerable = enumerable;
            this.handler = handler;
            this.defaultValueOnException = defaultValueOnException;
        }

        public IEnumerator<TValue> GetEnumerator() => new CatchableEnumeratorForCatch<TValue, TException>(this.enumerable.GetEnumerator(), this.handler, this.defaultValueOnException);

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

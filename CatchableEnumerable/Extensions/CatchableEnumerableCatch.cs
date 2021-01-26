using System;
using System.Collections;
using System.Collections.Generic;

namespace CatchableEnumerable
{
    public static partial class CatchableEnumerable
    {
        /// <summary>
        /// Provides catching exceptions and returns enumerable without values that raises an exception
        /// </summary>
        /// <typeparam name="TValue">The type of objects to enumerate</typeparam>
        /// <typeparam name="TException">Type of Exception to be catched</typeparam>
        /// <param name="source">Source enumerable</param>
        /// <param name="handler">Exception handler</param>
        /// <returns>Enumerable without values that raises an exception</returns>
        public static ICatchableEnumerable<TValue> Catch<TValue, TException>(this ICatchableEnumerable<TValue> source, Action<TException> handler) where TException : Exception => 
            new CatchableEnumerableForCatch<TValue, TException>(source, handler);

        /// <summary>
        /// Provides catching exceptions and returns enumerable with user-defined values for failure elements
        /// </summary>
        /// <typeparam name="TValue">The type of objects to enumerate</typeparam>
        /// <typeparam name="TException">Type of Exception to be catched</typeparam>
        /// <param name="source">Source enumerable</param>
        /// <param name="handler">Exception handler</param>
        /// <param name="defaultValueOnException">Value selector for exception state</param>
        /// <returns>Enumerable with user-defined values for failure elements</returns>
        public static ICatchableEnumerable<TValue> Catch<TValue, TException>(this ICatchableEnumerable<TValue> source, Action<TException> handler, Func<TException, TValue> defaultValueOnException) where TException : Exception => 
            new CatchableEnumerableForCatchWithDefault<TValue, TException>(source, handler, defaultValueOnException);

        /// <summary>
        /// Provides catching exceptions and returns enumerable with user-defined values for failure elements
        /// </summary>
        /// <typeparam name="TValue">The type of objects to enumerate</typeparam>
        /// <typeparam name="TException">Type of Exception to be catched</typeparam>
        /// <param name="source">Source enumerable</param>
        /// <param name="handler">Exception handler</param>
        /// <param name="defaultValueOnException">Value selector for exception state</param>
        /// <returns>Enumerable with user-defined values for failure elements</returns>
        public static ICatchableEnumerable<TValue> Catch<TValue, TException>(this ICatchableEnumerable<TValue> source, Action<TException> handler, Func<TValue> defaultValueOnException) where TException : Exception => 
            new CatchableEnumerableForCatchWithDefault<TValue, TException>(source, handler, _ => defaultValueOnException());
    }


    internal class CatchableEnumerableForCatchWithDefault<TValue, TException> : ICatchableEnumerable<TValue>
        where TException : Exception
    {
        private readonly IEnumerable<TValue> _source;

        private readonly Action<TException> _handler;

        private readonly Func<TException, TValue> _defaultValueOnException;

        internal CatchableEnumerableForCatchWithDefault(IEnumerable<TValue> enumerable, Action<TException> handler, Func<TException, TValue> defaultValueOnException = null)
        {
            _source = enumerable;
            _handler = handler;
            _defaultValueOnException = defaultValueOnException;
        }

        public IEnumerator<TValue> GetEnumerator() => 
            new CatchableEnumeratorForCatchWithDefault<TValue, TException>(_source.GetEnumerator(), _handler, _defaultValueOnException);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        private class CatchableEnumeratorForCatchWithDefault<TValue, TException> : IEnumerator<TValue>
            where TException : Exception
        {
            private readonly IEnumerator<TValue> _enumerator;

            private readonly Action<TException> _handler;

            private readonly Func<TException, TValue> _defaultValueOnException;

            public CatchableEnumeratorForCatchWithDefault(IEnumerator<TValue> enumerator, Action<TException> handler, Func<TException, TValue> defaultValueOnException = null)
            {
                _enumerator = enumerator;
                _handler = handler;
                _defaultValueOnException = defaultValueOnException;
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public bool MoveNext()
            {
                while (true)
                {
                    try
                    {
                        if (_enumerator.MoveNext())
                        {
                            Current = _enumerator.Current;
                            return true;
                        }

                        return false;
                    }
                    catch (TException e)
                    {
                        _handler(e);
                        if (_defaultValueOnException != null)
                        {
                            Current = _defaultValueOnException(e);
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

            object IEnumerator.Current => Current;
        }
    }

    internal class CatchableEnumerableForCatch<TValue, TException> : ICatchableEnumerable<TValue>
        where TException : Exception
    {
        private readonly IEnumerable<TValue> _source;

        private readonly Action<TException> _handler;

        internal CatchableEnumerableForCatch(IEnumerable<TValue> enumerable, Action<TException> handler)
        {
            _source = enumerable;
            _handler = handler;
        }

        public IEnumerator<TValue> GetEnumerator() =>
            new CatchableEnumeratorForCatch<TValue, TException>(_source.GetEnumerator(), _handler);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        private class CatchableEnumeratorForCatch<TValue, TException> : IEnumerator<TValue>
            where TException : Exception
        {
            private readonly IEnumerator<TValue> _enumerator;

            private readonly Action<TException> _handler;

            public CatchableEnumeratorForCatch(IEnumerator<TValue> enumerator, Action<TException> handler)
            {
                _enumerator = enumerator;
                _handler = handler;
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public bool MoveNext()
            {
                while (true)
                {
                    try
                    {
                        if (_enumerator.MoveNext())
                        {
                            Current = _enumerator.Current;
                            return true;
                        }

                        return false;
                    }
                    catch (TException e)
                    {
                        _handler(e);
                    }
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public TValue Current { get; private set; }

            object IEnumerator.Current => Current;
        }
    }
}

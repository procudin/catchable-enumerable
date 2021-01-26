using System;
using System.Collections;
using System.Collections.Generic;

namespace CatchableEnumerable
{
    public static partial class CatchableEnumerable
    {
        /// <summary>
        /// Extends Enumerable.SelectMany for exception handling
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <typeparam name="TCollection">The type of the intermediate elements collected by collectionSelector</typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting sequence</typeparam>
        /// <param name="source">A sequence of values to project</param>
        /// <param name="collectionSelector">A transform function to apply to each element of the input sequence</param>
        /// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence</param>
        /// <returns>An <see cref="ICatchableEnumerable{T}"/> whose elements are the result of
        ///     invoking the one-to-many transform function collectionSelector on each element
        ///     of source and then mapping each of those sequence elements and their corresponding
        ///     source element to a result element
        /// </returns>
        public static ICatchableEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this ICatchableEnumerable<TSource> source, Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
            => new CatchableEnumerableForSelectManyWithProjectionWithIdx<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);

        /// <summary>
        /// Extends Enumerable.SelectMany for exception handling
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <typeparam name="TCollection">The type of the intermediate elements collected by collectionSelector</typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting sequence</typeparam>
        /// <param name="source">A sequence of values to project</param>
        /// <param name="collectionSelector">A transform function to apply to each element of the input sequence</param>
        /// <param name="resultSelector">A transform function to apply to each element of the intermediate sequence</param>
        /// <returns>An <see cref="ICatchableEnumerable{T}"/> whose elements are the result of
        ///     invoking the one-to-many transform function collectionSelector on each element
        ///     of source and then mapping each of those sequence elements and their corresponding
        ///     source element to a result element
        /// </returns>
        public static ICatchableEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(this ICatchableEnumerable<TSource> source, Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
            => new CatchableEnumerableForSelectManyWithProjection<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);

        /// <summary>
        /// Extends Enumerable.SelectMany for exception handling
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting sequence</typeparam>
        /// <param name="source">A sequence of values to project</param>
        /// <param name="selector">A transform function to apply to each element of the source sequence</param>
        /// <returns>An <see cref="ICatchableEnumerable{T}"/> whose elements are the result of invoking the one-to-many transform function on each element of the input sequence
        /// </returns>
        public static ICatchableEnumerable<TResult> SelectMany<TSource, TResult>(this ICatchableEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
            => new CatchableEnumerableForSelectMany<TSource, TResult>(source, selector);

        /// <summary>
        /// Extends Enumerable.SelectMany for exception handling
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting sequence</typeparam>
        /// <param name="source">A sequence of values to project</param>
        /// <param name="selector">A transform function to apply to each element of the source sequence</param>
        /// <returns>An <see cref="ICatchableEnumerable{T}"/> whose elements are the result of invoking the one-to-many transform function on each element of the input sequence
        /// </returns>
        public static ICatchableEnumerable<TResult> SelectMany<TSource, TResult>(this ICatchableEnumerable<TSource> source, Func<TSource, int, IEnumerable<TResult>> selector)
            => new CatchableEnumerableForSelectManyWithIdx<TSource, TResult>(source, selector);
    }

    internal class CatchableEnumerableForSelectManyWithProjection<TSource, TCollection, TResult> : ICatchableEnumerable<TResult>
    {
        private readonly IEnumerable<TSource> _source;

        private readonly Func<TSource, IEnumerable<TCollection>> _collectionSelector;

        private readonly Func<TSource, TCollection, TResult> _resultSelector;

        internal CatchableEnumerableForSelectManyWithProjection(
            IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            _source = source;
            _collectionSelector = collectionSelector;
            _resultSelector = resultSelector;
        }

        public IEnumerator<TResult> GetEnumerator()
            => new CatchableEnumeratorForSelectManyWithProjection<TSource,TCollection,TResult>(_source.GetEnumerator(), _collectionSelector, _resultSelector);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class CatchableEnumeratorForSelectManyWithProjection<TSource, TCollection, TResult> : IEnumerator<TResult>
        {
            private readonly IEnumerator<TSource> _enumerator;

            private IEnumerator<TCollection> _innerEnumerator;

            private readonly Func<TSource, IEnumerable<TCollection>> _collectionSelector;

            private readonly Func<TSource, TCollection, TResult> _resultSelector;

            internal CatchableEnumeratorForSelectManyWithProjection(
                IEnumerator<TSource> enumerator,
                Func<TSource, IEnumerable<TCollection>> collectionSelector,
                Func<TSource, TCollection, TResult> resultSelector)
            {
                _enumerator = enumerator;
                _collectionSelector = collectionSelector;
                _resultSelector = resultSelector;
            }

            public bool MoveNext()
            {
                while (true)
                {
                    if (_innerEnumerator != null && _innerEnumerator.MoveNext())
                    {
                        Current = _resultSelector(_enumerator.Current, _innerEnumerator.Current);
                        return true;
                    }

                    _innerEnumerator?.Dispose();

                    if (_enumerator.MoveNext())
                    {
                        _innerEnumerator = _collectionSelector(_enumerator.Current)?.GetEnumerator();
                        continue;
                    }

                    return false;
                    break;
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public TResult Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                _enumerator.Dispose();
            }
        }
    }

    internal class CatchableEnumerableForSelectManyWithProjectionWithIdx<TSource, TCollection, TResult> : ICatchableEnumerable<TResult>
    {
        private readonly IEnumerable<TSource> _source;

        private readonly Func<TSource, int, IEnumerable<TCollection>> _collectionSelector;

        private readonly Func<TSource, TCollection, TResult> _resultSelector;

        internal CatchableEnumerableForSelectManyWithProjectionWithIdx(
            IEnumerable<TSource> source,
            Func<TSource, int, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            _source = source;
            _collectionSelector = collectionSelector;
            _resultSelector = resultSelector;
        }

        public IEnumerator<TResult> GetEnumerator()
            => new CatchableEnumeratorForSelectManyWithProjectionWithIdx<TSource, TCollection, TResult>(_source.GetEnumerator(), _collectionSelector, _resultSelector);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class CatchableEnumeratorForSelectManyWithProjectionWithIdx<TSource, TCollection, TResult> : IEnumerator<TResult>
        {
            private readonly IEnumerator<TSource> _enumerator;

            private IEnumerator<TCollection> _innerEnumerator;

            private readonly Func<TSource, int, IEnumerable<TCollection>> _collectionSelector;

            private readonly Func<TSource, TCollection, TResult> _resultSelector;

            private int _idx;

            internal CatchableEnumeratorForSelectManyWithProjectionWithIdx(IEnumerator<TSource> enumerator,
                Func<TSource, int, IEnumerable<TCollection>> collectionSelector,
                Func<TSource, TCollection, TResult> resultSelector)
            {
                _enumerator = enumerator;
                _collectionSelector = collectionSelector;
                _resultSelector = resultSelector;
                _idx = 0;
            }

            public bool MoveNext()
            {
                while (true)
                {
                    if (_innerEnumerator != null && _innerEnumerator.MoveNext())
                    {
                        Current = _resultSelector(_enumerator.Current, _innerEnumerator.Current);
                        return true;
                    }

                    _innerEnumerator?.Dispose();

                    if (_enumerator.MoveNext())
                    {
                        _innerEnumerator = _collectionSelector(_enumerator.Current, _idx++)?.GetEnumerator();
                        continue;
                    }

                    return false;
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public TResult Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                _enumerator.Dispose();
            }
        }
    }

    internal class CatchableEnumerableForSelectMany<TSource, TResult> : ICatchableEnumerable<TResult>
    {
        private readonly IEnumerable<TSource> _source;

        private readonly Func<TSource, IEnumerable<TResult>> _selector;

        internal CatchableEnumerableForSelectMany(
            IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TResult>> selector)
        {
            _source = source;
            _selector = selector;
        }

        public IEnumerator<TResult> GetEnumerator()
            => new CatchableEnumeratorForSelectMany<TSource, TResult>(_source.GetEnumerator(), _selector);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class CatchableEnumeratorForSelectMany<TSource, TResult> : IEnumerator<TResult>
        {
            private readonly IEnumerator<TSource> _enumerator;

            private readonly Func<TSource, IEnumerable<TResult>> _selector;

            private IEnumerator<TResult> _innerEnumerator;

            internal CatchableEnumeratorForSelectMany(
                IEnumerator<TSource> enumerator,
                Func<TSource, IEnumerable<TResult>> selector)
            {
                _enumerator = enumerator;
                _selector = selector;
            }

            public bool MoveNext()
            {
                while (true)
                {
                    if (_innerEnumerator != null)
                    {
                        if (_innerEnumerator.MoveNext())
                        {
                            Current = _innerEnumerator.Current;
                            return true;
                        }
                        else
                        {
                            _innerEnumerator.Dispose();
                        }
                    }

                    if (_enumerator.MoveNext())
                    {
                        _innerEnumerator = _selector(_enumerator.Current)?.GetEnumerator();
                        continue;
                    }

                    return false;
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public TResult Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                _enumerator.Dispose();
            }
        }
    }

    internal class CatchableEnumerableForSelectManyWithIdx<TSource, TResult> : ICatchableEnumerable<TResult>
    {
        private readonly IEnumerable<TSource> _source;

        private readonly Func<TSource, int, IEnumerable<TResult>> _selector;

        internal CatchableEnumerableForSelectManyWithIdx(
            IEnumerable<TSource> source,
            Func<TSource, int, IEnumerable<TResult>> selector)
        {
            _source = source;
            _selector = selector;
        }

        public IEnumerator<TResult> GetEnumerator() => 
            new CatchableEnumeratorForSelectManyWithIdx<TSource, TResult>(_source.GetEnumerator(), _selector);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        private class CatchableEnumeratorForSelectManyWithIdx<TSource, TResult> : IEnumerator<TResult>
        {
            private readonly IEnumerator<TSource> _enumerator;

            private readonly Func<TSource, int, IEnumerable<TResult>> _selector;

            private IEnumerator<TResult> _innerEnumerator;

            private int _idx;

            internal CatchableEnumeratorForSelectManyWithIdx(
                IEnumerator<TSource> enumerator,
                Func<TSource, int, IEnumerable<TResult>> selector)
            {
                _enumerator = enumerator;
                _selector = selector;
                _idx = 0;
            }

            public bool MoveNext()
            {
                while (true)
                {
                    if (_innerEnumerator != null)
                    {
                        if (_innerEnumerator.MoveNext())
                        {
                            Current = _innerEnumerator.Current;
                            return true;
                        }
                        else
                        {
                            _innerEnumerator.Dispose();
                        }
                    }

                    if (_enumerator.MoveNext())
                    {
                        _innerEnumerator = _selector(_enumerator.Current, _idx++).GetEnumerator();
                        continue;
                    }

                    return false;
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public TResult Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                _innerEnumerator?.Dispose();
                _enumerator.Dispose();
            }
        }
    }
}
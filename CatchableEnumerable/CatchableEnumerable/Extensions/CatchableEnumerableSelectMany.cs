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
        public static ICatchableEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(
            this ICatchableEnumerable<TSource> source, 
            Func<TSource, int, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
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
        public static ICatchableEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(
            this ICatchableEnumerable<TSource> source,
            Func<TSource, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
            => new CatchableEnumerableForSelectManyWithProjection<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);

        /// <summary>
        /// Extends Enumerable.SelectMany for exception handling
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <typeparam name="TCollection">The type of the intermediate elements collected by collectionSelector</typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting sequence</typeparam>
        /// <param name="source">A sequence of values to project</param>
        /// <param name="selector">A transform function to apply to each element of the source sequence</param>
        /// <returns>An <see cref="ICatchableEnumerable{T}"/> whose elements are the result of invoking the one-to-many transform function on each element of the input sequence
        /// </returns>
        public static ICatchableEnumerable<TResult> SelectMany<TSource, TResult>(
            this ICatchableEnumerable<TSource> source, 
            Func<TSource, IEnumerable<TResult>> selector)
            => new CatchableEnumerableForSelectMany<TSource, TResult>(source, selector);

        /// <summary>
        /// Extends Enumerable.SelectMany for exception handling
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source</typeparam>
        /// <typeparam name="TCollection">The type of the intermediate elements collected by collectionSelector</typeparam>
        /// <typeparam name="TResult">The type of the elements of the resulting sequence</typeparam>
        /// <param name="source">A sequence of values to project</param>
        /// <param name="selector">A transform function to apply to each element of the source sequence</param>
        /// <returns>An <see cref="ICatchableEnumerable{T}"/> whose elements are the result of invoking the one-to-many transform function on each element of the input sequence
        /// </returns>
        public static ICatchableEnumerable<TResult> SelectMany<TSource, TResult>(
            this ICatchableEnumerable<TSource> source,
            Func<TSource, int, IEnumerable<TResult>> selector)
            => new CatchableEnumerableForSelectManyWithIdx<TSource, TResult>(source, selector);
    }

    internal class CatchableEnumerableForSelectManyWithProjection<TSource, TCollection, TResult> : ICatchableEnumerable<TResult>
    {
        private readonly IEnumerable<TSource> source;

        private readonly Func<TSource, IEnumerable<TCollection>> collectionSelector;

        private readonly Func<TSource, TCollection, TResult> resultSelector;

        internal CatchableEnumerableForSelectManyWithProjection(
            IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            this.source = source;
            this.collectionSelector = collectionSelector;
            this.resultSelector = resultSelector;
        }

        public IEnumerator<TResult> GetEnumerator()
            => new CatchableEnumeratorForSelectManyWithProjection<TSource,TCollection,TResult>(
                this.source.GetEnumerator(), 
                this.collectionSelector, 
                this.resultSelector);

        IEnumerator IEnumerable.GetEnumerator() 
            => this.GetEnumerator();

        private class CatchableEnumeratorForSelectManyWithProjection<TSource, TCollection, TResult> : IEnumerator<TResult>
        {
            private readonly IEnumerator<TSource> enumerator;

            private IEnumerator<TCollection> innerEnumerator;

            private readonly Func<TSource, IEnumerable<TCollection>> collectionSelector;

            private readonly Func<TSource, TCollection, TResult> resultSelector;

            internal CatchableEnumeratorForSelectManyWithProjection(IEnumerator<TSource> enumerator,
                Func<TSource, IEnumerable<TCollection>> collectionSelector,
                Func<TSource, TCollection, TResult> resultSelector)
            {
                this.enumerator = enumerator;
                this.collectionSelector = collectionSelector;
                this.resultSelector = resultSelector;
            }

            public bool MoveNext()
            {
                if (this.innerEnumerator != null && this.innerEnumerator.MoveNext())
                {
                    this.Current = this.resultSelector(this.enumerator.Current, this.innerEnumerator.Current);
                    return true;
                }

                if (enumerator.MoveNext())
                {
                    this.innerEnumerator = this.collectionSelector(enumerator.Current)?.GetEnumerator();
                    return this.MoveNext();
                }

                return false;
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public TResult Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
        }
    }

    internal class CatchableEnumerableForSelectManyWithProjectionWithIdx<TSource, TCollection, TResult> : ICatchableEnumerable<TResult>
    {
        private readonly IEnumerable<TSource> source;

        private readonly Func<TSource, int, IEnumerable<TCollection>> collectionSelector;

        private readonly Func<TSource, TCollection, TResult> resultSelector;

        internal CatchableEnumerableForSelectManyWithProjectionWithIdx(
            IEnumerable<TSource> source,
            Func<TSource, int, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            this.source = source;
            this.collectionSelector = collectionSelector;
            this.resultSelector = resultSelector;
        }

        public IEnumerator<TResult> GetEnumerator()
            => new CatchableEnumeratorForSelectManyWithProjectionWithIdx<TSource, TCollection, TResult>(
                this.source.GetEnumerator(),
                this.collectionSelector,
                this.resultSelector);

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();

        private class CatchableEnumeratorForSelectManyWithProjectionWithIdx<TSource, TCollection, TResult> : IEnumerator<TResult>
        {
            private readonly IEnumerator<TSource> enumerator;

            private IEnumerator<TCollection> innerEnumerator;

            private readonly Func<TSource, int, IEnumerable<TCollection>> collectionSelector;

            private readonly Func<TSource, TCollection, TResult> resultSelector;

            private int idx;

            internal CatchableEnumeratorForSelectManyWithProjectionWithIdx(IEnumerator<TSource> enumerator,
                Func<TSource, int, IEnumerable<TCollection>> collectionSelector,
                Func<TSource, TCollection, TResult> resultSelector)
            {
                this.enumerator = enumerator;
                this.collectionSelector = collectionSelector;
                this.resultSelector = resultSelector;
                this.idx = 0;
            }

            public bool MoveNext()
            {
                if (this.innerEnumerator != null && this.innerEnumerator.MoveNext())
                {
                    this.Current = this.resultSelector(this.enumerator.Current, this.innerEnumerator.Current);
                    return true;
                }

                if (enumerator.MoveNext())
                {
                    this.innerEnumerator = this.collectionSelector(enumerator.Current, this.idx++)?.GetEnumerator();
                    return this.MoveNext();
                }

                return false;
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public TResult Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
        }
    }

    internal class CatchableEnumerableForSelectMany<TSource, TResult> : ICatchableEnumerable<TResult>
    {
        private readonly IEnumerable<TSource> source;

        private readonly Func<TSource, IEnumerable<TResult>> selector;

        internal CatchableEnumerableForSelectMany(
            IEnumerable<TSource> source,
            Func<TSource, IEnumerable<TResult>> selector)
        {
            this.source = source;
            this.selector = selector;
        }

        public IEnumerator<TResult> GetEnumerator()
            => new CatchableEnumeratorForSelectMany<TSource, TResult>(
                this.source.GetEnumerator(),
                this.selector);

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();

        private class CatchableEnumeratorForSelectMany<TSource, TResult> : IEnumerator<TResult>
        {
            private readonly IEnumerator<TSource> enumerator;

            private readonly Func<TSource, IEnumerable<TResult>> selector;

            private IEnumerator<TResult> innerEnumerator;

            internal CatchableEnumeratorForSelectMany(
                IEnumerator<TSource> enumerator,
                 Func<TSource, IEnumerable<TResult>> selector)
            {
                this.enumerator = enumerator;
                this.selector = selector;
            }

            public bool MoveNext()
            {
                if (this.innerEnumerator != null && this.innerEnumerator.MoveNext())
                {
                    this.Current = this.innerEnumerator.Current;
                    return true;
                }

                if (enumerator.MoveNext())
                {
                    this.innerEnumerator = this.selector(enumerator.Current)?.GetEnumerator();
                    return this.MoveNext();
                }

                return false;
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public TResult Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
        }
    }

    internal class CatchableEnumerableForSelectManyWithIdx<TSource, TResult> : ICatchableEnumerable<TResult>
    {
        private readonly IEnumerable<TSource> source;

        private readonly Func<TSource, int, IEnumerable<TResult>> selector;

        internal CatchableEnumerableForSelectManyWithIdx(
            IEnumerable<TSource> source,
            Func<TSource, int, IEnumerable<TResult>> selector)
        {
            this.source = source;
            this.selector = selector;
        }

        public IEnumerator<TResult> GetEnumerator()
            => new CatchableEnumeratorForSelectManyWithIdx<TSource, TResult>(
                this.source.GetEnumerator(),
                this.selector);

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();

        private class CatchableEnumeratorForSelectManyWithIdx<TSource, TResult> : IEnumerator<TResult>
        {
            private readonly IEnumerator<TSource> enumerator;

            private readonly Func<TSource, int, IEnumerable<TResult>> selector;

            private IEnumerator<TResult> innerEnumerator;

            private int idx;

            internal CatchableEnumeratorForSelectManyWithIdx(
                IEnumerator<TSource> enumerator,
                Func<TSource, int, IEnumerable<TResult>> selector)
            {
                this.enumerator = enumerator;
                this.selector = selector;
                this.idx = 0;
            }

            public bool MoveNext()
            {
                if (this.innerEnumerator != null && this.innerEnumerator.MoveNext())
                {
                    this.Current = this.innerEnumerator.Current;
                    return true;
                }

                if (enumerator.MoveNext())
                {
                    this.innerEnumerator = this.selector(enumerator.Current, idx++)?.GetEnumerator();
                    return this.MoveNext();
                }

                return false;
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public TResult Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
        }
    }
}
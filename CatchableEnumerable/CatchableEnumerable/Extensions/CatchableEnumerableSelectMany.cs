using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CatchableEnumerable
{
    public static partial class CatchableEnumerable
    {
        public static ICatchableEnumerable<TResult> SelectMany<TSource, TCollection, TResult>(
            this ICatchableEnumerable<TSource> source, 
            Func<TSource, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
            => new CatchableEnumerableForSelectMany<TSource, TCollection, TResult>(source, collectionSelector, resultSelector);

        public static ICatchableEnumerable<TResult> SelectMany<TSource, TResult>(
            this ICatchableEnumerable<TSource> source, 
            Func<TSource, IEnumerable<TResult>> selector)
            => new CatchableEnumerableForSelectMany<TSource, TResult, TResult>(source, selector, (a, b) => b);
    }

    internal class CatchableEnumerableForSelectMany<TSource, TCollection, TResult> : ICatchableEnumerable<TResult>
    {
        private readonly IEnumerable<TSource> enumerable;

        private readonly Func<TSource, IEnumerable<TCollection>> collectionSelector;

        private readonly Func<TSource, TCollection, TResult> resultSelector;

        internal CatchableEnumerableForSelectMany(IEnumerable<TSource> enumerable,
            Func<TSource, IEnumerable<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            this.enumerable = enumerable;
            this.collectionSelector = collectionSelector;
            this.resultSelector = resultSelector;
        }

        public IEnumerator<TResult> GetEnumerator()
            => new CatchableEnumeratorForSelectMany<TSource,TCollection,TResult>(this.enumerable.GetEnumerator(), this.collectionSelector, this.resultSelector);

        IEnumerator IEnumerable.GetEnumerator() 
            => this.GetEnumerator();

        internal class CatchableEnumeratorForSelectMany<TSource, TCollection, TResult> : IEnumerator<TResult>
        {
            private readonly IEnumerator<TSource> enumerator;

            private IEnumerator<TCollection> innerEnumerator;

            private readonly Func<TSource, IEnumerable<TCollection>> _collectionSelector;

            private readonly Func<TSource, TCollection, TResult> _resultSelector;

            internal CatchableEnumeratorForSelectMany(IEnumerator<TSource> enumerator,
                Func<TSource, IEnumerable<TCollection>> collectionSelector,
                Func<TSource, TCollection, TResult> resultSelector)
            {
                this.enumerator = enumerator;
                this._collectionSelector = collectionSelector;
                this._resultSelector = resultSelector;
            }

            public bool MoveNext()
            {
                if (this.innerEnumerator != null && this.innerEnumerator.MoveNext())
                {
                    this.Current = this._resultSelector(this.enumerator.Current, this.innerEnumerator.Current);
                    return true;
                }

                if (enumerator.MoveNext())
                {
                    this.innerEnumerator = this._collectionSelector(enumerator.Current)?.GetEnumerator();
                    if (this.innerEnumerator != null && this.innerEnumerator.MoveNext())
                    {
                        this.Current = this._resultSelector(this.enumerator.Current, this.innerEnumerator.Current);
                        return true;
                    }
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
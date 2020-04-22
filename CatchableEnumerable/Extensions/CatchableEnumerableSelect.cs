﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace CatchableEnumerable
{
    public static partial class CatchableEnumerable
    {
        /// <summary>
        /// Extends Enumerable.Select for exception handling
        /// </summary>
        /// <typeparam name="TValue">Source type of objects to enumerate</typeparam>
        /// <typeparam name="TResult">Target type of objects to enumerate</typeparam>
        /// <param name="source">Source enumerable</param>
        /// <param name="selector">Projection</param>
        /// <returns>Target enumerable</returns>
        public static ICatchableEnumerable<TResult> Select<TValue, TResult>(
            this ICatchableEnumerable<TValue> source, 
            Func<TValue, TResult> selector)
            => new CatchableEnumerableForSelect<TValue, TResult>(source, selector);

        /// <summary>
        /// Extends Enumerable.Select for exception handling
        /// </summary>
        /// <typeparam name="TValue">Source type of objects to enumerate</typeparam>
        /// <typeparam name="TResult">Target type of objects to enumerate</typeparam>
        /// <param name="source">Source enumerable</param>
        /// <param name="selector">Projection</param>
        /// <returns>Target enumerable</returns>
        public static ICatchableEnumerable<TResult> Select<TValue, TResult>(
            this ICatchableEnumerable<TValue> source, 
            Func<TValue, int, TResult> selector)
            => new CatchableEnumerableForSelectWithIdx<TValue, TResult>(source, selector);
    }

    internal class CatchableEnumerableForSelect<TValue, TResult> : ICatchableEnumerable<TResult>
    {
        private readonly IEnumerable<TValue> source;

        private readonly Func<TValue, TResult> selector;

        internal CatchableEnumerableForSelect(IEnumerable<TValue> source, Func<TValue, TResult> selector)
        {
            this.source = source;
            this.selector = selector;
        }

        public IEnumerator<TResult> GetEnumerator() => new CatchableEnumeratorForSelect<TValue, TResult>(this.source.GetEnumerator(), this.selector);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        internal class CatchableEnumeratorForSelect<TValue, TResult> : IEnumerator<TResult>
        {
            private readonly Func<TValue, TResult> selector;

            private readonly IEnumerator<TValue> enumerator;

            public CatchableEnumeratorForSelect(IEnumerator<TValue> enumerator, Func<TValue, TResult> selector)
            {
                this.enumerator = enumerator;
                this.selector = selector;
            }

            public void Dispose()
            {
                this.enumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (!enumerator.MoveNext()) return false;

                var current = selector(this.enumerator.Current);
                this.Current = current;
                return true;
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public TResult Current { get; private set; }

            object IEnumerator.Current => this.Current;
        }
    }

    internal class CatchableEnumerableForSelectWithIdx<TValue, TResult> : ICatchableEnumerable<TResult>
    {
        private readonly IEnumerable<TValue> source;

        private readonly Func<TValue, int, TResult> selector;

        internal CatchableEnumerableForSelectWithIdx(IEnumerable<TValue> source, Func<TValue, int, TResult> selector)
        {
            this.source = source;
            this.selector = selector;
        }

        public IEnumerator<TResult> GetEnumerator() => new CatchableEnumeratorForSelect<TValue, TResult>(this.source.GetEnumerator(), this.selector);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        internal class CatchableEnumeratorForSelect<T, R> : IEnumerator<R>
        {
            private int idx;

            private readonly Func<T, int, R> selector;

            private readonly IEnumerator<T> enumerator;

            public CatchableEnumeratorForSelect(IEnumerator<T> enumerator, Func<T, int, R> selector)
            {
                this.enumerator = enumerator;
                this.selector = selector;
                this.idx = 0;
            }

            public void Dispose()
            {
                this.enumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (!enumerator.MoveNext()) return false;

                var current = selector(this.enumerator.Current, this.idx++);
                this.Current = current;
                return true;
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public R Current { get; private set; }

            object IEnumerator.Current => this.Current;
        }
    }
}
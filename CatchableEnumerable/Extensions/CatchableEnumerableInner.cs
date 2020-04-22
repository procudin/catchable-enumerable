using System.Collections;
using System.Collections.Generic;

namespace CatchableEnumerable
{
    /// <summary>
    /// Internal implementation of <see cref="ICatchableEnumerable{T}"/> interface
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate</typeparam>
    internal class CatchableEnumerableInner<T> : ICatchableEnumerable<T>
    {
        private readonly IEnumerable<T> source;

        internal CatchableEnumerableInner(IEnumerable<T> source)
        {
            this.source = source;
        }
        
        public IEnumerator<T> GetEnumerator() => this.source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}

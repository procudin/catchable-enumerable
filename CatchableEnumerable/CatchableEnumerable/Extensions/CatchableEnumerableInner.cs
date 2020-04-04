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
        private readonly IEnumerable<T> enumerable;

        internal CatchableEnumerableInner(IEnumerable<T> enumerable)
        {
            this.enumerable = enumerable;
        }
        
        public IEnumerator<T> GetEnumerator() => this.enumerable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}

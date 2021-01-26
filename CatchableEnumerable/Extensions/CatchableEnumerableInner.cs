using System.Collections;
using System.Collections.Generic;

namespace CatchableEnumerable
{
    /// <summary>
    /// Internal implementation of <see cref="ICatchableEnumerable{T}"/> interface
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate</typeparam>
    internal readonly struct CatchableEnumerableInner<T> : ICatchableEnumerable<T>
    {
        private readonly IEnumerable<T> _source;

        internal CatchableEnumerableInner(IEnumerable<T> source)
        {
            _source = source;
        }
        
        public IEnumerator<T> GetEnumerator() => _source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

using System.Collections.Generic;

namespace CatchableEnumerable
{
    /// <summary>
    /// Provides Linq extensions
    /// </summary>
    public static partial class CatchableEnumerable
    {
        /// <summary>
        /// Moves <see cref="IEnumerable{T}"/> to catchable context
        /// </summary>
        /// <typeparam name="T">The type of objects to enumerate</typeparam>
        /// <param name="source">Source enumerable</param>
        /// <returns>Enumerable with catchable context</returns>
        public static ICatchableEnumerable<T> AsCatchable<T>(this IEnumerable<T> source) => 
            (source is ICatchableEnumerable<T> enumerable)
                ? enumerable
                : new CatchableEnumerableInner<T>(source);
    }
}

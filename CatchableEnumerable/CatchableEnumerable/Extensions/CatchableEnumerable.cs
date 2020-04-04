using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        /// <param name="enumerable">Source enumerable</param>
        /// <returns>Enumerable with catchable context</returns>
        public static ICatchableEnumerable<T> AsCatchable<T>(this IEnumerable<T> enumerable)
            => (enumerable is ICatchableEnumerable<T>)
                ? (ICatchableEnumerable<T>)enumerable
                : new CatchableEnumerableInner<T>(enumerable);
    }
}

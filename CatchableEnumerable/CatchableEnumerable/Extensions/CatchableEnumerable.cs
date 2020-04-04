using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CatchableEnumerable
{
    public static partial class CatchableEnumerable
    {
        public static ICatchableEnumerable<T> AsCatchable<T>(this IEnumerable<T> enumerable)
            => (enumerable is ICatchableEnumerable<T>)
                ? (ICatchableEnumerable<T>)enumerable
                : new CatchableEnumerableInner<T>(enumerable);
    }
}

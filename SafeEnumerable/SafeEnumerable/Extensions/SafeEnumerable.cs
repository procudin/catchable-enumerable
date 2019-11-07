using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeEnumerable
{
    public static partial class SafeEnumerable
    {
        public static ISafeEnumerable<T> FromValue<T>(T value)
            => Enumerable.Repeat(value, 1).AsSafe();

        public static ISafeEnumerable<T> AsSafe<T>(this IEnumerable<T> enumerable)
            => new SafeEnumerableInner<T>(enumerable);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace CatchableEnumerableTests
{
    internal static class HelperExtensions
    {
        internal static string JoinWith(this IEnumerable<string> source, string delim)
            => string.Join(delim, source);
    }
}

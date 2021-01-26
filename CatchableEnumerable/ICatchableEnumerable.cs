using System.Collections.Generic;

namespace CatchableEnumerable
{
    /// <summary>
    /// Extends <see cref="IEnumerable{T}"/> for exception handling
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate</typeparam>
    public interface ICatchableEnumerable<T> : IEnumerable<T>
    {
    }
}

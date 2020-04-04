using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CatchableEnumerable
{
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

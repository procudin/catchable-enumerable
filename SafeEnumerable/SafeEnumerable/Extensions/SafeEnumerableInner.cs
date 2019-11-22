using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SafeEnumerable
{
    internal class SafeEnumerableInner<T> : ISafeEnumerable<T>
    {
        private readonly IEnumerable<T> _enumerable;

        internal SafeEnumerableInner(IEnumerable<T> enumerable)
        {
            this._enumerable = enumerable;
        }
        
        public IEnumerator<T> GetEnumerator() => this._enumerable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}

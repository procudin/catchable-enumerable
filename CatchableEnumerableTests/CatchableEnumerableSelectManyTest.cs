using System;
using System.Collections.Generic;
using System.Text;

namespace CatchableEnumerableTests
{
    using System.Linq;

    using CatchableEnumerable;

    using Xunit;

    public class CatchableEnumerableSelectManyTest
    {
        private static bool FuncWithEx(int val)
        {
            if (val == 2)
                throw new ArgumentException("2");
            if (val == 3)
                throw new InvalidOperationException("3");

            return true;
        }


        [Fact]
        public void SelectManyWithLetTest()
        {
            var collection = Enumerable.Range(0, 4).ToList();
            
            var query1 = 
                from i in collection.AsCatchable()
                from j in collection 
                let tmp = FuncWithEx(j)
                select j.ToString();
            Assert.Throws<ArgumentException>(() => query1.ToList());

            var safeQuery = query1.Catch((Exception e) => { });
            Assert.Equal("0,1,0,1,0,1,0,1", safeQuery.JoinWith(","));

            var query2 =
                from i in collection.AsCatchable()
                from j in collection
                let tmp = FuncWithEx(i)
                select j.ToString();
            Assert.Throws<ArgumentException>(() => query2.ToList());

            var safeQuery2 = query2.Catch((Exception e) => { });
            Assert.Equal("0,1,0,1,0,1,0,1", safeQuery.JoinWith(","));
        }
    }
}

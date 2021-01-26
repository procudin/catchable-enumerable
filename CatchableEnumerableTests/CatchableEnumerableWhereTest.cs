using System;
using System.Linq;
using CatchableEnumerable;
using Xunit;

namespace CatchableEnumerableTests
{
    public class CatchableEnumerableWhereTest
    {
        private static bool TestPredicate(int val)
        {
            if (val == 2)
                throw new ArgumentException("2");
            if (val == 3)
                throw new InvalidOperationException("3");

            return true;
        }

        [Fact]
        public void WhereTest()
        {
            var collectionWithEx = Enumerable.Range(0, 5)
                .AsCatchable()
                .Where(TestPredicate);

            // Must throw ArgumentException exception by default
            Assert.Throws<ArgumentException>(() => collectionWithEx.ToList());

            // Must throw InvalidOperationException exception if first was handled
            Assert.Throws<InvalidOperationException>(() =>
                collectionWithEx
                    .Catch((ArgumentException e) => { /* Do nothing */ })
                    .ToList());

            // Must throw ArgumentException if we dont handle it
            Assert.Throws<ArgumentException>(() =>
                collectionWithEx
                    .Catch((InvalidOperationException e) => { /* Do nothing */ })
                    .ToList());

            // Must not throw if we handle all exceptions
            collectionWithEx
                .Catch((ArgumentException e) => { /* Do nothing */ })
                .Catch((InvalidOperationException e) => { /* Do nothing */ })
                .ToList();

            // Must not throw if we handle all exceptions
            collectionWithEx
                .Catch((Exception e) => { /* Do nothing */ })
                .ToList();
        }


        [Fact]
        public void WhereWithIdxTest()
        {
            var collectionWithEx = Enumerable.Range(5, 5)
                .AsCatchable()
                .Where((v, idx) => TestPredicate(idx));

            // Must throw ArgumentException exception by default
            Assert.Throws<ArgumentException>(() => collectionWithEx.ToList());

            // Must throw InvalidOperationException exception if first was handled
            Assert.Throws<InvalidOperationException>(() =>
                collectionWithEx
                    .Catch((ArgumentException e) => { /* Do nothing */ })
                    .ToList());

            // Must throw ArgumentException if we dont handle it
            Assert.Throws<ArgumentException>(() =>
                collectionWithEx
                    .Catch((InvalidOperationException e) => { /* Do nothing */ })
                    .ToList());

            // Must not throw if we handle all exceptions
            collectionWithEx
                .Catch((ArgumentException e) => { /* Do nothing */ })
                .Catch((InvalidOperationException e) => { /* Do nothing */ })
                .ToList();

            // Must not throw if we handle all exceptions
            collectionWithEx
                .Catch((Exception e) => { /* Do nothing */ })
                .ToList();
        }


        [Fact]
        public void WhereDataTest()
        {
            var collectionWithEx = Enumerable.Range(5, 5)
                .AsCatchable()
                .Where((v, idx) => TestPredicate(idx))
                .Select(v => v.ToString());

            // Skip failure values
            var data = collectionWithEx
                .Catch((Exception e) => { /* Do nothing */ })
                .ToList();
            Assert.Equal("5,6,9", data.JoinWith(","));
        }
    }
}

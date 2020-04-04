using CatchableEnumerable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CatchableEnumerableTests
{
    
    public class CatchableEnumerableSelectTest
    {
        [Fact]
        public void SelectCatchingTest()
        {
            var collectionWithEx = Enumerable.Range(0, 5)
                .AsCatchable()
                .Select(v =>
                {
                    if (v == 2)
                        throw new ArgumentException("2");
                    if (v == 3)
                        throw new InvalidOperationException("3");

                    return v.ToString();
                });


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

            // Must not throw if we handle all exceptions + handler action test            
            collectionWithEx
                .Catch((ArgumentException e) => {  })
                .Catch((InvalidOperationException e) => { })
                .ToList();
        }

        [Fact]
        public void SelectDataTest()
        {
            var collectionWithEx = Enumerable.Range(0, 5)
                .AsCatchable()
                .Select(v =>
                {
                    if (v == 2)
                        throw new ArgumentException("2");
                    if (v == 3)
                        throw new InvalidOperationException("3");

                    return v.ToString();
                });

            // Skip failure values by default
            var data = collectionWithEx
                .Catch((Exception e) => { /* Do nothing */ })
                .ToList();
            Assert.Equal("0,1,4", data.JoinWith(","));

            // Replace failure values with "error"
            data = collectionWithEx
                .Catch((Exception e) => { /* Do nothing */ }, v => "error")
                .ToList();
            Assert.Equal("0,1,error,error,4", data.JoinWith(","));

            // Replace failure values with "<exceptionname>"
            data = collectionWithEx
                .Catch((ArgumentException e) => { /* Do nothing */ }, v => "ArgumentException")
                .Catch((InvalidOperationException e) => { /* Do nothing */ }, v => "InvalidOperationException")
                .ToList();
            Assert.Equal("0,1,ArgumentException,InvalidOperationException,4", data.JoinWith(","));
        }

        [Fact]
        public void SelectWithIdxCatchingTest()
        {
            var collectionWithEx = Enumerable.Range(5, 5)
                .AsCatchable()
                .Select((v, idx) =>
                {
                    if (idx == 2)
                        throw new ArgumentException("2");
                    if (idx == 3)
                        throw new InvalidOperationException("3");

                    return v.ToString();
                });


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
        public void SelectWithIdxDataTest()
        {
            var collectionWithEx = Enumerable.Range(5, 5)
                .AsCatchable()
                .Select((v, idx) =>
                {
                    if (idx == 2)
                        throw new ArgumentException("2");
                    if (idx == 3)
                        throw new InvalidOperationException("3");

                    return v.ToString();
                });

            // Skip failure values by default
            var data = collectionWithEx
                .Catch((Exception e) => { /* Do nothing */ })
                .ToList();
            Assert.Equal("5,6,9", data.JoinWith(","));

            // Replace failure values with "error"
            data = collectionWithEx
                .Catch((Exception e) => { /* Do nothing */ }, v => "error")
                .ToList();
            Assert.Equal("5,6,error,error,9", data.JoinWith(","));

            // Replace failure values with "<exceptionname>"
            data = collectionWithEx
                .Catch((ArgumentException e) => { /* Do nothing */ }, v => "ArgumentException")
                .Catch((InvalidOperationException e) => { /* Do nothing */ }, v => "InvalidOperationException")
                .ToList();
            Assert.Equal("5,6,ArgumentException,InvalidOperationException,9", data.JoinWith(","));
        }
    }
}

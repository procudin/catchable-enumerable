## Catch extension for LINQ

Extends default LINQ operations with exception handling method.

[![Nuget](https://img.shields.io/nuget/v/CatchableEnumerable?style=plastic)](https://www.nuget.org/packages/CatchableEnumerable/)

## Motivation

Sometimes there is a situation when we need to perform several consecutive transformations for the original collection. This can be done easily with LINQ.

```cs
var source = new[] { "123", "0", null, "1" };
var target = source
    .Select(v => int.Parse(v)) // can throw Exception for incorrect string
    .Select(v => v * 2)
    .ToArray();
```

But sometimes it's happens that a function called inside a request can throw an exception, which in some cases can be ignored or replaced with default value. However we must write a bunch of boilerplate to handle this exception:

```cs
var target = source
    .Select(v => 
        {
            try
            {
                return int.Parse(v);
            }
            catch (Exception)
            {
                return -1; // some default behaviour 
            }
        })
    .Select(v => v * 2)
    .ToArray();
```

The CatchableEnumerable library takes care of all the work, allowing you to write concise functional code with declarative style:

```cs
var target = source.AsCatchable() // move source to catchable context
    .Select(v => int.Parse(v))
    .Catch((Exception e) => { /* some action */ }, () => -1) 
    .Select(v => v * 2)
    .ToArray();
```

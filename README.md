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

# Details

The library uses it's own interface -- ```ICatchableEnumerable<T>```, which is just a wrapper for  ```IEnumerable<T>```. It can be created from ```IEnumerable<T>``` with ```AsCatchable()``` extension method.
```cs
var source = Enumerable.Range(0, 5); // typeof(source) == IEnumerable<int>
var target = source.AsCatchable();   // typeof(target) == ICatchableEnumerable<int>
```

Objects of ```ICatchableEnumerable<T>``` can use library-provided extenions for ```Select```, ```SelectMany``` and ```Where``` operations. This extensions consume and produce ```ICatchableEnumerable``` objects, so it is possible to create a chain of exception-safe transformations: 
```cs
target = target
    .Select(i => {
        if (i == 2)
            throw new ArgumentException("2");
        return i;
    })
    .Where(i => {
        if (i == 3)
            throw new NotImplementedException("3");
        return true;
    });
```
Finally you can handle exceptions with ```Catch``` extension method:

```cs
var handledTarget = target
    .Catch((ArgumentException ex) => { })                  // skip problem item
    .Catch((NotImplementedException ex) => { }, () => -3); // replace it with default one
string.Join(",", target); // "0,1,-3,4"
```

Note that the order of the ```Catch``` methods is important. If the type of the handled exception is wider than folowing onces, they won't be handled:

```cs
var handledTarget = target
    .Catch((ArgumentException ex) => { })  
    .Catch((Exception ex) => { })           // wider than folowing
    .Catch((NotImplementedException ex) => { /* will never be called */ }, () => -3);
string.Join(",", target); // "0,1,4"
```


Today's topic is about the standard implementations for the different ways to compare instances of a type. There are 7 interfaces in .NET that are used for this: `IEqualityComparer`, `IEqualityComparer<T>`, `IComparer`, `IComparer<T>`, `Comparable`, `IComparable<T>`, and `IEquatable<T>`. Notice the missing, non-generic IEquatable variant. This is because `object` already has those methods that can be overridden by any derived type. Let's now look at the standard implementations:

1. `IEqualityComparer`
  
```
public sealed class FooComparer : IEqualityComparer
{
  public bool Equals(object x, object y)
  {
    if (object.ReferenceEquals(x, y))
    {
      return true;
    }

    if (x == null || y == null)
    {
      return false;
    }
  
    // implementation specific logic should go here
    ...
  
    return EqualityComparer.Default.Equals(x, y);
  }
  
  public int GetHashCode(object obj)
  {
    if (obj == null)
    {
      throw new ArgumentNullException(nameof(obj));
    }
  
    // implementation specific logic should go here
    ...
  
    return EqualityComparer.Default.GetHashCode(x, y);
  }
}
```

There are a few notes to observe about this implementation. First, the use of `object.ReferenceEquals`. Normally, we could just write `if (x == y)`. However, because `x` is of type `object`, we do not know if `operator ==` has been overloaded for the underlying type of `x`. This might mean that `x == y` will perform many more operations than are necessary if `x` and `y` are the same instance. `object.ReferenceEquals` performs *exactly* this check. Second, note that we do not need to be concerned about `null` once the first two `if` statements are evaluated. If `x` and `y` are both `null`, `object.ReferenceEquals` will cause us to return `true`. This means that the second `if` statement does not need to check if they are both `null`, only if either one of them is `null`. If either is `null`, we know they must be different because we already know they aren't *both* `null`. Third, there is a possible exception that can be thrown in `GetHashCode`. Although it can be tempting to use a default value for the case where `obj` is `null` (often `0` is used), it is best to follow the contract specified by the interface and let the caller dictate whether to handle the `null` case or not. Using a default value ultimately skews the distribution of hash codes, which is not always ideal. Lastly, notice the use of `EqualityComparer.Default`. If our custom logic is unable to handle a particular type or type combination, we should delegate to the default .NET behavior. 
  
2. `IEqualityComparer<T>`

```
public sealed class FooComparer : IEqualityComparer<Foo>
{
  public bool Equals(Foo x, Foo y)
  {
    // omit this check if Foo is a value type
    if (object.ReferenceEquals(x, y))
    {
      return true;
    }

    // omit this check if Foo is a value type
    if (x == null || y == null)
    {
      return false;
    }
  
    // implementation specific logic should go here
    ...
  }
  
  public int GetHashCode(Foo obj)
  {
    // omit this check if Foo is a value type
    if (obj == null)
    {
      throw new ArgumentNullException(nameof(obj));
    }
  
    // implementation specific logic should go here
    ...
  }
}
```
  
The implementation of the generic interface is extremely close to the non-generic implementation. Notice that `object.ReferenceEquals` is still used. This is because, while we might know the `operator ==` implementation for `Foo` when the comparer is written, the implementer of `Foo` *could* change that overload in the future, and that would potentially break our logic. It is better in this case to be explicit about our intention (we are trying to check reference equality) and avoid being broken in the future. We are also able to remove the `null` checks for the generic implementation if `Foo` is a value type. Those checks might be necessary if `Foo` is later changed to a reference type, but such a change would be a breaking change and we cannot be reasonably expected to handle all possible future *breaking* changes. 
  
3. `IComparer`
  
```
public sealed class FooComparer : IComparer
{
  public int Compare(object x, object y)
  {
    if (object.ReferenceEquals(x, y))
    {
      return 0;
    }
  
    if (x == null)
    {
      return -1;
    }
  
    if (y == null)
    {
      return 1;
    }
  
    // implementation specific logic should go here
    ...
  
    return Comparer.Default.Compare(x, y);
  }
}
```

Probably the most striking note here is when `ArgumentException` is thrown. The [documentation](https://docs.microsoft.com/en-us/dotnet/api/system.collections.icomparer.compare?view=net-6.0#exceptions) is very specific about this, but is clearly not correct (there are many `IComparer` implementations that compare arguments that do not implement `IComparable` and throw no exceptions), or else this interface would not provide much value. Normally, when MSDN documentation makes note of an exception, it does so to specify that the exception *will* be thrown if the criteria are met. In this case, it indicates that the exception *may* be thrown if the criteria are met. Really, what this means is that your comparer can have custom logic to compare two objects that don't implement `IComparable`. However, if your comparer *cannot* handle such a case, the default logic should be to find one of the two objects that implement `IComparable` and use that implementation to compare them. In this situation, if neither object implements `IComparable`, then an `ArgumentException` should be thrown. Importantly, this is the exact beahvior of `Comparer.Default`, so we do not have to worry about implementing this logic for every `IComparer` implementation. 
  
Another interesting aspect of this implementation is that (per the documentation), `null` should always be considered "less" than a non-`null` value, so after we have confirmed with `object.ReferenceEquals` that *both* `x` and `y` are not `null`, then if *either* are `null`, that one is "less" than the other. 

4. `IComparer<T>`
  
```
public sealed class FooComparer : IComparer<Foo>
{
  public int Compare(Foo x, Foo y)
  {
    // omit this check if Foo is a value type
    if (object.ReferenceEquals(x, y))
    {
      return 0;
    }
  
    // omit this check if Foo is a value type
    if (x == null)
    {
      return -1;
    }
  
    // omit this check if Foo is a value type
    if (y == null)
    {
      return 1;
    }
  
    // implementation specific logic should go here
    ...
  }
}
```

The reasoning behind each part of this implementation is the same as the reasoning above for other interfaces. 
  
There is not much to say as a takeaway today. Following the above patterns will correctly implement the boilerplate for each interface while being forward-looking from a maintainability aspect. Next time, we will look at standard implementations for the other 3 interfaces. 

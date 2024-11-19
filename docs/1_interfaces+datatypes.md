## pattern overview

The general pattern is that the CSTs will be a discriminated union that corresponds directly to the [OData ABNF](https://docs.oasis-open.org/odata/odata/v4.01/cs01/abnf/odata-abnf-construction-rules.txt). The ASTs will be a discriminated union that corresponds to the same ABNF but with the string literals removed, as well as "overhead" from the CST like aliases. 

We should also stick to the naming conventions laid out in the [architecture](./architecture.md). So, we should have "parser"s to create CSTs from strings, "converter"s to move between CSTs and ASTs, "translator"s to move between different types of ASTs, "transcriber"s to create strings from CSTs, "serializer"s to go from user defined types to ASTs, and "deserializer"s to go from ASTs to user defined types. 

## odata request

An OData request has a verb, a URL, and a request body. Taking a bottom up approach, let's start with the URL and request body and build into an odata request type.

### odata uri

#### odata resource path

##### CST

```csharp
// this is the CST for 
// pulled from `odataRelativeUri` definition in https://docs.oasis-open.org/odata/odata/v4.01/cs01/abnf/odata-abnf-construction-rules.txt
public abstract class OdataRelativeUri
{
  private OdataRelativeUri()
  {
  }

  protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

  public abstract class Visitor<TResult, TContext>
  {
    public TResult Visit(OdataRelativeUri odataRelativeUri, TContext context)
    {
      return odataRelativeUri.Dispatch(this, context);
    }

    public abstract TResult Accept(Batch batch, TContext context);
  }

  public sealed class Batch : OdataRelativeUri
  {
    private Batch()
    {
    }

    public static Batch Instance { get; } = new Batch();

    protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
    {
      return visitor.Accept(this, context);
    }
  }
}
```

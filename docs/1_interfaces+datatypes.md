## pattern overview

The general pattern is that the CSTs will be a discriminated union that corresponds directly to the [OData ABNF](https://docs.oasis-open.org/odata/odata/v4.01/cs01/abnf/odata-abnf-construction-rules.txt). The ASTs will be a discriminated union that corresponds to the same ABNF but with the string literals removed, as well as "overhead" from the CST like aliases. 

We should also stick to the naming conventions laid out in the [architecture](./architecture.md). So, we should have "parser"s to create CSTs from strings, "converter"s to move between CSTs and ASTs, "translator"s to move between different types of ASTs, "transcriber"s to create strings from CSTs, "serializer"s to go from user defined types to ASTs, and "deserializer"s to go from ASTs to user defined types. 

## odata request

An OData request has a verb, a URL, and a request body. Taking a bottom up approach, let's start with the URL and request body and build into an odata request type.

### odata uri

#### odata resource path

##### CST

```csharp
// this is the AST for odata resource paths
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

    public abstract TResult Accept(BatchWithoutOptions batch, TContext context);
  }

  public sealed class BatchWithoutOptions : OdataRelativeUri
  {
    private BatchWithoutOptions()
    {
    }

    public static BatchWithoutOptions Instance { get; } = new BatchWithoutOptions();

    protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
    {
      return visitor.Accept(this, context);
    }
  }

  public sealed class BatchWithOptions : OdataRelativeUri
  {
    public BatchWithOptions(BatchOptions batchOptions)
    {
      this.BatchOptions = batchOptions;
    }

    public BatchOptions BatchOptions { get; }

    protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
    {
      return visitor.Accept(this, context);
    }
  }

  public sealed class EntityWithOptions : OdataRelativeUri
  {
    public EntityWithOptions(EntityOptions entityOptions)
    {
      this.EntityOptions = entityOptions;
    }

    public EntityOptions EntityOptions { get; }

    protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
    {
      return visitor.Accept(this, context);
    }
  }

  public sealed class EntityWithCast : OdataRelativeUri
  {
    public EntityWithCast(QualifiedEntityTypeName qualifiedEntityTypeName, EntityCastOptions entityCastOptions)
    {
      this.QualifiedEntityTypeName = qualifiedEntityTypeName;
      this.EntityCastOptions = entityCastOptions;
    }

    public QualifiedEntityTypeName QualifiedEntityTypeName { get; }
    public EntityCastOptions EntityCastOptions { get; }

    protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
    {
      return visitor.Accept(this, context);
    }
  }
}

// TODO this is just a stub for now
public abstract class BatchOptions
{
  private BatchOptions()
  {
  }

  protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

  public abstract class Visitor<TResult, TContext>
  {
    public TResult Visit(BatchOptions batchOptions, TContext context)
    {
      return batchOptions.Dispatch(this, context);
    }
  }
}

// TODO this is just a stub for now
public abstract class EntityOptions
{
  private EntityOptions()
  {
  }

  protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

  public abstract class Visitor<TResult, TContext>
  {
    public TResult Visit(EntityOptions entityOptions, TContext context)
    {
      return entityOptions.Dispatch(this, context);
    }
  }
}

// TODO this is just a stub for now
public abstract class QualifiedEntityTypeName
{
  private QualifiedEntityTypeName()
  {
  }

  protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

  public abstract class Visitor<TResult, TContext>
  {
    public TResult Visit(QualifiedEntityTypeName qualifiedEntityTypeName, TContext context)
    {
      return qualifiedEntityTypeName.Dispatch(this, context);
    }
  }
}

// TODO this is just a stub for now
public abstract class EntityCastOptions
{
  private EntityCastOptions()
  {
  }

  protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

  public abstract class Visitor<TResult, TContext>
  {
    public TResult Visit(EntityCastOptions entityCastOptions, TContext context)
    {
      return entityCastOptions.Dispatch(this, context);
    }
  }
}
```

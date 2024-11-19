## pattern overview

The general pattern is that the CSTs will be a discriminated union that corresponds directly to the [OData ABNF](https://docs.oasis-open.org/odata/odata/v4.01/cs01/abnf/odata-abnf-construction-rules.txt). The ASTs will be a discriminated union that corresponds to the same ABNF but with the string literals removed, as well as "overhead" from the CST like aliases. 

We should also stick to the naming conventions laid out in the [architecture](./architecture.md). So, we should have "parser"s to create CSTs from strings, "converter"s to move between CSTs and ASTs, "translator"s to move between different types of ASTs, "transcriber"s to create strings from CSTs, "serializer"s to go from user defined types to ASTs, and "deserializer"s to go from ASTs to user defined types. 

## odata request

An OData request has a verb, a URL, and a request body. Taking a bottom up approach, let's start with the URL and request body and build into an odata request type.

### odata uri

#### odata resource path

##### CST

```csharp
public abstract class OdataRelativeUri
{
  private OdataRelativeUri
  {
  }

  protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

  public abstract class Visitor<TResult, TContext>
  {
    public TResult Visit(OdataRelativeUri node, TContext context)
    {
      return odataRelativeUri.Dispatch(this, context);
    }
  }

  public sealed class BatchOnly : OdataRelativeUri
  {
    private BatchOnly()
    {
      this.Batch = Batch.Instance;
      this.QuestionMark = QuestionMark.Intance { get; }
    }

    public Batch Batch { get; }
    public QuestionMark QuestionMark { get; }

    public static BatchOnly Instance { get; } = new BatchOnly();
  }

  public sealed class BatchWithOptions : OdataRelativeUri
  {
    public BatchWithOptions(BatchOptions batchOptions)
    {
      this.Batch = Batch.Instance;
      this.QuestionMark = QuestionMark.Instance;
      this.BatchOptions = batchOptions;
    }

    public Batch Batch { get; }
    public QuestionMark QuestionMark { get; }
    public BatchOptions BatchOptions { get; }
  }
}

public sealed class Batch
{
  private Batch()
  {
  }

  public static Batch Intance { get; } = new Batch();
}

public sealed class QuestionMark
{
  private QuestionMark()
  {
  }

  public static QuestionMark Instance { get; } = new QuestionMark();
}

public sealed
```

##### AST

```csharp
namespace Root.OdataRelativeUri.Ast
{
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
      public TResult Visit(OdataRelativeUri node, TContext context)
      {
        return odataRelativeUri.Dispatch(this, context);
      }
  
      public abstract TResult Accept(BatchOnly node, TContext context);
      public abstract TResult Accept(BatchWithOptions node, TContext context);
      public abstract TResult Accept(EntityWithOptions node, TContext context);
      public abstract TResult Accept(EntityWithCast node, TContext context);
      public abstract TResult Accept(MetadataOnly node, TContext context);
      public abstract TResult Accept(MetadataWithOptions node, TContext context);
      public abstract TResult Accept(MetadataWithContext node, TContext context);
      public abstract TResult Accept(MetadataWithOptionsAndContext node, TContext context);
      public abstract TResult Accept(ResourcePathOnly node, TContext context);
      public abstract TResult Accept(ResourcePathWithQueryOptions node, TContext context);
    }
  
    public sealed class BatchOnly : OdataRelativeUri
    {
      private BatchOnly()
      {
      }
  
      public static BatchOnly Instance { get; } = new BatchOnly();
  
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
  
    public sealed class MetadataOnly : OdataRelativeUri
    {
      private MetadataOnly()
      {
      }
  
      public static MetadataOnly Instance { get; } = new MetadataOnly();
  
      protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
      {
        return visitor.Accept(this, context);
      }
    }
  
    public sealed class MetadataWithOptions : OdataRelativeUri
    {
      public MetadataWithOptions(MetadataOptions metadataOptions)
      {
        this.MetadataOptions = metadataOptions;
      }
  
      public MetadataOptions MetadataOptions { get; }
  
      protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
      {
        return visitor.Accept(this, context);
      }
    }
  
    public sealed class MetadataWithContext : OdataRelativeUri
    {
      public MetadataWithContext(Context context)
      {
        this.Context = context;
      }
  
      public Context Context { get; }
  
      protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
      {
        return visitor.Accept(this, context);
      }
    }
  
    public sealed class MetadataWithOptionsAndContext : OdataRelativeUri
    {
      public MetadataWithOptionsAndContext(MetadataOptions metadataOptions, Context context)
      {
        this.MetadataOptions = metadataOptions;
        this.context = context;
      }
  
      public MetadataOptions MetadataOptions { get; }
      public Context Context { get; }
  
      protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
      {
        return visitor.Accept(this, context);
      }
    }
  
    public sealed class ResourcePathOnly : OdataRelativeUri
    {
      public ResourcePathOnly(ResourcePath resourcePath)
      {
        this.ResourcePath = resourcePath;
      }
  
      public ResourcePath ResourcePath { get; }
  
      protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
      {
        return visitor.Accept(this, context);
      }
    }
  
    public sealed class ResourcePathWithQueryOptions : OdataRelativeUri
    {
      public ResourcePathWithQueryOptions(QueryOptions queryOptions)
      {
        this.QueryOptions = queryOptions;
      }
  
      public QueryOptions QueryOptions { get; }
  
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
      public TResult Visit(BatchOptions node, TContext context)
      {
        return node.Dispatch(this, context);
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
      public TResult Visit(EntityOptions node, TContext context)
      {
        return node.Dispatch(this, context);
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
      public TResult Visit(QualifiedEntityTypeName node, TContext context)
      {
        return node.Dispatch(this, context);
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
      public TResult Visit(EntityCastOptions node, TContext context)
      {
        return node.Dispatch(this, context);
      }
    }
  }
  
  // TODO this is just a stub for now
  public abstract class MetadataOptions
  {
    private MetadataOptions()
    {
    }
  
    protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
  
    public abstract class Visitor<TResult, TContext>
    {
      public TResult Visit(MetadataOptions node, TContext context)
      {
        return node.Dispatch(this, context);
      }
    }
  }
  
  // TODO this is just a stub for now
  public abstract class Context
  {
    private Context()
    {
    }
  
    protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
  
    public abstract class Visitor<TResult, TContext>
    {
      public TResult Visit(Context node, TContext context)
      {
        return node.Dispatch(this, context);
      }
    }
  }
  
  // TODO this is just a stub for now
  public abstract class ResourcePath
  {
    private ResourcePath()
    {
    }
  
    protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
  
    public abstract class Visitor<TResult, TContext>
    {
      public TResult Visit(ResourcePath node, TContext context)
      {
        return node.Dispatch(this, context);
      }
    }
  }
  
  // TODO this is just a stub for now
  public abstract class QueryOptions
  {
    private QueryOptions()
    {
    }
  
    protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
  
    public abstract class Visitor<TResult, TContext>
    {
      public TResult Visit(QueryOptions node, TContext context)
      {
        return node.Dispatch(this, context);
      }
    }
  }
}
```

TODO put the code in a repo

#### CST transcriber

TODO

#### CST to AST translator

TODO

#### AST to CST translator

TODO

#### uri parser

TODO

namespace Root.OdataResourcePath.ConcreteSyntaxTree
{
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
                return node.Dispatch(this, context);
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
                this.Batch = Batch.Instance;
                this.QuestionMark = QuestionMark.Instance;
            }

            public Batch Batch { get; }
            public QuestionMark QuestionMark { get; }

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
                this.Batch = Batch.Instance;
                this.QuestionMark = QuestionMark.Instance;
                this.BatchOptions = batchOptions;
            }

            public Batch Batch { get; }
            public QuestionMark QuestionMark { get; }
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
                this.Entity = Entity.Instance;
                this.QuestionMark = QuestionMark.Instance;
                this.EntityOptions = entityOptions;
            }

            public Entity Entity { get; }
            public QuestionMark QuestionMark { get; }
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
                this.Entity = Entity.Instance;
                this.Slash = Slash.Instance;
                this.QualifiedEntityTypeName = qualifiedEntityTypeName;
                this.QuestionMark = QuestionMark.Instance;
                this.EntityCastOptions = entityCastOptions;
            }

            public Entity Entity { get; }
            public Slash Slash { get; }
            public QualifiedEntityTypeName QualifiedEntityTypeName { get; }
            public QuestionMark QuestionMark { get; }
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
                this.Metadata = Metadata.Instance;
            }

            public Metadata Metadata { get; }

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
                this.Metadata = Metadata.Instance;
                this.QuestionMark = QuestionMark.Instance;
                this.MetadataOptions = metadataOptions;
            }

            public Metadata Metadata { get; }
            public QuestionMark QuestionMark { get; }
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
                this.Metadata = Metadata.Instance;
                this.Context = context;
            }

            public Metadata Metadata { get; }
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
                this.Metadata = Metadata.Instance;
                this.QuestionMark = QuestionMark.Instance;
                this.MetadataOptions = metadataOptions;
                this.Context = context;
            }

            public Metadata Metadata { get; }
            public QuestionMark QuestionMark { get; }
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
            public ResourcePathWithQueryOptions(ResourcePath resourcePath, QueryOptions queryOptions)
            {
                this.ResourcePath = resourcePath;
                this.QuestionMark = QuestionMark.Instance;
                this.QueryOptions = queryOptions;
            }

            public ResourcePath ResourcePath { get; }
            public QuestionMark QuestionMark { get; }
            public QueryOptions QueryOptions { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }

    public sealed class Batch
    {
        private Batch()
        {
        }

        public static Batch Instance { get; } = new Batch();
    }

    public sealed class QuestionMark
    {
        private QuestionMark()
        {
        }

        public static QuestionMark Instance { get; } = new QuestionMark();
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

    public sealed class Entity
    {
        private Entity()
        {
        }

        public static Entity Instance { get; }
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

    public sealed class Slash
    {
        private Slash()
        {
        }

        public static Slash Instance { get; } = new Slash();
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

    public sealed class Metadata
    {
        private Metadata()
        {
        }

        public static Metadata Instance { get; } = new Metadata();
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

namespace Root.OdataResourcePath.CstToAstConverters
{
    using Root;

    public sealed class OdataRelativeUriConverter :
        ConcreteSyntaxTreeNodes.OdataRelativeUri.Visitor<
            AbstractSyntaxTreeNodes.OdataRelativeUri,
            Void>
    {
        private readonly BatchOptionsConverter batchOptionsCstToAstTranslator;
        private readonly EntityOptionsConverter entityOptionsTranslator;
        private readonly QualifiedEntityTypeNameConverter qualifiedEntityTypeNameTranslator;
        private readonly EntityCastOptionsConverter entityCastOptionsTranslator;
        private readonly MetadataOptionsConverter metadataOptionsTranslator;
        private readonly ContextConverter contextTranslator;
        private readonly ResourcePathConverter resourcePathTranslator;
        private readonly QueryOptionsConverter queryOptionsTranslator;

        private OdataRelativeUriConverter(
            BatchOptionsConverter batchOptionsCstToAstTranslator,
            EntityOptionsConverter entityOptionsTranslator,
            QualifiedEntityTypeNameConverter qualifiedEntityTypeNameTranslator,
            EntityCastOptionsConverter entityCastOptionsTranslator,
            MetadataOptionsConverter metadataOptionsTranslator,
            ContextConverter contextTranslator,
            ResourcePathConverter resourcePathTranslator,
            QueryOptionsConverter queryOptionsTranslator)
        {
            this.batchOptionsCstToAstTranslator = batchOptionsCstToAstTranslator;
            this.entityOptionsTranslator = entityOptionsTranslator;
            this.qualifiedEntityTypeNameTranslator = qualifiedEntityTypeNameTranslator;
            this.entityCastOptionsTranslator = entityCastOptionsTranslator;
            this.metadataOptionsTranslator = metadataOptionsTranslator;
            this.contextTranslator = contextTranslator;
            this.resourcePathTranslator = resourcePathTranslator;
            this.queryOptionsTranslator = queryOptionsTranslator;
        }

        public static OdataRelativeUriConverter Default { get; } = new OdataRelativeUriConverter(
            BatchOptionsConverter.Default,
            EntityOptionsConverter.Default,
            QualifiedEntityTypeNameConverter.Default,
            EntityCastOptionsConverter.Default,
            MetadataOptionsConverter.Default,
            ContextConverter.Default,
            ResourcePathConverter.Default,
            QueryOptionsConverter.Default);

        public override AbstractSyntaxTreeNodes.OdataRelativeUri Accept(
            ConcreteSyntaxTreeNodes.OdataRelativeUri.BatchOnly node,
            Void context)
        {
            return AbstractSyntaxTreeNodes.OdataRelativeUri.BatchOnly.Instance;
        }

        public override AbstractSyntaxTreeNodes.OdataRelativeUri Accept(
            ConcreteSyntaxTreeNodes.OdataRelativeUri.BatchWithOptions node,
            Void context)
        {
            var batchOptions = this.batchOptionsCstToAstTranslator.Visit(node.BatchOptions, context);
            return new AbstractSyntaxTreeNodes.OdataRelativeUri.BatchWithOptions(batchOptions);
        }

        public override AbstractSyntaxTreeNodes.OdataRelativeUri Accept(
            ConcreteSyntaxTreeNodes.OdataRelativeUri.EntityWithOptions node,
            Void context)
        {
            var entityOptions = this.entityOptionsTranslator.Visit(node.EntityOptions, context);
            return new AbstractSyntaxTreeNodes.OdataRelativeUri.EntityWithOptions(entityOptions);
        }

        public override AbstractSyntaxTreeNodes.OdataRelativeUri Accept(
            ConcreteSyntaxTreeNodes.OdataRelativeUri.EntityWithCast node,
            Void context)
        {
            var qualifiedEntityTypeName = this.qualifiedEntityTypeNameTranslator.Visit(node.QualifiedEntityTypeName, context);
            var entityCastOptions = this.entityCastOptionsTranslator.Visit(node.EntityCastOptions, context);
            return new AbstractSyntaxTreeNodes.OdataRelativeUri.EntityWithCast(qualifiedEntityTypeName, entityCastOptions);
        }

        public override AbstractSyntaxTreeNodes.OdataRelativeUri Accept(
            ConcreteSyntaxTreeNodes.OdataRelativeUri.MetadataOnly node,
            Void context)
        {
            return AbstractSyntaxTreeNodes.OdataRelativeUri.MetadataOnly.Instance;
        }

        public override AbstractSyntaxTreeNodes.OdataRelativeUri Accept(
            ConcreteSyntaxTreeNodes.OdataRelativeUri.MetadataWithOptions node,
            Void context)
        {
            var metadataOptions = this.metadataOptionsTranslator.Visit(node.MetadataOptions, context);
            return new AbstractSyntaxTreeNodes.OdataRelativeUri.MetadataWithOptions(metadataOptions);
        }

        public override AbstractSyntaxTreeNodes.OdataRelativeUri Accept(
            ConcreteSyntaxTreeNodes.OdataRelativeUri.MetadataWithContext node,
            Void context)
        {
            var metadataContext = this.contextTranslator.Visit(node.Context, context);
            return new AbstractSyntaxTreeNodes.OdataRelativeUri.MetadataWithContext(metadataContext);
        }

        public override AbstractSyntaxTreeNodes.OdataRelativeUri Accept(
            ConcreteSyntaxTreeNodes.OdataRelativeUri.MetadataWithOptionsAndContext node,
            Void context)
        {
            var metadataOptions = this.metadataOptionsTranslator.Visit(node.MetadataOptions, context);
            var metadataContext = this.contextTranslator.Visit(node.Context, context);
            return new AbstractSyntaxTreeNodes.OdataRelativeUri.MetadataWithOptionsAndContext(metadataOptions, metadataContext);
        }

        public override AbstractSyntaxTreeNodes.OdataRelativeUri Accept(
            ConcreteSyntaxTreeNodes.OdataRelativeUri.ResourcePathOnly node,
            Void context)
        {
            var resourcePath = this.resourcePathTranslator.Visit(node.ResourcePath, context);
            return new AbstractSyntaxTreeNodes.OdataRelativeUri.ResourcePathOnly(resourcePath);
        }

        public override AbstractSyntaxTreeNodes.OdataRelativeUri Accept(
            ConcreteSyntaxTreeNodes.OdataRelativeUri.ResourcePathWithQueryOptions node,
            Void context)
        {
            var resourcePath = this.resourcePathTranslator.Visit(node.ResourcePath, context);
            var queryOptions = this.queryOptionsTranslator.Visit(node.QueryOptions, context);
            return new AbstractSyntaxTreeNodes.OdataRelativeUri.ResourcePathWithQueryOptions(resourcePath, queryOptions);
        }
    }
}

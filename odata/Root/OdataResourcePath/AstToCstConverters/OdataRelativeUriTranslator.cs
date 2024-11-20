namespace Root.OdataResourcePath.AstToCstConverters
{
    using Root;

    public sealed class OdataRelativeUriTranslator :
        AbstractSyntaxTreeNodes.OdataRelativeUri.Visitor<
            ConcreteSyntaxTreeNodes.OdataRelativeUri,
            Void>
    {
        private readonly BatchOptionsTranslator batchOptionsCstToAstTranslator;
        private readonly EntityOptionsTranslator entityOptionsTranslator;
        private readonly QualifiedEntityTypeNameTranslator qualifiedEntityTypeNameTranslator;
        private readonly EntityCastOptionsTranslator entityCastOptionsTranslator;
        private readonly MetadataOptionsTranslator metadataOptionsTranslator;
        private readonly ContextTranslator contextTranslator;
        private readonly ResourcePathTranslator resourcePathTranslator;
        private readonly QueryOptionsTranslator queryOptionsTranslator;

        private OdataRelativeUriTranslator(
            BatchOptionsTranslator batchOptionsCstToAstTranslator,
            EntityOptionsTranslator entityOptionsTranslator,
            QualifiedEntityTypeNameTranslator qualifiedEntityTypeNameTranslator,
            EntityCastOptionsTranslator entityCastOptionsTranslator,
            MetadataOptionsTranslator metadataOptionsTranslator,
            ContextTranslator contextTranslator,
            ResourcePathTranslator resourcePathTranslator,
            QueryOptionsTranslator queryOptionsTranslator)
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

        public static OdataRelativeUriTranslator Default { get; } = new OdataRelativeUriTranslator(
            BatchOptionsTranslator.Default,
            EntityOptionsTranslator.Default,
            QualifiedEntityTypeNameTranslator.Default,
            EntityCastOptionsTranslator.Default,
            MetadataOptionsTranslator.Default,
            ContextTranslator.Default,
            ResourcePathTranslator.Default,
            QueryOptionsTranslator.Default);

        public override ConcreteSyntaxTreeNodes.OdataRelativeUri Accept(
            AbstractSyntaxTreeNodes.OdataRelativeUri.BatchOnly node,
            Void context)
        {
            return ConcreteSyntaxTreeNodes.OdataRelativeUri.BatchOnly.Instance;
        }

        public override ConcreteSyntaxTreeNodes.OdataRelativeUri Accept(
            AbstractSyntaxTreeNodes.OdataRelativeUri.BatchWithOptions node,
            Void context)
        {
            var batchOptions = this.batchOptionsCstToAstTranslator.Visit(node.BatchOptions, context);
            return new ConcreteSyntaxTreeNodes.OdataRelativeUri.BatchWithOptions(batchOptions);
        }

        public override ConcreteSyntaxTreeNodes.OdataRelativeUri Accept(
            AbstractSyntaxTreeNodes.OdataRelativeUri.EntityWithOptions node,
            Void context)
        {
            var entityOptions = this.entityOptionsTranslator.Visit(node.EntityOptions, context);
            return new ConcreteSyntaxTreeNodes.OdataRelativeUri.EntityWithOptions(entityOptions);
        }

        public override ConcreteSyntaxTreeNodes.OdataRelativeUri Accept(
            AbstractSyntaxTreeNodes.OdataRelativeUri.EntityWithCast node,
            Void context)
        {
            var qualifiedEntityTypeName = this.qualifiedEntityTypeNameTranslator.Visit(node.QualifiedEntityTypeName, context);
            var entityCastOptions = this.entityCastOptionsTranslator.Visit(node.EntityCastOptions, context);
            return new ConcreteSyntaxTreeNodes.OdataRelativeUri.EntityWithCast(qualifiedEntityTypeName, entityCastOptions);
        }

        public override ConcreteSyntaxTreeNodes.OdataRelativeUri Accept(
            AbstractSyntaxTreeNodes.OdataRelativeUri.MetadataOnly node,
            Void context)
        {
            return ConcreteSyntaxTreeNodes.OdataRelativeUri.MetadataOnly.Instance;
        }

        public override ConcreteSyntaxTreeNodes.OdataRelativeUri Accept(
            AbstractSyntaxTreeNodes.OdataRelativeUri.MetadataWithOptions node,
            Void context)
        {
            var metadataOptions = this.metadataOptionsTranslator.Visit(node.MetadataOptions, context);
            return new ConcreteSyntaxTreeNodes.OdataRelativeUri.MetadataWithOptions(metadataOptions);
        }

        public override ConcreteSyntaxTreeNodes.OdataRelativeUri Accept(
            AbstractSyntaxTreeNodes.OdataRelativeUri.MetadataWithContext node,
            Void context)
        {
            var metadataContext = this.contextTranslator.Visit(node.Context, context);
            return new ConcreteSyntaxTreeNodes.OdataRelativeUri.MetadataWithContext(metadataContext);
        }

        public override ConcreteSyntaxTreeNodes.OdataRelativeUri Accept(
            AbstractSyntaxTreeNodes.OdataRelativeUri.MetadataWithOptionsAndContext node,
            Void context)
        {
            var metadataOptions = this.metadataOptionsTranslator.Visit(node.MetadataOptions, context);
            var metadataContext = this.contextTranslator.Visit(node.Context, context);
            return new ConcreteSyntaxTreeNodes.OdataRelativeUri.MetadataWithOptionsAndContext(metadataOptions, metadataContext);
        }

        public override ConcreteSyntaxTreeNodes.OdataRelativeUri Accept(
            AbstractSyntaxTreeNodes.OdataRelativeUri.ResourcePathOnly node,
            Void context)
        {
            var resourcePath = this.resourcePathTranslator.Visit(node.ResourcePath, context);
            return new ConcreteSyntaxTreeNodes.OdataRelativeUri.ResourcePathOnly(resourcePath);
        }

        public override ConcreteSyntaxTreeNodes.OdataRelativeUri Accept(
            AbstractSyntaxTreeNodes.OdataRelativeUri.ResourcePathWithQueryOptions node,
            Void context)
        {
            var resourcePath = this.resourcePathTranslator.Visit(node.ResourcePath, context);
            var queryOptions = this.queryOptionsTranslator.Visit(node.QueryOptions, context);
            return new ConcreteSyntaxTreeNodes.OdataRelativeUri.ResourcePathWithQueryOptions(resourcePath, queryOptions);
        }
    }
}

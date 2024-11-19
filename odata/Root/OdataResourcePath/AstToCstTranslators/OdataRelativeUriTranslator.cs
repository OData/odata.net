namespace Root.OdataResourcePath.AstToCstTranslators
{
    using Root;

    public sealed class OdataRelativeUriTranslator :
        AbstractSyntaxTree.OdataRelativeUri.Visitor<
            ConcreteSyntaxTree.OdataRelativeUri,
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

        public override ConcreteSyntaxTree.OdataRelativeUri Accept(
            AbstractSyntaxTree.OdataRelativeUri.BatchOnly node,
            Void context)
        {
            return ConcreteSyntaxTree.OdataRelativeUri.BatchOnly.Instance;
        }

        public override ConcreteSyntaxTree.OdataRelativeUri Accept(
            AbstractSyntaxTree.OdataRelativeUri.BatchWithOptions node,
            Void context)
        {
            var batchOptions = this.batchOptionsCstToAstTranslator.Visit(node.BatchOptions, context);
            return new ConcreteSyntaxTree.OdataRelativeUri.BatchWithOptions(batchOptions);
        }

        public override ConcreteSyntaxTree.OdataRelativeUri Accept(
            AbstractSyntaxTree.OdataRelativeUri.EntityWithOptions node,
            Void context)
        {
            var entityOptions = this.entityOptionsTranslator.Visit(node.EntityOptions, context);
            return new ConcreteSyntaxTree.OdataRelativeUri.EntityWithOptions(entityOptions);
        }

        public override ConcreteSyntaxTree.OdataRelativeUri Accept(
            AbstractSyntaxTree.OdataRelativeUri.EntityWithCast node,
            Void context)
        {
            var qualifiedEntityTypeName = this.qualifiedEntityTypeNameTranslator.Visit(node.QualifiedEntityTypeName, context);
            var entityCastOptions = this.entityCastOptionsTranslator.Visit(node.EntityCastOptions, context);
            return new ConcreteSyntaxTree.OdataRelativeUri.EntityWithCast(qualifiedEntityTypeName, entityCastOptions);
        }

        public override ConcreteSyntaxTree.OdataRelativeUri Accept(
            AbstractSyntaxTree.OdataRelativeUri.MetadataOnly node,
            Void context)
        {
            return ConcreteSyntaxTree.OdataRelativeUri.MetadataOnly.Instance;
        }

        public override ConcreteSyntaxTree.OdataRelativeUri Accept(
            AbstractSyntaxTree.OdataRelativeUri.MetadataWithOptions node,
            Void context)
        {
            var metadataOptions = this.metadataOptionsTranslator.Visit(node.MetadataOptions, context);
            return new ConcreteSyntaxTree.OdataRelativeUri.MetadataWithOptions(metadataOptions);
        }

        public override ConcreteSyntaxTree.OdataRelativeUri Accept(
            AbstractSyntaxTree.OdataRelativeUri.MetadataWithContext node,
            Void context)
        {
            var metadataContext = this.contextTranslator.Visit(node.Context, context);
            return new ConcreteSyntaxTree.OdataRelativeUri.MetadataWithContext(metadataContext);
        }

        public override ConcreteSyntaxTree.OdataRelativeUri Accept(
            AbstractSyntaxTree.OdataRelativeUri.MetadataWithOptionsAndContext node,
            Void context)
        {
            var metadataOptions = this.metadataOptionsTranslator.Visit(node.MetadataOptions, context);
            var metadataContext = this.contextTranslator.Visit(node.Context, context);
            return new ConcreteSyntaxTree.OdataRelativeUri.MetadataWithOptionsAndContext(metadataOptions, metadataContext);
        }

        public override ConcreteSyntaxTree.OdataRelativeUri Accept(
            AbstractSyntaxTree.OdataRelativeUri.ResourcePathOnly node,
            Void context)
        {
            var resourcePath = this.resourcePathTranslator.Visit(node.ResourcePath, context);
            return new ConcreteSyntaxTree.OdataRelativeUri.ResourcePathOnly(resourcePath);
        }

        public override ConcreteSyntaxTree.OdataRelativeUri Accept(
            AbstractSyntaxTree.OdataRelativeUri.ResourcePathWithQueryOptions node,
            Void context)
        {
            var resourcePath = this.resourcePathTranslator.Visit(node.ResourcePath, context);
            var queryOptions = this.queryOptionsTranslator.Visit(node.QueryOptions, context);
            return new ConcreteSyntaxTree.OdataRelativeUri.ResourcePathWithQueryOptions(resourcePath, queryOptions);
        }
    }
}

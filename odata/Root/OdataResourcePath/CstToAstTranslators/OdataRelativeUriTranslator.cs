namespace Root.OdataResourcePath.CstToAstTranslators
{
    using System;

    public sealed class OdataRelativeUriTranslator :
        ConcreteSyntaxTree.OdataRelativeUri.Visitor<
            AbstractSyntaxTree.OdataRelativeUri,
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

        public override AbstractSyntaxTree.OdataRelativeUri Accept(
            ConcreteSyntaxTree.OdataRelativeUri.BatchOnly node,
            Void context)
        {
            return AbstractSyntaxTree.OdataRelativeUri.BatchOnly.Instance;
        }

        public override AbstractSyntaxTree.OdataRelativeUri Accept(
            ConcreteSyntaxTree.OdataRelativeUri.BatchWithOptions node,
            Void context)
        {
            var batchOptions = this.batchOptionsCstToAstTranslator.Visit(node.BatchOptions, context);
            return new AbstractSyntaxTree.OdataRelativeUri.BatchWithOptions(batchOptions);
        }

        public override AbstractSyntaxTree.OdataRelativeUri Accept(
            ConcreteSyntaxTree.OdataRelativeUri.EntityWithOptions node,
            Void context)
        {
            var entityOptions = this.entityOptionsTranslator.Visit(node.EntityOptions, context);
            return new AbstractSyntaxTree.OdataRelativeUri.EntityWithOptions(entityOptions);
        }

        public override AbstractSyntaxTree.OdataRelativeUri Accept(
            ConcreteSyntaxTree.OdataRelativeUri.EntityWithCast node,
            Void context)
        {
            var qualifiedEntityTypeName = this.qualifiedEntityTypeNameTranslator.Visit(node.QualifiedEntityTypeName, context);
            var entityCastOptions = this.entityCastOptionsTranslator.Visit(node.EntityCastOptions, context);
            return new AbstractSyntaxTree.OdataRelativeUri.EntityWithCast(qualifiedEntityTypeName, entityCastOptions);
        }

        public override AbstractSyntaxTree.OdataRelativeUri Accept(
            ConcreteSyntaxTree.OdataRelativeUri.MetadataOnly node,
            Void context)
        {
            return AbstractSyntaxTree.OdataRelativeUri.MetadataOnly.Instance;
        }

        public override AbstractSyntaxTree.OdataRelativeUri Accept(
            ConcreteSyntaxTree.OdataRelativeUri.MetadataWithOptions node,
            Void context)
        {
            var metadataOptions = this.metadataOptionsTranslator.Visit(node.MetadataOptions, context);
            return new AbstractSyntaxTree.OdataRelativeUri.MetadataWithOptions(metadataOptions);
        }

        public override AbstractSyntaxTree.OdataRelativeUri Accept(
            ConcreteSyntaxTree.OdataRelativeUri.MetadataWithContext node,
            Void context)
        {
            var metadataContext = this.contextTranslator.Visit(node.Context, context);
            return new AbstractSyntaxTree.OdataRelativeUri.MetadataWithContext(metadataContext);
        }

        public override AbstractSyntaxTree.OdataRelativeUri Accept(
            ConcreteSyntaxTree.OdataRelativeUri.MetadataWithOptionsAndContext node,
            Void context)
        {
            var metadataOptions = this.metadataOptionsTranslator.Visit(node.MetadataOptions, context);
            var metadataContext = this.contextTranslator.Visit(node.Context, context);
            return new AbstractSyntaxTree.OdataRelativeUri.MetadataWithOptionsAndContext(metadataOptions, metadataContext);
        }

        public override AbstractSyntaxTree.OdataRelativeUri Accept(
            ConcreteSyntaxTree.OdataRelativeUri.ResourcePathOnly node,
            Void context)
        {
            var resourcePath = this.resourcePathTranslator.Visit(node.ResourcePath, context);
            return new AbstractSyntaxTree.OdataRelativeUri.ResourcePathOnly(resourcePath);
        }

        public override AbstractSyntaxTree.OdataRelativeUri Accept(
            ConcreteSyntaxTree.OdataRelativeUri.ResourcePathWithQueryOptions node,
            Void context)
        {
            var resourcePath = this.resourcePathTranslator.Visit(node.ResourcePath, context);
            var queryOptions = this.queryOptionsTranslator.Visit(node.QueryOptions, context);
            return new AbstractSyntaxTree.OdataRelativeUri.ResourcePathWithQueryOptions(resourcePath, queryOptions);
        }
    }
}

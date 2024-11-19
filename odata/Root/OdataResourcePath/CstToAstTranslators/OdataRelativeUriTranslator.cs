namespace Root.OdataResourcePath.CstToAstTranslators
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class OdataRelativeUriTranslator :
        ConcreteSyntaxTree.OdataRelativeUri.Visitor<
            AbstractSyntaxTree.OdataRelativeUri,
            Void>
    {
        private readonly BatchOptionsTranslator batchOptionsCstToAstTranslator;

        private readonly EntityOptionsTranslator entityOptionsTranslator;

        private OdataRelativeUriTranslator(BatchOptionsTranslator batchOptionsCstToAstTranslator)
        {
            this.batchOptionsCstToAstTranslator = batchOptionsCstToAstTranslator;
        }

        public static OdataRelativeUriTranslator Default { get; } = new OdataRelativeUriTranslator(
            BatchOptionsTranslator.Default);

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
            throw new NotImplementedException();
        }

        public override AbstractSyntaxTree.OdataRelativeUri Accept(
            ConcreteSyntaxTree.OdataRelativeUri.EntityWithCast node,
            Void context)
        {
            throw new NotImplementedException();
        }

        public override AbstractSyntaxTree.OdataRelativeUri Accept(
            ConcreteSyntaxTree.OdataRelativeUri.MetadataOnly node,
            Void context)
        {
            throw new NotImplementedException();
        }

        public override AbstractSyntaxTree.OdataRelativeUri Accept(
            ConcreteSyntaxTree.OdataRelativeUri.MetadataWithOptions node,
            Void context)
        {
            throw new NotImplementedException();
        }

        public override AbstractSyntaxTree.OdataRelativeUri Accept(
            ConcreteSyntaxTree.OdataRelativeUri.MetadataWithContext node,
            Void context)
        {
            throw new NotImplementedException();
        }

        public override AbstractSyntaxTree.OdataRelativeUri Accept(
            ConcreteSyntaxTree.OdataRelativeUri.MetadataWithOptionsAndContext node,
            Void context)
        {
            throw new NotImplementedException();
        }

        public override AbstractSyntaxTree.OdataRelativeUri Accept(
            ConcreteSyntaxTree.OdataRelativeUri.ResourcePathOnly node,
            Void context)
        {
            throw new NotImplementedException();
        }

        public override AbstractSyntaxTree.OdataRelativeUri Accept(
            ConcreteSyntaxTree.OdataRelativeUri.ResourcePathWithQueryOptions node,
            Void context)
        {
            throw new NotImplementedException();
        }
    }
}

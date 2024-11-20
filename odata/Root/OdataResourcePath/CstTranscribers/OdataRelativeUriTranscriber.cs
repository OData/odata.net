namespace Root.OdataResourcePath.CstTranscribers
{
    using System.Text;

    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;

    public sealed class OdataRelativeUriTranscriber : OdataRelativeUri.Visitor<Void, StringBuilder>
    {
        private readonly BatchTranscriber batchTranscriber;
        private readonly QuestionMarkTranscriber questionMarkTranscriber;
        private readonly BatchOptionsTranscriber batchOptionsTranscriber;
        private readonly EntityTranscriber entityTranscriber;
        private readonly EntityOptionsTranscriber entityOptionsTranscriber;
        private readonly SlashTranscriber slashTranscriber;
        private readonly QualifiedEntityTypeNameTranscriber qualifiedEntityTypeNameTranscriber;
        private readonly EntityCastOptionsTranscriber entityCastOptionsTranscriber;
        private readonly MetadataTranscriber metadataTranscriber;
        private readonly MetadataOptionsTranscriber metadataOptionsTranscriber;
        private readonly ContextTranscriber contextTranscriber;
        private readonly ResourcePathTranscriber resourcePathTranscriber;
        private readonly QueryOptionsTranscriber queryOptionsTranscriber;

        private OdataRelativeUriTranscriber(
            BatchTranscriber batchTranscriber,
            QuestionMarkTranscriber questionMarkTranscriber,
            BatchOptionsTranscriber batchOptionsTranscriber,
            EntityTranscriber entityTranscriber,
            EntityOptionsTranscriber entityOptionsTranscriber,
            SlashTranscriber slashTranscriber,
            QualifiedEntityTypeNameTranscriber qualifiedEntityTypeNameTranscriber,
            EntityCastOptionsTranscriber entityCastOptionsTranscriber,
            MetadataTranscriber metadataTranscriber,
            MetadataOptionsTranscriber metadataOptionsTranscriber,
            ContextTranscriber contextTranscriber,
            ResourcePathTranscriber resourcePathTranscriber,
            QueryOptionsTranscriber queryOptionsTranscriber)
        {
            this.batchTranscriber = batchTranscriber;
            this.questionMarkTranscriber = questionMarkTranscriber;
            this.batchOptionsTranscriber = batchOptionsTranscriber;
            this.entityTranscriber = entityTranscriber;
            this.entityOptionsTranscriber = entityOptionsTranscriber;
            this.slashTranscriber = slashTranscriber;
            this.qualifiedEntityTypeNameTranscriber = qualifiedEntityTypeNameTranscriber;
            this.entityCastOptionsTranscriber = entityCastOptionsTranscriber;
            this.metadataTranscriber = metadataTranscriber;
            this.metadataOptionsTranscriber = metadataOptionsTranscriber;
            this.contextTranscriber = contextTranscriber;
            this.resourcePathTranscriber = resourcePathTranscriber;
            this.queryOptionsTranscriber = queryOptionsTranscriber;
        }

        public static OdataRelativeUriTranscriber Default { get; } = new OdataRelativeUriTranscriber(
            BatchTranscriber.Default,
            QuestionMarkTranscriber.Default,
            BatchOptionsTranscriber.Default,
            EntityTranscriber.Default,
            EntityOptionsTranscriber.Default,
            SlashTranscriber.Default,
            QualifiedEntityTypeNameTranscriber.Default,
            EntityCastOptionsTranscriber.Default,
            MetadataTranscriber.Default,
            MetadataOptionsTranscriber.Default,
            ContextTranscriber.Default,
            ResourcePathTranscriber.Default,
            QueryOptionsTranscriber.Default);

        public override Void Accept(OdataRelativeUri.BatchOnly node, StringBuilder context)
        {
            this.batchTranscriber.Transcribe(node.Batch, context);
            return default;
        }

        public override Void Accept(OdataRelativeUri.BatchWithOptions node, StringBuilder context)
        {
            this.batchTranscriber.Transcribe(node.Batch, context);
            this.questionMarkTranscriber.Transcribe(node.QuestionMark, context);
            this.batchOptionsTranscriber.Visit(node.BatchOptions, context);
            return default;
        }

        public override Void Accept(OdataRelativeUri.EntityWithOptions node, StringBuilder context)
        {
            this.entityTranscriber.Transcribe(node.Entity, context);
            this.questionMarkTranscriber.Transcribe(node.QuestionMark, context);
            this.entityOptionsTranscriber.Visit(node.EntityOptions, context);
            return default;
        }

        public override Void Accept(OdataRelativeUri.EntityWithCast node, StringBuilder context)
        {
            this.entityTranscriber.Transcribe(node.Entity, context);
            this.slashTranscriber.Transcribe(node.Slash, context);
            this.qualifiedEntityTypeNameTranscriber.Visit(node.QualifiedEntityTypeName, context);
            this.questionMarkTranscriber.Transcribe(node.QuestionMark, context);
            this.entityCastOptionsTranscriber.Visit(node.EntityCastOptions, context);
            return default;
        }

        public override Void Accept(OdataRelativeUri.MetadataOnly node, StringBuilder context)
        {
            this.metadataTranscriber.Transcribe(node.Metadata, context);
            return default;
        }

        public override Void Accept(OdataRelativeUri.MetadataWithOptions node, StringBuilder context)
        {
            this.metadataTranscriber.Transcribe(node.Metadata, context);
            this.questionMarkTranscriber.Transcribe(node.QuestionMark, context);
            this.metadataOptionsTranscriber.Visit(node.MetadataOptions, context);
            return default;
        }

        public override Void Accept(OdataRelativeUri.MetadataWithContext node, StringBuilder context)
        {
            this.metadataTranscriber.Transcribe(node.Metadata, context);
            this.contextTranscriber.Visit(node.Context, context);
            return default;
        }

        public override Void Accept(OdataRelativeUri.MetadataWithOptionsAndContext node, StringBuilder context)
        {
            this.metadataTranscriber.Transcribe(node.Metadata, context);
            this.questionMarkTranscriber.Transcribe(node.QuestionMark, context);
            this.metadataOptionsTranscriber.Visit(node.MetadataOptions, context);
            this.contextTranscriber.Visit(node.Context, context);
            return default;
        }

        public override Void Accept(OdataRelativeUri.ResourcePathOnly node, StringBuilder context)
        {
            this.resourcePathTranscriber.Visit(node.ResourcePath, context);
            return default;
        }

        public override Void Accept(OdataRelativeUri.ResourcePathWithQueryOptions node, StringBuilder context)
        {
            this.resourcePathTranscriber.Visit(node.ResourcePath, context);
            this.questionMarkTranscriber.Transcribe(node.QuestionMark, context);
            this.queryOptionsTranscriber.Visit(node.QueryOptions, context);
            return default;
        }
    }
}

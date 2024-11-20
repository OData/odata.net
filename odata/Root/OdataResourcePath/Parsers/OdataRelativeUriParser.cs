namespace Root.OdataResourcePath.Parsers
{
    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;
    using Sprache;

    public static class OdataRelativeUriParser
    {
        public static Parser<OdataRelativeUri.BatchOnly> BatchOnly { get; } =
            from batch in BatchParser.Instance
            select OdataRelativeUri.BatchOnly.Instance;

        public static Parser<OdataRelativeUri.BatchWithOptions> BatchWithOptions { get; } =
            //// TODO can you avoid using this syntax?
            from batch in BatchParser.Instance
            from questionMark in QuestionMarkParser.Instance
            from batchOptions in BatchOptionsParser.Instance
            select new OdataRelativeUri.BatchWithOptions(batchOptions);

        public static Parser<OdataRelativeUri.EntityWithOptions> EntityWithOptions { get; } =
            from entity in EntityParser.Instance
            from questionMark in QuestionMarkParser.Instance
            from entityOptions in EntityOptionsParser.Instance
            select new OdataRelativeUri.EntityWithOptions(entityOptions);

        public static Parser<OdataRelativeUri.EntityWithCast> EntityWithCast { get; } =
            from entity in EntityParser.Instance
            from slash in SlashParser.Instance
            from qualifiedTypeName in QualifiedTypeNameParser.Instance
            from questionMark in QuestionMarkParser.Instance
            from entityCastOptions in EntityCastOptionsParser.Instance
            select new OdataRelativeUri.EntityWithCast(qualifiedTypeName, entityCastOptions);

        public static Parser<OdataRelativeUri.MetadataOnly> MetadataOnly { get; } =
            from metadata in MetadataParser.Instance
            select OdataRelativeUri.MetadataOnly.Instance;

        public static Parser<OdataRelativeUri.MetadataWithOptions> MetadataWithOptions { get; } =
            from metadata in MetadataParser.Instance
            from questionMark in QuestionMarkParser.Instance
            from metadataOptions in MetadataOptionsParser.Instance
            select new OdataRelativeUri.MetadataWithOptions(metadataOptions);

        public static Parser<OdataRelativeUri.MetadataWithContext> MetadataWithContext { get; } =
            from metadata in MetadataParser.Instance
            from context in ContextParser.Instance
            select new OdataRelativeUri.MetadataWithContext(context);

        public static Parser<OdataRelativeUri.MetadataWithOptionsAndContext> MetadataWithOptionsAndContext { get; } =
            from metadata in MetadataParser.Instance
            from questionMark in QuestionMarkParser.Instance
            from metadataOptions in MetadataOptionsParser.Instance
            from context in ContextParser.Instance
            select new OdataRelativeUri.MetadataWithOptionsAndContext(metadataOptions, context);

        public static Parser<OdataRelativeUri.ResourcePathOnly> ResourcePathOnly { get; } =
            from resourcePath in ResourcePathParser.Instance
            select new OdataRelativeUri.ResourcePathOnly(resourcePath);

        public static Parser<OdataRelativeUri.ResourcePathWithQueryOptions> ResourcePathWithQueryOptions { get; } =
            from resourcePath in ResourcePathParser.Instance
            from questionMark in QuestionMarkParser.Instance
            from queryOptions in QueryOptionsParser.Instance
            select new OdataRelativeUri.ResourcePathWithQueryOptions(resourcePath, queryOptions);

        public static Parser<OdataRelativeUri> Instance { get; } =
            BatchOnly
            .Or<OdataRelativeUri>(BatchWithOptions)
            .Or(EntityWithOptions)
            .Or(EntityWithCast)
            .Or(MetadataOnly)
            .Or(MetadataWithOptions)
            .Or(MetadataWithContext)
            .Or(MetadataWithOptionsAndContext)
            .Or(ResourcePathOnly)
            .Or(ResourcePathWithQueryOptions);
    }
}

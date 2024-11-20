namespace Root.OdataResourcePath.Parsers
{
    using Root.OdataResourcePath.ConcreteSyntaxTreeNodes;
    using Sprache;

    /// <summary>
    /// TODO figure out the naming and structuring...
    /// </summary>
    public static class OdataRelativeUriParser
    {
        public static Parser<OdataRelativeUri> Instance { get; } =
            //// TODO fix nullability; i think this is a sprache issue...
            ((Parser<OdataRelativeUri>)BatchOnly)
            .Or(BatchWithOptions)
            .Or(EntityWithOptions)
            .Or(EntityWithCast)
            .Or(MetadataOnly)
            .Or(MetadataWithOptions)
            .Or(MetadataWithContext)
            .Or(MetadataWithOptionsAndContext)
            .Or(ResourcePathOnly)
            .Or(ResourcePathWithQueryOptions);

        public static Parser<OdataRelativeUri.BatchOnly> BatchOnly { get; } = 
            BatchParser.Instance
            .Return(OdataRelativeUri.BatchOnly.Instance);

        public static Parser<OdataRelativeUri.BatchWithOptions> BatchWithOptions { get; } =
            //// TODO can you avoid using this syntax?
            from batch in BatchParser.Instance
            from batchOptions in BatchOptionsParser.Instance
            select new OdataRelativeUri.BatchWithOptions(batchOptions);

        public static Parser<OdataRelativeUri.EntityWithOptions> EntityWithOptions { get; } =
            from entity in EntityParser.Instance
            from entityOptions in 

        public static Parser<OdataRelativeUri.EntityWithCast> EntityWithCast { get; }
        
        public static Parser<OdataRelativeUri.MetadataOnly> MetadataOnly { get; }
        
        public static Parser<OdataRelativeUri.MetadataWithOptions> MetadataWithOptions { get; }
        
        public static Parser<OdataRelativeUri.MetadataWithContext> MetadataWithContext { get; }
        
        public static Parser<OdataRelativeUri.MetadataWithOptionsAndContext> MetadataWithOptionsAndContext { get; }

        public static Parser<OdataRelativeUri.ResourcePathOnly> ResourcePathOnly { get; }

        public static Parser<OdataRelativeUri.ResourcePathWithQueryOptions> ResourcePathWithQueryOptions { get; }
    }
}

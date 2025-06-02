namespace NewStuff._Design._0_Convention.Sample
{
    public sealed class GraphConvention : IConvention
    {
        private readonly IConvention convention;
        private readonly string authorizationToken;

        public GraphConvention(IConvention convention, string authorizationToken)
        {
            this.convention = convention;
            this.authorizationToken = authorizationToken;
        }

        public IGetRequestWriter Get()
        {
            return this.convention.Get().OverrideHeader(originalWriter => new GetHeaderWriter(originalWriter, this.authorizationToken));
        }

        private sealed class GetHeaderWriter : IGetHeaderWriter
        {
            private readonly IGetHeaderWriter nextWriter;

            public GetHeaderWriter(IGetHeaderWriter originalWriter, string authorizationToken)
            {
                var customHeaderWriter = originalWriter.CommitCustomHeader();
                var headerFieldValueWriter = customHeaderWriter.Commit(new HeaderFieldName("Authorization"));
                this.nextWriter = headerFieldValueWriter.Commit(new HeaderFieldValue(authorizationToken));
            }

            public IGetBodyWriter Commit()
            {
                return this.nextWriter.Commit();
            }

            public ICustomHeaderWriter<IGetHeaderWriter> CommitCustomHeader()
            {
                return this.nextWriter.CommitCustomHeader();
            }

            public IOdataMaxPageSizeHeaderWriter CommitOdataMaxPageSize()
            {
                return this.nextWriter.CommitOdataMaxPageSize();
            }

            public IOdataMaxVersionHeaderWriter CommitOdataMaxVersion()
            {
                return this.nextWriter.CommitOdataMaxVersion();
            }
        }

        public IPatchRequestWriter Patch()
        {
            return this.convention.Patch().OverrideHeader(originalWriter => new PatchHeaderWriter(originalWriter, this.authorizationToken));
        }

        private sealed class PatchHeaderWriter : IPatchHeaderWriter
        {
            private readonly IPatchHeaderWriter nextWriter;

            public PatchHeaderWriter(IPatchHeaderWriter originalWriter, string authorizationToken)
            {
                var customHeaderWriter = originalWriter.CommitCustomHeader();
                var headerFieldValueWriter = customHeaderWriter.Commit(new HeaderFieldName("Authorization"));
                this.nextWriter = headerFieldValueWriter.Commit(new HeaderFieldValue(authorizationToken));
            }

            public IPatchRequestBodyWriter Commit()
            {
                return this.nextWriter.Commit();
            }

            public ICustomHeaderWriter<IPatchHeaderWriter> CommitCustomHeader()
            {
                return this.nextWriter.CommitCustomHeader();
            }

            public IEtagWriter CommitEtag()
            {
                return this.nextWriter.CommitEtag();
            }
        }

        public IPatchRequestWriter Post()
        {
            return this.convention.Post().OverrideHeader(originalWriter => new PatchHeaderWriter(originalWriter, this.authorizationToken));
        }
    }
}

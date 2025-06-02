namespace NewStuff._Design._0_Convention.Sample
{
    using System.Threading.Tasks;

    public sealed class GraphConvention : IConvention
    {
        private readonly IConvention convention;
        private readonly string authorizationToken;

        public GraphConvention(IConvention convention, string authorizationToken)
        {
            this.convention = convention;
            this.authorizationToken = authorizationToken;
        }

        public async Task<IGetRequestWriter> Get()
        {
            var getRequestWriter = await this.convention.Get().ConfigureAwait(false);
            return 
                await 
                    getRequestWriter
                        .OverrideHeader(
                            async originalWriter => 
                                await 
                                    GetHeaderWriter
                                        .Create(
                                            originalWriter, 
                                            this.authorizationToken)
                                .ConfigureAwait(false))
                .ConfigureAwait(false);
        }

        private sealed class GetHeaderWriter : IGetHeaderWriter
        {
            private readonly IGetHeaderWriter nextWriter;

            public GetHeaderWriter(IGetHeaderWriter nextWriter)
            {
                this.nextWriter = nextWriter;
            }

            public static async Task<GetHeaderWriter> Create(IGetHeaderWriter originalWriter, string authorizationToken)
            {
                var customHeaderWriter = await originalWriter.CommitCustomHeader().ConfigureAwait(false);
                var headerFieldValueWriter = await customHeaderWriter.Commit(new HeaderFieldName("Authorization")).ConfigureAwait(false);
                var nextWriter = await headerFieldValueWriter.Commit(new HeaderFieldValue(authorizationToken)).ConfigureAwait(false);
                return new GetHeaderWriter(nextWriter);
            }

            public async Task<IGetBodyWriter> Commit()
            {
                return await this.nextWriter.Commit().ConfigureAwait(false);
            }

            public async Task<ICustomHeaderWriter<IGetHeaderWriter>> CommitCustomHeader()
            {
                return await this.nextWriter.CommitCustomHeader().ConfigureAwait(false);
            }

            public async Task<IOdataMaxPageSizeHeaderWriter> CommitOdataMaxPageSize()
            {
                return await this.nextWriter.CommitOdataMaxPageSize().ConfigureAwait(false);
            }

            public async Task<IOdataMaxVersionHeaderWriter> CommitOdataMaxVersion()
            {
                return await this.nextWriter.CommitOdataMaxVersion().ConfigureAwait(false);
            }
        }

        public IPatchRequestWriter Patch()
        {
            return this.convention.Patch().OverrideHeader(async originalWriter => await PatchHeaderWriter.Create(originalWriter, this.authorizationToken).ConfigureAwait(false));
        }

        private sealed class PatchHeaderWriter : IPatchHeaderWriter
        {
            private readonly IPatchHeaderWriter nextWriter;

            private PatchHeaderWriter(IPatchHeaderWriter nextWriter)
            {
                this.nextWriter = nextWriter;
            }

            public static async Task<PatchHeaderWriter> Create(IPatchHeaderWriter originalWriter, string authorizationToken)
            {
                var customHeaderWriter = originalWriter.CommitCustomHeader();
                var headerFieldValueWriter = await customHeaderWriter.Commit(new HeaderFieldName("Authorization")).ConfigureAwait(false);
                var nextWriter = await headerFieldValueWriter.Commit(new HeaderFieldValue(authorizationToken)).ConfigureAwait(false);
                return new PatchHeaderWriter(nextWriter);
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
            return this.convention.Patch().OverrideHeader(async originalWriter => await PatchHeaderWriter.Create(originalWriter, this.authorizationToken).ConfigureAwait(false));
        }
    }
}

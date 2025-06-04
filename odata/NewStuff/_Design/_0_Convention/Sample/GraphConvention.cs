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
            IGetRequestWriter? getRequestWriter = null;
            try
            {
                getRequestWriter = await this.convention.Get().ConfigureAwait(false);
                return
                    getRequestWriter
                        .OverrideHeader(
                            async originalWriter =>
                                await
                                    GetHeaderWriter
                                        .Create(
                                            originalWriter,
                                            this.authorizationToken)
                                .ConfigureAwait(false));
            }
            catch
            {
                if (getRequestWriter != null)
                {
                    await getRequestWriter.DisposeAsync().ConfigureAwait(false);
                }

                throw;
            }
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
            IPatchRequestWriter? patchRequestWriter = null;
            try
            {
                patchRequestWriter = this.convention.Patch();
                return patchRequestWriter.OverrideHeader(async originalWriter => await PatchHeaderWriter.Create(originalWriter, this.authorizationToken).ConfigureAwait(false));
            }
            catch
            {
                if (patchRequestWriter != null)
                {
                    patchRequestWriter.DisposeAsync().ConfigureAwait(false).GetAwaiter().GetResult(); //// TODO make async
                }

                throw;
            }
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
                var customHeaderWriter = await originalWriter.CommitCustomHeader().ConfigureAwait(false);
                var headerFieldValueWriter = await customHeaderWriter.Commit(new HeaderFieldName("Authorization")).ConfigureAwait(false);
                var nextWriter = await headerFieldValueWriter.Commit(new HeaderFieldValue(authorizationToken)).ConfigureAwait(false);
                return new PatchHeaderWriter(nextWriter);
            }

            public async Task<IPatchRequestBodyWriter> Commit()
            {
                return await this.nextWriter.Commit().ConfigureAwait(false);
            }

            public async Task<ICustomHeaderWriter<IPatchHeaderWriter>> CommitCustomHeader()
            {
                return await this.nextWriter.CommitCustomHeader().ConfigureAwait(false);
            }

            public async Task<IEtagWriter> CommitEtag()
            {
                return await this.nextWriter.CommitEtag().ConfigureAwait(false);
            }
        }

        public IPatchRequestWriter Post()
        {
            IPatchRequestWriter? patchRequestWriter = null;
            try
            {
                patchRequestWriter = this.convention.Post();
                return patchRequestWriter.OverrideHeader(async originalWriter => await PatchHeaderWriter.Create(originalWriter, this.authorizationToken).ConfigureAwait(false));
            }
            catch
            {
                if (patchRequestWriter != null)
                {
                    patchRequestWriter.DisposeAsync().ConfigureAwait(false).GetAwaiter().GetResult(); //// TODO make async
                }

                throw;
            }
        }
    }
}

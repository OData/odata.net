namespace NewStuff._Design._0_Convention.Sample
{
    public sealed class GraphConvention : IConvention
    {
        private readonly IConvention convention;

        public GraphConvention(IConvention convention)
        {
            this.convention = convention;
        }

        public IGetRequestWriter Get()
        {
            return this.convention.Get().OverrideUriScheme(originalWriter => new SchemeWriter(originalWriter));
        }

        private sealed class SchemeWriter : IUriSchemeWriter<IGetHeaderWriter>
        {
            private readonly IUriSchemeWriter<IGetHeaderWriter> originalUriSchemeWriter;

            public SchemeWriter(IUriSchemeWriter<IGetHeaderWriter> originalUriSchemeWriter)
            {
                this.originalUriSchemeWriter = originalUriSchemeWriter;
            }

            public IUriDomainWriter<IGetHeaderWriter> Commit(UriScheme uriScheme)
            {
                return this.originalUriSchemeWriter.Commit(new UriScheme("https"));
            }
        }

        public IPatchRequestWriter Patch()
        {
            throw new System.NotImplementedException();
        }

        public IPatchRequestWriter Post()
        {
            throw new System.NotImplementedException();
        }
    }
}

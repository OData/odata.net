namespace NewStuff._Design._0_Convention
{
    using System;
    using System.Threading.Tasks;

    public static class WriterOverriders
    {
        public static IGetRequestWriter OverrideUriWriter(this IGetRequestWriter getRequestWriter, Func<IUriWriter<IGetHeaderWriter>, Task<IUriWriter<IGetHeaderWriter>>> writerSelector)
        {
            return new OverrideUriWriterGetRequestWriter(getRequestWriter, writerSelector);
        }

        private sealed class OverrideUriWriterGetRequestWriter : IGetRequestWriter
        {
            private readonly IGetRequestWriter getRequestWriter;
            private readonly Func<IUriWriter<IGetHeaderWriter>, Task<IUriWriter<IGetHeaderWriter>>> writerSelector;

            public OverrideUriWriterGetRequestWriter(IGetRequestWriter getRequestWriter, Func<IUriWriter<IGetHeaderWriter>, Task<IUriWriter<IGetHeaderWriter>>> writerSelector)
            {
                this.getRequestWriter = getRequestWriter;
                this.writerSelector = writerSelector;
            }

            public async Task<IUriWriter<IGetHeaderWriter>> Commit()
            {
                return await this.writerSelector(await this.getRequestWriter.Commit().ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        public static IGetRequestWriter OverrideUriScheme(this IGetRequestWriter getRequestWriter, Func<IUriSchemeWriter<IGetHeaderWriter>, Task<IUriSchemeWriter<IGetHeaderWriter>>> writerSelector)
        {
            return getRequestWriter.OverrideUriWriter(async originalWriter => await Task.FromResult(new UriWriter<IGetHeaderWriter>(originalWriter, writerSelector)).ConfigureAwait(false));
        }

        private sealed class UriWriter<T> : IUriWriter<T>
        {
            private readonly IUriWriter<T> originalWriter;
            private readonly Func<IUriSchemeWriter<T>, Task<IUriSchemeWriter<T>>> writerSelector;

            public UriWriter(IUriWriter<T> originalWriter, Func<IUriSchemeWriter<T>, Task<IUriSchemeWriter<T>>> writerSelector)
            {
                this.originalWriter = originalWriter;
                this.writerSelector = writerSelector;
            }

            public async Task<IUriSchemeWriter<T>> Commit()
            {
                return await this.writerSelector(await this.originalWriter.Commit().ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        public static IGetRequestWriter OverrideUriDomain(this IGetRequestWriter getRequestWriter, Func<IUriDomainWriter<IGetHeaderWriter>, Task<IUriDomainWriter<IGetHeaderWriter>>> writerSelector)
        {
            return getRequestWriter.OverrideUriScheme(async originalWriter => await Task.FromResult(new UriSchemeWriter<IGetHeaderWriter>(originalWriter, writerSelector)).ConfigureAwait(false));
        }

        private sealed class UriSchemeWriter<T> : IUriSchemeWriter<T>
        {
            private readonly IUriSchemeWriter<T> originalWriter;
            private readonly Func<IUriDomainWriter<T>, Task<IUriDomainWriter<T>>> writerSelector;

            public UriSchemeWriter(IUriSchemeWriter<T> originalWriter, Func<IUriDomainWriter<T>, Task<IUriDomainWriter<T>>> writerSelector)
            {
                this.originalWriter = originalWriter;
                this.writerSelector = writerSelector;
            }

            public async Task<IUriDomainWriter<T>> Commit(UriScheme uriScheme)
            {
                return await this.writerSelector(await this.originalWriter.Commit(uriScheme).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        public static IGetRequestWriter OverrideUriPort(this IGetRequestWriter getRequestWriter, Func<IUriPortWriter<IGetHeaderWriter>, Task<IUriPortWriter<IGetHeaderWriter>>> writerSelector)
        {
            return getRequestWriter.OverrideUriDomain(async originalWriter => await Task.FromResult(new UriDomainWriter<IGetHeaderWriter>(originalWriter, writerSelector)).ConfigureAwait(false));
        }

        private sealed class UriDomainWriter<T> : IUriDomainWriter<T>
        {
            private readonly IUriDomainWriter<T> originalWriter;
            private readonly Func<IUriPortWriter<T>, Task<IUriPortWriter<T>>> writerSelector;

            public UriDomainWriter(IUriDomainWriter<T> originalWriter, Func<IUriPortWriter<T>, Task<IUriPortWriter<T>>> writerSelector)
            {
                this.originalWriter = originalWriter;
                this.writerSelector = writerSelector;
            }

            public async Task<IUriPortWriter<T>> Commit(UriDomain uriDomain)
            {
                return await this.writerSelector(await this.originalWriter.Commit(uriDomain).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        public static IGetRequestWriter OverrideUriPathSegment(this IGetRequestWriter getRequestWriter, Func<IUriPathSegmentWriter<IGetHeaderWriter>, Task<IUriPathSegmentWriter<IGetHeaderWriter>>> writerSelector)
        {
            return getRequestWriter.OverrideUriPort(originalWriter => new UriPortWriter<IGetHeaderWriter>(originalWriter, writerSelector));
        }

        private sealed class UriPortWriter<T> : IUriPortWriter<T>
        {
            private readonly IUriPortWriter<T> originalWriter;
            private readonly Func<IUriPathSegmentWriter<T>, Task<IUriPathSegmentWriter<T>>> writerSelector;

            public UriPortWriter(IUriPortWriter<T> originalWriter, Func<IUriPathSegmentWriter<T>, Task<IUriPathSegmentWriter<T>>> writerSelector)
            {
                this.originalWriter = originalWriter;
                this.writerSelector = writerSelector;
            }

            public Task<IUriPathSegmentWriter<T>> Commit()
            {
                return this.writerSelector(this.originalWriter.Commit());
            }

            public Task<IUriPathSegmentWriter<T>> Commit(UriPort uriPort)
            {
                return this.writerSelector(this.originalWriter.Commit(uriPort));
            }
        }

        public static IGetRequestWriter OverrideQueryOption(this IGetRequestWriter getRequestWriter, Func<IQueryOptionWriter<IGetHeaderWriter>, Task<IQueryOptionWriter<IGetHeaderWriter>>> writerSelector)
        {
            return getRequestWriter.OverrideUriPathSegment(originalWriter => new UriPathSegmentWriter<IGetHeaderWriter>(originalWriter, writerSelector));
        }

        private sealed class UriPathSegmentWriter<T> : IUriPathSegmentWriter<T>
        {
            private readonly IUriPathSegmentWriter<T> originalWriter;
            private readonly Func<IQueryOptionWriter<T>, Task<IQueryOptionWriter<T>>> writerSelector;

            public UriPathSegmentWriter(IUriPathSegmentWriter<T> originalWriter, Func<IQueryOptionWriter<T>, Task<IQueryOptionWriter<T>>> writerSelector)
            {
                this.originalWriter = originalWriter;
                this.writerSelector = writerSelector;
            }

            public Task<IQueryOptionWriter<T>> Commit()
            {
                return this.writerSelector(this.originalWriter.Commit());
            }

            public Task<IUriPathSegmentWriter<T>> Commit(UriPathSegment uriPathSegment)
            {
                return new UriPathSegmentWriter<T>(this.originalWriter.Commit(uriPathSegment), this.writerSelector);
            }
        }

        public static IGetRequestWriter OverrideFragment(this IGetRequestWriter getRequestWriter, Func<IFragmentWriter<IGetHeaderWriter>, Task<IFragmentWriter<IGetHeaderWriter>>> writerSelector)
        {
            return getRequestWriter.OverrideQueryOption(originalWriter => new QueryOptionWriter<IGetHeaderWriter>(originalWriter, writerSelector));
        }

        private sealed class QueryOptionWriter<T> : IQueryOptionWriter<T>
        {
            private readonly IQueryOptionWriter<T> originalWriter;
            private readonly Func<IFragmentWriter<T>, Task<IFragmentWriter<T>>> writerSelector;

            public QueryOptionWriter(IQueryOptionWriter<T> originalWriter, Func<IFragmentWriter<T>, Task<IFragmentWriter<T>>> writerSelector)
            {
                this.originalWriter = originalWriter;
                this.writerSelector = writerSelector;
            }

            public Task<T> Commit()
            {
                return this.originalWriter.Commit();
            }

            public Task<IFragmentWriter<T>> CommitFragment()
            {
                return this.writerSelector(this.originalWriter.CommitFragment());
            }

            public Task<IQueryParameterWriter<T>> CommitParameter()
            {
                return new QueryParameterWriter(this.originalWriter.CommitParameter(), this.writerSelector);
            }

            private sealed class QueryParameterWriter : IQueryParameterWriter<T>
            {
                private readonly IQueryParameterWriter<T> originalWriter;
                private readonly Func<IFragmentWriter<T>, Task<IFragmentWriter<T>>> writerSelector;

                public QueryParameterWriter(IQueryParameterWriter<T> originalWriter, Func<IFragmentWriter<T>, Task<IFragmentWriter<T>>> writerSelector)
                {
                    this.originalWriter = originalWriter;
                    this.writerSelector = writerSelector;
                }

                public Task<IQueryValueWriter<T>> Commit(QueryParameter queryParameter)
                {
                    return new QueryValueWriter(this.originalWriter.Commit(queryParameter), this.writerSelector);
                }

                private sealed class QueryValueWriter : IQueryValueWriter<T>
                {
                    private readonly IQueryValueWriter<T> originalWriter;
                    private readonly Func<IFragmentWriter<T>, Task<IFragmentWriter<T>>> writerSelector;

                    public QueryValueWriter(IQueryValueWriter<T> originalWriter, Func<IFragmentWriter<T>, Task<IFragmentWriter<T>>> writerSelector)
                    {
                        this.originalWriter = originalWriter;
                        this.writerSelector = writerSelector;
                    }

                    public Task<IQueryOptionWriter<T>> Commit()
                    {
                        return new QueryOptionWriter<T>(this.originalWriter.Commit(), this.writerSelector);
                    }

                    public Task<IQueryOptionWriter<T>> Commit(QueryValue queryValue)
                    {
                        return new QueryOptionWriter<T>(this.originalWriter.Commit(queryValue), this.writerSelector);
                    }
                }
            }
        }

        public static async Task<IGetRequestWriter> OverrideHeader(this IGetRequestWriter getRequestWriter, Func<IGetHeaderWriter, Task<IGetHeaderWriter>> writerSelector)
        {
            return getRequestWriter
                .OverrideQueryOption(originalWriter => new QueryOptionWriter2<IGetHeaderWriter>(originalWriter, writerSelector))
                .OverrideFragment(originalWriter => new FragmentWriter<IGetHeaderWriter>(originalWriter, writerSelector));
        }

        private sealed class QueryOptionWriter2<T> : IQueryOptionWriter<T>
        {
            private readonly IQueryOptionWriter<T> originalWriter;
            private readonly Func<T, Task<T>> writerSelector;

            public QueryOptionWriter2(IQueryOptionWriter<T> originalWriter, Func<T, Task<T>> writerSelector)
            {
                this.originalWriter = originalWriter;
                this.writerSelector = writerSelector;
            }

            public async Task<T> Commit()
            {
                return await this.writerSelector(await this.originalWriter.Commit().ConfigureAwait(false)).ConfigureAwait(false);
            }

            public async Task<IFragmentWriter<T>>> CommitFragment()
            {
                return new FragmentWriter<T>(await this.originalWriter.CommitFragment().ConfigureAwait(false), this.writerSelector);
            }

            public Task<IQueryParameterWriter<T>>> CommitParameter()
            {
                return new QueryParameterWriter(this.originalWriter.CommitParameter(), this.writerSelector);
            }

            private sealed class QueryParameterWriter : IQueryParameterWriter<T>
            {
                private readonly IQueryParameterWriter<T> originalWriter;
                private readonly Func<T, Task<T>> writerSelector;

                public QueryParameterWriter(IQueryParameterWriter<T> originalWriter, Func<T, Task<T>> writerSelector)
                {
                    this.originalWriter = originalWriter;
                    this.writerSelector = writerSelector;
                }

                public Task<IQueryValueWriter<T>> Commit(QueryParameter queryParameter)
                {
                    return new QueryValueWriter(this.originalWriter.Commit(queryParameter), this.writerSelector);
                }

                private sealed class QueryValueWriter : IQueryValueWriter<T>
                {
                    private readonly IQueryValueWriter<T> originalWriter;
                    private readonly Func<T, Task<T>> writerSelector;

                    public QueryValueWriter(IQueryValueWriter<T> originalWriter, Func<T, Task<T>> writerSelector)
                    {
                        this.originalWriter = originalWriter;
                        this.writerSelector = writerSelector;
                    }

                    public Task<IQueryOptionWriter<T>> Commit()
                    {
                        return new QueryOptionWriter2<T>(this.originalWriter.Commit(), this.writerSelector);
                    }

                    public Task<IQueryOptionWriter<T>> Commit(QueryValue queryValue)
                    {
                        return new QueryOptionWriter2<T>(this.originalWriter.Commit(queryValue), this.writerSelector);
                    }
                }
            }
        }

        private sealed class FragmentWriter<T> : IFragmentWriter<T>
        {
            private readonly IFragmentWriter<T> originalWriter;
            private readonly Func<T, Task<T>> writerSelector;

            public FragmentWriter(IFragmentWriter<T> originalWriter, Func<T, Task<T>> writerSelector)
            {
                this.originalWriter = originalWriter;
                this.writerSelector = writerSelector;
            }

            public Task<T> Commit(Fragment fragment)
            {
                return this.writerSelector(this.originalWriter.Commit(fragment));
            }
        }

        public static IPatchRequestWriter OverrideUriWriter(this IPatchRequestWriter getRequestWriter, Func<IUriWriter<IPatchHeaderWriter>, Task<IUriWriter<IPatchHeaderWriter>>> writerSelector)
        {
            return new OverrideUriWriterPatchRequestWriter(getRequestWriter, writerSelector);
        }

        private sealed class OverrideUriWriterPatchRequestWriter : IPatchRequestWriter
        {
            private readonly IPatchRequestWriter patchRequestWriter;
            private readonly Func<IUriWriter<IPatchHeaderWriter>, Task<IUriWriter<IPatchHeaderWriter>>> writerSelector;

            public OverrideUriWriterPatchRequestWriter(IPatchRequestWriter getRequestWriter, Func<IUriWriter<IPatchHeaderWriter>, Task<IUriWriter<IPatchHeaderWriter>>> writerSelector)
            {
                this.patchRequestWriter = getRequestWriter;
                this.writerSelector = writerSelector;
            }

            public Task<IUriWriter<IPatchHeaderWriter>> Commit()
            {
                return this.writerSelector(this.patchRequestWriter.Commit());
            }
        }

        public static IPatchRequestWriter OverrideUriScheme(this IPatchRequestWriter getRequestWriter, Func<IUriSchemeWriter<IPatchHeaderWriter>, Task<IUriSchemeWriter<IPatchHeaderWriter>>> writerSelector)
        {
            return getRequestWriter.OverrideUriWriter(originalWriter => new UriWriter<IPatchHeaderWriter>(originalWriter, writerSelector));
        }

        public static IPatchRequestWriter OverrideUriDomain(this IPatchRequestWriter getRequestWriter, Func<IUriDomainWriter<IPatchHeaderWriter>, Task<IUriDomainWriter<IPatchHeaderWriter>>> writerSelector)
        {
            return getRequestWriter.OverrideUriScheme(originalWriter => new UriSchemeWriter<IPatchHeaderWriter>(originalWriter, writerSelector));
        }

        public static IPatchRequestWriter OverrideUriPort(this IPatchRequestWriter getRequestWriter, Func<IUriPortWriter<IPatchHeaderWriter>, Task<IUriPortWriter<IPatchHeaderWriter>>> writerSelector)
        {
            return getRequestWriter.OverrideUriDomain(originalWriter => new UriDomainWriter<IPatchHeaderWriter>(originalWriter, writerSelector));
        }

        public static IPatchRequestWriter OverrideUriPathSegment(this IPatchRequestWriter getRequestWriter, Func<IUriPathSegmentWriter<IPatchHeaderWriter>, Task<IUriPathSegmentWriter<IPatchHeaderWriter>>> writerSelector)
        {
            return getRequestWriter.OverrideUriPort(originalWriter => new UriPortWriter<IPatchHeaderWriter>(originalWriter, writerSelector));
        }

        public static IPatchRequestWriter OverrideQueryOption(this IPatchRequestWriter getRequestWriter, Func<IQueryOptionWriter<IPatchHeaderWriter>, Task<IQueryOptionWriter<IPatchHeaderWriter>>> writerSelector)
        {
            return getRequestWriter.OverrideUriPathSegment(originalWriter => new UriPathSegmentWriter<IPatchHeaderWriter>(originalWriter, writerSelector));
        }

        public static IPatchRequestWriter OverrideFragment(this IPatchRequestWriter getRequestWriter, Func<IFragmentWriter<IPatchHeaderWriter>, Task<IFragmentWriter<IPatchHeaderWriter>>> writerSelector)
        {
            return getRequestWriter.OverrideQueryOption(originalWriter => new QueryOptionWriter<IPatchHeaderWriter>(originalWriter, writerSelector));
        }

        public static IPatchRequestWriter OverrideHeader(this IPatchRequestWriter getRequestWriter, Func<IPatchHeaderWriter, Task<IPatchHeaderWriter>> writerSelector)
        {
            return getRequestWriter
                .OverrideQueryOption(originalWriter => new QueryOptionWriter2<IPatchHeaderWriter>(originalWriter, writerSelector))
                .OverrideFragment(originalWriter => new FragmentWriter<IPatchHeaderWriter>(originalWriter, writerSelector));
        }
    }

    public static class ReaderOverriders
    {
        //// TODO 
    }
}

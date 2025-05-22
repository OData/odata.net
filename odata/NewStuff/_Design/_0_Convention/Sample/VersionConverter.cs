using System;

namespace NewStuff._Design._0_Convention.Sample
{
    public static class VersionConverter
    {
        public static void Convert(RequestReader requestReader, RequestWriter requestWriter)
        {
            // we are a passthrough serivce and we received a request that we need to send along, but we want to rewrite the version header

            var token = requestReader.Next();
            if (token is RequestToken.Get get)
            {
                Convert(get.GetRequestReader, requestWriter.CommitGet());
            }
            else if (token is RequestToken.Patch patch)
            {
                throw new NotImplementedException("TODO");
            }
            else if (token is RequestToken.Post post)
            {
                throw new NotImplementedException("TODO");
            }
            else
            {
                throw new System.Exception("TODO implement visitor");
            }
        }

        private static void Convert(GetRequestReader getRequestReader, GetRequestWriter getRequestWriter)
        {
            Convert(getRequestReader.Next(), getRequestWriter.Commit());
        }

        private static void Convert(UriReader<GetHeaderReader> uriReader, UriWriter<GetHeaderWriter> uriWriter)
        {
            Convert(uriReader.Next(), uriWriter.Commit());
        }

        private static void Convert(UriSchemeReader<GetHeaderReader> uriSchemeReader, UriSchemeWriter<GetHeaderWriter> uriSchemeWriter)
        {
            Convert(uriSchemeReader.Next(), uriSchemeWriter.Commit(uriSchemeReader.UriScheme));
        }

        private static void Convert(UriDomainReader<GetHeaderReader> uriDomainReader, UriDomainWriter<GetHeaderWriter> uriDomainWriter)
        {
            Convert(uriDomainReader.Next(), uriDomainWriter.Commit(uriDomainReader.UriDomain));
        }

        private static void Convert(UriPortReader<GetHeaderReader> uriPortReader, UriPortWriter<GetHeaderWriter> uriPortWriter)
        {
            Convert(uriPortReader.Next(), uriPortWriter.Commit(uriPortReader.UriPort));
        }

        private static void Convert(UriPathSegmentReader<GetHeaderReader> uriPathSegmentReader, UriPathSegmentWriter<GetHeaderWriter> uriPathSegmentWriter)
        {
            uriPathSegmentWriter.Commit(uriPathSegmentReader.UriPathSegment);

            var pathSegmentToken = uriPathSegmentReader.Next();
            if (pathSegmentToken is PathSegmentToken<GetHeaderReader>.PathSegment pathSegment)
            {
            }
            else if (pathSegmentToken is PathSegmentToken<GetHeaderReader>.QueryOption queryOptions)
            {
            }
        }








































        public static void Convert(RequestReader requestReader)
        {
            // TODO we are a service where we need to, for compatibility, update the incoming version header so that our (older code) can make sense of it
        }
    }
}

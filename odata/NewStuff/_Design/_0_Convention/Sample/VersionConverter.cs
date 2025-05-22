namespace NewStuff._Design._0_Convention.Sample
{
    using System;

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
            var nextWriter = uriPathSegmentWriter.Commit(uriPathSegmentReader.UriPathSegment);

            var pathSegmentToken = uriPathSegmentReader.Next();
            if (pathSegmentToken is PathSegmentToken<GetHeaderReader>.PathSegment pathSegment)
            {
                Convert(pathSegment.UriPathSegmentReader, nextWriter);
            }
            else if (pathSegmentToken is PathSegmentToken<GetHeaderReader>.QueryOption queryOption)
            {
                Convert(queryOption.QueryOptionsReader, nextWriter.Commit());
            }
            else
            {
                throw new Exception("TODO implement visitor");
            }
        }

        private static void Convert(QueryOptionReader<GetHeaderReader> queryOption, QueryOptionWriter<GetHeaderWriter> queryOptionWriter)
        {
            var queryOptionToken = queryOption.Next();
            if (queryOptionToken is QueryOptionToken<GetHeaderReader>.QueryParameter queryParameter)
            {
                Convert(queryParameter.QueryParameterReader, queryOptionWriter.CommitParameter());
            }
            else if (queryOptionToken is QueryOptionToken<GetHeaderReader>.Fragment fragment)
            {
                Convert(fragment.FragmentReader, queryOptionWriter.CommitFragment());
            }
            else
            {
                throw new Exception("TODO implement visitor");
            }
        }

        private static void Convert(QueryParameterReader<GetHeaderReader> queryParameterReader, QueryParameterWriter<GetHeaderWriter> queryParameterWriter)
        {
            var nextWriter = queryParameterWriter.Commit(queryParameterReader.QueryParameter);

            var queryParameterToken = queryParameterReader.Next();
            if (queryParameterToken is QueryParameterToken<GetHeaderReader>.QueryOption queryOption)
            {
                Convert(queryOption.QueryOptionReader, nextWriter.Commit());
            }
            else if (queryParameterToken is QueryParameterToken<GetHeaderReader>.QueryValue queryValue)
            {
                Convert(queryValue.QueryValueReader, nextWriter);
            }
            else
            {
                throw new Exception("TODO implement visitor");
            }
        }

        private static void Convert(QueryValueReader<GetHeaderReader> queryValueReader, QueryValueWriter<GetHeaderWriter> queryValueWriter)
        {
            Convert(queryValueReader.Next(), queryValueWriter.Commit(queryValueReader.QueryValue));
        }

        private static void Convert(FragmentReader<GetHeaderReader> fragmentReader, FragmentWriter<GetHeaderWriter> fragmentWriter)
        {
            Convert(fragmentReader.Next(), fragmentWriter.Commit(fragmentReader.Fragment));
        }

        private static void Convert(GetHeaderReader getHeaderReader, GetHeaderWriter getHeaderWriter)
        {
            var getHeaderToken = getHeaderReader.Next();
            if (getHeaderToken is GetHeaderToken.OdataMaxVersion odataMaxVersion)
            {
                Convert(odataMaxVersion.OdataMaxVersionHeaderReader, getHeaderWriter.CommitOdataMaxVersion());
            }
            else if (getHeaderToken is GetHeaderToken.OdataMaxPageSize odataMaxPageSize)
            {
                Convert(odataMaxPageSize.OdataMaxPageSizeHeaderReader, getHeaderWriter.CommitOdataMaxPageSize());
            }
            else if (getHeaderToken is GetHeaderToken.Custom custom)
            {
                //// TODO you are here
            }
            else if (getHeaderToken is GetHeaderToken.GetBody getBody)
            {
            }
            else
            {
                throw new Exception("TODO implement visitor");
            }
        }

        private static void Convert(OdataMaxVersionHeaderReader odataMaxVersionHeaderReader, OdataMaxVersionHeaderWriter odataMaxVersionHeaderWriter)
        {
        }

        private static void Convert(OdataMaxPageSizeHeaderReader odataMaxPageSizeHeaderReader, OdataMaxPageSizeHeaderWriter odataMaxPageSizeHeaderWriter)
        {
        }








































        public static void Convert(RequestReader requestReader)
        {
            // TODO we are a service where we need to, for compatibility, update the incoming version header so that our (older code) can make sense of it
        }
    }
}

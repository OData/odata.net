namespace NewStuff._Design._0_Convention.Sample
{
    using System;

    public static class VersionConverter
    {
        public static void Convert(IRequestReader requestReader, IRequestWriter requestWriter)
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

        private static void Convert(IGetRequestReader getRequestReader, IGetRequestWriter getRequestWriter)
        {
            Convert(getRequestReader.Next(), getRequestWriter.Commit());
        }

        private static void Convert(IUriReader<GetHeaderReader> uriReader, IUriWriter<GetHeaderWriter> uriWriter)
        {
            Convert(uriReader.Next(), uriWriter.Commit());
        }

        private static void Convert(IUriSchemeReader<GetHeaderReader> uriSchemeReader, IUriSchemeWriter<GetHeaderWriter> uriSchemeWriter)
        {
            Convert(uriSchemeReader.Next(), uriSchemeWriter.Commit(uriSchemeReader.UriScheme));
        }

        private static void Convert(IUriDomainReader<GetHeaderReader> uriDomainReader, IUriDomainWriter<GetHeaderWriter> uriDomainWriter)
        {
            Convert(uriDomainReader.Next(), uriDomainWriter.Commit(uriDomainReader.UriDomain));
        }

        private static void Convert(IUriPortReader<GetHeaderReader> uriPortReader, IUriPortWriter<GetHeaderWriter> uriPortWriter)
        {
            Convert(uriPortReader.Next(), uriPortWriter.Commit(uriPortReader.UriPort));
        }

        private static void Convert(IUriPathSegmentReader<GetHeaderReader> uriPathSegmentReader, IUriPathSegmentWriter<GetHeaderWriter> uriPathSegmentWriter)
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

        private static void Convert(IQueryOptionReader<GetHeaderReader> queryOption, IQueryOptionWriter<GetHeaderWriter> queryOptionWriter)
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

        private static void Convert(IQueryParameterReader<GetHeaderReader> queryParameterReader, IQueryParameterWriter<GetHeaderWriter> queryParameterWriter)
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

        private static void Convert(IQueryValueReader<GetHeaderReader> queryValueReader, IQueryValueWriter<GetHeaderWriter> queryValueWriter)
        {
            Convert(queryValueReader.Next(), queryValueWriter.Commit(queryValueReader.QueryValue));
        }

        private static void Convert(IFragmentReader<GetHeaderReader> fragmentReader, IFragmentWriter<GetHeaderWriter> fragmentWriter)
        {
            Convert(fragmentReader.Next(), fragmentWriter.Commit(fragmentReader.Fragment));
        }

        private static void Convert(IGetHeaderReader getHeaderReader, IGetHeaderWriter getHeaderWriter)
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
                Convert(custom.CustomHeaderReader, getHeaderWriter.CommitCustomHeader());
            }
            else if (getHeaderToken is GetHeaderToken.GetBody getBody)
            {
                Convert(getBody.GetBodyReader, getHeaderWriter.Commit());
            }
            else
            {
                throw new Exception("TODO implement visitor");
            }
        }

        private static void Convert(IOdataMaxVersionHeaderReader odataMaxVersionHeaderReader, IOdataMaxVersionHeaderWriter odataMaxVersionHeaderWriter)
        {
            const OdataVersion newVersion = null!;

            Convert(odataMaxVersionHeaderReader.Next(), odataMaxVersionHeaderWriter.Commit(newVersion));
        }

        private static void Convert(IOdataMaxPageSizeHeaderReader odataMaxPageSizeHeaderReader, IOdataMaxPageSizeHeaderWriter odataMaxPageSizeHeaderWriter)
        {
            Convert(odataMaxPageSizeHeaderReader.Next(), odataMaxPageSizeHeaderWriter.Commit(odataMaxPageSizeHeaderReader.OdataMaxPageSize));
        }

        private static void Convert(ICustomHeaderReader customHeaderReader, ICustomHeaderWriter customHeaderWriter)
        {
            var nextWriter = customHeaderWriter.Commit(customHeaderReader.HeaderFieldName);
            var customHeaderToken = customHeaderReader.Next();
            if (customHeaderToken is CustomHeaderToken.FieldValue fieldValue)
            {
                Convert(fieldValue.HeaderFieldValueReader, nextWriter);
            }
            else if (customHeaderToken is CustomHeaderToken.GetHeader getHeader)
            {
                Convert(getHeader.GetHeaderReader, nextWriter.Commit());
            }
            else
            {
                throw new Exception("TODO implement visitor");
            }
        }

        private static void Convert(IHeaderFieldValueReader headerFieldValueReader, IHeaderFieldValueWriter headerFieldValueWriter)
        {
            Convert(headerFieldValueReader.Next(), headerFieldValueWriter.Commit(headerFieldValueReader.HeaderFieldValue));
        }

        private static void Convert(IGetBodyReader getBodyReader, IGetBodyWriter getBodyWriter)
        {
            getBodyWriter.Commit();
        }
    }
}

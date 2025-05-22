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
                var uriReader = get.GetRequestReader.Next();
                var schemeReader = uriReader.Next();

                requestWriter.CommitGet().Commit().Commit().Commit(schemeReader.UriScheme).Commit();
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










































        public static void Convert(RequestReader requestReader)
        {
            // TODO we are a service where we need to, for compatibility, update the incoming version header so that our (older code) can make sense of it
        }
    }
}

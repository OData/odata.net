namespace NewStuff._Design._4.Sample
{
    using System;

    using NewStuff._Design._0_Convention.Sample;
    using NewStuff._Design._1_Protocol;
    using NewStuff._Design._1_Protocol.Sample;

    public static class MultiValuedProtocolFactory
    {
        public static IMultiValuedProtocol Create(Func<IHttpClient> httpClientFactory)
        {
            var convention = ConventionFactory.Create(httpClientFactory);
            return new MultiValuedProtocol(convention, new Uri("https://graph.microsoft.com/v1.0/users"));
        }
    }
}

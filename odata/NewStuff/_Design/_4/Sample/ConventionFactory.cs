namespace NewStuff._Design._4.Sample
{
    using NewStuff._Design._0_Convention;
    using NewStuff._Design._0_Convention.Sample;
    using System;

    public static class ConventionFactory
    {
        public static IConvention Create(Func<IHttpClient> httpClientFactory)
        {
            return new HttpClientConvention(httpClientFactory);
        }
    }
}

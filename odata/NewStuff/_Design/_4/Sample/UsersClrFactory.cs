namespace NewStuff._Design._4.Sample
{
    using System;

    using NewStuff._Design._0_Convention.Sample;
    using NewStuff._Design._2_Clr;
    using NewStuff._Design._2_Clr.Sample;

    public static class UsersClrFactory
    {
        public static ICollectionClr<User, string> Create(Func<IHttpClient> httpClientFactory)
        {
            var multiValuedProtocol = MultiValuedProtocolFactory.Create(httpClientFactory);
            return new UsersClr(multiValuedProtocol);
        }
    }
}

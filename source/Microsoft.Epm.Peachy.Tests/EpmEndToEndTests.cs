namespace Microsoft.Epm.Peachy.Tests //// TODO get the right namespace
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public sealed class EpmEndToEndTests //// TODO probably this should go in a microsoft.epm.testcore to be shared across the peachy implementation, the webapi implementation, etc
    {
        private readonly Uri rootUri;

        public EpmEndToEndTests(Uri rootUri)
        {
            if (rootUri == null)
            {
                throw new ArgumentNullException(nameof(rootUri));
            }

            this.rootUri = rootUri;
        }

        public async Task GetAuthorizationSystem()
        {
            using (var httpClient = new HttpClient())
            {
                //// TODO make this look more like the api.md
                using (var httpResponse = await httpClient.GetAsync(new Uri(this.rootUri, "foo")))
                {
                    Assert.AreEqual(208, (int)httpResponse.StatusCode);
                    CollectionAssert.Contains(httpResponse.Headers.Select(_ => (_.Key, _.Value.Single())).Select(_ => string.Concat(_.Key, ": ", _.Item2)).ToArray(), "gdebruin: did this also work?");
                    Assert.AreEqual("TODO this should be a 404", await httpResponse.Content.ReadAsStringAsync());
                }
            }
        }
    }
}

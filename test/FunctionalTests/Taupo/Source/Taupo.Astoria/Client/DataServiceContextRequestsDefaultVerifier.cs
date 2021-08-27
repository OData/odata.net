//---------------------------------------------------------------------
// <copyright file="DataServiceContextRequestsDefaultVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Default verifier for the requests sent by the data service context. Does not verify headers or request bodies.
    /// </summary>
    [ImplementationName(typeof(IDataServiceContextRequestsVerifier), "Default")]
    public class DataServiceContextRequestsDefaultVerifier : IDataServiceContextRequestsVerifier
    {
        /// <summary>
        /// Gets or sets the assertion handler to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public AssertionHandler Assert { get; set; }

        /// <summary>
        /// Verifies the requests sent by the data service context.
        /// </summary>
        /// <param name="expected">The expected request data.</param>
        /// <param name="observed">The observed requests.</param>
        public void VerifyRequests(IEnumerable<HttpRequestData> expected, IEnumerable<SendingRequest2EventArgs> observed)
        {
            var expectedList = expected.ToList();
            var observedList = observed.ToList();

            this.Assert.AreEqual(expectedList.Count, observedList.Count, "Unexpected number of requests");

            for (int i = 0; i < expectedList.Count; i++)
            {
                var expectedRequest = expectedList[i];
                var observedRequest = observedList[i];

                string expectedMethod = expectedRequest.Verb.ToHttpMethod();
                Uri observedUri = observedRequest.RequestMessage.Url;

                this.Assert.AreEqual(expectedMethod, observedRequest.RequestMessage.Method.ToString().ToUpperInvariant(), EqualityComparer<string>.Default, "Verb for request #{0} did not match", i + 1);
                this.Assert.IsNotNull(expectedRequest.Uri, string.Format(CultureInfo.InvariantCulture, "Expected request #{0} did not have a uri", i + 1));
                if (expectedRequest.Uri.IsAbsoluteUri)
                {
                    this.Assert.AreEqual(expectedRequest.Uri.OriginalString, observedUri.OriginalString, string.Format(CultureInfo.InvariantCulture, "Uri for request #{0} did not match", i + 1));
                }
                else
                {
                    // TODO: if/when the product adds a way to read the xml:base from the feed, remove this and always expect absolute URIs
                    this.Assert.IsTrue(
                        observedUri.OriginalString.EndsWith(expectedRequest.Uri.OriginalString, StringComparison.Ordinal),
                        string.Format(CultureInfo.InvariantCulture, "Relative uri for request #{0} did not match. Expected '{1}', observed '{2}'", i + 1, expectedRequest.Uri.OriginalString, observedUri.OriginalString));
                }

                // Skip this now for SL tests because the sending request header verification fails for update verbs.
                // TODO: throw if there are unexpected headers. This may not be worth the effort, however.
                foreach (var expectedHeader in expectedRequest.Headers)
                {
                    var observedHeader = observedRequest.RequestMessage.GetHeader(expectedHeader.Key);

                    // null is used to black-list headers
                    if (expectedHeader.Value == null)
                    {
                        this.Assert.IsFalse((observedRequest.RequestMessage.GetHeader(expectedHeader.Key) != null), string.Format(CultureInfo.InvariantCulture, "Request #{0} had unexpected header '{1}' with value '{2}'", i + 1, expectedHeader.Key, observedHeader));
                    }
                    else
                    {
                        this.Assert.AreEqual(expectedHeader.Value, observedHeader, string.Format(CultureInfo.InvariantCulture, "Value of header '{0}' did not match for request #{1}", expectedHeader.Key, i + 1));
                    }
                }
            }
        }
    }
}

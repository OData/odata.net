//---------------------------------------------------------------------
// <copyright file="DataServiceResponsePreferenceVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System.Globalization;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Contract for DataServiceResponsePreference verification.
    /// </summary>
    [ImplementationName(typeof(IDataServiceResponsePreferenceVerifier), "Default")]
    public class DataServiceResponsePreferenceVerifier : IDataServiceResponsePreferenceVerifier
    {
        /// <summary>
        /// Gets or sets the http tracker to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDataServiceContextHttpTracker HttpTracker { get; set; }

        /// <summary>
        /// Gets or sets assertion class to be used.
        /// </summary>
        [InjectDependency]
        public AssertionHandler Assert { get; set; }

        /// <summary>
        /// Gets or sets context type format applier
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IClientDataContextFormatApplier ClientDataContextFormatApplier { get; set; }

        /// <summary>
        /// Registers this verifier on the context's event
        /// </summary>
        /// <param name="context">The context to verify events on</param>
        public void RegisterEventHandler(DataServiceContext context)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            this.HttpTracker.RegisterHandler(context, this.HandleRequestResponsePair);
        }

        /// <summary>
        /// Unregisters this verifier from the context's event
        /// </summary>
        /// <param name="context">The context to stop verifing events on</param>
        /// <param name="inErrorState">A value indicating that we are recovering from an error</param>
        public void UnregisterEventHandler(DataServiceContext context, bool inErrorState)
        {
            ExceptionUtilities.CheckArgumentNotNull(context, "context");
            this.HttpTracker.UnregisterHandler(context, this.HandleRequestResponsePair, !inErrorState);
        }

        /// <summary>
        /// Verifies the headers present on the HTTP request
        /// </summary>
        /// <param name="context">The data service context</param>
        /// <param name="request">The http request</param>
        /// <param name="response">The http response</param>
        public void HandleRequestResponsePair(DataServiceContext context, HttpRequestData request, HttpResponseData response)
        {
            this.VerifyPreferHeaderOnRequest(request, context.AddAndUpdateResponsePreference);
            if (context.AddAndUpdateResponsePreference != DataServiceResponsePreference.None && this.IsCreateOrUpdate(request.GetEffectiveVerb()))
            {
                this.VerifyVersionHeaders(request);
            }
        }

        private void VerifyPreferHeaderOnRequest(HttpRequestData request, DataServiceResponsePreference responsePreference)
        {
            bool preferExists = request.Headers.Keys.Contains(HttpHeaders.Prefer);

            // NOTE: if the prefer header is not present, we cannot tell if it should have been, because we don't 
            // know here whether or not this is an entity-specific operation
            // In other words: even though this is POST/PUT/PATCH, it might be AddLink, SetLink, or a media-resource 
            // update, and therefore should NOT have a prefer header
            // For now, this component will simply verify what it can, and the rest will be covered by the 
            // verification of the requests sent during SaveChanges. We may be able to remove this component altogether.
            if (responsePreference == DataServiceResponsePreference.None || !this.IsCreateOrUpdate(request.GetEffectiveVerb()))
            {
                this.Assert.IsFalse(preferExists, "Unexpected presence of Prefer Header");
            }
            else if (preferExists)
            {
                string value = request.Headers[HttpHeaders.Prefer];
                if (responsePreference == DataServiceResponsePreference.IncludeContent)
                {
                    this.Assert.AreEqual(HttpHeaders.ReturnContent, value, "Incorrect value of Prefer Header");
                }

                if (responsePreference == DataServiceResponsePreference.NoContent)
                {
                    this.Assert.AreEqual(HttpHeaders.ReturnNoContent, value, "Incorrect value of Prefer Header");
                }
            }
        }

        private void VerifyVersionHeaders(HttpRequestData request)
        {
            if (request.Headers.Keys.Contains(HttpHeaders.Prefer))
            {
                this.Assert.IsTrue(DataServiceProtocolVersion.V4 <= request.GetDataServiceVersion(), string.Format(CultureInfo.InvariantCulture, "Request DSV should be at least 3 if prefer is specified. Value was '{0}'.", request.Headers[HttpHeaders.DataServiceVersion]));
                this.Assert.IsTrue(DataServiceProtocolVersion.V4 <= request.GetMaxDataServiceVersion(), string.Format(CultureInfo.InvariantCulture, "Request MDSV should be at least 3 if prefer is specified. Value was '{0}'.", request.Headers[HttpHeaders.MaxDataServiceVersion]));
            }
        }

        private bool IsCreateOrUpdate(HttpVerb verb)
        {
            return verb.IsUpdateVerb() || verb == HttpVerb.Post;
        }
    }
}

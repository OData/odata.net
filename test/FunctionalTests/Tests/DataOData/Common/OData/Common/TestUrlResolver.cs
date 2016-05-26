//---------------------------------------------------------------------
// <copyright file="TestUrlResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.OData;
    #endregion Namespaces

    /// <summary>
    /// A test implementation of the IODataPayloadUriConverter interface
    /// </summary>
    public class TestUrlResolver : IODataPayloadUriConverter
    {
        /// <summary>
        /// List of calls made to the resolver so far.
        /// </summary>
        private List<KeyValuePair<Uri, Uri>> calls;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestUrlResolver()
        {
            this.calls = new List<KeyValuePair<Uri, Uri>>();
        }

        /// <summary>
        /// The resolver func to call for resolutions.
        /// </summary>
        public Func<Uri, Uri, Uri> ResolutionCallback
        {
            get;
            set;
        }

        /// <summary>
        /// List of calls made to the resolver so far. The Key is the base URI and the Value is the payload URI.
        /// </summary>
        public IEnumerable<KeyValuePair<Uri, Uri>> Calls
        {
            get
            {
                return this.calls;
            }
        }

        /// <summary>
        /// Clears all the calls remembered.
        /// </summary>
        public void ClearCalls()
        {
            this.calls.Clear();
        }

        /// <summary>
        /// Method to implement a custom URL resolution scheme.
        /// This method returns null if no custom resolution is desired.
        /// If the method returns a non-null URL that value will be used without further validation.
        /// </summary>
        /// <param name="baseUri">The (optional) base URI to use for the resolution.</param>
        /// <param name="payloadUri">The URI read from the payload.</param>
        /// <returns>
        /// A <see cref="Uri"/> instance that reflects the custom resolution of the method arguments
        /// into a URL or null if no custom resolution is desired; in that case the default resolution is used.
        /// </returns>
        public Uri ConvertPayloadUri(Uri baseUri, Uri payloadUri)
        {
            this.calls.Add(new KeyValuePair<Uri, Uri>(baseUri, payloadUri));
            if (this.ResolutionCallback == null)
            {
                return null;
            }
            else
            {
                return this.ResolutionCallback(baseUri, payloadUri);
            }
        }
    }
}

//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services
{
    using System;
    using System.Diagnostics;
    using Microsoft.Data.OData;

    /// <summary>
    /// Represents the client's preferences as specified in the 'prefer' request header.
    /// </summary>
    internal class ClientPreference
    {
        /// <summary>
        /// Singleton instance which expresses no client preferences.
        /// </summary>
        internal static readonly ClientPreference None = new ClientPreference(ResponseBodyPreference.None);

        /// <summary>
        /// The client's preference for whether or not to send a response body.
        /// </summary>
        private readonly ResponseBodyPreference responseBodyPreference;

        /// <summary>
        /// The client's preference for what instance annotations to send on the response.
        /// </summary>
        private readonly string annotationFilter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientPreference"/> class.
        /// </summary>
        /// <param name="requestDescription">The request description.</param>
        /// <param name="verb">The request verb.</param>
        /// <param name="requestMessage">The request message.</param>
        /// <param name="effectiveMaxResponseVersion">The effective max response version for the request, which is the min of MDSV and MPV.</param>
        public ClientPreference(RequestDescription requestDescription, HttpVerbs verb, IODataRequestMessage requestMessage, Version effectiveMaxResponseVersion)
            : this(InterpretClientPreference(requestDescription, verb, requestMessage))
        {
            if (effectiveMaxResponseVersion >= VersionUtil.Version3Dot0)
            {
                this.annotationFilter = requestMessage.PreferHeader().AnnotationFilter;
            }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="ClientPreference"/> class from being created.
        /// </summary>
        /// <param name="responseBodyPreference">The response body preference.</param>
        private ClientPreference(ResponseBodyPreference responseBodyPreference)
        {
            this.responseBodyPreference = responseBodyPreference;
        }

        /// <summary>
        /// Client preference for having/not-having a response body.
        /// </summary>
        private enum ResponseBodyPreference
        {
            /// <summary>
            /// No client preference was honored, default behavior.
            /// </summary>
            None,

            /// <summary>
            /// Client asked for no-content.
            /// </summary>
            NoContent,

            /// <summary>
            /// Client asked for content.
            /// </summary>
            Content
        }

        /// <summary>
        /// Gets a value indicating whether the client has a preference for whether or not to include a response body.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the client has a preference for whether or not to include a response body; otherwise, <c>false</c>.
        /// </value>
        public bool HasResponseBodyPreference
        {
            get { return this.responseBodyPreference != ResponseBodyPreference.None; }
        }

        /// <summary>
        /// Gets a value indicating that a the client prefers a response body.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the client prefers a response body; otherwise, <c>false</c>.
        /// </value>
        public bool ShouldIncludeResponseBody
        {
            get { return this.responseBodyPreference == ResponseBodyPreference.Content; }
        }

        /// <summary>
        /// Gets a value indicating that a the client prefers no response body.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the client prefers no response body; otherwise, <c>false</c>.
        /// </value>
        public bool ShouldNotIncludeResponseBody
        {
            get { return this.responseBodyPreference == ResponseBodyPreference.NoContent; }
        }

        /// <summary>
        /// The client's preference for what annotations to send on the response.
        /// </summary>
        public string AnnotationFilter
        {
            get { return this.annotationFilter; }
        }

        /// <summary>
        /// Gets the required response version based on whether preferences were applied.
        /// </summary>
        internal Version RequiredResponseVersion
        {
            get
            {
                if (this.HasResponseBodyPreference || !string.IsNullOrEmpty(this.AnnotationFilter))
                {
                    return VersionUtil.Version3Dot0;
                }

                return VersionUtil.Version1Dot0;
            }
        }

        /// <summary>
        /// Interprets the client preference for having a response body.
        /// </summary>
        /// <param name="requestDescription">The request description.</param>
        /// <param name="verb">The request verb.</param>
        /// <param name="requestMessage">The request message.</param>
        /// <returns>An enum representation of the client's preference.</returns>
        private static ResponseBodyPreference InterpretClientPreference(RequestDescription requestDescription, HttpVerbs verb, IODataRequestMessage requestMessage)
        {
            Debug.Assert(requestDescription != null, "requestDescription != null");

            // If no responseBodyPreference given, we have default behavior of producing content for POST and not producing it for PUT/MERGE.
            // If responseBodyPreference is given we honor the responseBodyPreference only if the request was for an entity and following conditions are true:
            // This is not a service operation invoke
            // DSV was set to 3.0 and above
            // Server is configured to be >= 3.0
            if (requestDescription.LinkUri || requestDescription.SegmentInfos[0].TargetSource == RequestTargetSource.ServiceOperation || requestDescription.RequestVersion < VersionUtil.Version3Dot0)
            {
                return ResponseBodyPreference.None;
            }

            if ((verb.IsInsert()) || ((verb.IsUpdate()) && (requestDescription.TargetKind == RequestTargetKind.Resource || requestDescription.IsRequestForNonEntityProperty)))
            {
                if (requestMessage.PreferHeader().ReturnContent.HasValue)
                {
                    return requestMessage.PreferHeader().ReturnContent.Value ? ResponseBodyPreference.Content : ResponseBodyPreference.NoContent;
                }
            }

            // TODO: move logic for when/when-not-to write a response body here and remove all checks for 'none' elsewhere
            return ResponseBodyPreference.None;
        }
    }
}

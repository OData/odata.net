//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Implementation of the batch-specific URL resolver that resolves cross-referencing URLs properly.
    /// </summary>
    internal sealed class ODataBatchUrlResolver : IODataUrlResolver
    {
        /// <summary>The URL resolver from the batch message.</summary>
        private readonly IODataUrlResolver batchMessageUrlResolver;

        /// <summary>A hashset with all content IDs used so far in the batch; this is used for cross-referencing URL resolution.</summary>
        private HashSet<string> contentIdCache;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="batchMessageUrlResolver">The URL resolver from the batch message.</param>
        internal ODataBatchUrlResolver(IODataUrlResolver batchMessageUrlResolver)
        {
            this.batchMessageUrlResolver = batchMessageUrlResolver;
        }

        /// <summary>
        /// The URL resolver from the batch message.
        /// </summary>
        internal IODataUrlResolver BatchMessageUrlResolver
        {
            get
            {
                return this.batchMessageUrlResolver;
            }
        }

        /// <summary>
        /// Method to implement a custom URL resolution scheme.
        /// This method returns null if not custom resolution is desired.
        /// If the method returns a non-null URL that value will be used without further validation.
        /// </summary>
        /// <param name="baseUri">The (optional) base URI to use for the resolution.</param>
        /// <param name="payloadUri">The URI read from the payload.</param>
        /// <returns>
        /// A <see cref="Uri"/> instance that reflects the custom resolution of the method arguments
        /// into a URL or null if no custom resolution is desired; in that case the default resolution is used.
        /// </returns>
        Uri IODataUrlResolver.ResolveUrl(Uri baseUri, Uri payloadUri)
        {
            ExceptionUtils.CheckArgumentNotNull(payloadUri, "payloadUri");

            if (this.contentIdCache != null && !payloadUri.IsAbsoluteUri)
            {
                // On relative URIs none of the properties except for OriginalString, IsAbsoluteUri and UserEscaped are allowed
                string originalString = UriUtils.UriToString(payloadUri);
                Debug.Assert(originalString != null, "Original strings cannot be null in System.Uri.");

                if (originalString.Length > 0 && originalString[0] == '$')
                {
                    // if the original strings starts with '$' find the first '/' and see whether
                    // the characters before it are recognized as content ID token
                    string tokenString;
                    int ix = originalString.IndexOf('/', 1);
                    if (ix > 0)
                    {
                        tokenString = originalString.Substring(1, ix - 1);
                    }
                    else
                    {
                        tokenString = originalString.Substring(1);
                    }

                    if (this.contentIdCache.Contains(tokenString))
                    {
                        // We found a valid content ID cross reference; return the payload URI unchanged.
                        return payloadUri;
                    }
                }
            }

            if (this.batchMessageUrlResolver != null)
            {
                // If we have a resolver from the batch message use it as the fallback.
                return this.batchMessageUrlResolver.ResolveUrl(baseUri, payloadUri);
            }

            // Otherwise return null to use the default URL resolution instead.
            return null;
        }

        /// <summary>
        /// Add the content ID to the hashset of valid content IDs.
        /// </summary>
        /// <param name="contentId">The (non-null) content ID to add.</param>
        internal void AddContentId(string contentId)
        {
            Debug.Assert(contentId != null, "contentId != null");

            if (this.contentIdCache == null)
            {
                this.contentIdCache = new HashSet<string>(StringComparer.Ordinal);
            }

            Debug.Assert(!this.contentIdCache.Contains(contentId), "Should have verified that we don't see duplicate content IDs.");
            this.contentIdCache.Add(contentId);
        }

        /// <summary>
        /// Checks whether a given (non-null) content ID is already in the content ID cache.
        /// </summary>
        /// <param name="contentId">The content ID to check for.</param>
        /// <returns>true if the content ID cache already contains a content ID with value <paramref name="contentId"/>; otherwise false.</returns>
        internal bool ContainsContentId(string contentId)
        {
            Debug.Assert(contentId != null, "contentId != null");

            if (this.contentIdCache == null)
            {
                return false;
            }

            return this.contentIdCache.Contains(contentId);
        }

        /// <summary>
        /// Resets the cache of content IDs. This is called at the end of each changeset
        /// since content IDs are only unique within a changeset.
        /// </summary>
        internal void Reset()
        {
            if (this.contentIdCache != null)
            {
                this.contentIdCache.Clear();
            }
        }
    }
}

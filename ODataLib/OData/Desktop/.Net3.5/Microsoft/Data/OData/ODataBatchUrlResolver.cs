//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData
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
            DebugUtils.CheckNoExternalCallers();

            this.batchMessageUrlResolver = batchMessageUrlResolver;
        }

        /// <summary>
        /// The URL resolver from the batch message.
        /// </summary>
        internal IODataUrlResolver BatchMessageUrlResolver
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();

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
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(payloadUri, "payloadUri");

            if (this.contentIdCache != null && !payloadUri.IsAbsoluteUri)
            {
                // On relative URIs none of the properties except for OriginalString, IsAbsoluteUri and UserEscaped are allowed
                string originalString = UriUtilsCommon.UriToString(payloadUri);
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();

            if (this.contentIdCache != null)
            {
                this.contentIdCache.Clear();
            }
        }
    }
}

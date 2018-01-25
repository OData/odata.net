//---------------------------------------------------------------------
// <copyright file="ODataBatchPayloadUriConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Implementation of the batch-specific URL converter that converts cross-referencing URLs properly.
    /// </summary>
    internal sealed class ODataBatchPayloadUriConverter : IODataPayloadUriConverter
    {
        /// <summary>The URL converter from the batch message.</summary>
        private readonly IODataPayloadUriConverter batchMessagePayloadUriConverter;

        /// <summary>A hashset with all content IDs used so far in the batch; this is used for cross-referencing URL conversion.</summary>
        private HashSet<string> contentIdCache;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="batchMessagePayloadUriConverter">The URL converter from the batch message.</param>
        internal ODataBatchPayloadUriConverter(IODataPayloadUriConverter batchMessagePayloadUriConverter)
        {
            this.batchMessagePayloadUriConverter = batchMessagePayloadUriConverter;
        }

        /// <summary>
        /// The URL converter from the batch message.
        /// </summary>
        internal IODataPayloadUriConverter BatchMessagePayloadUriConverter
        {
            get
            {
                return this.batchMessagePayloadUriConverter;
            }
        }

        /// <summary>
        /// A read-only enumeration of <code>contentIdCache</code>.
        /// </summary>
        internal IEnumerable<string> ContentIdCache
        {
            get { return this.contentIdCache; }
        }

        /// <summary>
        /// Method to implement a custom URL conversion scheme.
        /// This method returns null if not custom conversion is desired.
        /// If the method returns a non-null URL that value will be used without further validation.
        /// </summary>
        /// <param name="baseUri">The (optional) base URI to use for the conversion.</param>
        /// <param name="payloadUri">The URI read from the payload.</param>
        /// <returns>
        /// A <see cref="Uri"/> instance that reflects the custom conversion of the method arguments
        /// into a URL or null if no custom conversion is desired; in that case the default conversion is used.
        /// </returns>
        Uri IODataPayloadUriConverter.ConvertPayloadUri(Uri baseUri, Uri payloadUri)
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

            if (this.batchMessagePayloadUriConverter != null)
            {
                // If we have a converter from the batch message use it as the fallback.
                return this.batchMessagePayloadUriConverter.ConvertPayloadUri(baseUri, payloadUri);
            }

            // Otherwise return null to use the default URL conversion instead.
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

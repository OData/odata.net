//---------------------------------------------------------------------
// <copyright file="BatchPayloadBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Utility class for building batch payloads
    /// </summary>
    public static class BatchPayloadBuilder
    {
        /// <summary>
        /// ContentId to use in batch payload
        /// </summary>
        private static int contentId = 0;

        /// <summary>
        /// Builds a request changeset with the given information
        /// </summary>
        /// <param name="boundary">The changeset's boundary</param>
        /// <param name="charset">The changeset's character set</param>
        /// <param name="parts">The parts of the changeset</param>
        /// <returns>The request changeset</returns>
        public static BatchRequestChangeset RequestChangeset(string boundary, string charset, params IMimePart[] parts)
        {
            return Changeset<BatchRequestChangeset, IHttpRequest>(boundary, charset, () => new BatchRequestChangeset(), parts);
        }

        /// <summary>
        /// Builds a response changeset with the given information
        /// </summary>
        /// <param name="boundary">The changeset's boundary</param>
        /// <param name="charset">The changeset's character set</param>
        /// <param name="parts">The parts of the changeset</param>
        /// <returns>The response changeset</returns>
        public static BatchResponseChangeset ResponseChangeset(string boundary, string charset, params IMimePart[] parts)
        {
            return Changeset<BatchResponseChangeset, HttpResponseData>(boundary, charset, () => new BatchResponseChangeset(), parts);
        }

        /// <summary>
        /// Wraps the given request with default content-type and transfer-encoding headers so that it can be added to a batch
        /// </summary>
        /// <param name="request">The request</param>
        /// <returns>The fragment with default values for its batch-specific headers</returns>
        public static MimePartData<IHttpRequest> AsBatchFragment(this IHttpRequest request)
        {
            return request.AsBatchFragment<IHttpRequest>();
        }

        /// <summary>
        /// Wraps the given response with default content-type and transfer-encoding headers so that it can be added to a batch
        /// </summary>
        /// <param name="response">The response</param>
        /// <returns>The fragment with default values for its batch-specific headers</returns>
        public static MimePartData<HttpResponseData> AsBatchFragment(this HttpResponseData response)
        {
            return response.AsBatchFragment<HttpResponseData>();
        }
        
        private static TChangeset Changeset<TChangeset, TOperation>(string boundary, string charset, Func<TChangeset> createChangeset, params IMimePart[] parts)
            where TChangeset : BatchChangeset<TOperation>
            where TOperation : class, IMimePart
        {
            ExceptionUtilities.CheckCollectionDoesNotContainNulls(parts, "parts");

            var changeset = createChangeset();
            changeset.Headers[HttpHeaders.ContentType] = HttpUtilities.BuildContentType(MimeTypes.MultipartMixed, charset, boundary);

            foreach (var part in parts)
            {
                var wrappedRequest = part as MimePartData<TOperation>;
                if (wrappedRequest != null)
                {
                    changeset.Add(wrappedRequest);
                }
                else
                {
                    var operation = part as TOperation;
                    ExceptionUtilities.CheckObjectNotNull(operation, "Given part was of unexpected type '{0}'", part.GetType());
                    string contentId;
                    if (operation.Headers.TryGetValue(HttpHeaders.ContentId, out contentId))
                    {
                        operation.Headers.Remove(HttpHeaders.ContentId);
                    }

                    changeset.Add(operation.AsBatchFragment(contentId));
                }
            }

            return changeset;
        }

        private static MimePartData<TPart> AsBatchFragment<TPart>(this TPart part, string contentId = null) where TPart : IMimePart
        {
            ExceptionUtilities.CheckArgumentNotNull(part, "part");

            return new MimePartData<TPart>()
            {
                Headers =
                {
                    { HttpHeaders.ContentType, MimeTypes.ApplicationHttp },
                    { HttpHeaders.ContentTransferEncoding, "binary" },
                    { HttpHeaders.ContentId, contentId ?? (++BatchPayloadBuilder.contentId).ToString() },
                },
                Body = part
            };
        }
    }
}

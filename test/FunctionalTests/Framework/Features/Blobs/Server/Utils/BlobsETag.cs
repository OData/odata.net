//---------------------------------------------------------------------
// <copyright file="BlobsETag.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Net;

namespace System.Data.Test.Astoria
{
    //---------------------------------------------------------------------
    // Extends BlobsRequest with ETag verification.
    //---------------------------------------------------------------------
    public partial class BlobsRequest
    {
        const string unknownETag = "W/\"'something'\"";
        protected internal static string ETagMLE = unknownETag; // updated after each request
        protected internal static string ETagMRR = unknownETag; // updated in MediaLinkEntry.Create()

        //---------------------------------------------------------------------
        // Sends and verifies requests with matching and non-matching ETags.
        //---------------------------------------------------------------------
        public void TestETag(bool mle)
        {
            // Save current state.
            HttpStatusCode normalStatusCode = base.ExpectedStatusCode;
            bool normalETagExpected = base.ETagHeaderExpected;

            // Choose ETag to work with.
            string etag = mle ? ETagMLE : ETagMRR;

            switch (Verb)
            {
                // POST
                case RequestVerb.Post:

                    // All variations should fail.
                    base.ExpectedStatusCode = HttpStatusCode.BadRequest;
                    base.ETagHeaderExpected = false;
                    SendAndVerify(null, ConcurrencyUtil.IfMatchHeader, unknownETag);
                    SendAndVerify(null, ConcurrencyUtil.IfMatchHeader, etag);
                    SendAndVerify(null, ConcurrencyUtil.IfNoneMatchHeader, unknownETag);
                    SendAndVerify(null, ConcurrencyUtil.IfNoneMatchHeader, etag);
                    break;

                // GET
                case RequestVerb.Get:

                    // ETag checks expected to fail.
                    base.ExpectedStatusCode = HttpStatusCode.PreconditionFailed;
                    base.ETagHeaderExpected = false;
                    SendAndVerify(null, ConcurrencyUtil.IfMatchHeader, unknownETag);
                    base.ExpectedStatusCode = HttpStatusCode.NotModified;
                    base.ETagHeaderExpected = true;
                    SendAndVerify(null, ConcurrencyUtil.IfNoneMatchHeader, etag);

                    // ETag checks expected to succeed.
                    base.ExpectedStatusCode = normalStatusCode;
                    base.ETagHeaderExpected = true;
                    SendAndVerify(null, ConcurrencyUtil.IfMatchHeader, etag);
                    SendAndVerify(null, ConcurrencyUtil.IfNoneMatchHeader, unknownETag);
                    break;

                // PUT and PATCH
                case RequestVerb.Put:
                case RequestVerb.Patch:

                    // ETag checks expected to fail.
                    base.ExpectedStatusCode = HttpStatusCode.PreconditionFailed;
                    base.ETagHeaderExpected = false;
                    SendAndVerify(null, ConcurrencyUtil.IfMatchHeader, unknownETag);
                    base.ExpectedStatusCode = HttpStatusCode.BadRequest;
                    SendAndVerify(null, ConcurrencyUtil.IfNoneMatchHeader, etag);
                    SendAndVerify(null, ConcurrencyUtil.IfNoneMatchHeader, unknownETag);

                    // ETag checks expected to succeed.
                    base.ExpectedStatusCode = normalStatusCode;
                    base.ETagHeaderExpected = true;
                    SendAndVerify(null, ConcurrencyUtil.IfMatchHeader, etag);
                    break;

                // DELETE
                case RequestVerb.Delete:

                    // ETag checks expected to fail.
                    base.ExpectedStatusCode = HttpStatusCode.PreconditionFailed;
                    base.ETagHeaderExpected = false;
                    SendAndVerify(null, ConcurrencyUtil.IfMatchHeader, unknownETag);
                    base.ExpectedStatusCode = HttpStatusCode.BadRequest;
                    SendAndVerify(null, ConcurrencyUtil.IfNoneMatchHeader, etag);
                    SendAndVerify(null, ConcurrencyUtil.IfNoneMatchHeader, unknownETag);

                    // Changes expected.
                    base.ExpectedStatusCode = normalStatusCode;
                    base.ETagHeaderExpected = false;
                    SendAndVerify(null, ConcurrencyUtil.IfMatchHeader, etag);
                    break;

                // UNKNOWN VERB
                default:
                    AstoriaTestLog.FailAndThrow("Test issue - unknown request verb (1)");
                    break;
            }

            // Restore original state.
            base.ExpectedStatusCode = normalStatusCode;
            base.ETagHeaderExpected = normalETagExpected;
        }
    }
}
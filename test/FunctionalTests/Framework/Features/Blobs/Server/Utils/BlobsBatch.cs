//---------------------------------------------------------------------
// <copyright file="BlobsBatch.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    //---------------------------------------------------------------------
    // Holds sequence of BlobsRequests.
    //---------------------------------------------------------------------
    public class BlobsBatch : BatchRequest
    {
        //---------------------------------------------------------------------
        public BlobsBatch(Workspace w) : base(w) { }

        //---------------------------------------------------------------------
        public BatchResponse SendAndVerify(params AstoriaRequest[] requests)
        {
            // Populate batch.
            BatchChangeset cs = base.GetChangeset();
            foreach (var r in requests)
            {
                if (r == null) // Begin new changeset.
                    cs = base.GetChangeset();
                else
                {
                    // Expect no dummy etags from stream provider for batched requests.
                    r.ETagHeaderExpected = false;

                    if (r.Verb == RequestVerb.Get)  // Add request to batch.
                    {
                        r.ContentType = null;
                        base.Add(r, true);
                    }
                    else // Add request to current changeset.
                    {
                        if (r.Verb == RequestVerb.Delete)
                            r.ContentType = null;
                        cs.Add(r, true);
                    }
                }
            }

            // Send and verify batch request. SendAndVerify() logic not applied.
            BatchResponse response = base.GetResponse() as BatchResponse;
            response.Verify();
            return response;
        }
    }
}

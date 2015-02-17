//---------------------------------------------------------------------
// <copyright file="BlobsAccess.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Net;

namespace System.Data.Test.Astoria
{
    //---------------------------------------------------------------------
    // Extends BlobsRequest with entity set access rights verification.
    //---------------------------------------------------------------------
    public partial class BlobsRequest
    {
        //---------------------------------------------------------------------
        // Sends and verifies request for all possible access rights.
        //---------------------------------------------------------------------
        public void TestAccess(string container)
        {
            // Save current state.
            HttpStatusCode normalStatusCode = ExpectedStatusCode;

#if !ClientSKUFramework
            for (uint i = 0; i <= (uint)Microsoft.OData.Service.EntitySetRights.All; i++)
            {
                // Set entity set access right.
                Workspace.DataService.ConfigSettings.SetEntitySetAccessRule(container, (Microsoft.OData.Service.EntitySetRights)i);

                // Calculate minimum access needed for request to succeed.
                Microsoft.OData.Service.EntitySetRights minAccess = Microsoft.OData.Service.EntitySetRights.All;
                switch (Verb)
                {


                    case RequestVerb.Post: minAccess = Microsoft.OData.Service.EntitySetRights.WriteAppend; break;
                    case RequestVerb.Get: minAccess = Microsoft.OData.Service.EntitySetRights.ReadSingle; break;
                    case RequestVerb.Put: minAccess = Microsoft.OData.Service.EntitySetRights.WriteReplace | Microsoft.OData.Service.EntitySetRights.ReadSingle; break;
                    case RequestVerb.Patch: minAccess = Microsoft.OData.Service.EntitySetRights.WriteMerge | Microsoft.OData.Service.EntitySetRights.ReadSingle; break;
                    case RequestVerb.Delete: minAccess = Microsoft.OData.Service.EntitySetRights.WriteDelete | Microsoft.OData.Service.EntitySetRights.ReadSingle; break;

                    default:
                        AstoriaTestLog.FailAndThrow("Test issue - unknown request verb (2)");
                        break;
                }

                // Calculate expected status code.
                ExpectedStatusCode =
                    i == 0 ? HttpStatusCode.NotFound
                    : (i & (uint)minAccess) == (uint)minAccess ? normalStatusCode : HttpStatusCode.Forbidden;

                // Send and verify request.
                AstoriaResponse response = SendAndVerify(null);

                // Delete the entity if it was created to avoid "key already exists" collisions.
                if (response.Headers.ContainsKey("Location"))
                {
                    string newEntity = response.Headers["Location"];

                    Workspace.DataService.ConfigSettings.SetEntitySetAccessRule(container, Microsoft.OData.Service.EntitySetRights.WriteDelete | Microsoft.OData.Service.EntitySetRights.ReadSingle);


                    BlobsRequest.MLE(Workspace, Format, RequestVerb.Delete, null, HttpStatusCode.NoContent, newEntity).SendAndVerify(null);
                }
            }
#endif


            // Restore previous state.
            ExpectedStatusCode = normalStatusCode;
#if !ClientSKUFramework
            Workspace.DataService.ConfigSettings.SetEntitySetAccessRule(container, Microsoft.OData.Service.EntitySetRights.All);
#endif


        }
    }
}
//---------------------------------------------------------------------
// <copyright file="DeleteVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Test.ModuleCore;
using System.Net;
using AstoriaUnitTests.Data;
using System.Text.RegularExpressions;

namespace System.Data.Test.Astoria
{
    public class DeleteVerifier : PayloadVerifier
    {
        public static void Verify(AstoriaResponse response)
        {
            DeleteVerifier verifier = new DeleteVerifier(response);
            verifier.Verify();
        }

        public static bool Applies(AstoriaResponse response)
        {
            if (response.Request.EffectiveVerb != RequestVerb.Delete)
                return false;
            if (response.Request is BlobsRequest)
                return false;
            return true;
        }

        private DeleteVerifier(AstoriaResponse response)
            : base(response)
        { }

        protected override void Verify()
        {
            if (!Applies(Response))
                return;

            AstoriaRequest request = Response.Request;

            ResourceProperty property = request.GetPropertyFromQuery();
            if (property == null)
                RequestUtil.GetAndVerifyStatusCode(Response.Workspace, request.URI, System.Net.HttpStatusCode.NotFound);
            else
            {
                if (property.IsNavigation && property.OtherAssociationEnd.Multiplicity != Multiplicity.One)
                    RequestUtil.GetAndVerifyStatusCode(Response.Workspace, request.URI, System.Net.HttpStatusCode.NoContent);
                else
                    // TODO: is this always right if its not a nav prop?
                    RequestUtil.GetAndVerifyStatusCode(Response.Workspace, request.URI, System.Net.HttpStatusCode.NotFound);
            }
        }
    }
}

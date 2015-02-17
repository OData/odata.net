//---------------------------------------------------------------------
// <copyright file="ODataRequestExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// A set of useful extension methods for working with OData requests
    /// </summary>
    public static class ODataRequestExtensions
    {
        private static readonly HttpVerb[] applicableVerbsForPrefer = new[] { HttpVerb.Post, HttpVerb.Put, HttpVerb.Patch };

        /// <summary>
        /// Checks if the server will process the prefer header for the given request
        /// </summary>
        /// <param name="request">the request</param>
        /// <param name="maxProtocolVersion">The max protocol version of the server</param>
        /// <returns>true if the server will process the prefer header for the given request</returns>
        public static bool ResponsePayloadExpected(this ODataRequest request, DataServiceProtocolVersion maxProtocolVersion)
        {
            HttpVerb effectiveVerb = request.GetEffectiveVerb();

            if (effectiveVerb == HttpVerb.Delete)
            {
                return false;
            }
            else if (effectiveVerb == HttpVerb.Post)
            {
                // TODO: need to deal with Service operations when then come in
                if (request.Uri.IsEntityReferenceLink() || (request.PreferHeaderApplies(maxProtocolVersion) && request.GetHeaderValueIfExists(HttpHeaders.Prefer) == HttpHeaders.ReturnNoContent))
                {
                    return false;
                }

                return true;
            }
            else if (effectiveVerb.IsUpdateVerb())
            {
                if (request.PreferHeaderApplies(maxProtocolVersion) && request.GetHeaderValueIfExists(HttpHeaders.Prefer) == HttpHeaders.ReturnContent)
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the server will process the prefer header for the given request
        /// </summary>
        /// <param name="request">the request</param>
        /// <param name="maxProtocolVersion">The max protocol version of the server</param>
        /// <returns>true if the server will process the prefer header for the given request</returns>
        public static bool PreferHeaderApplies(this ODataRequest request, DataServiceProtocolVersion maxProtocolVersion)
        {
            // the header should only apply if:
            // 1) the effective version of the request is >= 3
            // 2) the request verb is one of: POST, PUT, PATCH
            // 3) the uri refers to a single entity for PUT/PATCH, or an entity set for POST
            var effectiveVersion = VersionHelper.GetEffectiveProtocolVersion(request.Headers, maxProtocolVersion);

            if (effectiveVersion < DataServiceProtocolVersion.V4 || maxProtocolVersion < DataServiceProtocolVersion.V4)
            {
                return false;
            }

            return request.PreferHeaderApplies();
        }

        /// <summary>
        /// Checks if the server will process the prefer header for the given request, without regard to versioning
        /// </summary>
        /// <param name="request">the request</param>
        /// <returns>true if the server will process the prefer header for the given request</returns>
        private static bool PreferHeaderApplies(this ODataRequest request)
        {
            // the header should only apply if:
            // 1) the request verb is one of: POST, PUT, PATCH
            // 2) the uri refers to a single entity or property for PUT/PATCH, or an entity set for POST
            var verb = request.GetEffectiveVerb();

            if (!applicableVerbsForPrefer.Contains(verb))
            {
                return false;
            }

            if (verb == HttpVerb.Post)
            {
                return request.Uri.IsEntitySet();
            }
            else
            {
                return request.Uri.IsEntity() || request.Uri.IsProperty() || request.Uri.IsPropertyValue();
            }
        }
    }
}

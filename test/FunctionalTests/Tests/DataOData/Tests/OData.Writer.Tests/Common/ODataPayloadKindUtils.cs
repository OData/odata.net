//---------------------------------------------------------------------
// <copyright file="ODataPayloadKindUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Common
{
    #region Namespaces
    using Microsoft.OData;
    #endregion Namespaces

    public static class ODataPayloadKindUtils
    {
        public static bool IsSupported(this ODataPayloadKind payloadKind, WriterTestConfiguration testConfiguration)
        {
            if (payloadKind == ODataPayloadKind.Unsupported)
            {
                return false;
            }

            if (testConfiguration.IsRequest)
            {
                // some payloads are not supported in requests
                if (payloadKind == ODataPayloadKind.Collection ||
                    payloadKind == ODataPayloadKind.Error ||
                    payloadKind == ODataPayloadKind.ServiceDocument ||
                    payloadKind == ODataPayloadKind.MetadataDocument ||
                    // Top-level EntityReferenceLinks payload write requests are not allowed.
                    payloadKind == ODataPayloadKind.EntityReferenceLinks)
                {
                    return false;
                }
            }
            else
            {
                // Parameter payload is not supported in responses.
                if (payloadKind == ODataPayloadKind.Parameter)
                {
                    return false;
                }
            }

            if (payloadKind == ODataPayloadKind.MetadataDocument && !testConfiguration.Synchronous)
            {
                // Metadata writing is only supported in synchronous scenarios
                return false;
            }

            if (payloadKind == ODataPayloadKind.Batch ||
                payloadKind == ODataPayloadKind.Value ||
                payloadKind == ODataPayloadKind.BinaryValue ||
                payloadKind == ODataPayloadKind.MetadataDocument)
            {
                if (testConfiguration.Format != null)
                {
                    return false;
                }
            }

            if (payloadKind == ODataPayloadKind.Parameter && (testConfiguration.Format != ODataFormat.Json))
            {
                return false;
            }

            return true;
        }
    }
}
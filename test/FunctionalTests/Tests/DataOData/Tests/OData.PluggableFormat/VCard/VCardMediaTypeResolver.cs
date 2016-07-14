//---------------------------------------------------------------------
// <copyright file="VCardMediaTypeResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.PluggableFormat.VCard
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;

    public class VCardMediaTypeResolver : ODataMediaTypeResolver
    {
        private readonly ODataMediaTypeFormat[] mediaTypeFormats = new ODataMediaTypeFormat[]
        {
            new ODataMediaTypeFormat(new ODataMediaType("text", "x-vCard"), new VCardFormat())
        };

        public override IEnumerable<ODataMediaTypeFormat> GetMediaTypeFormats(ODataPayloadKind payloadKind)
        {
            if (payloadKind == ODataPayloadKind.Resource
                || payloadKind == ODataPayloadKind.Property)
            {
                return mediaTypeFormats.Concat(base.GetMediaTypeFormats(payloadKind));
            }

            return base.GetMediaTypeFormats(payloadKind);
        }
    }
}

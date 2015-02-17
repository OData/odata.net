//---------------------------------------------------------------------
// <copyright file="VCardMediaTypeResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.PluggableFormat.VCard
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core;

    public class VCardMediaTypeResolver : ODataMediaTypeResolver
    {
        private static readonly VCardMediaTypeResolver instance = new VCardMediaTypeResolver();
        private readonly ODataMediaTypeFormat[] mediaTypeFormats = new ODataMediaTypeFormat[]
        {
            new ODataMediaTypeFormat(new ODataMediaType("text", "x-vCard"), new VCardFormat())
        };

        private VCardMediaTypeResolver() { }

        public static VCardMediaTypeResolver Instance { get { return instance; } }

        public override IEnumerable<ODataMediaTypeFormat> GetMediaTypeFormats(ODataPayloadKind payloadKind)
        {
            if (payloadKind == ODataPayloadKind.Property)
            {
                return mediaTypeFormats.Concat(base.GetMediaTypeFormats(payloadKind));
            }

            return base.GetMediaTypeFormats(payloadKind);
        }
    }
}

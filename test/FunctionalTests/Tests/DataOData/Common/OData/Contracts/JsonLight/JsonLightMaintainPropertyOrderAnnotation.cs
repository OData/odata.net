//---------------------------------------------------------------------
// <copyright file="JsonLightMaintainPropertyOrderAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts.JsonLight
{
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;

    /// <summary>
    /// Indicates to the test serializer that the resultant json light properties 
    /// should not be reordered.
    /// </summary>
    public class JsonLightMaintainPropertyOrderAnnotation : ODataPayloadElementAnnotation
    {
        public override string StringRepresentation
        {
            get { return "(do not reorder properties)"; }
        }

        public override ODataPayloadElementAnnotation Clone()
        {
            return new JsonLightMaintainPropertyOrderAnnotation();
        }
    }
}

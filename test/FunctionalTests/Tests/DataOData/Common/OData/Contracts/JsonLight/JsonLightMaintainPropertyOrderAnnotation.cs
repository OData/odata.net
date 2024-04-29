//---------------------------------------------------------------------
// <copyright file="JsonMaintainPropertyOrderAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts.Json
{
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;

    /// <summary>
    /// Indicates to the test serializer that the resultant json light properties 
    /// should not be reordered.
    /// </summary>
    public class JsonMaintainPropertyOrderAnnotation : ODataPayloadElementAnnotation
    {
        public override string StringRepresentation
        {
            get { return "(do not reorder properties)"; }
        }

        public override ODataPayloadElementAnnotation Clone()
        {
            return new JsonMaintainPropertyOrderAnnotation();
        }
    }
}

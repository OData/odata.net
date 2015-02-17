//---------------------------------------------------------------------
// <copyright file="ODataPayloadJsonNormalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;

    /// <summary>
    /// Normalizer for handling JSON specific manipulations for ODataPayloadElement before serialization. 
    /// </summary>
    public class ODataPayloadJsonNormalizer : ODataPayloadElementVisitorBase, IODataPayloadElementVisitor
    {
        /// <summary>
        /// Remove fulltype names on PrimitiveValue 
        /// </summary>
        /// <param name="payloadElement">payloadElement to remove the type names from</param>
        public override void Visit(PrimitiveValue payloadElement)
        {
            payloadElement.FullTypeName = null;
        }
    }
}


//---------------------------------------------------------------------
// <copyright file="NullStringTypenameFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Fixups
{
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// As ODataLib is expected to not write typename when type is string this visitor updates all primitive values 
    /// of type Edm.String to have a typename of null
    /// </summary>
    public class NullStringTypenameFixup : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(PrimitiveValue payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            if (payloadElement.FullTypeName == "Edm.String")
            {
                payloadElement.FullTypeName = null;
            }
        }
    }
}

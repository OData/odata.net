//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementNullIDVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Fixups
{
    using System;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Converts any entry with a null ID to string.empty 
    /// </summary>
    public class ODataPayloadElementNullIDVisitor : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EntityInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            if (payloadElement.Id == null)
            {
                payloadElement.Id = String.Empty;
            }
            base.Visit(payloadElement);
        }       
    }
}

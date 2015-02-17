//---------------------------------------------------------------------
// <copyright file="RemoveComplexWithNoProperties.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Fixups
{
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;

    public class RemoveComplexWithNoProperties : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// Visits the entity instance and removes any complex with no properties as this will not be written.
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EntityInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            foreach (var property in payloadElement.Properties.ToList())
            {
                ComplexProperty complex = property as ComplexProperty;
                if (complex != null && complex.Value.Properties.Count() == 0)
                {
                    payloadElement.Remove(complex);
                }
            }

            base.Visit(payloadElement);
        }
    }
}

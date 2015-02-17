//---------------------------------------------------------------------
// <copyright file="ReorderProperties.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Fixups
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Reorders properties to put the nav links first which are always read back first in JSON
    /// </summary>
    public class ReorderProperties : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EntityInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            var properties = new List<PropertyInstance>();
            var navprops = new List<PropertyInstance>();
            foreach (var propertyInstance in payloadElement.Properties)
            {
                this.Recurse(propertyInstance);
                var isnavprop = propertyInstance as NavigationPropertyInstance;
                if (isnavprop != null)
                {
                    navprops.Add(propertyInstance);
                }
                else
                {
                    properties.Add(propertyInstance);
                }
            }
            payloadElement.Properties = navprops.Concat(properties);

            foreach (var serviceOperationDescriptor in payloadElement.ServiceOperationDescriptors)
            {
                this.Recurse(serviceOperationDescriptor);
            }
        }
    }
}

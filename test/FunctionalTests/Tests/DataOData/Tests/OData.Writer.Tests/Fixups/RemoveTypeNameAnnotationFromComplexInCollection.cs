//---------------------------------------------------------------------
// <copyright file="RemoveTypeNameAnnotationFromComplexInCollection.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Fixups
{
    using System.Collections;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;

    public class RemoveTypeNameAnnotationFromComplexInCollection : ODataPayloadElementVisitorBase
    {
        private bool withinCollection = false;

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ComplexInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            if (this.withinCollection)
            {
                var annotation = payloadElement.Annotations.Where(a => a is SerializationTypeNameTestAnnotation).SingleOrDefault();
                payloadElement.Annotations.Remove(annotation);
            }

            this.withinCollection = false;
            base.Visit(payloadElement);
        }

        /// <summary>
        /// Helper method for visiting collections
        /// </summary>
        /// <param name="payloadElement">The collection to visit</param>
        protected override void VisitCollection(ODataPayloadElementCollection payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            var enumerable = payloadElement as IEnumerable;
            if (enumerable != null)
            {
                foreach (var element in enumerable.Cast<ODataPayloadElement>())
                {
                    this.withinCollection = true;
                    this.Recurse(element);
                }
            }

            this.withinCollection = false;
        }
    }
}

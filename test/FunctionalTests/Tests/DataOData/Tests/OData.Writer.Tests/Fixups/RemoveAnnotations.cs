//---------------------------------------------------------------------
// <copyright file="RemoveAnnotations.cs" company="Microsoft">
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

    /// <summary>
    /// Removes annotations added by the metadata resolver
    /// </summary>
    public class RemoveAnnotations : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// Visits the entity set and removes the title and entity set annotations
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(BatchRequestPayload payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            
            var annotation = payloadElement.Annotations.Where(a => a is BatchBoundaryAnnotation).SingleOrDefault();
            payloadElement.Annotations.Remove(annotation);
            base.Visit(payloadElement);
        }

        /// <summary>
        /// Visits the entity set and removes the title and entity set annotations
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(BatchResponsePayload payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            var annotation = payloadElement.Annotations.Where(a => a is BatchBoundaryAnnotation).SingleOrDefault();
            payloadElement.Annotations.Remove(annotation);
            base.Visit(payloadElement);
            
        }

        /// <summary>
        /// Visits the entity set and removes the title and entity set annotations
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EntitySetInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            var annotation = payloadElement.Annotations.Where(a => a is TitleAnnotation).SingleOrDefault();
            payloadElement.Annotations.Remove(annotation);
            annotation = payloadElement.Annotations.Where(a => a is EntitySetAnnotation).SingleOrDefault();
            payloadElement.Annotations.Remove(annotation);

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Visits the entity instance and removes entity set and data type annotations
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EntityInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            var annotation = payloadElement.Annotations.Where(a => a is EntitySetAnnotation).SingleOrDefault();
            payloadElement.Annotations.Remove(annotation);
            annotation = payloadElement.Annotations.Where(a => a is DataTypeAnnotation).SingleOrDefault();
            payloadElement.Annotations.Remove(annotation);

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ExpandedLink payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var annotation = payloadElement.Annotations.Where(a => a is NavigationPropertyAnnotation).SingleOrDefault();
            payloadElement.Annotations.Remove(annotation);
            if (payloadElement.ExpandedElement != null)
            {
                this.Recurse(payloadElement.ExpandedElement);
            }
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(NavigationPropertyInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var annotation = payloadElement.Annotations.Where(a => a is NavigationPropertyAnnotation).SingleOrDefault();
            payloadElement.Annotations.Remove(annotation);
            
            if (payloadElement.Value != null)
            {
                this.Recurse(payloadElement.Value);
            }
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ComplexMultiValueProperty payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            var annotation = payloadElement.Annotations.Where(a => a is MemberPropertyAnnotation).SingleOrDefault();
            payloadElement.Annotations.Remove(annotation);

            this.VisitProperty(payloadElement, payloadElement.Value);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ComplexMultiValue payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            var annotation = payloadElement.Annotations.Where(a => a is DataTypeAnnotation).SingleOrDefault();
            payloadElement.Annotations.Remove(annotation);

            this.VisitCollection(payloadElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ComplexInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            
            var annotation = payloadElement.Annotations.Where(a => a is DataTypeAnnotation).SingleOrDefault();
            payloadElement.Annotations.Remove(annotation);

            foreach (var propertyInstance in payloadElement.Properties)
            {
                this.Recurse(propertyInstance);
            }
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ComplexProperty payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var annotation = payloadElement.Annotations.Where(a => a is MemberPropertyAnnotation).SingleOrDefault();
            payloadElement.Annotations.Remove(annotation);

            this.VisitProperty(payloadElement, payloadElement.Value);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(PrimitiveProperty payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var annotation = payloadElement.Annotations.Where(a => a is MemberPropertyAnnotation).SingleOrDefault();
            payloadElement.Annotations.Remove(annotation);

            this.VisitProperty(payloadElement, payloadElement.Value);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(PrimitiveValue payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var annotation = payloadElement.Annotations.Where(a => a is DataTypeAnnotation).SingleOrDefault();
            payloadElement.Annotations.Remove(annotation);
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="PathSegmentValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//--------

using System;
using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser.Validation.ValidationEngine
{
    /// <summary>
    /// Walks a path, validating the path segments
    /// </summary>
    internal class PathSegmentValidator : PathSegmentHandler
    {
        private Action<object> ValidateItem;
        private Action<QueryNode> ValidateExpression;

        /// <summary>
        /// Creates an instance of a PathSegmentValidator
        /// </summary>
        /// <param name="context">The validation context used by the path segment validator.</param>
        public PathSegmentValidator(ODataUrlValidationContext context)
        {
            this.ValidateItem = (item) => context.UrlValidator.ValidateItem(item, context);
            this.ValidateExpression = (node) => context.ExpressionValidator.ValidateNode(node);
        }

        /// <summary>
        /// Entry point for validating an <see cref="ODataPathSegment"/>.
        /// </summary>
        /// <param name="segment">The odata path segment to validate</param>
        public void ValidatePath(ODataPathSegment segment)
        {
            segment.HandleWith(this);
        }

        #region Segment Handlers

        /// <summary>
        /// Handle validating a MetadataSegment
        /// </summary>
        /// <param name="segment">The metadata segment to valdiate.</param>
        public override void Handle(MetadataSegment segment) 
        {
            ValidateItem(segment);
        }

        /// <summary>
        /// Handle validating a BatchReferenceSegment
        /// </summary>
        /// <param name="segment">The batch reference segment to valdiate.</param>
        public override void Handle(BatchReferenceSegment segment)
        {
            ValidatePath(segment);
            ValidateItem(segment.EntitySet);
        }

        /// <summary>
        /// Handle validating a BatchSegment
        /// </summary>
        /// <param name="segment">The batch segment to valdiate.</param>
        public override void Handle(BatchSegment segment)
        {
            ValidateItem(segment);
        }

        /// <summary>
        /// Handle validating a ValueSegment
        /// </summary>
        /// <param name="segment">The value segment to valdiate.</param>
        public override void Handle(ValueSegment segment)
        {
            ValidateItem(segment);
        }

        /// <summary>
        /// Handle validating a NavigationPropertyLinkSegment
        /// </summary>
        /// <param name="segment">The navigation reference segment to valdiate.</param>
        public override void Handle(NavigationPropertyLinkSegment segment)
        {
            ValidateItemAndType(segment);
            ValidateItem(segment.NavigationProperty);
        }

        /// <summary>
        /// Handle validating a EachSegment
        /// </summary>
        /// <param name="segment">The each segment to valdiate.</param>
        public override void Handle(EachSegment segment)
        {
            ValidateItem(segment);
        }

        /// <summary>
        /// Handle validating a ReferenceSegment
        /// </summary>
        /// <param name="segment">The reference segment to valdiate.</param>
        public override void Handle(ReferenceSegment segment)
        {
            ValidateItem(segment);
        }

        /// <summary>
        /// Handle validating a FilterSegment
        /// </summary>
        /// <param name="segment">The filter segment to valdiate.</param>
        public override void Handle(FilterSegment segment)
        {
            ValidateItem(segment);
            ValidateExpression(segment.Expression);
        }

        /// <summary>
        /// Handle validating a CountSegment
        /// </summary>
        /// <param name="segment">The count segment to valdiate.</param>
        public override void Handle(CountSegment segment)
        {
            ValidateItem(segment);
        }

        /// <summary>
        /// Handle validating a DynamicPathSegment
        /// </summary>
        /// <param name="segment">The dynamic path segment to valdiate.</param>
        public override void Handle(DynamicPathSegment segment)
        {
            ValidateItemAndType(segment);
        }

        /// <summary>
        /// Handle validating a OperationSegment
        /// </summary>
        /// <param name="segment">The operation segment to valdiate.</param>
        public override void Handle(OperationSegment segment)
        {
            ValidateItemAndType(segment);
            ValidateItem(segment.EntitySet);
            foreach (OperationSegmentParameter parameter in segment.Parameters)
            {
                ValidateItem(parameter);
            }

            foreach (IEdmOperation operation in segment.Operations)
            {
                ValidateItem(operation);
                ValidateItem(operation.ReturnType);
            }
        }

        /// <summary>
        /// Handle validating a OperationImportSegment
        /// </summary>
        /// <param name="segment">The operation import segment to valdiate.</param>
        public override void Handle(OperationImportSegment segment)
        {
            ValidateItemAndType(segment);
            ValidateItem(segment.EntitySet);
            foreach (OperationSegmentParameter parameter in segment.Parameters)
            {
                ValidateItem(parameter);
            }

            foreach (IEdmOperation operation in segment.OperationImports)
            {
                ValidateItem(operation);
                ValidateItem(operation.ReturnType);
            }
        }

        /// <summary>
        /// Handle validating a AnnotationSegment
        /// </summary>
        /// <param name="segment">The annotation segment to valdiate.</param>
        public override void Handle(AnnotationSegment segment)
        {
            ValidateItemAndType(segment);
            ValidateItem(segment.Term);
        }

        /// <summary>
        /// Handle validating a PropertySegment
        /// </summary>
        /// <param name="segment">The property segment to valdiate.</param>
        public override void Handle(PropertySegment segment)
        {
            ValidateItemAndType(segment);
            ValidateItem(segment.Property);
        }

        /// <summary>
        /// Handle validating a KeySegment
        /// </summary>
        /// <param name="segment">The key segment to valdiate.</param>
        public override void Handle(KeySegment segment)
        {
            ValidateItem(segment);
        }

        /// <summary>
        /// Handle validating a SingletonSegment
        /// </summary>
        /// <param name="segment">The singleton segment to valdiate.</param>
        public override void Handle(SingletonSegment segment)
        {
            ValidateItemAndType(segment);
            ValidateItem(segment.Singleton);
        }

        /// <summary>
        /// Handle validating a EntitySetSegment
        /// </summary>
        /// <param name="segment">The entity set segment to valdiate.</param>
        public override void Handle(EntitySetSegment segment)
        {
            ValidateItemAndType(segment);
            ValidateItem(segment.EntitySet);
        }

        /// <summary>
        /// Handle validating a NavigationPropertySegment
        /// </summary>
        /// <param name="segment">The navigation property segment to valdiate.</param>
        public override void Handle(NavigationPropertySegment segment)
        {
            ValidateItemAndType(segment);
            ValidateItem(segment.NavigationProperty);
            ValidateItem(segment.NavigationSource);
        }

        /// <summary>
        /// Handle validating a TypeSegment
        /// </summary>
        /// <param name="segment">The type segment to valdiate.</param>
        public override void Handle(TypeSegment segment)
        {
            ValidateItemAndType(segment);
        }

        /// <summary>
        /// Handle validating a PathTemplateSegment
        /// </summary>
        /// <param name="segment">The path template segment to valdiate.</param>
        public override void Handle(PathTemplateSegment segment)
        {
            ValidateItemAndType(segment);
        }

        /// <summary>
        /// Handle validating a ODataPathSegment
        /// </summary>
        /// <param name="segment">The path segment to valdiate.</param>
        public override void Handle(ODataPathSegment segment)
        {
            ValidateItemAndType(segment);
        }

        #endregion

        /// <summary>
        /// Validate a segment that defines the segment type (i.e., a property, operation, entityset, singleton, cast...)
        /// </summary>
        /// <param name="segment">Segment (and type) to be validated.</param>
        private void ValidateItemAndType(ODataPathSegment segment)
        {
            ValidateItem(segment);
            ValidateItem(segment.EdmType);
            IEdmCollectionType collectionType = segment.EdmType as IEdmCollectionType;
            if (collectionType != null)
            {
                ValidateItem(collectionType.ElementType.Definition);
            }
        }
    }
}
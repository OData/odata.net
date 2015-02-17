//---------------------------------------------------------------------
// <copyright file="AstoriaAttributeToPropertyMappingAnnotationConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.EntityModel.Edm;
    using Microsoft.Test.Taupo.Platforms;

    /// <summary>
    /// Looks at product annotations and converts it to test annotations in a <see cref="EntityModelSchema"/>.
    /// </summary>
    [ImplementationName(typeof(IAttributeToPropertyMappingAnnotationConverter), "Default")]
    public class AstoriaAttributeToPropertyMappingAnnotationConverter : EntityModelSchemaVisitor, IAttributeToPropertyMappingAnnotationConverter
    {
        /// <summary>
        /// Converts product annotations on the model to test annotations.
        /// </summary>
        /// <param name="model">model for which to convert the annotations</param>
        public void ConvertToTestAnnotations(EntityModelSchema model)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");

            this.Visit(model);
        }

        /// <summary>
        /// Visit entity type to convert annotations
        /// </summary>
        /// <param name="entity">Entity type to visit</param>
        protected override void VisitEntityType(EntityType entity)
        {
            this.ConvertHasStreamAnnotation(entity);
            base.VisitEntityType(entity);
        }

        /// <summary>
        /// Visit member property to convert annotations
        /// </summary>
        /// <param name="memberProperty">member property to visit</param>
        protected override void VisitMemberProperty(MemberProperty memberProperty)
        {
            this.ConvertMimeTypeAnnotations(memberProperty);
            base.VisitMemberProperty(memberProperty);
        }

        /// <summary>
        /// Visit Function Import to convert Mime Type annotations on it
        /// </summary>
        /// <param name="functionImport">function import to convert annotations on</param>
        protected override void VisitFunctionImport(FunctionImport functionImport)
        {
            this.ConvertMimeTypeAnnotations(functionImport);
        }

        /// <summary>
        /// Converts mime type annotations from xml annotation to MimeType annotation
        /// </summary>
        /// <param name="annotatedItem">item to convert annotation on</param>
        private void ConvertMimeTypeAnnotations(AnnotatedItem annotatedItem)
        {
            ExceptionUtilities.CheckArgumentNotNull(annotatedItem, "annotatedItem");
            var mimeTypeAnnotation = annotatedItem.Annotations.OfType<AttributeAnnotation>()
                .Where(ann => ann.Content.Name.LocalName.Equals("MimeType", StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
            if (mimeTypeAnnotation != null)
            {
                annotatedItem.Add(new MimeTypeAnnotation(mimeTypeAnnotation.Content.Value));
            }
        }

        /// <summary>
        /// Converts a potential HasStream annotation to the corresponding test annotation on the <paramref name="entityType"/>.
        /// </summary>
        /// <param name="entityType">EntityType to convert the annotations on.</param>
        private void ConvertHasStreamAnnotation(EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");

            var hasStreamAnnotation = entityType.Annotations.OfType<AttributeAnnotation>()
                .Where(ann => ann.Content != null && ann.Content.Name.LocalName == "HasStream").SingleOrDefault();

            if (hasStreamAnnotation != null)
            {
                bool hasStreamValue = bool.Parse(hasStreamAnnotation.Content.Value);
                if (hasStreamValue)
                {
                    entityType.Add(new HasStreamAnnotation());
                }
            }
        }
    }
}

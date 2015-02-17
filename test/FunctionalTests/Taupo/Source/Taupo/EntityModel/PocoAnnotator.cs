//---------------------------------------------------------------------
// <copyright file="PocoAnnotator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Annotates the EntityModelSchema as per the POCOGenerationOption supplied.
    /// </summary>
    public static class PocoAnnotator
    {
        /// <summary>
        /// Annotates the model based on the POCO Generation options.
        /// </summary>
        /// <param name="model">The EntityModelSchema that has to be annotated</param>
        /// <param name="option">The POCOGeneration option for model annotation</param>
        public static void Annotate(EntityModelSchema model, PocoOption option)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");

            switch (option)
            {
                case PocoOption.None:
                    break;
                case PocoOption.Pure:
                    CustomizeModelForPurePocoGeneration(model);
                    break;
                case PocoOption.NavigationPropertiesVirtual:
                    CustomizeModelForPocoGenerationWithNavigationPropertiesVirtual(model);
                    break;
                case PocoOption.AllPropertiesVirtual:
                    CustomizeModelForPocoGenerationWithAllPropertiesVirtual(model);
                    break;
                default:
                    throw new TaupoArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid Poco option '{0}' specified. Use any of the allowed Poco option values such as '{1}', '{2}', '{3}' or '{4}' instead!", option, PocoOption.None, PocoOption.Pure, PocoOption.NavigationPropertiesVirtual, PocoOption.AllPropertiesVirtual));
            }
        }

        /// <summary>
        /// Customizes the model for poco generation with all properties virtual.
        /// </summary>
        /// <param name="model">The model.</param>
        private static void CustomizeModelForPocoGenerationWithAllPropertiesVirtual(EntityModelSchema model)
        {
            AnnotatePropertiesAsVirtual(model.EntityTypes.SelectMany(e => e.Properties).Cast<AnnotatedItem>());
            AnnotatePropertiesAsVirtual(model.ComplexTypes.SelectMany(e => e.Properties).Cast<AnnotatedItem>());
            AnnotatePropertiesAsVirtual(model.EntityTypes.SelectMany(e => e.NavigationProperties).Cast<AnnotatedItem>());
        }

        /// <summary>
        /// Customizes the model for Poco generation with navigation properties virtual.
        /// </summary>
        /// <param name="model">The model.</param>
        private static void CustomizeModelForPocoGenerationWithNavigationPropertiesVirtual(EntityModelSchema model)
        {
            RemoveVirtualAnnotationFromProperties(model.EntityTypes.SelectMany(e => e.Properties).Cast<AnnotatedItem>().Where(p => p.Annotations.Any(a => a is VirtualAnnotation)));
            RemoveVirtualAnnotationFromProperties(model.ComplexTypes.SelectMany(e => e.Properties).Cast<AnnotatedItem>().Where(p => p.Annotations.Any(a => a is VirtualAnnotation)));
            AnnotatePropertiesAsVirtual(model.EntityTypes.SelectMany(e => e.NavigationProperties).Cast<AnnotatedItem>());
        }

        /// <summary>
        /// Customizes the model for pure poco generation, by removing any of the virtual annotations defined in the model
        /// </summary>
        /// <param name="model">The model.</param>
        private static void CustomizeModelForPurePocoGeneration(EntityModelSchema model)
        {
            RemoveVirtualAnnotationFromProperties(model.EntityTypes.SelectMany(e => e.Properties).Cast<AnnotatedItem>().Where(p => p.Annotations.Any(a => a is VirtualAnnotation)));
            RemoveVirtualAnnotationFromProperties(model.ComplexTypes.SelectMany(e => e.Properties).Cast<AnnotatedItem>().Where(p => p.Annotations.Any(a => a is VirtualAnnotation)));
            RemoveVirtualAnnotationFromProperties(model.EntityTypes.SelectMany(e => e.NavigationProperties).Cast<AnnotatedItem>().Where(p => p.Annotations.Any(a => a is VirtualAnnotation)));
        }

        /// <summary>
        /// Removes annotation of Type T from the collection of properties passed in
        /// </summary>
        /// <param name="annotatedProperties">The annotated properties.</param>
        private static void RemoveVirtualAnnotationFromProperties(IEnumerable<AnnotatedItem> annotatedProperties)
        {
            foreach (var property in annotatedProperties)
            {
                RemoveVirtualAnnotationFromProperty(property);
            }
        }

        /// <summary>
        /// Removes an annotation of type T from a property.
        /// </summary>
        /// <param name="property">The property.</param>
        private static void RemoveVirtualAnnotationFromProperty(AnnotatedItem property)
        {
            foreach (var annotation in property.Annotations.OfType<VirtualAnnotation>().ToArray())
            {
                property.Annotations.Remove(annotation);
            }
        }

        /// <summary>
        /// Annotates the properties with Annotation of type T 
        /// </summary>
        /// <param name="properties">The properties.</param>
        private static void AnnotatePropertiesAsVirtual(IEnumerable<AnnotatedItem> properties)
        {
            foreach (var property in properties)
            {
                AnnotatePropertyAsVirtual(property);
            }
        }

        /// <summary>
        /// Annotates a property with Annotation of type T
        /// </summary>
        /// <param name="property">The property.</param>
        private static void AnnotatePropertyAsVirtual(AnnotatedItem property)
        {
            if (!property.Annotations.Any(a => a is VirtualAnnotation))
            {
                property.Annotations.Add(new VirtualAnnotation());
            }
        }
    }
}

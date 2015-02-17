//---------------------------------------------------------------------
// <copyright file="CustomAnnotationConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Reflection;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;

    /// <summary>
    /// Converts structural and attribute annotations in <see cref="EdmConstants.TaupoAnnotationsNamespace"/> namespace
    /// to custom annotations.
    /// </summary>
    public class CustomAnnotationConverter
    {
        /// <summary>
        /// Initializes a new instance of the CustomAnnotationConverter class.
        /// </summary>
        public CustomAnnotationConverter()
        {
            this.AnnotationNamespaces = new List<string>
            {
                typeof(TagAnnotation).Namespace
            };

            this.AnnotationAssemblies = new List<Assembly>
            {
                typeof(TagAnnotation).GetAssembly()
            };
        }

        /// <summary>
        /// Gets the namespaces which contain annotation types.
        /// </summary>
        public IList<string> AnnotationNamespaces { get; private set; }

        /// <summary>
        /// Gets the assemblies which contain annotation types.
        /// </summary>
        public IList<Assembly> AnnotationAssemblies { get; private set; }

        /// <summary>
        /// Converts the annotations for the given schema.
        /// </summary>
        /// <param name="schema">The schema to convert annotations.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Complexity is unavoidable, we need to visit all AnnotatedItems in a model.")]
        public void ConvertAnnotations(EntityModelSchema schema)
        {
            foreach (var container in schema.EntityContainers)
            {
                this.ConvertAnnotations(container);

                foreach (var entitySet in container.EntitySets)
                {
                    this.ConvertAnnotations(entitySet);
                }

                foreach (var associationSet in container.AssociationSets)
                {
                    this.ConvertAnnotations(associationSet);
                }

                foreach (var functionImport in container.FunctionImports)
                {
                    this.ConvertAnnotations(functionImport);
                    foreach (var param in functionImport.Parameters)
                    {
                        this.ConvertAnnotations(param);
                    }
                }
            }

            foreach (var type in schema.EntityTypes)
            {
                this.ConvertAnnotations(type);
                foreach (var prop in type.Properties)
                {
                    this.ConvertAnnotations(prop);
                }

                foreach (var navprop in type.NavigationProperties)
                {
                    this.ConvertAnnotations(navprop);
                }
            }

            foreach (var type in schema.ComplexTypes)
            {
                this.ConvertAnnotations(type);
                foreach (var prop in type.Properties)
                {
                    this.ConvertAnnotations(prop);
                }
            }

            foreach (var function in schema.Functions)
            {
                this.ConvertAnnotations(function);

                foreach (var param in function.Parameters)
                {
                    this.ConvertAnnotations(param);
                }
            }

            foreach (var association in schema.Associations)
            {
                this.ConvertAnnotations(association);

                foreach (var end in association.Ends)
                {
                    this.ConvertAnnotations(end);
                }
            }
        }

        private void ConvertAnnotations(AnnotatedItem annotatedItem)
        {
            for (int index = 0; index < annotatedItem.Annotations.Count; index++)
            {
                var annotation = annotatedItem.Annotations[index];

                var structuralAnnotation = annotation as StructuralAnnotation;
                if (structuralAnnotation != null)
                {
                    annotatedItem.Annotations[index] = this.ConvertSingleAnnotation(structuralAnnotation);
                }

                var attributeAnnotation = annotation as AttributeAnnotation;
                if (attributeAnnotation != null)
                {
                    annotatedItem.Annotations[index] = this.ConvertSingleAnnotation(attributeAnnotation);
                }
            }
        }

        private Annotation ConvertSingleAnnotation(AttributeAnnotation annotation)
        {
            if (annotation.Content == null)
            {
                return annotation;
            }

            if (annotation.Content.Name.Namespace == EdmConstants.AnnotationNamespace && annotation.Content.Name.LocalName == "StoreGeneratedPattern")
            {
                if (annotation.Content.Value == StoreGeneratedPatternAnnotation.None.Name)
                {
                    return StoreGeneratedPatternAnnotation.None;
                }
                else if (annotation.Content.Value == StoreGeneratedPatternAnnotation.Identity.Name)
                {
                    return StoreGeneratedPatternAnnotation.Identity;
                }
                else
                {
                    ExceptionUtilities.Assert(annotation.Content.Value == StoreGeneratedPatternAnnotation.Computed.Name, "Unrecognized store generated annotation: '{0}'", annotation.Content.Value);
                    return StoreGeneratedPatternAnnotation.Computed;
                }
            }

            if (annotation.Content.Name.Namespace != EdmConstants.TaupoAnnotationsNamespace)
            {
                return annotation;
            }

            Type annotationType = this.FindAnnotationType(annotation.Content.Name);
            if (annotationType == null)
            {
                return annotation;
            }

            // if the type can do custom serialization...
            if (typeof(ICustomAnnotationSerializer).IsAssignableFrom(annotationType))
            {
                // since this interface is implemented, we also expect a constructor
                // which takes an XAttribute to be there, so let's just invoke it
                // on our XAttribute
                return (Annotation)Activator.CreateInstance(annotationType, annotation.Content);
            }

            // if it is a tag, just create instance of that class
            if (typeof(TagAnnotation).IsAssignableFrom(annotationType))
            {
                return (Annotation)Activator.CreateInstance(annotationType);
            }

            // otherwise return the original 
            return annotation;
        }

        private Annotation ConvertSingleAnnotation(StructuralAnnotation annotation)
        {
            if (annotation.Content == null)
            {
                return annotation;
            }

            if (annotation.Content.Name.Namespace != EdmConstants.TaupoAnnotationsNamespace)
            {
                return annotation;
            }

            Type annotationType = this.FindAnnotationType(annotation.Content.Name);
            if (annotationType == null)
            {
                return annotation;
            }

            if (typeof(ICustomAnnotationSerializer).IsAssignableFrom(annotationType))
            {
                // since this interface is implemented, we also expect a constructor
                // which takes an XElement to be there, so let's just invoke it
                // on our XElement
                return (Annotation)Activator.CreateInstance(annotationType, annotation.Content);
            }

            if (typeof(CompositeAnnotation).IsAssignableFrom(annotationType))
            {
                return this.DeserializeCompositeAnnotation(annotationType, annotation);
            }

            // any other annotation type - just return it
            return annotation;
        }

        private CompositeAnnotation DeserializeCompositeAnnotation(Type annotationType, StructuralAnnotation annotation)
        {
            var composite = (CompositeAnnotation)Activator.CreateInstance(annotationType, true);

            foreach (var prop in composite.GetType().GetAllInstanceProperties())
            {
                if (prop.CanWrite)
                {
                    var element = annotation.Content.Element(EdmConstants.TaupoAnnotationsNamespace + prop.Name);
                    if (element != null)
                    {
                        Type targetType = prop.PropertyType;

                        // if it's Nullable<T>, extract T
                        targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;

                        object typedValue;
                        string stringValue = element.Value;

                        if (targetType == typeof(string))
                        {
                            typedValue = stringValue;
                        }
                        else if (targetType.IsEnum())
                        {
                            typedValue = Enum.Parse(targetType, stringValue, false);
                        }
                        else if (targetType == typeof(Guid))
                        {
                            typedValue = new Guid(stringValue);
                        }
                        else
                        {
                            ExceptionUtilities.Assert(targetType.IsPrimitive() || targetType == typeof(decimal), "Non-primitive types are not supported.");
                            typedValue = Convert.ChangeType(element.Value, targetType, CultureInfo.InvariantCulture);
                        }

                        prop.SetValue(composite, typedValue, null);
                    }
                }
            }

            return composite;
        }

        private Type FindAnnotationType(XName name)
        {
            foreach (var asm in this.AnnotationAssemblies)
            {
                foreach (var ns in this.AnnotationNamespaces)
                {
                    string fullTypeName = ns + "." + name.LocalName;
                    var type = asm.GetType(fullTypeName);
                    if (type != null)
                    {
                        return type;
                    }
                }
            }

            return null;
        }
    }
}

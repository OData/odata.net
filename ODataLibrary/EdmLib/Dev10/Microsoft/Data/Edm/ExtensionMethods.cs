//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm
{
    /// <summary>
    /// Contains extension methods for <see cref="IEdmModel"/> interfaces.
    /// </summary>
    public static class ExtensionMethods
    {
        #region IEdmModel
        /// <summary>
        /// Gets the value for the EDM version of the <paramref name="model"/>.
        /// </summary>
        /// <param name="model">Model the version has been set for.</param>
        /// <returns>The version.</returns>
        public static Version GetEdmVersion(this IEdmModel model)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            return model.GetAnnotation<Version>(EdmConstants.InternalUri, EdmConstants.EdmVersionAnnotation);
        }

        /// <summary>
        /// Sets a value of EDM version attribute of the <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The model the version should be set for.</param>
        /// <param name="version">The version.</param>
        public static void SetEdmVersion(this IEdmModel model, Version version)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            model.SetAnnotation(EdmConstants.InternalUri, EdmConstants.EdmVersionAnnotation, version);
        }

        #endregion

        #region IEdmElement
        /// <summary>
        /// Gets documentation for the specified element.
        /// </summary>
        /// <param name="element">Reference to the calling object.</param>
        /// <returns>Documentation that exists on the element. Otherwise, null.</returns>
        public static IEdmDocumentation GetDocumentation(this IEdmElement element)
        {
            EdmUtil.CheckArgumentNull(element, "element");
            return (IEdmDocumentation)element.GetAnnotation(EdmConstants.DocumentationUri, EdmConstants.DocumentationLocalName);
        }

        /// <summary>
        /// Sets documentation for the specified element.
        /// </summary>
        /// <param name="element">Reference to the calling object.</param>
        /// <param name="documentation">Documentation to set.</param>
        public static void SetDocumentation(this IEdmElement element, IEdmDocumentation documentation)
        {
            EdmUtil.CheckArgumentNull(element, "element");
            element.SetAnnotation(EdmConstants.DocumentationUri, EdmConstants.DocumentationLocalName, documentation);
        }

        /// <summary>
        /// Gets the location, if any, of this element.
        /// </summary>
        /// <param name="item">Reference to the calling object.</param>
        /// <returns>The location of the element, or null if the element is not locatable.</returns>
        public static EdmLocation Location(this IEdmElement item)
        {
            EdmUtil.CheckArgumentNull(item, "item");
            IEdmLocatable locatable = item as IEdmLocatable;
            return locatable != null ? locatable.Location : null;
        }

        /// <summary>
        /// Gets the errors, if any, that belong to this element.
        /// </summary>
        /// <param name="element">Reference to the calling object.</param>
        /// <returns>Any errors that belong to this element.</returns>
        private static IEnumerable<EdmError> ImmediateErrors(this IEdmElement element)
        {
            IEdmCheckable checkable = element as IEdmCheckable;
            return checkable != null ? checkable.Errors : Enumerable.Empty<EdmError>();
        }

        /// <summary>
        /// Returns true if this element is an invalid element or is likely affected by an invalid element.
        /// </summary>
        /// <param name="element">Reference to the calling object.</param>
        /// <returns>This element is an invalid element.</returns>
        public static bool IsBad(this IEdmElement element)
        {
            return element.ImmediateErrors().Count() != 0;
        }

        /// <summary>
        /// Gets the errors, if any, that belong to this element or other nearby elements that might affect the validity of this element.
        /// </summary>
        /// <param name="element">Reference to the calling object.</param>
        /// <returns>Any errors that belong to this element or other nearby elements that might affect the validity of this element.</returns>
        public static IEnumerable<EdmError> Errors(this IEdmElement element)
        {
            return element.ImmediateErrors();
        }

        #endregion

        private static class TypeName<T>
        {
            public readonly static string LocalName = typeof(T).FullName;
        }

        #region IEdmAnnotatable
        /// <summary>
        /// Gets an annotatable element's vocabulary annotations, including those not defined directly on the element.
        /// </summary>
        /// <param name="element">Reference to the calling object.</param>
        /// <param name="model">Model to check for annotations.</param>
        /// <returns>Annotations defined directly on the element and those defined elsewhere in the model.</returns>
        public static IEnumerable<IEdmVocabularyAnnotation> VocabularyAnnotations(this IEdmAnnotatable element, IEdmModel model)
        {
            EdmUtil.CheckArgumentNull(element, "element");
            EdmUtil.CheckArgumentNull(model, "model");
            return element.Annotations.OfType<IEdmVocabularyAnnotation>().Concat(model.FindVocabularyAnnotations(element));
        }

        /// <summary>
        /// Gets an annotation corresponding to the given namespace and name provided.
        /// </summary>
        /// <typeparam name="T">Type of the annotation being returned.</typeparam>
        /// <param name="annotatable">Reference to the calling object.</param>
        /// <param name="namespaceName">Namespace of the annotation.</param>
        /// <param name="localName">Name of the annotation inside the namespace.</param>
        /// <returns>The requested annotation, if it exists. Otherwise, null.</returns>
        public static T GetAnnotation<T>(this IEdmAnnotatable annotatable, string namespaceName, string localName) where T : class
        {
            EdmUtil.CheckArgumentNull(annotatable, "annotatable");
            object annotation = annotatable.GetAnnotation(namespaceName, localName);
            if (annotation != null)
            {
                T specificAnnotation = annotation as T;
                if (specificAnnotation != null)
                {
                    return specificAnnotation;
                }

                IEdmValue valueAnnotation = annotation as IEdmValue;
                if (valueAnnotation != null)
                {
                    // ToDo JHamby: Do something to try to map the IEdmValue to T.
                }

                throw new InvalidOperationException(Edm.Strings.Annotations_TypeMismatch(annotation.GetType().Name, typeof(T).Name));
            }

            return null;
        }

        // Strongly-typed simple wrappers.
        /// <summary>
        /// Sets an annotation on the annotatable element.
        /// </summary>
        /// <param name="annotatable">Reference to the calling object.</param>
        /// <param name="namespaceName">Namespace of the annotation.</param>
        /// <param name="localName">Name of the annotation inside the namespace.</param>
        /// <param name="value">Contents of the new annotation.</param>
        public static void SetAnnotation(this IEdmAnnotatable annotatable, string namespaceName, string localName, IEdmValue value)
        {
            EdmUtil.CheckArgumentNull(annotatable, "annotatable");
            EdmUtil.CheckArgumentNull(namespaceName, "namespaceName");
            EdmUtil.CheckArgumentNull(localName, "localName");
            EdmUtil.CheckArgumentNull(value, "value");
            annotatable.SetAnnotation(namespaceName, localName, value);
        }

        /// <summary>
        /// Gets an annotation from the annotatable element.
        /// </summary>
        /// <param name="annotatable">Reference to the calling object.</param>
        /// <param name="name">Name of the annotation to retrieve.</param>
        /// <returns>The requested annotation if it exists. Otherwise, null.</returns>
        public static IEdmValue GetAnnotation(this IEdmAnnotatable annotatable, EdmTermName name)
        {
            EdmUtil.CheckArgumentNull(annotatable, "annotatable");
            EdmUtil.CheckArgumentNull(name, "name");
            return annotatable.GetAnnotation<IEdmValue>(name.Namespace, name.LocalName);
        }

        /// <summary>
        /// Sets an annotation on the annotatable element.
        /// </summary>
        /// <typeparam name="T">Type of the annotation being set.</typeparam>
        /// <param name="annotatable">Reference to the calling object.</param>
        /// <param name="namespaceName">Namespace of the annotation.</param>
        /// <param name="localName">Name of the annotation inside the namespace.</param>
        /// <param name="value">Contents of the new annotation.</param>
        public static void SetAnnotation<T>(this IEdmAnnotatable annotatable, string namespaceName, string localName, T value) where T : class
        {
            EdmUtil.CheckArgumentNull(annotatable, "annotatable");
            annotatable.SetAnnotation(namespaceName, localName, value);
        }

        // Strongly-typed wrappers with packaged names.
        /// <summary>
        /// Gets an annotation from the annotatable element.
        /// </summary>
        /// <typeparam name="T">Type of the annotation being returned.</typeparam>
        /// <param name="annotatable">Reference to the calling object.</param>
        /// <param name="name">Name of the annotation to retrieve.</param>
        /// <returns>The requested annotation if it exists. Otherwise, null</returns>
        public static T GetAnnotation<T>(this IEdmAnnotatable annotatable, EdmTermName name) where T : class
        {
            EdmUtil.CheckArgumentNull(annotatable, "annotatable");
            EdmUtil.CheckArgumentNull(name, "name");
            return annotatable.GetAnnotation<T>(name.Namespace, name.LocalName);
        }

        /// <summary>
        /// Sets an annotation on the annotatable element.
        /// </summary>
        /// <typeparam name="T">Type of the annotation being set.</typeparam>
        /// <param name="annotatable">Reference to the calling object.</param>
        /// <param name="name">Name of the annotation to set.</param>
        /// <param name="value">Contents of the new annotation.</param>
        public static void SetAnnotation<T>(this IEdmAnnotatable annotatable, EdmTermName name, T value) where T : class
        {
            EdmUtil.CheckArgumentNull(annotatable, "annotatable");
            EdmUtil.CheckArgumentNull(name, "name");
            EdmUtil.CheckArgumentNull(value, "value");
            annotatable.SetAnnotation(name.Namespace, name.LocalName, value);
        }

        // Strongly-typed wrappers for unnamed annotations keyed by CLR type.
        /// <summary>
        /// Gets an annotation from the annotatable element.
        /// </summary>
        /// <typeparam name="T">Type of the annotation being returned.</typeparam>
        /// <param name="annotatable">Reference to the calling object.</param>
        /// <returns>The requested annotation if it exists. Otherwise, null.</returns>
        public static T GetAnnotation<T>(this IEdmAnnotatable annotatable) where T : class
        {
            EdmUtil.CheckArgumentNull(annotatable, "annotatable");
            return annotatable.GetAnnotation<T>(EdmConstants.InternalUri, TypeName<T>.LocalName);
        }

        /// <summary>
        /// Sets an annotation on the annotatable element.
        /// </summary>
        /// <typeparam name="T">Type of the annotation being set.</typeparam>
        /// <param name="annotatable">Reference to the calling object.</param>
        /// <param name="value">Contents of the new annotation.</param>
        public static void SetAnnotation<T>(this IEdmAnnotatable annotatable, T value) where T : class
        {
            EdmUtil.CheckArgumentNull(annotatable, "annotatable");
            annotatable.SetAnnotation(EdmConstants.InternalUri, TypeName<T>.LocalName, value);
        }
        #endregion

        #region IEdmAnnotation
        /// <summary>
        /// Gets the namespace of an annotation.
        /// </summary>
        /// <param name="annotation">Reference to the annotation.</param>
        /// <returns>The namespace of the annotation.</returns>
        public static string Namespace(this IEdmAnnotation annotation)
        {
            EdmUtil.CheckArgumentNull(annotation, "annotation");
            return annotation.Term.NamespaceUri;
        }

        /// <summary>
        /// Gets the name of an annotation.
        /// </summary>
        /// <param name="annotation">Reference to the annotation.</param>
        /// <returns>The name of the annotation.</returns>
        public static string LocalName(this IEdmAnnotation annotation)
        {
            EdmUtil.CheckArgumentNull(annotation, "annotation");
            return annotation.Term.Name;
        }
        #endregion

        #region IEdmSchemaElement
        /// <summary>
        /// Gets the full name of the element.
        /// </summary>
        /// <param name="element">Reference to the calling object.</param>
        /// <returns>The full name of the element.</returns>
        public static string FullName(this IEdmSchemaElement element)
        {
            EdmUtil.CheckArgumentNull(element, "element");
            return element.Namespace + "." + element.Name;
        }
        #endregion

        #region IEdmEntityContainer

        /// <summary>
        /// Returns entity sets belonging to an IEdmEntityContainer.
        /// </summary>
        /// <param name="container">Reference to the calling object.</param>
        /// <returns>Entity sets belonging to an IEdmEntityContainer.</returns>
        public static IEnumerable<IEdmEntitySet> EntitySets(this IEdmEntityContainer container)
        {
            EdmUtil.CheckArgumentNull(container, "container");
            return container.Elements.OfType<IEdmEntitySet>();
        }

        /// <summary>
        /// Returns association sets belonging to an IEdmEntityContainer.
        /// </summary>
        /// <param name="container">Reference to the calling object.</param>
        /// <returns>Association sets belonging to an IEdmEntityContainer.</returns>
        public static IEnumerable<IEdmAssociationSet> AssociationSets(this IEdmEntityContainer container)
        {
            EdmUtil.CheckArgumentNull(container, "container");
            return container.Elements.OfType<IEdmAssociationSet>();
        }

        /// <summary>
        /// Returns function imports belonging to an IEdmEntityContainer.
        /// </summary>
        /// <param name="container">Reference to the calling object.</param>
        /// <returns>Function imports belonging to an IEdmEntityContainer.</returns>
        public static IEnumerable<IEdmFunctionImport> FunctionImports(this IEdmEntityContainer container)
        {
            EdmUtil.CheckArgumentNull(container, "container");
            return container.Elements.OfType<IEdmFunctionImport>();
        }
        #endregion

        #region IEdmEntitySet

        internal static bool IsEquivalentTo(this IEdmEntitySet thisSet, IEdmEntitySet otherSet)
        {
            EdmUtil.CheckArgumentNull(thisSet, "thisSet");
            EdmUtil.CheckArgumentNull(otherSet, "otherSet");
            return thisSet.ElementType.IsEquivalentTo(otherSet.ElementType) && thisSet.Name == otherSet.Name;
        }

        /// <summary>
        /// Returns true if this element is an invalid element or is likely affected by an invalid element.
        /// </summary>
        /// <param name="set">Reference to the calling object.</param>
        /// <returns>This element is an invalid element.</returns>
        public static bool IsBad(this IEdmEntitySet set)
        {
            EdmUtil.CheckArgumentNull(set, "set");
            return ((IEdmElement)set).IsBad() || set.ElementType.IsBad();
        }

        #endregion

        #region IEdmAssociationSet

        internal static bool IsEquivalentTo(this IEdmAssociationSet thisSet, IEdmAssociationSet otherSet)
        {
            EdmUtil.CheckArgumentNull(thisSet, "thisSet");
            EdmUtil.CheckArgumentNull(otherSet, "otherSet");
            return thisSet.Association.IsEquivalentTo(otherSet.Association) && thisSet.Name == otherSet.Name;
        }

        /// <summary>
        /// Returns true if this element is an invalid element or is likely affected by an invalid element.
        /// </summary>
        /// <param name="set">Reference to the calling object.</param>
        /// <returns>This element is an invalid element.</returns>
        public static bool IsBad(this IEdmAssociationSet set)
        {
            EdmUtil.CheckArgumentNull(set, "set");
            return ((IEdmElement)set).IsBad() || set.Association.IsBad() || set.End1.IsBad() || set.End2.IsBad();
        }

        /// <summary>
        /// Gets the errors, if any, that belong to this element or other nearby elements that might affect the validity of this element.
        /// </summary>
        /// <param name="set">Reference to the calling object.</param>
        /// <returns>Any errors that belong to this element or other nearby elements that might affect the validity of this element.</returns>
        public static IEnumerable<EdmError> Errors(this IEdmAssociationSet set)
        {
            EdmUtil.CheckArgumentNull(set, "set");
            return YieldFirst(set.ImmediateErrors(), set.End1.Errors(), set.End2.Errors());
        }

        #endregion

        #region IEdmAssociationSetEnd

        internal static bool IsEquivalentTo(this IEdmAssociationSetEnd end1, IEdmAssociationSetEnd end2)
        {
            EdmUtil.CheckArgumentNull(end1, "end1");
            EdmUtil.CheckArgumentNull(end2, "end2");
            return end1.EntitySet.IsEquivalentTo(end2.EntitySet) && end1.Role.IsEquivalentTo(end2.Role);
        }

        /// <summary>
        /// Returns true if this element is an invalid element or is likely affected by an invalid element.
        /// </summary>
        /// <param name="end">Reference to the calling object.</param>
        /// <returns>This element is an invalid element.</returns>
        public static bool IsBad(this IEdmAssociationSetEnd end)
        {
            EdmUtil.CheckArgumentNull(end, "end");
            return ((IEdmElement)end).IsBad() || end.Role.IsBad() || end.EntitySet.IsBad();
        }

        #endregion

        #region IEdmAssociation

        internal static bool IsEquivalentTo(this IEdmAssociation thisSet, IEdmAssociation otherSet)
        {
            EdmUtil.CheckArgumentNull(thisSet, "thisSet");
            EdmUtil.CheckArgumentNull(otherSet, "otherSet");
            return thisSet.FullName() == otherSet.FullName();
        }

        /// <summary>
        /// Returns the text representation of the current object.
        /// </summary>
        /// <param name="schemaElement">Reference to the calling object.</param>
        /// <returns>The text representation of the current object.</returns>
        public static string ToTraceString(this IEdmSchemaElement schemaElement)
        {
            return schemaElement.FullName();
        }

        #endregion

        #region IEdmAssociationEnd

        internal static bool IsEquivalentTo(this IEdmAssociationEnd end1, IEdmAssociationEnd end2)
        {
            EdmUtil.CheckArgumentNull(end1, "end1");
            EdmUtil.CheckArgumentNull(end2, "end2");
            return end1.DeclaringAssociation.IsEquivalentTo(end2.DeclaringAssociation) &&
                end1.Name == end2.Name;
        }

        /// <summary>
        /// Returns true if this element is an invalid element or is likely affected by an invalid element.
        /// </summary>
        /// <param name="end">Reference to the calling object.</param>
        /// <returns>This element is an invalid element.</returns>
        public static bool IsBad(this IEdmAssociationEnd end)
        {
            EdmUtil.CheckArgumentNull(end, "end");
            return ((IEdmElement)end).IsBad() || end.EntityType.IsBad();
        }

        #endregion

        #region IEdmTypeReference
        /// <summary>
        /// Gets the type kind of the type references definition.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The type kind of the reference.</returns>
        public static EdmTypeKind TypeKind(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.Definition.TypeKind;
        }

        /// <summary>
        /// Gets the full name of the definition referred to by the type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The full name of this references definition.</returns>
        public static string FullName(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            var namedDefinition = type.Definition as IEdmSchemaElement;
            return namedDefinition != null ? namedDefinition.FullName() : null;
        }

        /// <summary>
        /// Returns true if this element is an invalid element or is likely affected by an invalid element.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This element is an invalid element.</returns>
        public static bool IsBad(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return ((IEdmElement)type).IsBad() || type.Definition.IsBad(); 
        }

        /// <summary>
        /// Gets the errors, if any, that belong to this element or other nearby elements that might affect the validity of this element.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>Any errors that belong to this element or other nearby elements that might affect the validity of this element.</returns>
        public static IEnumerable<EdmError> Errors(this  IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return YieldFirst(type.ImmediateErrors(), type.Definition.Errors());
        }

        #endregion

        #region IEdmPrimitiveTypeReference
        /// <summary>
        /// Gets the definition of this primitive type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>Definition of this primitive type reference.</returns>
        public static IEdmPrimitiveType PrimitiveDefinition(this IEdmPrimitiveTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return (IEdmPrimitiveType)type.Definition;
        }

        /// <summary>
        /// Gets the primitive kind of the definition referred to by this type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>Primitive kind of the definition of this reference.</returns>
        public static EdmPrimitiveTypeKind PrimitiveKind(this IEdmPrimitiveTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveDefinition().PrimitiveKind;
        }
        #endregion

        #region IEdmStructuredTypeDefinition
        /// <summary>
        /// Gets all properties of the structured type definition and its base types.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>Properties of this type.</returns>
        public static IEnumerable<IEdmProperty> Properties(this IEdmStructuredType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            if (type.BaseType != null)
            {
                foreach (IEdmProperty baseProperty in type.BaseType.Properties())
                {
                    yield return baseProperty;
                }
            }

            foreach (IEdmProperty declaredProperty in type.DeclaredProperties)
            {
                yield return declaredProperty;
            }
        }

        /// <summary>
        /// Gets all structural properties declared in the IEdmStructuredTypeDefinition.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>All structural properties declared in the IEdmStructuredTypeDefinition.</returns>
        public static IEnumerable<IEdmStructuralProperty> DeclaredStructuralProperties(this IEdmStructuredType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.DeclaredProperties.OfType<IEdmStructuralProperty>();
        }

        /// <summary>
        /// Gets the structural properties declared in this type definition and all base types.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The structural properties declared in this type definition and all base types.</returns>
        public static IEnumerable<IEdmStructuralProperty> StructuralProperties(this IEdmStructuredType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.Properties().OfType<IEdmStructuralProperty>();
        }

        /// <summary>
        /// Returns true if this element is an invalid element or is likely affected by an invalid element.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This element is an invalid element.</returns>
        public static bool IsBad(this IEdmStructuredType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            if (type.BaseType == null)
            {
                return ((IEdmElement)type).IsBad();
            }
            else
            {
                return ((IEdmElement)type).IsBad() || type.BaseType.IsBad();
            }
        }
        #endregion

        #region IEdmStructuredTypeReference
        /// <summary>
        /// Gets the definition of this structured type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The definition of this structured type reference.</returns>
        public static IEdmStructuredType StructuredDefinition(this IEdmStructuredTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return (IEdmStructuredType)type.Definition;
        }

        /// <summary>
        /// Returns true if the definition of this reference is abstract.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>If the definition of this reference is abstract.</returns>
        public static bool IsAbstract(this IEdmStructuredTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.StructuredDefinition().IsAbstract;
        }

        /// <summary>
        /// Returns true if the definition of this reference is open.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>If the definition of this reference is open.</returns>
        public static bool IsOpen(this IEdmStructuredTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.StructuredDefinition().IsOpen;
        }

        /// <summary>
        /// Returns the base type of the definition of this reference. 
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The base type of the definition of this reference. </returns>
        public static IEdmStructuredType BaseType(this IEdmStructuredTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.StructuredDefinition().BaseType;
        }

        /// <summary>
        /// Gets all structural properties declared in the definition of this reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>All structural properties declared in the definition of this reference.</returns>
        public static IEnumerable<IEdmStructuralProperty> DeclaredStructuralProperties(this IEdmStructuredTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.StructuredDefinition().DeclaredStructuralProperties();
        }

        /// <summary>
        /// Gets all structural properties declared in the definition of this reference and all its base types.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>All structural properties declared in the definition of this reference and all its base types.</returns>
        public static IEnumerable<IEdmStructuralProperty> StructuralProperties(this IEdmStructuredTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.StructuredDefinition().StructuralProperties();
        }

        /// <summary>
        /// Finds a property from the definition of this reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <param name="name">Name of the property to find.</param>
        /// <returns>The requested property if it exists. Otherwise, null.</returns>
        public static IEdmProperty FindProperty(this IEdmStructuredTypeReference type, string name)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            EdmUtil.CheckArgumentNull(name, "name");
            return type.StructuredDefinition().FindProperty(name);
        }
        #endregion

        #region IEdmEntityTypeDefinition
        /// <summary>
        /// Gets the base type of this entity type definition.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The base type of this entity type definition.</returns>
        public static IEdmEntityType BaseEntityType(this IEdmEntityType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return (IEdmEntityType)type.BaseType;
        }

        /// <summary>
        /// Gets the navigation properties declared in this entity definition.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The navigation properties declared in this entity definition.</returns>
        public static IEnumerable<IEdmNavigationProperty> DeclaredNavigationProperties(this IEdmEntityType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.DeclaredProperties.OfType<IEdmNavigationProperty>();
        }

        /// <summary>
        /// Get the navigation properties declared in this entity type and all base types.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The navigation properties declared in this entity type and all base types.</returns>
        public static IEnumerable<IEdmNavigationProperty> NavigationProperties(this IEdmEntityType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.Properties().OfType<IEdmNavigationProperty>();
        }

        /// <summary>
        /// Gets the declared key of the most defined entity with a declared key present.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>Key of this type.</returns>
        public static IEnumerable<IEdmStructuralProperty> Key(this IEdmEntityType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmEntityType checkingType = type;
            while (checkingType != null)
            {
                if (checkingType.DeclaredKey != null)
                {
                    return checkingType.DeclaredKey;
                }

                checkingType = checkingType.BaseEntityType();
            }

            return Enumerable.Empty<IEdmStructuralProperty>();
        }
        #endregion

        #region IEdmEntityTypeReference
        /// <summary>
        /// Gets the definition of this entity reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The definition of this entity reference.</returns>
        public static IEdmEntityType EntityDefinition(this IEdmEntityTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return (IEdmEntityType)type.Definition;
        }

        /// <summary>
        /// Gets the base type of the definition of this reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The base type of the definition of this reference.</returns>
        public static IEdmEntityType BaseEntityType(this IEdmEntityTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.EntityDefinition().BaseEntityType();
        }

        /// <summary>
        /// Gets the entity key of the definition of this reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The entity key of the definition of this reference.</returns>
        public static IEnumerable<IEdmStructuralProperty> Key(this IEdmEntityTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.EntityDefinition().Key();
        }

        /// <summary>
        /// Gets the navigation properties declared in the definition of this reference and its base types.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The navigation properties declared in the definition of this reference and its base types.</returns>
        public static IEnumerable<IEdmNavigationProperty> NavigationProperties(this IEdmEntityTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.EntityDefinition().NavigationProperties();
        }

        /// <summary>
        /// Gets the navigation properties declared in the definition of this reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The navigation properties declared in the definition of this reference.</returns>
        public static IEnumerable<IEdmNavigationProperty> DeclaredNavigationProperties(this IEdmEntityTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.EntityDefinition().DeclaredNavigationProperties();
        }

        /// <summary>
        /// Finds a navigation property declared in the definition of this reference by name.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <param name="name">Name of the navigation property to find.</param>
        /// <returns>The requested navigation property if it exists. Otherwise, null.</returns>
        public static IEdmNavigationProperty FindNavigationProperty(this IEdmEntityTypeReference type, string name)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            EdmUtil.CheckArgumentNull(name, "name");
            return type.EntityDefinition().FindProperty(name) as IEdmNavigationProperty;
        }
        #endregion

        #region IEdmComplexTypeDefinition
        /// <summary>
        /// Gets the base type of this references definition.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The base type of this references definition.</returns>
        public static IEdmComplexType BaseComplexType(this IEdmComplexType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return (IEdmComplexType)type.BaseType;
        }
        #endregion

        #region IEdmComplexTypeReference
        /// <summary>
        /// Gets the definition of this reference typed as an IEdmComplexTypeDefinition.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The definition of this reference typed as an IEdmComplexTypeDefinition.</returns>
        public static IEdmComplexType ComplexDefinition(this IEdmComplexTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return (IEdmComplexType)type.Definition;
        }

        /// <summary>
        /// Gets the base type of this reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The base type of this reference.</returns>
        public static IEdmComplexType BaseComplexType(this IEdmComplexTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.ComplexDefinition().BaseComplexType();
        }
        #endregion

        #region IEdmAssocaition

        /// <summary>
        /// Returns true if this element is an invalid element or is likely affected by an invalid element.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This element is an invalid element.</returns>
        public static bool IsBad(this IEdmAssociation type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            if (type.ReferentialConstraint != null)
            {
                return ((IEdmElement)type).IsBad() || type.End1.IsBad() || type.End2.IsBad() || type.ReferentialConstraint.IsBad();
            }
            else
            {
                return ((IEdmElement)type).IsBad() || type.End1.IsBad() || type.End2.IsBad();
            }
        }

        /// <summary>
        /// Gets the errors, if any, that belong to this element or other nearby elements that might affect the validity of this element.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>Any errors that belong to this element or other nearby elements that might affect the validity of this element.</returns>
        public static IEnumerable<EdmError> Errors(this  IEdmAssociation type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.ReferentialConstraint != null 
                ? YieldFirst(((IEdmElement)type).Errors(), type.End1.Errors(), type.End2.Errors(), type.ReferentialConstraint.Errors()) 
                : YieldFirst(((IEdmElement)type).Errors(), type.End1.Errors(), type.End2.Errors());
        }

        #endregion

        #region IEdmEntityReferenceTypeReference
        /// <summary>
        /// Gets the definition of this entity reference type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The definition of this entity reference type reference.</returns>
        public static IEdmEntityReferenceType EntityReferenceDefinition(this IEdmEntityReferenceTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return (IEdmEntityReferenceType)type.Definition;
        }

        /// <summary>
        /// Gets the entity type referred to by the definition of this entity reference type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The entity type referred to by the definition of this entity reference type reference.</returns>
        public static IEdmEntityType EntityType(this IEdmEntityReferenceTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.EntityReferenceDefinition().EntityType;
        }
        #endregion

        #region IEdmEntityReferenceType

        /// <summary>
        /// Returns true if this element is an invalid element or is likely affected by an invalid element.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This element is an invalid element.</returns>
        public static bool IsBad(this IEdmEntityReferenceType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return ((IEdmElement)type).IsBad() || type.EntityType.IsBad();
        }

        #endregion

        #region IEdmCollectionTypeReference
        /// <summary>
        /// Gets the definition of this multivalue reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The definition of this multivalue reference.</returns>
        public static IEdmCollectionType CollectionDefinition(this IEdmCollectionTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return (IEdmCollectionType)type.Definition;
        }

        /// <summary>
        /// Gets the element type of the definition of this collection reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The element type of the definition of this collection reference.</returns>
        public static IEdmTypeReference ElementType(this IEdmCollectionTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.CollectionDefinition().ElementType;
        }

        /// <summary>
        /// Returns true if this reference refers to a collection.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a collection.</returns>
        public static bool IsAtomic(this IEdmCollectionTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.CollectionDefinition().IsAtomic;
        }
        #endregion

        #region IEdmEnumTypeReference
        /// <summary>
        /// Gets the definition of this enumeration reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The definition of this enumeration reference.</returns>
        public static IEdmEnumType EnumDefinition(this IEdmEnumTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return (IEdmEnumType)type.Definition;
        }
        #endregion

        #region IEdmEnumType

        /// <summary>
        /// Returns true if this element is an invalid element or is likely affected by an invalid element.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This element is an invalid element.</returns>
        public static bool IsBad(this IEdmEnumType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return ((IEdmElement)type).IsBad() || type.UnderlyingType.IsBad();
        }

        /// <summary>
        /// Gets the errors, if any, that belong to this element or other nearby elements that might affect the validity of this element.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>Any errors that belong to this element or other nearby elements that might affect the validity of this element.</returns>
        public static IEnumerable<EdmError> Errors(this IEdmEnumType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return YieldFirst(type.ImmediateErrors(), type.UnderlyingType.Errors());
        }

        #endregion

        #region IEdmProperty

        internal static bool IsEquivalentTo(this IEdmProperty thisProp, IEdmProperty otherProp)
        {
            EdmUtil.CheckArgumentNull(thisProp, "thisProp");
            EdmUtil.CheckArgumentNull(otherProp, "otherProp");
            return thisProp.DeclaringType.IsEquivalentTo(otherProp.DeclaringType) && thisProp.Name == otherProp.Name;
        }

        /// <summary>
        /// Returns true if this element is an invalid element or is likely affected by an invalid element.
        /// </summary>
        /// <param name="property">Reference to the calling object.</param>
        /// <returns>This element is an invalid element.</returns>
        public static bool IsBad(this IEdmProperty property)
        {
            EdmUtil.CheckArgumentNull(property, "property");
            return ((IEdmElement)property).IsBad() || property.DeclaringType.IsBad() || property.Type.IsBad();
        }

        /// <summary>
        /// Gets the errors, if any, that belong to this element or other nearby elements that might affect the validity of this element.
        /// </summary>
        /// <param name="property">Reference to the calling object.</param>
        /// <returns>Any errors that belong to this element or other nearby elements that might affect the validity of this element.</returns>
        public static IEnumerable<EdmError> Errors(this IEdmProperty property)
        {
            EdmUtil.CheckArgumentNull(property, "property");
            return YieldFirst(property.ImmediateErrors(), property.Type.Errors());
        }

        #endregion

        #region IEdmNavigationProperty
        /// <summary>
        /// Gets the association type that describes this navigation property.
        /// </summary>
        /// <param name="property">Reference to the calling object.</param>
        /// <returns>The association type that describes this navigation property.</returns>
        public static IEdmAssociation Association(this IEdmNavigationProperty property)
        {
            EdmUtil.CheckArgumentNull(property, "property");
            return property.To.DeclaringAssociation;
        }

        /// <summary>
        /// Gets the from end of this navigation property.
        /// </summary>
        /// <param name="property">Reference to the calling object.</param>
        /// <returns>The from end of this navigation property.</returns>
        public static IEdmAssociationEnd From(this IEdmNavigationProperty property)
        {
            EdmUtil.CheckArgumentNull(property, "property");
            IEdmAssociationEnd to = property.To;
            IEdmAssociation definingAssociation = to.DeclaringAssociation;
            return to == definingAssociation.End1 ? definingAssociation.End2 : definingAssociation.End1;
        }

        /// <summary>
        /// Returns true if this element is an invalid element or is likely affected by an invalid element.
        /// </summary>
        /// <param name="property">Reference to the calling object.</param>
        /// <returns>This element is an invalid element.</returns>
        public static bool IsBad(this IEdmNavigationProperty property)
        {
            EdmUtil.CheckArgumentNull(property, "property");
            // We must check for errors in To before Type because type is a product of the To role (at least in our implementations)
            return ((IEdmElement)property).IsBad() || property.To.IsBad() || property.Type.IsBad() || property.DeclaringType.IsBad();
        }

        #endregion

        #region IEdmReferentialConstraint
        /// <summary>
        /// Gets the dependent end of this referential constraint.
        /// </summary>
        /// <param name="constraint">Reference to the calling object.</param>
        /// <returns>The dependent end of this referential constraint.</returns>
        public static IEdmAssociationEnd DependentEnd(this IEdmReferentialConstraint constraint)
        {
            EdmUtil.CheckArgumentNull(constraint, "constraint");
            IEdmAssociation association = constraint.PrincipalEnd.DeclaringAssociation;
            return constraint.PrincipalEnd == association.End1 ? association.End2 : association.End1;
        }

        /// <summary>
        /// Returns true if this element is an invalid element or is likely affected by an invalid element.
        /// </summary>
        /// <param name="constraint">Reference to the calling object.</param>
        /// <returns>This element is an invalid element.</returns>
        public static bool IsBad(this IEdmReferentialConstraint constraint)
        {
            EdmUtil.CheckArgumentNull(constraint, "constraint");
            return ((IEdmElement)constraint).IsBad() || constraint.PrincipalEnd.IsBad();
        }

        #endregion

        #region IEdmRowTypeReference
        /// <summary>
        /// Gets the definition of this row type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The definition of this row type reference.</returns>
        public static IEdmRowType RowDefinition(this IEdmRowTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return (IEdmRowType)type.Definition;
        }
        #endregion

        #region IEdmTypeAnnotation
        /// <summary>
        /// Gets the type term of this type annotation.
        /// </summary>
        /// <param name="annotation">Reference to the calling object.</param>
        /// <returns>The type term of this type annotation.</returns>
        public static IEdmEntityType TypeTerm(this IEdmTypeAnnotation annotation)
        {
            EdmUtil.CheckArgumentNull(annotation, "annotation");
            return (IEdmEntityType)annotation.Term;
        }
        #endregion

        #region IEdmValueAnnotation
        /// <summary>
        /// Gets the value term of this value annotation.
        /// </summary>
        /// <param name="annotation">Reference to the calling object.</param>
        /// <returns>The value term of this value annotation.</returns>
        public static IEdmValueTerm ValueTerm(this IEdmValueAnnotation annotation)
        {
            EdmUtil.CheckArgumentNull(annotation, "annotation");
            return (IEdmValueTerm)annotation.Term;
        }
        #endregion

        #region IEdmFunctionImport

        /// <summary>
        /// Returns true if this element is an invalid element or is likely affected by an invalid element.
        /// </summary>
        /// <param name="functionImport">Reference to the calling object.</param>
        /// <returns>This element is an invalid element.</returns>
        public static bool IsBad(this IEdmFunctionImport functionImport)
        {
            EdmUtil.CheckArgumentNull(functionImport, "functionImport");
            if (functionImport.EntitySet != null)
            {
                return ((IEdmFunctionBase)functionImport).IsBad() || functionImport.EntitySet.IsBad();
            }
            else
            {
                return ((IEdmFunctionBase)functionImport).IsBad();
            }
        }

        #endregion

        #region IEdmFunctionBase

        /// <summary>
        /// Returns true if this element is an invalid element or is likely affected by an invalid element.
        /// </summary>
        /// <param name="function">Reference to the calling object.</param>
        /// <returns>This element is an invalid element.</returns>
        public static bool IsBad(this IEdmFunctionBase function)
        {
            EdmUtil.CheckArgumentNull(function, "function");
            if (function.ReturnType == null)
            {
                return ((IEdmElement)function).IsBad();
            }
            else
            {
                return ((IEdmElement)function).IsBad() || function.ReturnType.IsBad();
            }
        }

        /// <summary>
        /// Gets the errors, if any, that belong to this element or other nearby elements that might affect the validity of this element.
        /// </summary>
        /// <param name="function">Reference to the calling object.</param>
        /// <returns>Any errors that belong to this element or other nearby elements that might affect the validity of this element.</returns>
        public static IEnumerable<EdmError> Errors(this IEdmFunctionBase function)
        {
            EdmUtil.CheckArgumentNull(function, "function");
            return function.ReturnType == null ? ((IEdmElement)function).ImmediateErrors() : YieldFirst(function.ImmediateErrors(), function.ReturnType.Errors());
        }

        #endregion

        #region IEdmFunctionParameter

        /// <summary>
        /// Returns true if this element is an invalid element or is likely affected by an invalid element.
        /// </summary>
        /// <param name="parameter">Reference to the calling object.</param>
        /// <returns>This element is an invalid element.</returns>
        public static bool IsBad(this IEdmFunctionParameter parameter)
        {
            EdmUtil.CheckArgumentNull(parameter, "parameter");
            return ((IEdmElement)parameter).IsBad() || parameter.Type.IsBad();
        }

        /// <summary>
        /// Gets the errors, if any, that belong to this element or other nearby elements that might affect the validity of this element.
        /// </summary>
        /// <param name="parameter">Reference to the calling object.</param>
        /// <returns>Any errors that belong to this element or other nearby elements that might affect the validity of this element.</returns>
        public static IEnumerable<EdmError> Errors(this IEdmFunctionParameter parameter)
        {
            EdmUtil.CheckArgumentNull(parameter, "parameter");
            return YieldFirst(parameter.ImmediateErrors(), parameter.Type.Errors());
        }

        #endregion

        private static IEnumerable<EdmError> YieldFirst(params IEnumerable<EdmError>[] enumerableList)
        {
            foreach (var enumerable in enumerableList)
            {
                if (enumerable.Count() > 0)
                {
                    return enumerable;
                }
            }

            return Enumerable.Empty<EdmError>();
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="EdmUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Utilities for Edm.
    /// </summary>
    public static class EdmUtil
    {
        // this is what we should be doing for CDM schemas
        // the RegEx for valid identifiers are taken from the C# Language Specification (2.4.2 Identifiers)
        // (except that we exclude _ as a valid starting character).
        // This results in a somewhat smaller set of identifier from what System.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier
        // allows. Not all identifiers allowed by IsValidLanguageIndependentIdentifier are valid in C#.IsValidLanguageIndependentIdentifier allows:
        //    Mn, Mc, and Pc as a leading character (which the spec and C# (at least for some Mn and Mc characters) do not allow)
        //    characters that Char.GetUnicodeCategory says are in Nl and Cf but which the RegEx does not accept (and which C# does allow).
        //
        // we could create the StartCharacterExp and OtherCharacterExp dynamically to force inclusion of the missing Nl and Cf characters...
        private const string StartCharacterExp = @"[\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}]";
        private const string OtherCharacterExp = @"[\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}]";
        private const string NameExp = StartCharacterExp + OtherCharacterExp + "{0,}";

        // private static Regex ValidDottedName=new Regex(@"^"+NameExp+@"(\."+NameExp+@"){0,}$",RegexOptions.Singleline);
        private static Regex UndottedNameValidator = PlatformHelper.CreateCompiled(@"^" + NameExp + @"$", RegexOptions.Singleline);

        /// <summary>
        /// Checks whether the <paramref name="annotatableProperty"/> has a MIME type annotation.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotation.</param>
        /// <param name="annotatableProperty">The <see cref="IEdmProperty"/> to check.</param>
        /// <returns>The (non-null) value of the MIME type annotation of the <paramref name="annotatableProperty"/> or null if no MIME type annotation exists.</returns>
        public static string GetMimeType(this IEdmModel model, IEdmProperty annotatableProperty)
        {
            return GetStringAnnotationValue(model, annotatableProperty, EdmConstants.MimeTypeAttributeName, () => Strings.EdmUtil_NullValueForMimeTypeAnnotation);
        }

        /// <summary>
        /// Sets the MIME type annotation of the <paramref name="annotatableProperty"/> to <paramref name="mimeType"/>.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotation.</param>
        /// <param name="annotatableProperty">The <see cref="IEdmProperty"/> to modify.</param>
        /// <param name="mimeType">The MIME type value to set as annotation value; if null, an existing annotation will be removed.</param>
        /// <remarks>The MIME type annotation is only supported on service operations and primitive properties for serialization purposes.</remarks>
        public static void SetMimeType(this IEdmModel model, IEdmProperty annotatableProperty, string mimeType)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(annotatableProperty, "annotatableProperty");

            model.SetAnnotation(annotatableProperty, EdmConstants.MimeTypeAttributeName, mimeType);
        }

        /// <summary>
        /// Checks whether the <paramref name="annotatableOperation"/> has a MIME type annotation.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotation.</param>
        /// <param name="annotatableOperation">The <see cref="IEdmOperation"/> to check.</param>
        /// <returns>The (non-null) value of the MIME type annotation of the <paramref name="annotatableOperation"/> or null if no MIME type annotation exists.</returns>
        public static string GetMimeType(this IEdmModel model, IEdmOperation annotatableOperation)
        {
            return GetStringAnnotationValue(model, annotatableOperation, EdmConstants.MimeTypeAttributeName, () => Strings.EdmUtil_NullValueForMimeTypeAnnotation);
        }

        /// <summary>
        /// Gets the symbolic string of an annotated element.
        /// In the next breaking change, it's better to add a property into <see cref="IEdmVocabularyAnnotatable"/>.
        /// </summary>
        /// <param name="annotatedElement">The annotatable element.</param>
        /// <returns>null or a symbolic string.</returns>
        public static string GetSymbolicString(this IEdmVocabularyAnnotatable annotatedElement)
        {
            IEdmSchemaElement schemaElement = annotatedElement as IEdmSchemaElement;
            if (schemaElement != null)
            {
                // EntityType, ComplexType, EnumType, TypeDefinition
                if (schemaElement.SchemaElementKind == EdmSchemaElementKind.TypeDefinition)
                {
                    IEdmType edmType = (IEdmType)schemaElement;
                    switch (edmType.TypeKind)
                    {
                        case EdmTypeKind.Complex:
                            return "ComplexType";
                        case EdmTypeKind.Entity:
                            return "EntityType";
                        case EdmTypeKind.Enum:
                            return "EnumType";
                        case EdmTypeKind.TypeDefinition:
                            return "TypeDefinition";
                        default:
                            return null;
                    }
                }
                else
                {
                    // Action, Function, Term, EntityContainer
                    return schemaElement.SchemaElementKind.ToString();
                }
            }

            IEdmEntityContainerElement containerElement = annotatedElement as IEdmEntityContainerElement;
            if (containerElement != null)
            {
                // ActionImport, FunctionImport, EntitySet, Singleton
                return containerElement.ContainerElementKind.ToString();
            }

            IEdmProperty property = annotatedElement as IEdmProperty;
            if (property != null)
            {
                // NavigationProperty, Property
                switch (property.PropertyKind)
                {
                    case EdmPropertyKind.Navigation:
                        return "NavigationProperty";
                    case EdmPropertyKind.Structural:
                        return "Property";
                    default:
                        return null;
                }
            }

            IEdmExpression expression = annotatedElement as IEdmExpression;
            if (expression != null)
            {
                switch (expression.ExpressionKind)
                {
                    case EdmExpressionKind.FunctionApplication:
                        return "Apply";
                    case EdmExpressionKind.IsType:
                        return "IsOf";
                    case EdmExpressionKind.Labeled:
                        return "LabeledElement";
                    case EdmExpressionKind.Cast:
                    case EdmExpressionKind.Collection:
                    case EdmExpressionKind.If:
                    case EdmExpressionKind.Null:
                    case EdmExpressionKind.Record:
                        return expression.ExpressionKind.ToString();
                    default:
                        return null;
                }
            }

            if (annotatedElement is IEdmOperationParameter)
            {
                return "Parameter";
            }
            else if (annotatedElement is IEdmOperationReturn)
            {
                return "ReturnType";
            }
            else if (annotatedElement is IEdmReference)
            {
                return "Reference";
            }
            else if (annotatedElement is IEdmInclude)
            {
                return "Include";
            }
            else if (annotatedElement is IEdmReferentialConstraint)
            {
                return "ReferentialConstraint";
            }
            else if (annotatedElement is IEdmEnumMember)
            {
                return "Member";
            }
            else if (annotatedElement is IEdmVocabularyAnnotation)
            {
                return "Annotation";
            }
            else if (annotatedElement is IEdmPropertyConstructor)
            {
                return "PropertyValue";
            }

            // It's not supported "Schema, UrlRef, OnDelete"
            return null;
        }

        /// <summary>
        /// Sets the MIME type annotation of the <paramref name="annotatableOperation"/> to <paramref name="mimeType"/>.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotation.</param>
        /// <param name="annotatableOperation">The <see cref="IEdmOperation"/> to modify.</param>
        /// <param name="mimeType">The MIME type value to set as annotation value; if null, an existing annotation will be removed.</param>
        /// <remarks>The MIME type annotation is only supported on service operations and primitive properties for serialization purposes.</remarks>
        public static void SetMimeType(this IEdmModel model, IEdmOperation annotatableOperation, string mimeType)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(annotatableOperation, "annotatableOperation");

            model.SetAnnotation(annotatableOperation, EdmConstants.MimeTypeAttributeName, mimeType);
        }

        /// <summary>
        /// Tries the name of the parse container qualified element.
        /// </summary>
        /// <param name="containerQualifiedElementName">Name of the container qualified container element.</param>
        /// <param name="containerName">Name of the container that was determined.</param>
        /// <param name="containerElementName">The fully qualified name of the container element that was determined.</param>
        /// <returns>Returns true if parsing was successful and false if not.</returns>
        internal static bool TryParseContainerQualifiedElementName(string containerQualifiedElementName, out string containerName, out string containerElementName)
        {
            containerName = null;
            containerElementName = null;

            int indexOfContainerNameAndElementNameSeparator = containerQualifiedElementName.LastIndexOf('.');
            if (indexOfContainerNameAndElementNameSeparator < 0)
            {
                return false;
            }

            containerName = containerQualifiedElementName.Substring(0, indexOfContainerNameAndElementNameSeparator);
            containerElementName = containerQualifiedElementName.Substring(indexOfContainerNameAndElementNameSeparator + 1);
            if (String.IsNullOrEmpty(containerName) || String.IsNullOrEmpty(containerElementName))
            {
                return false;
            }

            return true;
        }

        internal static bool IsNullOrWhiteSpaceInternal(String value)
        {
            return value == null || value.ToCharArray().All(Char.IsWhiteSpace);
        }

        internal static String JoinInternal<T>(String separator, IEnumerable<T> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (separator == null)
            {
                separator = String.Empty;
            }

            using (IEnumerator<T> en = values.GetEnumerator())
            {
                if (!en.MoveNext())
                {
                    return String.Empty;
                }

                StringBuilder result = new StringBuilder();
                if (en.Current != null)
                {
                    // handle the case that the enumeration has null entries
                    // and the case where their ToString() override is broken
                    string value = en.Current.ToString();
                    if (value != null)
                    {
                        result.Append(value);
                    }
                }

                while (en.MoveNext())
                {
                    result.Append(separator);
                    if (en.Current != null)
                    {
                        // handle the case that the enumeration has null entries
                        // and the case where their ToString() override is broken
                        string value = en.Current.ToString();
                        if (value != null)
                        {
                            result.Append(value);
                        }
                    }
                }

                return result.ToString();
            }
        }

        // This is testing if the name can be parsed and serialized, not if it is valid.
        internal static bool IsQualifiedName(string name)
        {
            bool isAllWhiteSpace = true;
            bool hasDot = false;

            foreach (char ch in name)
            {
                if (ch == '.')
                {
                    if (isAllWhiteSpace)
                    {
                        return false;
                    }

                    hasDot = true;
                    isAllWhiteSpace = true;
                }
                else if (!char.IsWhiteSpace(ch))
                {
                    isAllWhiteSpace = false;
                }
            }

            return hasDot && !isAllWhiteSpace;
        }

        internal static bool IsValidUndottedName(string name)
        {
            return (!String.IsNullOrEmpty(name) && UndottedNameValidator.IsMatch(name));
        }

        internal static bool IsValidDottedName(string name)
        {
            // Each part of the dotted name needs to be a valid name.
            return name.Split('.').All(IsValidUndottedName);
        }

        internal static string ParameterizedName(IEdmOperation operation)
        {
            int index = 0;
            int parameterCount = operation.Parameters.Count();
            StringBuilder sb = new StringBuilder();

            UnresolvedOperation unresolvedOperationImport = operation as UnresolvedOperation;
            if (unresolvedOperationImport != null)
            {
                sb.Append(unresolvedOperationImport.Namespace);
                sb.Append("/");
                sb.Append(unresolvedOperationImport.Name);
                return sb.ToString();
            }

            // If we have a operation (rather than a operation import), we want the parameterized name to include the namespace
            IEdmSchemaElement schemaFunction = operation as IEdmSchemaElement;
            if (schemaFunction != null)
            {
                sb.Append(schemaFunction.Namespace);
                sb.Append(".");
            }

            sb.Append(operation.Name);
            sb.Append("(");
            foreach (IEdmOperationParameter parameter in operation.Parameters)
            {
                string typeName = "";
                if (parameter.Type == null)
                {
                    typeName = CsdlConstants.TypeName_Untyped;
                }
                else if (parameter.Type.IsCollection())
                {
                    typeName = CsdlConstants.Value_Collection + "(" + parameter.Type.AsCollection().ElementType().FullName() + ")";
                }
                else if (parameter.Type.IsEntityReference())
                {
                    typeName = CsdlConstants.Value_Ref + "(" + parameter.Type.AsEntityReference().EntityType().FullName() + ")";
                }
                else
                {
                    typeName = parameter.Type.FullName();
                }

                sb.Append(typeName);
                index++;
                if (index < parameterCount)
                {
                    sb.Append(", ");
                }
            }

            sb.Append(")");
            return sb.ToString();
        }

        internal static bool TryGetNamespaceNameFromQualifiedName(string qualifiedName, out string namespaceName, out string name, out string fullName)
        {
            bool foundNamespace = EdmUtil.TryGetNamespaceNameFromQualifiedName(qualifiedName, out namespaceName, out name);

            fullName = EdmUtil.GetFullNameForSchemaElement(namespaceName, name);
            return foundNamespace;
        }

        internal static bool TryGetNamespaceNameFromQualifiedName(string qualifiedName, out string namespaceName, out string name)
        {
            // Qualified name can be a operation import name which is separated by '/'
            int lastSlash = qualifiedName.LastIndexOf('/');
            if (lastSlash < 0)
            {
                // Not a OperationImport
                int lastDot = qualifiedName.LastIndexOf('.');
                if (lastDot < 0)
                {
                    namespaceName = String.Empty;
                    name = qualifiedName;
                    return false;
                }

                namespaceName = qualifiedName.Substring(0, lastDot);
                name = qualifiedName.Substring(lastDot + 1);
                return true;
            }

            namespaceName = qualifiedName.Substring(0, lastSlash);
            name = qualifiedName.Substring(lastSlash + 1);
            return true;
        }

        internal static string FullyQualifiedName(IEdmVocabularyAnnotatable element)
        {
            IEdmSchemaElement schemaElement = element as IEdmSchemaElement;
            if (schemaElement != null)
            {
                IEdmOperation operation = schemaElement as IEdmOperation;
                if (operation != null)
                {
                    return ParameterizedName(operation);
                }
                else
                {
                    return schemaElement.FullName();
                }
            }
            else
            {
                IEdmEntityContainerElement containerElement = element as IEdmEntityContainerElement;
                if (containerElement != null)
                {
                    return containerElement.Container.FullName() + "/" + containerElement.Name;
                }
                else
                {
                    IEdmProperty property = element as IEdmProperty;
                    if (property != null)
                    {
                        IEdmSchemaType declaringSchemaType = property.DeclaringType as IEdmSchemaType;
                        if (declaringSchemaType != null)
                        {
                            string propertyOwnerName = FullyQualifiedName(declaringSchemaType);
                            if (propertyOwnerName != null)
                            {
                                return propertyOwnerName + "/" + property.Name;
                            }
                        }
                    }
                    else
                    {
                        IEdmOperationReturn operationReturn;
                        IEdmEnumMember enumMember;
                        IEdmOperationParameter parameter = element as IEdmOperationParameter;
                        if (parameter != null)
                        {
                            string parameterOwnerName = FullyQualifiedName(parameter.DeclaringOperation);
                            if (parameterOwnerName != null)
                            {
                                return parameterOwnerName + "/" + parameter.Name;
                            }
                        }
                        else if ((enumMember = element as IEdmEnumMember) != null)
                        {
                            string enumMemberOwnerName = FullyQualifiedName(enumMember.DeclaringType);
                            if (enumMemberOwnerName != null)
                            {
                                return enumMemberOwnerName + "/" + enumMember.Name;
                            }
                        }
                        else if ((operationReturn = element as IEdmOperationReturn) != null)
                        {
                            string operationName = FullyQualifiedName(operationReturn.DeclaringOperation);
                            if (operationName != null)
                            {
                                return operationName + "/" + CsdlConstants.OperationReturnExternalTarget;
                            }
                        }
                    }
                }
            }

            return null;
        }

        [DebuggerStepThrough]
        internal static T CheckArgumentNull<T>([ValidatedNotNull]T value, string parameterName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        internal static bool EqualsOrdinal(this string string1, string string2)
        {
            return String.Equals(string1, string2, StringComparison.Ordinal);
        }

        internal static bool EqualsOrdinalIgnoreCase(this string string1, string string2)
        {
            return String.Equals(string1, string2, StringComparison.OrdinalIgnoreCase);
        }


        /// <summary>
        /// Sets the annotation with the OData metadata namespace and the specified <paramref name="localName" /> on the <paramref name="annotatable"/>.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotations."/></param>
        /// <param name="annotatable">The <see cref="IEdmElement"/> to set the annotation on.</param>
        /// <param name="localName">The local name of the annotation to set.</param>
        /// <param name="value">The value of the annotation to set.</param>
        internal static void SetAnnotation(this IEdmModel model, IEdmElement annotatable, string localName, string value)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(annotatable != null, "annotatable != null");
            Debug.Assert(!String.IsNullOrEmpty(localName), "!string.IsNullOrEmpty(localName)");

            IEdmStringValue stringValue = null;
            if (value != null)
            {
                IEdmStringTypeReference typeReference = EdmCoreModel.Instance.GetString(/*nullable*/true);
                stringValue = new EdmStringConstant(typeReference, value);
            }

            model.SetAnnotationValue(annotatable, CsdlConstants.ODataMetadataNamespace, localName, stringValue);
        }

        /// <summary>
        /// Returns the annotation in the OData metadata namespace with the specified <paramref name="localName" />.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotation.</param>
        /// <param name="annotatable">The <see cref="IEdmElement"/> to get the annotation from.</param>
        /// <param name="localName">The local name of the annotation to find.</param>
        /// <param name="value">The value of the annotation in the OData metadata namespace and with the specified <paramref name="localName"/>.</param>
        /// <returns>true if an annotation with the specified local name was found; otherwise false.</returns>
        internal static bool TryGetAnnotation(this IEdmModel model, IEdmElement annotatable, string localName, out string value)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(annotatable != null, "annotatable != null");
            Debug.Assert(!String.IsNullOrEmpty(localName), "!string.IsNullOrEmpty(localName)");

            object annotationValue = model.GetAnnotationValue(annotatable, CsdlConstants.ODataMetadataNamespace, localName);
            if (annotationValue == null)
            {
                value = null;
                return false;
            }

            IEdmStringValue annotationStringValue = annotationValue as IEdmStringValue;
            if (annotationStringValue == null)
            {
                // invalid annotation type found
                throw new InvalidOperationException(Strings.EdmUtil_InvalidAnnotationValue(localName, annotationValue.GetType().FullName));
            }

            value = annotationStringValue.Value;
            return true;
        }

        /// <summary>
        /// Query dictionary for certain key, and update it if not exist
        /// </summary>
        /// <typeparam name="TKey">Key type for dictionary</typeparam>
        /// <typeparam name="TValue">Value type for dictionary</typeparam>
        /// <param name="dictionary">The dictionary to look up</param>
        /// <param name="key">The key property</param>
        /// <param name="computeValue">The function to compute value if key not exist in dictionary</param>
        /// <returns>The value for the key</returns>
        internal static TValue DictionaryGetOrUpdate<TKey, TValue>(
            ConcurrentDictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TKey, TValue> computeValue)
        {
            CheckArgumentNull(dictionary, "dictionary");
            CheckArgumentNull(computeValue, "computeValue");

            return dictionary.GetOrAdd(key, computeValue);
        }

        /// <summary>
        /// Query dictionary for certain key, return default if not present
        /// </summary>
        /// <typeparam name="TKey">Key type for dictionary</typeparam>
        /// <typeparam name="TValue">Value type for dictionary</typeparam>
        /// <param name="dictionary">The dictionary to look up</param>
        /// <param name="key">The key property</param>
        /// <returns>The value for the key, or default if the value does not exist</returns>
        internal static TValue DictionarySafeGet<TKey, TValue>(
            ConcurrentDictionary<TKey, TValue> dictionary,
            TKey key)
        {
            CheckArgumentNull(dictionary, "dictionary");

            TValue val;
            dictionary.TryGetValue(key, out val);
            return val;
        }

        /// <summary>
        /// Gets full name for the schema element with the provided namespace and name
        /// </summary>
        /// <param name="elementNamespace">Namespace of the element</param>
        /// <param name="elementName">The element name</param>
        /// <returns>The full name of the element</returns>
        internal static string GetFullNameForSchemaElement(string elementNamespace, string elementName)
        {
            if (elementName == null)
            {
                return string.Empty;
            }

            if (elementNamespace == null)
            {
                return elementName;
            }

            return elementNamespace + "." + elementName;
        }

        /// <summary>
        /// Checks whether the <paramref name="annotatable"/> has an annotation.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotation.</param>
        /// <param name="annotatable">The <see cref="IEdmElement"/> to check.</param>
        /// <param name="localName">The local name of the annotation to lookup.</param>
        /// <param name="getFoundAnnotationValueErrorString">The error message to throw if a null value is found in an annotation.</param>
        /// <returns>The (non-null) value of the annotation of the <paramref name="annotatable"/> or null if no annotation exists.</returns>
        /// <typeparam name="TEdmElement">Any Type that derives from IEdmElement.</typeparam>
        private static string GetStringAnnotationValue<TEdmElement>(this IEdmModel model, TEdmElement annotatable, string localName, Func<string> getFoundAnnotationValueErrorString) where TEdmElement : class, IEdmElement
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(annotatable, "annotatable");

            string foundValue;
            if (model.TryGetAnnotation(annotatable, localName, out foundValue))
            {
                if (foundValue == null)
                {
                    throw new InvalidOperationException(getFoundAnnotationValueErrorString());
                }

                return foundValue;
            }

            return null;
        }

        // Hack to alert FXCop that we do check for null.
        [AttributeUsage(AttributeTargets.Parameter)]
        private sealed class ValidatedNotNullAttribute : Attribute
        {
        }
    }
}

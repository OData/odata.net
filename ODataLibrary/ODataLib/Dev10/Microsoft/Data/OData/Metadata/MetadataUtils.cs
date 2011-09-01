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

namespace Microsoft.Data.OData.Metadata
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Annotations;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Atom;

    using Strings = Microsoft.Data.OData.Strings;  
    #endregion Namespaces

    /// <summary>
    /// Class with utility methods for dealing with OData metadata.
    /// </summary>
    internal static class MetadataUtils
    {
        /// <summary>
        /// The set of supported primitive CLR types.
        /// </summary>
        private static readonly HashSet<Type> PrimitiveClrTypes = new HashSet<Type>(EqualityComparer<Type>.Default);

        /// <summary>
        /// Constructor.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Need to use the static constructor for the phone platform.")]
        static MetadataUtils()
        {
            PrimitiveClrTypes.Add(typeof(string));
            PrimitiveClrTypes.Add(typeof(Boolean));
            PrimitiveClrTypes.Add(typeof(Boolean?));
            PrimitiveClrTypes.Add(typeof(Byte));
            PrimitiveClrTypes.Add(typeof(Byte?));
            PrimitiveClrTypes.Add(typeof(DateTime));
            PrimitiveClrTypes.Add(typeof(DateTime?));
            PrimitiveClrTypes.Add(typeof(Decimal));
            PrimitiveClrTypes.Add(typeof(Decimal?));
            PrimitiveClrTypes.Add(typeof(Double));
            PrimitiveClrTypes.Add(typeof(Double?));
            PrimitiveClrTypes.Add(typeof(Guid));
            PrimitiveClrTypes.Add(typeof(Guid?));
            PrimitiveClrTypes.Add(typeof(Int16));
            PrimitiveClrTypes.Add(typeof(Int16?));
            PrimitiveClrTypes.Add(typeof(Int32));
            PrimitiveClrTypes.Add(typeof(Int32?));
            PrimitiveClrTypes.Add(typeof(Int64));
            PrimitiveClrTypes.Add(typeof(Int64?));
            PrimitiveClrTypes.Add(typeof(SByte));
            PrimitiveClrTypes.Add(typeof(SByte?));
            PrimitiveClrTypes.Add(typeof(Single));
            PrimitiveClrTypes.Add(typeof(Single?));
            PrimitiveClrTypes.Add(typeof(byte[]));
            PrimitiveClrTypes.Add(typeof(Stream));
        }

        /// <summary>
        /// A method that determines whether a given model is a user model or one of the built-in core models
        /// that can only used for primitive type resolution.
        /// </summary>
        /// <param name="model">The model to check.</param>
        /// <returns>true if the <paramref name="model"/> is a user model; otherwise false.</returns>
        internal static bool IsUserModel(this IEdmModel model)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");

            return !(model is EdmCoreModel);
        }

        /// <summary>
        /// A method that returns the entity sets of the single entity container in the <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The model to get the entity sets for.</param>
        /// <returns>The set of entity sets for the <paramref name="model"/>.</returns>
        internal static IEnumerable<IEdmEntitySet> EntitySets(this IEdmModel model)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");
            Debug.Assert(model.EntityContainers.Count() == 1, "Expected a single entity container.");

            IEdmEntityContainer container = model.EntityContainers.Single();
            return container.Elements.OfType<IEdmEntitySet>();
        }

        /// <summary>
        /// Returns the annotation in the OData metadata namespace with the specified <paramref name="localName" />.
        /// </summary>
        /// <param name="annotatable">The <see cref="IEdmAnnotatable"/> to get the annotation from.</param>
        /// <param name="localName">The local name of the annotation to find.</param>
        /// <returns>The value of the annotation in the OData metadata namespace and with the specified <paramref name="localName"/>; or null if not found.</returns>
        internal static string GetODataAnnotation(this IEdmAnnotatable annotatable, string localName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(annotatable != null, "annotatable != null");
            Debug.Assert(!string.IsNullOrEmpty(localName), "!string.IsNullOrEmpty(localName)");

            object annotationValue = annotatable.GetAnnotation(AtomConstants.ODataMetadataNamespace, localName);
            if (annotationValue == null)
            {
                return null;
            }

            IEdmStringValue annotationStringValue = annotationValue as IEdmStringValue;
            if (annotationStringValue == null)
            {
                // invalid annotation type found
                throw new ODataException(Strings.ODataAtomWriterMetadataUtils_InvalidAnnotationValue(localName, annotationValue.GetType().FullName));
            }

            return annotationStringValue.Value;
        }

        /// <summary>
        /// Sets the annotation with the OData metadata namespace and the specified <paramref name="localName" /> on the <paramref name="annotatable"/>.
        /// </summary>
        /// <param name="annotatable">The <see cref="IEdmAnnotatable"/> to set the annotation on.</param>
        /// <param name="localName">The local name of the annotation to set.</param>
        /// <param name="value">The value of the annotation to set.</param>
        internal static void SetODataAnnotation(this IEdmAnnotatable annotatable, string localName, string value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(annotatable != null, "annotatable != null");
            Debug.Assert(!string.IsNullOrEmpty(localName), "!string.IsNullOrEmpty(localName)");

            IEdmStringValue stringValue = null;
            if (value != null)
            {
                IEdmStringTypeReference typeReference = EdmCoreModel.Instance.GetString(/*nullable*/true);
                stringValue = new EdmStringValue(typeReference, value);
            }

            annotatable.SetAnnotation(AtomConstants.ODataMetadataNamespace, localName, stringValue);
        }

        /// <summary>
        /// Gets all the serializable annotations in the OData metadata namespace on the <paramref name="annotatable"/>.
        /// </summary>
        /// <param name="annotatable">The <see cref="IEdmAnnotatable"/> to get the annotations from.</param>
        /// <returns>All annotations in the OData metadata namespace; or null if no annotations are found.</returns>
        internal static IEnumerable<IEdmAnnotation> GetODataAnnotations(this IEdmAnnotatable annotatable)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(annotatable != null, "annotatable != null");

            if (annotatable.Annotations == null)
            {
                return null;
            }

            return annotatable.Annotations.Where(a => a.Namespace() == AtomConstants.ODataMetadataNamespace);
        }

        /// <summary>
        /// Checks whether the provided <paramref name="clrType"/> is a supported primitive type.
        /// </summary>
        /// <param name="clrType">The CLR type to check.</param>
        /// <returns>true if the <paramref name="clrType"/> is a supported primitive type; otherwise false.</returns>
        internal static bool IsPrimitiveType(Type clrType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(clrType != null, "clrType != null");

            return PrimitiveClrTypes.Contains(clrType);
        }

        /// <summary>
        /// Returns MultiValue item type name or null if the provided type name is not a multiValue.
        /// </summary>
        /// <param name="typeName">MultiValue type name read from payload.</param>
        /// <returns>MultiValue element type name or null if not a multiValue.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "MultiValue is a valid term.")]
        internal static string GetMultiValueItemTypeName(string typeName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(typeName), "!string.IsNullOrEmpty(typeName)");

            return GetMultiValueItemTypeName(typeName, false);
        }

        /// <summary>
        /// Resolves the name of a primitive, complex or multivalue type to the respective type.
        /// </summary>
        /// <param name="model">The model to use or null if no model is available.</param>
        /// <param name="typeName">The name of the type to resolve.</param>
        /// <returns>The <see cref="IEdmType"/> representing the type specified by the <paramref name="typeName"/>;
        /// or null if no such type could be found.</returns>
        internal static IEdmType ResolveTypeName(IEdmModel model, string typeName)
        {
            DebugUtils.CheckNoExternalCallers();
            EdmTypeKind typeKind;
            return ResolveTypeName(model, typeName, out typeKind);
        }

        /// <summary>
        /// Resolves the name of a primitive, complex or multivalue type to the respective type.
        /// </summary>
        /// <param name="model">The model to use or null if no model is available.</param>
        /// <param name="typeName">The name of the type to resolve.</param>
        /// <param name="typeKind">The type kind of the type, if it could be determined. This will be None if we couldn't tell. It might be filled
        /// even if the method returns null, for example for MultiValue types with item types which are not recognized.</param>
        /// <returns>The <see cref="IEdmType"/> representing the type specified by the <paramref name="typeName"/>;
        /// or null if no such type could be found.</returns>
        internal static IEdmType ResolveTypeName(IEdmModel model, string typeName, out EdmTypeKind typeKind)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");
            Debug.Assert(!string.IsNullOrEmpty(typeName), "!string.IsNullOrEmpty(typeName)");
            IEdmType resolvedType = null;

            string itemTypeName = MetadataUtils.GetMultiValueItemTypeName(typeName);
            if (itemTypeName == null)
            {
                resolvedType = model.FindType(typeName) ?? EdmCoreModel.Instance.FindType(typeName);
                typeKind = resolvedType == null ? EdmTypeKind.None : resolvedType.TypeKind;
            }
            else
            {
                // Multivalue
                typeKind = EdmTypeKind.Collection;
                IEdmType itemType = ResolveTypeName(model, itemTypeName);
                if (itemType != null)
                {
                    resolvedType = EdmLibraryExtensions.GetMultiValueType(itemType);
                }
            }

            return resolvedType;
        }

        /// <summary>
        /// Casts an <see cref="IEdmTypeReference"/> to a <see cref="IEdmComplexTypeReference"/> or returns null if this is not supported.
        /// </summary>
        /// <param name="typeReference">The type reference to convert.</param>
        /// <returns>An <see cref="IEdmComplexTypeReference"/> instance or null if the <paramref name="typeReference"/> cannot be converted.</returns>
        internal static IEdmComplexTypeReference AsComplexOrNull(this IEdmTypeReference typeReference)
        {
            DebugUtils.CheckNoExternalCallers();

            if (typeReference == null)
            {
                return null;
            }

            IEdmComplexTypeReference complexTypeReference = typeReference.AsComplex();
            return complexTypeReference.IsBad() ? null : complexTypeReference;
        }

        /// <summary>
        /// Casts an <see cref="IEdmTypeReference"/> to a <see cref="IEdmCollectionTypeReference"/> or returns null if this is not supported.
        /// </summary>
        /// <param name="typeReference">The type reference to convert.</param>
        /// <returns>An <see cref="IEdmCollectionTypeReference"/> instance or null if the <paramref name="typeReference"/> cannot be converted.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "MultiValue is a valid term.")]
        internal static IEdmCollectionTypeReference AsMultiValueOrNull(this IEdmTypeReference typeReference)
        {
            DebugUtils.CheckNoExternalCallers();

            if (typeReference == null)
            {
                return null;
            }

            IEdmCollectionTypeReference collectionTypeReference = typeReference.AsCollection();
            if (!collectionTypeReference.IsAtomic() || collectionTypeReference.IsBad())
            {
                return null;
            }

            return collectionTypeReference;
        }

        /// <summary>
        /// Returns MultiValue item type name or null if the provided type name is not a multiValue.
        /// </summary>
        /// <param name="typeName">MultiValue type name.</param>
        /// <param name="isNested">Whether it is a nested (recursive) call.</param>
        /// <returns>MultiValue element type name or null if not a multiValue.</returns>
        /// <remarks>
        /// The following rules are used for multiValue type names:
        /// - it has to start with "MultiValue(" and end with ")" - trailing and leading whitespaces make the type not to be recognized as multiValue.
        /// - there is to be no characters (including whitespaces) between "MultiValue" and "(" - otherwise it won't berecognized as multiValue
        /// - multiValue item type name has to be a non-empty string - i.e. "MultiValue()" won't be recognized as multiValue
        /// - nested multiValue - e.g. "MultiValue(MultiValue(Edm.Int32))" - are not supported - we will throw
        /// Note the following are examples of valid type names which are not multiValue:
        /// - "MultiValue()"
        /// - " MultiValue(Edm.Int32)"
        /// - "MultiValue (Edm.Int32)"
        /// - "MultiValue("
        /// </remarks>
        private static string GetMultiValueItemTypeName(string typeName, bool isNested)
        {
            int multiValueTypeQualifierLength = EdmConstants.MultiValueTypeQualifier.Length;

            // to be recognized as a multiValue wireTypeName must not be null, has to start with "MultiValue(" and end with ")" and must not be "MultiValue()"
            if (typeName != null &&
                typeName.StartsWith(EdmConstants.MultiValueTypeQualifier + "(", StringComparison.Ordinal) &&
                typeName[typeName.Length - 1] == ')' &&
                typeName.Length != multiValueTypeQualifierLength + 2)
            {
                if (isNested)
                {
                    throw new ODataException(Strings.ValidationUtils_NestedMultiValuesAreNotSupported);
                }

                string innerTypeName = typeName.Substring(multiValueTypeQualifierLength + 1, typeName.Length - (multiValueTypeQualifierLength + 2));

                // Check if it is not a nested multiValue and throw if it is
                GetMultiValueItemTypeName(innerTypeName, true);

                return innerTypeName;
            }

            return null;
        }
    }
}

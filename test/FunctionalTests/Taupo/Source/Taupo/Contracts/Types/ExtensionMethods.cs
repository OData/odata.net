//---------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    using System;

    /// <summary>
    /// Extension methods for manipulating primitive types.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Attaches store type name to the type.
        /// </summary>
        /// <typeparam name="TPrimitiveType">The type of the primitive type.</typeparam>
        /// <param name="primitiveType">Type primitive type.</param>
        /// <param name="baseName">Base name of the base type.</param>
        /// <param name="qualifiedName">Qualified name of the type.</param>
        /// <returns>Instance of <typeparamref name="TPrimitiveType"/> combined 
        /// with <see cref="UnqualifiedDatabaseTypeNameFacet" /> and
        /// <see cref="QualifiedDatabaseTypeNameFacet"/> facets.</returns>
        public static TPrimitiveType WithStoreTypeName<TPrimitiveType>(this TPrimitiveType primitiveType, string baseName, string qualifiedName)
            where TPrimitiveType : PrimitiveDataType
        {
            return primitiveType
                .WithFacet(new UnqualifiedDatabaseTypeNameFacet(baseName))
                .WithFacet(new QualifiedDatabaseTypeNameFacet(qualifiedName))
                .WithDebuggerDisplay(qualifiedName);
        }

        /// <summary>
        /// Withes the name of the unqualified store type.
        /// </summary>
        /// <typeparam name="TPrimitiveType">The type of the primitive type.</typeparam>
        /// <param name="primitiveType">Type of the primitive.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>Type with <see cref="UnqualifiedDatabaseTypeNameFacet"/> attached.</returns>
        public static TPrimitiveType WithUnqualifiedStoreTypeName<TPrimitiveType>(this TPrimitiveType primitiveType, string typeName)
            where TPrimitiveType : PrimitiveDataType
        {
            return primitiveType.WithFacet(new UnqualifiedDatabaseTypeNameFacet(typeName));
        }

        /// <summary>
        /// Sets the size of the type in bits. Commonly used to distinguish between integer or floating point types.
        /// </summary>
        /// <typeparam name="TPrimitiveType">The type of the primitive type.</typeparam>
        /// <param name="primitiveType">The primitive type.</param>
        /// <param name="size">The type size (in bits, common values are 8, 16, 32, 64, etc.).</param>
        /// <returns>The type with the specified size.</returns>
        public static TPrimitiveType WithSize<TPrimitiveType>(this TPrimitiveType primitiveType, int size)
            where TPrimitiveType : PrimitiveDataType
        {
            return primitiveType.WithFacet(new TypeSizeFacet(size));
        }

        /// <summary>
        /// Sets that primitive CLR type to use with this type in CLR context.
        /// </summary>
        /// <typeparam name="TPrimitiveType">The type of the primitive type.</typeparam>
        /// <param name="primitiveType">The primitive type.</param>
        /// <param name="primitiveClrType">The primitive Clr Type.</param>
        /// <returns>The type with the specified primitive CLR type facet.</returns>
        public static TPrimitiveType WithPrimitiveClrType<TPrimitiveType>(this TPrimitiveType primitiveType, Type primitiveClrType)
            where TPrimitiveType : PrimitiveDataType
        {
            return primitiveType.WithFacet(new PrimitiveClrTypeFacet(primitiveClrType));
        }

        /// <summary>
        /// Sets that debugger display string for this type.
        /// </summary>
        /// <typeparam name="TPrimitiveType">The type of the primitive type.</typeparam>
        /// <param name="primitiveType">The primitive type.</param>
        /// <param name="debuggerDisplay">The display string.</param>
        /// <returns>
        /// The type with specified debugger display string.
        /// </returns>
        public static TPrimitiveType WithDebuggerDisplay<TPrimitiveType>(this TPrimitiveType primitiveType, string debuggerDisplay)
            where TPrimitiveType : PrimitiveDataType
        {
            return primitiveType.WithFacet(new DebuggerDisplayFacet(debuggerDisplay));
        }

        /// <summary>
        /// Creates a new instance of the specified type with current facets combined with the new facet.
        /// </summary>
        /// <typeparam name="TPrimitiveType">The type of the primitive type.</typeparam>
        /// <param name="primitiveType">The primitive type.</param>
        /// <param name="facet">The new facet.</param>
        /// <returns>Instance of <typeparamref name="TPrimitiveType"/> with combined facets.</returns>
        /// <remarks>
        /// Only one instance of each facet is allowed on a type. The facet is inserted at the appropriate position 
        /// in the facet list to maintain alphabetically-sorted list. If the facet of the specified type is already 
        /// present in the facet list it gets replaced with a new one.
        /// </remarks>
        public static TPrimitiveType WithFacet<TPrimitiveType>(this TPrimitiveType primitiveType, PrimitiveDataTypeFacet facet)
            where TPrimitiveType : PrimitiveDataType
        {
            return (TPrimitiveType)primitiveType.Combine(facet);
        }

        /// <summary>
        /// Creates a nullable or non-nullable version of the specified primitive type.
        /// </summary>
        /// <typeparam name="TPrimitiveType">The type of the primitive type.</typeparam>
        /// <param name="primitiveType">The primitive type.</param>
        /// <param name="isNullable">If set to <c>true</c> a nullable version will be returned, if false, non-nullable version will be returned.</param>
        /// <returns>Nullable version of the primitive type.</returns>
        public static TPrimitiveType Nullable<TPrimitiveType>(this TPrimitiveType primitiveType, bool isNullable)
            where TPrimitiveType : PrimitiveDataType
        {
            return (TPrimitiveType)primitiveType.Create(isNullable, primitiveType.Facets);
        }

        /// <summary>
        /// Creates a nullable version of the specified primitive type.
        /// </summary>
        /// <typeparam name="TPrimitiveType">The type of the primitive type.</typeparam>
        /// <param name="primitiveType">The primitive type.</param>
        /// <returns>Nullable version of the primitive type.</returns>
        public static TPrimitiveType Nullable<TPrimitiveType>(this TPrimitiveType primitiveType)
            where TPrimitiveType : PrimitiveDataType
        {
            return primitiveType.Nullable(true);
        }

        /// <summary>
        /// Creates a non-nullable version of the specified primitive type.
        /// </summary>
        /// <typeparam name="TPrimitiveType">The type of the primitive type.</typeparam>
        /// <param name="primitiveType">The primitive type.</param>
        /// <returns>Non-nullable version of the primitive type.</returns>
        public static TPrimitiveType NotNullable<TPrimitiveType>(this TPrimitiveType primitiveType)
            where TPrimitiveType : PrimitiveDataType
        {
            return primitiveType.Nullable(false);
        }

        /// <summary>
        /// Clones the the specified primitive type.
        /// </summary>
        /// <typeparam name="TPrimitiveType">The type of the primitive type.</typeparam>
        /// <param name="primitiveType">The primitive type.</param>
        /// <returns>Cloned version of the primitive type.</returns>
        public static TPrimitiveType Clone<TPrimitiveType>(this TPrimitiveType primitiveType)
            where TPrimitiveType : PrimitiveDataType
        {
            return (TPrimitiveType)primitiveType.Create(primitiveType.IsNullable, primitiveType.Facets);
        }
    }
}

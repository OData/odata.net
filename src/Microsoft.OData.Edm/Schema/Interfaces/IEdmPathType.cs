//---------------------------------------------------------------------
// <copyright file="IEdmPathType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Enumerates the kinds of Edm Path type.
    /// </summary>
    public enum EdmPathTypeKind
    {
        /// <summary>
        ///  Represents a path type of unknown kind.
        /// </summary>
        None,

        /// <summary>
        /// Represents a Edm.AnnotationPath type.
        /// </summary>
        Annotation,

        /// <summary>
        /// Represents a Edm.PropertyPath type.
        /// </summary>
        Property,

        /// <summary>
        /// Represents a Edm.NavigationPropertyPath type.
        /// </summary>
        NavigationProperty
    }

    /// <summary>
    /// Represents a definition of a Path type.
    /// </summary>
    public interface IEdmPathType : IEdmSchemaType
    {
        EdmPathTypeKind PathKind { get; }
    }
}

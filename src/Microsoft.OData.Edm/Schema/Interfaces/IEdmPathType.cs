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
        AnnotationPath,

        /// <summary>
        /// Represents a Edm.PropertyPath type.
        /// </summary>
        PropertyPath,

        /// <summary>
        /// Represents a Edm.NavigationPropertyPath type.
        /// </summary>
        NavigationPropertyPath
    }

    /// <summary>
    /// Represents a definition of a Path type.
    /// </summary>
    public interface IEdmPathType : IEdmSchemaType
    {
        /// <summary>
        /// Gets the path kind of this type.
        /// </summary>
        EdmPathTypeKind PathKind { get; }
    }
}

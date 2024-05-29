//---------------------------------------------------------------------
// <copyright file="ODataLibraryCompatibility.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData
{
    /// <summary>
    /// Library compatibility levels.
    /// </summary>
    [Flags]
    public enum ODataLibraryCompatibility
    {
        /// <summary>
        /// No backward compatibility flag set.
        /// </summary>
        None = 0,

        /// <summary>
        /// When enabled, the CSDL Scale and SRID "variable" value will be written "Variable" instead of "variable" for compatibility with older versions of the library.
        /// </summary>
        UseLegacyVariableCasing = 1 << 0,

        /// <summary>
        /// When enabled, writes '@odata.null=true' if a top-level property is single-valued and has null value.
        /// </summary>
        WriteTopLevelODataNullAnnotation = 1 << 1,

        /// <summary>
        /// When enabled, writes @odata.context annotation for navigation property.
        /// </summary>
        WriteODataContextAnnotationForNavProperty = 1 << 2,

        /// <summary>
        /// When enabled, validation for a single-valued top-level property that has a null value doesn't throw an exception.
        /// </summary>
        DoNotThrowExceptionForTopLevelNullProperty = 1 << 3,

        /// <summary>
        /// When enabled, OData nested inner error is serialized with property name as "internalexception".
        /// </summary>
        UseLegacyODataInnerErrorSerialization = 1 << 4,

        /// <summary>
        /// Version 6.x
        /// </summary>
        Version6 = UseLegacyVariableCasing | WriteTopLevelODataNullAnnotation | WriteODataContextAnnotationForNavProperty | DoNotThrowExceptionForTopLevelNullProperty | UseLegacyODataInnerErrorSerialization,

        /// <summary>
        /// Version 7.x
        /// </summary>
        Version7 = UseLegacyVariableCasing | UseLegacyODataInnerErrorSerialization
    }
}

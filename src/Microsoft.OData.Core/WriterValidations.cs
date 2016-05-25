//---------------------------------------------------------------------
// <copyright file="WriterValidations.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;

    /// <summary>
    /// Message writer validation settings.
    /// </summary>
    [Flags]
    public enum WriterValidations
    {
        /// <summary>
        /// No validation will be done during writing.
        /// </summary>
        None = 0,

        /// <summary>
        /// Enable internal validations which are not controlled by the following flags.
        /// </summary>
        BasicValidation = 1,

        /// <summary>
        /// Writers will disallow writing duplicate properties of entries and complex values
        /// (i.e., properties that have the same name).
        /// </summary>
        ThrowOnDuplicatePropertyNames = 2,

        /// <summary>
        /// Writers will disallow writing null values when the metadata specifies a non-nullable primitive type.
        /// </summary>
        ThrowOnNullValuesForNonNullablePrimitiveTypes = 4,

        /// <summary>
        /// Writing undeclared properties will be prohibited.
        /// </summary>
        ThrowOnUndeclaredProperty = 8,

        /// <summary>
        /// Enable all validations.
        /// </summary>
        FullValidation = ~0
    }
 }

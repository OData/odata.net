//---------------------------------------------------------------------
// <copyright file="ValidationKinds.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;

    /// <summary>
    /// Validation kinds used in ODataMessageReaderSettings and ODataMessageWriterSettings.
    /// </summary>
    [Flags]
    public enum ValidationKinds
    {
        /// <summary>
        /// No validations.
        /// </summary>
        None = 0,

        /// <summary>
        /// Disallow duplicate properties in ODataResource (i.e., properties with the same name).
        /// If no duplication can be guaranteed, this flag can be turned off for better performance.
        /// </summary>
        ThrowOnDuplicatePropertyNames = 1,

        /// <summary>
        /// Do not support for undeclared property for non open type.
        /// </summary>
        ThrowOnUndeclaredPropertyForNonOpenType = 2,

        /// <summary>
        /// Validates that the type in input must exactly match the model.
        /// If the input can be guaranteed to be valid, this flag can be turned off for better performance.
        /// </summary>
        ThrowIfTypeConflictsWithMetadata = 4,

        /// <summary>
        /// Enable all validations.
        /// </summary>
        All = ~0
    }
}

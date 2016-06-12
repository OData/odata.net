//---------------------------------------------------------------------
// <copyright file="ReaderValidations.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;

    /// <summary>
    /// Message reader validation settings.
    /// </summary>
    [Flags]
    public enum ReaderValidations
    {
        /// <summary>
        /// Enable no validations.
        /// </summary>
        None = 0,

        /// <summary>
        /// Enable internal validations which are not controlled by the following flags.
        /// </summary>
        BasicValidation = 1,

        /// <summary>
        /// Disallow reading duplicate properties of entries and complex values (i.e., properties with the same name).
        /// </summary>
        ThrowOnDuplicatePropertyNames = 2,

        /// <summary>
        /// Enable strict (non-lax) metadata validation where input must exactly match the model.
        /// </summary>
        StrictMetadataValidation = 4,

        /// <summary>
        /// Disable support for undeclared value property.
        /// </summary>
        ThrowOnUndeclaredValueProperty = 8,

        /// <summary>
        /// Disable support for undeclared link property.
        /// </summary>
        /// <remarks>
        /// Link properties include:
        /// - Navigation links
        /// - Association links
        /// - Stream properties
        /// </remarks>
        ThrowOnUndeclaredLinkProperty = 16,

        /// <summary>
        /// Enable all validations.
        /// </summary>
        FullValidation = ~0
    }
 }

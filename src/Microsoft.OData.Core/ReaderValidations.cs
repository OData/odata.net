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
        /// <remarks>
        /// true - metadata validation is strict, the input must exactly match against the model.
        /// false - metadata validation is lax, the input doesn't have to match the model in all cases.
        /// This property has effect only if the metadata model is specified.
        /// Strict metadata validation:
        ///   Primitive values: The wire type must be convertible to the expected type.
        ///   Complex values: The wire type must resolve against the model and it must exactly match the expected type.
        ///   Entities: The wire type must resolve against the model and it must be assignable to the expected type.
        ///   Collections: The wire type must exactly match the expected type.
        ///   If no expected type is available we use the payload type.
        /// Lax metadata validation:
        ///   Primitive values: If expected type is available, we ignore the wire type.
        ///   Complex values: The wire type is used if the model defines it. If the model doesn't define such a type, the expected type is used.
        ///     If the wire type is not equal to the expected type, but it's assignable, we fail because we don't support complex type inheritance.
        ///     If the wire type if not assignable we use the expected type.
        ///   Entities: same as complex values except that if the payload type is assignable we use the payload type. This allows derived entity types.
        ///   Collections: If expected type is available, we ignore the wire type, except we fail if the item type is a derived complex type.
        ///   If no expected type is available we use the payload type and it must resolve against the model.
        /// If DisablePrimitiveTypeConversion is on, the rules for primitive values don't apply
        ///   and the primitive values are always read with the type from the wire.
        /// </remarks>
        StrictMetadataValidation = 4,

        /// <summary>
        /// Disable support for undeclared property.
        /// </summary>
        ThrowOnUndeclaredProperty = 8,

        /// <summary>
        /// Enable all validations.
        /// </summary>
        FullValidation = ~0
    }
 }

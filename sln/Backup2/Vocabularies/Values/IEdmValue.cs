//---------------------------------------------------------------------
// <copyright file="IEdmValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Defines Edm values
    /// </summary>
    public enum EdmValueKind
    {
        /// <summary>
        /// Represents a value with an unknown or error kind.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents a value implementing <see cref="IEdmBinaryValue"/>.
        /// </summary>
        Binary,

        /// <summary>
        /// Represents a value implementing <see cref="IEdmBooleanValue"/>.
        /// </summary>
        Boolean,

        /// <summary>
        /// Represents a value implementing <see cref="IEdmCollectionValue"/>.
        /// </summary>
        Collection,

        /// <summary>
        /// Represents a value implementing <see cref="IEdmDateTimeOffsetValue"/>.
        /// </summary>
        DateTimeOffset,

        /// <summary>
        /// Represents a value implementing <see cref="IEdmDecimalValue"/>.
        /// </summary>
        Decimal,

        /// <summary>
        /// Represents a value implementing <see cref="IEdmEnumValue"/>.
        /// </summary>
        Enum,

        /// <summary>
        /// Represents a value implementing <see cref="IEdmFloatingValue"/>.
        /// </summary>
        Floating,

        /// <summary>
        /// Represents a value implementing <see cref="IEdmGuidValue"/>.
        /// </summary>
        Guid,

        /// <summary>
        /// Represents a value implementing <see cref="IEdmIntegerValue"/>.
        /// </summary>
        Integer,

        /// <summary>
        /// Represents a value implementing <see cref="IEdmNullValue"/>.
        /// </summary>
        Null,

        /// <summary>
        /// Represents a value implementing <see cref="IEdmStringValue"/>.
        /// </summary>
        String,

        /// <summary>
        /// Represents a value implementing <see cref="IEdmStructuredValue"/>.
        /// </summary>
        Structured,

        /// <summary>
        /// Represents a value implementing <see cref="IEdmDurationValue"/>.
        /// </summary>
        Duration,

        /// <summary>
        /// Represents a value implementing <see cref="IEdmDateValue"/>.
        /// </summary>
        Date,

        /// <summary>
        /// Represents a value implementing <see cref="IEdmTimeOfDayValue"/>.
        /// </summary>
        TimeOfDay,
    }

    /// <summary>
    /// Represents an EDM value.
    /// </summary>
    public interface IEdmValue : IEdmElement
    {
        /// <summary>
        /// Gets the type of this value.
        /// </summary>
        IEdmTypeReference Type { get; }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        EdmValueKind ValueKind { get; }
    }
}

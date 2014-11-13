//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.Edm.Values
{
    /// <summary>
    /// Defines Edm values
    /// </summary>
    public enum EdmValueKind
    {
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
        /// Represents a value implementing <see cref="IEdmDateTimeValue"/>. 
        /// </summary>
        DateTime,

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
        /// Represents a value implementing <see cref="IEdmTimeValue"/>. 
        /// </summary>
        Time,

        /// <summary>
        /// Represents a value with an unknown or error kind.
        /// </summary>
        None,
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

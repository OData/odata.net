//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Values
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

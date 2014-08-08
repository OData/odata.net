//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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

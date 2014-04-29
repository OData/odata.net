//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    /// <summary>
    /// Represents a single property of an entry.
    /// </summary>
    public sealed class ODataProperty : ODataAnnotatable
    {
        /// <summary>
        /// The value of this property, accessed and set by both <seealso cref="Value"/> and <seealso cref="ODataValue"/>.
        /// </summary>
        private ODataValue odataValue;

        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataWriter"/> for this <see cref="ODataProperty"/>.
        /// </summary>
        private ODataPropertySerializationInfo serializationInfo;

        /// <summary>Gets or sets the property name.</summary>
        /// <returns>The property name.</returns>
        public string Name 
        { 
            get; 
            set; 
        }

        /// <summary>Gets or sets the property value.</summary>
        /// <returns>The property value.</returns>
        public object Value
        {
            get
            {
                if (this.odataValue == null)
                {
                    return null;
                }

                return this.odataValue.FromODataValue();
            }

            set
            {
                this.odataValue = value.ToODataValue();
            }
        }

        /// <summary>
        /// Property value, represented as an ODataValue.
        /// </summary>
        /// <remarks>
        /// This value is the same as <see cref="Value"/>, except that primitive types are wrapped 
        /// in an instance of ODataPrimitiveValue, and null values are represented by an instance of ODataNullValue.
        /// </remarks>
        internal ODataValue ODataValue
        {
            get
            {
                return this.odataValue;
            }
        }

        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataWriter"/> for this <see cref="ODataProperty"/>.
        /// </summary>
        internal ODataPropertySerializationInfo SerializationInfo
        {
            get
            {
                return this.serializationInfo;
            }

            set
            {
                this.serializationInfo = value;
            }
        }
    }
}

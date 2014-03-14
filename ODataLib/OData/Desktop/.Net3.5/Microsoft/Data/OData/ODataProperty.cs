//   Copyright 2011 Microsoft Corporation
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

namespace Microsoft.Data.OData
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
                DebugUtils.CheckNoExternalCallers();
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
                DebugUtils.CheckNoExternalCallers();
                return this.serializationInfo;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.serializationInfo = value;
            }
        }
    }
}

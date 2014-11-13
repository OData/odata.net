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

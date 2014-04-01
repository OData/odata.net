//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Spatial;
    using Microsoft.OData.Core.Metadata;

    /// <summary>
    /// Represents a primitive property value.
    /// </summary>
    public sealed class ODataPrimitiveValue : ODataValue
    {
        /// <summary>
        /// Creates a new primitive value from the given CLR value.
        /// </summary>
        /// <param name="value">The primitive to wrap.</param>
        /// <remarks>The primitive value should not be an instance of ODataValue.</remarks>
        public ODataPrimitiveValue(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(Strings.ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromNull, (Exception)null);
            }

            if (!EdmLibraryExtensions.IsPrimitiveType(value.GetType()))
            {
                throw new ODataException(Strings.ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromUnsupportedValueType(value.GetType()));
            }

            this.Value = value;
        }

        /// <summary>
        /// Gets the underlying CLR object wrapped by this <see cref="ODataPrimitiveValue"/>.
        /// </summary>
        /// <value> The underlying primitive CLR value. </value>
        public object Value { get; private set; }
    }
}

//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.OData.Edm.PrimitiveValueConverters
{
    /// <summary>
    /// Class for defining a primitive value conversion for a type definition.
    /// Suppose a type definition defines a primitive type X (underlying type) as a new type Y,
    /// and the type Y has a logically corresponding CLR type Z,
    /// the ConvertToUnderlyingType method converts value from Z to X
    /// and the ConvertFromUnderlyingType method converts value from X to Z.
    /// </summary>
    public interface IPrimitiveValueConverter
    {
        /// <summary>
        /// Converts the given primitive value from the CLR type to the underlying type defined in a type definition.
        /// </summary>
        /// <param name="value">The given CLR value.</param>
        /// <returns>The converted value of the underlying type.</returns>
        object ConvertToUnderlyingType(object value);

        /// <summary>
        /// Converts the given primitive value from the underlying type to the CLR type defined in a type definition.
        /// </summary>
        /// <param name="value">The given value of the CLR type.</param>
        /// <returns>The converted CLR value.</returns>
        object ConvertFromUnderlyingType(object value);
    }
}

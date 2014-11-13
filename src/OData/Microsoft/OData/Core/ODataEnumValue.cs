//   OData .NET Libraries ver. 6.8.1
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

namespace Microsoft.OData.Core
{
    /// <summary>
    /// OData enum value
    /// </summary>
    public sealed class ODataEnumValue : ODataValue
    {
        /// <summary>Constructor</summary>
        /// <param name="value">The backing type, can be "3" or "White" or "Black,Yellow,Red".</param>
        public ODataEnumValue(string value)
        {
            this.Value = value;
            this.TypeName = null;
        }

        /// <summary>Constructor</summary>
        /// <param name="value">The backing type, can be "3" or "White" or "Black,Yellow,Red".</param>
        /// <param name="typeName">The type name in edm model.</param>
        public ODataEnumValue(string value, string typeName)
        {
            this.Value = value;
            this.TypeName = typeName;
        }

        /// <summary>Get backing type value, can be "3" or "White" or "Black,Yellow,Red".</summary>
        public string Value { get; private set; }

        /// <summary>Get the type name in edm model.</summary>
        public string TypeName { get; private set; }
    }
}

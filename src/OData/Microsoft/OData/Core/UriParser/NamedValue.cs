//   OData .NET Libraries ver. 6.9
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

#if ASTORIA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.Core.UriParser
#endif
{
    using Microsoft.OData.Core.UriParser.Syntactic;

    /// <summary>
    /// Class representing a single named value (name and value pair).
    /// </summary>
    internal sealed class NamedValue
    {
        /// <summary>
        /// The name of the value. Or null if the name was not used for this value.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The value - a literal.
        /// </summary>
        private readonly LiteralToken value;

        /// <summary>
        /// Create a new NamedValue lookup given name and value.
        /// </summary>
        /// <param name="name">The name of the value. Or null if the name was not used for this value.</param>
        /// <param name="value">The value - a literal.</param>
        public NamedValue(string name, LiteralToken value)
        {
            ExceptionUtils.CheckArgumentNotNull(value, "value");

            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// The name of the value. Or null if the name was not used for this value.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// The value - a literal.
        /// </summary>
        public LiteralToken Value
        {
            get { return this.value; }
        }
    }
}

//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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

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

namespace Microsoft.Data.OData.Query
{
    /// <summary>
    /// Class representing a single named value (name and value pair).
    /// </summary>
#if INTERNAL_DROP
    internal sealed class NamedValue
#else
    public sealed class NamedValue
#endif
    {
        /// <summary>
        /// The name of the value. Or null if the name was not used for this value.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The value - a literal.
        /// </summary>
        private readonly LiteralQueryToken value;

        /// <summary>
        /// Create a new NamedValue lookup given name and value.
        /// </summary>
        /// <param name="name">The name of the value. Or null if the name was not used for this value.</param>
        /// <param name="value">The value - a literal.</param>
        public NamedValue(string name, LiteralQueryToken value)
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
        public LiteralQueryToken Value
        {
            get { return this.value; }
        }
    }
}

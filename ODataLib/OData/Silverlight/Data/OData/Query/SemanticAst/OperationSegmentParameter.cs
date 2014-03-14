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

namespace Microsoft.Data.OData.Query.SemanticAst
{
    /// <summary>
    /// Represents a named parameter value for invoking an operation in an OData path.
    /// </summary>
    public sealed class OperationSegmentParameter : ODataAnnotatable
    {
        /// <summary>
        /// Initializes a new instance of <see cref="OperationSegmentParameter"/>.
        /// </summary>
        /// <param name="name">The name of the parameter. Cannot be null or empty.</param>
        /// <param name="value">The value of the parameter.</param>
        public OperationSegmentParameter(string name, object value)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(name, "name");
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The parameter value.
        /// </summary>
        public object Value { get; private set; }
    }
}

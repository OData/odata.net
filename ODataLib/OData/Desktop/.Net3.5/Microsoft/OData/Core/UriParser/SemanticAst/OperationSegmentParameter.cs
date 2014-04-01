//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
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

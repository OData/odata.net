//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Represents an aliased parameter in a function call that has not yet been resolved to a specific value.
    /// </summary>
    public class ODataUnresolvedFunctionParameterAlias : ODataValue
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ODataUnresolvedFunctionParameterAlias"/>.
        /// </summary>
        /// <param name="alias">The alias provided as the parameter value.</param>
        /// <param name="type">The EDM type of the parameter represented by this alias.</param>
        public ODataUnresolvedFunctionParameterAlias(string alias, IEdmTypeReference type) 
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(alias, "alias");
            this.Alias = alias;
            this.Type = type;
        }

        /// <summary>
        /// The EDM type of the parameter represented by this alias.
        /// </summary>
        public IEdmTypeReference Type { get; private set; }

        /// <summary>
        /// The alias provided as the parameter value.
        /// </summary>
        public string Alias { get; private set; }
    }
}

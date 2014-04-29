//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Class that knows how to bind ParameterQueryTokens.
    /// </summary>
    internal sealed class RangeVariableBinder
    {
        /// <summary>
        /// Binds a parameter token.
        /// </summary>
        /// <param name="rangeVariableToken">The parameter token to bind.</param>
        /// <param name="state">The state of metadata binding.</param>
        /// <returns>The bound query node.</returns>
        internal static SingleValueNode BindRangeVariableToken(RangeVariableToken rangeVariableToken, BindingState state)
        {
            ExceptionUtils.CheckArgumentNotNull(rangeVariableToken, "rangeVariableToken");
            
            RangeVariable rangeVariable = state.RangeVariables.SingleOrDefault(p => p.Name == rangeVariableToken.Name);

            if (rangeVariable == null)
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_ParameterNotInScope(rangeVariableToken.Name));
            }

            return NodeFactory.CreateRangeVariableReferenceNode(rangeVariable);
        }
    }
}

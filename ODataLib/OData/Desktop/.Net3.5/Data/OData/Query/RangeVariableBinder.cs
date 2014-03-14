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
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Microsoft.Data.OData.Query.SyntacticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

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
            DebugUtils.CheckNoExternalCallers(); 
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

//---------------------------------------------------------------------
// <copyright file="RangeVariableBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using Microsoft.OData.Edm;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.UriParser
{
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

            // If $this is the rangeVariableToken Name, we push a rangeVariable with the same TypeReference as the ImplicitRangevariable
            // to the state.RangeVariables stack.
            if (rangeVariableToken.Name == ExpressionConstants.This)
            {
                // $this range variable will always have the same TypeReference as the ImplicitRangevariable.
                IEdmTypeReference thisRangeVariableTypeReference = state.ImplicitRangeVariable.TypeReference;
                RangeVariable thisRangeVariable = new NonResourceRangeVariable(ExpressionConstants.This, thisRangeVariableTypeReference, null);
                state.RangeVariables.Push(thisRangeVariable);
            }

            RangeVariable rangeVariable = state.RangeVariables.SingleOrDefault(p => p.Name == rangeVariableToken.Name);

            if (rangeVariable == null)
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_ParameterNotInScope(rangeVariableToken.Name));
            }

            return NodeFactory.CreateRangeVariableReferenceNode(rangeVariable);
        }
    }
}
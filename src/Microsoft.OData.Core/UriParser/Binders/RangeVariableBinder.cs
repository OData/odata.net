//---------------------------------------------------------------------
// <copyright file="RangeVariableBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
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

            RangeVariable rangeVariable;

            if (rangeVariableToken.Name == ExpressionConstants.It)
            {
                RangeVariable implicitRangeVariable = state.ImplicitRangeVariable;
                RangeVariable topRangeVariable = state.RangeVariables.Where(p => p.Name == rangeVariableToken.Name).FirstOrDefault();

                // If we added another $it rangeVariable to the Stack (when binding the expandToken),
                // We create the RangeVariableReferenceNode using the added $it rangeVariable.
                // ASSUMPTION: Max 2 $it rangeVariables can be added to the Stack.
                rangeVariable = topRangeVariable != implicitRangeVariable ? topRangeVariable : implicitRangeVariable;
            }
            else
            {
                rangeVariable = state.RangeVariables.FirstOrDefault(p => p.Name == rangeVariableToken.Name);
            }

            if (rangeVariable == null)
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_ParameterNotInScope(rangeVariableToken.Name));
            }

            return NodeFactory.CreateRangeVariableReferenceNode(rangeVariable);
        }
    }
}
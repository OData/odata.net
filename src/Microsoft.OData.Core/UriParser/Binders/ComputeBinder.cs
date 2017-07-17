//---------------------------------------------------------------------
// <copyright file="ComputeBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
    internal sealed class ComputeBinder
    {
        private MetadataBinder.QueryTokenVisitor bindMethod;

        public ComputeBinder(MetadataBinder.QueryTokenVisitor bindMethod)
        {
            this.bindMethod = bindMethod;
        }

        public ComputeClause BindCompute(ComputeToken token)
        {
            ExceptionUtils.CheckArgumentNotNull(token, "token");

            List<ComputeExpression> transformations = new List<ComputeExpression>();
            foreach (ComputeExpressionToken expression in token.Expressions)
            {
                ComputeExpression compute = this.BindComputeExpressionToken(expression);
                transformations.Add(compute);
            }

            return new ComputeClause(transformations);
        }

        private ComputeExpression BindComputeExpressionToken(ComputeExpressionToken token)
        {
            SingleValueNode node = this.bindMethod(token.Expression) as SingleValueNode;
            ComputeExpression expression = new ComputeExpression(node, token.Alias, node.TypeReference);

            return expression;
        }
    }
}

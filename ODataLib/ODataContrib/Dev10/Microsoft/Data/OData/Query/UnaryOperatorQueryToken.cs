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
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Lexical token representing a unary operator.
    /// </summary>
#if INTERNAL_DROP
    internal sealed class UnaryOperatorQueryToken : QueryToken
#else
    public sealed class UnaryOperatorQueryToken : QueryToken
#endif
    {
        /// <summary>
        /// The operator represented by this node.
        /// </summary>
        private readonly UnaryOperatorKind operatorKind;

        /// <summary>
        /// The operand.
        /// </summary>
        private readonly QueryToken operand;

        /// <summary>
        /// Create a new UnaryOperatorQueryToken given the operator and operand
        /// </summary>
        /// <param name="operatorKind">The operator represented by this node.</param>
        /// <param name="operand">The operand.</param>
        public UnaryOperatorQueryToken(UnaryOperatorKind operatorKind, QueryToken operand)
        {
            ExceptionUtils.CheckArgumentNotNull(operand, "operand");

            this.operatorKind = operatorKind;
            this.operand = operand;
        }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.UnaryOperator; }
        }

        /// <summary>
        /// The operator represented by this node.
        /// </summary>
        public UnaryOperatorKind OperatorKind
        {
            get { return this.operatorKind; }
        }

        /// <summary>
        /// The operand.
        /// </summary>
        public QueryToken Operand
        {
            get { return this.operand; }
        }
    }
}

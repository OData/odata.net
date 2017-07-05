//---------------------------------------------------------------------
// <copyright file="MetadataBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using ODataErrorStrings = Microsoft.OData.Strings;

    #endregion Namespaces

    /// <summary>
    /// Binder which applies metadata to a lexical QueryToken tree and produces a bound semantic QueryNode tree.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Keeping the visitor in one place makes sense.")]
    internal class MetadataBinder
    {
        /// <summary>
        /// Encapsulates the state of the metadata binding.
        /// </summary>
        private BindingState bindingState;

        /// <summary>
        /// Constructs a MetadataBinder with the given <paramref name="initialState"/>.
        /// This constructor gets used if you are not calling the top level resource point ParseQuery.
        /// This is an at-your-own-risk constructor, since you must provide valid initial state.
        /// </summary>
        /// <param name="initialState">The initialState to use for binding.</param>
        /// <exception cref="System.ArgumentNullException">Throws if initial state is null.</exception>
        /// <exception cref="System.ArgumentNullException">Throws if initialState.Model is null.</exception>
        internal MetadataBinder(BindingState initialState)
        {
            ExceptionUtils.CheckArgumentNotNull(initialState, "initialState");
            ExceptionUtils.CheckArgumentNotNull(initialState.Model, "initialState.Model");

            this.BindingState = initialState;
        }

        /// <summary>
        /// Delegate for a function that visits a QueryToken and translates it into a bound QueryNode.
        /// TODO: Eventually replace this with a real interface for a visitor.
        /// </summary>
        /// <param name="token">QueryToken to visit.</param>
        /// <returns>Metadata bound QueryNode.</returns>
        internal delegate QueryNode QueryTokenVisitor(QueryToken token);

        /// <summary>
        /// Encapsulates the state of the metadata binding.
        /// </summary>
        internal BindingState BindingState
        {
            get
            {
                return this.bindingState;
            }

            private set
            {
                this.bindingState = value;
            }
        }

        /// <summary>
        /// Processes the skip operator (if any) and returns the combined query.
        /// </summary>
        /// <param name="skip">The skip amount or null if none was specified.</param>
        /// <returns> the skip clause </returns>
        /// <exception cref="ODataException">Throws if skip is less than 0.</exception>
        public static long? ProcessSkip(long? skip)
        {
            if (skip.HasValue)
            {
                if (skip < 0)
                {
                    throw new ODataException(ODataErrorStrings.MetadataBinder_SkipRequiresNonNegativeInteger(skip.ToString()));
                }

                return skip;
            }

            return null;
        }

        /// <summary>
        /// Processes the top operator (if any) and returns the combined query.
        /// </summary>
        /// <param name="top">The top amount or null if none was specified.</param>
        /// <returns> the top clause </returns>
        /// <exception cref="ODataException">Throws if top is less than 0.</exception>
        public static long? ProcessTop(long? top)
        {
            if (top.HasValue)
            {
                if (top < 0)
                {
                    throw new ODataException(ODataErrorStrings.MetadataBinder_TopRequiresNonNegativeInteger(top.ToString()));
                }

                return top;
            }

            return null;
        }

        /// <summary>
        /// Process the remaining query options (represent the set of custom query options after
        /// service operation parameters and system query options have been removed).
        /// </summary>
        /// <param name="bindingState">the current state of the binding algorithm.</param>
        /// <param name="bindMethod">pointer to a binder method.</param>
        /// <returns>The list of <see cref="QueryNode"/> instances after binding.</returns>
        /// <exception cref="ODataException">Throws if bindingState is null.</exception>
        /// <exception cref="ODataException">Throws if bindMethod is null.</exception>
        public static List<QueryNode> ProcessQueryOptions(BindingState bindingState, MetadataBinder.QueryTokenVisitor bindMethod)
        {
            if (bindingState == null || bindingState.QueryOptions == null)
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_QueryOptionsBindStateCannotBeNull);
            }

            if (bindMethod == null)
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_QueryOptionsBindMethodCannotBeNull);
            }

            List<QueryNode> customQueryOptionNodes = new List<QueryNode>();

            foreach (CustomQueryOptionToken queryToken in bindingState.QueryOptions)
            {
                QueryNode customQueryOptionNode = bindMethod(queryToken);
                if (customQueryOptionNode != null)
                {
                    customQueryOptionNodes.Add(customQueryOptionNode);
                }
            }

            bindingState.QueryOptions = null;
            return customQueryOptionNodes;
        }

        /// <summary>
        /// Visits a <see cref="QueryToken"/> in the lexical tree and binds it to metadata producing a semantic <see cref="QueryNode"/>.
        /// </summary>
        /// <param name="token">The query token on the input.</param>
        /// <returns>The bound query node output.</returns>
        protected internal QueryNode Bind(QueryToken token)
        {
            ExceptionUtils.CheckArgumentNotNull(token, "token");
            this.BindingState.RecurseEnter();
            QueryNode result;
            switch (token.Kind)
            {
                case QueryTokenKind.Any:
                    result = this.BindAnyAll((AnyToken)token);
                    break;
                case QueryTokenKind.All:
                    result = this.BindAnyAll((AllToken)token);
                    break;
                case QueryTokenKind.InnerPath:
                    result = this.BindInnerPathSegment((InnerPathToken)token);
                    break;
                case QueryTokenKind.Literal:
                    result = this.BindLiteral((LiteralToken)token);
                    break;
                case QueryTokenKind.StringLiteral:
                    result = this.BindStringLiteral((StringLiteralToken)token);
                    break;
                case QueryTokenKind.BinaryOperator:
                    result = this.BindBinaryOperator((BinaryOperatorToken)token);
                    break;
                case QueryTokenKind.UnaryOperator:
                    result = this.BindUnaryOperator((UnaryOperatorToken)token);
                    break;
                case QueryTokenKind.EndPath:
                    result = this.BindEndPath((EndPathToken)token);
                    break;
                case QueryTokenKind.FunctionCall:
                    result = this.BindFunctionCall((FunctionCallToken)token);
                    break;
                case QueryTokenKind.DottedIdentifier:
                    result = this.BindCast((DottedIdentifierToken)token);
                    break;
                case QueryTokenKind.RangeVariable:
                    result = this.BindRangeVariable((RangeVariableToken)token);
                    break;
                case QueryTokenKind.FunctionParameterAlias:
                    result = this.BindParameterAlias((FunctionParameterAliasToken)token);
                    break;
                case QueryTokenKind.FunctionParameter:
                    result = this.BindFunctionParameter((FunctionParameterToken)token);
                    break;
                default:
                    throw new ODataException(ODataErrorStrings.MetadataBinder_UnsupportedQueryTokenKind(token.Kind));
            }

            if (result == null)
            {
                throw new ODataException(ODataErrorStrings.MetadataBinder_BoundNodeCannotBeNull(token.Kind));
            }

            this.BindingState.RecurseLeave();
            return result;
        }

        /// <summary>
        /// Bind parameter alias (figuring out its type by first parsing and binding its value expression).
        /// </summary>
        /// <param name="functionParameterAliasToken">The alias syntatics token.</param>
        /// <returns>The semantics node for parameter alias.</returns>
        protected virtual SingleValueNode BindParameterAlias(FunctionParameterAliasToken functionParameterAliasToken)
        {
            ParameterAliasBinder binder = new ParameterAliasBinder(this.Bind);
            return binder.BindParameterAlias(this.BindingState, functionParameterAliasToken);
        }

        /// <summary>
        /// Bind a function parameter token
        /// </summary>
        /// <param name="token">The token to bind.</param>
        /// <returns>A semantically bound FunctionCallNode</returns>
        protected virtual QueryNode BindFunctionParameter(FunctionParameterToken token)
        {
            // TODO: extract this into its own binder class.
            if (token.ParameterName != null)
            {
                return new NamedFunctionParameterNode(token.ParameterName, this.Bind(token.ValueToken));
            }

            return this.Bind(token.ValueToken);
        }

        /// <summary>
        /// Binds an InnerPathToken.
        /// </summary>
        /// <param name="token">Token to bind.</param>
        /// <returns>Either a SingleNavigationNode, CollectionNavigationNode, SinglePropertyAccessNode (complex),
        /// or CollectionPropertyAccessNode (primitive or complex) that is the metadata-bound version of the given token.</returns>
        protected virtual QueryNode BindInnerPathSegment(InnerPathToken token)
        {
            InnerPathTokenBinder innerPathTokenBinder = new InnerPathTokenBinder(this.Bind, this.BindingState);
            return innerPathTokenBinder.BindInnerPathSegment(token);
        }

        /// <summary>
        /// Binds a parameter token.
        /// </summary>
        /// <param name="rangeVariableToken">The parameter token to bind.</param>
        /// <returns>The bound query node.</returns>
        protected virtual SingleValueNode BindRangeVariable(RangeVariableToken rangeVariableToken)
        {
            return RangeVariableBinder.BindRangeVariableToken(rangeVariableToken, this.BindingState);
        }

        /// <summary>
        /// Binds a literal token.
        /// </summary>
        /// <param name="literalToken">The literal token to bind.</param>
        /// <returns>The bound literal token.</returns>
        protected virtual QueryNode BindLiteral(LiteralToken literalToken)
        {
            return LiteralBinder.BindLiteral(literalToken);
        }

        /// <summary>
        /// Binds a binary operator token.
        /// </summary>
        /// <param name="binaryOperatorToken">The binary operator token to bind.</param>
        /// <returns>The bound binary operator token.</returns>
        protected virtual QueryNode BindBinaryOperator(BinaryOperatorToken binaryOperatorToken)
        {
            BinaryOperatorBinder binaryOperatorBinder = new BinaryOperatorBinder(this.Bind, this.BindingState.Configuration.Resolver);
            return binaryOperatorBinder.BindBinaryOperator(binaryOperatorToken);
        }

        /// <summary>
        /// Binds a unary operator token.
        /// </summary>
        /// <param name="unaryOperatorToken">The unary operator token to bind.</param>
        /// <returns>The bound unary operator token.</returns>
        protected virtual QueryNode BindUnaryOperator(UnaryOperatorToken unaryOperatorToken)
        {
            UnaryOperatorBinder unaryOperatorBinder = new UnaryOperatorBinder(this.Bind);
            return unaryOperatorBinder.BindUnaryOperator(unaryOperatorToken);
        }

        /// <summary>
        /// Binds a type startPath token.
        /// </summary>
        /// <param name="dottedIdentifierToken">The type startPath token to bind.</param>
        /// <returns>The bound type startPath token.</returns>
        protected virtual QueryNode BindCast(DottedIdentifierToken dottedIdentifierToken)
        {
            DottedIdentifierBinder dottedIdentifierBinder = new DottedIdentifierBinder(this.Bind, this.BindingState);
            return dottedIdentifierBinder.BindDottedIdentifier(dottedIdentifierToken);
        }

        /// <summary>
        /// Binds a LambdaToken.
        /// </summary>
        /// <param name="lambdaToken">The LambdaToken to bind.</param>
        /// <returns>A bound Any or All node.</returns>
        protected virtual QueryNode BindAnyAll(LambdaToken lambdaToken)
        {
            ExceptionUtils.CheckArgumentNotNull(lambdaToken, "LambdaToken");

            LambdaBinder binder = new LambdaBinder(this.Bind);
            return binder.BindLambdaToken(lambdaToken, this.BindingState);
        }

        /// <summary>
        /// Binds a property access token.
        /// </summary>
        /// <param name="endPathToken">The property access token to bind.</param>
        /// <returns>The bound property access token.</returns>
        protected virtual QueryNode BindEndPath(EndPathToken endPathToken)
        {
            EndPathBinder endPathBinder = new EndPathBinder(this.Bind, this.BindingState);
            return endPathBinder.BindEndPath(endPathToken);
        }

        /// <summary>
        /// Binds a function call token.
        /// </summary>
        /// <param name="functionCallToken">The function call token to bind.</param>
        /// <returns>The bound function call token.</returns>
        protected virtual QueryNode BindFunctionCall(FunctionCallToken functionCallToken)
        {
            FunctionCallBinder functionCallBinder = new FunctionCallBinder(this.Bind, this.BindingState);
            return functionCallBinder.BindFunctionCall(functionCallToken);
        }

        /// <summary>
        /// Binds a StringLiteral token.
        /// </summary>
        /// <param name="stringLiteralToken">The StringLiteral token to bind.</param>
        /// <returns>The bound StringLiteral token.</returns>
        protected virtual QueryNode BindStringLiteral(StringLiteralToken stringLiteralToken)
        {
            return new SearchTermNode(stringLiteralToken.Text);
        }
    }
}
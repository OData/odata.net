//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.Query
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Microsoft.Data.OData.Query.SyntacticAst;
    using Microsoft.Data.OData.Query.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    /// <summary>
    /// Bind an entire Uri to Metadata.
    /// </summary>
    internal sealed class ODataUriSemanticBinder
    {
        /// <summary>
        /// The current state of the binding algorithm.
        /// </summary>
        private readonly BindingState bindingState;

        /// <summary>
        /// pointer to the metadata bind method.
        /// </summary>
        private readonly MetadataBinder.QueryTokenVisitor bindMethod;

        /// <summary>
        /// Create a new ODataUriSemanticBinder to bind an entire uri to Metadata.
        /// </summary>
        /// <param name="bindingState">the current state of the binding algorithm</param>
        /// <param name="bindMethod">pointer to the metadata bind method.</param>
        public ODataUriSemanticBinder(BindingState bindingState, MetadataBinder.QueryTokenVisitor bindMethod)
        {
            ExceptionUtils.CheckArgumentNotNull(bindingState, "bindingState");
            ExceptionUtils.CheckArgumentNotNull(bindMethod, "bindMethod");

            this.bindingState = bindingState;
            this.bindMethod = bindMethod;
        }

        /// <summary>
        /// Binds a <see cref="SyntacticTree"/>.
        /// </summary>
        /// <param name="syntax">The query descriptor token to bind.</param>
        /// <returns>The bound query descriptor.</returns>
        public ODataUri BindTree(SyntacticTree syntax)
        {
            ExceptionUtils.CheckArgumentNotNull(syntax, "syntax");
            ExceptionUtils.CheckArgumentNotNull(syntax.Path, "syntax.Path");

            // Make a copy of query options since we may consume some of them as we bind the query
            bindingState.QueryOptions = new List<CustomQueryOptionToken>(syntax.QueryOptions);

            ODataPath path = null;
            FilterClause filter = null;
            OrderByClause orderBy = null;
            long? skip = null;
            long? top = null;
            SelectExpandClause selectExpand = null;
            InlineCountKind? inlineCount = null;

            // First bind the path
            path = ODataPathFactory.BindPath(syntax.Path, this.bindingState.Configuration);

            // If the path leads to a collection, then create a range variable that represents iterating over the collection
            var rangeVariable = NodeFactory.CreateImplicitRangeVariable(path);
            if (rangeVariable != null)
            {
                this.bindingState.RangeVariables.Push(rangeVariable);
            }

            if (syntax.Filter != null || syntax.OrderByTokens.Any())
            {
                this.bindingState.ImplicitRangeVariable = this.bindingState.RangeVariables.Peek();
            }

            // Apply filter first, then order-by, skip, top, select and expand
            filter = BindFilter(syntax, rangeVariable);

            orderBy = BindOrderBy(syntax, rangeVariable, path);

            skip = BindSkip(syntax, rangeVariable, path);

            top = BindTop(syntax, rangeVariable, path);

            selectExpand = BindSelectExpand(syntax, path, this.bindingState.Configuration);

            inlineCount = BindInlineCount(syntax, path);

            // Add the remaining query options to the query descriptor.
            List<QueryNode> boundQueryOptions = MetadataBinder.ProcessQueryOptions(this.bindingState, this.bindMethod);
            Debug.Assert(bindingState.QueryOptions == null, "this.queryOptions == null");
            bindingState.RangeVariables.Pop();
            bindingState.ImplicitRangeVariable = null;

            return new ODataUri(path, boundQueryOptions, selectExpand, filter, orderBy, skip, top, inlineCount);
        }

        /// <summary>
        /// Bind an inline count option
        /// </summary>
        /// <param name="syntax">The inline count option to bind.</param>
        /// <param name="path">the top level path</param>
        /// <returns>an InlineCountKind representing this inline count option</returns>
        public static InlineCountKind? BindInlineCount(SyntacticTree syntax, ODataPath path)
        {
            if (syntax.InlineCount != null)
            {
                if (!path.EdmType().IsEntityCollection())
                {
                    throw new ODataException(ODataErrorStrings.MetadataBinder_QueryOptionNotApplicable("$inlinecount"));
                }

                return syntax.InlineCount.Value;
            }

            return null;
        }

        /// <summary>
        /// Bind a select and expand option.
        /// </summary>
        /// <param name="syntax">A syntax tree containing the select and expand options to bind</param>
        /// <param name="path">the top level path</param>
        /// <param name="configuration">The configuration to use for binding.</param>
        /// <returns>a select expand clause bound to metadata</returns>
        public static SelectExpandClause BindSelectExpand(SyntacticTree syntax, ODataPath path, ODataUriParserConfiguration configuration)
        {
            if (syntax.Select != null || syntax.Expand != null)
            {
                if (!path.EdmType().IsEntityCollection() && !path.EdmType().IsEntity())
                {
                    throw new ODataException(ODataErrorStrings.MetadataBinder_QueryOptionNotApplicable("$select or $expand"));
                }

                return SelectExpandSemanticBinder.Parse((IEdmEntityType)((IEdmCollectionTypeReference)path.EdmType()).ElementType().Definition, path.EntitySet(), syntax.Expand, syntax.Select, configuration);
            }

            return null;
        }

        /// <summary>
        /// Bind a top option
        /// </summary>
        /// <param name="syntax">a syntax tree containing the top option to bind</param>
        /// <param name="rangeVariable">the range variable that iterates over the top level collection</param>
        /// <param name="path">the top level path</param>
        /// <returns>a nullable long representing this top option</returns>
        public static long? BindTop(SyntacticTree syntax, RangeVariable rangeVariable, ODataPath path)
        {
            if (syntax.Top != null)
            {
                if (rangeVariable == null || !path.EdmType().IsEntityCollection())
                {
                    throw new ODataException(ODataErrorStrings.MetadataBinder_QueryOptionNotApplicable("$top"));
                }

                return MetadataBinder.ProcessTop(syntax.Top);
            }

            return null;
        }

        /// <summary>
        /// Bind a skip option
        /// </summary>
        /// <param name="syntax">a syntax tree containing the skip option</param>
        /// <param name="rangeVariable">the range variable that iterates over the top level collection</param>
        /// <param name="path">the top level path.</param>
        /// <returns>a nullable long representing this skip option</returns>
        public static long? BindSkip(SyntacticTree syntax, RangeVariable rangeVariable, ODataPath path)
        {
            if (syntax.Skip != null)
            {
                if (rangeVariable == null || !path.EdmType().IsEntityCollection())
                {
                    throw new ODataException(ODataErrorStrings.MetadataBinder_QueryOptionNotApplicable("$skip"));
                }

                return MetadataBinder.ProcessSkip(syntax.Skip);
            }

            return null;
        }

        /// <summary>
        /// Bind an orderby option
        /// </summary>
        /// <param name="syntax">a syntac tree containing the orderby option</param>
        /// <param name="rangeVariable">the range variable that iterates over the top level collection</param>
        /// <param name="path">the top level path</param>
        /// <returns>an OrderByClause representing this orderby option</returns>
        public OrderByClause BindOrderBy(SyntacticTree syntax, RangeVariable rangeVariable, ODataPath path)
        {
            if (syntax.OrderByTokens != null && syntax.OrderByTokens.Any())
            {
                if (rangeVariable == null || !path.EdmType().IsEntityCollection())
                {
                    throw new ODataException(ODataErrorStrings.MetadataBinder_QueryOptionNotApplicable("$orderby"));
                }

                OrderByBinder orderByBinder = new OrderByBinder(this.bindMethod);
                return orderByBinder.BindOrderBy(this.bindingState, syntax.OrderByTokens);
            }

            return null;
        }

        /// <summary>
        /// Bind a filter option
        /// </summary>
        /// <param name="syntax">a syntactic tree containing the filter option</param>
        /// <param name="rangeVariable">the range variable that iterates over the top level collection.</param>
        /// <returns>A filter clause representing this filter option</returns>
        public FilterClause BindFilter(SyntacticTree syntax, RangeVariable rangeVariable)
        {
            if (syntax.Filter != null)
            {
                if (rangeVariable == null)
                {
                    throw new ODataException(ODataErrorStrings.MetadataBinder_QueryOptionNotApplicable("$filter"));
                }

                FilterBinder filterBinder = new FilterBinder(this.bindMethod, this.bindingState);
                return filterBinder.BindFilter(syntax.Filter);
            }

            return null;
        }
    }
}

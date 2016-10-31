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
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Microsoft.Data.OData.Query.SyntacticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    /// <summary>
    /// Main Public API to parse an ODataURI.
    /// </summary>
    public sealed class ODataUriParser
    {
        /// <summary>
        /// The parser's configuration.
        /// </summary>
        private readonly ODataUriParserConfiguration configuration;

        /// <summary>
        /// Build an ODataUriParser
        /// </summary>
        /// <param name="model">Model to use for metadata binding.</param>
        /// <param name="serviceRoot">Absolute URI of the service root.</param>
        /// <exception cref="System.ArgumentNullException">Throws if input model is null.</exception>
        /// <exception cref="ArgumentException">Throws if the input serviceRoot is not an AbsoluteUri</exception>
        public ODataUriParser(IEdmModel model, Uri serviceRoot)
        {
            this.configuration = new ODataUriParserConfiguration(model, serviceRoot);
        }

        /// <summary>
        /// The settings for this instance of <see cref="ODataUriParser"/>. Refer to the documentation for the individual properties of <see cref="ODataUriParserSettings"/> for more information.
        /// </summary>
        public ODataUriParserSettings Settings
        {
            get { return this.configuration.Settings; }
        }

        /// <summary>
        /// Gets the model for this ODataUriParser
        /// </summary>
        public IEdmModel Model
        {
            get { return this.configuration.Model; }
        }

        /// <summary>
        /// Gets the absolute URI of the service root.
        /// </summary>
        public Uri ServiceRoot
        {
            get { return this.configuration.ServiceRoot; }
        }

        /// <summary>
        /// Gets or Sets the <see cref="ODataUrlConventions"/> to use while parsing, specifically
        /// whether to recognize keys as segments or not.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Throws if the input value is null.</exception>
        public ODataUrlConventions UrlConventions
        {
            get { return this.configuration.UrlConventions; }
            set { this.configuration.UrlConventions = value; }
        }

        /// <summary>
        /// Gets or Sets a callback that returns a BatchReferenceSegment (to be used for $0 in batch)
        /// </summary>
        public Func<string, BatchReferenceSegment> BatchReferenceCallback
        {
            get { return this.configuration.BatchReferenceCallback; }
            set { this.configuration.BatchReferenceCallback = value; }
        }

        /// <summary>
        /// Gets or sets a callback that returns the raw string value for an aliased function parameter.
        /// </summary>
        public Func<string, string> FunctionParameterAliasCallback
        {
            get { return this.configuration.FunctionParameterAliasCallback; }
            set { this.configuration.FunctionParameterAliasCallback = value; }
        }

        /// <summary>
        /// Parses a <paramref name="filter"/> clause on the given <paramref name="elementType"/>, binding
        /// the text into semantic nodes using the provided <paramref name="model"/>.
        /// </summary>
        /// <param name="filter">String representation of the filter expression.</param>
        /// <param name="model">Model to use for metadata binding.</param>
        /// <param name="elementType">Type that the filter clause refers to.</param>
        /// <returns>A <see cref="FilterClause"/> representing the metadata bound filter expression.</returns>
        public static FilterClause ParseFilter(string filter, IEdmModel model, IEdmType elementType)
        {
            ODataUriParser parser = new ODataUriParser(model, null);
            return parser.ParseFilter(filter, elementType, null);
        }

        /// <summary>
        /// Parses a <paramref name="filter"/> clause on the given <paramref name="elementType"/>, binding
        /// the text into semantic nodes using the provided <paramref name="model"/>.
        /// </summary>
        /// <param name="filter">String representation of the filter expression.</param>
        /// <param name="model">Model to use for metadata binding.</param>
        /// <param name="elementType">Type that the filter clause refers to.</param>
        /// <param name="entitySet">EntitySet that the elements beign filtered are from.</param>
        /// <returns>A <see cref="FilterClause"/> representing the metadata bound filter expression.</returns>
        public static FilterClause ParseFilter(string filter, IEdmModel model, IEdmType elementType, IEdmEntitySet entitySet)
        {
            ODataUriParser parser = new ODataUriParser(model, null);
            return parser.ParseFilter(filter, elementType, entitySet);
        }

        /// <summary>
        /// Parses a <paramref name="orderBy"/> clause on the given <paramref name="elementType"/>, binding
        /// the text into semantic nodes using the provided <paramref name="model"/>.
        /// </summary>
        /// <param name="orderBy">String representation of the orderby expression.</param>
        /// <param name="model">Model to use for metadata binding.</param>
        /// <param name="elementType">Type that the orderby clause refers to.</param>
        /// <returns>A <see cref="OrderByClause"/> representing the metadata bound orderby expression.</returns>
        public static OrderByClause ParseOrderBy(string orderBy, IEdmModel model, IEdmType elementType)
        {
            ODataUriParser parser = new ODataUriParser(model, null);
            return parser.ParseOrderBy(orderBy, elementType, null);
        }

        /// <summary>
        /// Parses a <paramref name="orderBy "/> clause on the given <paramref name="elementType"/>, binding
        /// the text into semantic nodes using the provided <paramref name="model"/>.
        /// </summary>
        /// <param name="orderBy">String representation of the orderby expression.</param>
        /// <param name="model">Model to use for metadata binding.</param>
        /// <param name="elementType">Type that the orderby clause refers to.</param>
        /// <param name="entitySet">EntitySet that the elements beign filtered are from.</param>
        /// <returns>A <see cref="OrderByClause"/> representing the metadata bound orderby expression.</returns>
        public static OrderByClause ParseOrderBy(string orderBy, IEdmModel model, IEdmType elementType, IEdmEntitySet entitySet)
        {
            ODataUriParser parser = new ODataUriParser(model, null);
            return parser.ParseOrderBy(orderBy, elementType, entitySet);
        }

        /// <summary>
        /// Parse a filter clause from an instantiated class.
        /// </summary>
        /// <param name="filter">the filter clause to parse</param>
        /// <param name="elementType">Type that the select and expand clauses are projecting.</param>
        /// <param name="entitySet">EntitySet that the elements being filtered are from.</param>
        /// <returns>A FilterClause representing the metadata bound filter expression.</returns>
        public FilterClause ParseFilter(string filter, IEdmType elementType, IEdmEntitySet entitySet)
        {
            return this.ParseFilterImplementation(filter, elementType, entitySet);
        }

        /// <summary>
        /// Parse an orderby clause from an instance of this class
        /// </summary>
        /// <param name="orderBy">the orderby clause to parse</param>
        /// <param name="elementType">Type that the select and expand clauses are projecting.</param>
        /// <param name="entitySet">EntitySet that the elements being filtered are from.</param>
        /// <returns>An OrderByClause representing the metadata bound orderby expression.</returns>
        public OrderByClause ParseOrderBy(string orderBy, IEdmType elementType, IEdmEntitySet entitySet)
        {
            return this.ParseOrderByImplementation(orderBy, elementType, entitySet);
        }

        /// <summary>
        /// Parses a <paramref name="pathUri"/> into a semantic <see cref="ODataPath"/> object. 
        /// </summary>
        /// <remarks>
        /// This is designed to parse the Path of a URL. If it is used to parse paths that are contained
        /// within other places, such as $filter expressions, then it may not enforce correct rules.
        /// </remarks>
        /// <param name="pathUri">The absolute URI which holds the path to parse.</param>
        /// <returns>An <see cref="ODataPath"/> representing the metadata-bound path expression.</returns>
        /// <exception cref="ODataException">Throws if the serviceRoot member is null, or if the input path is not an absolute uri.</exception>
        public ODataPath ParsePath(Uri pathUri)
        {
            ExceptionUtils.CheckArgumentNotNull(pathUri, "pathUri");
            if (this.configuration.ServiceRoot == null)
            {
                throw new ODataException(ODataErrorStrings.UriParser_NeedServiceRootForThisOverload);
            }

            if (!pathUri.IsAbsoluteUri)
            {
                throw new ODataException(ODataErrorStrings.UriParser_UriMustBeAbsolute(pathUri));
            }

            UriPathParser pathParser = new UriPathParser(this.Settings.PathLimit);
            var rawSegments = pathParser.ParsePathIntoSegments(pathUri, this.configuration.ServiceRoot);
            return ODataPathFactory.BindPath(rawSegments, this.configuration);
        }

        /// <summary>
        /// ParseSelectAndExpand from an instantiated class
        /// </summary>
        /// <param name="select">the select to parse</param>
        /// <param name="expand">the expand to parse</param>
        /// <param name="elementType">Type that the select and expand clauses are projecting.</param>
        /// <param name="entitySet">EntitySet that the elements being filtered are from. This can be null, if so that null will propagate through the resulting SelectExpandClause.</param>
        /// <returns>A SelectExpandClause with the semantic representation of select and expand terms</returns>
        public SelectExpandClause ParseSelectAndExpand(string select, string expand, IEdmEntityType elementType, IEdmEntitySet entitySet)
        {
            return this.ParseSelectAndExpandImplementation(select, expand, elementType, entitySet);
        }

        /// <summary>
        /// Parse a full Uri into its contingent parts with semantic meaning attached to each part. 
        /// See <see cref="ODataUri"/>.
        /// </summary>
        /// <param name="fullUri">The full uri to parse.</param>
        /// <returns>An <see cref="ODataUri"/> representing the full uri.</returns>
        internal ODataUri ParseUri(Uri fullUri)
        {
            DebugUtils.CheckNoExternalCallers();
            return this.ParseUriImplementation(fullUri);
        }

        /// <summary>
        /// Parses an inline count option
        /// </summary>
        /// <param name="inlineCount">The inline count string from the query</param>
        /// <returns>An InlineCountKind representing that inline count option.</returns>
        internal InlineCountKind ParseInlineCount(string inlineCount)
        {
            DebugUtils.CheckNoExternalCallers();
            return this.ParseInlineCountImplementation(inlineCount);
        }

        /// <summary>
        /// Parses the full Uri.
        /// </summary>
        /// <param name="fullUri">The full uri to parse</param>
        /// <returns>An ODataUri representing the full uri</returns>
        private ODataUri ParseUriImplementation(Uri fullUri)
        {
            ExceptionUtils.CheckArgumentNotNull(this.configuration.Model, "model");
            ExceptionUtils.CheckArgumentNotNull(this.configuration.ServiceRoot, "serviceRoot");
            ExceptionUtils.CheckArgumentNotNull(fullUri, "fullUri");

            SyntacticTree syntax = SyntacticTree.ParseUri(fullUri, this.configuration.ServiceRoot, this.Settings.FilterLimit);
            ExceptionUtils.CheckArgumentNotNull(syntax, "syntax");
            BindingState state = new BindingState(this.configuration);
            MetadataBinder binder = new MetadataBinder(state);
            ODataUriSemanticBinder uriBinder = new ODataUriSemanticBinder(state, binder.Bind);
            return uriBinder.BindTree(syntax);
        }

        /// <summary>
        /// Parses a <paramref name="filter"/> clause on the given <paramref name="elementType"/>, binding
        /// the text into semantic nodes using the provided.
        /// </summary>
        /// <param name="filter">String representation of the filter expression.</param>
        /// <param name="elementType">Type that the filter clause refers to.</param>
        /// <param name="entitySet">EntitySet that the elements beign filtered are from.</param>
        /// <returns>A <see cref="FilterClause"/> representing the metadata bound filter expression.</returns>
        private FilterClause ParseFilterImplementation(string filter, IEdmType elementType, IEdmEntitySet entitySet)
        {
            ExceptionUtils.CheckArgumentNotNull(this.configuration, "this.configuration");
            ExceptionUtils.CheckArgumentNotNull(elementType, "elementType");
            ExceptionUtils.CheckArgumentNotNull(filter, "filter");

            // Get the syntactic representation of the filter expression
            UriQueryExpressionParser expressionParser = new UriQueryExpressionParser(this.Settings.FilterLimit);
            QueryToken filterToken = expressionParser.ParseFilter(filter);

            // Bind it to metadata
            BindingState state = new BindingState(this.configuration);
            state.ImplicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(elementType.ToTypeReference(), entitySet);
            state.RangeVariables.Push(state.ImplicitRangeVariable);
            MetadataBinder binder = new MetadataBinder(state);
            FilterBinder filterBinder = new FilterBinder(binder.Bind, state);
            FilterClause boundNode = filterBinder.BindFilter(filterToken);

            return boundNode;
        }

        /// <summary>
        /// Parses a <paramref name="orderBy "/> clause on the given <paramref name="elementType"/>, binding
        /// the text into semantic nodes using the provided model.
        /// </summary>
        /// <param name="orderBy">String representation of the orderby expression.</param>
        /// <param name="elementType">Type that the orderby clause refers to.</param>
        /// <param name="entitySet">EntitySet that the elements beign filtered are from.</param>
        /// <returns>A <see cref="OrderByClause"/> representing the metadata bound orderby expression.</returns>
        private OrderByClause ParseOrderByImplementation(string orderBy, IEdmType elementType, IEdmEntitySet entitySet)
        {
            ExceptionUtils.CheckArgumentNotNull(this.configuration.Model, "model");
            ExceptionUtils.CheckArgumentNotNull(elementType, "elementType");
            ExceptionUtils.CheckArgumentNotNull(orderBy, "orderBy");

            // Get the syntactic representation of the filter expression
            UriQueryExpressionParser expressionParser = new UriQueryExpressionParser(this.Settings.OrderByLimit);
            var orderByQueryTokens = expressionParser.ParseOrderBy(orderBy);

            // Bind it to metadata
            BindingState state = new BindingState(this.configuration);
            state.ImplicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(elementType.ToTypeReference(), entitySet);
            state.RangeVariables.Push(state.ImplicitRangeVariable);
            MetadataBinder binder = new MetadataBinder(state);
            OrderByBinder orderByBinder = new OrderByBinder(binder.Bind);
            OrderByClause orderByClause = orderByBinder.BindOrderBy(state, orderByQueryTokens);

            return orderByClause;
        }

        /// <summary>
        /// Parses the <paramref name="select"/> and <paramref name="expand"/> clauses on the given <paramref name="elementType"/>, binding
        /// the text into a metadata-bound list of properties to be selected using the provided model.
        /// </summary>
        /// <param name="select">String representation of the select expression from the URI.</param>
        /// <param name="expand">String representation of the expand expression from the URI.</param>
        /// <param name="elementType">Type that the select and expand clauses are projecting.</param>
        /// <param name="entitySet">EntitySet that the elements being filtered are from.</param>
        /// <returns>A <see cref="SelectExpandClause"/> representing the metadata bound orderby expression.</returns>
        private SelectExpandClause ParseSelectAndExpandImplementation(string select, string expand, IEdmEntityType elementType, IEdmEntitySet entitySet)
        {
            DebugUtils.CheckNoExternalCallers();
            ExceptionUtils.CheckArgumentNotNull(this.configuration.Model, "model");
            ExceptionUtils.CheckArgumentNotNull(elementType, "elementType");

            ISelectExpandTermParser selectParser = SelectExpandTermParserFactory.Create(select, this.Settings);
            SelectToken selectToken = selectParser.ParseSelect();

            ISelectExpandTermParser expandParser = SelectExpandTermParserFactory.Create(expand, this.Settings);
            ExpandToken expandToken = expandParser.ParseExpand();

            return SelectExpandSemanticBinder.Parse(elementType, entitySet, expandToken, selectToken, this.configuration);
        }

        /// <summary>
        /// Parses an inline count option
        /// </summary>
        /// <param name="inlineCount">The inline count string from the query</param>
        /// <returns>An InlineCountKind representing that inline count option.</returns>
        /// <exception cref="ODataException">Throws if the input inlineCount is not a valid $inlinecount value.</exception>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Needs to be instance to avoid collisions with callers.")]
        private InlineCountKind ParseInlineCountImplementation(string inlineCount)
        {
            inlineCount = inlineCount.Trim();
            switch (inlineCount)
            {
                case ExpressionConstants.InlineCountAllPages:
                    {
                        return InlineCountKind.AllPages;
                    }

                case ExpressionConstants.InlineCountNone:
                    {
                        return InlineCountKind.None;
                    }

                default:
                    {
                        throw new ODataException(ODataErrorStrings.ODataUriParser_InvalidInlineCount(inlineCount));
                    }
            }
        }
    }
}

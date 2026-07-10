//---------------------------------------------------------------------
// <copyright file="BindingState.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core;

    /// <summary>
    /// Encapsulates the state of metadata binding.
    /// TODO: finish moving fields from MetadataBinder here and see if anything can be removed.
    /// </summary>
    internal sealed class BindingState
    {
        /// <summary>
        /// The configuration used for binding.
        /// </summary>
        private readonly ODataUriParserConfiguration configuration;

        /// <summary>
        /// The dictionary used to store mappings between Any visitor and corresponding segment paths
        /// </summary>
        private readonly Stack<RangeVariable> rangeVariables = new Stack<RangeVariable>();

        /// <summary>
        /// If there is a  $filter or $orderby, then this member holds the reference to the parameter node for the
        /// implicit parameter ($it) for all expressions.
        /// </summary>
        private RangeVariable implicitRangeVariable;

        /// <summary>
        /// The current recursion depth of binding.
        /// </summary>
        private int bindingRecursionDepth;

        /// <summary>
        /// Collection of query option tokens associated with the current query being processed.
        /// If a given query option is bound it should be removed from this collection.
        /// </summary>
        private List<CustomQueryOptionToken> queryOptions;

        /// <summary>
        /// The parsed segments in path and query option.
        /// </summary>
        private List<ODataPathSegment> parsedSegments = new List<ODataPathSegment>();

        /// <summary>
        /// Constructs a <see cref="BindingState"/> with the given <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration used for binding.</param>
        internal BindingState(ODataUriParserConfiguration configuration)
        {
            ExceptionUtils.CheckArgumentNotNull(configuration, "configuration");
            this.configuration = configuration;
            this.bindingRecursionDepth = 0;
        }

        internal BindingState(ODataUriParserConfiguration configuration, List<ODataPathSegment> parsedSegments)
        {
            ExceptionUtils.CheckArgumentNotNull(configuration, "configuration");
            this.configuration = configuration;
            this.bindingRecursionDepth = 0;
            this.parsedSegments = parsedSegments;
        }

        /// <summary>
        /// Constructs a <see cref="BindingState"/> with the given <paramref name="configuration"/> and an
        /// initial recursion depth. Used when creating an inner binding context (e.g. for a nested
        /// <c>$count($filter=...)</c>) so that the depth budget accumulated by the outer binder is
        /// carried forward and the recursion limit is correctly enforced across nesting levels.
        /// </summary>
        /// <param name="configuration">The configuration used for binding.</param>
        /// <param name="initialRecursionDepth">The recursion depth already consumed by the outer binder.</param>
        internal BindingState(ODataUriParserConfiguration configuration, int initialRecursionDepth)
        {
            ExceptionUtils.CheckArgumentNotNull(configuration, nameof(configuration));
            this.configuration = configuration;
            this.bindingRecursionDepth = initialRecursionDepth;
        }

        /// <summary>
        /// The model used for binding.
        /// </summary>
        internal IEdmModel Model
        {
            get
            {
                return this.configuration.Model;
            }
        }

        /// <summary>
        /// The configuration used for binding.
        /// </summary>
        internal ODataUriParserConfiguration Configuration
        {
            get
            {
                return this.configuration;
            }
        }

        /// <summary>
        /// If there is a  $filter or $orderby, then this member holds the reference to the parameter node for the
        /// implicit parameter ($it) for all expressions.
        /// </summary>
        internal RangeVariable ImplicitRangeVariable
        {
            get
            {
                return this.implicitRangeVariable;
            }

            set
            {
                Debug.Assert(this.implicitRangeVariable == null || value == null, "This should only get set once when first starting to bind a tree.");
                this.implicitRangeVariable = value;
            }
        }

        /// <summary>
        /// The dictionary used to store mappings between Any visitor and corresponding segment paths
        /// </summary>
        internal Stack<RangeVariable> RangeVariables
        {
            get
            {
                return this.rangeVariables;
            }
        }

        /// <summary>
        /// Collection of query option tokens associated with the current query being processed.
        /// If a given query option is bound it should be removed from this collection.
        /// </summary>
        internal List<CustomQueryOptionToken> QueryOptions
        {
            get
            {
                return this.queryOptions;
            }

            set
            {
                this.queryOptions = value;
            }
        }

        /// <summary>
        /// Collection of aggregated property names after applying an aggregate transformation.
        /// </summary>
        internal HashSet<EndPathToken> AggregatedPropertyNames { get; set; }

        /// <summary>
        /// The property set when group by or aggregation is done and properties are collapsed out of scope
        /// </summary>
        internal bool IsCollapsed { get; set; }

        /// <summary>
        /// The property set when entering entity set aggregation context
        /// </summary>
        internal bool InEntitySetAggregation { get; set; }

        /// <summary>
        /// The navigation source at the resource path.
        /// </summary>
        internal IEdmNavigationSource ResourcePathNavigationSource { get; set; }

        /// <summary>
        /// The parsed segments in path and query option.
        /// </summary>
        internal List<ODataPathSegment> ParsedSegments
        {
            get { return parsedSegments; }
        }

        /// <summary>
        /// The current binding recursion depth. Exposed so that nested binding contexts (e.g. for
        /// <c>$count($filter=...)</c>) can inherit the accumulated depth and not inadvertently reset
        /// the recursion counter to zero.
        /// </summary>
        internal int BindingRecursionDepth
        {
            get { return this.bindingRecursionDepth; }
        }

        /// <summary>
        /// Marks the fact that a recursive method was entered, and checks that the depth is allowed.
        /// </summary>
        internal void RecurseEnter()
        {
            this.bindingRecursionDepth++;

            // TODO: add BindingLimit, use uniform error message
            if (this.bindingRecursionDepth > this.configuration.Settings.FilterLimit)
            {
                throw new ODataException(SRResources.UriQueryExpressionParser_TooDeep);
            }
        }

        /// <summary>
        /// Marks the fact that a recursive method is leaving.
        /// </summary>
        internal void RecurseLeave()
        {
            Debug.Assert(this.bindingRecursionDepth > 0, "Decreasing recursion depth below zero, imbalanced recursion calls.");

            this.bindingRecursionDepth--;
        }
    }
}
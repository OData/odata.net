//---------------------------------------------------------------------
// <copyright file="ODataUriParserSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using ODataErrorStrings = Microsoft.OData.Strings;

    /// <summary>
    /// Settings used by <see cref="ODataUriParser"/>.
    /// </summary>
    public sealed class ODataUriParserSettings
    {
        /// <summary>
        /// Default recursive call limit for Filter
        /// </summary>
        internal const int DefaultFilterLimit = 800;

        /// <summary>
        /// Default recursive call limit for OrderBy
        /// </summary>
        internal const int DefaultOrderByLimit = 800;

        /// <summary>
        /// Default tree depth for Select and Expand
        /// </summary>
        internal const int DefaultSelectExpandLimit = 800;

        /// <summary>
        /// Default limit for the path parser.
        /// </summary>
        internal const int DefaultPathLimit = 100;

        /// <summary>
        /// Default limit for the search parser.
        /// </summary>
        internal const int DefaultSearchLimit = 100;

        /// <summary>
        /// the recursive depth of the Syntactic tree for a filter clause
        /// </summary>
        private int filterLimit;

        /// <summary>
        /// the maximum depth of the syntactic tree for an orderby clause
        /// </summary>
        private int orderByLimit;

        /// <summary>
        /// the maximum number of segments in a path
        /// </summary>
        private int pathLimit;

        /// <summary>
        /// the maximum depth of the Syntactic or Semantic tree for a Select or Expand clause
        /// </summary>
        private int selectExpandLimit;

        /// <summary>
        /// The maximum depth of the tree that results from parsing $expand.
        /// </summary>
        private int maxExpandDepth;

        /// <summary>
        /// The maximum number of <see cref="ExpandedNavigationSelectItem"/> instances that can appear in the tree that results from parsing $expand.
        /// </summary>
        private int maxExpandCount;

        /// <summary>
        /// the maximum depth of the syntactic tree for an search clause
        /// </summary>
        private int searchLimit;

        /// <summary>
        /// Initializes a new instance of <see cref="ODataUriParserSettings"/> with default values.
        /// </summary>
        public ODataUriParserSettings()
        {
            this.FilterLimit = DefaultFilterLimit;
            this.OrderByLimit = DefaultOrderByLimit;
            this.PathLimit = DefaultPathLimit;
            this.SelectExpandLimit = DefaultSelectExpandLimit;
            this.SearchLimit = DefaultSearchLimit;

            this.MaximumExpansionDepth = int.MaxValue;
            this.MaximumExpansionCount = int.MaxValue;
        }

        /// <summary>
        /// Gets or sets the maximum depth of the tree that results from parsing $expand.
        /// </summary>
        /// <remarks>
        /// This will be validated after parsing completes, and so should not be used to prevent the instantiation of large trees.
        /// Further, redundant expansions will be pruned before validation and will not count towards the maximum.
        /// </remarks>
        public int MaximumExpansionDepth
        {
            get
            {
                return this.maxExpandDepth;
            }

            set
            {
                if (value < 0)
                {
                    throw new ODataException(ODataErrorStrings.UriParser_NegativeLimit);
                }

                this.maxExpandDepth = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of <see cref="ExpandedNavigationSelectItem"/> instances that can appear in the tree that results from parsing $expand.
        /// </summary>
        /// <remarks>
        /// This will be validated after parsing completes, and so should not be used to prevent the instantiation of large trees.
        /// Further, redundant expansions will be pruned before validation and will not count towards the maximum.
        /// </remarks>
        public int MaximumExpansionCount
        {
            get
            {
                return this.maxExpandCount;
            }

            set
            {
                if (value < 0)
                {
                    throw new ODataException(ODataErrorStrings.UriParser_NegativeLimit);
                }

                this.maxExpandCount = value;
            }
        }

        /// <summary>
        /// Gets or Sets the maximum recursive depth for a select and expand clause, which limits the maximum depth of the tree that can be parsed by the
        /// syntactic parser. This guarantees a set level of performance.
        /// </summary>
        /// <remarks>
        /// The number here doesn't necessarily correspond exactly with the actual maximum recursive depth of the syntactic tree,
        /// i.e  a limit of 20 doesn't necessarily mean that a tree will have depth exactly 20, it may have depth 10 (but never over 20).
        /// Think of it more as an upper bound.
        /// </remarks>
        /// <exception cref="ODataException">Throws if the input value is negative.</exception>
        internal int SelectExpandLimit
        {
            get
            {
                return this.selectExpandLimit;
            }

            set
            {
                if (value < 0)
                {
                    throw new ODataException(ODataErrorStrings.UriParser_NegativeLimit);
                }

                this.selectExpandLimit = value;
            }
        }

        /// <summary>
        /// Gets or Sets the limit on the maximum depth of the filter tree that can be parsed by the
        /// syntactic parser. This guarantees a set level of performance.
        /// </summary>
        /// <remarks>
        /// The number here doesn't necessarily correspond exactly with the actual maximum recursive depth of the syntactic tree,
        /// i.e  a limit of 20 doesn't necessarily mean that a tree will have depth exactly 20, it may have depth 10 (but never over 20).
        /// Think of it more as an upper bound.
        /// </remarks>
        /// <exception cref="ODataException">Throws if the input value is negative.</exception>
        internal int FilterLimit
        {
            get
            {
                return this.filterLimit;
            }

            set
            {
                if (value < 0)
                {
                    throw new ODataException(ODataErrorStrings.UriParser_NegativeLimit);
                }

                this.filterLimit = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum recursive depth for an orderby clause, which limits the maximum depth of the tree that can be parsed by the
        /// syntactic parser. This guarantees a set level of performance.
        /// </summary>
        /// <remarks>
        /// The number here doesn't necessarily correspond exactly with the actual maximum recursive depth of the syntactic tree,
        /// i.e  a limit of 20 doesn't necessarily mean that a tree will have depth exactly 20, it may have depth 10 (but never over 20).
        /// Think of it more as an upper bound.
        /// </remarks>
        /// <exception cref="ODataException">Throws if the input value is negative.</exception>
        internal int OrderByLimit
        {
            get
            {
                return this.orderByLimit;
            }

            set
            {
                if (value < 0)
                {
                    throw new ODataException(ODataErrorStrings.UriParser_NegativeLimit);
                }

                this.orderByLimit = value;
            }
        }

        /// <summary>
        /// Gets or Sets the limit on the maximum number of segments that can be parsed by the
        /// syntactic parser. This guarantees a set level of performance.
        /// </summary>
        /// <remarks>
        /// Unlike Filter, OrderBy, and SelectExpand, this Limit is more concrete, and will
        /// limit the segments to exactly the number that is specified... i.e. a limit of
        /// 20 will throw if and only if there are more than 20 segments in the path.
        /// </remarks>
        /// <exception cref="ODataException">Throws if the input value is negative.</exception>
        internal int PathLimit
        {
            get
            {
                return this.pathLimit;
            }

            set
            {
                if (value < 0)
                {
                    throw new ODataException(ODataErrorStrings.UriParser_NegativeLimit);
                }

                this.pathLimit = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum recursive depth for an search clause, which limits the maximum depth of the tree that can be parsed by the
        /// syntactic parser. This guarantees a set level of performance.
        /// </summary>
        /// <remarks>
        /// The number here doesn't necessarily correspond exactly with the actual maximum recursive depth of the syntactic tree,
        /// i.e  a limit of 20 doesn't necessarily mean that a tree will have depth exactly 20, it may have depth 10 (but never over 20).
        /// Think of it more as an upper bound.
        /// </remarks>
        /// <exception cref="ODataException">Throws if the input value is negative.</exception>
        internal int SearchLimit
        {
            get
            {
                return this.searchLimit;
            }

            set
            {
                if (value < 0)
                {
                    throw new ODataException(ODataErrorStrings.UriParser_NegativeLimit);
                }

                this.searchLimit = value;
            }
        }
    }
}
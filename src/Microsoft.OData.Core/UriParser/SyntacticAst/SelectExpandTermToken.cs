//---------------------------------------------------------------------
// <copyright file="SelectExpandTermToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client.ALinq.UriParser
#else
namespace Microsoft.OData.UriParser
#endif
{
    #region Namespaces
    using System.Collections.Generic;

    #endregion Namespaces

    /// <summary>
    /// Base class for <see cref="ExpandTermToken"/> and <see cref="SelectTermToken"/>.
    /// </summary>
    public abstract class SelectExpandTermToken : QueryToken
    {
        /// <summary>
        /// Initializes a new instance of  <see cref="SelectExpandTermToken"/> class.
        /// </summary>
        /// <param name="pathToProperty">the path to property for this select or expand term</param>
        /// <param name="filterOption">the filter option for this select or expand term</param>
        /// <param name="orderByOptions">the orderby options for this select or expand term</param>
        /// <param name="topOption">the top option for this select or expand term</param>
        /// <param name="skipOption">the skip option for this select or expand term</param>
        /// <param name="countQueryOption">the query count option for this select or expand term</param>
        /// <param name="searchOption">the search option for this select or expand term</param>
        /// <param name="selectOption">the select option for this select or expand term</param>
        /// <param name="computeOption">the compute option for this select or expand term</param>
        protected SelectExpandTermToken(
            PathSegmentToken pathToProperty,
            QueryToken filterOption,
            IEnumerable<OrderByToken> orderByOptions,
            long? topOption,
            long? skipOption,
            bool? countQueryOption,
            QueryToken searchOption,
            SelectToken selectOption,
            ComputeToken computeOption)
        {
            ExceptionUtils.CheckArgumentNotNull(pathToProperty, "property");

            PathToProperty = pathToProperty;
            FilterOption = filterOption;
            OrderByOptions = orderByOptions;
            TopOption = topOption;
            SkipOption = skipOption;
            CountQueryOption = countQueryOption;
            SearchOption = searchOption;
            SelectOption = selectOption;
            ComputeOption = computeOption;
        }

        /// <summary>
        /// Gets the property for this select or expand term.
        /// </summary>
        public PathSegmentToken PathToProperty { get; internal set; }

        /// <summary>
        /// Gets the filter option for this select or expand term.
        /// </summary>
        public QueryToken FilterOption { get; private set; }

        /// <summary>
        /// Gets the orderby options for this select or expand term.
        /// </summary>
        public IEnumerable<OrderByToken> OrderByOptions { get; private set; }

        /// <summary>
        /// Gets the search option for this select or expand term.
        /// </summary>
        public QueryToken SearchOption { get; private set; }

        /// <summary>
        /// Gets the top option for this select or expand term.
        /// </summary>
        public long? TopOption { get; private set; }

        /// <summary>
        /// Gets the skip option for this select or expand term.
        /// </summary>
        public long? SkipOption { get; private set; }

        /// <summary>
        /// Gets the query count option for this select or expand term.
        /// </summary>
        public bool? CountQueryOption { get; private set; }

        /// <summary>
        /// Gets the select option for this select or expand term.
        /// </summary>
        public SelectToken SelectOption { get; internal set; }

        /// <summary>
        /// Gets the compute option for this select or expand term.
        /// </summary>
        public ComputeToken ComputeOption { get; private set; }
    }
}

//---------------------------------------------------------------------
// <copyright file="ExpandSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using Microsoft.OData.Service.Providers;
    #endregion Namespaces

    /// <summary>
    /// Provides a description of a segment in an $expand query option for a WCF Data Service.
    /// </summary>
    /// <remarks>
    /// INTERNAL
    /// Expand providers may replace segments to indicate a different expansion shape. However, they are
    /// unable to set the MaxResultsExpected. The value for the instances created by external providers
    /// will always be Int32.MaxValue, but the value enforced by the serializers will be the one declared
    /// by the data service in the configuration.
    /// 
    /// When the configuration supports a more granular value, we should overload the constructor to make
    /// the MaxResultsExpected property settable as well. 
    /// </remarks>
    [DebuggerDisplay("ExpandSegment ({name},Filter={filter})]")]
    public class ExpandSegment
    {
        #region Private fields

        /// <summary>Container to which the segment belongs; possibly null.</summary>
        private readonly ResourceSetWrapper container;

        /// <summary>Filter expression for this segment on an $expand path.</summary>
        private readonly Expression filter;

        /// <summary>Name for this segment on an $expand path.</summary>
        private readonly string name;

        /// <summary>Property being expanded.</summary>
        private readonly ResourceProperty expandedProperty;

        /// <summary>
        /// The maximum number of results expected for this property; Int32.MaxValue if no limit is expected.
        /// </summary>
        private readonly int maxResultsExpected;

        /// <summary>Collection of ordering information for this segment, used for paging</summary>
        private readonly OrderingInfo orderingInfo;

        /// <summary>Target resource type of the segment.</summary>
        private readonly ResourceType targetResourceType;

        #endregion Private fields

        #region Constructors

        /// <summary>Initializes an <see cref="T:Microsoft.OData.Service.ExpandSegment" /> object with the specified property name and a filtering expression, possibly null.</summary>
        /// <param name="name">The name of the segment to be expanded.</param>
        /// <param name="filter">The filter option in the query to which the expand segment applies.</param>
        public ExpandSegment(string name, Expression filter) 
            : this(name, filter, Int32.MaxValue, null, null, null, null)
        {
        }

        /// <summary>Initializes a new <see cref="ExpandSegment"/> instance.</summary>
        /// <param name="name">Segment name.</param>
        /// <param name="filter">Filter expression for segment, possibly null.</param>
        /// <param name="maxResultsExpected">
        /// Expand providers may choose to return at most MaxResultsExpected + 1 elements to allow the
        /// data service to detect a failure to meet this constraint.
        /// </param>
        /// <param name="container">Container to which the segment belongs; possibly null.</param>
        /// <param name="targetResourceType">Target resource type on which the expansion needs to happen.</param>
        /// <param name="expandedProperty">Property expanded by this expand segment</param>
        /// <param name="orderingInfo">Collection of ordering information for this segment, used for paging</param>
        internal ExpandSegment(
            string name, 
            Expression filter, 
            int maxResultsExpected, 
            ResourceSetWrapper container,
            ResourceType targetResourceType,
            ResourceProperty expandedProperty, 
            OrderingInfo orderingInfo)
        {
            WebUtil.CheckArgumentNull(name, "name");
            CheckFilterType(filter);
            this.name = name;
            this.filter = filter;
            this.container = container;
            this.maxResultsExpected = maxResultsExpected;
            this.expandedProperty = expandedProperty;
            this.orderingInfo = orderingInfo;
            this.targetResourceType = targetResourceType;
        }

        #endregion Constructors

        #region Public properties

        /// <summary>The filter option in the query to which the expand segment applies.</summary>
        /// <returns>An expression that specifies the filter on target data.</returns>
        public Expression Filter
        {
            get { return this.filter; }
        }

        /// <summary>A Boolean value that indicates whether the expand statement is used with a filter expression.</summary>
        /// <returns>True or false.</returns>
        public bool HasFilter
        {
            get { return this.Filter != null; }
        }

        /// <summary>Gets the maximum number of results expected.</summary>
        /// <returns>The integer value that indicates maximum number of results.</returns>
        /// <remarks>
        /// Expand providers may choose to return at most MaxResultsExpected + 1 elements to allow the
        /// data service to detect a failure to meet this constraint.
        /// </remarks>
        public int MaxResultsExpected
        {
            get { return this.maxResultsExpected; }
        }

        /// <summary>The name of the property to be expanded.</summary>
        /// <returns>A string value containing the name of the property.</returns>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>Gets the property to be expanded.</summary>
        /// <returns>The property to expand.</returns>
        public ResourceProperty ExpandedProperty
        {
            get { return this.expandedProperty; }
        }

        /// <summary>Collection of ordering information for this segment, used for paging</summary>
        internal OrderingInfo OrderingInfo
        {
            get
            {
                return this.orderingInfo;
            }
        }

        #endregion Public properties

        #region Internal properties

        /// <summary>Gets the container to which this segment belongs; possibly null.</summary>
        internal ResourceSetWrapper Container
        {
            get { return this.container; }
        }

        /// <summary>The resource type for which <see cref="ExpandedProperty"/> needs to be projected.</summary>
        /// <remarks>Making this internal since we do not want to support this feature on IExpandProvider
        /// which is sort of deprecated.</remarks>
        internal ResourceType TargetResourceType
        {
            get { return this.targetResourceType; }
        }

        #endregion Internal properties

        #region Public methods.

        /// <summary>A Boolean value that indicates whether any segments in the specified <paramref name="path"/> have a filter.</summary>
        /// <returns>True if any of the segments in the path has a filter; false otherwise.</returns>
        /// <param name="path">The enumeration of segments to check for filters.</param>
        public static bool PathHasFilter(IEnumerable<ExpandSegment> path)
        {
            WebUtil.CheckArgumentNull(path, "path");
            return System.Linq.Enumerable.Any(path, segment => segment.HasFilter);
        }

        #endregion Public methods.

        #region Private methods.

        /// <summary>Checks that the specified filter is of the right type.</summary>
        /// <param name="filter">Filter to check.</param>
        private static void CheckFilterType(Expression filter)
        {
            if (filter == null)
            {
                return;
            }

            if (filter.NodeType != ExpressionType.Lambda)
            {
                throw new ArgumentException(Strings.ExpandSegment_FilterShouldBeLambda(filter.NodeType), "filter");
            }

            LambdaExpression lambda = (LambdaExpression)filter;
            if (lambda.Body.Type != typeof(bool) && lambda.Body.Type != typeof(bool?))
            {
                throw new ArgumentException(
                    Strings.ExpandSegment_FilterBodyShouldReturnBool(lambda.Body.Type), "filter");
            }

            if (lambda.Parameters.Count != 1)
            {
                throw new ArgumentException(
                    Strings.ExpandSegment_FilterBodyShouldTakeOneParameter(lambda.Parameters.Count), "filter");
            }
        }

        #endregion Private methods.
    }
}

//---------------------------------------------------------------------
// <copyright file="DataServiceQueryContinuation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    #endregion Namespaces

    /// <summary>Encapsulates a URI that returns the next page of a paged WCF Data Services query result.</summary>
    [DebuggerDisplay("{NextLinkUri}")]
    public abstract class DataServiceQueryContinuation
    {
        #region Private fields

        /// <summary>URI to next page of data.</summary>
        private Uri nextLinkUri;

        /// <summary>Projection plan for results of next page.</summary>
        private ProjectionPlan plan;

#if DEBUG
        /// <summary> True,if this instance is being deserialized via DataContractSerialization, false otherwise </summary>
        private bool deserializing;
#endif
        #endregion Private fields

        #region Constructors

        /// <summary>Initializes a new <see cref="DataServiceQueryContinuation"/> instance.</summary>
        /// <param name="nextLinkUri">URI to next page of data.</param>
        /// <param name="plan">Projection plan for results of next page.</param>
        internal DataServiceQueryContinuation(Uri nextLinkUri, ProjectionPlan plan)
        {
            Debug.Assert(nextLinkUri != null, "nextLinkUri != null");
            Debug.Assert(plan != null, "plan != null");

            this.nextLinkUri = nextLinkUri;
            this.plan = plan;
#if DEBUG
            this.deserializing = false;
#endif
        }

        #endregion Contructors.

        #region Properties

        /// <summary>Gets the URI that is used to return the next page of data from a paged query result.</summary>
        /// <returns>A URI that returns the next page of data.</returns>
        public Uri NextLinkUri
        {
            get
            {
                return this.nextLinkUri;
            }

            internal set
            {
#if DEBUG
                Debug.Assert(this.deserializing, "This property can only be set during deserialization");
#endif
                this.nextLinkUri = value;
            }
        }

        /// <summary>Type of element to be paged over.</summary>
        internal abstract Type ElementType
        {
            get;
        }

        /// <summary>Projection plan for the next page of data; null if not available.</summary>
        internal ProjectionPlan Plan
        {
            get
            {
                return this.plan;
            }

            set
            {
#if DEBUG
                Debug.Assert(this.deserializing, "This property can only be set during deserialization");
#endif
                this.plan = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>Returns the next link URI as a string.</summary>
        /// <returns>A string representation of the next link URI.  </returns>
        public override string ToString()
        {
            return this.NextLinkUri.ToString();
        }

        /// <summary>Creates a new <see cref="DataServiceQueryContinuation"/> instance.</summary>
        /// <param name="nextLinkUri">Link to next page of data (possibly null).</param>
        /// <param name="plan">Plan to materialize the data (only null if nextLinkUri is null).</param>
        /// <returns>A new continuation object; null if nextLinkUri is null.</returns>
        internal static DataServiceQueryContinuation Create(Uri nextLinkUri, ProjectionPlan plan)
        {
            Debug.Assert(plan != null || nextLinkUri == null, "plan != null || nextLinkUri == null");

            if (nextLinkUri == null)
            {
                return null;
            }

            var constructors = typeof(DataServiceQueryContinuation<>).MakeGenericType(plan.ProjectedType).GetInstanceConstructors(false /*isPublic*/);
            object result = Util.ConstructorInvoke(constructors.Single(), new object[] { nextLinkUri, plan });
            return (DataServiceQueryContinuation)result;
        }

        /// <summary>
        /// Initializes a new <see cref="QueryComponents"/> instance that can
        /// be used for this continuation.
        /// </summary>
        /// <returns>A new initializes <see cref="QueryComponents"/>.</returns>
        internal QueryComponents CreateQueryComponents()
        {
            // DSV needs to be 2.0 since $skiptoken will be on the uri.
            QueryComponents result = new QueryComponents(this.NextLinkUri, Util.ODataVersionEmpty, this.Plan.LastSegmentType, null, null);
            return result;
        }
        #endregion Methods
    }

    /// <summary>Encapsulates a URI that returns the next page of a paged WCF Data Services query result. </summary>
        /// <typeparam name="T">The type of continuation token.</typeparam>
    public sealed class DataServiceQueryContinuation<T> : DataServiceQueryContinuation
    {
        #region Contructors.

        /// <summary>Initializes a new typed instance.</summary>
        /// <param name="nextLinkUri">URI to next page of data.</param>
        /// <param name="plan">Projection plan for results of next page.</param>
        internal DataServiceQueryContinuation(Uri nextLinkUri, ProjectionPlan plan)
            : base(nextLinkUri, plan)
        {
        }

        #endregion Contructors.

        #region Properties

        /// <summary>Type of element to be paged over.</summary>
        internal override Type ElementType
        {
            get { return typeof(T); }
        }

        #endregion Properties
    }
}

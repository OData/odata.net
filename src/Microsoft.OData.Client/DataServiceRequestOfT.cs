//---------------------------------------------------------------------
// <copyright file="DataServiceRequestOfT.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Diagnostics;

    #endregion Namespaces

    /// <summary>
    /// Holds a Uri and type for the request.
    /// </summary>
    /// <typeparam name="TElement">The type to construct for the request results</typeparam>
    public sealed class DataServiceRequest<TElement> : DataServiceRequest
    {
        #region Private fields

        /// <summary>The ProjectionPlan for the request (if precompiled in a previous page).</summary>
        private readonly ProjectionPlan plan;

        /// <summary>Request uri for the current request.</summary>
        private Uri requestUri;

        /// <summary>The QueryComponents for the request</summary>
        private QueryComponents queryComponents;

        #endregion Private fields

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceRequest`1" /> class. </summary>
        /// <param name="requestUri">The URI object that contains the request string.</param>
        public DataServiceRequest(Uri requestUri)
        {
            Util.CheckArgumentNull(requestUri, "requestUri");
            this.requestUri = requestUri;
        }

        /// <summary>Create a request for a specific Uri</summary>
        /// <param name="requestUri">The URI for the request.</param>
        /// <param name="queryComponents">The query components for the request</param>
        /// <param name="plan">Projection plan to reuse (possibly null).</param>
        internal DataServiceRequest(Uri requestUri, QueryComponents queryComponents, ProjectionPlan plan)
            : this(requestUri)
        {
            Debug.Assert(requestUri != null, "requestUri != null");
            Debug.Assert(queryComponents != null, "queryComponents != null");

            this.queryComponents = queryComponents;
            this.plan = plan;
        }

        #endregion Constructors

        /// <summary>Gets the type of the object used to create the <see cref="T:Microsoft.OData.Client.DataServiceRequest`1" /> instance.</summary>
        /// <returns>A <see cref="System.Type" /> value that indicates the type of data returned.</returns>
        public override Type ElementType
        {
            get { return typeof(TElement); }
        }

        /// <summary>Gets the URI object that contains the request string. </summary>
        /// <returns>A <see cref="System.Uri" /> object that contains the request string.</returns>
        public override Uri RequestUri
        {
            get
            {
                return this.requestUri;
            }

            internal set
            {
                Debug.Assert(value != null, "RequestUri should never be set to null");
                this.requestUri = value;
            }
        }

        /// <summary>The ProjectionPlan for the request, if precompiled in a previous page; null otherwise.</summary>
        internal override ProjectionPlan Plan
        {
            get
            {
                return this.plan;
            }
        }

        /// <summary>Represents the URI of the query to the data service. </summary>
        /// <returns>The requested URI as a <see cref="T:System.String" /> value.</returns>
        public override string ToString()
        {
            return this.requestUri.ToString();
        }

        /// <summary>The QueryComponents associated with this request</summary>
        /// <param name="model">The client model.</param>
        /// <returns>an instance of QueryComponents.</returns>
        internal override QueryComponents QueryComponents(ClientEdmModel model)
        {
            if (this.queryComponents == null)
            {
                Type elementType = typeof(TElement);

                // for 1..* navigation properties we need the type of the entity of the collection that is being navigated to. Otherwise we use TElement.
                elementType = PrimitiveType.IsKnownType(elementType) || WebUtil.IsCLRTypeCollection(elementType, model) ? elementType : TypeSystem.GetElementType(elementType);
                this.queryComponents = new QueryComponents(this.requestUri, Util.ODataVersionEmpty, elementType, null, null);
            }

            return this.queryComponents;
        }
    }
}

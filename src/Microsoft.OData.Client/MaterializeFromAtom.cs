//---------------------------------------------------------------------
// <copyright file="MaterializeFromAtom.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Xml;
    using Microsoft.OData.Client.Materialization;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData;

    #endregion Namespaces

    /// <summary>
    /// Use this class to materialize objects from an application/atom+xml stream.
    /// </summary>
    internal class MaterializeAtom : IDisposable, IEnumerable, IEnumerator
    {
        #region Private fields

        /// <summary>Backreference to the context to allow type resolution.</summary>
        private readonly ResponseInfo responseInfo;

        /// <summary>base type of the object to create from the stream.</summary>
        private readonly Type elementType;

        /// <summary>when materializing a known type (like string)</summary>
        /// <remarks>&lt;property&gt; text-value &lt;/property&gt;</remarks>
        private readonly bool expectingPrimitiveValue;

        /// <summary>Materializer from which instances are read.</summary>
        /// <remarks>
        /// The log for the materializer stores all changes for the
        /// associated data context.
        /// </remarks>
        private readonly ODataMaterializer materializer;

        /// <summary>untyped current object</summary>
        private object current;

        /// <summary>has GetEnumerator been called?</summary>
        private bool calledGetEnumerator;

        /// <summary>Whether anything has been read.</summary>
        private bool moved;

        /// <summary>
        /// output writer, set using reflection
        /// </summary>
#if DEBUG
        private System.IO.TextWriter writer = new System.IO.StringWriter(System.Globalization.CultureInfo.InvariantCulture);
#else
#pragma warning disable 649
        private System.IO.TextWriter writer;
#pragma warning restore 649
#endif

        #endregion Private fields

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="responseInfo">originating context</param>
        /// <param name="queryComponents">Query components (projection, expected type)</param>
        /// <param name="plan">Projection plan (if compiled in an earlier query).</param>
        /// <param name="responseMessage">responseMessage</param>
        /// <param name="payloadKind">The kind of the payload to materialize.</param>
        internal MaterializeAtom(
            ResponseInfo responseInfo,
            QueryComponents queryComponents,
            ProjectionPlan plan,
            IODataResponseMessage responseMessage,
            ODataPayloadKind payloadKind)
        {
            Debug.Assert(queryComponents != null, "queryComponents != null");

            this.responseInfo = responseInfo;
            this.elementType = queryComponents.LastSegmentType;
            this.expectingPrimitiveValue = PrimitiveType.IsKnownNullableType(elementType);

            Debug.Assert(responseMessage != null, "Response message is null! Did you mean to use Materializer.ResultsWrapper/EmptyResults?");

            Type implementationType;
            Type materializerType = GetTypeForMaterializer(this.expectingPrimitiveValue, this.elementType, responseInfo.Model, out implementationType);
            this.materializer = ODataMaterializer.CreateMaterializerForMessage(responseMessage, responseInfo, materializerType, queryComponents, plan, payloadKind);
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="responseInfo">originating context</param>
        /// <param name="entries">entries that needs to be materialized.</param>
        /// <param name="elementType">result type.</param>
        /// <param name="format">The format of the response being materialized from.</param>
        internal MaterializeAtom(ResponseInfo responseInfo, IEnumerable<ODataResource> entries, Type elementType, ODataFormat format)
        {
            this.responseInfo = responseInfo;
            this.elementType = elementType;
            this.expectingPrimitiveValue = PrimitiveType.IsKnownNullableType(elementType);

            Type implementationType;
            Type materializerType = GetTypeForMaterializer(this.expectingPrimitiveValue, this.elementType, responseInfo.Model, out implementationType);
            QueryComponents qc = new QueryComponents(null, Util.ODataVersionEmpty, elementType, null, null);
            ODataMaterializerContext context = new ODataMaterializerContext(responseInfo);
            EntityTrackingAdapter entityTrackingAdapter = new EntityTrackingAdapter(responseInfo.EntityTracker, responseInfo.MergeOption, responseInfo.Model, responseInfo.Context);
            this.materializer = new ODataEntriesEntityMaterializer(entries, context, entityTrackingAdapter, qc, materializerType, null, format);
        }

        /// <summary>
        /// Private internal constructor used for creating empty wrapper.
        /// </summary>
        private MaterializeAtom()
        {
        }

        #region Current

        /// <summary>Loosely typed current object property.</summary>
        /// <remarks>
        /// For collection properties this property will be of AtomContentProperties to allow further materialization processing.
        /// Otherwise the value should be of the right type, as the materializer takes the expected type into account.
        /// </remarks>
        public object Current
        {
            get
            {
                object currentValue = this.current;
                return currentValue;
            }
        }

        #endregion

        /// <summary>
        /// A materializer for empty results
        /// </summary>
        internal static MaterializeAtom EmptyResults
        {
            get
            {
                return new ResultsWrapper(null, null, null);
            }
        }

        /// <summary>
        /// if the response from server is null (no-content)
        /// </summary>
        /// <returns>return true if it no content.</returns>
        internal bool IsNoContentResponse()
        {
            ResultsWrapper wrapper = this as ResultsWrapper;
            return wrapper != null && wrapper.IsEmptyResult();
        }

        /// <summary>
        /// Returns true if the underlying object used for counting is available
        /// </summary>
        internal bool IsCountable
        {
            get { return this.materializer != null && this.materializer.IsCountable; }
        }

        /// <summary>
        /// The data service context object this materializer belongs to
        /// </summary>
        internal virtual DataServiceContext Context
        {
            get { return this.responseInfo.Context; }
        }

        /// <summary>
        /// The action to set instance annotation for property or entry or feed
        /// </summary>
        internal Action<IDictionary<string, object>> SetInstanceAnnotations
        {
            get
            {
                return this.materializer == null ? null : this.materializer.SetInstanceAnnotations;
            }
            set
            {
                if (this.materializer != null)
                {
                    this.materializer.SetInstanceAnnotations = value;
                }
            }
        }

        #region IDisposable
        /// <summary>
        /// dispose
        /// </summary>
        public void Dispose()
        {
            this.current = null;

            if (this.materializer != null)
            {
                this.materializer.Dispose();
            }

            if (this.writer != null)
            {
                this.writer.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        #endregion

        #region IEnumerable
        /// <summary>
        /// as IEnumerable
        /// </summary>
        /// <returns>this</returns>
        public virtual IEnumerator GetEnumerator()
        {
            this.CheckGetEnumerator();
            return this;
        }
        #endregion

        /// <summary>
        /// Gets the type that of the instances that will be returned by materializer.
        /// </summary>
        /// <param name="expectingPrimitiveValue">Whether the expected is a primitive type.</param>
        /// <param name="elementType">Actual type on the client.</param>
        /// <param name="model">The client model used.</param>
        /// <param name="implementationType">The actual type that implements ICollection&lt;&gt;</param>
        /// <returns>Type of the instances that will be returned by materializer.</returns>
        /// <remarks>
        /// For collection navigation properties (i.e. ICollection&lt;T&gt; where T is an entity the method returns T. Materializer will
        /// return single entity each time MoveNext() is called. However for collection properties we need the whole property instead of just a
        /// single collection item.
        /// </remarks>
        private static Type GetTypeForMaterializer(bool expectingPrimitiveValue, Type elementType, ClientEdmModel model, out Type implementationType)
        {
            if (!expectingPrimitiveValue && typeof(IEnumerable).IsAssignableFrom(elementType))
            {
                implementationType = ClientTypeUtil.GetImplementationType(elementType, typeof(ICollection<>));
                if (implementationType != null)
                {
                    Type expectedType = implementationType.GetGenericArguments()[0]; // already know its ICollection<>

                    // We should use the inner type only if this is a collection of entities (as opposed to a collection of primitive or complex types)
                    if (ClientTypeUtil.TypeIsStructured(expectedType, model))
                    {
                        return expectedType;
                    }
                }
            }

            implementationType = null;
            return elementType;
        }

        /// <summary>
        /// Creates the next object from the stream.
        /// </summary>
        /// <returns>false if stream is finished</returns>
        public bool MoveNext()
        {
            bool applying = this.responseInfo.ApplyingChanges;
            try
            {
                this.responseInfo.ApplyingChanges = true;
                return this.MoveNextInternal();
            }
            finally
            {
                this.responseInfo.ApplyingChanges = applying;
            }
        }

        /// <summary>
        /// Creates the next object from the stream.
        /// </summary>
        /// <returns>false if stream is finished</returns>
        private bool MoveNextInternal()
        {
            // For empty results using ResultsWrapper, just return false.
            if (this.materializer == null)
            {
                Debug.Assert(this.current == null, "this.current == null -- otherwise this.materializer should have some value.");
                return false;
            }

            this.current = null;
            this.materializer.ClearLog();

            bool result = false;
            Type implementationType;
            GetTypeForMaterializer(this.expectingPrimitiveValue, this.elementType, this.responseInfo.Model, out implementationType);

            // if implementationType is not null this is a collection of entities
            if (implementationType != null)
            {
                // this is for CreateQuery/Execute<Collection<T>>
                // We should only "move" once in here, in the first iteration we will load everything and return true
                // Second iteration we will always return false.

                if (this.moved)
                {
                    return false;
                }

                Type expectedType = implementationType.GetGenericArguments()[0]; // already know its IList<>
                implementationType = this.elementType;
                if (implementationType.IsInterface())
                {
                    implementationType = typeof(System.Collections.ObjectModel.Collection<>).MakeGenericType(expectedType);
                }

                IList list = (IList)Activator.CreateInstance(implementationType);

                while (this.materializer.Read())
                {
                    list.Add(this.materializer.CurrentValue);
                }

                this.moved = true;

                this.current = list;
                result = true;
            }

            if (this.current == null)
            {
                if (this.expectingPrimitiveValue && this.moved)
                {
                    result = false;
                }
                else
                {
                    result = this.materializer.Read();
                    if (result)
                    {
                        if (this.materializer.CurrentValue is IBaseEntityType entity)
                        {
                            entity.Context = this.responseInfo.Context;
                        }

                        this.current = this.materializer.CurrentValue;
                    }

                    this.moved = true;
                }
            }

            this.materializer.ApplyLogToContext();

            return result;
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotSupportedException">Always thrown</exception>
        void System.Collections.IEnumerator.Reset()
        {
            throw Error.NotSupported();
        }

        /// <summary>
        ///  Creates materializer for results
        /// </summary>
        /// <param name="context">Context of expression to analyze.</param>
        /// <param name="results">the results to wrap</param>
        /// <returns>a new materializer</returns>
        internal static MaterializeAtom CreateWrapper(DataServiceContext context, IEnumerable results)
        {
            return new ResultsWrapper(context, results, null);
        }

        /// <summary>Creates a materializer for partial result sets.</summary>
        /// <param name="context">Context of expression to analyze.</param>
        /// <param name="results">The current page of results</param>
        /// <param name="continuation">The continuation for the results.</param>
        /// <returns>A new materializer.</returns>
        internal static MaterializeAtom CreateWrapper(DataServiceContext context, IEnumerable results, DataServiceQueryContinuation continuation)
        {
            return new ResultsWrapper(context, results, continuation);
        }

        /// <summary>set the inserted object expected in the response</summary>
        /// <param name="addedObject">object being inserted that the response is looking for</param>
        internal void SetInsertingObject(object addedObject)
        {
            Debug.Assert(this.materializer is ODataEntityMaterializer, "this.materializer is ODataEntityMaterializer");
            ((ODataEntityMaterializer)this.materializer).TargetInstance = addedObject;
        }

        /// <summary>
        /// The count tag's value, if requested
        /// </summary>
        /// <returns>The count value returned from the server</returns>
        internal long CountValue()
        {
            return this.materializer.CountValue;
        }

        /// <summary>
        /// Returns the next link URI for the collection key
        /// </summary>
        /// <param name="key">The collection for which the Uri is returned, or null, if the top level link is to be returned</param>
        /// <returns>An Uri pointing to the next page for the collection</returns>
        internal virtual DataServiceQueryContinuation GetContinuation(IEnumerable key)
        {
            Debug.Assert(this.materializer != null, "Materializer is null!");

            DataServiceQueryContinuation result;
            if (key == null)
            {
                if ((this.expectingPrimitiveValue && !this.moved) || (!this.expectingPrimitiveValue && !this.materializer.IsEndOfStream))
                {
                    // expectingSingleValue && !moved : haven't started parsing single value (single value should not have next link anyway)
                    // !expectingSingleValue && !IsEndOfStream : collection type feed did not finish parsing yet
                    throw new InvalidOperationException(Strings.MaterializeFromAtom_TopLevelLinkNotAvailable);
                }

                // we have already moved to the end of stream
                // are we singleton or just an entry?
                if (this.expectingPrimitiveValue || this.materializer.CurrentFeed == null)
                {
                    result = null;
                }
                else
                {
                    // DEVNOTE(pqian): The next link uri should never be edited by the client, and therefore it must be absolute
                    result = DataServiceQueryContinuation.Create(
                        this.materializer.CurrentFeed.NextPageLink,
                        this.materializer.MaterializeEntryPlan);
                }
            }
            else
            {
                if (!this.materializer.NextLinkTable.TryGetValue(key, out result))
                {
                    // someone has asked for a collection that's "out of scope" or doesn't exist
                    throw new ArgumentException(Strings.MaterializeFromAtom_CollectionKeyNotPresentInLinkTable);
                }
            }

            return result;
        }

        /// <summary>verify the GetEnumerator can only be called once</summary>
        private void CheckGetEnumerator()
        {
            if (this.calledGetEnumerator)
            {
                throw Error.NotSupported(Strings.Deserialize_GetEnumerator);
            }

            this.calledGetEnumerator = true;
        }

        internal static string ReadElementString(XmlReader reader, bool checkNullAttribute)
        {
            Debug.Assert(reader != null, "reader != null");
            Debug.Assert(
                reader.NodeType == XmlNodeType.Element,
                "reader.NodeType == XmlNodeType.Element -- otherwise caller is confused as to where the reader is");

            string result = null;
            bool empty = checkNullAttribute && !Util.DoesNullAttributeSayTrue(reader);

            if (reader.IsEmptyElement)
            {
                return (empty ? String.Empty : null);
            }

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        return result ?? (empty ? String.Empty : null);
                    case XmlNodeType.CDATA:
                    case XmlNodeType.Text:
                    case XmlNodeType.SignificantWhitespace:
                        if (result != null)
                        {
                            throw Error.InvalidOperation(Strings.Deserialize_MixedTextWithComment);
                        }

                        result = reader.Value;
                        break;
                    case XmlNodeType.Comment:
                    case XmlNodeType.Whitespace:
                        break;

                    #region XmlNodeType error
                    case XmlNodeType.Element:
                        goto default;

                    default:
                        throw Error.InvalidOperation(Strings.Deserialize_ExpectingSimpleValue);
                    #endregion
                }
            }

            // xml ended before EndElement?
            throw Error.InvalidOperation(Strings.Deserialize_ExpectingSimpleValue);
        }

        /// <summary>
        /// Private type to wrap partial (paged) results and make it look like standard materialized results.
        /// </summary>
        private class ResultsWrapper : MaterializeAtom
        {
            #region Private fields

            /// <summary> The results to wrap </summary>
            private readonly IEnumerable results;

            /// <summary>A continuation to the next page of results.</summary>
            private readonly DataServiceQueryContinuation continuation;

            /// <summary>The data service context this result belongs to</summary>
            private readonly DataServiceContext context;

            #endregion Private fields

            /// <summary>
            /// Creates a wrapper for raw results
            /// </summary>
            /// <param name="context">Context of expression to analyze.</param>
            /// <param name="results">the results to wrap</param>
            /// <param name="continuation">The continuation for this query.</param>
            internal ResultsWrapper(DataServiceContext context, IEnumerable results, DataServiceQueryContinuation continuation)
            {
                this.context = context;
                this.results = results ?? Enumerable.Empty<object>();
                this.continuation = continuation;
            }

            /// <summary>
            /// The data service context this result belongs to
            /// </summary>
            internal override DataServiceContext Context
            {
                get
                {
                    return this.context;
                }
            }

            /// <summary>
            /// Get the next link to the result set
            /// </summary>
            /// <param name="key">When equals to null, returns the next link associated with this collection. Otherwise throws InvalidOperationException.</param>
            /// <returns>The continuation for this query.</returns>
            internal override DataServiceQueryContinuation GetContinuation(IEnumerable key)
            {
                if (key == null)
                {
                    return this.continuation;
                }
                else
                {
                    throw new InvalidOperationException(Strings.MaterializeFromAtom_GetNestLinkForFlatCollection);
                }
            }

            /// <summary>
            /// If the ResultWrapper is empty. When response code is No-Content, it is empty.
            /// </summary>
            /// <returns>Return true if it is empty result.</returns>
            internal bool IsEmptyResult()
            {
                return this.context == null && this.continuation == null;
            }

            /// <summary>
            /// Gets Enumerator for wrapped results.
            /// </summary>
            /// <returns>IEnumerator for results</returns>
            public override IEnumerator GetEnumerator()
            {
                return this.results.GetEnumerator();
            }
        }
    }
}

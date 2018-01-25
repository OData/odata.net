//---------------------------------------------------------------------
// <copyright file="DataServiceCollectionOfT.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using Microsoft.OData.Client.Materialization;

    #endregion Namespaces

    /// <summary>Determines whether changes that are made to a <see cref="T:Microsoft.OData.Client.DataServiceCollection`1" /> are tracked.</summary>
    public enum TrackingMode
    {
        /// <summary>The collection should not track changes.</summary>
        None = 0,

        /// <summary>The collection should automatically track changes to the entities
        /// in the collection.</summary>
        AutoChangeTracking
    }

    /// <summary>Represents a dynamic entity collection that provides notifications when items get added, removed, or when the list is refreshed.</summary>
        /// <typeparam name="T">An entity type.</typeparam>
    public class DataServiceCollection<T> : ObservableCollection<T>
    {
        #region Private fields
        /// <summary>The BindingObserver associated with the DataServiceCollection</summary>
        private BindingObserver observer;

        /// <summary>Is this a root collection</summary>
        private bool rootCollection;

        /// <summary>The continuation for partial collections.</summary>
        private DataServiceQueryContinuation<T> continuation;

        /// <summary>True if tracking setup was deferred to first Load() call.</summary>
        private bool trackingOnLoad;

        /// <summary>Callback tracked until tracking is enabled.</summary>
        private Func<EntityChangedParams, bool> entityChangedCallback;

        /// <summary>Callback tracked until tracking is enabled.</summary>
        private Func<EntityCollectionChangedParams, bool> collectionChangedCallback;

        /// <summary>Entity set name tracked until tracking is enabled.</summary>
        private string entitySetName;

        /// <summary>
        /// The async handle for the current LoadAsync Operation
        /// </summary>
        private IAsyncResult ongoingAsyncOperation;

        #endregion Private fields

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceCollection`1" /> class.</summary>
        /// <remarks>Creates a default data service collection, with auto-change tracking enabled as soon as data is loaded into it.</remarks>
        public DataServiceCollection()
            : this(null, null, TrackingMode.AutoChangeTracking, null, null, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceCollection`1" /> class based on query execution.</summary>
        /// <param name="item">A <see cref="T:Microsoft.OData.Client.DataServiceQuerySingle`1" /> or LINQ query that returns an object that are used to initialize the collection.</param>
        public DataServiceCollection(DataServiceQuerySingle<T> item)
            : this(null, item.Query, TrackingMode.AutoChangeTracking, null, null, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceCollection`1" /> class based on query execution.</summary>
        /// <param name="items">A <see cref="T:Microsoft.OData.Client.DataServiceQuery`1" /> or LINQ query that  returns an <see cref="T:System.Collections.Generic.IEnumerable`1" /> collection of objects that are used to initialize the collection.</param>
        public DataServiceCollection(IEnumerable<T> items)
            : this(null, items, TrackingMode.AutoChangeTracking, null, null, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceCollection`1" /> class based on query execution and with the specified tracking mode.</summary>
        /// <param name="trackingMode">A <see cref="T:Microsoft.OData.Client.TrackingMode" /> value that indicated whether or not changes made to items in the collection are automatically tracked.</param>
        /// <param name="item">A <see cref="T:Microsoft.OData.Client.DataServiceQuerySingle`1" /> or LINQ query that returns an object that are used to initialize the collection.</param>
        public DataServiceCollection(TrackingMode trackingMode, DataServiceQuerySingle<T> item)
            : this(null, item.Query, trackingMode, null, null, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceCollection`1" /> class based on query execution and with the specified tracking mode.</summary>
        /// <param name="items">A <see cref="T:Microsoft.OData.Client.DataServiceQuery`1" /> or LINQ query that returns an <see cref="T:System.Collections.Generic.IEnumerable`1" /> collection of objects that are used to initialize the collection.</param>
        /// <param name="trackingMode">A <see cref="T:Microsoft.OData.Client.TrackingMode" /> value that indicated whether or not changes made to items in the collection are automatically tracked.</param>
        public DataServiceCollection(IEnumerable<T> items, TrackingMode trackingMode)
            : this(null, items, trackingMode, null, null, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceCollection`1" /> class that uses the specified <see cref="T:Microsoft.OData.Client.DataServiceContext" />.</summary>
        /// <param name="context">The <see cref="T:Microsoft.OData.Client.DataServiceContext" /> used to track changes to objects in the collection.</param>
        public DataServiceCollection(DataServiceContext context)
            : this(context, null, TrackingMode.AutoChangeTracking, null, null, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceCollection`1" /> class with the supplied change method delegates and that uses the specified <see cref="T:Microsoft.OData.Client.DataServiceContext" />.</summary>
        /// <param name="context">The <see cref="T:Microsoft.OData.Client.DataServiceContext" /> used to track items in the collection.</param>
        /// <param name="entitySetName">The entity set of the objects in the collection.</param>
        /// <param name="entityChangedCallback">A delegate that encapsulates a method that is called when an entity changes.</param>
        /// <param name="collectionChangedCallback">A delegate that encapsulates a method that is called when the collection of entities changes.</param>
        public DataServiceCollection(
            DataServiceContext context,
            string entitySetName,
            Func<EntityChangedParams, bool> entityChangedCallback,
            Func<EntityCollectionChangedParams, bool> collectionChangedCallback)
            : this(context, null, TrackingMode.AutoChangeTracking, entitySetName, entityChangedCallback, collectionChangedCallback)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceCollection`1" /> class based on query execution and with the supplied change method delegates.</summary>
        /// <param name="items">A <see cref="T:Microsoft.OData.Client.DataServiceQuery`1" /> or LINQ query that returns an <see cref="T:System.Collections.Generic.IEnumerable`1" /> collection of objects that are used to initialize the collection.</param>
        /// <param name="trackingMode">A <see cref="T:Microsoft.OData.Client.TrackingMode" /> value that indicated whether or not changes made to items in the collection are automatically tracked.</param>
        /// <param name="entitySetName">The entity set of the objects in the collection.</param>
        /// <param name="entityChangedCallback">A delegate that encapsulates a method that is called when an entity changes.</param>
        /// <param name="collectionChangedCallback">A delegate that encapsulates a method that is called when the collection of entities changes.</param>
        public DataServiceCollection(
            IEnumerable<T> items,
            TrackingMode trackingMode,
            string entitySetName,
            Func<EntityChangedParams, bool> entityChangedCallback,
            Func<EntityCollectionChangedParams, bool> collectionChangedCallback)
            : this(null, items, trackingMode, entitySetName, entityChangedCallback, collectionChangedCallback)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceCollection`1" /> class based on query execution, with the supplied change method delegates, and that uses the supplied <see cref="T:Microsoft.OData.Client.DataServiceContext" />.</summary>
        /// <param name="context">The <see cref="T:Microsoft.OData.Client.DataServiceContext" /> used to track items in the collection.</param>
        /// <param name="items">A <see cref="T:Microsoft.OData.Client.DataServiceQuery`1" /> or LINQ query that returns an <see cref="T:System.Collections.Generic.IEnumerable`1" /> collection of objects that are used to initialize the collection.</param>
        /// <param name="trackingMode">A <see cref="T:Microsoft.OData.Client.TrackingMode" /> value that indicated whether or not changes made to items in the collection are automatically tracked.</param>
        /// <param name="entitySetName">The entity set of the objects in the collection.</param>
        /// <param name="entityChangedCallback">A delegate that encapsulates a method that is called when an entity changes.</param>
        /// <param name="collectionChangedCallback">A delegate that encapsulates a method that is called when the collection of entities changes.</param>
        public DataServiceCollection(
            DataServiceContext context,
            IEnumerable<T> items,
            TrackingMode trackingMode,
            string entitySetName,
            Func<EntityChangedParams, bool> entityChangedCallback,
            Func<EntityCollectionChangedParams, bool> collectionChangedCallback)
        {
            if (trackingMode == TrackingMode.AutoChangeTracking)
            {
                if (context == null)
                {
                    if (items == null)
                    {
                        // Enable tracking on first Load/LoadAsync call, when we can obtain a context
                        this.trackingOnLoad = true;

                        // Save off these for when we enable tracking later
                        this.entitySetName = entitySetName;
                        this.entityChangedCallback = entityChangedCallback;
                        this.collectionChangedCallback = collectionChangedCallback;
                    }
                    else
                    {
                        // This throws if no context can be obtained, no need to check here
                        context = DataServiceCollection<T>.GetContextFromItems(items);
                    }
                }

                if (!this.trackingOnLoad)
                {
                    if (items != null)
                    {
                        DataServiceCollection<T>.ValidateIteratorParameter(items);
                    }

                    this.StartTracking(context, items, entitySetName, entityChangedCallback, collectionChangedCallback);
                }
            }
            else if (items != null)
            {
                this.Load(items);
            }
        }

        /// <summary>Creates new DataServiceCollection.</summary>
        /// <param name="entityMaterializer">The materializer</param>
        /// <param name="context"><see cref="DataServiceContext"/> associated with the new collection.</param>
        /// <param name="items">Enumeration of items to initialize the new DataServiceCollection with.</param>
        /// <param name="trackingMode">The tracking mode for the new collection.</param>
        /// <param name="entitySetName">The name of the entity set the elements in the collection belong to.</param>
        /// <param name="entityChangedCallback">Delegate that gets called when an entity changes.</param>
        /// <param name="collectionChangedCallback">Delegate that gets called when an entity collection changes.</param>
        /// <remarks>This is the internal constructor called from materializer and used inside our projection queries.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800", Justification = "Constructor and debug-only code can't reuse cast.")]
        internal DataServiceCollection(
            object entityMaterializer,
            DataServiceContext context,
            IEnumerable<T> items,
            TrackingMode trackingMode,
            string entitySetName,
            Func<EntityChangedParams, bool> entityChangedCallback,
            Func<EntityCollectionChangedParams, bool> collectionChangedCallback)
            : this(
                context != null ? context : ((ODataEntityMaterializer)entityMaterializer).EntityTrackingAdapter.Context,
                items,
                trackingMode,
                entitySetName,
                entityChangedCallback,
                collectionChangedCallback)
        {
            Debug.Assert(entityMaterializer != null, "entityMaterializer != null");
            Debug.Assert(((ODataEntityMaterializer)entityMaterializer).EntityTrackingAdapter != null, "EntityTrackingAdapter != null");

            if (items != null)
            {
                ((ODataEntityMaterializer)entityMaterializer).PropagateContinuation(items, this);
            }
        }

        #region Properties
        /// <summary>A completion event for the <see cref="LoadAsync(System.Linq.IQueryable&lt;T&gt;)"/>, <see cref="LoadAsync()"/>
        /// and <see cref="LoadNextPartialSetAsync"/> method.</summary>
        /// <remarks>This event is raised exactly once for each call to the <see cref="LoadAsync(System.Linq.IQueryable&lt;T&gt;)"/>,
        /// <see cref="LoadAsync()"/> or <see cref="LoadNextPartialSetAsync"/> method. It is called both when the operation
        /// succeeded and/or when it failed.</remarks>
        public event EventHandler<LoadCompletedEventArgs> LoadCompleted;

        /// <summary>Gets a continuation object that is used to return the next set of paged results.</summary>
        /// <returns>A <see cref="T:Microsoft.OData.Client.DataServiceQueryContinuation`1" /> object that contains the URI to return the next set of paged results.</returns>
        public DataServiceQueryContinuation<T> Continuation
        {
            get { return this.continuation; }
            set { this.continuation = value; }
        }

        /// <summary>Observer for the collection.</summary>
        /// <remarks>The setter would get called only for child collections in the graph.</remarks>
        internal BindingObserver Observer
        {
            get
            {
                return this.observer;
            }

            set
            {
                Debug.Assert(typeof(System.ComponentModel.INotifyPropertyChanged).IsAssignableFrom(typeof(T)), "The entity type must be trackable (by implementing INotifyPropertyChanged interface)");
                this.observer = value;
            }
        }

        /// <summary>
        /// Whether this collection is actively tracking
        /// </summary>
        internal bool IsTracking
        {
            get { return this.observer != null; }
        }
        #endregion Properties

        /// <summary>Loads a collection of entity objects into the collection.</summary>
        /// <param name="items">Collection of entity objects to be added to the <see cref="T:Microsoft.OData.Client.DataServiceCollection`1" />.</param>
        /// <remarks>
        /// When tracking is enabled, the behavior of Load would be to attach all those entities that are not already tracked by the context
        /// associated with the collection. The operation will go deep into the input entities so that all related
        /// entities are attached to the context if not already present. All entities in <paramref name="items"/>
        /// will be tracked after Load is done.
        /// Load method checks for duplication. The collection will ignore any duplicated items been loaded.
        /// For large amount of items, consider DataServiceContext.LoadProperty instead.
        /// </remarks>
        public void Load(IEnumerable<T> items)
        {
            DataServiceCollection<T>.ValidateIteratorParameter(items);

            if (this.trackingOnLoad)
            {
                // This throws if no context can be obtained, no need to check here
                DataServiceContext context = DataServiceCollection<T>.GetContextFromItems(items);

                this.trackingOnLoad = false;

                this.StartTracking(context, items, this.entitySetName, this.entityChangedCallback, this.collectionChangedCallback);
            }
            else
            {
                this.StartLoading();
                try
                {
                    this.InternalLoadCollection(items);
                }
                finally
                {
                    this.FinishLoading();
                }
            }
        }

        /// <summary>Asynchronously loads the collection by executing a <see cref="T:Microsoft.OData.Client.DataServiceQuery`1" />.Supported only by the WCF Data Services 5.0 client for Silverlight.</summary>
        /// <param name="query">The <see cref="T:Microsoft.OData.Client.DataServiceQuery`1" /> that, when executed, returns the entities to load into the collection.</param>
        /// <exception cref="T:System.ArgumentException">When query is null or not a <see cref="T:Microsoft.OData.Client.DataServiceQuery`1" />.</exception>
        /// <exception cref="T:System.InvalidOperationException">When a previous call to <see cref="M:Microsoft.OData.Client.DataServiceCollection`1.LoadAsync" /> is not yet complete.</exception>
        /// <remarks>This method uses the event-based async pattern.
        /// The method returns immediately without waiting for the query to complete. Then it calls the handler of the
        /// <see cref="LoadCompleted"/> event exactly once on the UI thread. The event will be raised regardless
        /// if the query succeeded or not.
        /// This class only support one asynchronous operation in flight.</remarks>
        public void LoadAsync(System.Linq.IQueryable<T> query)
        {
            Util.CheckArgumentNull(query, "query");
            DataServiceQuery<T> dsq = query as DataServiceQuery<T>;
            if (dsq == null)
            {
                throw new ArgumentException(Strings.DataServiceCollection_LoadAsyncRequiresDataServiceQuery, "query");
            }

            if (this.ongoingAsyncOperation != null)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_MultipleLoadAsyncOperationsAtTheSameTime);
            }

            if (this.trackingOnLoad)
            {
                this.StartTracking(
                    ((DataServiceQueryProvider)dsq.Provider).Context,
                    null,
                    this.entitySetName,
                    this.entityChangedCallback,
                    this.collectionChangedCallback);
                this.trackingOnLoad = false;
            }

            this.BeginLoadAsyncOperation(
                asyncCallback => dsq.BeginExecute(asyncCallback, null),
                asyncResult =>
                {
                    QueryOperationResponse<T> response = (QueryOperationResponse<T>)dsq.EndExecute(asyncResult);
                    this.Load(response);
                    return response;
                });
        }

        /// <summary>Loads the collection asynchronously by loading the results from the request Uri.</summary>
        /// <param name="requestUri">The request uri to download results from.</param>
        /// <remarks>This method uses the event-based async pattern.
        /// The method returns immediately without waiting for the query to complete. Then it calls the handler of the
        /// <see cref="LoadCompleted"/> event exactly once on the UI thread. The event will be raised regradless
        /// if the query succeeded or not.
        /// This class only support one asynchronous operation in flight.</remarks>
        public void LoadAsync(Uri requestUri)
        {
            Util.CheckArgumentNull(requestUri, "requestUri");

            if (!this.IsTracking)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_OperationForTrackedOnly);
            }

            if (this.ongoingAsyncOperation != null)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_MultipleLoadAsyncOperationsAtTheSameTime);
            }

            DataServiceContext context = this.observer.Context;
            requestUri = UriUtil.CreateUri(context.BaseUri, requestUri);

            this.BeginLoadAsyncOperation(
                asyncCallback => context.BeginExecute<T>(requestUri, asyncCallback, null),
                asyncResult =>
                {
                    QueryOperationResponse<T> response = (QueryOperationResponse<T>)context.EndExecute<T>(asyncResult);
                    this.Load(response);
                    return response;
                });
        }

        /// <summary>Asynchronously loads items into the collection, when it represents the navigation property of an entity.Supported only by the WCF Data Services 5.0 client for Silverlight.</summary>
        /// <exception cref="T:System.InvalidOperationException">When the collection does not belong to a parent entity.-or-When the parent entity is not tracked by the <see cref="T:Microsoft.OData.Client.DataServiceContext" />.-or-When a previous call to <see cref="M:Microsoft.OData.Client.DataServiceCollection`1.LoadAsync" /> is not yet complete.</exception>
        /// <remarks>This method loads the content of a property represented by this DataServiceCollection.
        /// If this instance is not associated with any property and entity the method will fail.
        /// This method uses the event-based async pattern.
        /// The method returns immediately without waiting for the query to complete. Then it calls the handler of the
        /// <see cref="LoadCompleted"/> event exactly once on the UI thread. The event will be raised regradless
        /// if the query succeeded or not.
        /// This class only support one asynchronous operation in flight.</remarks>
        public void LoadAsync()
        {
            if (!this.IsTracking)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_OperationForTrackedOnly);
            }

            object parent;
            string property;
            if (!this.observer.LookupParent(this, out parent, out property))
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_LoadAsyncNoParamsWithoutParentEntity);
            }

            if (this.ongoingAsyncOperation != null)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_MultipleLoadAsyncOperationsAtTheSameTime);
            }

            this.BeginLoadAsyncOperation(
                asyncCallback => this.observer.Context.BeginLoadProperty(parent, property, asyncCallback, null),
                asyncResult => (QueryOperationResponse)this.observer.Context.EndLoadProperty(asyncResult));
        }

        /// <summary>Loads the next page of data into the collection.Supported only by the WCF Data Services 5.0 client for Silverlight.</summary>
        /// <returns>A <see cref="T:System.Boolean" /> value that is true when the Microsoft.OData.Client.DataServiceCollection has a continuation token; otherwise false.</returns>
        /// <remarks>This method is the same as <see cref="LoadAsync(System.Linq.IQueryable&lt;T&gt;)"/> except that it runs the query as defined
        /// by the continuation token of this collection.
        /// The method returns immediately without waiting for the query to complete. Then it calls the handler of the
        /// <see cref="LoadCompleted"/> event exactly once on the UI thread. The event will be raised regradless
        /// if the query succeeded or not. Even if the method returns false, the event will be raised (immeditaly)
        /// This class only support one asynchronous operation in flight.
        /// If this collection doesn't have a continuation token (this.Continuation == null) then this method
        /// returns false and does not issue any request.
        /// If there is a continuation token the method will return true and will start a request to load
        /// the next partial set based on that continuation token.</remarks>
        public bool LoadNextPartialSetAsync()
        {
            if (!this.IsTracking)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_OperationForTrackedOnly);
            }

            if (this.ongoingAsyncOperation != null)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_MultipleLoadAsyncOperationsAtTheSameTime);
            }

            if (this.Continuation == null)
            {
                if (this.LoadCompleted != null)
                {
                    this.LoadCompleted(this, new LoadCompletedEventArgs(null, null));
                }

                return false;
            }

            this.BeginLoadAsyncOperation(
                asyncCallback => this.observer.Context.BeginExecute(this.Continuation, asyncCallback, null),
                asyncResult =>
                {
                    QueryOperationResponse<T> response = (QueryOperationResponse<T>)this.observer.Context.EndExecute<T>(asyncResult);
                    this.Load(response);
                    return response;
                });

            return true;
        }

        /// <summary>Cancels any running LoadAsync operations and calls the LoadCompleted event handler after cancellation.</summary>
        public void CancelAsyncLoad()
        {
            if (this.ongoingAsyncOperation != null)
            {
                this.observer.Context.CancelRequest(this.ongoingAsyncOperation);
            }
        }

        /// <summary>Loads a single entity object into the collection.</summary>
        /// <param name="item">Entity object to be added.</param>
        /// <remarks>
        /// When tracking is enabled, the behavior of Load would be to attach the entity if it is not already tracked by the context
        /// associated with the collection. The operation will go deep into the input entity so that all related
        /// entities are attached to the context if not already present. The <paramref name="item"/> will be
        /// tracked after Load is done.
        /// Load method checks for duplication. The collection will ignore any duplicated items been loaded.
        /// </remarks>
        public void Load(T item)
        {
            // When loading a single item,
            if (item == null)
            {
                throw Error.ArgumentNull("item");
            }

            this.StartLoading();
            try
            {
                if (!this.Contains(item))
                {
                    this.Add(item);
                }
            }
            finally
            {
                this.FinishLoading();
            }
        }

        /// <summary>Indicates whether all the items from the collection are removed.</summary>
        /// <param name="stopTracking">true if all the items from the collection are removed; otherwise, false.</param>
        public void Clear(bool stopTracking)
        {
            if (!this.IsTracking)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_OperationForTrackedOnly);
            }

            if (!stopTracking)
            {
                // non-binding or just clear
                this.Clear();
            }
            else
            {
                Debug.Assert(this.observer.Context != null, "Must have valid context when the collection is being observed.");
                try
                {
                    this.observer.DetachBehavior = true;
                    this.Clear();
                }
                finally
                {
                    this.observer.DetachBehavior = false;
                }
            }
        }

        /// <summary>Disables the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> tracking of all items in the collection.</summary>
        /// <remarks>
        /// All the entitities in the root collection and all it's related objects will be untracked at the
        /// end of this operation.
        /// </remarks>
        public void Detach()
        {
            if (!this.IsTracking)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_OperationForTrackedOnly);
            }

            // Operation only allowed on root collections.
            if (!this.rootCollection)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_CannotStopTrackingChildCollection);
            }

            this.observer.StopTracking();
            this.observer = null;

            this.rootCollection = false;
        }

        /// <summary>Adds a specified item to the collection at the specified index.</summary>
        /// <param name="index">Index at which to add the item.</param>
        /// <param name="item">The item to add.</param>
        /// <remarks>
        /// Override to prevent additions to the collection in "deferred tracking" mode, and to verify that the item implements INotifyPropertyChanged.
        /// Overridding this method will cover items that are added to the collection via Add and Insert.
        /// </remarks>
        protected override void InsertItem(int index, T item)
        {
            if (this.trackingOnLoad)
            {
                throw new InvalidOperationException(Strings.DataServiceCollection_InsertIntoTrackedButNotLoadedCollection);
            }

            if (this.IsTracking && item != null)
            {
                INotifyPropertyChanged notify = item as INotifyPropertyChanged;
                if (notify == null)
                {
                    throw new InvalidOperationException(Strings.DataBinding_NotifyPropertyChangedNotImpl(item.GetType()));
                }
            }

            base.InsertItem(index, item);
        }

        /// <summary>
        /// Verifies that input iterator parameter is not null and in case
        /// of Silverlight, it is not of DataServiceQuery type.
        /// </summary>
        /// <param name="items">Input iterator parameter.</param>
        private static void ValidateIteratorParameter(IEnumerable<T> items)
        {
            Util.CheckArgumentNull(items, "items");
#if PORTABLELIB
            DataServiceQuery<T> dsq = items as DataServiceQuery<T>;
            if (dsq != null)
            {
                throw new ArgumentException(Strings.DataServiceCollection_DataServiceQueryCanNotBeEnumerated);
            }
#endif
        }

        /// <summary>
        /// Obtain the DataServiceContext from the incoming enumerable
        /// </summary>
        /// <param name="items">An IEnumerable that may be a DataServiceQuery or QueryOperationResponse object</param>
        /// <returns>DataServiceContext instance associated with the input</returns>
        private static DataServiceContext GetContextFromItems(IEnumerable<T> items)
        {
            Debug.Assert(items != null, "items != null");

            DataServiceQuery<T> dataServiceQuery = items as DataServiceQuery<T>;
            if (dataServiceQuery != null)
            {
                DataServiceQueryProvider queryProvider = dataServiceQuery.Provider as DataServiceQueryProvider;
                Debug.Assert(queryProvider != null, "Got DataServiceQuery with unknown query provider.");
                DataServiceContext context = queryProvider.Context;
                Debug.Assert(context != null, "Query provider must always have valid context.");
                return context;
            }

            QueryOperationResponse queryOperationResponse = items as QueryOperationResponse;
            if (queryOperationResponse != null)
            {
                Debug.Assert(queryOperationResponse.Results != null, "Got QueryOperationResponse without valid results.");
                DataServiceContext context = queryOperationResponse.Results.Context;
                Debug.Assert(context != null, "Materializer must always have valid context.");
                return context;
            }

            throw new ArgumentException(Strings.DataServiceCollection_CannotDetermineContextFromItems);
        }

        /// <summary>
        /// Populate this collection with another collection of items
        /// </summary>
        /// <param name="items">The items to populate this collection with</param>
        private void InternalLoadCollection(IEnumerable<T> items)
        {
            Debug.Assert(items != null, "items != null");
#if !PORTABLELIB
            // For SDP, we must execute the Query implicitly
            DataServiceQuery<T> query = items as DataServiceQuery<T>;
            if (query != null)
            {
                items = query.Execute() as QueryOperationResponse<T>;
            }
#else
            Debug.Assert(!(items is DataServiceQuery), "SL Client using DSQ as items...should have been caught by ValidateIteratorParameter.");
#endif

            foreach (T item in items)
            {
                // if this is too slow, consider hashing the set
                // or just use LoadProperties
                if (!this.Contains(item))
                {
                    this.Add(item);
                }
            }

            QueryOperationResponse<T> response = items as QueryOperationResponse<T>;
            if (response != null)
            {
                // this should never be throwing (since we've enumerated already)!
                // Note: Inner collection's nextPartLinkUri is set by the materializer
                this.continuation = response.GetContinuation();
            }
            else
            {
                this.continuation = null;
            }
        }

        /// <summary>
        /// Prepare the collection for loading. For tracked collections, we enter the attaching state
        /// </summary>
        private void StartLoading()
        {
            if (this.IsTracking)
            {
                // Observer must be present on the target collection which implies that the operation would fail on default constructed objects.
                if (this.observer.Context == null)
                {
                    throw new InvalidOperationException(Strings.DataServiceCollection_LoadRequiresTargetCollectionObserved);
                }

                this.observer.AttachBehavior = true;
            }
        }

        /// <summary>
        /// Reset the collection after loading. For tracked collections, we exit the attaching state.
        /// </summary>
        private void FinishLoading()
        {
            if (this.IsTracking)
            {
                this.observer.AttachBehavior = false;
            }
        }

        /// <summary>Initialize and start tracking an DataServiceCollection</summary>
        /// <param name="context">The context</param>
        /// <param name="items">Collection to initialize with</param>
        /// <param name="entitySet">The entity set of the elements in the collection.</param>
        /// <param name="entityChanged">delegate that needs to be called when an entity changes.</param>
        /// <param name="collectionChanged">delegate that needs to be called when an entity collection is changed.</param>
        private void StartTracking(
            DataServiceContext context,
            IEnumerable<T> items,
            String entitySet,
            Func<EntityChangedParams, bool> entityChanged,
            Func<EntityCollectionChangedParams, bool> collectionChanged)
        {
            Debug.Assert(context != null, "Must have a valid context to initialize.");
            Debug.Assert(this.observer == null, "Must have no observer which implies Initialize should only be called once.");

            context.UsingDataServiceCollection = true;

            // Verify that T corresponds to an entity type.
            // Validate here before any items are added to the collection because if this fails we want to prevent the collection from being populated.
            if (!BindingEntityInfo.IsEntityType(typeof(T), context.Model))
            {
                throw new ArgumentException(Strings.DataBinding_DataServiceCollectionArgumentMustHaveEntityType(typeof(T)));
            }

            // Set up the observer first because we want the collection to know it's supposed to be tracked while the items are being loaded.
            // Items being added to the collection are not added to the binding graph until later when StartTracking is called on the observer.
            this.observer = new BindingObserver(context, entityChanged, collectionChanged);

            // Add everything from the input collection.
            if (items != null)
            {
                try
                {
                    this.InternalLoadCollection(items);
                }
                catch
                {
                    // If any failures occur, reset the observer since we couldn't successfully start tracking
                    this.observer = null;
                    throw;
                }
            }

            this.observer.StartTracking(this, entitySet);

            this.rootCollection = true;
        }

        /// <summary>Helper method to start a LoadAsync operation.</summary>
        /// <param name="beginCall">Function which calls the Begin method for the load. It should take <see cref="AsyncCallback"/>
        /// parameter which should be used as the callback for the Begin call. It should return <see cref="IAsyncResult"/>
        /// of the started asynchronous operation (or throw).</param>
        /// <param name="endCall">Function which calls the End method for the load. It should take <see cref="IAsyncResult"/>
        /// which represents the asynchronous operation in flight. It should return <see cref="QueryOperationResponse"/>
        /// with the result of the operation (or throw).</param>
        /// <remarks>The method takes care of error handling as well as maintaining the <see cref="ongoingAsyncOperation"/>.
        /// Note that it does not check the <see cref="ongoingAsyncOperation"/> to disallow multiple operations in flight.
        /// The method makes sure that the <paramref name="endCall"/> will be called from the UI thread. It makes no assumptions
        /// about the calling thread of this method.
        /// The method does not process the results of the <paramref name="endCall"/>, it just raises the <see cref="LoadCompleted"/>
        /// event as appropriate. If there's some processing to be done for the results it should all be done by the
        /// <paramref name="endCall"/> method before it returns.</remarks>
        private void BeginLoadAsyncOperation(
            Func<AsyncCallback, IAsyncResult> beginCall,
            Func<IAsyncResult, QueryOperationResponse> endCall)
        {
            Debug.Assert(this.ongoingAsyncOperation == null, "Trying to start a new LoadAsync while another is still in progress. We should have thrown.");

            // NOTE: this is Silverlight-only, use BackgroundWorker instead of Deployment.Current.Dispatcher
            // to do this in WinForms/WCF once we decide to add it there as well.
            // Note that we must mark the operation as in progress before we actually call Begin
            //   as the async operation might end immediately inside the Begin call and we have no control
            //   over the ordering between the End callback thread, the thread Begin is called from
            //   and the UI thread on which we process the end event.
            this.ongoingAsyncOperation = null;
            try
            {
                AsyncCallback endLoadAsyncOperation;

                // If SynchronizationContext.Current is not null, use that to invoke the end call and event firing, otherwise
                // just invoke those operations on the callback thread. If we can't automatically get the SynchronizationContext.Current,
                // the user can still call SynchronizationContext.SetSynchronizationContext and set a specific context to use,
                // or can invoke operations themselves in a specific context in the event handler.
                // This is not the common scenario so we are not providing a way to directly set this in our API.
                SynchronizationContext syncContext = SynchronizationContext.Current;
                if (syncContext == null)
                {
                    endLoadAsyncOperation = (ar) => this.EndLoadAsyncOperation(endCall, ar);
                }
                else
                {
                    endLoadAsyncOperation = (ar) => syncContext.Post((unused) => this.EndLoadAsyncOperation(endCall, ar), null);
                }

                this.ongoingAsyncOperation = beginCall(endLoadAsyncOperation);
            }
            catch (Exception)
            {
                this.ongoingAsyncOperation = null;
                throw;
            }
        }

        /// <summary>
        /// Calls the end method for the LoadAsync operation and fires the LoadCompleted event.
        /// </summary>
        /// <param name="endCall">End method to complete the asynchronous query execution.</param>
        /// <param name="asyncResult">IAsyncResult to be passed to <paramref name="endCall"/>.</param>
        private void EndLoadAsyncOperation(Func<IAsyncResult, QueryOperationResponse> endCall, IAsyncResult asyncResult)
        {
            try
            {
                QueryOperationResponse result = endCall(asyncResult);
                this.ongoingAsyncOperation = null;
                if (this.LoadCompleted != null)
                {
                    this.LoadCompleted(this, new LoadCompletedEventArgs(result, null));
                }
            }
            catch (Exception ex)
            {
                if (!CommonUtil.IsCatchableExceptionType(ex))
                {
                    throw;
                }

                this.ongoingAsyncOperation = null;
                if (this.LoadCompleted != null)
                {
                    this.LoadCompleted(this, new LoadCompletedEventArgs(null, ex));
                }
            }
        }
    }
}

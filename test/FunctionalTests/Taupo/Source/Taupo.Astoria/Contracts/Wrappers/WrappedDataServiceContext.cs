//---------------------------------------------------------------------
// <copyright file="WrappedDataServiceContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

// this code has been generated using WrapperGenerator
// Do not modify the file, but instead modify and re-run the generator.

// disable warnings about 'new' keyword being required/not needed
#pragma warning disable 108, 109

namespace Microsoft.Test.Taupo.Astoria.Contracts.Wrappers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.Wrappers;
    using Microsoft.OData.Client;
    /// <summary>
    /// Wraps the 'Microsoft.OData.Client.DataServiceContext' type.
    /// </summary>
    public partial class WrappedDataServiceContext : WrappedObject
    {
        private static readonly Type WrappedObjectType;
        private static readonly IDictionary<string, MethodInfo> MethodInfoCache = new Dictionary<string, MethodInfo>();
        private static readonly IDictionary<string, ConstructorInfo> BeforeEventArgs = new Dictionary<string, ConstructorInfo>();
        private static readonly IDictionary<string, ConstructorInfo> AfterEventArgs = new Dictionary<string, ConstructorInfo>();
        private static readonly IDictionary<string, MethodInfo> BeforeEvents = new Dictionary<string, MethodInfo>();
        private static readonly IDictionary<string, MethodInfo> AfterEvents = new Dictionary<string, MethodInfo>();

        /// <summary>
        /// Initializes static members of the WrappedDataServiceContext class.
        /// </summary>
        static WrappedDataServiceContext()
        {
            WrappedObjectType = AstoriaWrapperUtilities.GetTypeFromAssembly("Microsoft.OData.Client.DataServiceContext", "Microsoft.OData.Client");
            BeforeEventArgs["Void AddObject(System.String, System.Object)"] = typeof(EventsTracker.AddObjectEventArgs).GetInstanceConstructors(true).First();
            AfterEventArgs["Void AddObject(System.String, System.Object)"] = typeof(EventsTracker.AddObjectEventArgs).GetInstanceConstructors(true).First();
            BeforeEvents["Void AddObject(System.String, System.Object)"] = typeof(EventsTracker).GetMethod("RaiseBeforeAddObject", null, false);
            AfterEvents["Void AddObject(System.String, System.Object)"] = typeof(EventsTracker).GetMethod("RaiseAfterAddObject", null, false);
            BeforeEventArgs["Boolean DetachLink(System.Object, System.String, System.Object)"] = typeof(EventsTracker.BeforeDetachLinkEventArgs).GetInstanceConstructors(true).First();
            AfterEventArgs["Boolean DetachLink(System.Object, System.String, System.Object)"] = typeof(EventsTracker.AfterDetachLinkEventArgs).GetInstanceConstructors(true).First();
            BeforeEvents["Boolean DetachLink(System.Object, System.String, System.Object)"] = typeof(EventsTracker).GetMethod("RaiseBeforeDetachLink", null, false);
            AfterEvents["Boolean DetachLink(System.Object, System.String, System.Object)"] = typeof(EventsTracker).GetMethod("RaiseAfterDetachLink", null, false);
        }

        /// <summary>
        /// Initializes a new instance of the WrappedDataServiceContext class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        public WrappedDataServiceContext(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
            this.TrackEvents = new EventsTracker(this);
        }

        /// <summary>
        /// Gets or sets a value of the 'AddAndUpdateResponsePreference' property on 'Microsoft.OData.Client.DataServiceContext'
        /// </summary>
        public virtual WrappedEnum AddAndUpdateResponsePreference
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedEnum>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceResponsePreference get_AddAndUpdateResponsePreference()"), new object[] { });
            }

            set
            {
                WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void set_AddAndUpdateResponsePreference(Microsoft.OData.Client.DataServiceResponsePreference)"), new object[] { value });
            }
        }

        /// <summary>
        /// Gets a value indicating whether ApplyingChanges on 'Microsoft.OData.Client.DataServiceContext' is set to true
        /// </summary>
        public virtual bool ApplyingChanges
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<bool>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Boolean get_ApplyingChanges()"), new object[] { });
            }
        }

        /// <summary>
        /// Gets or sets a value of the 'BaseUri' property on 'Microsoft.OData.Client.DataServiceContext'
        /// </summary>
        public virtual System.Uri BaseUri
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<System.Uri>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Uri get_BaseUri()"), new object[] { });
            }

            set
            {
                WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void set_BaseUri(System.Uri)"), new object[] { value });
            }
        }

        /// <summary>
        /// Gets or sets a value of the 'Credentials' property on 'Microsoft.OData.Client.DataServiceContext'
        /// </summary>
        public virtual WrappedObject Credentials
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedObject>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Net.ICredentials get_Credentials()"), new object[] { });
            }

            set
            {
                WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void set_Credentials(System.Net.ICredentials)"), new object[] { value });
            }
        }

        /// <summary>
        /// Gets or sets a value of the 'DataNamespace' property on 'Microsoft.OData.Client.DataServiceContext'
        /// </summary>
        public virtual string DataNamespace
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<string>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.String get_DataNamespace()"), new object[] { });
            }

            set
            {
                WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void set_DataNamespace(System.String)"), new object[] { value });
            }
        }

        /// <summary>
        /// Gets a value of the 'Entities' property on 'Microsoft.OData.Client.DataServiceContext'
        /// </summary>
        public virtual WrappedReadOnlyCollection<WrappedEntityDescriptor> Entities
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedReadOnlyCollection<WrappedEntityDescriptor>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Collections.ObjectModel.ReadOnlyCollection`1[Microsoft.OData.Client.EntityDescriptor] get_Entities()"), new object[] { });
            }
        }

        ///// <summary>
        ///// Gets or sets a value indicating whether IgnoreMissingProperties on 'Microsoft.OData.Client.DataServiceContext' is set to true
        ///// </summary>
        //public virtual UndeclaredPropertyBehavior UndeclaredPropertyBehavior
        //{
        //    get
        //    {
        //        return WrapperUtilities.InvokeMethodAndCast<UndeclaredPropertyBehavior>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "UndeclaredPropertyBehavior get_UndeclaredPropertyBehavior()"), new object[] { });
        //    }

        //    set
        //    {
        //        WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void set_UndeclaredPropertyBehavior(UndeclaredPropertyBehavior)"), new object[] { value });
        //    }
        //}

        /// <summary>
        /// Gets or sets a value indicating whether IgnoreResourceNotFoundException on 'Microsoft.OData.Client.DataServiceContext' is set to true
        /// </summary>
        public virtual bool IgnoreResourceNotFoundException
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<bool>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Boolean get_IgnoreResourceNotFoundException()"), new object[] { });
            }

            set
            {
                WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void set_IgnoreResourceNotFoundException(Boolean)"), new object[] { value });
            }
        }

        /// <summary>
        /// Gets a value of the 'Links' property on 'Microsoft.OData.Client.DataServiceContext'
        /// </summary>
        public virtual WrappedReadOnlyCollection<WrappedLinkDescriptor> Links
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedReadOnlyCollection<WrappedLinkDescriptor>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Collections.ObjectModel.ReadOnlyCollection`1[Microsoft.OData.Client.LinkDescriptor] get_Links()"), new object[] { });
            }
        }

        /// <summary>
        /// Gets a value of the 'MaxProtocolVersion' property on 'Microsoft.OData.Client.DataServiceContext'
        /// </summary>
        public virtual WrappedEnum MaxProtocolVersion
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedEnum>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.ODataProtocolVersion get_MaxProtocolVersion()"), new object[] { });
            }
        }

        /// <summary>
        /// Gets or sets a value of the 'MergeOption' property on 'Microsoft.OData.Client.DataServiceContext'
        /// </summary>
        public virtual WrappedEnum MergeOption
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedEnum>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.MergeOption get_MergeOption()"), new object[] { });
            }

            set
            {
                WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void set_MergeOption(Microsoft.OData.Client.MergeOption)"), new object[] { value });
            }
        }

        /// <summary>
        /// Gets or sets a value of the 'ResolveEntitySet' property on 'Microsoft.OData.Client.DataServiceContext'
        /// </summary>
        public virtual System.Func<string, System.Uri> ResolveEntitySet
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<System.Func<string, System.Uri>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Func`2[System.String,System.Uri] get_ResolveEntitySet()"), new object[] { });
            }

            set
            {
                WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void set_ResolveEntitySet(System.Func`2[System.String,System.Uri])"), new object[] { value });
            }
        }

        /// <summary>
        /// Gets or sets a value of the 'ResolveName' property on 'Microsoft.OData.Client.DataServiceContext'
        /// </summary>
        public virtual System.Func<Type, string> ResolveName
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<System.Func<Type, string>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Func`2[System.Type,System.String] get_ResolveName()"), new object[] { });
            }

            set
            {
                WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void set_ResolveName(System.Func`2[System.Type,System.String])"), new object[] { value });
            }
        }

        /// <summary>
        /// Gets or sets a value of the 'ResolveType' property on 'Microsoft.OData.Client.DataServiceContext'
        /// </summary>
        public virtual System.Func<string, Type> ResolveType
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<System.Func<string, Type>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Func`2[System.String,System.Type] get_ResolveType()"), new object[] { });
            }

            set
            {
                WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void set_ResolveType(System.Func`2[System.String,System.Type])"), new object[] { value });
            }
        }

        /// <summary>
        /// Gets or sets a value of the 'SaveChangesDefaultOptions' property on 'Microsoft.OData.Client.DataServiceContext'
        /// </summary>
        public virtual WrappedEnum SaveChangesDefaultOptions
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndWrap<WrappedEnum>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.SaveChangesOptions get_SaveChangesDefaultOptions()"), new object[] { });
            }

            set
            {
                WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void set_SaveChangesDefaultOptions(Microsoft.OData.Client.SaveChangesOptions)"), new object[] { value });
            }
        }

        /// <summary>
        /// Gets or sets a value of the 'Timeout' property on 'Microsoft.OData.Client.DataServiceContext'
        /// </summary>
        public virtual int Timeout
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<int>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Int32 get_Timeout()"), new object[] { });
            }

            set
            {
                WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void set_Timeout(Int32)"), new object[] { value });
            }
        }

        /// <summary>
        /// Gets or sets a value of the 'TypeScheme' property on 'Microsoft.OData.Client.DataServiceContext'
        /// </summary>
        public virtual System.Uri TypeScheme
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<System.Uri>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Uri get_TypeScheme()"), new object[] { });
            }

            set
            {
                WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void set_TypeScheme(System.Uri)"), new object[] { value });
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether UsePostTunneling on 'Microsoft.OData.Client.DataServiceContext' is set to true
        /// </summary>
        public virtual bool UsePostTunneling
        {
            get
            {
                return WrapperUtilities.InvokeMethodAndCast<bool>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Boolean get_UsePostTunneling()"), new object[] { });
            }

            set
            {
                WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void set_UsePostTunneling(Boolean)"), new object[] { value });
            }
        }

        /// <summary>
        /// Gets the value of the EventsTracker class
        /// </summary>
        public EventsTracker TrackEvents { get; private set; }

        /// <summary>
        /// Wraps the 'Void AddLink(System.Object, System.String, System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="source">The value of the 'source' parameter.</param>
        /// <param name="sourceProperty">The value of the 'sourceProperty' parameter.</param>
        /// <param name="target">The value of the 'target' parameter.</param>
        public virtual void AddLink(WrappedObject source, string sourceProperty, WrappedObject target)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void AddLink(System.Object, System.String, System.Object)"), new object[] { source, sourceProperty, target });
        }

        /// <summary>
        /// Wraps the 'Void AddObject(System.String, System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entitySetName">The value of the 'entitySetName' parameter.</param>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        public virtual void AddObject(string entitySetName, WrappedObject entity)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void AddObject(System.String, System.Object)"), new object[] { entitySetName, entity }, BeforeEvents["Void AddObject(System.String, System.Object)"], AfterEvents["Void AddObject(System.String, System.Object)"], BeforeEventArgs["Void AddObject(System.String, System.Object)"], AfterEventArgs["Void AddObject(System.String, System.Object)"]);
        }

        /// <summary>
        /// Wraps the 'Void AddRelatedObject(System.Object, System.String, System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="source">The value of the 'source' parameter.</param>
        /// <param name="sourceProperty">The value of the 'sourceProperty' parameter.</param>
        /// <param name="target">The value of the 'target' parameter.</param>
        public virtual void AddRelatedObject(WrappedObject source, string sourceProperty, WrappedObject target)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void AddRelatedObject(System.Object, System.String, System.Object)"), new object[] { source, sourceProperty, target });
        }

        /// <summary>
        /// Wraps the 'Void AttachLink(System.Object, System.String, System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="source">The value of the 'source' parameter.</param>
        /// <param name="sourceProperty">The value of the 'sourceProperty' parameter.</param>
        /// <param name="target">The value of the 'target' parameter.</param>
        public virtual void AttachLink(WrappedObject source, string sourceProperty, WrappedObject target)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void AttachLink(System.Object, System.String, System.Object)"), new object[] { source, sourceProperty, target });
        }

        /// <summary>
        /// Wraps the 'Void AttachTo(System.String, System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entitySetName">The value of the 'entitySetName' parameter.</param>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        public virtual void AttachTo(string entitySetName, WrappedObject entity)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void AttachTo(System.String, System.Object)"), new object[] { entitySetName, entity });
        }

        /// <summary>
        /// Wraps the 'Void AttachTo(System.String, System.Object, System.String)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entitySetName">The value of the 'entitySetName' parameter.</param>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <param name="etag">The value of the 'etag' parameter.</param>
        public virtual void AttachTo(string entitySetName, WrappedObject entity, string etag)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void AttachTo(System.String, System.Object, System.String)"), new object[] { entitySetName, entity, etag });
        }

        /// <summary>
        /// Wraps the 'System.IAsyncResult BeginExecute(System.Uri, System.AsyncCallback, System.Object, System.String, Microsoft.OData.Client.OperationParameter[])' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="requestUri">The value of the 'requestUri' parameter.</param>
        /// <param name="callback">The value of the 'callback' parameter.</param>
        /// <param name="state">The value of the 'state' parameter.</param>
        /// <param name="httpMethod">The value of the 'httpMethod' parameter.</param>
        /// <param name="operationParameters">The value of the 'operationParameters' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual System.IAsyncResult BeginExecute(System.Uri requestUri, System.AsyncCallback callback, WrappedObject state, string httpMethod, WrappedArray<WrappedOperationParameter> operationParameters)
        {
            return WrapperUtilities.InvokeMethodAndCast<System.IAsyncResult>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.IAsyncResult BeginExecute(System.Uri, System.AsyncCallback, System.Object, System.String, Microsoft.OData.Client.OperationParameter[])"), new object[] { requestUri, callback, state, httpMethod, operationParameters });
        }

        /// <summary>
        /// Wraps the 'System.IAsyncResult BeginExecute[T](Microsoft.OData.Client.DataServiceQueryContinuation`1[T], System.AsyncCallback, System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <typeparam name="T">The wrapper type for the 'T' generic parameter.</typeparam>
        /// <param name="typeT">The CLR generic type for the 'T' parameter.</param>
        /// <param name="continuation">The value of the 'continuation' parameter.</param>
        /// <param name="callback">The value of the 'callback' parameter.</param>
        /// <param name="state">The value of the 'state' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual System.IAsyncResult BeginExecute<T>(Type typeT, WrappedDataServiceQueryContinuation<T> continuation, System.AsyncCallback callback, WrappedObject state)
          where T : WrappedObject
        {
            return WrapperUtilities.InvokeMethodAndCast<System.IAsyncResult>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.IAsyncResult BeginExecute[T](Microsoft.OData.Client.DataServiceQueryContinuation`1[T], System.AsyncCallback, System.Object)"), new object[] { continuation, callback, state }, new Type[] { typeT });
        }

        /// <summary>
        /// Wraps the 'System.IAsyncResult BeginExecute[TElement](System.Uri, System.AsyncCallback, System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <typeparam name="TElement">The wrapper type for the 'TElement' generic parameter.</typeparam>
        /// <param name="typeTElement">The CLR generic type for the 'TElement' parameter.</param>
        /// <param name="requestUri">The value of the 'requestUri' parameter.</param>
        /// <param name="callback">The value of the 'callback' parameter.</param>
        /// <param name="state">The value of the 'state' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual System.IAsyncResult BeginExecute<TElement>(Type typeTElement, System.Uri requestUri, System.AsyncCallback callback, WrappedObject state)
          where TElement : WrappedObject
        {
            return WrapperUtilities.InvokeMethodAndCast<System.IAsyncResult>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.IAsyncResult BeginExecute[TElement](System.Uri, System.AsyncCallback, System.Object)"), new object[] { requestUri, callback, state }, new Type[] { typeTElement });
        }

        /// <summary>
        /// Wraps the 'System.IAsyncResult BeginExecute[TElement](System.Uri, System.AsyncCallback, System.Object, System.String, Boolean, Microsoft.OData.Client.OperationParameter[])' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <typeparam name="TElement">The wrapper type for the 'TElement' generic parameter.</typeparam>
        /// <param name="typeTElement">The CLR generic type for the 'TElement' parameter.</param>
        /// <param name="requestUri">The value of the 'requestUri' parameter.</param>
        /// <param name="callback">The value of the 'callback' parameter.</param>
        /// <param name="state">The value of the 'state' parameter.</param>
        /// <param name="httpMethod">The value of the 'httpMethod' parameter.</param>
        /// <param name="singleResult">The value of the 'singleResult' parameter.</param>
        /// <param name="operationParameters">The value of the 'operationParameters' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual System.IAsyncResult BeginExecute<TElement>(Type typeTElement, System.Uri requestUri, System.AsyncCallback callback, WrappedObject state, string httpMethod, bool singleResult, WrappedArray<WrappedOperationParameter> operationParameters)
          where TElement : WrappedObject
        {
            return WrapperUtilities.InvokeMethodAndCast<System.IAsyncResult>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.IAsyncResult BeginExecute[TElement](System.Uri, System.AsyncCallback, System.Object, System.String, Boolean, Microsoft.OData.Client.OperationParameter[])"), new object[] { requestUri, callback, state, httpMethod, singleResult, operationParameters }, new Type[] { typeTElement });
        }

        /// <summary>
        /// Wraps the 'System.IAsyncResult BeginExecuteBatch(System.AsyncCallback, System.Object, Microsoft.OData.Client.DataServiceRequest[])' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="callback">The value of the 'callback' parameter.</param>
        /// <param name="state">The value of the 'state' parameter.</param>
        /// <param name="queries">The value of the 'queries' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual System.IAsyncResult BeginExecuteBatch(System.AsyncCallback callback, WrappedObject state, WrappedArray<WrappedDataServiceRequest> queries)
        {
            return WrapperUtilities.InvokeMethodAndCast<System.IAsyncResult>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.IAsyncResult BeginExecuteBatch(System.AsyncCallback, System.Object, Microsoft.OData.Client.DataServiceRequest[])"), new object[] { callback, state, queries });
        }

        /// <summary>
        /// Wraps the 'System.IAsyncResult BeginGetReadStream(System.Object, Microsoft.OData.Client.DataServiceRequestArgs, System.AsyncCallback, System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <param name="args">The value of the 'args' parameter.</param>
        /// <param name="callback">The value of the 'callback' parameter.</param>
        /// <param name="state">The value of the 'state' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual System.IAsyncResult BeginGetReadStream(WrappedObject entity, WrappedObject args, System.AsyncCallback callback, WrappedObject state)
        {
            return WrapperUtilities.InvokeMethodAndCast<System.IAsyncResult>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.IAsyncResult BeginGetReadStream(System.Object, Microsoft.OData.Client.DataServiceRequestArgs, System.AsyncCallback, System.Object)"), new object[] { entity, args, callback, state });
        }

        /// <summary>
        /// Wraps the 'System.IAsyncResult BeginGetReadStream(System.Object, System.String, Microsoft.OData.Client.DataServiceRequestArgs, System.AsyncCallback, System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <param name="name">The value of the 'name' parameter.</param>
        /// <param name="args">The value of the 'args' parameter.</param>
        /// <param name="callback">The value of the 'callback' parameter.</param>
        /// <param name="state">The value of the 'state' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual System.IAsyncResult BeginGetReadStream(WrappedObject entity, string name, WrappedObject args, System.AsyncCallback callback, WrappedObject state)
        {
            return WrapperUtilities.InvokeMethodAndCast<System.IAsyncResult>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.IAsyncResult BeginGetReadStream(System.Object, System.String, Microsoft.OData.Client.DataServiceRequestArgs, System.AsyncCallback, System.Object)"), new object[] { entity, name, args, callback, state });
        }

        /// <summary>
        /// Wraps the 'System.IAsyncResult BeginLoadProperty(System.Object, System.String, System.AsyncCallback, System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <param name="propertyName">The value of the 'propertyName' parameter.</param>
        /// <param name="callback">The value of the 'callback' parameter.</param>
        /// <param name="state">The value of the 'state' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual System.IAsyncResult BeginLoadProperty(WrappedObject entity, string propertyName, System.AsyncCallback callback, WrappedObject state)
        {
            return WrapperUtilities.InvokeMethodAndCast<System.IAsyncResult>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.IAsyncResult BeginLoadProperty(System.Object, System.String, System.AsyncCallback, System.Object)"), new object[] { entity, propertyName, callback, state });
        }

        /// <summary>
        /// Wraps the 'System.IAsyncResult BeginLoadProperty(System.Object, System.String, Microsoft.OData.Client.DataServiceQueryContinuation, System.AsyncCallback, System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <param name="propertyName">The value of the 'propertyName' parameter.</param>
        /// <param name="continuation">The value of the 'continuation' parameter.</param>
        /// <param name="callback">The value of the 'callback' parameter.</param>
        /// <param name="state">The value of the 'state' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual System.IAsyncResult BeginLoadProperty(WrappedObject entity, string propertyName, WrappedDataServiceQueryContinuation continuation, System.AsyncCallback callback, WrappedObject state)
        {
            return WrapperUtilities.InvokeMethodAndCast<System.IAsyncResult>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.IAsyncResult BeginLoadProperty(System.Object, System.String, Microsoft.OData.Client.DataServiceQueryContinuation, System.AsyncCallback, System.Object)"), new object[] { entity, propertyName, continuation, callback, state });
        }

        /// <summary>
        /// Wraps the 'System.IAsyncResult BeginLoadProperty(System.Object, System.String, System.Uri, System.AsyncCallback, System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <param name="propertyName">The value of the 'propertyName' parameter.</param>
        /// <param name="nextLinkUri">The value of the 'nextLinkUri' parameter.</param>
        /// <param name="callback">The value of the 'callback' parameter.</param>
        /// <param name="state">The value of the 'state' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual System.IAsyncResult BeginLoadProperty(WrappedObject entity, string propertyName, System.Uri nextLinkUri, System.AsyncCallback callback, WrappedObject state)
        {
            return WrapperUtilities.InvokeMethodAndCast<System.IAsyncResult>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.IAsyncResult BeginLoadProperty(System.Object, System.String, System.Uri, System.AsyncCallback, System.Object)"), new object[] { entity, propertyName, nextLinkUri, callback, state });
        }

        /// <summary>
        /// Wraps the 'System.IAsyncResult BeginSaveChanges(System.AsyncCallback, System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="callback">The value of the 'callback' parameter.</param>
        /// <param name="state">The value of the 'state' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual System.IAsyncResult BeginSaveChanges(System.AsyncCallback callback, WrappedObject state)
        {
            return WrapperUtilities.InvokeMethodAndCast<System.IAsyncResult>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.IAsyncResult BeginSaveChanges(System.AsyncCallback, System.Object)"), new object[] { callback, state });
        }

        /// <summary>
        /// Wraps the 'System.IAsyncResult BeginSaveChanges(Microsoft.OData.Client.SaveChangesOptions, System.AsyncCallback, System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="options">The value of the 'options' parameter.</param>
        /// <param name="callback">The value of the 'callback' parameter.</param>
        /// <param name="state">The value of the 'state' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual System.IAsyncResult BeginSaveChanges(WrappedEnum options, System.AsyncCallback callback, WrappedObject state)
        {
            return WrapperUtilities.InvokeMethodAndCast<System.IAsyncResult>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.IAsyncResult BeginSaveChanges(Microsoft.OData.Client.SaveChangesOptions, System.AsyncCallback, System.Object)"), new object[] { options, callback, state });
        }

        /// <summary>
        /// Wraps the 'Void CancelRequest(System.IAsyncResult)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="asyncResult">The value of the 'asyncResult' parameter.</param>
        public virtual void CancelRequest(System.IAsyncResult asyncResult)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void CancelRequest(System.IAsyncResult)"), new object[] { asyncResult });
        }

        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.DataServiceQuery`1[T] CreateQuery[T](System.String)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <typeparam name="T">The wrapper type for the 'T' generic parameter.</typeparam>
        /// <param name="typeT">The CLR generic type for the 'T' parameter.</param>
        /// <param name="entitySetName">The value of the 'entitySetName' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedDataServiceQuery<T> CreateQuery<T>(Type typeT, string entitySetName)
          where T : WrappedObject
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceQuery<T>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceQuery`1[T] CreateQuery[T](System.String)"), new object[] { entitySetName }, new Type[] { typeT });
        }

        /// <summary>
        /// Wraps the 'Void DeleteLink(System.Object, System.String, System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="source">The value of the 'source' parameter.</param>
        /// <param name="sourceProperty">The value of the 'sourceProperty' parameter.</param>
        /// <param name="target">The value of the 'target' parameter.</param>
        public virtual void DeleteLink(WrappedObject source, string sourceProperty, WrappedObject target)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void DeleteLink(System.Object, System.String, System.Object)"), new object[] { source, sourceProperty, target });
        }

        /// <summary>
        /// Wraps the 'Void DeleteObject(System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        public virtual void DeleteObject(WrappedObject entity)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void DeleteObject(System.Object)"), new object[] { entity });
        }

        /// <summary>
        /// Wraps the 'Boolean Detach(System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual bool Detach(WrappedObject entity)
        {
            return WrapperUtilities.InvokeMethodAndCast<bool>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Boolean Detach(System.Object)"), new object[] { entity });
        }

        /// <summary>
        /// Wraps the 'Boolean DetachLink(System.Object, System.String, System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="source">The value of the 'source' parameter.</param>
        /// <param name="sourceProperty">The value of the 'sourceProperty' parameter.</param>
        /// <param name="target">The value of the 'target' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual bool DetachLink(WrappedObject source, string sourceProperty, WrappedObject target)
        {
            return WrapperUtilities.InvokeMethodAndCast<bool>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Boolean DetachLink(System.Object, System.String, System.Object)"), new object[] { source, sourceProperty, target }, BeforeEvents["Boolean DetachLink(System.Object, System.String, System.Object)"], AfterEvents["Boolean DetachLink(System.Object, System.String, System.Object)"], BeforeEventArgs["Boolean DetachLink(System.Object, System.String, System.Object)"], AfterEventArgs["Boolean DetachLink(System.Object, System.String, System.Object)"]);
        }

        /// <summary>
        /// Wraps the 'System.Collections.Generic.IEnumerable`1[TElement] EndExecute[TElement](System.IAsyncResult)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <typeparam name="TElement">The wrapper type for the 'TElement' generic parameter.</typeparam>
        /// <param name="typeTElement">The CLR generic type for the 'TElement' parameter.</param>
        /// <param name="asyncResult">The value of the 'asyncResult' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedIEnumerable<TElement> EndExecute<TElement>(Type typeTElement, System.IAsyncResult asyncResult)
          where TElement : WrappedObject
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedIEnumerable<TElement>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Collections.Generic.IEnumerable`1[TElement] EndExecute[TElement](System.IAsyncResult)"), new object[] { asyncResult }, new Type[] { typeTElement });
        }

        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.OperationResponse EndExecute(System.IAsyncResult)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="asyncResult">The value of the 'asyncResult' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedObject EndExecute(System.IAsyncResult asyncResult)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedObject>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.OperationResponse EndExecute(System.IAsyncResult)"), new object[] { asyncResult });
        }

        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.DataServiceResponse EndExecuteBatch(System.IAsyncResult)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="asyncResult">The value of the 'asyncResult' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedDataServiceResponse EndExecuteBatch(System.IAsyncResult asyncResult)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceResponse>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceResponse EndExecuteBatch(System.IAsyncResult)"), new object[] { asyncResult });
        }

        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.DataServiceStreamResponse EndGetReadStream(System.IAsyncResult)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="asyncResult">The value of the 'asyncResult' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedDataServiceStreamResponse EndGetReadStream(System.IAsyncResult asyncResult)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceStreamResponse>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceStreamResponse EndGetReadStream(System.IAsyncResult)"), new object[] { asyncResult });
        }

        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.QueryOperationResponse EndLoadProperty(System.IAsyncResult)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="asyncResult">The value of the 'asyncResult' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedQueryOperationResponse EndLoadProperty(System.IAsyncResult asyncResult)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedQueryOperationResponse>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.QueryOperationResponse EndLoadProperty(System.IAsyncResult)"), new object[] { asyncResult });
        }

        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.DataServiceResponse EndSaveChanges(System.IAsyncResult)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="asyncResult">The value of the 'asyncResult' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedDataServiceResponse EndSaveChanges(System.IAsyncResult asyncResult)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceResponse>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceResponse EndSaveChanges(System.IAsyncResult)"), new object[] { asyncResult });
        }

        /// <summary>
        /// Wraps the 'System.Collections.Generic.IEnumerable`1[TElement] Execute[TElement](System.Uri)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <typeparam name="TElement">The wrapper type for the 'TElement' generic parameter.</typeparam>
        /// <param name="typeTElement">The CLR generic type for the 'TElement' parameter.</param>
        /// <param name="requestUri">The value of the 'requestUri' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedIEnumerable<TElement> Execute<TElement>(Type typeTElement, System.Uri requestUri)
          where TElement : WrappedObject
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedIEnumerable<TElement>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Collections.Generic.IEnumerable`1[TElement] Execute[TElement](System.Uri)"), new object[] { requestUri }, new Type[] { typeTElement });
        }

        /// <summary>
        /// Wraps the 'System.Collections.Generic.IEnumerable`1[TElement] Execute[TElement](System.Uri, System.String, Boolean, Microsoft.OData.Client.OperationParameter[])' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <typeparam name="TElement">The wrapper type for the 'TElement' generic parameter.</typeparam>
        /// <param name="typeTElement">The CLR generic type for the 'TElement' parameter.</param>
        /// <param name="requestUri">The value of the 'requestUri' parameter.</param>
        /// <param name="httpMethod">The value of the 'httpMethod' parameter.</param>
        /// <param name="singleResult">The value of the 'singleResult' parameter.</param>
        /// <param name="operationParameters">The value of the 'operationParameters' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedIEnumerable<TElement> Execute<TElement>(Type typeTElement, System.Uri requestUri, string httpMethod, bool singleResult, WrappedArray<WrappedOperationParameter> operationParameters)
          where TElement : WrappedObject
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedIEnumerable<TElement>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Collections.Generic.IEnumerable`1[TElement] Execute[TElement](System.Uri, System.String, Boolean, Microsoft.OData.Client.OperationParameter[])"), new object[] { requestUri, httpMethod, singleResult, operationParameters }, new Type[] { typeTElement });
        }

        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.OperationResponse Execute(System.Uri, System.String, Microsoft.OData.Client.OperationParameter[])' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="requestUri">The value of the 'requestUri' parameter.</param>
        /// <param name="httpMethod">The value of the 'httpMethod' parameter.</param>
        /// <param name="operationParameters">The value of the 'operationParameters' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedObject Execute(System.Uri requestUri, string httpMethod, WrappedArray<WrappedOperationParameter> operationParameters)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedObject>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.OperationResponse Execute(System.Uri, System.String, Microsoft.OData.Client.OperationParameter[])"), new object[] { requestUri, httpMethod, operationParameters });
        }

        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.QueryOperationResponse`1[T] Execute[T](Microsoft.OData.Client.DataServiceQueryContinuation`1[T])' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <typeparam name="T">The wrapper type for the 'T' generic parameter.</typeparam>
        /// <param name="typeT">The CLR generic type for the 'T' parameter.</param>
        /// <param name="continuation">The value of the 'continuation' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedQueryOperationResponse<T> Execute<T>(Type typeT, WrappedDataServiceQueryContinuation<T> continuation)
          where T : WrappedObject
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedQueryOperationResponse<T>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.QueryOperationResponse`1[T] Execute[T](Microsoft.OData.Client.DataServiceQueryContinuation`1[T])"), new object[] { continuation }, new Type[] { typeT });
        }

        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.DataServiceResponse ExecuteBatch(Microsoft.OData.Client.DataServiceRequest[])' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="queries">The value of the 'queries' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedDataServiceResponse ExecuteBatch(WrappedArray<WrappedDataServiceRequest> queries)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceResponse>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceResponse ExecuteBatch(Microsoft.OData.Client.DataServiceRequest[])"), new object[] { queries });
        }

        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.EntityDescriptor GetEntityDescriptor(System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedEntityDescriptor GetEntityDescriptor(WrappedObject entity)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedEntityDescriptor>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.EntityDescriptor GetEntityDescriptor(System.Object)"), new object[] { entity });
        }

        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.LinkDescriptor GetLinkDescriptor(System.Object, System.String, System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="source">The value of the 'source' parameter.</param>
        /// <param name="sourceProperty">The value of the 'sourceProperty' parameter.</param>
        /// <param name="target">The value of the 'target' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedLinkDescriptor GetLinkDescriptor(WrappedObject source, string sourceProperty, WrappedObject target)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedLinkDescriptor>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.LinkDescriptor GetLinkDescriptor(System.Object, System.String, System.Object)"), new object[] { source, sourceProperty, target });
        }

        /// <summary>
        /// Wraps the 'System.Uri GetMetadataUri()' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual System.Uri GetMetadataUri()
        {
            return WrapperUtilities.InvokeMethodAndCast<System.Uri>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Uri GetMetadataUri()"), new object[] { });
        }

        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.DataServiceStreamResponse GetReadStream(System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedDataServiceStreamResponse GetReadStream(WrappedObject entity)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceStreamResponse>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceStreamResponse GetReadStream(System.Object)"), new object[] { entity });
        }

        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.DataServiceStreamResponse GetReadStream(System.Object, Microsoft.OData.Client.DataServiceRequestArgs)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <param name="args">The value of the 'args' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedDataServiceStreamResponse GetReadStream(WrappedObject entity, WrappedObject args)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceStreamResponse>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceStreamResponse GetReadStream(System.Object, Microsoft.OData.Client.DataServiceRequestArgs)"), new object[] { entity, args });
        }

        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.DataServiceStreamResponse GetReadStream(System.Object, System.String)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <param name="acceptContentType">The value of the 'acceptContentType' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedDataServiceStreamResponse GetReadStream(WrappedObject entity, string acceptContentType)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceStreamResponse>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceStreamResponse GetReadStream(System.Object, System.String)"), new object[] { entity, acceptContentType });
        }

        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.DataServiceStreamResponse GetReadStream(System.Object, System.String, Microsoft.OData.Client.DataServiceRequestArgs)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <param name="name">The value of the 'name' parameter.</param>
        /// <param name="args">The value of the 'args' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedDataServiceStreamResponse GetReadStream(WrappedObject entity, string name, WrappedObject args)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceStreamResponse>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceStreamResponse GetReadStream(System.Object, System.String, Microsoft.OData.Client.DataServiceRequestArgs)"), new object[] { entity, name, args });
        }

        /// <summary>
        /// Wraps the 'System.Uri GetReadStreamUri(System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual System.Uri GetReadStreamUri(WrappedObject entity)
        {
            return WrapperUtilities.InvokeMethodAndCast<System.Uri>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Uri GetReadStreamUri(System.Object)"), new object[] { entity });
        }

        /// <summary>
        /// Wraps the 'System.Uri GetReadStreamUri(System.Object, System.String)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <param name="name">The value of the 'name' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual System.Uri GetReadStreamUri(WrappedObject entity, string name)
        {
            return WrapperUtilities.InvokeMethodAndCast<System.Uri>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "System.Uri GetReadStreamUri(System.Object, System.String)"), new object[] { entity, name });
        }

        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.QueryOperationResponse LoadProperty(System.Object, System.String)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <param name="propertyName">The value of the 'propertyName' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedQueryOperationResponse LoadProperty(WrappedObject entity, string propertyName)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedQueryOperationResponse>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.QueryOperationResponse LoadProperty(System.Object, System.String)"), new object[] { entity, propertyName });
        }

        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.QueryOperationResponse LoadProperty(System.Object, System.String, Microsoft.OData.Client.DataServiceQueryContinuation)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <param name="propertyName">The value of the 'propertyName' parameter.</param>
        /// <param name="continuation">The value of the 'continuation' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedQueryOperationResponse LoadProperty(WrappedObject entity, string propertyName, WrappedDataServiceQueryContinuation continuation)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedQueryOperationResponse>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.QueryOperationResponse LoadProperty(System.Object, System.String, Microsoft.OData.Client.DataServiceQueryContinuation)"), new object[] { entity, propertyName, continuation });
        }

        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.QueryOperationResponse LoadProperty(System.Object, System.String, System.Uri)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <param name="propertyName">The value of the 'propertyName' parameter.</param>
        /// <param name="nextLinkUri">The value of the 'nextLinkUri' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedQueryOperationResponse LoadProperty(WrappedObject entity, string propertyName, System.Uri nextLinkUri)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedQueryOperationResponse>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.QueryOperationResponse LoadProperty(System.Object, System.String, System.Uri)"), new object[] { entity, propertyName, nextLinkUri });
        }

        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.QueryOperationResponse`1[T] LoadProperty[T](System.Object, System.String, Microsoft.OData.Client.DataServiceQueryContinuation`1[T])' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <typeparam name="T">The wrapper type for the 'T' generic parameter.</typeparam>
        /// <param name="typeT">The CLR generic type for the 'T' parameter.</param>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <param name="propertyName">The value of the 'propertyName' parameter.</param>
        /// <param name="continuation">The value of the 'continuation' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedQueryOperationResponse<T> LoadProperty<T>(Type typeT, WrappedObject entity, string propertyName, WrappedDataServiceQueryContinuation<T> continuation)
          where T : WrappedObject
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedQueryOperationResponse<T>>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.QueryOperationResponse`1[T] LoadProperty[T](System.Object, System.String, Microsoft.OData.Client.DataServiceQueryContinuation`1[T])"), new object[] { entity, propertyName, continuation }, new Type[] { typeT });
        }

        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.DataServiceResponse SaveChanges()' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedDataServiceResponse SaveChanges()
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceResponse>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceResponse SaveChanges()"), new object[] { });
        }

        /// <summary>
        /// Wraps the 'Microsoft.OData.Client.DataServiceResponse SaveChanges(Microsoft.OData.Client.SaveChangesOptions)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="options">The value of the 'options' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual WrappedDataServiceResponse SaveChanges(WrappedEnum options)
        {
            return WrapperUtilities.InvokeMethodAndWrap<WrappedDataServiceResponse>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Microsoft.OData.Client.DataServiceResponse SaveChanges(Microsoft.OData.Client.SaveChangesOptions)"), new object[] { options });
        }

        /// <summary>
        /// Wraps the 'Void SetLink(System.Object, System.String, System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="source">The value of the 'source' parameter.</param>
        /// <param name="sourceProperty">The value of the 'sourceProperty' parameter.</param>
        /// <param name="target">The value of the 'target' parameter.</param>
        public virtual void SetLink(WrappedObject source, string sourceProperty, WrappedObject target)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void SetLink(System.Object, System.String, System.Object)"), new object[] { source, sourceProperty, target });
        }

        /// <summary>
        /// Wraps the 'Void SetSaveStream(System.Object, System.IO.Stream, Boolean, Microsoft.OData.Client.DataServiceRequestArgs)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <param name="stream">The value of the 'stream' parameter.</param>
        /// <param name="closeStream">The value of the 'closeStream' parameter.</param>
        /// <param name="args">The value of the 'args' parameter.</param>
        public virtual void SetSaveStream(WrappedObject entity, Stream stream, bool closeStream, WrappedObject args)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void SetSaveStream(System.Object, System.IO.Stream, Boolean, Microsoft.OData.Client.DataServiceRequestArgs)"), new object[] { entity, stream, closeStream, args });
        }

        /// <summary>
        /// Wraps the 'Void SetSaveStream(System.Object, System.IO.Stream, Boolean, System.String, System.String)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <param name="stream">The value of the 'stream' parameter.</param>
        /// <param name="closeStream">The value of the 'closeStream' parameter.</param>
        /// <param name="contentType">The value of the 'contentType' parameter.</param>
        /// <param name="slug">The value of the 'slug' parameter.</param>
        public virtual void SetSaveStream(WrappedObject entity, Stream stream, bool closeStream, string contentType, string slug)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void SetSaveStream(System.Object, System.IO.Stream, Boolean, System.String, System.String)"), new object[] { entity, stream, closeStream, contentType, slug });
        }

        /// <summary>
        /// Wraps the 'Void SetSaveStream(System.Object, System.String, System.IO.Stream, Boolean, Microsoft.OData.Client.DataServiceRequestArgs)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <param name="name">The value of the 'name' parameter.</param>
        /// <param name="stream">The value of the 'stream' parameter.</param>
        /// <param name="closeStream">The value of the 'closeStream' parameter.</param>
        /// <param name="args">The value of the 'args' parameter.</param>
        public virtual void SetSaveStream(WrappedObject entity, string name, Stream stream, bool closeStream, WrappedObject args)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void SetSaveStream(System.Object, System.String, System.IO.Stream, Boolean, Microsoft.OData.Client.DataServiceRequestArgs)"), new object[] { entity, name, stream, closeStream, args });
        }

        /// <summary>
        /// Wraps the 'Void SetSaveStream(System.Object, System.String, System.IO.Stream, Boolean, System.String)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <param name="name">The value of the 'name' parameter.</param>
        /// <param name="stream">The value of the 'stream' parameter.</param>
        /// <param name="closeStream">The value of the 'closeStream' parameter.</param>
        /// <param name="contentType">The value of the 'contentType' parameter.</param>
        public virtual void SetSaveStream(WrappedObject entity, string name, Stream stream, bool closeStream, string contentType)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void SetSaveStream(System.Object, System.String, System.IO.Stream, Boolean, System.String)"), new object[] { entity, name, stream, closeStream, contentType });
        }

        /// <summary>
        /// Wraps the 'Boolean TryGetEntity[TEntity](System.Uri, TEntity ByRef)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <typeparam name="TEntity">The wrapper type for the 'TEntity' generic parameter.</typeparam>
        /// <param name="typeTEntity">The CLR generic type for the 'TEntity' parameter.</param>
        /// <param name="identity">The value of the 'identity' parameter.</param>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual bool TryGetEntity<TEntity>(Type typeTEntity, System.Uri identity, out TEntity entity)
          where TEntity : WrappedObject
        {
            object[] arguments = new object[] { identity, null };
            bool result = WrapperUtilities.InvokeMethodAndCast<bool>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Boolean TryGetEntity[TEntity](System.Uri, TEntity ByRef)"), arguments, new Type[] { typeTEntity });
            entity = this.Scope.Wrap<TEntity>(arguments[1]);

            return result;
        }

        /// <summary>
        /// Wraps the 'Boolean TryGetUri(System.Object, System.Uri ByRef)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        /// <param name="identity">The value of the 'identity' parameter.</param>
        /// <returns>The value returned by the underlying method.</returns>
        public virtual bool TryGetUri(WrappedObject entity, out System.Uri identity)
        {
            object[] arguments = new object[] { entity, null };
            bool result = WrapperUtilities.InvokeMethodAndCast<bool>(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Boolean TryGetUri(System.Object, System.Uri ByRef)"), arguments);
            identity = (System.Uri)arguments[1];

            return result;
        }

        /// <summary>
        /// Wraps the 'Void UpdateObject(System.Object)' on the 'Microsoft.OData.Client.DataServiceContext' type.
        /// </summary>
        /// <param name="entity">The value of the 'entity' parameter.</param>
        public virtual void UpdateObject(WrappedObject entity)
        {
            WrapperUtilities.InvokeMethodWithoutResult(this, AstoriaWrapperUtilities.GetMethodInfo(WrappedObjectType, MethodInfoCache, "Void UpdateObject(System.Object)"), new object[] { entity });
        }

        /// <summary>
        /// Initializes a new instance of the EventsTracker class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Type must be visible")]
        public class EventsTracker
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Code is generated")]
            private WrappedDataServiceContext wrapperType;

            /// <summary>
            /// Initializes a new instance of the EventsTracker class.
            /// </summary>
            /// <param name="wrapType">The wrapper type.</param>
            public EventsTracker(WrappedDataServiceContext wrapType)
            {
                this.wrapperType = wrapType;
            }

            /// <summary>
            /// Event for AddObjectBefore.
            /// </summary>
            public event EventHandler<AddObjectEventArgs> AddObjectBeforeEvent;

            /// <summary>
            /// Event for AddObjectAfter.
            /// </summary>
            public event EventHandler<AddObjectEventArgs> AddObjectAfterEvent;

            /// <summary>
            /// Event for DetachLinkBefore.
            /// </summary>
            public event EventHandler<BeforeDetachLinkEventArgs> DetachLinkBeforeEvent;

            /// <summary>
            /// Event for DetachLinkAfter.
            /// </summary>
            public event EventHandler<AfterDetachLinkEventArgs> DetachLinkAfterEvent;

            /// <summary>
            /// Raises the BeforeAddObject event.
            /// </summary>
            /// <param name="args">The arguments.</param>
            public void RaiseBeforeAddObject(AddObjectEventArgs args)
            {
                if (this.AddObjectBeforeEvent != null)
                {
                    this.AddObjectBeforeEvent(this, args);
                }
            }

            /// <summary>
            /// Raises the AfterAddObject event.
            /// </summary>
            /// <param name="args">The arguments.</param>
            public void RaiseAfterAddObject(AddObjectEventArgs args)
            {
                if (this.AddObjectAfterEvent != null)
                {
                    this.AddObjectAfterEvent(this, args);
                }
            }

            /// <summary>
            /// Raises the BeforeDetachLink event.
            /// </summary>
            /// <param name="args">The arguments.</param>
            public void RaiseBeforeDetachLink(BeforeDetachLinkEventArgs args)
            {
                if (this.DetachLinkBeforeEvent != null)
                {
                    this.DetachLinkBeforeEvent(this, args);
                }
            }

            /// <summary>
            /// Raises the AfterDetachLink event.
            /// </summary>
            /// <param name="args">The arguments.</param>
            public void RaiseAfterDetachLink(AfterDetachLinkEventArgs args)
            {
                if (this.DetachLinkAfterEvent != null)
                {
                    this.DetachLinkAfterEvent(this, args);
                }
            }

            /// <summary>
            /// Represents the event arguments.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Type must be visible")]
            public class AddObjectEventArgs : EventArgs
            {
                /// <summary>
                /// Initializes a new instance of the AddObjectEventArgs class.
                /// </summary>
                /// <param name="entitySetName">The value of the 'entitySetName' parameter.</param>
                /// <param name="entity">The value of the 'entity' parameter.</param>
                public AddObjectEventArgs(string entitySetName, WrappedObject entity)
                {
                    this.EntitySetName = entitySetName;
                    this.Entity = entity;
                }

                /// <summary>
                /// Gets the value of EntitySetName property.
                /// </summary>
                public string EntitySetName { get; private set; }

                /// <summary>
                /// Gets the value of Entity property.
                /// </summary>
                public WrappedObject Entity { get; private set; }
            }

            /// <summary>
            /// Represents the event arguments.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Type must be visible")]
            public class BeforeDetachLinkEventArgs : EventArgs
            {
                /// <summary>
                /// Initializes a new instance of the BeforeDetachLinkEventArgs class.
                /// </summary>
                /// <param name="source">The value of the 'source' parameter.</param>
                /// <param name="sourceProperty">The value of the 'sourceProperty' parameter.</param>
                /// <param name="target">The value of the 'target' parameter.</param>
                public BeforeDetachLinkEventArgs(WrappedObject source, string sourceProperty, WrappedObject target)
                {
                    this.Source = source;
                    this.SourceProperty = sourceProperty;
                    this.Target = target;
                }

                /// <summary>
                /// Gets the value of Source property.
                /// </summary>
                public WrappedObject Source { get; private set; }

                /// <summary>
                /// Gets the value of SourceProperty property.
                /// </summary>
                public string SourceProperty { get; private set; }

                /// <summary>
                /// Gets the value of Target property.
                /// </summary>
                public WrappedObject Target { get; private set; }
            }

            /// <summary>
            /// Represents the event arguments.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Type must be visible")]
            public class AfterDetachLinkEventArgs : EventArgs
            {
                /// <summary>
                /// Initializes a new instance of the AfterDetachLinkEventArgs class.
                /// </summary>
                /// <param name="source">The value of the 'source' parameter.</param>
                /// <param name="sourceProperty">The value of the 'sourceProperty' parameter.</param>
                /// <param name="target">The value of the 'target' parameter.</param>
                /// <param name="result">The value of the Result parameter.</param>
                public AfterDetachLinkEventArgs(WrappedObject source, string sourceProperty, WrappedObject target, bool result)
                {
                    this.Source = source;
                    this.SourceProperty = sourceProperty;
                    this.Target = target;
                    this.Result = result;
                }

                /// <summary>
                /// Gets the value of Source property.
                /// </summary>
                public WrappedObject Source { get; private set; }

                /// <summary>
                /// Gets the value of SourceProperty property.
                /// </summary>
                public string SourceProperty { get; private set; }

                /// <summary>
                /// Gets the value of Target property.
                /// </summary>
                public WrappedObject Target { get; private set; }

                /// <summary>
                /// Gets a value indicating whether the Result property is set to true.
                /// </summary>
                public bool Result { get; private set; }
            }
        }
    }
}

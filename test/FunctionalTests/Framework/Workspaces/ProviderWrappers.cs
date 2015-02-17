//---------------------------------------------------------------------
// <copyright file="ProviderWrappers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using Microsoft.OData.Service;
using System.Data.Test.Astoria.CallOrder;
using System.IO;
using System.Linq;
using DSP = Microsoft.OData.Service.Providers;
using System.Threading;

namespace System.Data.Test.Astoria.Providers
{
    public abstract class ProviderWrapper<T> : IDisposable where T : class
    {
        protected ProviderWrapper(T underlying)
        {
            UnderlyingProvider = underlying;
        }

        protected T UnderlyingProvider
        {
            get;
            private set;
        }

        #region IDisposable Members

        public void Dispose()
        {
            APICallLog.Current.Add(typeof(T).FullName + ".Dispose");
            try
            {
                IDisposable d = UnderlyingProvider as IDisposable;
                if (d != null)
                {
                    d.Dispose();
                    UnderlyingProvider = null;
                }
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        #endregion
    }

    #region IEnumerator
    public class EnumeratorWrapper : ProviderWrapper<System.Collections.IEnumerator>, System.Collections.IEnumerator
    {
        public EnumeratorWrapper(System.Collections.IEnumerator underlying)
            : base(underlying)
        { }

        public object Current 
        {
            get
            {
                return UnderlyingProvider.Current;
            }
        }

        public bool MoveNext()
        {
            return UnderlyingProvider.MoveNext();
        }

        public void Reset()
        {
            UnderlyingProvider.Reset();
        }
    }
    #endregion

    #region Stream
    public class StreamWrapper : Stream
    {
        private Stream underlyingStream;

        internal StreamWrapper(Stream underlyingStream)
        {
            this.underlyingStream = underlyingStream;
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead
        {
            get 
            {
                APICallLog.Current.Stream.CanRead();
                try
                {
                    return this.underlyingStream.CanRead;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public override bool CanSeek
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanTimeout
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanWrite
        {
            get 
            {
                APICallLog.Current.Stream.CanWrite();
                try
                {
                    return this.underlyingStream.CanWrite;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public override void Close()
        {
            APICallLog.Current.Stream.Close();
            try
            {
                this.underlyingStream.Close();
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            APICallLog.Current.Stream.Read(buffer, offset, count);
            try
            {
                return this.underlyingStream.Read(buffer, offset, count);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public override int ReadByte()
        {
            throw new NotImplementedException();
        }

        public override int ReadTimeout
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            APICallLog.Current.Stream.Write(buffer, offset, count);
            try
            {
                this.underlyingStream.Write(buffer, offset, count);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public override void WriteByte(byte value)
        {
            throw new NotImplementedException();
        }

        public override int WriteTimeout
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        protected override void Dispose(bool disposing)
        {
            APICallLog.Current.Stream.Dispose();
            try
            {
                this.underlyingStream.Dispose();
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }
    }
    #endregion

    #region IUpdatable
    public class UpdatableWrapper : ProviderWrapper<Microsoft.OData.Service.IUpdatable>, Microsoft.OData.Service.IUpdatable
    {
        public UpdatableWrapper(Microsoft.OData.Service.IUpdatable underlying)
            : base(underlying)
        { }

        #region IUpdatable Members

        public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            APICallLog.Current.Updatable.AddReferenceToCollection(targetResource, propertyName, resourceToBeAdded);
            try
            {
                UnderlyingProvider.AddReferenceToCollection(targetResource, propertyName, resourceToBeAdded);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public void ClearChanges()
        {
            APICallLog.Current.Updatable.ClearChanges();
            try
            {
                UnderlyingProvider.ClearChanges();
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public object CreateResource(string containerName, string fullTypeName)
        {
            APICallLog.Current.Updatable.CreateResource(containerName, fullTypeName);
            try
            {
                return UnderlyingProvider.CreateResource(containerName, fullTypeName);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public void DeleteResource(object targetResource)
        {
            APICallLog.Current.Updatable.DeleteResource(targetResource);
            try
            {
                UnderlyingProvider.DeleteResource(targetResource);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public object GetResource(IQueryable query, string fullTypeName)
        {
            APICallLog.Current.Updatable.GetResource(query, fullTypeName);
            try
            {
                return UnderlyingProvider.GetResource(query, fullTypeName);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public object GetValue(object targetResource, string propertyName)
        {
            APICallLog.Current.Updatable.GetValue(targetResource, propertyName);
            try
            {
                return UnderlyingProvider.GetValue(targetResource, propertyName);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            APICallLog.Current.Updatable.RemoveReferenceFromCollection(targetResource, propertyName, resourceToBeRemoved);
            try
            {
                UnderlyingProvider.RemoveReferenceFromCollection(targetResource, propertyName, resourceToBeRemoved);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public object ResetResource(object resource)
        {
            APICallLog.Current.Updatable.ResetResource(resource);
            try
            {
                return UnderlyingProvider.ResetResource(resource);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public object ResolveResource(object resource)
        {
            APICallLog.Current.Updatable.ResolveResource(resource);
            try
            {
                return UnderlyingProvider.ResolveResource(resource);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public void SaveChanges()
        {
            APICallLog.Current.Updatable.SaveChanges();
            try
            {
                UnderlyingProvider.SaveChanges();
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public void SetReference(object targetResource, string propertyName, object propertyValue)
        {
            APICallLog.Current.Updatable.SetReference(targetResource, propertyName, propertyValue);
            try
            {
                UnderlyingProvider.SetReference(targetResource, propertyName, propertyValue);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public void SetValue(object targetResource, string propertyName, object propertyValue)
        {
            APICallLog.Current.Updatable.SetValue(targetResource, propertyName, propertyValue);
            try
            {
                UnderlyingProvider.SetValue(targetResource, propertyName, propertyValue);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        #endregion
    }
    #endregion

    #region IExpandProvider
#pragma warning disable 618 // Disable "obsolete" warning for the IExpandProvider interface. Used for backwards compatibilty.
    public class ExpandProviderWrapper : ProviderWrapper<Microsoft.OData.Service.IExpandProvider>, Microsoft.OData.Service.IExpandProvider
    {
        public ExpandProviderWrapper(Microsoft.OData.Service.IExpandProvider underlying)
            : base(underlying)
        { }

        #region IExpandProvider Members
        public IEnumerable ApplyExpansions(IQueryable queryable, ICollection<ExpandSegmentCollection> expandPaths)
        {
            APICallLog.Current.ExpandProvider.ApplyExpansions(queryable, expandPaths);
            try
            {
                return UnderlyingProvider.ApplyExpansions(queryable, expandPaths);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }
        #endregion
    }
#pragma warning restore 618
    #endregion

    #region IDataServiceHost
    public class IDSHWrapper : ProviderWrapper<Microsoft.OData.Service.IDataServiceHost>, Microsoft.OData.Service.IDataServiceHost
    {
        public IDSHWrapper(Microsoft.OData.Service.IDataServiceHost underlying)
            : base(underlying)
        { }

        #region IDataServiceHost Members
        public Uri AbsoluteRequestUri
        {
            get
            {
                APICallLog.Current.IDSH.AbsoluteRequestUri();
                try
                {
                    return UnderlyingProvider.AbsoluteRequestUri;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public Uri AbsoluteServiceUri
        {
            get
            {
                APICallLog.Current.IDSH.AbsoluteServiceUri();
                try
                {
                    return UnderlyingProvider.AbsoluteServiceUri;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string GetQueryStringItem(string item)
        {
            APICallLog.Current.IDSH.GetQueryStringItem(item);
            try
            {
                return UnderlyingProvider.GetQueryStringItem(item);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public void ProcessException(HandleExceptionArgs args)
        {
            APICallLog.Current.IDSH.ProcessException(args);
            try
            {
                UnderlyingProvider.ProcessException(args);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public string RequestAccept
        {
            get
            {
                APICallLog.Current.IDSH.RequestAccept();
                try
                {
                    return UnderlyingProvider.RequestAccept;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string RequestAcceptCharSet
        {
            get
            {
                APICallLog.Current.IDSH.RequestAcceptCharSet();
                try
                {
                    return UnderlyingProvider.RequestAcceptCharSet;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string RequestContentType
        {
            get
            {
                APICallLog.Current.IDSH.RequestContentType();
                try
                {
                    return UnderlyingProvider.RequestContentType;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string RequestHttpMethod
        {
            get
            {
                APICallLog.Current.IDSH.RequestHttpMethod();
                try
                {
                    return UnderlyingProvider.RequestHttpMethod;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string RequestIfMatch
        {
            get
            {
                APICallLog.Current.IDSH.RequestIfMatch();
                try
                {
                    return UnderlyingProvider.RequestIfMatch;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string RequestIfNoneMatch
        {
            get
            {
                APICallLog.Current.IDSH.RequestIfNoneMatch();
                try
                {
                    return UnderlyingProvider.RequestIfNoneMatch;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string RequestMaxVersion
        {
            get
            {
                APICallLog.Current.IDSH.RequestMaxVersion();
                try
                {
                    return UnderlyingProvider.RequestMaxVersion;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public Stream RequestStream
        {
            get
            {
                APICallLog.Current.IDSH.RequestStream();
                try
                {
                    return UnderlyingProvider.RequestStream;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string RequestVersion
        {
            get
            {
                APICallLog.Current.IDSH.RequestVersion();
                try
                {
                    return UnderlyingProvider.RequestVersion;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string ResponseCacheControl
        {
            get
            {
                string x = APICallLog.Current.IDSH.ResponseCacheControl;
                try
                {
                    return UnderlyingProvider.ResponseCacheControl;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
            set
            {
                APICallLog.Current.IDSH.ResponseCacheControl = value;
                try
                {
                    UnderlyingProvider.ResponseCacheControl = value;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string ResponseContentType
        {
            get
            {
                string x = APICallLog.Current.IDSH.ResponseContentType;
                try
                {
                    return UnderlyingProvider.ResponseContentType;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
            set
            {
                APICallLog.Current.IDSH.ResponseContentType = value;
                try
                {
                    UnderlyingProvider.ResponseContentType = value;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string ResponseETag
        {
            get
            {
                string x = APICallLog.Current.IDSH.ResponseETag;
                try
                {
                    return UnderlyingProvider.ResponseETag;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
            set
            {
                APICallLog.Current.IDSH.ResponseETag = value;
                try
                {
                    UnderlyingProvider.ResponseETag = value;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string ResponseLocation
        {
            get
            {
                string x = APICallLog.Current.IDSH.ResponseLocation;
                try
                {
                    return UnderlyingProvider.ResponseLocation;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
            set
            {
                APICallLog.Current.IDSH.ResponseLocation = value;
                try
                {
                    UnderlyingProvider.ResponseLocation = value;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public int ResponseStatusCode
        {
            get
            {
                int x = APICallLog.Current.IDSH.ResponseStatusCode;
                try
                {
                    return UnderlyingProvider.ResponseStatusCode;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
            set
            {
                APICallLog.Current.IDSH.ResponseStatusCode = value;
                try
                {
                    UnderlyingProvider.ResponseStatusCode = value;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public Stream ResponseStream
        {
            get
            {
                APICallLog.Current.IDSH.ResponseStream();
                try
                {
                    return UnderlyingProvider.ResponseStream;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string ResponseVersion
        {
            get
            {
                string x = APICallLog.Current.IDSH.ResponseVersion;
                try
                {
                    return UnderlyingProvider.ResponseVersion;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
            set
            {
                APICallLog.Current.IDSH.ResponseVersion = value;
                try
                {
                    UnderlyingProvider.ResponseVersion = value;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        #endregion
    }
    #endregion

#if !ASTORIA_PRE_V2
    #region IDataServiceMetadataProvider
    public class MetadataProviderWrapper : ProviderWrapper<DSP.IDataServiceMetadataProvider>, DSP.IDataServiceMetadataProvider
    {
        private HashSet<string> materializedTypes = new HashSet<string>();

        public static bool ValidateTypeResolution = true;

        public MetadataProviderWrapper(DSP.IDataServiceMetadataProvider underlying)
            : base(underlying)
        { }

        #region IDataServiceMetadataProvider Members

        public string ContainerName
        {
            get
            {
                APICallLog.Current.MetadataProvider.ContainerName();
                try
                {
                    return UnderlyingProvider.ContainerName;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string ContainerNamespace
        {
            get
            {
                APICallLog.Current.MetadataProvider.ContainerNamespace();
                try
                {
                    return UnderlyingProvider.ContainerNamespace;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public IEnumerable<DSP.ResourceType> GetDerivedTypes(DSP.ResourceType resourceType)
        {
            APICallLog.Current.MetadataProvider.GetDerivedTypes(resourceType);
            IEnumerable<DSP.ResourceType> types;
            try
            {
                types = this.UnderlyingProvider.GetDerivedTypes(resourceType);
            }
            finally
            {
                APICallLog.Current.Pop();
            }

            foreach (var type in types)
            {
                this.materializedTypes.Add(type.FullName);
                yield return type;
            }
        }

        public DSP.ResourceAssociationSet GetResourceAssociationSet(DSP.ResourceSet resourceSet, DSP.ResourceType resourceType, DSP.ResourceProperty resourceProperty)
        {
            APICallLog.Current.MetadataProvider.GetResourceAssociationSet(resourceSet, resourceType, resourceProperty);
            try
            {
                return UnderlyingProvider.GetResourceAssociationSet(resourceSet, resourceType, resourceProperty);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public bool HasDerivedTypes(DSP.ResourceType resourceType)
        {
            APICallLog.Current.MetadataProvider.HasDerivedTypes(resourceType);
            try
            {
                return UnderlyingProvider.HasDerivedTypes(resourceType);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public IEnumerable<DSP.ResourceSet> ResourceSets
        {
            get
            {
                APICallLog.Current.MetadataProvider.ResourceSets();
                try
                {
                    return UnderlyingProvider.ResourceSets;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public IEnumerable<DSP.ServiceOperation> ServiceOperations
        {
            get
            {
                APICallLog.Current.MetadataProvider.ServiceOperations();
                try
                {
                    return UnderlyingProvider.ServiceOperations;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public bool TryResolveResourceSet(string name, out DSP.ResourceSet resourceSet)
        {
            APICallLog.Current.MetadataProvider.TryResolveResourceSet(name);
            try
            {
                return UnderlyingProvider.TryResolveResourceSet(name, out resourceSet);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public bool TryResolveResourceType(string name, out DSP.ResourceType resourceType)
        {
            APICallLog.Current.MetadataProvider.TryResolveResourceType(name);
            try
            {
                // Note: the DataServiceConfiguration.EnableTypeAccess method will blindly call TryResolveType, so we only
                // want to do this validation in some cases. This will be controlled via the static property 'ValidateTypeResolution'
                // which defaults to true, and will be explicitly turned off by the test components which call EnableTypeAccess
                if (this.materializedTypes.Contains(name) && ValidateTypeResolution)
                {
                    throw new DataServiceException(500, string.Format("Type '{0}' was already returned from the provider, so TryResolveResourceType should not have been called", name));
                }

                if(UnderlyingProvider.TryResolveResourceType(name, out resourceType))
                {
                    if (ValidateTypeResolution)
                    {
                        this.materializedTypes.Add(name);
                    }

                    return true;
                }

                return false;
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public bool TryResolveServiceOperation(string name, out DSP.ServiceOperation serviceOperation)
        {
            APICallLog.Current.MetadataProvider.TryResolveServiceOperation(name);
            try
            {
                return UnderlyingProvider.TryResolveServiceOperation(name, out serviceOperation);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public IEnumerable<DSP.ResourceType> Types
        {
            get
            {
                APICallLog.Current.MetadataProvider.Types();
                try
                {
                    // some test cases cause the underlying provider to return null intentionally
                    var types = this.UnderlyingProvider.Types;
                    if (types != null)
                    {
                        foreach (var type in types)
                        {
                            this.materializedTypes.Add(type.FullName);
                            yield return type;
                        }
                    }
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        #endregion
    }
    #endregion

    #region IDataServiceQueryProvider
    public class QueryProviderWrapper : ProviderWrapper<DSP.IDataServiceQueryProvider>, DSP.IDataServiceQueryProvider
    {
        public QueryProviderWrapper(DSP.IDataServiceQueryProvider underlying)
            : base(underlying)
        { }

        #region IDataServiceQueryProvider Members

        public object CurrentDataSource
        {
            get
            {
                object x = APICallLog.Current.QueryProvider.CurrentDataSource;
                try
                {
                    return UnderlyingProvider.CurrentDataSource;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
            set
            {
                APICallLog.Current.QueryProvider.CurrentDataSource = value;
                try
                {
                    UnderlyingProvider.CurrentDataSource = value;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public object GetOpenPropertyValue(object target, string propertyName)
        {
            APICallLog.Current.QueryProvider.GetOpenPropertyValue(target, propertyName);
            try
            {
                return UnderlyingProvider.GetOpenPropertyValue(target, propertyName);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(object target)
        {
            APICallLog.Current.QueryProvider.GetOpenPropertyValues(target);
            try
            {
                return UnderlyingProvider.GetOpenPropertyValues(target);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public object GetPropertyValue(object target, DSP.ResourceProperty resourceProperty)
        {
            APICallLog.Current.QueryProvider.GetPropertyValue(target, resourceProperty);
            try
            {
                return UnderlyingProvider.GetPropertyValue(target, resourceProperty);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public IQueryable GetQueryRootForResourceSet(DSP.ResourceSet resourceSet)
        {
            APICallLog.Current.QueryProvider.GetQueryRootForResourceSet(resourceSet);
            try
            {
                return UnderlyingProvider.GetQueryRootForResourceSet(resourceSet);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public DSP.ResourceType GetResourceType(object target)
        {
            APICallLog.Current.QueryProvider.GetResourceType(target);
            try
            {
                return UnderlyingProvider.GetResourceType(target);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public object InvokeServiceOperation(DSP.ServiceOperation serviceOperation, object[] parameters)
        {
            APICallLog.Current.QueryProvider.InvokeServiceOperation(serviceOperation, parameters);
            try
            {
                return UnderlyingProvider.InvokeServiceOperation(serviceOperation, parameters);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public bool IsNullPropagationRequired
        {
            get
            {
                APICallLog.Current.QueryProvider.IsNullPropagationRequired();
                try
                {
                    return UnderlyingProvider.IsNullPropagationRequired;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        #endregion
    }
    #endregion

    #region IDataServiceUpdateProvider
    public class UpdateProviderWrapper : UpdatableWrapper, DSP.IDataServiceUpdateProvider
    {
        public UpdateProviderWrapper(DSP.IDataServiceUpdateProvider underlying)
            : base(underlying)
        { }

        #region IDataServiceUpdateProvider Members

        public void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
        {
            APICallLog.Current.UpdateProvider.SetConcurrencyValues(resourceCookie, checkForEquality, concurrencyValues);
            try
            {
                (UnderlyingProvider as DSP.IDataServiceUpdateProvider).SetConcurrencyValues(resourceCookie, checkForEquality, concurrencyValues);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        #endregion
    }
    #endregion

    #region IDataServiceStreamProvider
    public class StreamProviderWrapper : ProviderWrapper<DSP.IDataServiceStreamProvider>, DSP.IDataServiceStreamProvider
    {
        public StreamProviderWrapper(DSP.IDataServiceStreamProvider underlying)
            : base(underlying)
        { }

        #region IDataServiceStreamProvider Members

        public void DeleteStream(object entity, DataServiceOperationContext operationContext)
        {
            APICallLog.Current.StreamProvider.DeleteStream(entity, operationContext);
            try
            {
                UnderlyingProvider.DeleteStream(entity, operationContext);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public Stream GetReadStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            APICallLog.Current.StreamProvider.GetReadStream(entity, etag, checkETagForEquality, operationContext);
            try
            {
                return new StreamWrapper(UnderlyingProvider.GetReadStream(entity, etag, checkETagForEquality, operationContext));
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public Uri GetReadStreamUri(object entity, DataServiceOperationContext operationContext)
        {
            APICallLog.Current.StreamProvider.GetReadStreamUri(entity, operationContext);
            try
            {
                return UnderlyingProvider.GetReadStreamUri(entity, operationContext);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public string GetStreamContentType(object entity, DataServiceOperationContext operationContext)
        {
            APICallLog.Current.StreamProvider.GetStreamContentType(entity, operationContext);
            try
            {
                return UnderlyingProvider.GetStreamContentType(entity, operationContext);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public string GetStreamETag(object entity, DataServiceOperationContext operationContext)
        {
            APICallLog.Current.StreamProvider.GetStreamETag(entity, operationContext);
            try
            {
                return UnderlyingProvider.GetStreamETag(entity, operationContext);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public Stream GetWriteStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            APICallLog.Current.StreamProvider.GetWriteStream(entity, etag, checkETagForEquality, operationContext);
            try
            {
                return new StreamWrapper(UnderlyingProvider.GetWriteStream(entity, etag, checkETagForEquality, operationContext));
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public string ResolveType(string entitySetName, DataServiceOperationContext operationContext)
        {
            APICallLog.Current.StreamProvider.ResolveType(entitySetName, operationContext);
            try
            {
                return UnderlyingProvider.ResolveType(entitySetName, operationContext);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public int StreamBufferSize
        {
            get
            {
                APICallLog.Current.StreamProvider.StreamBufferSize();
                try
                {
                    return UnderlyingProvider.StreamBufferSize;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        #endregion
    }
    #endregion

    #region IDataServicePagingProvider
    public class PagingProviderWrapper : ProviderWrapper<DSP.IDataServicePagingProvider>, DSP.IDataServicePagingProvider
    {
        public PagingProviderWrapper(DSP.IDataServicePagingProvider underlying)
            : base(underlying)
        { }

        #region IDataServicePagingProvider Members

        public object[] GetContinuationToken(IEnumerator enumerator)
        {
            APICallLog.Current.PagingProvider.GetContinuationToken(enumerator);
            try
            {
                return UnderlyingProvider.GetContinuationToken(enumerator);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public void SetContinuationToken(IQueryable query, DSP.ResourceType resourceType, object[] continuationToken)
        {
            APICallLog.Current.PagingProvider.SetContinuationToken(query, resourceType, continuationToken);
            try
            {
                UnderlyingProvider.SetContinuationToken(query, resourceType, continuationToken);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        #endregion
    }
    #endregion

    #region IDataServiceHost2
    public class IDSH2Wrapper : ProviderWrapper<Microsoft.OData.Service.IDataServiceHost2>, Microsoft.OData.Service.IDataServiceHost2
    {
        public IDSH2Wrapper(Microsoft.OData.Service.IDataServiceHost2 underlying)
            : base(underlying)
        { }

        #region IDataServiceHost Members
        public Uri AbsoluteRequestUri
        {
            get
            {
                APICallLog.Current.IDSH.AbsoluteRequestUri();
                try
                {
                    return UnderlyingProvider.AbsoluteRequestUri;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public Uri AbsoluteServiceUri
        {
            get
            {
                APICallLog.Current.IDSH.AbsoluteServiceUri();
                try
                {
                    return UnderlyingProvider.AbsoluteServiceUri;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string GetQueryStringItem(string item)
        {
            APICallLog.Current.IDSH.GetQueryStringItem(item);
            try
            {
                return UnderlyingProvider.GetQueryStringItem(item);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public void ProcessException(HandleExceptionArgs args)
        {
            APICallLog.Current.IDSH.ProcessException(args);
            try
            {
                UnderlyingProvider.ProcessException(args);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public string RequestAccept
        {
            get
            {
                APICallLog.Current.IDSH.RequestAccept();
                try
                {
                    return UnderlyingProvider.RequestAccept;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string RequestAcceptCharSet
        {
            get
            {
                APICallLog.Current.IDSH.RequestAcceptCharSet();
                try
                {
                    return UnderlyingProvider.RequestAcceptCharSet;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string RequestContentType
        {
            get
            {
                APICallLog.Current.IDSH.RequestContentType();
                try
                {
                    return UnderlyingProvider.RequestContentType;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string RequestHttpMethod
        {
            get
            {
                APICallLog.Current.IDSH.RequestHttpMethod();
                try
                {
                    return UnderlyingProvider.RequestHttpMethod;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string RequestIfMatch
        {
            get
            {
                APICallLog.Current.IDSH.RequestIfMatch();
                try
                {
                    return UnderlyingProvider.RequestIfMatch;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string RequestIfNoneMatch
        {
            get
            {
                APICallLog.Current.IDSH.RequestIfNoneMatch();
                try
                {
                    return UnderlyingProvider.RequestIfNoneMatch;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string RequestMaxVersion
        {
            get
            {
                APICallLog.Current.IDSH.RequestMaxVersion();
                try
                {
                    return UnderlyingProvider.RequestMaxVersion;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public Stream RequestStream
        {
            get
            {
                APICallLog.Current.IDSH.RequestStream();
                try
                {
                    return UnderlyingProvider.RequestStream;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string RequestVersion
        {
            get
            {
                APICallLog.Current.IDSH.RequestVersion();
                try
                {
                    return UnderlyingProvider.RequestVersion;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string ResponseCacheControl
        {
            get
            {
                string x = APICallLog.Current.IDSH.ResponseCacheControl;
                try
                {
                    return UnderlyingProvider.ResponseCacheControl;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
            set
            {
                APICallLog.Current.IDSH.ResponseCacheControl = value;
                try
                {
                    UnderlyingProvider.ResponseCacheControl = value;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string ResponseContentType
        {
            get
            {
                string x = APICallLog.Current.IDSH.ResponseContentType;
                try
                {
                    return UnderlyingProvider.ResponseContentType;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
            set
            {
                APICallLog.Current.IDSH.ResponseContentType = value;
                try
                {
                    UnderlyingProvider.ResponseContentType = value;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string ResponseETag
        {
            get
            {
                string x = APICallLog.Current.IDSH.ResponseETag;
                try
                {
                    return UnderlyingProvider.ResponseETag;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
            set
            {
                APICallLog.Current.IDSH.ResponseETag = value;
                try
                {
                    UnderlyingProvider.ResponseETag = value;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string ResponseLocation
        {
            get
            {
                string x = APICallLog.Current.IDSH.ResponseLocation;
                try
                {
                    return UnderlyingProvider.ResponseLocation;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
            set
            {
                APICallLog.Current.IDSH.ResponseLocation = value;
                try
                {
                    UnderlyingProvider.ResponseLocation = value;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public int ResponseStatusCode
        {
            get
            {
                int x = APICallLog.Current.IDSH.ResponseStatusCode;
                try
                {
                    return UnderlyingProvider.ResponseStatusCode;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
            set
            {
                APICallLog.Current.IDSH.ResponseStatusCode = value;
                try
                {
                    UnderlyingProvider.ResponseStatusCode = value;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public Stream ResponseStream
        {
            get
            {
                APICallLog.Current.IDSH.ResponseStream();
                try
                {
                    return UnderlyingProvider.ResponseStream;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public string ResponseVersion
        {
            get
            {
                string x = APICallLog.Current.IDSH.ResponseVersion;
                try
                {
                    return UnderlyingProvider.ResponseVersion;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
            set
            {
                APICallLog.Current.IDSH.ResponseVersion = value;
                try
                {
                    UnderlyingProvider.ResponseVersion = value;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        #endregion

        #region IDataServiceHost2 Members

        public System.Net.WebHeaderCollection RequestHeaders
        {
            get
            {
                APICallLog.Current.IDSH2.RequestHeaders();
                try
                {
                    return UnderlyingProvider.RequestHeaders;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }

        public System.Net.WebHeaderCollection ResponseHeaders
        {
            get
            {
                APICallLog.Current.IDSH2.ResponseHeaders();
                try
                {
                    return UnderlyingProvider.ResponseHeaders;
                }
                finally
                {
                    APICallLog.Current.Pop();
                }
            }
        }
        #endregion
    }
    #endregion
#endif

#if !ASTORIA_PRE_V3
    #region IDataServiceStreamProvider2
    public class StreamProvider2Wrapper : StreamProviderWrapper, DSP.IDataServiceStreamProvider2
    {
        public StreamProvider2Wrapper(DSP.IDataServiceStreamProvider2 underlying)
            : base(underlying)
        { }

        public Stream GetReadStream(object entity, DSP.ResourceProperty streamProperty, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            APICallLog.Current.StreamProvider2.GetReadStream(entity, streamProperty, etag, checkETagForEquality, operationContext);
            try
            {
                return new StreamWrapper((UnderlyingProvider as DSP.IDataServiceStreamProvider2).GetReadStream(entity, streamProperty, etag, checkETagForEquality, operationContext));
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public Uri GetReadStreamUri(object entity, DSP.ResourceProperty streamProperty, DataServiceOperationContext operationContext)
        {
            APICallLog.Current.StreamProvider2.GetReadStreamUri(entity, streamProperty, operationContext);
            try
            {
                return (UnderlyingProvider as DSP.IDataServiceStreamProvider2).GetReadStreamUri(entity, streamProperty, operationContext);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public string GetStreamContentType(object entity, DSP.ResourceProperty streamProperty, DataServiceOperationContext operationContext)
        {
            APICallLog.Current.StreamProvider2.GetStreamContentType(entity, streamProperty, operationContext);
            try
            {
                return (UnderlyingProvider as DSP.IDataServiceStreamProvider2).GetStreamContentType(entity, streamProperty, operationContext);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public string GetStreamETag(object entity, DSP.ResourceProperty streamProperty, DataServiceOperationContext operationContext)
        {
            APICallLog.Current.StreamProvider2.GetStreamETag(entity, streamProperty, operationContext);
            try
            {
                return (UnderlyingProvider as DSP.IDataServiceStreamProvider2).GetStreamETag(entity, streamProperty, operationContext);
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }

        public Stream GetWriteStream(object entity, DSP.ResourceProperty streamProperty, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            APICallLog.Current.StreamProvider2.GetWriteStream(entity, streamProperty, etag, checkETagForEquality, operationContext);
            try
            {
                return new StreamWrapper((UnderlyingProvider as DSP.IDataServiceStreamProvider2).GetWriteStream(entity, streamProperty, etag, checkETagForEquality, operationContext));
            }
            finally
            {
                APICallLog.Current.Pop();
            }
        }
    }
    #endregion
#endif
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.OData.Client
{
    public interface IDataServiceContext
    {
        DataServiceResponsePreference AddAndUpdateResponsePreference { get; set; }
        bool ApplyingChanges { get; }
        Uri BaseUri { get; set; }
        DataServiceClientConfigurations Configurations { get; }
        ICredentials Credentials { get; set; }
        bool DisableInstanceAnnotationMaterialization { get; set; }
        bool EnableWritingODataAnnotationWithoutPrefix { get; set; }
        ReadOnlyCollection<EntityDescriptor> Entities { get; }
        EntityParameterSendOption EntityParameterSendOption { get; set; }
        EntityTracker EntityTracker { get; set; }
        DataServiceClientFormat Format { get; }
        bool IgnoreResourceNotFoundException { get; set; }
        ReadOnlyCollection<LinkDescriptor> Links { get; }
        ODataProtocolVersion MaxProtocolVersion { get; }
        MergeOption MergeOption { get; set; }
        Func<string, Uri> ResolveEntitySet { get; set; }
        Func<Type, string> ResolveName { get; set; }
        Func<string, Type> ResolveType { get; set; }
        SaveChangesOptions SaveChangesDefaultOptions { get; set; }
#if !PORTABLELIB
        int Timeout { get; set; }
#endif
        DataServiceUrlKeyDelimiter UrlKeyDelimiter { get; set; }
        bool UsePostTunneling { get; set; }

        event EventHandler<BuildingRequestEventArgs> BuildingRequest;
        event EventHandler<ReceivingResponseEventArgs> ReceivingResponse;
        event EventHandler<SendingRequest2EventArgs> SendingRequest2;

        void AddLink(object source, string sourceProperty, object target);
        void AddObject(string entitySetName, object entity);
        void AddRelatedObject(object source, string sourceProperty, object target);
        void AttachLink(object source, string sourceProperty, object target);
        void AttachTo(string entitySetName, object entity);
        void AttachTo(string entitySetName, object entity, string etag);
        IAsyncResult BeginExecute(Uri requestUri, AsyncCallback callback, object state, string httpMethod, params OperationParameter[] operationParameters);
        IAsyncResult BeginExecute<T>(DataServiceQueryContinuation<T> continuation, AsyncCallback callback, object state);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IAsyncResult BeginExecute<TElement>(Uri requestUri, AsyncCallback callback, object state);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IAsyncResult BeginExecute<TElement>(Uri requestUri, AsyncCallback callback, object state, string httpMethod, bool singleResult, params OperationParameter[] operationParameters);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IAsyncResult BeginExecute<TElement>(Uri requestUri, AsyncCallback callback, object state, string httpMethod, params OperationParameter[] operationParameters);
        IAsyncResult BeginExecuteBatch(AsyncCallback callback, object state, params DataServiceRequest[] queries);
        IAsyncResult BeginGetReadStream(object entity, DataServiceRequestArgs args, AsyncCallback callback, object state);
        IAsyncResult BeginGetReadStream(object entity, string name, DataServiceRequestArgs args, AsyncCallback callback, object state);
        IAsyncResult BeginLoadProperty(object entity, string propertyName, AsyncCallback callback, object state);
        IAsyncResult BeginLoadProperty(object entity, string propertyName, DataServiceQueryContinuation continuation, AsyncCallback callback, object state);
        IAsyncResult BeginLoadProperty(object entity, string propertyName, Uri nextLinkUri, AsyncCallback callback, object state);
        IAsyncResult BeginSaveChanges(AsyncCallback callback, object state);
        IAsyncResult BeginSaveChanges(SaveChangesOptions options, AsyncCallback callback, object state);
        void CancelRequest(IAsyncResult asyncResult);
        void ChangeState(object entity, EntityStates state);
        DataServiceQuery<T> CreateFunctionQuery<T>();
        DataServiceQuery<T> CreateFunctionQuery<T>(string path, string functionName, bool isComposable, params UriOperationParameter[] parameters);
        DataServiceQuerySingle<T> CreateFunctionQuerySingle<T>(string path, string functionName, bool isComposable, params UriOperationParameter[] parameters);
        DataServiceQuery<T> CreateQuery<T>(string entitySetName);
        DataServiceQuery<T> CreateQuery<T>(string resourcePath, bool isComposable);
        DataServiceQuery<T> CreateSingletonQuery<T>(string singletonName);
        void DeleteLink(object source, string sourceProperty, object target);
        void DeleteObject(object entity);
        bool Detach(object entity);
        bool DetachLink(object source, string sourceProperty, object target);
        OperationResponse EndExecute(IAsyncResult asyncResult);
        IEnumerable<TElement> EndExecute<TElement>(IAsyncResult asyncResult);
        DataServiceResponse EndExecuteBatch(IAsyncResult asyncResult);
        DataServiceStreamResponse EndGetReadStream(IAsyncResult asyncResult);
        QueryOperationResponse EndLoadProperty(IAsyncResult asyncResult);
        DataServiceResponse EndSaveChanges(IAsyncResult asyncResult);
#if !PORTABLELIB
        OperationResponse Execute(Uri requestUri, string httpMethod, params OperationParameter[] operationParameters);
        QueryOperationResponse<T> Execute<T>(DataServiceQueryContinuation<T> continuation);
        IEnumerable<TElement> Execute<TElement>(Uri requestUri);
        IEnumerable<TElement> Execute<TElement>(Uri requestUri, string httpMethod, bool singleResult, params OperationParameter[] operationParameters);
        IEnumerable<TElement> Execute<TElement>(Uri requestUri, string httpMethod, params OperationParameter[] operationParameters);
#endif
        Task<OperationResponse> ExecuteAsync(Uri requestUri, string httpMethod, params OperationParameter[] operationParameters);
        Task<IEnumerable<TElement>> ExecuteAsync<TElement>(DataServiceQueryContinuation<TElement> continuation);
        Task<IEnumerable<TElement>> ExecuteAsync<TElement>(Uri requestUri);
        Task<IEnumerable<TElement>> ExecuteAsync<TElement>(Uri requestUri, string httpMethod, bool singleResult, params OperationParameter[] operationParameters);
        Task<IEnumerable<TElement>> ExecuteAsync<TElement>(Uri requestUri, string httpMethod, params OperationParameter[] operationParameters);
#if !PORTABLELIB
        DataServiceResponse ExecuteBatch(params DataServiceRequest[] queries);
#endif
        Task<DataServiceResponse> ExecuteBatchAsync(params DataServiceRequest[] queries);
        EntityDescriptor GetEntityDescriptor(object entity);
        LinkDescriptor GetLinkDescriptor(object source, string sourceProperty, object target);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        Uri GetMetadataUri();
#if !PORTABLELIB
        DataServiceStreamResponse GetReadStream(object entity);
        DataServiceStreamResponse GetReadStream(object entity, DataServiceRequestArgs args);
        DataServiceStreamResponse GetReadStream(object entity, string acceptContentType);
        DataServiceStreamResponse GetReadStream(object entity, string name, DataServiceRequestArgs args);
#endif
        Task<DataServiceStreamResponse> GetReadStreamAsync(object entity, DataServiceRequestArgs args);
        Task<DataServiceStreamResponse> GetReadStreamAsync(object entity, string name, DataServiceRequestArgs args);
        Uri GetReadStreamUri(object entity);
        Uri GetReadStreamUri(object entity, string name);
#if !PORTABLELIB
        QueryOperationResponse LoadProperty(object entity, string propertyName);
        QueryOperationResponse LoadProperty(object entity, string propertyName, DataServiceQueryContinuation continuation);
        QueryOperationResponse LoadProperty(object entity, string propertyName, Uri nextLinkUri);
        QueryOperationResponse<T> LoadProperty<T>(object entity, string propertyName, DataServiceQueryContinuation<T> continuation);
#endif
        Task<QueryOperationResponse> LoadPropertyAsync(object entity, string propertyName);
        Task<QueryOperationResponse> LoadPropertyAsync(object entity, string propertyName, DataServiceQueryContinuation continuation);
        Task<QueryOperationResponse> LoadPropertyAsync(object entity, string propertyName, Uri nextLinkUri);
#if !PORTABLELIB 
        DataServiceResponse SaveChanges();
        DataServiceResponse SaveChanges(SaveChangesOptions options);
#endif
        Task<DataServiceResponse> SaveChangesAsync();
        Task<DataServiceResponse> SaveChangesAsync(SaveChangesOptions options);
        void SetLink(object source, string sourceProperty, object target);
        void SetSaveStream(object entity, Stream stream, bool closeStream, DataServiceRequestArgs args);
        void SetSaveStream(object entity, Stream stream, bool closeStream, string contentType, string slug);
        void SetSaveStream(object entity, string name, Stream stream, bool closeStream, DataServiceRequestArgs args);
        void SetSaveStream(object entity, string name, Stream stream, bool closeStream, string contentType);
        bool TryGetAnnotation<TFunc, TResult>(Expression<TFunc> expression, string term, out TResult annotation);
        bool TryGetAnnotation<TFunc, TResult>(Expression<TFunc> expression, string term, string qualifier, out TResult annotation);
        bool TryGetAnnotation<TResult>(object source, string term, out TResult annotation);
        bool TryGetAnnotation<TResult>(object source, string term, string qualifier, out TResult annotation);
        bool TryGetEntity<TEntity>(Uri identity, out TEntity entity) where TEntity : class;
        bool TryGetUri(object entity, out Uri identity);
        void UpdateObject(object entity);
        void UpdateRelatedObject(object source, string sourceProperty, object target);
    }
}
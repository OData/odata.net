//---------------------------------------------------------------------
// <copyright file="StreamingService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using Microsoft.OData.Client;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.ServiceModel.Channels;
    using System.Net;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml;
    using System.Reflection;

    public class StreamingServiceObjectBase
    {
        public int ID { get; set; }
        internal virtual void ReInit() { }
    }

    public class StreamingServicePhotoBase : StreamingServiceObjectBase
    {
        public int Length { get; set; }
        public string Name { get; set; }
        public bool AlternativeUri { get; set; }

        protected byte[] content;
        protected string contentType;

        internal byte[] GetContent()
        {
            return this.content;
        }

        internal void SetContent(byte[] content)
        {
            this.content = content;
            this.Length = content.Length;
        }

        internal string GetContentType()
        {
            return this.contentType;
        }

        internal void SetContentType(string contentType)
        {
            this.contentType = contentType;
        }

        internal override void ReInit()
        {
            this.Name = null;
            this.AlternativeUri = false;
            base.ReInit();
        } 
    }

    [HasStream]
    public class StreamingServicePhoto : StreamingServicePhotoBase
    {
        internal byte[] Content 
        {
            get { return this.content; }
            set { this.content = value; }
        }
        internal string ContentType 
        {
            get { return this.contentType; }
            set { this.contentType = value; }
        }
    }

    [MediaEntry("Content")]
    [MimeTypeProperty("Content", "ContentType")]
    [HasStream]
    public class StreamingServiceV1Photo : StreamingServicePhotoBase
    {
        public byte[] Content
        {
            get { return this.content; }
            set { this.content = value; }
        }
        public string ContentType
        {
            get { return this.contentType; }
            set { this.contentType = value; }
        }
    }

    public class StreamingServiceEntityWithoutStream : StreamingServiceObjectBase
    {
        public string Name { get; set; }

        internal override void ReInit()
        {
            this.Name = null;
            base.ReInit();
        }
    }

    [HasStream]
    public class StreamingServiceRequestHeaders : StreamingServiceObjectBase
    {
        public string Method { get; set; }
        public string Uri { get; set; }
        internal XDocument Headers { get; set; }

        internal override void ReInit()
        {
            this.Method = null;
            this.Uri = null;
            base.ReInit();
        }
    }

    public class StreamingServiceContext : IUpdatable
    {
        private static List<StreamingServicePhoto> _photos;
        private static List<StreamingServiceV1Photo> _v1photos;
        private static List<StreamingServiceEntityWithoutStream> _entitiesWithoutStream;
        private static List<StreamingServiceRequestHeaders> _requestHeaders;

        private static int _id = 100;

        static StreamingServiceContext()
        {
            ResetContent();
        }

        public static void ResetContent()
        {
            _photos = new List<StreamingServicePhoto> {
                new StreamingServicePhoto() { ID = 1, Name = "Number1", Length = 3, Content = new byte[] { 11, 12, 13 }, ContentType = "type/1" },
                new StreamingServicePhoto() { ID = 2, Name = "Number2", Length = 3, Content = new byte[] { 21, 22, 23 }, ContentType = "type/2" },
                new StreamingServicePhoto() { ID = 3, Name = "Number3", Length = 5, Content = new byte[] { 31, 32, 33, 34, 35 }, ContentType = "type/3" },
                new StreamingServicePhoto() { ID = 4, Name = "Number1", Length = 3, Content = new byte[] { 41, 42, 43 }, ContentType = "type/4", AlternativeUri = true },
                new StreamingServicePhoto() { ID = 5, Name = "Number2", Length = 3, Content = new byte[] { 51, 52, 53 }, ContentType = "type/5", AlternativeUri = true },
                new StreamingServicePhoto() { ID = 6, Name = "Number3", Length = 5, Content = new byte[] { 61, 62, 63, 64, 65 }, ContentType = "type/6", AlternativeUri = true },
            };

            _v1photos = new List<StreamingServiceV1Photo>
            {
                new StreamingServiceV1Photo() { ID = 1, Name = "Number1", Length = 3, Content = new byte[] { 11, 12, 13 }, ContentType = "type/1" },
                new StreamingServiceV1Photo() { ID = 2, Name = "Number2", Length = 3, Content = new byte[] { 21, 22, 23 }, ContentType = "type/2" },
                new StreamingServiceV1Photo() { ID = 3, Name = "Number3", Length = 5, Content = new byte[] { 31, 32, 33, 34, 35 }, ContentType = "type/3" },
                new StreamingServiceV1Photo() { ID = 4, Name = "Number1", Length = 3, Content = new byte[] { 41, 42, 43 }, ContentType = "type/4", AlternativeUri = true },
                new StreamingServiceV1Photo() { ID = 5, Name = "Number2", Length = 3, Content = new byte[] { 51, 52, 53 }, ContentType = "type/5", AlternativeUri = true },
                new StreamingServiceV1Photo() { ID = 6, Name = "Number3", Length = 5, Content = new byte[] { 61, 62, 63, 64, 65 }, ContentType = "type/6", AlternativeUri = true },
            };

            _entitiesWithoutStream = new List<StreamingServiceEntityWithoutStream> {
                new StreamingServiceEntityWithoutStream() { ID = 1, Name = "First" },
                new StreamingServiceEntityWithoutStream() { ID = 2, Name = "Second" },
            };

            _requestHeaders = new List<StreamingServiceRequestHeaders>();
        }

        public IQueryable<StreamingServicePhoto> Photos
        {
            get { return _photos.AsQueryable(); }
        }

        public IQueryable<StreamingServiceV1Photo> V1Photos
        {
            get { return _v1photos.AsQueryable(); }
        }

        public IQueryable<StreamingServiceEntityWithoutStream> EntitiesWithoutStream
        {
            get { return _entitiesWithoutStream.AsQueryable(); }
        }

        public IQueryable<StreamingServiceRequestHeaders> RequestHeaders
        {
            get { return _requestHeaders.AsQueryable(); }
        }

        public static StreamingServicePhoto GetPhoto(int id)
        {
            return _photos.Where(p => p.ID == id).SingleOrDefault();
        }

        public static void ResetRequestHeaders()
        {
            _requestHeaders.Clear();
        }

        public static void AddRequestHeaders(string method, string uri, XDocument headers)
        {
            _requestHeaders.Add(new StreamingServiceRequestHeaders() { ID = _id++, Method = method, Headers = headers, Uri = uri });
        }

        #region IUpdatable Members

        public object CreateResource(string containerName, string fullTypeName)
        {
            StreamingServiceObjectBase resource = null;

            if (containerName == "Photos")
            {
                if (fullTypeName == (typeof(StreamingServicePhoto).Namespace + "." + typeof(StreamingServicePhoto).Name))
                {
                    resource = new StreamingServicePhoto();
                    resource.ID = _id++;
                    this.pendingChanges.Add(new PendingChange() { ChangeType = ChangeType.Add, List = _photos, Resource = resource });
                }
            }
            else if (containerName == "V1Photos")
            {
                if (fullTypeName == (typeof(StreamingServiceV1Photo).Namespace + "." + typeof(StreamingServiceV1Photo).Name))
                {
                    resource = new StreamingServiceV1Photo();
                    resource.ID = _id++;
                    this.pendingChanges.Add(new PendingChange() { ChangeType = ChangeType.Add, List = _v1photos, Resource = resource });
                }
            }
            else if (containerName == "EntitiesWithoutStream")
            {
                if (fullTypeName == (typeof(StreamingServiceEntityWithoutStream).Namespace + "." + typeof(StreamingServiceEntityWithoutStream).Name))
                {
                    resource = new StreamingServiceEntityWithoutStream();
                    resource.ID = _id++;
                    this.pendingChanges.Add(new PendingChange() { ChangeType = ChangeType.Add, List = _entitiesWithoutStream, Resource = resource });
                }
            }

            if (resource == null)
            {
                throw new Exception("Unknown resource to be created: " + containerName + ", " + fullTypeName);
            }
            return resource;
        }

        public object GetResource(IQueryable query, string fullTypeName)
        {
            object resource = null;
            foreach (object r in query)
            {
                if (resource != null)
                {
                    throw new Exception("query returning more than one type");
                }

                resource = r;
            }

            return resource;
        }

        public object ResetResource(object resource)
        {
            if (resource is StreamingServiceObjectBase)
            {
                ((StreamingServiceObjectBase)resource).ReInit();
                return resource;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void SetValue(object targetResource, string propertyName, object propertyValue)
        {
            if (propertyName == "ID") return;
            PropertyInfo pi = targetResource.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            pi.SetValue(targetResource, propertyValue, null);
        }

        public object GetValue(object targetResource, string propertyName)
        {
            PropertyInfo pi = targetResource.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            return pi.GetValue(targetResource, null);
        }

        public void SetReference(object targetResource, string propertyName, object propertyValue)
        {
            throw new NotImplementedException();
        }

        public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            throw new NotImplementedException();
        }

        public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            throw new NotImplementedException();
        }

        public void DeleteResource(object targetResource)
        {
            if (targetResource is StreamingServiceV1Photo)
            {
                this.pendingChanges.Add(new PendingChange() { ChangeType = ChangeType.Delete, List = _v1photos, Resource = targetResource }); 
            }
            else if (targetResource is StreamingServiceEntityWithoutStream)
            {
                this.pendingChanges.Add(new PendingChange() { ChangeType = ChangeType.Delete, List = _entitiesWithoutStream, Resource = targetResource });
            }
            else
            {
                this.pendingChanges.Add(new PendingChange() { ChangeType = ChangeType.Delete, List = _photos, Resource = targetResource });
            }
        }

        public void SaveChanges()
        {
            foreach (PendingChange pc in this.pendingChanges)
            {
                switch(pc.ChangeType)
                {
                    case ChangeType.Add:
                        pc.List.Add(pc.Resource);
                        break;

                    case ChangeType.Delete:
                        pc.List.Remove(pc.Resource);
                        break;

                    case ChangeType.UpdateStream:
                        ((PhotoWriteStream)pc.Resource).Apply();
                        break;
                }
            }
        }

        public object ResolveResource(object resource)
        {
            return resource;
        }

        public void ClearChanges()
        {
            this.pendingChanges.Clear();
        }

        #endregion

        public enum ChangeType
        {
            Add,
            Delete,
            UpdateStream,
        }

        private class PendingChange
        {

            public ChangeType ChangeType { get; set; }
            public IList List { get; set; }
            public object Resource { get; set; }
        }

        private List<PendingChange> pendingChanges = new List<PendingChange>();

        public Stream GetPhotoWriteStream(StreamingServicePhotoBase photo, string contentType)
        {
            PhotoWriteStream stream = new PhotoWriteStream(photo, contentType);
            this.pendingChanges.Add(new PendingChange() { ChangeType = ChangeType.UpdateStream, List = null, Resource = stream });
            return stream;
        }

        #region PhotoWriteStream
        public class PhotoWriteStream : Stream, IDisposable
        {
            private StreamingServicePhotoBase photo;
            private MemoryStream stream;
            private string contentType;

            public PhotoWriteStream(StreamingServicePhotoBase photo, string contentType)
            {
                this.photo = photo;
                this.stream = new MemoryStream();
                this.contentType = contentType;
            }

            public override bool CanRead
            {
                get { return false; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return true; }
            }

            public override void Flush()
            {
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
                throw new NotImplementedException();
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
                this.stream.Write(buffer, offset, count);
            }

            public void Apply()
            {
                this.photo.SetContent(this.stream.ToArray());
                this.photo.SetContentType(this.contentType);
            }
        }
        #endregion
    }

    // Took a breaking change, since the content type now must be correct.
    // In V1/V2, astoria client used to parse text/plain payloads even when response content type value was application/atom+xml
    // After integration, it must be text/plain.
    [MimeType("SetContentServiceUri", "text/plain")]
    public class StreamingService : OpenWebDataService<StreamingServiceContext>, IServiceProvider, IDataServiceStreamProvider
    {
        public static Func<object, string, bool?, DataServiceOperationContext, System.IO.Stream> GetReadStreamOverride;
        public static Action<DataServiceConfiguration> InitializeServiceOverride;

        public static new void InitializeService(DataServiceConfiguration config)
        {
            if (InitializeServiceOverride != null)
            {
                InitializeServiceOverride(config);
            }
            else
            {
                config.SetEntitySetAccessRule("*", EntitySetRights.All);
                config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                config.UseVerboseErrors = true;
                config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            }
        }

        private static Uri contentServiceUri;

        [WebGet]
        public bool SetContentServiceUri(string uri)
        {
            if (!uri.EndsWith("/"))
            {
                uri = uri + "/";
            }
            contentServiceUri = new Uri(uri);
            return true;
        }

        [WebGet]
        public bool ResetRequestHeaders()
        {
            StreamingServiceContext.ResetRequestHeaders();
            return true;
        }

        [WebGet]
        public bool ResetContent()
        {
            StreamingServiceContext.ResetContent();
            return true;
        }

        protected override void OnStartProcessingRequest(ProcessRequestArgs args)
        {
            base.OnStartProcessingRequest(args);
            WebOperationContext context = WebOperationContext.Current;
            if (context != null)
            {
                if (!args.IsBatchOperation)
                {
                    StreamingServiceContext.AddRequestHeaders(context.IncomingRequest.Method, args.RequestUri.ToString(),
                        StreamingServiceHelpers.SerializeRequestHeaders(context.IncomingRequest.Headers));
                }
                if (context.IncomingRequest.Headers["Test_FailThisRequest"] == "true")
                {
                    throw new InvalidOperationException("Request failed for testing purposes.");
                }
            }
        }

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IDataServiceStreamProvider))
            {
                return this;
            }
            else if (serviceType == typeof(IUpdatable))
            {
                return (StreamingServiceContext)this.CurrentDataSource;
            }

            return null;
        }

        #endregion

        #region IDataServiceStreamProvider Members

        public int StreamBufferSize
        {
            get { return 0; }
        }

        public System.IO.Stream GetReadStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            if (GetReadStreamOverride != null)
            {
                return GetReadStreamOverride(entity, etag, checkETagForEquality, operationContext);
            }

            StreamingServiceRequestHeaders headers = entity as StreamingServiceRequestHeaders;
            if (headers != null)
            {
                return new MemoryStream(Encoding.UTF8.GetBytes(headers.Headers.ToString()));
            }

            StreamingServicePhotoBase photo = entity as StreamingServicePhotoBase;
            if (photo == null)
            {
                throw new ArgumentException("The specified entity is not a photo entity.", "entity");
            }

            return new MemoryStream(photo.GetContent());
        }

        public System.IO.Stream GetWriteStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            if (operationContext.RequestHeaders["Content-Type"] == "test/fail-this-request")
            {
                throw new InvalidOperationException("Request failed for test reasons.");
            }
            StreamingServicePhotoBase photo = entity as StreamingServicePhotoBase;
            if (photo == null)
            {
                throw new ArgumentException("The specified entity is not a photo entity.", "entity");
            }
            return ((StreamingServiceContext)this.CurrentDataSource).GetPhotoWriteStream(photo, operationContext.RequestHeaders["Content-Type"]);
        }

        public void DeleteStream(object entity, DataServiceOperationContext operationContext)
        {
            // Do nothing - the stream gets deleted with the MLE
        }

        public string GetStreamContentType(object entity, DataServiceOperationContext operationContext)
        {
            StreamingServiceRequestHeaders headers = entity as StreamingServiceRequestHeaders;
            if (headers != null)
            {
                return "text/plain";
            }

            StreamingServicePhotoBase photo = entity as StreamingServicePhotoBase;
            if (photo == null)
            {
                throw new ArgumentException("The specified entity is not a photo entity.", "entity");
            }

            return photo.GetContentType();
        }

        public Uri GetReadStreamUri(object entity, DataServiceOperationContext operationContext)
        {
            StreamingServiceRequestHeaders headers = entity as StreamingServiceRequestHeaders;
            if (headers == null)
            {
                StreamingServicePhotoBase photo = entity as StreamingServicePhotoBase;
                if (photo == null)
                {
                    throw new ArgumentException("The specified entity is not a photo entity.", "entity");
                }

                StreamingServicePhoto photoV2 = photo as StreamingServicePhoto;
                if (photoV2 != null && photoV2.AlternativeUri && contentServiceUri != null)
                {
                    return new Uri(contentServiceUri.ToString() + "PhotoContent?id=" + photoV2.ID.ToString(), UriKind.Absolute);
                }
            }

            return null;
        }

        public string GetStreamETag(object entity, DataServiceOperationContext operationContext)
        {
            return null;
        }

        public string ResolveType(string entitySetName, DataServiceOperationContext operationContext)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

    }

    internal static class StreamingServiceHelpers
    {
        internal static XDocument SerializeRequestHeaders(WebHeaderCollection headers)
        {
            XDocument doc = new XDocument();
            doc.Add(new XElement("headers"));
            foreach (string name in headers.Keys)
            {
                XElement header = new XElement("header", new XAttribute("name", name));
                foreach (string value in headers.GetValues(name))
                {
                    header.Add(new XElement("value", value));
                }
                doc.Root.Add(header);
            }
            return doc;
        }
    }

    [ServiceContract]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class StreamingContentService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "PhotoContent?id={id}")]
        public Stream GetPhotoContent(int id)
        {
            WebOperationContext context = WebOperationContext.Current;

            StreamingServicePhoto photo = StreamingServiceContext.GetPhoto(id);
            if (photo == null)
            {
                context.OutgoingResponse.SetStatusAsNotFound();
                return new MemoryStream();
            }

            if (context.IncomingRequest.Headers[HttpRequestHeader.Accept] == "test/ReplyWithHeaders")
            {
                XDocument doc = StreamingServiceHelpers.SerializeRequestHeaders(context.IncomingRequest.Headers);

                context.OutgoingResponse.Headers[HttpResponseHeader.ContentEncoding] = "UTF-8";
                MemoryStream s = new MemoryStream();
                XmlWriterSettings ws = new XmlWriterSettings()
                {
                    OmitXmlDeclaration = true,
                    Encoding = Encoding.UTF8
                };

                using (XmlWriter w = XmlWriter.Create(s, ws))
                {
                    doc.Save(w);
                }

                context.OutgoingResponse.ContentLength = s.Length;
                s.Seek(0, SeekOrigin.Begin);
                return s;
            }

            if (context.IncomingRequest.Headers[HttpRequestHeader.Accept] == "test/AddTestHeaders")
            {
                context.OutgoingResponse.Headers.Add("test-single", "value1");
                context.OutgoingResponse.Headers.Add("test-multi", "value2");
                context.OutgoingResponse.Headers.Add("test-multi", "value3");
            }

            if (context.IncomingRequest.Headers[HttpRequestHeader.Accept] == "test/AddContentPropertyHeaders")
            {
                context.OutgoingResponse.Headers.Add(HttpResponseHeader.ContentType, "test/ContentType");
                context.OutgoingResponse.Headers.Add("Content-Disposition", "test-content-disposition");
            }

            if (context.OutgoingResponse.ContentType == null)
            {
                if (photo.ContentType != "no-content-type")
                {
                    context.OutgoingResponse.ContentType = photo.ContentType;
                }
            }

            context.OutgoingResponse.ContentLength = photo.Content.Length;
            return new MemoryStream(photo.Content);
        }
    }
}

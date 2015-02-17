//---------------------------------------------------------------------
// <copyright file="RequestInfoContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service.Providers;
    using System.ServiceModel.Web;
    using System.ServiceModel.Activation;
    using System.IO;

    public class HeaderAsEntity
    {
        public string ID { get; set; }
        public string Value { get; set; }
    }

    public class SimpleEntity
    {
        public string ID { get; set; }
    }

    [MediaEntry("Photo")]
    [MimeTypeProperty("Photo", "PhotoType")]
    [MimeType("Photo", "image/jpeg")] // this is the server-side one
    public class PhotoEntity
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public byte[] Photo { get; set; }

        public string PhotoType { get; set; }
    }

    [MediaEntry("PhotoWrong")]
    [MimeTypeProperty("Photo", "PhotoType")]
    public class PhotoEntityWrongName1
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public byte[] Photo { get; set; }

        public string PhotoType { get; set; }
    }

    [MediaEntry("Photo")]
    [MimeTypeProperty("Photo", "PhotoTypeWrong")]
    public class PhotoEntityWrongName2
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public byte[] Photo { get; set; }

        public string PhotoType { get; set; }
    }

    [MediaEntry("Photo")]
    [MimeTypeProperty("PhotoNameWrong", "PhotoType")]
    public class PhotoEntityWrongName3
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public byte[] Photo { get; set; }

        public string PhotoType { get; set; }
    }

    public class RequestInfoContext : IUpdatable
    {
        // this collection holds "inserted" header entities and it's combined with the actual headers
        // (only kept within a single request)
        private List<HeaderAsEntity> _inserted = new List<HeaderAsEntity>();

        // pending for insert during modification operation
        private List<HeaderAsEntity> _pendingInsert = new List<HeaderAsEntity>();

        public IQueryable<HeaderAsEntity> HttpHeaders
        {
            get
            {
                // if things don't look good just return an empty set
                if (OperationContext.Current == null ||
                    OperationContext.Current.IncomingMessageProperties == null ||
                    ((HttpRequestMessageProperty)OperationContext.Current.IncomingMessageProperties["httpRequest"]).Headers == null)
                {
                    return new HeaderAsEntity[0].AsQueryable();
                }

                var h = ((HttpRequestMessageProperty)OperationContext.Current.IncomingMessageProperties["httpRequest"]).Headers;

                return (from k in h.AllKeys
                        select new HeaderAsEntity
                        {
                            ID = k,
                            Value = h[k]
                        })
                        .Union(_inserted)
                        .AsQueryable();
            }
        }

        public IQueryable<SimpleEntity> FixedList
        {
            get
            {
                SimpleEntity[] list = new SimpleEntity[] {
                    new SimpleEntity { ID = "aaa" },
                    new SimpleEntity { ID = "bbb" },
                    new SimpleEntity { ID = "ccc" }
                };

                return list.AsQueryable();
            }
        }

        // this is used to simulate media link entries
        public IQueryable<PhotoEntity> Photos
        {
            get
            {
                PhotoEntity[] list = new PhotoEntity[] {
                    new PhotoEntity { ID = 1, Name = "aaa", Photo = new byte[] { 11, 12, 13 }, PhotoType = null },
                    new PhotoEntity { ID = 2, Name = "bbb", Photo = new byte[] { 21, 22, 23 }, PhotoType = null },
                    new PhotoEntity { ID = 3, Name = "ccc", Photo = new byte[] { 31, 32, 33 }, PhotoType = null }
                };

                return list.AsQueryable();
            }
        }

        #region IUpdatable Members

        public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            throw new System.NotImplementedException();
        }

        public object CreateResource(string containerName, string fullTypeName)
        {
            HeaderAsEntity header = new HeaderAsEntity();
            _pendingInsert.Add(header);
            return header;
        }

        public void DeleteResource(object targetResource)
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object>> GetKeys(object targetResource)
        {
            throw new System.NotImplementedException();
        }

        public object GetResource(IQueryable query, string fullTypeName)
        {
            throw new System.NotImplementedException();
        }

        public object GetValue(object targetResource, string propertyName)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            throw new System.NotImplementedException();
        }

        public object ResetResource(object resource)
        {
            throw new System.NotImplementedException();
        }

        public object ResolveResource(object resource)
        {
            return resource;
        }

        public void SaveChanges()
        {
            _inserted.AddRange(_pendingInsert);
            _pendingInsert.Clear();
        }

        public void SetReference(object targetResource, string propertyName, object propertyValue)
        {
            throw new System.NotImplementedException();
        }

        public void SetValue(object targetResource, string propertyName, object propertyValue)
        {
            HeaderAsEntity header = targetResource as HeaderAsEntity;
            if (header == null)
            {
                throw new InvalidOperationException("Only HeaderAsEntity types are supported in this update interface implementation");
            }

            switch (propertyName)
            {
                case "ID":
                    header.ID = (string)propertyValue;
                    break;

                case "Value":
                    header.Value = (string)propertyValue;
                    break;

                default:
                    throw new InvalidOperationException("Only the 'ID' and 'Value' properties can be modified");
            }
        }

        public void ClearChanges()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    [MimeType("EchoAtom", "application/atom+xml")]
    public class RequestInfoService : DataService<RequestInfoContext>
    {
        public static void InitializeService(IDataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
            config.UseVerboseErrors = true;
        }

        [ChangeInterceptor("HttpHeaders")]
        public void HttpHeadersChangeInterceptor(HeaderAsEntity header, UpdateOperations action)
        {
            if (action == UpdateOperations.Add && header.Value == null)
            {
                if (OperationContext.Current != null &&
                    OperationContext.Current.IncomingMessageProperties != null &&
                    ((HttpRequestMessageProperty)OperationContext.Current.IncomingMessageProperties["httpRequest"]).Headers != null)
                {
                    var h = ((HttpRequestMessageProperty)OperationContext.Current.IncomingMessageProperties["httpRequest"]).Headers;
                    header.Value = h["CustomTestHeader"];
                }
            }
        }

        // used by the custom namespaces test to make Astoria serve content that wouldn't normally serve
        [WebGet]
        [SingleResult]
        public IQueryable<string> EchoAtom(string s)
        {
            s = System.Web.HttpUtility.UrlDecode(s);
            return new string[] { s }.AsQueryable();
        }

        [WebGet]
        public SimpleEntity EchoHeaders()
        {
            SimpleEntity e = new SimpleEntity();
            e.ID = WebOperationContext.Current.IncomingRequest.Headers.ToString();
            return e;
        }
    }
}

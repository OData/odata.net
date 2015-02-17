//---------------------------------------------------------------------
// <copyright file="BlobServer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests
{
    using Microsoft.OData.Service;
    using System;
    using Microsoft.OData.Service.Providers;
    using System.IO;
    using System.Data.Test.Astoria;
    using System.Collections.Generic;
    using System.Collections;
    using System.Reflection;
    using System.Data;
    using Microsoft.OData.Client;
    using System.Net;
    using System.Linq;
    using System.Diagnostics;

    #region BLOB Server

    public class PhotoService : DataService<PhotoContext>, IServiceProvider
    {
        public static void InitializeService(IDataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            ((DataServiceConfiguration)config).DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
        }

        #region IServiceProvider Members

        object IServiceProvider.GetService(Type serviceType)
        {
            if (serviceType == typeof(IDataServiceStreamProvider))
            {
                return new DataServiceStreamProvider();
            }

            return null;
        }

        #endregion
    }

    public class DataServiceStreamProvider : IDataServiceStreamProvider
    {
        internal static string RootStoragePath = Path.Combine(Environment.CurrentDirectory, typeof(DataServiceStreamProvider).Name + "_" + Guid.NewGuid().ToString("N"));
        internal static string RootPhotosStoragePath = Path.Combine(RootStoragePath, "Photos");
        internal static string RootCustomerBlobStoragePath = Path.Combine(RootStoragePath, "CustomerBlob");
        internal static int DefaultStreamBufferSize = 4000;
        internal static bool UseAlternativeReadStreamUri = true;
        private bool isDisposed;

        public DataServiceStreamProvider()
        {
            Init();
        }

        internal static void Init()
        {
            IOUtil.EnsureDirectoryExists(RootStoragePath);
            IOUtil.EnsureDirectoryExists(RootPhotosStoragePath);
            IOUtil.EnsureDirectoryExists(RootCustomerBlobStoragePath);
        }

        #region IDataServiceStreamProvider Members

        public int StreamBufferSize
        {
            get
            {
                this.ThrowIfDisposed();
                return DefaultStreamBufferSize;
            }
        }

        public Stream GetReadStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            this.ThrowIfDisposed();
            ValidateArguments(entity, operationContext);
            string specialHeader = operationContext.RequestHeaders["Test_RoundtripHeader"];
            if (!string.IsNullOrEmpty(specialHeader))
            {
                operationContext.ResponseHeaders["Test_RoundtripHeader"] = specialHeader;
            }

            specialHeader = operationContext.RequestHeaders["Test_ReplyWithThisContent"];
            if (string.IsNullOrEmpty(specialHeader))
            {
                FileStream fs = File.OpenRead(GetStoragePath(entity));
                return fs;
            }
            else
            {
                return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(specialHeader));
            }
        }

        public Stream GetWriteStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            this.ThrowIfDisposed();
            int entityId = 0;

            if (entity.GetType() == typeof(Photo))
            {
                Photo p = (Photo)entity;
                p.LastUpdated = DateTime.Now;

                if (operationContext.RequestMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(operationContext.RequestHeaders["Slug"]))
                {
                    string slug = operationContext.RequestHeaders["Slug"];
                    int id;
                    if (int.TryParse(slug, out id))
                    {
                        p.ID = id;
                    }
                    else
                    {
                        p.Description = slug;
                    }
                }

                entityId = p.ID;
            }
            else if (entity.GetType() == typeof(ClrNamespace.CustomerBlob) || entity.GetType() == typeof(ClrNamespace.CustomerBlobWithBirthday))
            {
                ClrNamespace.Customer c = (ClrNamespace.Customer)entity;

                if (operationContext.RequestMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(operationContext.RequestHeaders["Slug"]))
                {
                    string slug = operationContext.RequestHeaders["Slug"];
                    c.Concurrency = slug;
                    int id;
                    if (int.TryParse(slug, out id))
                    {
                        c.ID = id;
                        c.Name = "Customer " + slug;
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("Invalid Slug header value '{0}'.", slug));
                    }
                }

                entityId = c.ID;
            }

            ValidateArguments(entity, operationContext);

            if (checkETagForEquality == true)
            {
                if (etag != "\"MediaResourceETag" + entityId + "\"")
                {
                    throw new InvalidOperationException("ETag for media resource did not match.");
                }
            }

            FileStream fs = File.Open(GetStoragePath(entity), FileMode.Create, FileAccess.Write);
            return fs;
        }

        public void DeleteStream(object entity, DataServiceOperationContext operationContext)
        {
            this.ThrowIfDisposed();
            ValidateArguments(entity, operationContext);

            if (entity.GetType() == typeof(Photo))
            {
                Photo p = (Photo)entity;
                p.LastUpdated = DateTime.Now;
            }

            File.Delete(GetStoragePath(entity));
        }

        public string GetStreamContentType(object entity, DataServiceOperationContext operationContext)
        {
            this.ThrowIfDisposed();
            ValidateArguments(entity, operationContext);
            return GetContentType(entity);
        }

        public Uri GetReadStreamUri(object entity, DataServiceOperationContext operationContext)
        {
            this.ThrowIfDisposed();
            ValidateArguments(entity, operationContext);
            return GetReadStreamUri(entity);
        }

        public string GetStreamETag(object entity, DataServiceOperationContext operationContext)
        {
            this.ThrowIfDisposed();
            ValidateArguments(entity, operationContext);

            int entityId = 0;
            if (entity.GetType() == typeof(Photo))
            {
                entityId = ((Photo)entity).ID;
            }
            else if (entity.GetType() == typeof(ClrNamespace.CustomerBlob) || entity.GetType() == typeof(ClrNamespace.CustomerBlobWithBirthday))
            {
                entityId = ((ClrNamespace.Customer)entity).ID;
            }

            return "\"MediaResourceETag" + entityId + "\"";
        }

        public string ResolveType(string entitySetName, DataServiceOperationContext operationContext)
        {
            this.ThrowIfDisposed();
            string type = operationContext.RequestHeaders["CustomRequestHeader_ItemType"];
            return type;
        }

        public void Dispose()
        {
            this.ThrowIfDisposed();
            this.isDisposed = true;
        }

        #endregion

        internal static Uri GetReadStreamUri(object entity)
        {
            if (entity.GetType() == typeof(Photo))
            {
                if (!UseAlternativeReadStreamUri)
                {
                    return null;
                }
                Photo p = (Photo)entity;
                switch (p.ID % 2)
                {
                    case 1:
                        return new Uri("http://localhost/someuri/");
                    default:
                        return null;
                }
            }

            return null;
        }

        internal static string GetStoragePath(object entity)
        {
            if (entity.GetType() == typeof(Photo))
            {
                Photo p = (Photo)entity;
                string extension = GetContentType(entity).Split('/')[1];

                return Path.Combine(RootPhotosStoragePath, p.ID + "." + extension);
            }
            else if (entity.GetType() == typeof(ClrNamespace.CustomerBlob) || entity.GetType() == typeof(ClrNamespace.CustomerBlobWithBirthday))
            {
                ClrNamespace.Customer c = (ClrNamespace.Customer)entity;
                return Path.Combine(RootCustomerBlobStoragePath, c.ID + ".blob");
            }

            throw new NotSupportedException("Unsupported entity type: " + entity.GetType());
        }

        internal static string GetContentType(object entity)
        {
            if (entity.GetType() == typeof(Photo))
            {
                Photo p = (Photo)entity;
                switch (p.ID % 3)
                {
                    case 1:
                        return "image/jpeg";
                    case 2:
                        return "image/tiff";
                    default:
                        return "image/png";
                }
            }
            else if (entity.GetType() == typeof(ClrNamespace.CustomerBlob) || entity.GetType() == typeof(ClrNamespace.CustomerBlobWithBirthday))
            {
                return "CustomType/Custom/SubType";
            }

            throw new NotSupportedException("Unsupported entity type: " + entity.GetType());
        }

        private void ThrowIfDisposed()
        {
            if (this.isDisposed)
            {
                throw new InvalidOperationException("The DataServiceStreamProvider instance had already been disposed.");
            }
        }

        private void ValidateArguments(object entity, DataServiceOperationContext operationContext)
        {
            Type entityType = entity.GetType();
            if (entityType != typeof(Photo) && entityType != typeof(ClrNamespace.CustomerBlob) && entityType != typeof(ClrNamespace.CustomerBlobWithBirthday))
            {
                throw new DataServiceException(String.Format("Unsupported streaming type '{0}'", entity.GetType().FullName));
            }

            if (!operationContext.RequestMethod.Equals("DELETE", StringComparison.OrdinalIgnoreCase) && !operationContext.RequestMethod.Equals("GET", StringComparison.OrdinalIgnoreCase) && operationContext.RequestHeaders[HttpRequestHeader.ContentType] != GetContentType(entity))
            {
                throw new DataServiceException(String.Format("Incorrect Content-Type header value '{0}', expected '{1}'.", operationContext.RequestHeaders[HttpRequestHeader.ContentType], GetContentType(entity)));
            }
        }
    }

    public class PhotoContext : IUpdatable
    {
        #region Entity Sets

        internal static List<Item> _items;
        internal static List<Folder> _folders;

        internal static int NextItemID = 0;
        internal static int NextFolderID = 0;
        private Dictionary<int, object> tokens = new Dictionary<int, object>();


        public IQueryable<Item> Items
        {
            get { return _items.AsQueryable<Item>(); }
        }

        public IQueryable<Folder> Folders
        {
            get { return _folders.AsQueryable<Folder>(); }
        }

        #endregion Entity Sets

        #region Basic Plumbing

        private List<KeyValuePair<object, EntityState>> _pendingChanges;
        private static bool? preserveChanges;

        public PhotoContext()
        {
            if (preserveChanges == false || _items == null)
            {
                PopulateData();
            }
        }

        internal List<KeyValuePair<object, EntityState>> PendingChanges
        {
            get
            {
                if (_pendingChanges == null)
                {
                    _pendingChanges = new List<KeyValuePair<object, EntityState>>();
                }

                return _pendingChanges;
            }
        }

        internal static void PopulateData()
        {
            ClearData();

            // populate data here...
            Item i = new Item()
            {
                ID = NextItemID++,
                Description = "Default Item 0",
                Name = "Item 0",
                LastUpdated = DateTime.Now
            };
            _items.Add(i);

            Photo p = new Photo()
            {
                ID = NextItemID++,
                Description = "Default Photo 1",
                Name = "Photo 1",
                Rating = 3,
                LastUpdated = DateTime.Now
            };
            _items.Add(p);

            DataServiceStreamProvider.Init();

            using (Stream s = File.OpenWrite(DataServiceStreamProvider.GetStoragePath(p)))
            {
                byte[] buffer = new byte[] { 1, 2, 3, 4 };
                s.Write(buffer, 0, 4);
                s.Close();
            }

            Folder f = new Folder()
            {
                ID = NextFolderID++,
                Name = "Folder1"
            };
            f.Items.Add(i);
            f.Items.Add(p);
            _folders.Add(f);
        }

        internal static void ClearData()
        {
            _items = new List<Item>();
            _folders = new List<Folder>();

            NextItemID = 0;
            NextFolderID = 0;

            IOUtil.EnsureEmptyDirectoryExists(DataServiceStreamProvider.RootStoragePath);
        }

        private static IList GetEntitySet(Type entityType)
        {
            if (_items == null)
            {
                PopulateData();
            }

            if (typeof(Item).IsAssignableFrom(entityType))
            {
                return _items;
            }

            if (typeof(Folder).IsAssignableFrom(entityType))
            {
                return _folders;
            }

            throw new Exception("Unexpected EntityType encountered: " + entityType.FullName);
        }

        private static void AddResource(object resource, bool throwIfDuplicate)
        {
            IList entitySetInstance = GetEntitySet(resource.GetType());

            // check if there is not another instance with the same id
            object dup = TryGetEntity(entitySetInstance, resource);
            if (dup != null)
            {
                if (throwIfDuplicate)
                {
                    throw new DataServiceException(400, String.Format("Entity with the same key already present. EntityType: '{0}'",
                        resource.GetType().Name));
                }

                // if its already there, do not add it to the global context
                return;
            }

            entitySetInstance.Add(resource);
        }

        private void DeleteEntity(IEnumerable collection, object entity, bool throwIfNotPresent)
        {
            object entityToBeDeleted = TryGetEntity(collection, entity);

            if (entityToBeDeleted == null && throwIfNotPresent)
            {
                throw new Exception("No entity found with the given ID");
            }

            if (entityToBeDeleted != null)
            {
                // Make sure that property type implements ICollection<T> If yes, then call remove method on it to remove the
                // resource
                Type elementType = Utils.GetTypeParameter(collection.GetType(), typeof(ICollection<>), 0);
                typeof(ICollection<>).MakeGenericType(elementType).InvokeMember(
                                                "Remove",
                                                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                                                null,
                                                collection,
                                                new object[] { entityToBeDeleted });
            }
        }

        private static object TryGetEntity(IEnumerable collection, object entity)
        {
            object matchingEntity = null;

            foreach (object element in collection)
            {
                // check if there is not another instance with the same id
                if (Equal(element, entity))
                {
                    matchingEntity = element;
                    break;
                }
            }

            return matchingEntity;
        }

        private static bool Equal(object resource1, object resource2)
        {
            if (resource1.GetType() != resource2.GetType())
            {
                return false;
            }

            // check if there is not another instance with the same id
            return (bool)resource1.GetType().InvokeMember("Equals",
                                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod,
                                        null,
                                        resource1,
                                        new object[] { resource2 });
        }

        private object ResourceToToken(object resource)
        {
            int token = this.tokens.Count;
            this.tokens[token] = resource;
            return token;
        }

        private object TokenToResource(object token)
        {
            return this.tokens[(int)token];
        }

        private static bool IsPrimitiveType(Type type)
        {
            return (type.IsPrimitive ||
                    type == typeof(String) ||
                    type == typeof(Guid) ||
                    type == typeof(Decimal) ||
                    type == typeof(DateTimeOffset) ||
                    type == typeof(byte[]));
        }

        #endregion Basic Plumbing

        #region Change Scope

        public static IDisposable CreateChangeScope()
        {
            if (preserveChanges.HasValue && preserveChanges.Value)
            {
                throw new InvalidOperationException("Changes are already being preserved.");
            }

            PopulateData();
            preserveChanges = true;
            return new ChangeScope();
        }

        private class ChangeScope : IDisposable
        {
            public void Dispose()
            {
                preserveChanges = false;
                PopulateData();
            }
        }

        #endregion Change Scope

        #region IUpdatable Members

        void IUpdatable.AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            targetResource = this.TokenToResource(targetResource);
            resourceToBeAdded = this.TokenToResource(resourceToBeAdded);

            object propertyValue = targetResource.GetType().InvokeMember(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
                null,
                targetResource,
                null);

            propertyValue.GetType().InvokeMember(
                "Add",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                null,
                propertyValue,
                new object[] { resourceToBeAdded });
        }

        void IUpdatable.ClearChanges()
        {
            PendingChanges.Clear();
        }

        object IUpdatable.CreateResource(string containerName, string fullTypeName)
        {
            object resource = null;
            if (containerName == "Items" && fullTypeName == typeof(Item).FullName)
            {
                Item i = new Item();
                i.ID = NextFolderID++;
                resource = i;
            }
            else if (containerName == "Items" && fullTypeName == typeof(Photo).FullName)
            {
                Photo p = new Photo();
                p.ID = NextItemID++;
                resource = p;
            }
            else if (containerName == "Folders" && fullTypeName == typeof(Folder).FullName)
            {
                Folder f = new Folder();
                f.ID = NextFolderID++;
                resource = f;
            }

            if (resource == null)
            {
                throw new InvalidOperationException(String.Format("Invalid container name '{0}' or type name specified '{1}'", containerName, fullTypeName));
            }
            else
            {
                PendingChanges.Add(new KeyValuePair<object, EntityState>(resource, EntityState.Added));
            }

            return this.ResourceToToken(resource);
        }

        void IUpdatable.DeleteResource(object targetResource)
        {
            targetResource = this.TokenToResource(targetResource);
            PendingChanges.Add(new KeyValuePair<object, EntityState>(targetResource, EntityState.Deleted));
        }

        object IUpdatable.GetResource(IQueryable query, string fullTypeName)
        {
            object resource = null;

            foreach (object r in query)
            {
                if (resource != null)
                {
                    throw new ArgumentException(String.Format("Invalid Uri specified. The query '{0}' must refer to a single resource", query.ToString()));
                }

                resource = r;
            }

            if (resource != null && fullTypeName != null)
            {
                if (resource.GetType().FullName != fullTypeName)
                {
                    throw new ArgumentException(String.Format("Invalid uri specified. ExpectedType: '{0}', ActualType: '{1}'", fullTypeName, resource.GetType().FullName));
                }
            }

            return this.ResourceToToken(resource);
        }

        object IUpdatable.GetValue(object token, string propertyName)
        {
            object targetResource = this.TokenToResource(token);

            object propertyValue = targetResource.GetType().InvokeMember(
                                        propertyName,
                                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
                                        null,
                                        targetResource,
                                        null);

            if (propertyValue != null && !IsPrimitiveType(propertyValue.GetType()))
            {
                propertyValue = this.ResourceToToken(propertyValue);
            }

            return propertyValue;
        }

        void IUpdatable.RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            targetResource = this.TokenToResource(targetResource);
            resourceToBeRemoved = this.TokenToResource(resourceToBeRemoved);

            object propertyValue = targetResource.GetType().InvokeMember(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
                null,
                targetResource,
                null);

            propertyValue.GetType().InvokeMember(
                "Remove",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                null,
                propertyValue,
                new object[] { resourceToBeRemoved });
        }

        object IUpdatable.ResetResource(object token)
        {
            Debug.Assert(token != null, "token != null");
            object resource = this.TokenToResource(token);

            if (typeof(Item).IsAssignableFrom(resource.GetType()))
            {
                ((Item)resource).ReInit();
            }
            else if (typeof(Folder).IsAssignableFrom(resource.GetType()))
            {
                ((Folder)resource).ReInit();
            }
            else
            {
                throw new InvalidOperationException("Unsupported type '" + resource.GetType().FullName + "'.");
            }

            return token;
        }

        object IUpdatable.ResolveResource(object resource)
        {
            return this.TokenToResource(resource);
        }

        void IUpdatable.SaveChanges()
        {
            foreach (KeyValuePair<object, EntityState> pendingChange in this.PendingChanges)
            {
                // find the entity set for the object
                IList entitySetInstance = GetEntitySet(pendingChange.Key.GetType());

                switch (pendingChange.Value)
                {
                    case EntityState.Added:
                        AddResource(pendingChange.Key, true /*throwIfDuplicate*/);
                        break;
                    case EntityState.Deleted:
                        DeleteEntity(entitySetInstance, pendingChange.Key, true /*throwIfNotPresent*/);
                        if (typeof(Item).IsAssignableFrom(pendingChange.Key.GetType()))
                        {
                            foreach (Folder f in this.Folders)
                            {
                                DeleteEntity(f.Items, pendingChange.Key, false);
                            }
                        }
                        else if (typeof(Folder).IsAssignableFrom(pendingChange.Key.GetType()))
                        {
                            foreach (Folder f in this.Folders)
                            {
                                DeleteEntity(f.Folders, pendingChange.Key, false);
                            }
                        }
                        break;
                    default:
                        throw new Exception("Unsupported State");
                }
            }

            this.PendingChanges.Clear();
        }

        void IUpdatable.SetReference(object token, string propertyName, object propertyValueToken)
        {
            object targetResource = this.TokenToResource(token);
            object propertyValue = null;

            if (propertyValueToken != null)
            {
                propertyValue = this.TokenToResource(propertyValueToken);
            }

            PropertyInfo pi = targetResource.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);

            if (propertyValue != null && !pi.PropertyType.IsAssignableFrom(propertyValue.GetType()))
            {
                throw new DataServiceException(
                    400,
                    String.Format("Bad Request. The resource type '{0}' is not a valid type for the property '{1}' in resource '{2}'. Please make sure that the uri refers to the correct type",
                                  propertyValue.GetType().FullName, propertyName, targetResource.GetType().FullName));
            }

            pi.SetValue(targetResource, propertyValue, null);
        }

        void IUpdatable.SetValue(object token, string propertyName, object propertyValue)
        {
            object targetResource = this.TokenToResource(token);

            if (propertyValue != null && !IsPrimitiveType(propertyValue.GetType()))
            {
                propertyValue = this.TokenToResource(propertyValue);
            }

            PropertyInfo pi = targetResource.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);

            if (propertyValue != null && !pi.PropertyType.IsAssignableFrom(propertyValue.GetType()))
            {
                throw new DataServiceException(
                    400,
                    String.Format("Bad Request. The resource type '{0}' is not a valid type for the property '{1}' in resource '{2}'. Please make sure that the uri refers to the correct type",
                                  propertyValue.GetType().FullName, propertyName, targetResource.GetType().FullName));
            }

            pi.SetValue(targetResource, propertyValue, null);
        }

        #endregion
    }

    public class Item : IEquatable<Item>
    {
        public Item()
        {
            ReInit();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public Item Icon { get; set; }

        internal virtual void ReInit()
        {
            Name = null;
            Description = null;
            LastUpdated = DateTimeOffset.MinValue;
        }

        public bool Equals(Item other)
        {
            return this.ID == other.ID;
        }
    }

    [HasStream]
    public class Photo : Item, IEquatable<Photo>
    {
        public Photo()
        {
            ReInit();
        }

        public int Rating { get; set; }
        public byte[] ThumbNail { get; set; }

        internal override void ReInit()
        {
            base.ReInit();
            Rating = 0;
            ThumbNail = null;
        }

        #region IEquatable<Photo> Members

        public bool Equals(Photo other)
        {
            return this.ID == other.ID;
        }

        #endregion
    }

    public class Folder : IEquatable<Folder>
    {
        List<Item> _items;
        List<Folder> _folders;

        public Folder()
        {
            ReInit();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public List<Item> Items
        {
            get { return _items; }
        }
        public List<Folder> Folders
        {
            get { return _folders; }
        }

        internal void ReInit()
        {
            Name = null;
            _items = new List<Item>();
            _folders = new List<Folder>();
        }

        #region IEquatable<Folder> Members

        public bool Equals(Folder other)
        {
            return this.ID == other.ID;
        }

        #endregion
    }

    [MediaEntry("Content")]
    [MimeTypeProperty("Content", "ContentType")]
    public class PhotoMLE : Item
    {
        public int Rating { get; set; }
        public byte[] ThumbNail { get; set; }
        public byte[] Content { set; get; }
        public string ContentType
        {
            set { }
            get { return DataServiceStreamProvider.GetContentType(new Photo { ID = this.ID }); }
        }
    }

    #endregion BLOB Server
}
//---------------------------------------------------------------------
// <copyright file="DataServiceStreamProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs
{
    using Microsoft.OData.Service.Providers;
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using System.IO;
    using System.Diagnostics;
    using System.Threading;
    using System.Data.Test.Astoria;
    using System.Text;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Net;

    public class DataServiceStreamProvider : IDataServiceStreamProvider, IDisposable
    {
        public static string RootStoragePath = Path.Combine(TestUtil.GeneratedFilesLocation, typeof(DataServiceStreamProvider).Name);
        public static string RootPhotosStoragePath = Path.Combine(RootStoragePath, "Photos");
        public static string RootCustomersStoragePath = Path.Combine(RootStoragePath, "Customers");
        public static int UnDisposedInstances = 0;
        public static int InstantiatedCount = 0;
        public static int DefaultStreamBufferSize = 4000;
        public static Dictionary<string, LargeStream> LargeStreamStorage;
        public static StorageMode ProviderStorageMode = StorageMode.Disk;
        public static Func<object, DataServiceOperationContext, string> GetStreamContentTypeOverride = null;
        public static Func<object, DataServiceOperationContext, Uri> GetReadStreamUriOverride = null;
        public static Func<object, DataServiceOperationContext, string> GetStreamETagOverride = null;
        public static Func<object, DataServiceOperationContext, string> GetPostWriteStreamETagOverride = null;
        public static Func<string, DataServiceOperationContext, string> ResolveTypeOverride = null;
        public static bool SkipValidation = false;
        public static bool ThrowDataServiceException304 = false;
        public static string CallOrderLog;

        private const bool tracesNeedFileInfo = false;
        private int instanceId;
        private bool isDisposed;
        private static bool GetWriteStreamCalled;

        public enum StorageMode
        {
            Disk,
            LargeStream
        }

        public static void SetupLargeStreamStorage()
        {
            LargeStreamStorage = new Dictionary<string, LargeStream>();
            ProviderStorageMode = StorageMode.LargeStream;
        }

        public static LargeStream GetLargeStream(object entity)
        {
            return LargeStreamStorage[GetStoragePath(entity)];
        }

        public DataServiceStreamProvider()
        {
            Init();
            this.instanceId = GenerateInstanceId();
            PrefixStreamEventNode(
                new StreamEventNode()
                {
                    InstanceId = this.instanceId,
                    IsCreation = true,
                    StackTraceText = new StackTrace(tracesNeedFileInfo).ToString()
                });
            Interlocked.Increment(ref UnDisposedInstances);
            Interlocked.Increment(ref InstantiatedCount);
            Trace.WriteLine("created object #" + this.instanceId + ", for " + UnDisposedInstances);
            CallOrderLog += "[DataServiceStreamProvider";
            GetWriteStreamCalled = false;
        }

        class StreamEventNode
        {
            public int InstanceId;
            public bool IsCreation;
            public string StackTraceText;
            public StreamEventNode Next;
            public int ThreadId;
        }

        private static int instanceIdGenerator;
        private static StreamEventNode root;

        public static int GenerateInstanceId()
        {
            return Interlocked.Increment(ref instanceIdGenerator);
        }

        private static void PrefixStreamEventNode(StreamEventNode head)
        {
            head.ThreadId = Thread.CurrentThread.ManagedThreadId;
            while (true)
            {
                StreamEventNode n = root;
                head.Next = n;
                if (Interlocked.CompareExchange(ref root, head, n) == n)
                {
                    return;
                }
            }
        }

        public static string ConsumeTraceDescription()
        {
            StringBuilder result = new StringBuilder();
            StreamEventNode n = root;
            root = null;
            while (n != null)
            {
                result.AppendLine(
                    "object #" + n.InstanceId +
                    " was " + (n.IsCreation ? "created " : "disposed") +
                    " on thread #" + n.ThreadId + " at:\r\n" + n.StackTraceText);
                n = n.Next;
            }

            return result.ToString();
        }

        public static void Init()
        {
            IOUtil.EnsureDirectoryExists(RootStoragePath);
            IOUtil.EnsureDirectoryExists(RootPhotosStoragePath);
            IOUtil.EnsureDirectoryExists(RootCustomersStoragePath);
        }

        #region IDataServiceStreamProvider Members

        public int StreamBufferSize
        {
            get
            {
                this.ThrowIfDisposed();
                CallOrderLog += "-StreamBufferSize";
                return DefaultStreamBufferSize;
            }
        }

        public Stream GetReadStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            this.ThrowIfDisposed();
            CallOrderLog += "-GetReadStream";
            bool returnEmptyStream;
            CheckETag(entity, etag, checkETagForEquality, operationContext, out returnEmptyStream);

            ValidateArguments(entity, operationContext);
            SetCustomResponseHeaders(operationContext);

            if (returnEmptyStream)
            {
                return new MemoryStream(new byte[0]);
            }

            Stream s;
            switch (DataServiceStreamProvider.ProviderStorageMode)
            {
                case StorageMode.Disk:
                    s = File.OpenRead(GetStoragePath(entity));
                    break;
                case StorageMode.LargeStream:
                    s = GetLargeStream(entity);
                    ((LargeStream)s).ReOpen();
                    break;
                default:
                    throw new InvalidOperationException("Unknown StorageMode!");
            }

            return s;
        }

        public Stream GetWriteStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            this.ThrowIfDisposed();
            CallOrderLog += "-GetWriteStream";
            bool returnEmptyStream;
            if (!operationContext.IsBatchRequest || !operationContext.AbsoluteServiceUri.MakeRelativeUri(operationContext.AbsoluteRequestUri).OriginalString.StartsWith("$"))
            {
                CheckETag(entity, etag, checkETagForEquality, operationContext, out returnEmptyStream);
            }

            string slug = operationContext.RequestHeaders["Slug"];
            if (entity.GetType() == typeof(Photo))
            {
                Photo p = (Photo)entity;

                if (operationContext.RequestMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(slug))
                {
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
            }
            else if (entity.GetType() == typeof(NorthwindModel.Customers))
            {
                NorthwindModel.Customers c = (NorthwindModel.Customers)entity;

                if (operationContext.RequestMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
                {
                    Random rand = TestUtil.Random;
                    c.CustomerID = "C" + rand.Next(1000, 9999).ToString();
                    c.CompanyName = "Microsoft";

                    if (!string.IsNullOrEmpty(slug))
                    {
                        c.CustomerID = slug;
                    }
                }
            }
            else if (entity.GetType() == typeof(NorthwindModel.Orders))
            {
                NorthwindModel.Orders o = (NorthwindModel.Orders)entity;
                if (operationContext.RequestMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
                {
                    Random rand = TestUtil.Random;
                    o.OrderID = rand.Next(1000, 9999);
                }
            }
            else if (entity.GetType() == typeof(RowEntityTypeWithIDAsKey) && (((RowEntityTypeWithIDAsKey)entity).TypeName == CustomRowBasedContext.CustomerFullName || ((RowEntityTypeWithIDAsKey)entity).TypeName == CustomRowBasedContext.CustomerWithBirthdayFullName))
            {
                int id;
                if (int.TryParse(slug, out id))
                {
                    ((RowEntityTypeWithIDAsKey)entity).ID = id;
                }
                else
                {
                    ((RowEntityTypeWithIDAsKey)entity).ID = TestUtil.Random.Next(1000, 9999);
                }
            }
            else if (entity.GetType() == typeof(RowComplexType) && (((RowComplexType)entity).TypeName == "AstoriaUnitTests.Stubs.Customer" || ((RowComplexType)entity).TypeName == "AstoriaUnitTests.Stubs.CustomerWithBirthday"))
            {
                int id;
                if (int.TryParse(slug, out id))
                {
                    ((RowComplexType)entity).Properties["ID"] = id;
                }
                else
                {
                    ((RowComplexType)entity).Properties["ID"] = TestUtil.Random.Next(1000, 9999);
                }
            }

            ValidateArguments(entity, operationContext);
            SetCustomResponseHeaders(operationContext);

            Stream s;
            switch (DataServiceStreamProvider.ProviderStorageMode)
            {
                case StorageMode.Disk:
                    s = File.Open(GetStoragePath(entity), FileMode.Create, FileAccess.Write);
                    break;
                case StorageMode.LargeStream:
                    s = new LargeStream();
                    LargeStreamStorage[GetStoragePath(entity)] = (LargeStream)s;
                    break;
                default:
                    throw new InvalidOperationException("Unknown StorageMode!");
            }

            GetWriteStreamCalled = true;
            return s;
        }

        public void DeleteStream(object entity, DataServiceOperationContext operationContext)
        {
            this.ThrowIfDisposed();
            CallOrderLog += "-DeleteStream";
            ValidateArguments(entity, operationContext);
            SetCustomResponseHeaders(operationContext);

            if (entity.GetType() == typeof(Photo))
            {
                Photo p = (Photo)entity;
            }

            switch (DataServiceStreamProvider.ProviderStorageMode)
            {
                case StorageMode.Disk:
                    File.Delete(GetStoragePath(entity));
                    break;
                case StorageMode.LargeStream:
                    LargeStreamStorage.Remove(GetStoragePath(entity));
                    break;
                default:
                    throw new InvalidOperationException("Unknown StorageMode!");
            }
        }

        public string GetStreamContentType(object entity, DataServiceOperationContext operationContext)
        {
            this.ThrowIfDisposed();
            CallOrderLog += "-GetStreamContentType";
            ValidateArguments(entity, operationContext);
            if (DataServiceStreamProvider.GetStreamContentTypeOverride != null)
            {
                return DataServiceStreamProvider.GetStreamContentTypeOverride(entity, operationContext);
            }
            else
            {
                return GetContentType(entity);
            }
        }

        public Uri GetReadStreamUri(object entity, DataServiceOperationContext operationContext)
        {
            this.ThrowIfDisposed();
            CallOrderLog += "-GetReadStreamUri";
            ValidateArguments(entity, operationContext);
            if (DataServiceStreamProvider.GetReadStreamUriOverride != null)
            {
                return DataServiceStreamProvider.GetReadStreamUriOverride(entity, operationContext);
            }
            else
            {
                return GetReadStreamUri(entity);
            }
        }

        public string GetStreamETag(object entity, DataServiceOperationContext operationContext)
        {
            this.ThrowIfDisposed();
            CallOrderLog += "-GetStreamETag";
            ValidateArguments(entity, operationContext);
            return this.InternalGetStreamETag(entity, operationContext);
        }

        private string InternalGetStreamETag(object entity, DataServiceOperationContext operationContext)
        {
            if (DataServiceStreamProvider.GetPostWriteStreamETagOverride != null && GetWriteStreamCalled == true)
            {
                return DataServiceStreamProvider.GetPostWriteStreamETagOverride(entity, operationContext);
            }
            else if (DataServiceStreamProvider.GetStreamETagOverride != null)
            {
                return DataServiceStreamProvider.GetStreamETagOverride(entity, operationContext);
            }

            return null;
        }

        public string ResolveType(string entitySetName, DataServiceOperationContext operationContext)
        {
            // This code is required by a test verifying that we can call GetQueryStringItem in an stream provider method.
            var forceErrorValue = operationContext.GetQueryStringValue("Query-String-Header-Force-Error");
            if (forceErrorValue == "yes")
            {
                throw new DataServiceException(418, "User code threw a Query-String-Header-Force-Error exception.");
            }

            this.ThrowIfDisposed();
            CallOrderLog += "-ResolveType";
            if (DataServiceStreamProvider.ResolveTypeOverride != null)
            {
                return DataServiceStreamProvider.ResolveTypeOverride(entitySetName, operationContext);
            }

            return operationContext.RequestHeaders["CustomRequestHeader_ItemType"];
        }

        public void Dispose()
        {
            CallOrderLog += "-Dispose]";
            this.isDisposed = true;
            Interlocked.Decrement(ref UnDisposedInstances);
            Trace.WriteLine("released obj.  #" + this.instanceId + ", for" + UnDisposedInstances);
            PrefixStreamEventNode(
                new StreamEventNode()
                {
                    InstanceId = this.instanceId,
                    IsCreation = false,
                    StackTraceText = new StackTrace(tracesNeedFileInfo).ToString()
                });
            GetWriteStreamCalled = false;
        }

        #endregion

        public static Uri GetReadStreamUri(object entity)
        {
            if (entity.GetType() == typeof(Photo))
            {
                Photo p = (Photo)entity;
                switch (p.ID % 2)
                {
                    case 1:
                        return new Uri("http://localhost/someuri/", UriKind.Absolute);
                    default:
                        return null;
                }
            }

            return null;
        }

        public static string GetStoragePath(object entity)
        {
            if (typeof(Photo).IsAssignableFrom(entity.GetType()))
            {
                Photo p = (Photo)entity;
                string extension = GetContentType(entity).Split('/')[1];

                return Path.Combine(RootPhotosStoragePath, p.ID + "." + extension);
            }
            else if (entity.GetType() == typeof(NorthwindModel.Customers))
            {
                NorthwindModel.Customers c = (NorthwindModel.Customers)entity;
                return Path.Combine(RootCustomersStoragePath, c.CustomerID + ".blob");
            }
            else if (entity.GetType() == typeof(RowEntityTypeWithIDAsKey) && (((RowEntityTypeWithIDAsKey)entity).TypeName == CustomRowBasedContext.CustomerFullName || ((RowEntityTypeWithIDAsKey)entity).TypeName == CustomRowBasedContext.CustomerWithBirthdayFullName))
            {
                int id = ((RowEntityTypeWithIDAsKey)entity).ID;
                return Path.Combine(RootCustomersStoragePath, id + ".blob");
            }
            else if (entity.GetType() == typeof(RowComplexType) && (((RowComplexType)entity).TypeName == "AstoriaUnitTests.Stubs.Customer" || ((RowComplexType)entity).TypeName == "AstoriaUnitTests.Stubs.CustomerWithBirthday"))
            {
                int id = (int)((RowComplexType)entity).Properties["ID"];
                return Path.Combine(RootCustomersStoragePath, id + ".blob");
            }

            throw new NotSupportedException("Unsupported entity type: " + entity.GetType());
        }

        public static string GetContentType(object entity)
        {
            if (typeof(Photo).IsAssignableFrom(entity.GetType()))
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
            else if (entity.GetType() == typeof(NorthwindModel.Customers) || entity.GetType() == typeof(NorthwindModel.Orders) ||
                (entity.GetType() == typeof(RowEntityTypeWithIDAsKey) && (((RowEntityTypeWithIDAsKey)entity).TypeName == CustomRowBasedContext.CustomerFullName || ((RowEntityTypeWithIDAsKey)entity).TypeName == CustomRowBasedContext.CustomerWithBirthdayFullName)) ||
                (entity.GetType() == typeof(RowComplexType) && (((RowComplexType)entity).TypeName == "AstoriaUnitTests.Stubs.Customer" || ((RowComplexType)entity).TypeName == "AstoriaUnitTests.Stubs.CustomerWithBirthday")))
            {
                return "CustomType/CustomSubType";
            }

            throw new NotSupportedException("Unsupported entity type: " + entity.GetType());
        }

        public static void ValidateInstantiatedInstances()
        {
            int count = InstantiatedCount;
            InstantiatedCount = 0;
            Assert.IsTrue(count == 0 || count == 1, "InstantiatedCount = '{0}', expected 0 or 1.", count);
        }

        /// <summary>
        /// Returns false if the given etag value is not valid.
        /// Look in http://www.ietf.org/rfc/rfc2616.txt?number=2616 (Section 14.26) for more information
        /// </summary>
        /// <param name="etag">etag value to be checked.</param>
        /// <returns>returns true if the etag value is valid, otherwise returns false.</returns>
        private static bool IsETagValueValid(string etag)
        {
            if (String.IsNullOrEmpty(etag) || etag == "*")
            {
                return true;
            }

            // HTTP RFC 2616, section 3.11:
            //   entity-tag = [ weak ] opaque-tag
            //   weak       = "W/"
            //   opaque-tag = quoted-string
            if (etag.Length <= 4 || (!etag.StartsWith("W/") && etag[0] != '"') || etag[etag.Length - 1] != '"')
            {
                return false;
            }

            for (int i = 3; i < etag.Length - 1; i++)
            {
                // Format of etag looks something like: W/"etag property values"
                // according to HTTP RFC 2616, if someone wants to specify more than 1 etag value,
                // then need to specify something like this: W/"etag values", W/"etag values", ...
                // To make sure only one etag is specified, we need to ensure that
                // only the third and last characters are quotes.
                // If " is part of the key value, it needs to be escaped.
                if (etag[i] == '"')
                {
                    return false;
                }
            }

            return true;
        }

        private void CheckETag(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext, out bool returnEmptyStream)
        {
            returnEmptyStream = false;

            // We do not check etag for POST
            if (operationContext.RequestMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string currentEtag = this.InternalGetStreamETag(entity, operationContext);
            if (!IsETagValueValid(currentEtag))
            {
                // we are doing negative testing on the GetStreamETag API
                return;
            }

            if (!operationContext.RequestMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(currentEtag) && !string.IsNullOrEmpty(etag))
                {
                    throw new DataServiceException(900, "If-Match or If-None-Match headers cannot be specified if the target media resource does not have an etag defined.");
                }

                if (!string.IsNullOrEmpty(currentEtag) && string.IsNullOrEmpty(etag))
                {
                    throw new DataServiceException(900, "Since the target media resource has an etag defined, If-Match HTTP header must be specified for DELETE/PUT operations on it.");
                }
            }

            if (!string.IsNullOrEmpty(etag) && checkETagForEquality.HasValue)
            {
                if (checkETagForEquality.Value == true && currentEtag != etag)
                {
                    throw new DataServiceException(912, "If-Match precondition failed for target media resource.");
                }
                else if (checkETagForEquality.Value == false && currentEtag == etag)
                {
                    if (operationContext.RequestMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
                    {
                        if (ThrowDataServiceException304)
                        {
                            throw new DataServiceException(304, "No Change, Thrown by BlobSupportTest.");
                        }
                        else
                        {
                            returnEmptyStream = true;
                            operationContext.ResponseStatusCode = 304;
                        }
                    }
                    else if (operationContext.RequestMethod.Equals("PUT", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new DataServiceException(900, "If-None-Match HTTP header cannot be specified for PUT operations.");
                    }
                    else if (operationContext.RequestMethod.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new DataServiceException(900, "If-None-Match HTTP header cannot be specified for DELETE operations.");
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }
            }
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
            if (!SkipValidation)
            {
                Type projectedWrapperType = typeof(DataService<>).Assembly.GetType("Microsoft.OData.Service.Internal.ProjectedWrapper");

                if (projectedWrapperType.IsAssignableFrom(entity.GetType()))
                {
                    throw new DataServiceException("Expected ResourceType but received ProjectedWrapper");
                }

                Type entityType = entity.GetType();
                if (!typeof(Photo).IsAssignableFrom(entityType) && entityType != typeof(NorthwindModel.Customers) && entityType != typeof(NorthwindModel.Orders) &&
                    (entityType != typeof(RowEntityTypeWithIDAsKey) || (((RowEntityTypeWithIDAsKey)entity).TypeName != CustomRowBasedContext.CustomerFullName && ((RowEntityTypeWithIDAsKey)entity).TypeName != CustomRowBasedContext.CustomerWithBirthdayFullName)) &&
                    (entityType != typeof(RowComplexType) || (((RowComplexType)entity).TypeName != "AstoriaUnitTests.Stubs.Customer" && ((RowComplexType)entity).TypeName != "AstoriaUnitTests.Stubs.CustomerWithBirthday")))
                {
                    throw new DataServiceException(String.Format("Unsupported streaming type '{0}'", entity.GetType().FullName));
                }

                // For PUT to $value or POST requests, the content type of the MR should match
                if (((operationContext.AbsoluteRequestUri.OriginalString.Contains("$value") && operationContext.RequestMethod.Equals("PUT", StringComparison.OrdinalIgnoreCase)) ||
                      operationContext.RequestMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
                    && operationContext.RequestHeaders[HttpRequestHeader.ContentType] != GetContentType(entity))
                {
                    throw new DataServiceException(String.Format("Incorrect Content-Type header value '{0}', expected '{1}'.", operationContext.RequestHeaders["Content-Type"], GetContentType(entity)));
                }

                if (!ValidateCustomRequestHeaders(operationContext))
                {
                    throw new DataServiceException("Expected custom request headers not found!");
                }
            }
        }

        private static bool ValidateCustomRequestHeaders(DataServiceOperationContext operationContext)
        {
            string customHeader1 = operationContext.RequestHeaders["CustomRequestHeader1"];
            string customHeader2 = operationContext.RequestHeaders["CustomRequestHeader2"];
            if (customHeader1 == "CustomRequestHeaderValue1" && customHeader2 == "CustomRequestHeaderValue2")
            {
                return true;
            }

            return false;
        }

        private void SetCustomResponseHeaders(DataServiceOperationContext operationContext)
        {
            operationContext.ResponseHeaders["CustomResponseHeader1"] = "CustomResponseHeaderValue1";
            operationContext.ResponseHeaders["CustomResponseHeader2"] = "CustomResponseHeaderValue2";

            bool seenHeader1 = false;
            bool seenHeader2 = false;

            foreach (string responseHeader in operationContext.ResponseHeaders.AllKeys)
            {
                if (responseHeader == "CustomResponseHeader1")
                {
                    seenHeader1 = true;
                    if (operationContext.ResponseHeaders[responseHeader] != "CustomResponseHeaderValue1")
                    {
                        throw new DataServiceException("Invalid value for CustomResponseHeader1.");
                    }
                }
                else if (responseHeader == "CustomResponseHeader2")
                {
                    seenHeader2 = true;
                    if (operationContext.ResponseHeaders[responseHeader] != "CustomResponseHeaderValue2")
                    {
                        throw new DataServiceException("Invalid value for CustomResponseHeader2.");
                    }
                }
            }

            if (!seenHeader1 || !seenHeader2)
            {
                throw new DataServiceException("CustomResponseHeader1 or CustomResponseHeader2 is not set.");
            }
        }

        public static bool ValidateCustomResponseHeaders(TestWebRequest request)
        {
            if (request.ResponseHeaders.ContainsKey("CustomResponseHeader1") &&
               request.ResponseHeaders["CustomResponseHeader1"] == "CustomResponseHeaderValue1" &&
               request.ResponseHeaders.ContainsKey("CustomResponseHeader2") &&
               request.ResponseHeaders["CustomResponseHeader2"] == "CustomResponseHeaderValue2")
            {
                return true;
            }

            return false;
        }
    }
}
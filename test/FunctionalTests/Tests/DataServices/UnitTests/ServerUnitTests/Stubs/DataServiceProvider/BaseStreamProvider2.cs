//---------------------------------------------------------------------
// <copyright file="BaseStreamProvider2.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using AstoriaUnitTests.Stubs.DataServiceProvider;

    /// <summary>
    /// Stream Provider Implementation
    /// </summary>
    public abstract class BaseStreamProvider2 : IDataServiceStreamProvider2, IDisposable
    {
        #region Statics and Consts

        /// <summary>
        /// Default buffer size
        /// </summary>
        public static int DefaultBufferSize = 64 * 1024;

        /// <summary>
        /// Custom http header to use for ResolveType().
        /// </summary>
        public static string ResolveTypeHeaderName = "CustomRequestHeader_ItemType";

        /// <summary>
        /// Keep track and make sure dispose() is called once on each instance of the stream provider.
        /// </summary>
        private static int InstanceCount;

        #endregion Statics and Consts

        #region Private members

        /// <summary>
        /// In memory storage for streams.
        /// </summary>
        private DSPMediaResourceStorage streamStorage;

        /// <summary>
        /// True if dispose is called
        /// </summary>
        private bool isDisposed;

        #endregion Private members

        /// <summary>
        /// Constructs an instance of the stream provider.
        /// </summary>
        /// <param name="streamStorage">Storage for the streams</param>
        public BaseStreamProvider2(DSPMediaResourceStorage streamStorage)
        {
            CheckArgumentNull(streamStorage, "streamStorage");

            this.streamStorage = streamStorage;
            if (InstanceCount != 0)
            {
                throw new InvalidOperationException("There are '" + InstanceCount + "' undisposed instance(s).");
            }

            InstanceCount++;
        }

        #region IDataServiceStreamProvider2 interface

        #region GetReadStream

        private Stream GetReadStreamInternal(object entity, ResourceProperty streamProperty, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            this.ThrowIfDisposed();

            CheckArgumentNull(entity, "entity");
            CheckArgumentNull(operationContext, "operationContext");

            ValidateEntity(entity, streamProperty);
            DSPMediaResource mediaResource;
            if (!this.streamStorage.TryGetMediaResource(entity, streamProperty, out mediaResource))
            {
                return null;
            }

            ValidateETag(etag, checkETagForEquality, operationContext, mediaResource);

            if (operationContext.RequestMethod != "GET")
            {
                throw new InvalidOperationException("Unsupported http method '" + operationContext.RequestMethod + "'.");
            }

            bool isEmptyStream;
            Stream readStream = mediaResource.GetReadStream(out isEmptyStream);
            if (isEmptyStream)
            {
                operationContext.ResponseStatusCode = 204;
            }

            return readStream;
        }

        public Stream GetReadStream(object entity, ResourceProperty streamProperty, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            CheckArgumentNull(streamProperty, "streamProperty");
            return this.GetReadStreamInternal(entity, streamProperty, etag, checkETagForEquality, operationContext);
        }

        public Stream GetReadStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            return this.GetReadStreamInternal(entity, null /*null for default stream*/, etag, checkETagForEquality, operationContext);
        }

        #endregion GetReadStream

        #region GetReadStreamUri

        private Uri GetReadStreamUriInternal(object entity, ResourceProperty streamProperty, DataServiceOperationContext operationContext)
        {
            this.ThrowIfDisposed();

            CheckArgumentNull(entity, "entity");
            CheckArgumentNull(operationContext, "operationContext");

            ValidateEntity(entity, streamProperty);
            DSPMediaResource mediaResource;
            if (this.streamStorage.TryGetMediaResource(entity, streamProperty, out mediaResource))
            {
                return mediaResource.ReadStreamUri;
            }

            return null;
        }

        public Uri GetReadStreamUri(object entity, ResourceProperty streamProperty, DataServiceOperationContext operationContext)
        {
            CheckArgumentNull(streamProperty, "streamProperty");
            return this.GetReadStreamUriInternal(entity, streamProperty, operationContext);
        }

        public Uri GetReadStreamUri(object entity, DataServiceOperationContext operationContext)
        {
            return this.GetReadStreamUriInternal(entity, null /*null for default stream*/, operationContext);
        }

        #endregion GetReadStreamUri

        #region GetStreamContentType

        private string GetStreamContentTypeInternal(object entity, ResourceProperty streamProperty, DataServiceOperationContext operationContext)
        {
            this.ThrowIfDisposed();

            CheckArgumentNull(entity, "entity");
            CheckArgumentNull(operationContext, "operationContext");

            ValidateEntity(entity, streamProperty);
            DSPMediaResource mediaResource;
            if (this.streamStorage.TryGetMediaResource(entity, streamProperty, out mediaResource))
            {
                return mediaResource.ContentType;
            }

            return null;
        }

        public string GetStreamContentType(object entity, ResourceProperty streamProperty, DataServiceOperationContext operationContext)
        {
            CheckArgumentNull(streamProperty, "streamProperty");
            return this.GetStreamContentTypeInternal(entity, streamProperty, operationContext);
        }

        public string GetStreamContentType(object entity, DataServiceOperationContext operationContext)
        {
            return this.GetStreamContentTypeInternal(entity, null /*null for default stream*/, operationContext);
        }

        #endregion GetStreamContentType

        #region GetStreamETag

        private string GetStreamETagInternal(object entity, ResourceProperty streamProperty, DataServiceOperationContext operationContext)
        {
            this.ThrowIfDisposed();

            CheckArgumentNull(entity, "entity");
            CheckArgumentNull(operationContext, "operationContext");

            ValidateEntity(entity, streamProperty);
            DSPMediaResource mediaResource;
            if (this.streamStorage.TryGetMediaResource(entity, streamProperty, out mediaResource))
            {
                return mediaResource.Etag;
            }

            return null;
        }

        public string GetStreamETag(object entity, ResourceProperty streamProperty, DataServiceOperationContext operationContext)
        {
            CheckArgumentNull(streamProperty, "streamProperty");
            return this.GetStreamETagInternal(entity, streamProperty, operationContext);
        }

        public string GetStreamETag(object entity, DataServiceOperationContext operationContext)
        {
            return this.GetStreamETagInternal(entity, null /*null for default stream*/, operationContext);
        }

        #endregion GetStreamETag

        #region GetWriteStream

        private Stream GetWriteStreamInternal(object entity, ResourceProperty streamProperty, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            this.ThrowIfDisposed();

            CheckArgumentNull(entity, "entity");
            CheckArgumentNull(operationContext, "operationContext");

            ValidateEntity(entity, streamProperty);
            DSPMediaResource mediaResource;
            if (!this.streamStorage.TryGetMediaResource(entity, streamProperty, out mediaResource))
            {
                mediaResource = this.streamStorage.CreateMediaResource(entity, streamProperty);
            }

            ValidateETag(etag, checkETagForEquality, operationContext, mediaResource);

            if (operationContext.RequestMethod == "POST")
            {
                if (streamProperty != null)
                {
                    throw new InvalidOperationException("POST is allowed only for default streams.");
                }

                string slug = operationContext.RequestHeaders["Slug"];
                if (!string.IsNullOrEmpty(slug))
                {
                    this.ApplySlugHeader(slug, entity);
                }
            }
            else if (operationContext.RequestMethod != "PUT")
            {
                throw new InvalidOperationException("Unsupported http method '" + operationContext.RequestMethod + "'.");
            }

            mediaResource.ContentType = operationContext.RequestHeaders["Content-Type"];
            if (string.IsNullOrEmpty(mediaResource.ContentType))
            {
                throw new InvalidOperationException("Content-Type header missing.");
            }

            mediaResource.Etag = DSPMediaResource.GenerateStreamETag();
            return mediaResource.GetWriteStream();
        }

        public Stream GetWriteStream(object entity, ResourceProperty streamProperty, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            CheckArgumentNull(streamProperty, "streamProperty");
            return this.GetWriteStreamInternal(entity, streamProperty, etag, checkETagForEquality, operationContext);
        }

        public Stream GetWriteStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext)
        {
            return this.GetWriteStreamInternal(entity, null /*null for default stream*/, etag, checkETagForEquality, operationContext);
        }

        #endregion GetWriteStream

        public void DeleteStream(object entity, DataServiceOperationContext operationContext)
        {
            this.ThrowIfDisposed();

            CheckArgumentNull(entity, "entity");
            CheckArgumentNull(operationContext, "operationContext");

            ValidateEntity(entity);
            this.DeleteStreams(entity);
        }

        public string ResolveType(string entitySetName, DataServiceOperationContext operationContext)
        {
            CheckStringArgumentNull(entitySetName, "entitySetName");
            CheckArgumentNull(operationContext, "operationContext");

            this.ThrowIfDisposed();
            return this.ResolveTypeInternal(entitySetName, operationContext);
        }

        public int StreamBufferSize
        {
            get
            {
                this.ThrowIfDisposed();
                return DefaultBufferSize;
            }
        }

        #endregion IDataServiceStreamProvider2 interface

        public void Dispose()
        {
            this.ThrowIfDisposed();
            this.isDisposed = true;
            InstanceCount--;
        }

        #region Protected Methods

        /// <summary>Converts a string to a primitive value.</summary>
        /// <param name="text">String text to convert.</param>
        /// <param name="targetType">Type to convert string to.</param>
        /// <returns>value converted to the target type.</returns>
        protected static object StringToPrimitive(string text, Type targetType)
        {
            Debug.Assert(text != null, "text != null");
            Debug.Assert(targetType != null, "targetType != null");

            object targetValue = null;
            targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (typeof(String) == targetType)
            {
                targetValue = text;
            }
            else if (typeof(Boolean) == targetType)
            {
                targetValue = XmlConvert.ToBoolean(text);
            }
            else if (typeof(Byte) == targetType)
            {
                targetValue = XmlConvert.ToByte(text);
            }
            else if (typeof(byte[]) == targetType)
            {
                targetValue = Convert.FromBase64String(text);
            }
            else if (typeof(System.Data.Linq.Binary) == targetType)
            {
                targetValue = new System.Data.Linq.Binary(Convert.FromBase64String(text));
            }
            else if (typeof(SByte) == targetType)
            {
                targetValue = XmlConvert.ToSByte(text);
            }
            else if (typeof(DateTime) == targetType)
            {
                targetValue = XmlConvert.ToDateTime(text, XmlDateTimeSerializationMode.RoundtripKind);
            }
            else if (typeof(Decimal) == targetType)
            {
                targetValue = XmlConvert.ToDecimal(text);
            }
            else if (typeof(Double) == targetType)
            {
                targetValue = XmlConvert.ToDouble(text);
            }
            else if (typeof(Guid) == targetType)
            {
                targetValue = new Guid(text);
            }
            else if (typeof(Int16) == targetType)
            {
                targetValue = XmlConvert.ToInt16(text);
            }
            else if (typeof(Int32) == targetType)
            {
                targetValue = XmlConvert.ToInt32(text);
            }
            else if (typeof(Int64) == targetType)
            {
                targetValue = XmlConvert.ToInt64(text);
            }
            else if (typeof(System.Xml.Linq.XElement) == targetType)
            {
                targetValue = System.Xml.Linq.XElement.Parse(text, System.Xml.Linq.LoadOptions.PreserveWhitespace);
            }
            else
            {
                Debug.Assert(typeof(Single) == targetType, "typeof(Single) == targetType(" + targetType + ")");
                targetValue = XmlConvert.ToSingle(text);
            }

            return targetValue;
        }

        /// <summary>Checks whether the given property is a key property.</summary>
        /// <param name="property">property to check</param>
        /// <returns>true if this is a key property, else returns false</returns>
        protected static bool IsPropertyKeyProperty(PropertyInfo property)
        {
            // Only primitive types are allowed to be keys.
            // Checks for generic to exclude Nullable<> value-type primitives, since we don't allows keys to be null.
            if (property.PropertyType.IsPrimitive && !property.PropertyType.IsGenericType)
            {
                KeyAttribute keyAttribute = property.ReflectedType.GetCustomAttributes(true).OfType<KeyAttribute>().FirstOrDefault();
                if (keyAttribute != null && keyAttribute.KeyNames.Contains(property.Name))
                {
                    return true;
                }

                // Check for {TypeName}ID or Id
                if (property.Name == property.DeclaringType.Name + "ID")
                {
                    return true;
                }
                else if (property.Name == "ID")
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Validates the entity.
        /// </summary>
        /// <param name="entity">entity instance.</param>
        protected abstract void ValidateEntity(object entity);

        /// <summary>
        /// Validates the entity and stream parameters.
        /// </summary>
        /// <param name="entity">entity instance.</param>
        /// <param name="streamProperty">stream info</param>
        protected abstract void ValidateEntity(object entity, Microsoft.OData.Service.Providers.ResourceProperty streamProperty);

        /// <summary>
        /// Applies the slug header value to the entity instance
        /// </summary>
        /// <param name="slug">slug header value</param>
        /// <param name="entity">entity instance to apply the slug value</param>
        protected abstract void ApplySlugHeader(string slug, object entity);

        /// <summary>
        /// Resolves the entity type for a POST MR operation.
        /// </summary>
        /// <param name="entitySetName">named of the entity set for the POST operation</param>
        /// <param name="operationContext">operation context instance</param>
        /// <returns>fully qualified entity type name to be created. May be null if the type hierarchy only contains 1 type or does not contain any MLE type</returns>
        protected abstract string ResolveTypeInternal(string entitySetName, DataServiceOperationContext operationContext);

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Checks the argument value for null and throw ArgumentNullException if it is null
        /// </summary>
        /// <typeparam name="T">type of the argument</typeparam>
        /// <param name="value">argument whose value needs to be checked</param>
        /// <param name="parameterName">name of the argument</param>
        /// <returns>returns the argument back</returns>
        private static T CheckArgumentNull<T>(T value, string parameterName) where T : class
        {
            if (null == value)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        /// <summary>
        /// Checks the string argument value for empty or null and throw ArgumentNullException if it is null
        /// </summary>
        /// <param name="value">argument whose value needs to be checked</param>
        /// <param name="parameterName">name of the argument</param>
        /// <returns>returns the argument back</returns>
        private static string CheckStringArgumentNull(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        /// <summary>
        /// Validates etag arguments
        /// </summary>
        /// <param name="etag">etag value</param>
        /// <param name="checkETagForEquality">If the etag was sent as the value of an If-None-Match request header, the value of �checkETagForEquality� will be set to false</param>
        private static void ValidateETag(string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext, DSPMediaResource mediaResource)
        {
            if (checkETagForEquality.HasValue)
            {
                CheckStringArgumentNull(etag, "etag");
            }
            else if (!string.IsNullOrEmpty(etag))
            {
                throw new ArgumentException("'etag' argument should be null when 'checkETagForEquality' argument has no value");
            }

            // We do not check etag for POST
            if (operationContext.RequestMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string currentEtag = mediaResource.Etag;

            if (!operationContext.RequestMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(currentEtag) && !checkETagForEquality.HasValue)
                {
                    throw new DataServiceException(400, "Since the target media resource has an etag defined, If-Match/If-Not-Match HTTP header must be specified.");
                }
            }

            if (checkETagForEquality.HasValue)
            {
                if (checkETagForEquality.Value == true && etag != currentEtag)
                {
                    throw new DataServiceException(412, "If-Match precondition failed for target media resource. Thrown by DSPStreamProvider.");
                }

                if (checkETagForEquality.Value == false && etag == currentEtag)
                {
                    if (operationContext.RequestMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new DataServiceException(304, "No Change, Thrown by DSPStreamProvider.");
                    }
                    else if (operationContext.RequestMethod.Equals("PUT", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new DataServiceException(400, "If-None-Match HTTP header cannot be specified for PUT operations. Thrown by DSPStreamProvider.");
                    }
                    else if (operationContext.RequestMethod.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new DataServiceException(400, "If-None-Match HTTP header cannot be specified for DELETE operations. Thrown by DSPStreamProvider.");
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }
            }
        }

        /// <summary>
        /// Removes all streams associated with <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">entity whose streams are to be deleted.</param>
        private void DeleteStreams(object entity)
        {
            this.streamStorage.DeleteMediaResources(entity);
        }

        /// <summary>
        /// Throw if disposed
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (this.isDisposed)
            {
                throw new InvalidOperationException("The DataServiceStreamProvider instance had already been disposed.");
            }
        }

        #endregion Private Methods
    }
}

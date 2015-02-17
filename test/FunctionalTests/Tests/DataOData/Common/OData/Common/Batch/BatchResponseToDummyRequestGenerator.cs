//---------------------------------------------------------------------
// <copyright file="BatchResponseToDummyRequestGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Batch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    
    /// <summary>
    /// This class generates a BatchRequestPayload based on the supplied response. 
    /// We can use literals here because this is a fake request we are using to trick the test deserializer into deserializing the response.
    /// </summary>
    public class BatchResponseToDummyRequestGenerator
    {
        [InjectDependency(IsRequired = true)]
        public IODataRequestManager RequestManager { get; set; }

        /// <summary>
        /// Generates a dummy request based on the supplied response and model
        /// </summary>
        /// <param name="batchResponse">Response to build the request for.</param>
        /// <param name="model">Model to get the types from.</param>
        /// <returns></returns>
        public BatchRequestPayload GenerateRequestPayload(BatchResponsePayload batchResponse, EntityModelSchema model)
        {
            ExceptionUtilities.CheckArgumentNotNull(batchResponse, "batchResponse");
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            var batchRequest = new BatchRequestPayload();
            foreach (var part in batchResponse.Parts)
            {
                var operation = part as MimePartData<HttpResponseData>;
                if (operation != null)
                {
                    batchRequest.Add(this.GenerateRequestOperation(operation.Body as ODataResponse, model).AsBatchFragment());
                }

                BatchResponseChangeset changeset = part as BatchResponseChangeset;
                if (changeset != null)
                {
                    batchRequest.Add(this.GenerateRequestChangeset(changeset, model));
                }
            }

            return batchRequest;
        }

        private BatchRequestChangeset GenerateRequestChangeset(BatchResponseChangeset batchResponseChangeset, EntityModelSchema model)
        {
            ExceptionUtilities.CheckArgumentNotNull(batchResponseChangeset, "batchResponseChangeset");
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            var contentTypeValue = batchResponseChangeset.GetHeaderValueIfExists(HttpHeaders.ContentType);
            ExceptionUtilities.CheckObjectNotNull(contentTypeValue, "Can't generate request for a response with no content type");
            var parts = contentTypeValue.Split(';');

            string boundary = parts.Where(s => s.Contains("boundary")).Single();
            string contentType = String.Concat(parts.Where(s=> s.Contains("charset")).ToArray());

            IList<MimePartData<IHttpRequest>> operations = new List<MimePartData<IHttpRequest>>();
            foreach (var operation in batchResponseChangeset.Operations)
            {
                operations.Add(this.GenerateRequestOperation((ODataResponse)operation, model).AsBatchFragment());
            }

            return BatchPayloadBuilder.RequestChangeset(boundary, contentType, operations.ToArray());
        }

        private IHttpRequest GenerateRequestOperation(ODataResponse batchResponseOperation, EntityModelSchema model)
        {
            ExceptionUtilities.CheckArgumentNotNull(batchResponseOperation, "batchResponseOperation");
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            var headers = new Dictionary<string, string> { { "GivenPayloadRequestHeader", "PayloadHeaderValue" } };
            
            string mergeContentType = HttpUtilities.BuildContentType(MimeTypes.ApplicationXml, Encoding.UTF8.WebName, null);
            ODataUri uri = null;
            HttpVerb verb = HttpVerb.Get;
            if (batchResponseOperation.RootElement != null)
            {
                var complexInstanceCollection = batchResponseOperation.RootElement as ComplexInstanceCollection;
                if (complexInstanceCollection != null)
                {
                    var function = batchResponseOperation.RootElement.GetAnnotation<FunctionAnnotation>();
                    uri = new ODataUri(ODataUriBuilder.Root(new Uri("http://www.odata.org")), ODataUriBuilder.ServiceOperation(function.Function));
                }

                var complexMultivalue = batchResponseOperation.RootElement as ComplexMultiValueProperty;
                if (complexMultivalue != null)
                {
                    var entityType = complexMultivalue.GetAnnotation<EntityModelTypeAnnotation>().EntityModelType as EntityDataType;
                    var entitySet = model.GetEntitySet(entityType.Definition.Name);
                    uri = new ODataUri(ODataUriBuilder.Root(new Uri("http://www.odata.org")), ODataUriBuilder.EntitySet(entitySet), ODataUriBuilder.Property(entityType.Definition, complexMultivalue.Name));
                }

                var complexProperty = batchResponseOperation.RootElement as ComplexProperty;
                if (complexProperty != null)
                {
                    var type = complexProperty.GetAnnotation<EntityModelTypeAnnotation>();
                    var complexDataType = type.EntityModelType as ComplexDataType;
                    // Using first because we don't need a specific entity just one that contains this type. If there is more than one the first works fine.
                    var entityType = model.EntityTypes.Where(et => et.Properties.Where(p => p.Name == complexProperty.Name).Count() == 1).First();
                    var complexType = model.ComplexTypes.Where(ct => complexDataType.Definition.Name == ct.Name).Single();
                    var complexPropertyName = entityType.AllProperties.Where(p => 
                        {
                            var complex = p.PropertyType as ComplexDataType;
                            if (complex == null)
                            {
                                return false;
                            }

                            return complex.Definition.Name == complexDataType.Definition.Name;

                        }).Single();
                    
                    uri = new ODataUri(ODataUriBuilder.Root(new Uri("http://www.odata.org")), ODataUriBuilder.Property(entityType, complexPropertyName.Name));
                }

                var deferredLink = batchResponseOperation.RootElement as DeferredLink;
                if (deferredLink != null)
                {
                    var navigationProperty = deferredLink.GetAnnotation<NavigationPropertyAnnotation>();
                    var entityType = model.EntityTypes.Where(et => et.Properties.Where(p => p.Name == navigationProperty.Property.Name).Count() == 1).First();
                    uri = new ODataUri(ODataUriBuilder.Root(new Uri("http://www.odata.org")), ODataUriBuilder.Property(entityType, navigationProperty.Property.Name));
                }

                var linkCollection = batchResponseOperation.RootElement as LinkCollection;
                if (linkCollection != null)
                {
                    uri = new ODataUri(ODataUriBuilder.Root(new Uri("http://www.odata.org")), SystemSegment.EntityReferenceLinks);
                }

                var entityInstance = batchResponseOperation.RootElement as EntityInstance;
                if (entityInstance != null)
                {
                    var type = entityInstance.GetAnnotation<EntityModelTypeAnnotation>().EntityModelType as EntityDataType;
                    var entitySetType = model.GetEntitySet(type.Definition.Name);
                    uri = new ODataUri(ODataUriBuilder.Root(new Uri("http://www.odata.org")), ODataUriBuilder.EntitySet(entitySetType));
                }

                var entitySetInstance = batchResponseOperation.RootElement as EntitySetInstance;
                if (entitySetInstance != null)
                {
                    var entitySetType = model.GetEntitySet(entityInstance.FullTypeName);
                    uri = new ODataUri(ODataUriBuilder.Root(new Uri("http://www.odata.org")), ODataUriBuilder.EntitySet(entitySetType));
                }

                var primitiveCollection = batchResponseOperation.RootElement as PrimitiveCollection;
                if (primitiveCollection != null)
                {
                    var function = batchResponseOperation.RootElement.GetAnnotation<FunctionAnnotation>();
                    uri = new ODataUri(ODataUriBuilder.Root(new Uri("http://www.odata.org")), ODataUriBuilder.ServiceOperation(function.Function));
                }

                var primitiveMultiValueProperty = batchResponseOperation.RootElement as PrimitiveMultiValueProperty;
                if (primitiveMultiValueProperty != null)
                {
                    var type = primitiveMultiValueProperty.GetAnnotation<EntityModelTypeAnnotation>();
                    var entityType = type.EntityModelType as EntityDataType;

                    uri = new ODataUri(ODataUriBuilder.Root(new Uri("http://www.odata.org")), ODataUriBuilder.EntitySet(model.GetEntitySet(entityType.Definition.Name)), ODataUriBuilder.Property(entityType.Definition, primitiveMultiValueProperty.Name));
                }

                var primitiveProperty = batchResponseOperation.RootElement as PrimitiveProperty;
                if (primitiveProperty != null)
                {
                    var type = complexProperty.GetAnnotation<EntityModelTypeAnnotation>();
                    var entityType = type.EntityModelType as EntityDataType;

                    uri = new ODataUri(ODataUriBuilder.Root(new Uri("http://www.odata.org")), ODataUriBuilder.Property(entityType.Definition, primitiveProperty.Name));
                }

                var primitiveValue = batchResponseOperation.RootElement as PrimitiveValue;
                if (primitiveValue != null)
                {
                    var primitiveType = primitiveValue.GetAnnotation<DataTypeAnnotation>();
                    var entityType = primitiveValue.GetAnnotation<EntityModelTypeAnnotation>().EntityModelType as EntityDataType;
                    var propertyName = entityType.Definition.Properties.Where(p=> p.PropertyType == primitiveType.DataType).First().Name;
                    uri = new ODataUri(ODataUriBuilder.Root(new Uri("http://www.odata.org")), ODataUriBuilder.Property(entityType.Definition, propertyName), SystemSegment.Value);
                }

                var odataError = batchResponseOperation.RootElement as ODataErrorPayload;
                if (odataError != null)
                {
                    uri = new ODataUri(ODataUriBuilder.Root(new Uri("http://www.odata.org")));
                }
            }
            else
            {
                verb = HttpVerb.Put;
                uri = new ODataUri(ODataUriBuilder.Root(new Uri("http://www.odata.org")));
            }

            ExceptionUtilities.Assert(uri != null, "The request URI has not been defined.");
            var request = this.RequestManager.BuildRequest(uri, verb, headers);
            return request;   
        }
    }
}

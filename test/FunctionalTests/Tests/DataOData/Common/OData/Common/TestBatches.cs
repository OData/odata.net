//---------------------------------------------------------------------
// <copyright file="TestBatches.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Common.Batch;
    using Microsoft.Test.Taupo.OData.Contracts;

    // Todo: this class needs to be refactored to Microsoft.Test.OData.Utils.ODataLibTest or some other namespace in order to use new EdmModel().Fixup
    public static class TestBatches
    {
        /// <summary>
        /// Creates several PayloadTestDescriptors containing Batch Requests
        /// </summary>
        /// <param name="requestManager">Used for building the requests</param>
        /// <param name="model">The model to use for adding additional types.</param>
        /// <param name="withTypeNames">Whether or not to use full type names.</param>
        /// <returns>PayloadTestDescriptors</returns>
        public static IEnumerable<PayloadTestDescriptor> CreateBatchRequestTestDescriptors(
            IODataRequestManager requestManager,
            EdmModel model,
            bool withTypeNames = false)
        {
            ExceptionUtilities.CheckArgumentNotNull(requestManager, "requestManager");
            EdmEntityType personType = null;
            EdmComplexType carType = null;
            EdmEntitySet personsEntitySet = null;
            EdmEntityContainer container = model.EntityContainer as EdmEntityContainer;

            if (model != null)
            {
                //TODO: Clone EdmModel
                //model = model.Clone();

                if (container == null)
                {
                    container = new EdmEntityContainer("TestModel", "DefaultContainer");
                    model.AddElement(container);
                }

                personType = model.FindDeclaredType("TestModel.TFPerson") as EdmEntityType;
                carType = model.FindDeclaredType("TestModel.TFCar") as EdmComplexType;

                // Create the metadata types for the entity instance used in the entity set
                if (carType == null)
                {
                    carType = new EdmComplexType("TestModel", "TFCar");
                    model.AddElement(carType);
                    carType.AddStructuralProperty("Make", EdmPrimitiveTypeKind.String, true);
                    carType.AddStructuralProperty("Color", EdmPrimitiveTypeKind.String, true);
                }

                if (personType == null)
                {
                    personType = new EdmEntityType("TestModel", "TFPerson");
                    model.AddElement(personType);
                    personType.AddKeys(personType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
                    personType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String, true);
                    personType.AddStructuralProperty("Car", carType.ToTypeReference());
                    container.AddEntitySet("Customers", personType);
                }
                
                personsEntitySet = container.AddEntitySet("People", personType);
            }

            ComplexInstance carInstance = PayloadBuilder.ComplexValue(withTypeNames ? "TestModel.TFCar" : null)
                .Property("Make", PayloadBuilder.PrimitiveValue("Ford"))
                .Property("Color", PayloadBuilder.PrimitiveValue("Blue"));
            ComplexProperty carProperty = (ComplexProperty)PayloadBuilder.Property("Car", carInstance)
                .WithTypeAnnotation(personType);

            EntityInstance personInstance = PayloadBuilder.Entity(withTypeNames ? "TestModel.TFPerson" : null)
                .Property("Id", PayloadBuilder.PrimitiveValue(1))
                .Property("Name", PayloadBuilder.PrimitiveValue("John Doe"))
                .Property("Car", carInstance)
                .WithTypeAnnotation(personType);
                
            var carPropertyPayload = new PayloadTestDescriptor()
            {
                PayloadElement = carProperty,
                PayloadEdmModel = model
            };

            var emptyPayload = new PayloadTestDescriptor()
            {
                PayloadEdmModel = CreateEmptyEdmModel()
            };

            var personPayload = new PayloadTestDescriptor()
            {
                PayloadElement = personInstance,
                PayloadEdmModel = model
            };

            var root = ODataUriBuilder.Root(new Uri("http://www.odata.org/service.svc"));
            var entityset = ODataUriBuilder.EntitySet(personsEntitySet);

            // Get operations
            var queryOperation1 = emptyPayload.InRequestOperation(HttpVerb.Get, new ODataUri(new ODataUriSegment[] { root }), requestManager);
            var queryOperation2 = emptyPayload.InRequestOperation(HttpVerb.Get, new ODataUri(new ODataUriSegment[] { root }), requestManager);

            // Post operation containing a complex property
            var postOperation = carPropertyPayload.InRequestOperation(HttpVerb.Post, new ODataUri(new ODataUriSegment[] {root, entityset}) , requestManager, MimeTypes.ApplicationJsonLight);
            // Delete operation with no payload
            var deleteOperation = emptyPayload.InRequestOperation(HttpVerb.Delete, new ODataUri(new ODataUriSegment[] { root, entityset }), requestManager);
            // Put operation where the payload is an EntityInstance
            var putOperation = personPayload.InRequestOperation(HttpVerb.Put, new ODataUri(new ODataUriSegment[] { root, entityset }), requestManager);

            // A changeset containing a delete with no payload and a put
            var twoOperationsChangeset = BatchUtils.GetRequestChangeset(new IMimePart[] { postOperation, deleteOperation }, requestManager);
            // A changeset containing a delete with no payload
            var oneOperationChangeset = BatchUtils.GetRequestChangeset(new IMimePart[] { deleteOperation }, requestManager);
            // A changeset containing a put, post and delete
            var threeOperationsChangeset = BatchUtils.GetRequestChangeset(new IMimePart[] { putOperation, postOperation, deleteOperation }, requestManager);
            // A changeset containing no operations
            var emptyChangeset = BatchUtils.GetRequestChangeset(new IMimePart[] { }, requestManager);

            // Empty Batch
            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.BatchRequestPayload()
                    .AddAnnotation(new BatchBoundaryAnnotation("bb_emptybatch")),
                PayloadEdmModel = emptyPayload.PayloadEdmModel,
                SkipTestConfiguration = (tc) => !tc.IsRequest,
            };

            // Single Operation
            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.BatchRequestPayload(queryOperation1)
                    .AddAnnotation(new BatchBoundaryAnnotation("bb_singleoperation")),
                PayloadEdmModel = model,
                SkipTestConfiguration = (tc) => !tc.IsRequest,
            };

            // Multiple Operations
            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.BatchRequestPayload(queryOperation1, queryOperation2)
                    .AddAnnotation(new BatchBoundaryAnnotation("bb_multipleoperations")),
                PayloadEdmModel = model,
                SkipTestConfiguration = (tc) => !tc.IsRequest,
            };

            // Single Changeset
            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.BatchRequestPayload(twoOperationsChangeset)
                    .AddAnnotation(new BatchBoundaryAnnotation("bb_singlechangeset")),
                PayloadEdmModel = model,
                SkipTestConfiguration = (tc) => !tc.IsRequest,
            };

            // Multiple Changesets (different content types)
            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.BatchRequestPayload(twoOperationsChangeset, oneOperationChangeset, emptyChangeset)
                    .AddAnnotation(new BatchBoundaryAnnotation("bb_multiplechangesets")),
                PayloadEdmModel = model,
                SkipTestConfiguration = (tc) => !tc.IsRequest,
            };

            // Operations and changesets
            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.BatchRequestPayload(twoOperationsChangeset, queryOperation1, oneOperationChangeset)
                    .AddAnnotation(new BatchBoundaryAnnotation("bb_operationsandchangesets_1")),
                PayloadEdmModel = model,
                SkipTestConfiguration = (tc) => !tc.IsRequest,
            };

            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.BatchRequestPayload(queryOperation1, oneOperationChangeset, queryOperation2)
                    .AddAnnotation(new BatchBoundaryAnnotation("bb_operationsandchangesets_2")),
                PayloadEdmModel = model,
                SkipTestConfiguration = (tc) => !tc.IsRequest,
            };

            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.BatchRequestPayload(queryOperation1, queryOperation2, twoOperationsChangeset, oneOperationChangeset)
                    .AddAnnotation(new BatchBoundaryAnnotation("bb_operationsandchangesets_3")),
                PayloadEdmModel = model,
                SkipTestConfiguration = (tc) => !tc.IsRequest,
            };

            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.BatchRequestPayload(queryOperation1, threeOperationsChangeset, queryOperation2, twoOperationsChangeset, queryOperation1, oneOperationChangeset)
                    .AddAnnotation(new BatchBoundaryAnnotation("bb_operationsandchangesets_4")),
                PayloadEdmModel = model,
                SkipTestConfiguration = (tc) => !tc.IsRequest,
            };

            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.BatchRequestPayload(queryOperation1, emptyChangeset, queryOperation1, threeOperationsChangeset, queryOperation2, oneOperationChangeset)
                    .AddAnnotation(new BatchBoundaryAnnotation("bb_operationsandchangesets_5")),
                PayloadEdmModel = model,
                SkipTestConfiguration = (tc) => !tc.IsRequest,
            };
        }

        /// <summary>
        /// Creates several PayloadTestDescriptors containing Batch Responses
        /// </summary>
        /// <param name="requestManager">Used for building the requests/responses.</param>
        /// <param name="model">The model to use for adding additional types.</param>
        /// <param name="withTypeNames">Whether or not to use full type names.</param>
        /// <returns>PayloadTestDescriptors</returns>
        public static IEnumerable<PayloadTestDescriptor> CreateBatchResponseTestDescriptors(
            IODataRequestManager requestManager,
            EdmModel model,
            bool withTypeNames = false)
        {
            EdmEntityType personType = null;
            EdmComplexType carType = null;

            if (model != null)
            {
                //TODO: CLONE for EdmModel
                //model = model.Clone();

                personType = model.FindDeclaredType("TestModel.TFPerson") as EdmEntityType;
                carType = model.FindDeclaredType("TestModel.TFCar") as EdmComplexType;

                // Create the metadata types for the entity instance used in the entity set
                if (carType == null)
                {
                    carType = new EdmComplexType("TestModel", "TFCar");
                    model.AddElement(carType);
                    carType.AddStructuralProperty("Make", EdmPrimitiveTypeKind.String, true);
                    carType.AddStructuralProperty("Color", EdmPrimitiveTypeKind.String, true);
                }

                if (personType == null)
                {
                    personType = new EdmEntityType("TestModel", "TFPerson");
                    model.AddElement(personType);
                    personType.AddKeys(personType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
                    personType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String, true);
                    personType.AddStructuralProperty("Car", carType.ToTypeReference());

                    EdmEntityContainer container = (EdmEntityContainer) model.EntityContainer;
                    if (container == null)
                    {
                        container = new EdmEntityContainer("TestModel", "DefaultContainer");
                        model.AddElement(container);
                        container.AddEntitySet("Customers", personType);
                        container.AddEntitySet("TFPerson", personType);
                    }
                }
            }

            ComplexInstance carInstance = PayloadBuilder.ComplexValue(withTypeNames ? "TestModel.TFCar" : null)
                .Property("Make", PayloadBuilder.PrimitiveValue("Ford"))
                .Property("Color", PayloadBuilder.PrimitiveValue("Blue"));
            ComplexProperty carProperty = (ComplexProperty)PayloadBuilder.Property("Car", carInstance)
                .WithTypeAnnotation(carType);

            EntityInstance personInstance = PayloadBuilder.Entity(withTypeNames ? "TestModel.TFPerson" : null)
                .Property("Id", PayloadBuilder.PrimitiveValue(1))
                .Property("Name", PayloadBuilder.PrimitiveValue("John Doe"))
                .Property("Car", carInstance)
                .WithTypeAnnotation(personType);

            ODataErrorPayload errorInstance = PayloadBuilder.Error("ErrorCode")
                .Message("ErrorValue")
                .InnerError(
                    PayloadBuilder.InnerError().Message("InnerErrorMessage").StackTrace("InnerErrorStackTrace").TypeName("InnerErrorTypeName"));
            
            var carPropertyPayload = new PayloadTestDescriptor()
            {
                PayloadElement = carProperty,
                PayloadEdmModel = model
            };

            var emptyPayload = new PayloadTestDescriptor()
            {
                PayloadEdmModel = CreateEmptyEdmModel()
            };

            var personPayload = new PayloadTestDescriptor()
            {
                // This payload will be serialised to JSON so we need the annotation to mark it as a response.
                PayloadElement = personInstance.AddAnnotation(new PayloadFormatVersionAnnotation() { Response = true, ResponseWrapper = true }),
                PayloadEdmModel = model
            };

            var errorPayload = new PayloadTestDescriptor()
            {
                PayloadElement = errorInstance,
                PayloadEdmModel = model,
            };

            // response operation with a status code of 5 containing a complex instance
            var carPropertyPayloadOperation = carPropertyPayload.InResponseOperation(5, requestManager);
            // response operation with no payload and a status code of 200
            var emptyPayloadOperation = emptyPayload.InResponseOperation(200, requestManager);
            // response operation with a status code of 418 containing an entity instance
            var personPayloadOperation = personPayload.InResponseOperation(418, requestManager, MimeTypes.ApplicationJsonLight);
            // response operation with a status code of 404 containing an error instance
            var errorPayloadOperation = errorPayload.InResponseOperation(404, requestManager);
            
            // changeset with multiple operations
            var twoOperationsChangeset = BatchUtils.GetResponseChangeset(new IMimePart[] { carPropertyPayloadOperation, emptyPayloadOperation }, requestManager);
            // changesets with a single operation
            var oneOperationChangeset = BatchUtils.GetResponseChangeset(new IMimePart[] { personPayloadOperation }, requestManager);
            var oneErrorChangeset = BatchUtils.GetResponseChangeset(new IMimePart[] { errorPayloadOperation }, requestManager);
            // empty changeset
            var emptyChangeset = BatchUtils.GetResponseChangeset(new IMimePart[] { }, requestManager);
           
            // Empty Batch
            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.BatchResponsePayload(new IMimePart[] {  })
                    .AddAnnotation(new BatchBoundaryAnnotation("bb_emptybatch")),
                PayloadEdmModel = emptyPayload.PayloadEdmModel,
                SkipTestConfiguration = (tc) => tc.IsRequest,
            };

            // Single Operation
            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.BatchResponsePayload(new IMimePart[] { carPropertyPayloadOperation })
                    .AddAnnotation(new BatchBoundaryAnnotation("bb_singleoperation")),
                PayloadEdmModel = model,
                SkipTestConfiguration = (tc) => tc.IsRequest,
            };

            // Multiple Operations
            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.BatchResponsePayload(new IMimePart[] { carPropertyPayloadOperation, emptyPayloadOperation, errorPayloadOperation, personPayloadOperation })
                    .AddAnnotation(new BatchBoundaryAnnotation("bb_multipleoperations")),
                PayloadEdmModel = model,
                SkipTestConfiguration = (tc) => tc.IsRequest,
            };

            // Single Changeset
            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.BatchResponsePayload(new IMimePart[] { twoOperationsChangeset })
                    .AddAnnotation(new BatchBoundaryAnnotation("bb_singlechangeset")),
                PayloadEdmModel = model,
                SkipTestConfiguration = (tc) => tc.IsRequest,
            };

            // Multiple Changesets
            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.BatchResponsePayload(new IMimePart[] { twoOperationsChangeset, oneOperationChangeset })
                    .AddAnnotation(new BatchBoundaryAnnotation("bb_multiplechangesets")),
                PayloadEdmModel = model,
                SkipTestConfiguration = (tc) => tc.IsRequest,
            };

            // Operations and changesets
            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.BatchResponsePayload(new IMimePart[] { twoOperationsChangeset, carPropertyPayloadOperation, oneOperationChangeset })
                    .AddAnnotation(new BatchBoundaryAnnotation("bb_operationsandchangesets_1")),
                PayloadEdmModel = model,
                SkipTestConfiguration = (tc) => tc.IsRequest,
            };

            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.BatchResponsePayload(new IMimePart[] { carPropertyPayloadOperation, oneOperationChangeset, emptyPayloadOperation })
                    .AddAnnotation(new BatchBoundaryAnnotation("bb_operationsandchangesets_2")),
                PayloadEdmModel = model,
                SkipTestConfiguration = (tc) => tc.IsRequest,
            };

            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.BatchResponsePayload(new IMimePart[] { errorPayloadOperation, carPropertyPayloadOperation, emptyPayloadOperation, twoOperationsChangeset, oneOperationChangeset })
                    .AddAnnotation(new BatchBoundaryAnnotation("bb_operationsandchangesets_3")),
                PayloadEdmModel = model,
                SkipTestConfiguration = (tc) => tc.IsRequest,
            };
            
            yield return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.BatchResponsePayload(
                        new IMimePart[] { oneErrorChangeset, carPropertyPayloadOperation, emptyChangeset, emptyPayloadOperation, twoOperationsChangeset, personPayloadOperation, oneOperationChangeset, errorPayloadOperation })
                    .AddAnnotation(new BatchBoundaryAnnotation("bb_operationsandchangesets_4")),
                PayloadEdmModel = model,
                SkipTestConfiguration = (tc) => tc.IsRequest,
            };   
        }

        /// <summary>
        /// Creates a batch payload with the specified number of changesets and the specified number of operations in each changeset.
        /// </summary>
        /// <param name="requestManager">Used for building the requests/responses.</param>
        /// <param name="changeSetCount">The number of changesets to create in the batch payload.</param>
        /// <param name="changeSetSizes">The size of each changeset.</param>
        /// <param name="forRequest">true if creating a batch request payload; otherwise false.</param>
        /// <param name="changeSetBoundary">The changeset boundary to use; or null to use an auto-generated one.</param>
        /// <returns>A <see cref="PayloadTestDescriptor"/> for the batch payload.</returns>
        public static PayloadTestDescriptor CreateDefaultChangeSetBatch(
            IODataRequestManager requestManager,
            int changeSetCount, 
            int[] changeSetSizes, 
            bool forRequest,
            string changeSetBoundary = null)
        {
            Debug.Assert(changeSetCount >= 0, "batchSize >= 0");
            Debug.Assert(changeSetSizes != null, "changeSetSizes != null");
            Debug.Assert(changeSetSizes.Length == changeSetCount, "Size of the batch must match the length of the change set sizes array!");

            var emptyPayload = new PayloadTestDescriptor()
            {
                PayloadEdmModel = CreateEmptyEdmModel()
            };

            var root = ODataUriBuilder.Root(new Uri("http://www.odata.org/service.svc"));
            IMimePart[] parts = new IMimePart[changeSetCount];

            if (forRequest)
            {
                // Delete operation with no payload
                var deleteOperation = emptyPayload.InRequestOperation(HttpVerb.Delete, new ODataUri(new ODataUriSegment[] { root }), requestManager);

                for (int i=0; i < changeSetCount; ++i)
                {
                    int changeSetSize = changeSetSizes[i];
                    var deleteOperations = new IMimePart[changeSetSize];
                    for(int j=0; j< changeSetSize; ++j)
                    {
                        deleteOperations[j] = deleteOperation;
                    }

                    var changeset = BatchUtils.GetRequestChangeset(deleteOperations, requestManager);
                    parts[i] = changeset;
                };

                string requestBoundary = changeSetBoundary ?? "bb_multiple_request_changesets_" + changeSetCount;
                return new PayloadTestDescriptor()
                {
                    PayloadElement = PayloadBuilder.BatchRequestPayload(parts)
                        .AddAnnotation(new BatchBoundaryAnnotation(requestBoundary)),
                };
            }

            // Response operation with no payload and a status code of 200
            var emptyPayloadOperation = emptyPayload.InResponseOperation(200, requestManager);

            for (int i=0; i < changeSetCount; ++i)
            {
                int changeSetSize = changeSetSizes[i];
                var operationResponses = new IMimePart[changeSetSize];
                for(int j=0; j< changeSetSize; ++j)
                {
                    operationResponses[j] = emptyPayloadOperation;
                }

                var changeset = BatchUtils.GetResponseChangeset(operationResponses, requestManager);
                parts[i] = changeset;
            };

            string responseBoundary = changeSetBoundary ?? "bb_multiple_response_changesets_" + changeSetCount;
            return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.BatchResponsePayload(parts)
                    .AddAnnotation(new BatchBoundaryAnnotation(responseBoundary)),
            };
        }

        /// <summary>
        /// Creates a batch payload with the specified number of changesets and the specified number of operations in each changeset.
        /// </summary>
        /// <param name="requestManager">Used for building the requests/responses.</param>
        /// <param name="changeSetCount">The number of changesets to create in the batch payload.</param>
        /// <param name="changeSetSizes">The size of each changeset.</param>
        /// <param name="forRequest">true if creating a batch request payload; otherwise false.</param>
        /// <param name="batchBoundary">The batch boundary to use; or null to use an auto-generated one.</param>
        /// <returns>A <see cref="PayloadTestDescriptor"/> for the batch payload.</returns>
        public static PayloadTestDescriptor CreateDefaultQueryBatch(
            IODataRequestManager requestManager,
            int queryCount, 
            bool forRequest,
            string batchBoundary = null)
        {
            Debug.Assert(queryCount >= 0, "batchSize >= 0");

            var emptyPayload = new PayloadTestDescriptor()
            {
                PayloadEdmModel = CreateEmptyEdmModel()
            };

            var root = ODataUriBuilder.Root(new Uri("http://www.odata.org/service.svc"));
            IMimePart[] parts = new IMimePart[queryCount];

            if (forRequest)
            {
                var queryOperation = emptyPayload.InRequestOperation(HttpVerb.Get, new ODataUri(new ODataUriSegment[] { root }), requestManager);

                for (int i = 0; i < queryCount; ++i)
                {
                    parts[i] = queryOperation;
                };

                string requestBoundary = batchBoundary ?? "bb_multiple_request_queries_" + queryCount;
                return new PayloadTestDescriptor()
                {
                    PayloadElement = PayloadBuilder.BatchRequestPayload(parts)
                        .AddAnnotation(new BatchBoundaryAnnotation(requestBoundary)),
                };
            }

            // Response operation with no payload and a status code of 200
            var emptyPayloadResponse = emptyPayload.InResponseOperation(200, requestManager);

            for (int i=0; i < queryCount; ++i)
            {
                parts[i] = emptyPayloadResponse;
            };

            string responseBoundary = batchBoundary ?? "bb_multiple_response_queries_" + queryCount;
            return new PayloadTestDescriptor()
            {
                PayloadElement = PayloadBuilder.BatchResponsePayload(parts)
                    .AddAnnotation(new BatchBoundaryAnnotation(responseBoundary)),
            };
        }

        private static IEdmModel CreateEmptyEdmModel()
        {
            EdmModel model = new EdmModel();

            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            model.AddElement(container);

            return model;
        }
    }
}

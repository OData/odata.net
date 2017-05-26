//---------------------------------------------------------------------
// <copyright file="TestParameters.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Helper class to create all interesting payload values to be used in properties.
    /// </summary>
    public static class TestParameters
    {
        /// <summary>
        /// Creates a set of interesting parameter values along with metadata
        /// </summary>
        /// <param name="model">The method creates complex and entity types for the complex and entity values and adds them to the model. The model cannot be null.</param>
        /// <param name="fullSet">true if all available parameter values should be returned, false if only the most interesting subset should be returned.</param>
        /// <returns>List of interesting parameter values.</returns>
        public static IEnumerable<ComplexInstance> CreateParameterValues(EdmModel model, bool fullSet = true)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");

            List<ODataPayloadElement> payloadValues =
                TestValues.CreatePrimitiveValuesWithMetadata(fullSet).OfType<ODataPayloadElement>()
                .Concat(TestValues.CreateComplexValues(model, true /*withTypeNames*/, fullSet).OfType<ODataPayloadElement>())
                .Concat(TestValues.CreatePrimitiveCollections(true /*withTypeNames*/, fullSet).OfType<ODataPayloadElement>())
                .Concat(TestValues.CreateComplexCollections(model, true /*withTypeNames*/, fullSet).OfType<ODataPayloadElement>())
                //
                // TODO: ODataLib test item: Add new ODataPayloadElement for parameters payload
                // We currently use ComplexInstance as a temporary solution until we have the new type in place. The new type need to support Entity and Feed.
                //.Concat(TestEntityInstances.CreateEntityInstanceTestDescriptors(model, true /*withTypeNames*/).Select(d => d.PayloadElement).OfType<ODataPayloadElement>())
                //.Concat(TestFeeds.CreateEntitySetTestDescriptors(model, true /*withTypeNames*/).Select(d => d.PayloadElement).OfType<ODataPayloadElement>())
                .ToList();

            int functionImportCount = 0;
            IEnumerable<ComplexInstance> parameterPayloads =
                payloadValues.Combinations(1).Select(payloadElements => PayloadElementsToParameterPayload(payloadElements, model, "FunctionImport" + ++functionImportCount))
                .Concat(payloadValues.Subsets(2, 5).Select(payloadElements => PayloadElementsToParameterPayload(payloadElements, model, "FunctionImport" + ++functionImportCount)));

            return parameterPayloads;
        }

        /// <summary>
        /// Builds a complex instance from the given payloadElements to represent a parameters payload.
        /// </summary>
        /// <param name="payloadElements">Each ODataPayloadElement represents the value for each parameter.</param>
        /// <param name="model">EdmModel instance.</param>
        /// <param name="functionImportName">Name of the function import to add to the model.</param>
        /// <returns></returns>
        private static ComplexInstance PayloadElementsToParameterPayload(ODataPayloadElement[] payloadElements, EdmModel model, string functionImportName)
        {
            EdmOperationImport operationImport = (EdmOperationImport)model.EntityContainer.FindOperationImports(functionImportName).FirstOrDefault();
            EdmOperation operation = (EdmOperation)operationImport.Operation;
            
            var parameterPayload = new ComplexInstance(null, false);
            for (int idx = 0; idx < payloadElements.Length; idx++)
            {
                ODataPayloadElement p = payloadElements[idx];
                string parameterName = "p" + idx;
                PropertyInstance parameter;
                IEdmTypeReference entityModelType = p.GetAnnotation<EntityModelTypeAnnotation>().EdmModelType;
                switch (p.ElementType)
                {
                    case ODataPayloadElementType.PrimitiveValue:
                        object clrValue = ((PrimitiveValue)p).ClrValue;
                        PrimitiveValue primitiveValue = new PrimitiveValue(clrValue == null ? null : clrValue.GetType().FullName, clrValue);
                        primitiveValue.CopyAnnotation<PrimitiveValue, EntityModelTypeAnnotation>(p);
                        parameter = new PrimitiveProperty(parameterName, primitiveValue);
                        operation.AddParameter(parameterName, MetadataUtils.GetPrimitiveTypeReference(primitiveValue.ClrValue.GetType()));
                        break;

                    case ODataPayloadElementType.ComplexInstance:
                        parameter = new ComplexProperty(parameterName, (ComplexInstance)p);
                        operation.AddParameter(parameterName, entityModelType);
                        break;

                    case ODataPayloadElementType.PrimitiveMultiValue:
                        PrimitiveMultiValue primitiveMultiValue = (PrimitiveMultiValue)p;
                        if (primitiveMultiValue.Annotations.OfType<JsonCollectionResultWrapperAnnotation>().SingleOrDefault() == null)
                        {
                            primitiveMultiValue.Annotations.Add(new JsonCollectionResultWrapperAnnotation(false));
                        }

                        parameter = new PrimitiveMultiValueProperty(parameterName, primitiveMultiValue);
                        operation.AddParameter(parameterName, entityModelType);
                        break;

                    case ODataPayloadElementType.ComplexMultiValue:
                        ComplexMultiValue complexMultiValue = (ComplexMultiValue)p;
                        if (complexMultiValue.Annotations.OfType<JsonCollectionResultWrapperAnnotation>().SingleOrDefault() == null)
                        {
                            complexMultiValue.Annotations.Add(new JsonCollectionResultWrapperAnnotation(false));
                        }

                        parameter = new ComplexMultiValueProperty(parameterName, complexMultiValue);
                        operation.AddParameter(parameterName, entityModelType);
                        break;

                    case ODataPayloadElementType.EntityInstance:
                        parameter = new NavigationPropertyInstance(parameterName, (EntityInstance)p);
                        operation.AddParameter(parameterName, entityModelType);
                        break;

                    case ODataPayloadElementType.EntitySetInstance:
                        parameter = new NavigationPropertyInstance(parameterName, (EntitySetInstance)p);
                        operation.AddParameter(parameterName, entityModelType);
                        break;

                    default:
                        throw new NotSupportedException("PayloadElementsToParameterPayload() is called on unsupported ODataPayloadElement type: " + p.ElementType);
                }

                parameterPayload.Add(parameter);
            }

            parameterPayload.ExpectedFunctionImport(operationImport);
            return parameterPayload;
        }
    }
}

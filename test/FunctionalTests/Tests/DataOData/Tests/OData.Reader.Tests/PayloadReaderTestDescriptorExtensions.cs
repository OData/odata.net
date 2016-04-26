//---------------------------------------------------------------------
// <copyright file="PayloadReaderTestDescriptorExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    #endregion Namespaces

    /// <summary>
    /// Extension methods for the <see cref="PayloadReaderTestDescriptor"/> class.
    /// </summary>
    internal static class PayloadReaderTestDescriptorExtensions
    {
        /// <summary>
        /// Puts the specified <paramref name="payload"/> into a property.
        /// </summary>
        /// <param name="payload">The payload to put into a property. This payload must represent a simple value (primitive, complex, collection).</param>
        /// <param name="propertyName">Optional name of the property to generate.</param>
        /// <returns>A property payload with the <paramref name="payload"/> as its value.</returns>
        public static PayloadReaderTestDescriptor InProperty(this PayloadReaderTestDescriptor payload, string propertyName = null)
        {
            return new PayloadReaderTestDescriptor(payload)
            {
                PayloadDescriptor = payload.PayloadDescriptor.InProperty(propertyName)
            };
        }

        /// <summary>
        /// Puts the specified <paramref name="payload"/> into an expanded link inside of a newly constructed entry.
        /// </summary>
        /// <param name="payload">The payload to be used as content for the expanded link.</param>
        /// <param name="isSingletonRelationship">true if the navigation property is of singleton cardinality; false for a cardinality many. Default is false.</param>
        /// <param name="randomDataGeneratorResolver">Random dataGeneration resolver</param>
        /// <param name="randomNumberGenerator">random number generator</param>
        /// <returns>An entry payload with an expanded link that contains the specified <paramref name="payload"/>.</returns>
        public static PayloadReaderTestDescriptor InEntryWithExpandedLink(
            this PayloadReaderTestDescriptor payload,
            bool isSingletonRelationship = false,
            IRandomDataGeneratorResolver randomDataGeneratorResolver = null,
            IRandomNumberGenerator randomNumberGenerator = null)
        {
            return new PayloadReaderTestDescriptor(payload)
            {
                PayloadDescriptor = payload.PayloadDescriptor.InEntryWithExpandedLink(isSingletonRelationship, randomDataGeneratorResolver, randomNumberGenerator)
            };
        }

        /// <summary>
        /// Puts the specified <paramref name="payload"/> into a feed. It accepts optional parameters to control other
        /// properties of the feed and the position of the <paramref name="payload"/> inside the feed.
        /// </summary>
        /// <param name="payload">The payload to be used inside of the feed. This must represent an entity instance payload.</param>
        /// <param name="inlineCount">An optional inline count value for the feed.</param>
        /// <param name="nextLink">An optional next-link value for the feed.</param>
        /// <param name="elementsBefore">An optional number of entries that should exist before the <paramref name="payload"/> in the feed.</param>
        /// <param name="elementsAfter">An optional number of entries that should exist after the <paramref name="payload"/> in the feed.</param>
        /// <returns>A feed payload with the <paramref name="payload"/> as one of its entities.</returns>
        public static PayloadReaderTestDescriptor InFeed(
            this PayloadReaderTestDescriptor payload,
            long? inlineCount = null,
            string nextLink = null,
            int elementsBefore = 0,
            int elementsAfter = 0)
        {
            return new PayloadReaderTestDescriptor(payload)
            {
                PayloadDescriptor = payload.PayloadDescriptor.InFeed(inlineCount, nextLink, elementsBefore, elementsAfter)
            };
        }

        /// <summary>
        /// Puts the specified <paramref name="payload"/> into a primitive or complex collection.
        /// </summary>
        /// <param name="payload">The payload to put into a collection. This payload must represent a value.</param>
        /// <param name="propertiesValuesBefore">Number of properties to put in the complex value before the <paramref name="payload"/>.</param>
        /// <param name="propertiesValuesAfter">Number of properties to put in the complex value after the <paramref name="payload"/>.</param>
        /// <returns>A collection payload with the <paramref name="payload"/> as one of its properties.</returns>
        public static PayloadReaderTestDescriptor InCollection(
            this PayloadReaderTestDescriptor payload,
            int propertiesValuesBefore = 0,
            int propertiesValuesAfter = 0)
        {
            return new PayloadReaderTestDescriptor(payload)
            {
                PayloadDescriptor = payload.PayloadDescriptor.InCollection(propertiesValuesBefore, propertiesValuesAfter)
            };
        }

        /// <summary>
        /// Puts the specified <paramref name="payload"/> into a complex value.
        /// </summary>
        /// <param name="payload">The payload to put into a complex value. This payload must represent a property instance.</param>
        /// <param name="propertiesBefore">Number of properties to put in the complex value before the <paramref name="payload"/>.</param>
        /// <param name="propertiesAfter">Number of properties to put in the complex value after the <paramref name="payload"/>.</param>
        /// <returns>A complex value payload with the <paramref name="payload"/> as one of its properties.</returns>
        public static PayloadReaderTestDescriptor InComplexValue(
            this PayloadReaderTestDescriptor payload,
            int propertiesBefore = 0,
            int propertiesAfter = 0)
        {
            return new PayloadReaderTestDescriptor(payload)
            {
                PayloadDescriptor = payload.PayloadDescriptor.InComplexValue(propertiesBefore, propertiesAfter)
            };
        }

        /// <summary>
        /// Puts the specified <paramref name="payload"/> into an entity.
        /// </summary>
        /// <param name="payload">The payload to put into an entity. This payload must represent a property instance.</param>
        /// <param name="propertiesBefore">Number of properties to put in the entity before the <paramref name="payload"/>.</param>
        /// <param name="propertiesAfter">Number of properties to put in the entity after the <paramref name="payload"/>.</param>
        /// <returns>An entity payload with the <paramref name="payload"/> as one of its properties.</returns>
        public static PayloadReaderTestDescriptor InEntity(
            this PayloadReaderTestDescriptor payload,
            int propertiesBefore = 0,
            int propertiesAfter = 0) 
        {
            return new PayloadReaderTestDescriptor(payload)
            {
                PayloadDescriptor = payload.PayloadDescriptor.InEdmEntity(propertiesBefore, propertiesAfter)
            };
        }

        /// <summary>
        /// Takes the <paramref name="navPropertyPayload"/> that represents a deferred navigation property and expands it by creating a new entity type,
        /// adding it to the model and then creating <paramref name="count"/> entities for the new type that act as content of the expanded link.
        /// </summary>
        /// <param name="navPropertyPayload">The deferred navigation property payload to be expanded.</param>
        /// <param name="isSingleton">true if a singleton navigation property should be expanded; otherwise false.</param>
        /// <param name="count">The number of entries to be used as expanded payload. Has to be '1' for singleton navigation properties.</param>
        /// <param name="nextLink">An optional next link to be used in an expanded link with a feed payload (requires <paramref name="isSingleton"/> to be false).</param>
        /// <returns>A new test descriptor with the expanded navigation property.</returns>
        public static PayloadReaderTestDescriptor ExpandNavigationProperty(this PayloadReaderTestDescriptor payload, bool isSingleton, uint count = 1, string nextLink = null)
        {
            return new PayloadReaderTestDescriptor(payload)
            {
                PayloadDescriptor = payload.PayloadDescriptor.ExpandNavigationProperty(isSingleton, count, nextLink)
            };
        }

        /// <summary>
        /// Makes sure to ignore top-level feed payloads in requests if the version is greater than V1.
        /// This is needed because for feeds in requests the MS-OData spec does not define a format and we 
        /// thus use the V1 format.
        /// </summary>
        /// <param name="descriptor">The test descriptor to filter.</param>
        /// <returns>
        /// The same or a new <see cref="PayloadReaderTestDescriptor"/> that ensures that top-level feed
        /// payloads are ignored in requests of versions greater than V1.
        /// </returns>
        public static PayloadReaderTestDescriptor FilterTopLevelFeed(this PayloadReaderTestDescriptor payload)
        {
            return new PayloadReaderTestDescriptor(payload)
            {
                PayloadDescriptor = payload.PayloadDescriptor.FilterTopLevelFeed()
            };
        }

        /// <summary>
        /// Wrapper to use the IPayloadGenerator for reader payloads.
        /// </summary>
        /// <param name="payload">The payload to generate reader input payloads for.</param>
        /// <param name="payloadGenerator">The implementation of the IPayloadGenerator to use.</param>
        /// <returns>A set of payloads that use the <paramref name="payload"/> in interesting places.</returns>
        internal static IEnumerable<PayloadReaderTestDescriptor> GenerateReaderPayloads(this IPayloadGenerator payloadGenerator, PayloadReaderTestDescriptor payload)
        {
            foreach (var payloadDescriptor in payloadGenerator.GeneratePayloads(payload.PayloadDescriptor))
            {
                yield return new PayloadReaderTestDescriptor(payload)
                {
                    PayloadDescriptor = payloadDescriptor,
                    IsGeneratedPayload = !object.ReferenceEquals(payload.PayloadDescriptor, payloadDescriptor),
                };
            }
        }

        /// <summary>
        /// Converts a <see cref="PayloadReaderTestDescriptor"/> to an in-stream error <see cref="PayloadReaderTestDescriptor"/>.
        /// </summary>
        /// <param name="descriptor">The <see cref="PayloadReaderTestDescriptor"/> to convert.</param>
        /// <param name="format">The <see cref="ODataFormat"/> the test descriptor is created for.</param>
        /// <returns>An in-stream error <see cref="PayloadReaderTestDescriptor"/> based on the <paramref name="descriptor"/>.</returns>
        /// <remarks>The <paramref name="descriptor"/> is expected to represent a top-level error payload test descriptor.</remarks>
        internal static PayloadReaderTestDescriptor ToInStreamErrorTestDescriptor(this PayloadReaderTestDescriptor descriptor, ODataFormat format)
        {
            ExceptionUtilities.CheckArgumentNotNull(descriptor, "descriptor");
            ExceptionUtilities.CheckArgumentNotNull(format, "format");
            ExceptionUtilities.Assert(descriptor.PayloadElement.ElementType == ODataPayloadElementType.ODataErrorPayload, "Only error payloads expected.");
            ExceptionUtilities.Assert(descriptor.ExpectedResultCallback == null, "No expected results callback expected.");
            ExceptionUtilities.Assert(descriptor.PayloadEdmModel == null, "No model expected.");

            // Model that defines the CityType to be used for wrapping the error payload to convert it into an in-stream error.
            IEdmModel testModel = Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            bool isValidTopLevelError = descriptor.ExpectedException == null;

            ODataPayloadElement wrappedPayloadElement = null;
            ExpectedException expectedException = null;

            if (format == ODataFormat.Json)
            {
                IEdmEntitySet citiesSet = testModel.EntityContainer.FindEntitySet("Cities");
                IEdmEntityType cityType = testModel.FindType("TestModel.CityType") as IEdmEntityType;

                var jsonRepresentation = (JsonPayloadElementRepresentationAnnotation)descriptor.PayloadElement.GetAnnotation(typeof(JsonPayloadElementRepresentationAnnotation));
                ExceptionUtilities.Assert(jsonRepresentation != null, "Expected a format-specific annotation.");
                JsonObject jsonObject = (JsonObject)jsonRepresentation.Json;

                // wrap the existing payload in an entity of type 'CityType'; the existing payload is used as value of the 'PoliceStation' property
                JsonObject wrapperObject = (JsonObject)JsonTextPreservingParser.ParseValue(new StringReader("{ \"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities/$entity\" }"));
                wrapperObject.Add(new JsonProperty("PoliceStation", jsonObject));

                // replace the payload and Json representation of the test descriptor and set the expected error message
                wrappedPayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                    .ExpectedEntityType(cityType, citiesSet)
                    .JsonRepresentation(wrapperObject);

                if (isValidTopLevelError)
                {
                    // if we have a valid top-level error we expect an ODataErrorException to be thrown.
                    ODataError error = ConvertErrorPayload((ODataErrorPayload)descriptor.PayloadElement, /*forAtom*/false);
                    expectedException = ODataExpectedExceptions.ODataErrorException(error, "ODataErrorException_GeneralError");
                }
                else
                {
                    // if the top-level error is too deeply recursive, we expect to fail in the same way for the in-stream error.
                    if (descriptor.ExpectedException.ExpectedMessage.ResourceIdentifier == "ValidationUtils_RecursionDepthLimitReached")
                    {
                        expectedException = descriptor.ExpectedException;
                    }
                    else
                    {
                        // otherwise, if the top-level error is not valid, we expect an error message that the first
                        // property of the invalid top-level error object is not defined on type OfficeType.
                        string firstPropertyName = jsonObject.Properties.First().Name;
                        expectedException = ODataExpectedExceptions.ODataException("ValidationUtils_PropertyDoesNotExistOnType", firstPropertyName, "TestModel.OfficeType");
                    }
                }
            }
            else
            {
                throw new TaupoInvalidOperationException("Unsupported format for error test descriptor found: " + format.GetType().Name);
            }

            return new PayloadReaderTestDescriptor(descriptor)
            {
                PayloadElement = wrappedPayloadElement,
                PayloadEdmModel = testModel,
                ExpectedResultPayloadElement = tc => descriptor.PayloadElement,
                ExpectedException = expectedException,
            };
        }

        /// <summary>
        /// Converts an <see cref="ODataErrorpayload"/> into the corresponding <see cref="ODataError"/>.
        /// </summary>
        /// <param name="errorPayload">The error payload to convert.</param>
        /// <param name="forAtom">true if the conversion follows ATOM format rules; false for JSON format rules.</param>
        /// <returns>A new <see cref="ODataError"/> representing the <paramref name="errorPayload"/>.</returns>
        private static ODataError ConvertErrorPayload(ODataErrorPayload errorPayload, bool forAtom)
        {
            ODataError error = new ODataError();

            error.ErrorCode = errorPayload.Code;
            error.Message = errorPayload.Message; ;

            ODataInternalExceptionPayload innerErrorPayload = errorPayload.InnerError;
            if (innerErrorPayload != null)
            {
                error.InnerError = ConvertInnerErrorPayload(innerErrorPayload);
            }

            return error;
        }

        /// <summary>
        /// Converts an <see cref="ODataInternalExceptionPayload"/> into the corresponding <see cref="ODataInnerError"/>.
        /// </summary>
        /// <param name="innerErrorPayload">The inner error payload to convert.</param>
        /// <returns>A new <see cref="ODataInnerError"/> representing the <paramref name="innerErrorPayload"/>.</returns>
        private static ODataInnerError ConvertInnerErrorPayload(ODataInternalExceptionPayload innerErrorPayload)
        {
            ODataInnerError innerError = new ODataInnerError();

            innerError.Message = innerErrorPayload.Message;
            innerError.TypeName = innerErrorPayload.TypeName;
            innerError.StackTrace = innerErrorPayload.StackTrace;

            ODataInternalExceptionPayload nestedInnerErrorPayload = innerErrorPayload.InternalException;
            if (nestedInnerErrorPayload != null)
            {
                innerError.InnerError = ConvertInnerErrorPayload(nestedInnerErrorPayload);
            }

            return innerError;
        }
    }
}

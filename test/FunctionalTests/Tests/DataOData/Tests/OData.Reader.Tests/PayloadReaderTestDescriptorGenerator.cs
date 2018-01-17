//---------------------------------------------------------------------
// <copyright file="PayloadReaderTestDescriptorGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using TestFeeds = Microsoft.Test.Taupo.OData.Common.TestFeeds;
    #endregion Namespaces

    /// <summary>
    /// Helper class to create PayloadReaderTestDescriptors.
    /// </summary>
    internal class PayloadReaderTestDescriptorGenerator
    {
        /// <summary>
        /// Creates a set of interesting service document instances.
        /// </summary>
        /// <param name="settings">The test descriptor settings to use.</param>
        /// <param name="withTitles">true if workspaces and collections should have a title; otherwise false.</param>
        /// <returns>List of test descriptors with interesting service documents as payload.</returns>
        internal static IEnumerable<PayloadReaderTestDescriptor> CreateServiceDocumentDescriptors(PayloadReaderTestDescriptor.Settings settings, string baseUri, bool withTitles)
        {
            return TestServiceDocuments.CreateServiceDocuments(withTitles, baseUri).Select(sd =>
                {
                    PayloadReaderTestDescriptor testDescriptor = new PayloadReaderTestDescriptor(settings)
                    {
                        PayloadElement = sd,
                        SkipTestConfiguration = tc => tc.IsRequest     // service docs are only allowed in responses
                    };

                    return testDescriptor;
                });
        }

        /// <summary>
        /// Creates a set of interesting entity set instances along with metadata.
        /// </summary>
        /// <param name="settings">The test descriptor settings to use.</param>
        /// <param name="model">If non-null, the method creates types as needed and adds them to the model.</param>
        /// <param name="withTypeNames">true if the payloads should specify type names.</param>
        /// <returns>List of test descriptors with interesting entity instances as payload.</returns>
        internal static IEnumerable<PayloadReaderTestDescriptor> CreateEntitySetInstanceDescriptors(
            PayloadReaderTestDescriptor.Settings settings,
            EdmModel model,
            bool withTypeNames)
        {
            List<PayloadReaderTestDescriptor> testDescriptors = new List<PayloadReaderTestDescriptor>();
            foreach (var payloadDescriptor in TestFeeds.CreateEntitySetTestDescriptors(model, withTypeNames))
            {
                testDescriptors.Add(new PayloadReaderTestDescriptor(settings)
                {
                    PayloadDescriptor = payloadDescriptor,
                    PayloadEdmModel = payloadDescriptor.PayloadEdmModel,
                    SkipTestConfiguration = tc => payloadDescriptor.SkipTestConfiguration == null ? false : payloadDescriptor.SkipTestConfiguration(tc)
                });
            }
            return testDescriptors;
        }

        /// <summary>
        /// Creates a set of test descriptors with interesting error payloads.
        /// </summary>
        /// <param name="settings">The test descriptor settings to use.</param>
        /// <returns>List of test descriptors with interesting errors as payload.</returns>
        internal static IEnumerable<PayloadReaderTestDescriptor> CreateErrorReaderTestDescriptors(PayloadReaderTestDescriptor.Settings settings)
        {
            return TestErrors.CreateErrorTestDescriptors().Select(err =>
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = err.PayloadElement,
                    SkipTestConfiguration = tc => tc.IsRequest
                });
        }

        /// <summary>
        /// Creates error reader test descriptors with deeply nested internal exceptions.
        /// </summary>
        /// <param name="settings">The test descriptor settings to use.</param>
        /// <param name="depthLimit">The maximum depth limit for nested errors. Payloads with error depth over this limit should fail.</param>
        /// <returns>An enumerable of <see cref="PayloadReaderTestDescriptor"/> representing the deeply nested error payloads.</returns>
        internal static IEnumerable<PayloadReaderTestDescriptor> CreateErrorDeeplyNestedReaderTestDescriptors(PayloadReaderTestDescriptor.Settings settings, int depthLimit)
        {
            ODataInternalExceptionPayload deeplyNestedInnerError = PayloadBuilder.InnerError();

            // Create 'depthLimit' levels of depth (loop 'depthLimit - 1' times since we've already constructed one inner error).
            for (int index = 0; index < depthLimit - 1; index++)
            {
                deeplyNestedInnerError.InnerError(deeplyNestedInnerError.DeepCopy());
            }

            // Add one more level of depth to create an invalid payload.
            ODataInternalExceptionPayload tooDeeplyNestedInnerError = PayloadBuilder.InnerError().InnerError(deeplyNestedInnerError.DeepCopy());

            yield return new PayloadReaderTestDescriptor(settings)
            {
                PayloadElement = PayloadBuilder.Error().InnerError(deeplyNestedInnerError),
                SkipTestConfiguration = tc => tc.IsRequest
            };

            yield return new PayloadReaderTestDescriptor(settings)
            {
                PayloadElement = PayloadBuilder.Error().InnerError(tooDeeplyNestedInnerError),
                ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_RecursionDepthLimitReached", Convert.ToString(depthLimit)),
                SkipTestConfiguration = tc => tc.IsRequest
            };
        }

        internal static IEnumerable<PayloadReaderTestDescriptor> CreateEntityInstanceDescriptors(
            PayloadReaderTestDescriptor.Settings settings,
            EdmModel model,
            bool withTypeNames)
        {
            List<PayloadReaderTestDescriptor> testDescriptors = new List<PayloadReaderTestDescriptor>();
            foreach (var payloadDescriptor in TestEntityInstances.CreateEntityInstanceTestDescriptors(model, withTypeNames))
            {
                // NOTE so far we don't have version-specific entity instances so we don't have to specify a
                //      minimum version or skip function; once we do we'll have to change that
                PayloadReaderTestDescriptor testDescriptor = new PayloadReaderTestDescriptor(settings)
                {
                    PayloadDescriptor = payloadDescriptor
                };
                testDescriptors.Add(testDescriptor);
            }
            return testDescriptors;
        }

        /// <summary>
        /// Creates a set of interesting primitive value test descriptors along with metadata.
        /// </summary>
        /// <param name="settings">The settings for payload reader test descriptor to use.</param>
        /// <param name="fullSet">true if all available primitive values should be returned, false if only the most interesting subset should be returned.</param>
        /// <returns>List of interesting test descriptors.</returns>
        internal static IEnumerable<PayloadReaderTestDescriptor> CreatePrimitiveValueTestDescriptors(
            PayloadReaderTestDescriptor.Settings settings,
            bool fullSet = true)
        {
            EdmModel model = new EdmModel().Fixup();
            IEnumerable<PrimitiveValue> primitiveValues = TestValues.CreatePrimitiveValuesWithMetadata(fullSet);
            return primitiveValues.Select(c => new PayloadReaderTestDescriptor(settings) { PayloadElement = c, PayloadEdmModel = model });
        }

        /// <summary>
        /// Creates a set of interesting complex value test descriptors along with metadata.
        /// </summary>
        /// <param name="settings">The settings for payload reader test descriptor to use.</param>
        /// <param name="withTypeNames">true if the complex value payloads should specify type names.</param>
        /// <param name="withMetadata">true if the generated test descriptors should have metadata.</param>
        /// <param name="fullSet">true if all available complex values should be returned, false if only the most interesting subset should be returned.</param>
        /// <returns>List of interesting test descriptors.</returns>
        internal static IEnumerable<PayloadReaderTestDescriptor> CreateComplexValueTestDescriptors(
            PayloadReaderTestDescriptor.Settings settings,
            bool withTypeNames,
            bool fullSet = true)
        {
            EdmModel model = new EdmModel().Fixup();
            IEnumerable<ComplexInstance> complexValues = TestValues.CreateComplexValues(model, withTypeNames, fullSet);
            return complexValues.Select(c => new PayloadReaderTestDescriptor(settings) { PayloadElement = c, PayloadEdmModel = model });
        }

        /// <summary>
        /// Creates a set of interesting collections with primitive items test descriptors along with metadata.
        /// </summary>
        /// <param name="withTypeNames">true if the complex value payloads should specify type names.</param>
        /// <param name="fullSet">true if all available primitive collections should be returned, false if only the most interesting subset should be returned.</param>
        /// <returns>List of interesting test descriptors.</returns>
        internal static IEnumerable<PayloadReaderTestDescriptor> CreatePrimitiveCollectionTestDescriptors(
            PayloadReaderTestDescriptor.Settings settings,
            bool withTypeNames,
            bool fullSet = true)
        {
            EdmModel model = new EdmModel().Fixup();
            IEnumerable<PrimitiveMultiValue> primitiveCollections = TestValues.CreatePrimitiveCollections(withTypeNames, fullSet);
            return primitiveCollections.Select(collection =>
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = collection,
                    PayloadEdmModel = model,
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4,
                });
        }

        /// <summary>
        /// Creates a set of interesting collections with complex items test descriptors along with metadata.
        /// </summary>
        /// <param name="withTypeNames">true if the complex value payloads should specify type names.</param>
        /// <param name="fullSet">true if all available complex collections should be returned, false if only the most interesting subset should be returned.</param>
        /// <returns>List of interesting test descriptors.</returns>
        internal static IEnumerable<PayloadReaderTestDescriptor> CreateComplexCollectionTestDescriptors(
            PayloadReaderTestDescriptor.Settings settings,
            bool withTypeNames,
            bool fullSet = true)
        {
            EdmModel model = new EdmModel().Fixup();
            IEnumerable<ComplexMultiValue> complexCollections = TestValues.CreateComplexCollections(model, withTypeNames, fullSet);
            
            return complexCollections.Select(collection =>
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = collection,
                    PayloadEdmModel = model,
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4,
                });
        }

        /// <summary>
        /// Creates a set of interesting collections test descriptors along with metadata.
        /// </summary>
        /// <param name="withTypeNames">true if the complex value payloads should specify type names.</param>
        /// <param name="fullSet">true if all available collections should be returned, false if only the most interesting subset should be returned.</param>
        /// <returns>List of interesting test descriptors.</returns>
        internal static IEnumerable<PayloadReaderTestDescriptor> CreateCollectionTestDescriptors(
            PayloadReaderTestDescriptor.Settings settings,
            bool withTypeNames,
            bool fullSet = true)
        {
            return CreatePrimitiveCollectionTestDescriptors(settings, withTypeNames, fullSet)
                .Concat(CreateComplexCollectionTestDescriptors(settings, withTypeNames, fullSet));
        }

        /// <summary>
        /// Creates a set of interesting stream reference (named stream) test descriptors, optionally with metadata.
        /// </summary>
        /// <param name="settings">The settings for payload reader test descriptor to use.</param>
        /// <param name="withMetadata">true if the generated test descriptors should have metadata.</param>
        /// <returns>Enumeration of interesting stream reference test descriptors.</returns>
        internal static IEnumerable<PayloadReaderTestDescriptor> CreateStreamReferenceValueTestDescriptors(PayloadReaderTestDescriptor.Settings settings, bool withMetadata)
        {
            EdmModel model = withMetadata ? new EdmModel().Fixup() : null;
            IEnumerable<NamedStreamInstance> namedStreams = TestValues.CreateStreamReferenceValues();
            return namedStreams.Select(c => new PayloadReaderTestDescriptor(settings) { PayloadElement = c, PayloadEdmModel = model });
        }

        /// <summary>
        /// Creates a set of interesting homogeneous collection value test descriptors along with metadata.
        /// </summary>
        /// <param name="settings">The settings for the payload reader test descriptor to use.</param>
        /// <param name="withMetadata">true if the generated test descriptors should have metadata.</param>
        /// <param name="withTypeNames">true if the collection value payloads should specify type names.</param>
        /// <param name="withExpectedType">true if an expected type annotation should be added to the generated payload element; otherwise false.</param>
        /// <param name="withcollectionName">true if the collection is not in the top level, otherwise false</param>
        /// <param name="fullSet">true if all available collection values should be returned, false if only the most interesting subset should be returned.</param>
        /// <returns>List of interesting test descriptors.</returns>
        internal static IEnumerable<PayloadReaderTestDescriptor> CreateHomogeneousCollectionValueTestDescriptors(
            PayloadReaderTestDescriptor.Settings settings,
            bool withMetadata,
            bool withTypeNames,
            bool withExpectedType,
            bool withcollectionName,
            bool fullSet = true)
        {
            EdmModel model = withMetadata ? new EdmModel() : null;
            IEnumerable<ODataPayloadElementCollection> collectionValues = TestValues.CreateHomogeneousCollectionValues(model, withTypeNames, withExpectedType, withcollectionName, fullSet);
             
            return collectionValues.Select(collectionValue =>
            {
                PayloadReaderTestDescriptor testDescriptor = new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = collectionValue,
                    PayloadEdmModel = model
                };

                if (withExpectedType && !withTypeNames)
                {
                    // fill in the type names for the expected result payload since they will be added based on the expected type
                    testDescriptor.ExpectedResultNormalizers.Add(tc => FillTypeNamesFromTypeAnnotationsPayloadElementVisitor.Visit);
                }

                if (!withExpectedType)
                {
                    testDescriptor.ExpectedResultNormalizers.Add(
                        tc => (Func<ODataPayloadElement, ODataPayloadElement>)null);
                }

                // Do not run the test descriptor if we do not have an expected type in JSON Light because it would fail.
                testDescriptor.SkipTestConfiguration = tc => tc.Format == ODataFormat.Json && !withExpectedType;

                return testDescriptor;
            });
        }

        /// <summary>
        /// Creates a set of interesting deferred navigation link test descriptors along with metadata.
        /// </summary>
        /// <param name="settings">The settings for payload reader test descriptor to use.</param>
        /// <param name="withMetadata">true if the generated test descriptors should have metadata.</param>
        /// <returns>Enumeration of interesting test descriptors.</returns>
        internal static IEnumerable<PayloadReaderTestDescriptor> CreateDeferredNavigationLinkTestDescriptors(
            PayloadReaderTestDescriptor.Settings settings,
            bool withMetadata)
        {
            EdmModel model = withMetadata ? new EdmModel().Fixup() : null;
            return TestValues.CreateDeferredNavigationLinks().Select(navigationLink =>
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = navigationLink,
                    PayloadEdmModel = model
                });
        }

        /// <summary>
        /// Creates a set of interesting expanded navigation link test descriptors along with metadata.
        /// </summary>
        /// <param name="settings">The settings for payload reader test descriptor to use.</param>
        /// <param name="withMetadata">true if the generated test descriptors should have metadata.</param>
        /// <returns>Enumeration of interesting test descriptors.</returns>
        internal static IEnumerable<PayloadReaderTestDescriptor> CreateExpandedNavigationLinkTestDescriptors(
            PayloadReaderTestDescriptor.Settings settings,
            bool withMetadata)
        {
            return (IEnumerable<PayloadReaderTestDescriptor>)CreateDeferredNavigationLinkTestDescriptors(settings, withMetadata).SelectMany(td => new[]
            {
                new PayloadReaderTestDescriptor(settings){ PayloadDescriptor = td.PayloadDescriptor.ExpandNavigationProperty(true) },
                new PayloadReaderTestDescriptor(settings){ PayloadDescriptor = td.PayloadDescriptor.ExpandNavigationProperty(false)},
                new PayloadReaderTestDescriptor(settings){ PayloadDescriptor = td.PayloadDescriptor.ExpandNavigationProperty(false, 5)},
                new PayloadReaderTestDescriptor(settings){ PayloadDescriptor = td.PayloadDescriptor.ExpandNavigationProperty(false, 5, "http://odata.org/expanded_navigation_link_next_feed_link")}
            });
        }
    }
}

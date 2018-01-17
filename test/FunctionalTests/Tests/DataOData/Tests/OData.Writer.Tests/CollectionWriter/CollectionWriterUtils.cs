//---------------------------------------------------------------------
// <copyright file="CollectionWriterUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.CollectionWriter
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Json;
    using Microsoft.Test.Taupo.OData.Writer.Tests.WriterCombinatorialEngine;

    #endregion Namespaces

    /// <summary>
    /// Helper methods for the collection writer tests.
    /// </summary>
    internal static class CollectionWriterUtils
    {
        /// <summary>
        /// Creates the expected result callback by computing the expected Xml and JSON responses
        /// based on the expected results for the items in a collection.
        /// </summary>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="itemDescriptions">The items descriptions to created the expected result for.</param>
        /// <param name="errorOnly">A flag indicating whether we are writing only error items or not.</param>
        /// <param name="assert">The assertion handler to use.</param>
        /// <returns>
        /// A <see cref="PayloadWriterTestDescriptor.WriterTestExpectedResultCallback"/> that will be used 
        /// during test execution to validate the results.
        /// </returns>
        internal static PayloadWriterTestDescriptor.WriterTestExpectedResultCallback CreateExpectedResultCallback(
            string collectionName,
            CollectionWriterTestDescriptor.ItemDescription[] itemDescriptions,
            bool errorOnly,
            WriterTestExpectedResults.Settings settings)
        {
            CollectionWriterTestDescriptor.ItemDescription[] items = itemDescriptions;
            bool writeCollection = items != null;
            items = items ?? new CollectionWriterTestDescriptor.ItemDescription[0];

            string jsonLightResultTemplate;
            CreateResultTemplates(writeCollection, items, out jsonLightResultTemplate);

            return (testConfiguration) =>
                {
                    if (testConfiguration.Format == ODataFormat.Json)
                    {
                        return new JsonWriterTestExpectedResults(settings)
                        {
                            Json = jsonLightResultTemplate,
                            FragmentExtractor = (result) => errorOnly
                                ? result.Object().PropertyValue(JsonLightConstants.ODataErrorPropertyName)
                                : result.Object().PropertyValue("value"),
                        };
                    }
                    else
                    {
                        throw new NotSupportedException("Unsupported format " + testConfiguration.Format.ToString() + " found.");
                    }
                };
        }

        /// <summary>
        /// Creates the expected result callback by setting the expected exception message(s).
        /// </summary>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="expectedExceptionFunc">A func to compute the expected exception from the writer test configuration.</param>
        /// <param name="settings">The settings to use.</param>
        /// <returns>
        /// A <see cref="PayloadWriterTestDescriptor.WriterTestExpectedResultCallback"/> that will be used 
        /// during test execution to validate the results.
        /// </returns>
        internal static PayloadWriterTestDescriptor.WriterTestExpectedResultCallback CreateExpectedErrorResultCallback(
            string collectionName,
            Func<WriterTestConfiguration, ExpectedException> expectedExceptionFunc,
            WriterTestExpectedResults.Settings settings)
        {
            return (testConfiguration) =>
            {
                if (testConfiguration.Format == ODataFormat.Json)
                {
                    return new JsonWriterTestExpectedResults(settings)
                    {
                        FragmentExtractor = (result) => JsonUtils.UnwrapTopLevelValue(testConfiguration, result),
                        ExpectedException2 = expectedExceptionFunc(testConfiguration)
                    };
                }
                else
                {
                    throw new NotSupportedException("Unsupported format " + testConfiguration.Format.ToString() + " found.");
                }
            };
        }


        /// <summary>
        /// Creates an <see cref="ODataCollectionWriter"/> for the specified format and the specified version and
        /// invokes the specified methods on it. It then parses
        /// the written Xml/JSON and compares it to the expected result as specified in the descriptor.
        /// </summary>
        /// <param name="descriptor">The test descriptor to process.</param>
        /// <param name="testConfiguration">The configuration of the test.</param>
        /// <param name="assert">The assertion handler to report errors to.</param>
        /// <param name="baselineLogger">Logger to log baseline.</param>
        internal static void WriteAndVerifyCollectionPayload(CollectionWriterTestDescriptor descriptor, WriterTestConfiguration testConfiguration, AssertionHandler assert, BaselineLogger baselineLogger)
        {
            baselineLogger.LogConfiguration(testConfiguration);
            baselineLogger.LogModelPresence(descriptor.Model);

            // serialize to a memory stream
            using (var memoryStream = new MemoryStream())
            using (var testMemoryStream = new TestStream(memoryStream, ignoreDispose: true))
            {
                TestMessage testMessage = null;
                Exception exception = TestExceptionUtils.RunCatching(() =>
                {
                    using (var messageWriter = TestWriterUtils.CreateMessageWriter(testMemoryStream, testConfiguration, assert, out testMessage, null, descriptor.Model))
                    {
                        IEdmTypeReference itemTypeReference = descriptor.ItemTypeParameter;
                        ODataCollectionWriter writer = itemTypeReference == null
                            ? messageWriter.CreateODataCollectionWriter()
                            : messageWriter.CreateODataCollectionWriter(itemTypeReference);
                        WriteCollectionPayload(messageWriter, writer, true, descriptor);
                    }
                });
                exception = TestExceptionUtils.UnwrapAggregateException(exception, assert);

                WriterTestExpectedResults expectedResults = descriptor.ExpectedResultCallback(testConfiguration);
                TestWriterUtils.ValidateExceptionOrLogResult(testMessage, testConfiguration, expectedResults, exception, assert, descriptor.TestDescriptorSettings.ExpectedResultSettings.ExceptionVerifier, baselineLogger);
                TestWriterUtils.ValidateContentType(testMessage, expectedResults, true, assert);
            }
        }

        /// <summary>
        /// Writes the collection payload as specified in the <paramref name="testDescriptor"/>.
        /// </summary>
        /// <param name="messageWriter">The message writer.</param>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="flush">True if the stream should be flush before returning; otherwise false.</param>
        /// <param name="testDescriptor">The test descriptor specifying the collection to write.</param>
        internal static void WriteCollectionPayload(ODataMessageWriterTestWrapper messageWriter, ODataCollectionWriter writer, bool flush, CollectionWriterTestDescriptor testDescriptor)
        {
            Debug.Assert(writer != null, "writer != null");
            Debug.Assert(testDescriptor != null, "testDescriptor != null");

            object[] payloadItems = testDescriptor.PayloadItems;
            int payloadItemIndex = 0;
            foreach (CollectionWriterTestDescriptor.WriterInvocations invocation in testDescriptor.Invocations)
            {
                switch (invocation)
                {
                    case CollectionWriterTestDescriptor.WriterInvocations.StartCollection:
                        ODataCollectionStartSerializationInfo serInfo = null;
                        if (!string.IsNullOrEmpty(testDescriptor.CollectionTypeName))
                        {
                            serInfo = new ODataCollectionStartSerializationInfo();
                            serInfo.CollectionTypeName = testDescriptor.CollectionTypeName;
                        }

                        writer.WriteStart(new ODataCollectionStart { Name = testDescriptor.CollectionName, SerializationInfo = serInfo });
                        break;

                    case CollectionWriterTestDescriptor.WriterInvocations.Item:
                        object payloadItem = payloadItems[payloadItemIndex];

                        ODataError error = payloadItem as ODataError;
                        if (error != null)
                        {
                            throw new InvalidOperationException("Expected payload item but found an error.");
                        }

                        writer.WriteItem(payloadItem);
                        payloadItemIndex++;
                        break;

                    case CollectionWriterTestDescriptor.WriterInvocations.Error:
                        ODataAnnotatedError error2 = testDescriptor.PayloadItems[payloadItemIndex] as ODataAnnotatedError;
                        if (error2 == null)
                        {
                            throw new InvalidOperationException("Expected an error but found a payload item.");
                        }

                        messageWriter.WriteError(error2.Error, error2.IncludeDebugInformation);
                        payloadItemIndex++;
                        break;

                    case CollectionWriterTestDescriptor.WriterInvocations.EndCollection:
                        writer.WriteEnd();
                        break;

                    case CollectionWriterTestDescriptor.WriterInvocations.UserException:
                        throw new Exception("User code triggered an exception.");

                    default:
                        break;
                }
            }

            if (flush)
            {
                writer.Flush();
            }
        }

        private static string PopulateXmlResultTemplate(string collectionName, string xmlResultTemplate)
        {
            return xmlResultTemplate == null
                ? null
                : string.Format(xmlResultTemplate,
                                collectionName,
                                TestAtomConstants.ODataNamespace,
                                TestAtomConstants.ODataMetadataNamespacePrefix,
                                TestAtomConstants.ODataCollectionItemElementName,
                                TestAtomConstants.AtomTypeAttributeName,
                                TestAtomConstants.ODataMetadataNamespace,
                                TestAtomConstants.ODataValueElementName,
                                TestAtomConstants.ODataNamespacePrefix);
        }

        private static void CreateResultTemplates(
            bool writeCollection,
            IEnumerable<CollectionWriterTestDescriptor.ItemDescription> itemResults,
            out string jsonLightResultTemplate)
        {
            List<string> xmlLines = new List<string>();
            List<string> jsonLightLines = new List<string>();

            if (itemResults != null)
            {

                bool firstItemInCollection = true;

                foreach (CollectionWriterTestDescriptor.ItemDescription itemResult in itemResults)
                {
                    string[] expectedJsonLines = itemResult.ExpectedJsonLightLines;
                    List<string> jsonLines = jsonLightLines;

                    bool firstLineInItem = true;

                    for (int i = 0; i < expectedJsonLines.Length; ++i)
                    {
                        if (i == 0)
                        {
                            if (firstItemInCollection && firstLineInItem)
                            {
                                // The very first line has to be properly indented.
                                jsonLines.Add(expectedJsonLines[i]);
                            }
                            else
                            {
                                // The first line of each expected result is appended the previous line
                                int lastIx = jsonLines.Count - 1;
                                string lastLine = jsonLines[lastIx];
                                lastLine = lastLine + "," + expectedJsonLines[i];
                                jsonLines[lastIx] = lastLine;
                            }
                        }
                        else
                        {
                            Debug.Assert(!firstLineInItem, "The 'firstLineInItem' flag should be false when we get here.");

                            // every line that is not the first for a given collection item is on its own line and indented
                            jsonLines.Add(expectedJsonLines[i]);
                        }

                        firstLineInItem = false;
                    }

                    xmlLines.Add(itemResult.ExpectedXml);
                    firstItemInCollection = false;
                }
            }

            // NOTE: can't get rid of the indent variables below and use JsonUtils.WrapTopLevel... instead because we are also
            //       writing error payloads that will result in invalid Json that we can't represent using the wrapping methods.

            if (xmlLines.Count == 0)
            {
                Debug.Assert(jsonLightLines.Count == 0);
                if (!writeCollection)
                {
                    jsonLightResultTemplate = null;
                    return;
                }

                // no items

                // "["
                jsonLightLines.Add(@"$(Indent)[");

                // write the empty line for JSON payloads without items
                jsonLightLines.Add("$(Indent)$(Indent)" + string.Empty);

                // "]"
                jsonLightLines.Add(@"$(Indent)]");

                // <value xmlns="odata-metadata-namespace" />
                xmlLines.Add(@"<{6} xmlns=""{5}"" xmlns:{7}=""{1}""/>");
            }
            else
            {
                // wrap the lines in the array scope
                // "["
                jsonLightLines.Insert(0, "$(Indent)[");


                for (int i = 1; i < jsonLightLines.Count; ++i)
                {
                    jsonLightLines[i] = "$(Indent)$(Indent)" + jsonLightLines[i];
                }

                // "]"
                jsonLightLines.Add("$(Indent)]");

                // wrap the items with the <m:value xmlns="odata-metadata-namespace"> element
                xmlLines.Insert(0, @"<{6} xmlns=""{5}"" xmlns:{7}=""{1}"">");

                // </m:value>
                xmlLines.Add(@"</{6}>");
            }
            jsonLightResultTemplate = string.Join("$(NL)", jsonLightLines);
        }

        /// <summary>
        /// Replace the arbitrary namespace prefixes used for the OData metadata namespace with the standard 'm' prefix to 
        /// make comparison easier.
        /// </summary>
        /// <param name="result">The result from be normalized.</param>
        /// <returns>The normalized data where the OData metadata namespace now has the 'm' prefix.</returns>
        private static XElement NormalizeNamespacePrefixes(XElement result)
        {
            XElement normalized = new XElement(result.Name, result.Attributes());
            foreach (XElement child in result.Elements())
            {
                if (child.Name == TestAtomConstants.ODataXNamespace + TestAtomConstants.ODataCollectionItemElementName)
                {
                    XElement normalizedChild = new XElement(TestAtomConstants.ODataXNamespace + TestAtomConstants.ODataCollectionItemElementName);
                    XAttribute metdataNamespaceAttribute = child.Attributes().Where(a => a.Value == TestAtomConstants.ODataMetadataNamespace).SingleOrDefault();
                    normalizedChild.Add(child.Attributes().Where(a => a != metdataNamespaceAttribute));
                    if (metdataNamespaceAttribute != null)
                    {
                        normalizedChild.Add(new XAttribute(XNamespace.Xmlns + TestAtomConstants.ODataMetadataNamespacePrefix, TestAtomConstants.ODataMetadataNamespace));
                    }

                    if (child.HasElements)
                    {
                        normalizedChild.Add(child.Elements());
                    }
                    else
                    {
                        normalizedChild.Value = child.Value;
                    }

                    normalized.Add(normalizedChild);
                }
                else
                {
                    normalized.Add(child);
                }
            }
            return normalized;
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="WriterActionAndFunctionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Writer
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.Json.TextAnnotations;
    using Microsoft.Test.Taupo.OData.Writer.Tests;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for writing m:action and m:function with the OData writer.
    /// </summary>
    [TestClass, TestCase]
    public class WriterActionAndFunctionTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/service/");

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        private string GetJsonLightForRelGroup(params ODataOperation[] operations)
        {
            bool useArray = operations.Length > 1;
            if (operations.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\"");
                string metadataString = operations[0].Metadata.GetComponents(UriComponents.SerializationInfoString, UriFormat.Unescaped);
                int idx = metadataString.IndexOf('#');
                metadataString = idx > 0 ? metadataString.Substring(idx, metadataString.Length - idx) : metadataString;
                sb.Append(metadataString);
                sb.Append("\":");
                if (useArray)
                {
                    sb.Append("[$(NL)");
                }

                foreach (ODataOperation operation in operations)
                {
                    sb.Append("{$(NL)");
                    sb.Append("\"title\":\"");
                    sb.Append(operation.Title);
                    sb.Append("\",");
                    sb.Append("\"target\":\"");
                    sb.Append(operation.Target.OriginalString);
                    sb.Append("\"$(NL)");
                    sb.Append("},");
                }

                sb.Remove(sb.Length - 1, 1);
                if (useArray)
                {
                    sb.Append("$(NL)]");
                }

                return sb.ToString();
            }
            else return null;
        }

        private string GetAtom(ODataOperation operation)
        {
            StringBuilder sb = new StringBuilder();
            if (operation is ODataAction)
            {
                sb.Append("<action");
            }
            else if (operation is ODataFunction)
            {
                sb.Append("<function");
            }
            else { throw new ArgumentException(); }

            sb.Append(" metadata=\"");
            sb.Append(TestUriUtils.ToEscapedUriString(operation.Metadata));
            sb.Append("\" title=\"");
            sb.Append(operation.Title);
            sb.Append("\" target=\"");
            sb.Append(operation.Target.OriginalString);
            sb.Append("\" xmlns=\"");
            sb.Append(TestAtomConstants.ODataMetadataNamespace);
            sb.Append("\" />");
            return sb.ToString();
        }

        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Validates the payloads for various m:action and m:function elements.")]
        public void ActionAndFunctionTest()
        {
            // <m:action Metadata=URI title?="title" target=URI />

            Uri actionMetadata = new Uri("http://odata.org/test/$metadata#defaultAction");
            Uri actionMetadata2 = new Uri("#action escaped relative metadata", UriKind.Relative);
            Uri actionMetadata3 = new Uri("../#action escaped relative metadata", UriKind.Relative);
            string actionTitle = "Default Action";
            Uri actionTarget = new Uri("http://odata.org/defaultActionTarget");
            Uri actionTarget2 = new Uri("http://odata.org/defaultActionTarget2");

            ODataAction action_r1_t1 = new ODataAction() { Metadata = actionMetadata, Title = actionTitle, Target = actionTarget };
            ODataAction action_r1_t2 = new ODataAction() { Metadata = actionMetadata, Title = actionTitle, Target = actionTarget2 };
            ODataAction action_r2_t1 = new ODataAction() { Metadata = actionMetadata2, Title = actionTitle, Target = actionTarget };
            ODataAction action_r3_t1 = new ODataAction() { Metadata = actionMetadata3, Title = actionTitle, Target = actionTarget };

            Uri functionMetadata = new Uri("http://odata.org/test/$metadata#defaultFunction");
            Uri functionMetadata2 = new Uri("#function escaped relative metadata", UriKind.Relative);
            Uri functionMetadata3 = new Uri("\\#function escaped relative metadata", UriKind.Relative);
            string functionTitle = "Default Function";
            Uri functionTarget = new Uri("http://odata.org/defaultFunctionTarget");
            Uri functionTarget2 = new Uri("http://odata.org/defaultFunctionTarget2");

            ODataFunction function_r1_t1 = new ODataFunction() { Metadata = functionMetadata, Title = functionTitle, Target = functionTarget };
            ODataFunction function_r1_t2 = new ODataFunction() { Metadata = functionMetadata, Title = functionTitle, Target = functionTarget2 };
            ODataFunction function_r2_t1 = new ODataFunction() { Metadata = functionMetadata2, Title = functionTitle, Target = functionTarget };
            ODataFunction function_r3_t1 = new ODataFunction() { Metadata = functionMetadata3, Title = functionTitle, Target = functionTarget };

            var actionCases = new[]
            {
                new {
                    ODataActions = new ODataAction[] { action_r1_t1 },
                    Atom = GetAtom(action_r1_t1),
                    JsonLight = GetJsonLightForRelGroup(action_r1_t1),
                },
                new {
                    ODataActions = new ODataAction[] { action_r1_t1, action_r1_t2 },
                    Atom = GetAtom(action_r1_t1) + GetAtom(action_r1_t2),
                    JsonLight = GetJsonLightForRelGroup(action_r1_t1, action_r1_t2),
                },
                new {
                    ODataActions = new ODataAction[] { action_r1_t1, action_r2_t1 },
                    Atom = GetAtom(action_r1_t1) + GetAtom(action_r2_t1),
                    JsonLight = GetJsonLightForRelGroup(action_r1_t1) + "," + GetJsonLightForRelGroup(action_r2_t1),
                },
                new {
                    ODataActions = new ODataAction[] { action_r1_t1, action_r2_t1, action_r1_t2 },
                    Atom = GetAtom(action_r1_t1) + GetAtom(action_r2_t1) + GetAtom(action_r1_t2),
                    JsonLight = GetJsonLightForRelGroup(action_r1_t1, action_r1_t2) + "," + GetJsonLightForRelGroup(action_r2_t1),
                },
                new {
                    ODataActions = new ODataAction[] { action_r3_t1 },
                    Atom = GetAtom(action_r3_t1),
                    JsonLight = GetJsonLightForRelGroup(action_r3_t1),
                },
            };

            var functionCases = new[]
            {
                new {
                    ODataFunctions = new ODataFunction[] { function_r1_t1 },
                    Atom = GetAtom(function_r1_t1),
                    JsonLight = GetJsonLightForRelGroup(function_r1_t1),
                },
                new {
                    ODataFunctions = new ODataFunction[] { function_r1_t1, function_r1_t2 },
                    Atom = GetAtom(function_r1_t1) + GetAtom(function_r1_t2),
                    JsonLight = GetJsonLightForRelGroup(function_r1_t1, function_r1_t2),
                },
                new {
                    ODataFunctions = new ODataFunction[] { function_r1_t1, function_r2_t1 },
                    Atom = GetAtom(function_r1_t1) + GetAtom(function_r2_t1),
                    JsonLight = GetJsonLightForRelGroup(function_r1_t1) + "," + GetJsonLightForRelGroup(function_r2_t1),
                },
                new {
                    ODataFunctions = new ODataFunction[] { function_r1_t1, function_r2_t1, function_r1_t2 },
                    Atom = GetAtom(function_r1_t1) + GetAtom(function_r2_t1) + GetAtom(function_r1_t2),
                    JsonLight = GetJsonLightForRelGroup(function_r1_t1, function_r1_t2) + "," + GetJsonLightForRelGroup(function_r2_t1),
                },
                new {
                    ODataFunctions = new ODataFunction[] { function_r3_t1 },
                    Atom = GetAtom(function_r3_t1),
                    JsonLight = GetJsonLightForRelGroup(function_r3_t1),
                },
            };

            var queryResults =
                from actionCase in actionCases
                from functionCase in functionCases
                select new
                {
                    actionCase.ODataActions,
                    functionCase.ODataFunctions,
                    Atom = string.Concat(actionCase.Atom, functionCase.Atom),
                    JsonLight = string.Join(",", new[] { actionCase.JsonLight, functionCase.JsonLight }.Where(x => x != null))
                };

            EdmModel model = new EdmModel();
            EdmEntityType edmEntityTypeCustomer = model.EntityType("Customer", "TestModel");
            EdmEntityContainer edmEntityContainer = model.EntityContainer("DefaultContainer","TestModel" );
            EdmEntitySet edmEntitySetCustermors = model.EntitySet("Customers", edmEntityTypeCustomer);

            var testDescriptors = queryResults.Select(testCase =>
            {
                ODataResource entry = ObjectModelUtils.CreateDefaultEntry("TestModel.Customer");

                if (testCase.ODataActions != null)
                {
                    foreach (var action in testCase.ODataActions)
                    {
                        entry.AddAction(action);
                    }
                }

                if (testCase.ODataFunctions != null)
                {
                    foreach (var function in testCase.ODataFunctions)
                    {
                        entry.AddFunction(function);
                    }
                }

                return new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    entry,
                    (testConfiguration) =>
                    {
                        if (testConfiguration.Format == ODataFormat.Json)
                        {
                            return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                            {
                                Json = string.Join(
                                    "$(NL)",
                                    "{",
                                    testCase.JsonLight,
                                    "}"),
                                ExpectedException2 =
                                    entry.Actions != null && entry.Actions.Contains(null)
                                        ? ODataExpectedExceptions.ODataException("ValidationUtils_EnumerableContainsANullItem", "ODataResource.Actions")
                                        : testConfiguration.IsRequest && entry.Actions != null && entry.Actions.Any()
                                            ? ODataExpectedExceptions.ODataException("WriterValidationUtils_OperationInRequest", GetFirstOperationMetadata(entry))
                                            : entry.Actions != null && entry.Actions.Any(a => !a.Metadata.IsAbsoluteUri && !a.Metadata.OriginalString.StartsWith("#"))
                                                ? ODataExpectedExceptions.ODataException("ValidationUtils_InvalidMetadataReferenceProperty", entry.Actions.First(a => a.Metadata.OriginalString.Contains(" ")).Metadata.OriginalString)
                                            : entry.Functions != null && entry.Functions.Contains(null)
                                                ? ODataExpectedExceptions.ODataException("ValidationUtils_EnumerableContainsANullItem", "ODataResource.Functions")
                                                : testConfiguration.IsRequest && entry.Functions != null && entry.Functions.Any()
                                                    ? ODataExpectedExceptions.ODataException("WriterValidationUtils_OperationInRequest", GetFirstOperationMetadata(entry))
                                                        : entry.Functions != null && entry.Functions.Any(f => !f.Metadata.IsAbsoluteUri && !f.Metadata.OriginalString.StartsWith("#"))
                                                            ? ODataExpectedExceptions.ODataException("ValidationUtils_InvalidMetadataReferenceProperty", entry.Functions.First(f => f.Metadata.OriginalString.Contains(" ")).Metadata.OriginalString)
                                                    : null,
                                FragmentExtractor = (result) =>
                                {
                                    var actionsAndFunctions = result.Object().Properties.Where(p => p.Name.Contains("#")).ToList();

                                    var jsonResult = new JsonObject();
                                    actionsAndFunctions.ForEach(p =>
                                    {
                                        // NOTE we remove all annotations here and in particular the text annotations to be able to easily compare
                                        //      against the expected results. This however means that we do not distinguish between the indented and non-indented case here.
                                        p.RemoveAllAnnotations(true);
                                        jsonResult.Add(p);
                                    });
                                    return jsonResult;
                                }
                            };
                        }
                        else
                        {
                            string formatName = testConfiguration.Format == null ? "null" : testConfiguration.Format.GetType().Name;
                            throw new NotSupportedException("Invalid format detected: " + formatName);
                        }
                    });
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.EntryPayloads),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    if (testConfiguration.Format == ODataFormat.Json)
                    {
                        if (testDescriptor.IsGeneratedPayload)
                        {
                            return;
                        }

                        // We need a model, entity set and entity type for JSON Light
                        testDescriptor = new PayloadWriterTestDescriptor<ODataItem>(testDescriptor)
                        {
                            Model = model,
                            PayloadEdmElementContainer = edmEntitySetCustermors,
                            PayloadEdmElementType = edmEntityTypeCustomer,
                        };
                    }

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        private string GetFirstOperationMetadata(ODataResource entry)
        {
            this.Assert.IsNotNull(entry, "entry != null");

            ODataOperation firstOperation = entry.Actions == null ? null : entry.Actions.FirstOrDefault();
            if (firstOperation == null)
            {
                firstOperation = entry.Functions == null ? null : entry.Functions.FirstOrDefault();
            }

            return firstOperation == null || firstOperation.Metadata == null
                ? null
                : firstOperation.Metadata.OriginalString;
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="ParameterWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.ParameterWriter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestModels = Microsoft.Test.OData.Utils.Metadata.TestModels;

    /// <summary>
    /// Tests for writing parameters payload with the ODataMessageWriter.
    /// </summary>
    [TestClass, TestCase]
    public class ParameterWriterTests : ODataWriterTestCase
    {
        #region DefineData

        // build collection of one/multiple primitive values
        string[] stringValues = new string[]
            {
                "foo",
                "bar",
                "collectionElement3",
                "4",
                "collectionElement---5",
                null,
                string.Empty,
                "-8",
                "This is collectionElement9",
                "-collectionElement10-",
            };

        // build collection of one/multiple complex values
        List<ODataResource> complexValues = new List<ODataResource>()
            {
                new ODataResource()
                {
                    TypeName = "TestModel.ComplexType",
                    Properties = new[] { new ODataProperty() { Name = "One", Value = 1 }}
                },
                new ODataResource()
                {
                    TypeName = "TestModel.ComplexType",
                    Properties = new[] { new ODataProperty() { Name = "One", Value = 2 }}
                },
                new ODataResource()
                {
                    TypeName = "TestModel.ComplexType",
                    Properties = new[] { new ODataProperty() { Name = "One", Value = 3 }}
                },
            };

        List<ODataResource> complexValuesNoTypeName = new List<ODataResource>()
            {
                new ODataResource()
                {
                    Properties = new[] { new ODataProperty() { Name = "One", Value = 1 }}
                },
                new ODataResource()
                {
                    Properties = new[] { new ODataProperty() { Name = "One", Value = 2 }}
                },
                new ODataResource()
                {
                    Properties = new[] { new ODataProperty() { Name = "One", Value = 3 }}
                },
            };

        // build collection of complex values containing collection properties
        List<ODataResource> complexWithCollections = new List<ODataResource>()
            {
                new ODataResource()
                {
                    TypeName = "TestModel.MyComplex",
                    Properties = new[]
                    {
                        new ODataProperty() { Name = "OneProperty", Value = 1 },
                        new ODataProperty() 
                        { 
                            Name = "OneCollectionProperty", 
                            Value = new ODataCollectionValue()
                            {
                                TypeName = EntityModelUtils.GetCollectionTypeName("Edm.Int32"),
                                Items = new object[] { 15, 20 }
                            }
                        },
                    }
                },
                new ODataResource()
                {
                    TypeName = "TestModel.MyComplex",
                    Properties = new[]
                    {
                        new ODataProperty() { Name = "OneProperty", Value = 2 },
                        new ODataProperty() 
                        { 
                            Name = "OneCollectionProperty", 
                            Value = new ODataCollectionValue()
                            {
                                TypeName = EntityModelUtils.GetCollectionTypeName("Edm.Int32"),
                                Items = new object[] { 25 }
                            } 
                        },
                    }
                },
            };

        private ODataCollectionStart GetPrimitiveStringCollection(int noOfItems)
        {
            return GetPrimitiveCollection("Edm.String", stringValues, noOfItems);
        }

        private ODataCollectionStart GetPrimitiveIntCollection(int noOfItems)
        {
            noOfItems = noOfItems < 0 ? 5 : noOfItems;
            return GetPrimitiveCollection("Edm.Int32", Enumerable.Range(0, noOfItems).ToArray(), noOfItems);
        }

        private ODataCollectionStart GetPrimitiveCollection<T>(string typeName, T[] items, int noOfItems)
        {
            ExceptionUtilities.Assert(!string.IsNullOrEmpty(typeName), "!string.IsNullOrEmpty(typeName)");
            ExceptionUtilities.Assert(items != null, "items != null");
            ExceptionUtilities.Assert(noOfItems <= items.Length, "noOfItems < items.Length");

            ODataCollectionStart primitiveItemCollectionStart = new ODataCollectionStart() { Name = EntityModelUtils.GetCollectionTypeName(typeName) };
            var primitiveItemAnnotation = new ODataCollectionItemsObjectModelAnnotation();
            int cnt = noOfItems == -1 ? items.Length : noOfItems;
            for (int i = 0; i < cnt; ++i)
            {
                primitiveItemAnnotation.Add(items[i]);
            }

            primitiveItemCollectionStart.SetAnnotation<ODataCollectionItemsObjectModelAnnotation>(primitiveItemAnnotation);
            return primitiveItemCollectionStart;
        }

        private ODataResourceSet GetComplexCollection(int noOfItems)
        {
            ODataResourceSet complexItemCollectionStart = new ODataResourceSet();
            var complexItemAnnotation = new ODataFeedEntriesObjectModelAnnotation();
            int cnt = noOfItems == -1 ? complexValues.Count : noOfItems;
            for (int i = 0; i < cnt; ++i)
            {
                complexItemAnnotation.Add(complexValues[i]);
            }

            complexItemCollectionStart.SetAnnotation(complexItemAnnotation);
            return complexItemCollectionStart;
        }

        private ODataResourceSet GetComplexCollectionNoTypeName(int noOfItems)
        {
            ODataResourceSet complexItemCollectionStart = new ODataResourceSet();
            var complexItemAnnotation = new ODataFeedEntriesObjectModelAnnotation();
            int cnt = noOfItems == -1 ? complexValuesNoTypeName.Count : noOfItems;
            for (int i = 0; i < cnt; ++i)
            {
                complexItemAnnotation.Add(complexValuesNoTypeName[i]);
            }

            complexItemCollectionStart.SetAnnotation(complexItemAnnotation);
            return complexItemCollectionStart;
        }

        private ODataResourceSet GetComplexCollectionContainingCollectionItem()
        {
            ODataResourceSet complexWithCollectionItemCollectionStart = new ODataResourceSet();
            var complexWithCollectionItemAnnotation = new ODataFeedEntriesObjectModelAnnotation();
            foreach (ODataResource cv in complexWithCollections)
            {
                complexWithCollectionItemAnnotation.Add(cv);
            }
            complexWithCollectionItemCollectionStart.SetAnnotation(complexWithCollectionItemAnnotation);

            return complexWithCollectionItemCollectionStart;
        }

        #endregion DefineData

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [TestMethod, Variation(Description = "Error test cases for ODataMessageWriter.CreateODataParameterWriter().")]
        public void CreateODataParameterWriterErrorTests()
        {
            // ODataMessageWriter.CreateODataParameterWriter() should fail on a response message.
            var testConfigurations = this.WriterTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => !tc.IsRequest);

            PayloadWriterTestDescriptor.WriterTestExpectedResultCallback resultCallback = testConfig =>
                {
                    if (testConfig.Format == ODataFormat.Json)
                    {
                        return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            ExpectedException2 = ODataExpectedExceptions.ODataException("ODataParameterWriter_CannotCreateParameterWriterOnResponseMessage")
                        };
                    }
                    else
                    {
                        throw new TaupoInvalidOperationException("Format '" + testConfig.Format.GetType().Name + "' not supported.");
                    }
                };
            var testDescriptor = new PayloadWriterTestDescriptor<ODataParameters>(this.Settings, ObjectModelUtils.CreateDefaultParameter(), resultCallback);
            this.CombinatorialEngineProvider.RunCombinations(testConfigurations, testConfiguration =>
            {
                using (var memoryStream = new TestStream())
                using (var messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, testConfiguration, this.Assert))
                {
                    TestWriterUtils.WriteAndVerifyODataParameterPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                }
            });

            // A model has to be provided in the ODataMessageWriterSettings when functionImport is not null.
            IEdmModel model = TestModels.BuildModelWithFunctionImport();
            IEdmOperationImport functionImport = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Primitive").First();
            testConfigurations = this.WriterTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => tc.IsRequest);
            resultCallback = testConfig =>
                {
                    if (testConfig.Format == ODataFormat.Json)
                    {
                        return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            ExpectedException2 = ODataExpectedExceptions.ODataException("ODataMessageWriter_CannotSpecifyOperationWithoutModel")
                        };
                    }
                    else
                    {
                        throw new TaupoInvalidOperationException("Format '" + testConfig.Format.GetType().Name + "' not supported.");
                    }
                };
            testDescriptor = new PayloadWriterTestDescriptor<ODataParameters>(this.Settings, ObjectModelUtils.CreateDefaultParameter(), resultCallback);
            this.CombinatorialEngineProvider.RunCombinations(testConfigurations, testConfiguration =>
            {
                using (var memoryStream = new TestStream())
                using (var messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, testConfiguration, this.Assert))
                {
                    TestWriterUtils.WriteAndVerifyODataParameterPayload(testDescriptor, testConfiguration, this.Assert, this.Logger, functionImport);
                }
            });
        }

        private void ParameterPayloadTest(bool withModel)
        {
            EdmModel model = new EdmModel();

            model.AddElement(new EdmComplexType("TestModel", "EmptyComplex"));
            var oneProperty = new EdmComplexType("TestModel", "OneProperty");
            oneProperty.AddStructuralProperty("One", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(oneProperty);
            var twoProperty = new EdmComplexType("TestModel", "TwoProperty");
            twoProperty.AddStructuralProperty("TwoPropertyValue", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(twoProperty);
            var collectionProperty = new EdmComplexType("TestModel", "CollectionProperty");
            collectionProperty.AddStructuralProperty("One", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true)));
            model.AddElement(collectionProperty);

            var multipleProperties = new EdmComplexType("TestModel", "MultipleProperties");
            multipleProperties.AddStructuralProperty("One", EdmCoreModel.Instance.GetInt32(false));
            multipleProperties.AddStructuralProperty("Two", EdmCoreModel.Instance.GetInt32(false));
            multipleProperties.AddStructuralProperty("Three", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(multipleProperties);

            var manyProperties = new EdmComplexType("TestModel", "ManyProperties");
            manyProperties.AddStructuralProperty("One", EdmCoreModel.Instance.GetInt32(false));
            manyProperties.AddStructuralProperty("Two", EdmCoreModel.Instance.GetDouble(false));
            manyProperties.AddStructuralProperty("Three", EdmCoreModel.Instance.GetDouble(false));
            manyProperties.AddStructuralProperty("Four", EdmCoreModel.Instance.GetInt32(true));
            manyProperties.AddStructuralProperty("Five", EdmCoreModel.Instance.GetString(true));
            manyProperties.AddStructuralProperty("Six", EdmCoreModel.Instance.GetString(true));
            manyProperties.AddStructuralProperty("Seven", EdmCoreModel.Instance.GetDateTimeOffset(false));
            manyProperties.AddStructuralProperty("Eight", EdmCoreModel.Instance.GetInt32(false));
            manyProperties.AddStructuralProperty("Nine", EdmCoreModel.Instance.GetInt32(false));
            manyProperties.AddStructuralProperty("Ten", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(manyProperties);

            var complexProperty = new EdmComplexType("TestModel", "ComplexProperty");
            complexProperty.AddStructuralProperty("One", EdmCoreModel.Instance.GetInt32(false));
            complexProperty.AddStructuralProperty("Two", new EdmComplexTypeReference(twoProperty, false));
            complexProperty.AddStructuralProperty("Three", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(complexProperty);

            var entityType = new EdmEntityType("TestModel", "TestEntityType");
            entityType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false));
            entityType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            entityType.AddStructuralProperty("ComplexProperty", new EdmComplexTypeReference(complexProperty, true));
            model.AddElement(entityType);

            var derivedEntityType = new EdmEntityType("TestModel", "TestDerivedEntityType", entityType);
            model.AddElement(derivedEntityType);
            var container = new EdmEntityContainer("TestModel", "TestContainer");
            model.AddElement(container);

            #region Feed
            ODataResource complex = new ODataResource();
            var nestedComplexAnnotation = new ODataNavigationLinkExpandedItemObjectModelAnnotation()
            {
                ExpandedItem = complex
            };
            ODataNestedResourceInfo nestedComplexInfo = new ODataNestedResourceInfo() { Name = "ComplexProperty", IsCollection = false };
            nestedComplexInfo.SetAnnotation(nestedComplexAnnotation);

            var nestedInfosAnnotation = new ODataEntryNavigationLinksObjectModelAnnotation();
            nestedInfosAnnotation.Add(nestedComplexInfo, 0);

            ODataResource entry = new ODataResource() { TypeName = "TestModel.TestEntityType", Properties = new List<ODataProperty>() { new ODataProperty() { Name = "ID", Value = 1 }, new ODataProperty() { Name = "Name", Value = "TestName" } } };
            entry.SetAnnotation(nestedInfosAnnotation);
            ODataResourceSet feed1 = new ODataResourceSet();
            feed1.SetAnnotation(new ODataFeedEntriesObjectModelAnnotation() { entry });

            ODataResource entry2 = new ODataResource() { TypeName = "TestModel.TestDerivedEntityType", Properties = new List<ODataProperty>() { new ODataProperty() { Name = "ID", Value = 1 }, new ODataProperty() { Name = "Name", Value = "TestName" } } };
            entry2.SetAnnotation(nestedInfosAnnotation);
            ODataResourceSet feed2 = new ODataResourceSet() { TypeName = "Collection(TestModel.TestEntityType)" };
            feed2.SetAnnotation(new ODataFeedEntriesObjectModelAnnotation() { entry2 });

            #endregion

            #region Complex with Nested Complex

            var complexWithNestedComplex_Nested = new ODataResource()
            {
                TypeName = "TestModel.TwoProperty",
                Properties = new[]
                {
                    new ODataProperty() { Name = "TwoPropertyValue", Value = 2 },
                }
            };
            var complexWithNestedComplex_NestdResourceAnnotation = new ODataNavigationLinkExpandedItemObjectModelAnnotation()
            {
                ExpandedItem = complexWithNestedComplex_Nested
            };

            var complexWithNestedComplex_NestedInfo = new ODataNestedResourceInfo()
            {
                Name = "Two",
                IsCollection = false
            };
            complexWithNestedComplex_NestedInfo.SetAnnotation(complexWithNestedComplex_NestdResourceAnnotation);

            var complexWithNestedComplex_NestedInfoAnnotation = new ODataEntryNavigationLinksObjectModelAnnotation();
            complexWithNestedComplex_NestedInfoAnnotation.Add(complexWithNestedComplex_NestedInfo, 0);

            var complexWithNestedComplex = new ODataResource()
            {
                TypeName = "TestModel.ComplexProperty",
                Properties = new[]
                {
                    new ODataProperty() { Name = "One", Value = 1 },
                    new ODataProperty() { Name = "Three", Value = 3 },
                }
            };
            complexWithNestedComplex.SetAnnotation(complexWithNestedComplex_NestedInfoAnnotation);

            #endregion

            var testCases = new[]
            {
                #region Empty parameters
                new
                {
                    DebugDescription = "Empty parameter payload",
                    ParameterPayload = new ODataParameters(),
                    JsonLight = "{}",
                },
                #endregion Empty parameters

                #region Single primitive parameter
                new
                {
                    DebugDescription = "Primitive string parameter",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", "foo"),
                    },
                    JsonLight = "{\"p1\":\"foo\"}"
                },
                new
                {
                    DebugDescription = "Primitive null parameter",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", null),
                    },
                    JsonLight = "{\"p1\":null}"
                },
                new
                {
                    DebugDescription = "Primitive empty string parameter",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", string.Empty),
                    },
                    JsonLight = "{\"p1\":\"\"}"
                },
                new
                {
                    DebugDescription = "Primitive integer parameter",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", 2),
                    },
                    JsonLight = "{\"p1\":2}"
                },
                new
                {
                    DebugDescription = "Primitive DateTime parameter",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", new DateTimeOffset(2011, 9, 26, 13, 20, 48, TimeSpan.Zero)),
                    },
                    JsonLight = "{\"p1\":\"2011-09-26T13:20:48Z\"}"
                },
                #endregion Single primitive parameter

                #region Single complex parameter

                new
                {
                    DebugDescription = "One complex parameter: empty complex value.",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p2", new ODataResource() { TypeName = "TestModel.EmptyComplex" })
                    },
                    JsonLight = "{\"p2\":{}}",
                },

                new
                {
                    DebugDescription = "One complex parameter: single primitive property.",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>(
                            "p1", 
                            new ODataResource()
                            {
                                TypeName = "TestModel.OneProperty",
                                Properties = new[]
                                {
                                    new ODataProperty() { Name = "One", Value = 1 },
                                }
                            })
                    },
                    JsonLight = "{\"p1\":{\"One\":1}}",
                },
                new
                {
                    DebugDescription = "One complex parameter: multiple primitive properties.",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>(
                            "p1", 
                            new ODataResource()
                            {
                                TypeName = "TestModel.MultipleProperties",
                                Properties = new[]
                                {
                                    new ODataProperty() { Name = "One", Value = 1 },
                                    new ODataProperty() { Name = "Two", Value = 2 },
                                    new ODataProperty() { Name = "Three", Value = 3 },
                                }
                            })
                    },
                    JsonLight = "{\"p1\":{\"One\":1,\"Two\":2,\"Three\":3}}",
                },
                new
                {
                    DebugDescription = "One complex parameter: many primitive properties.",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>(
                            "p1", 
                            new ODataResource()
                            {
                                TypeName = "TestModel.ManyProperties",
                                Properties = new[]
                                {
                                    new ODataProperty() { Name = "One", Value = 1 },
                                    new ODataProperty() { Name = "Two", Value = 2.222 },
                                    new ODataProperty() { Name = "Three", Value = -3.333 },
                                    new ODataProperty() { Name = "Four", Value = null },
                                    new ODataProperty() { Name = "Five", Value = string.Empty },
                                    new ODataProperty() { Name = "Six", Value = "Six" },
                                    new ODataProperty() { Name = "Seven", Value = new DateTimeOffset(2011, 9, 26, 13, 20, 48, TimeSpan.Zero) },
                                    new ODataProperty() { Name = "Eight", Value = 12345678 },
                                    new ODataProperty() { Name = "Nine", Value = 0 },
                                    new ODataProperty() { Name = "Ten", Value = -342356 },
                                }
                            })
                    },
                    JsonLight = "{\"p1\":{\"One\":1,\"Two\":2.222,\"Three\":-3.333,\"Four\":null,\"Five\":\"\",\"Six\":\"Six\",\"Seven\":\"2011-09-26T13:20:48Z\",\"Eight\":12345678,\"Nine\":0,\"Ten\":-342356}}",
                },
                new
                {
                    DebugDescription = "One complex parameter: nested complex value.",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", complexWithNestedComplex)
                    },
                    JsonLight = "{\"p1\":{\"One\":1,\"Three\":3,\"Two\":{\"TwoPropertyValue\":2}}}",
                },
                new
                {
                    DebugDescription = "One complex parameter: collection property.",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>(
                            "p1", 
                            new ODataResource()
                            {
                                TypeName = "TestModel.CollectionProperty",
                                Properties = new[]
                                {
                                    new ODataProperty() { 
                                        Name = "One", 
                                        Value = new ODataCollectionValue()
                                        {
                                            TypeName = EntityModelUtils.GetCollectionTypeName("Edm.String"),
                                            Items = new object[] { "item1", "item2" }
                                        }
                                    },
                                }
                             })
                    },
                    JsonLight = "{\"p1\":{\"One\":[\"item1\",\"item2\"]}}",
                },

                #endregion Single complex parameter

                #region Single collection parameter
                new
                {
                    DebugDescription = "Single collection parameter.",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p2", new ODataCollectionStart() { Name = "c" })
                    },
                    JsonLight = "{\"p2\":[]}",
                },
                #endregion

                #region Two parameters
                new
                {
                    DebugDescription = "Two parameters: two string parameters.",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", "foo"),
                        new KeyValuePair<string, object>("p2", "bar")
                    },
                    JsonLight = "{\"p1\":\"foo\",\"p2\":\"bar\"}",
                },
                new
                {
                    DebugDescription = "Two parameters: primitive and complex parameters.",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", "foo"),
                        new KeyValuePair<string, object>("p2", new ODataResource() { TypeName = "TestModel.EmptyComplex" })
                    },
                    JsonLight = "{\"p1\":\"foo\",\"p2\":{}}",
                },
                new
                {
                    DebugDescription = "Two parameters: primitive and collection parameters.",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", "foo"),
                        new KeyValuePair<string, object>("p2", new ODataCollectionStart() { Name = "c" })
                    },
                    JsonLight = "{\"p1\":\"foo\",\"p2\":[]}",
                },
                new
                {
                    DebugDescription = "Two parameters: collection and primitive parameters.",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", new ODataCollectionStart() { Name = "c" }),
                        new KeyValuePair<string, object>("p2", "foo"),
                    },
                    JsonLight = "{\"p1\":[],\"p2\":\"foo\"}",
                },
                new
                {
                    DebugDescription = "Two parameters: two collection parameters.",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", new ODataCollectionStart() { Name = "c" }),
                        new KeyValuePair<string, object>("p2", new ODataCollectionStart() { Name = "c" })
                    },
                    JsonLight = "{\"p1\":[],\"p2\":[]}",
                },
                new
                {
                    DebugDescription = "Two parameters: entity and complex parameters.",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", entry),
                        new KeyValuePair<string, object>("p2", new ODataResource()
                            {
                                TypeName = "TestModel.MultipleProperties",
                                Properties = new[]
                                {
                                    new ODataProperty() { Name = "One", Value = 1 },
                                    new ODataProperty() { Name = "Two", Value = 2 },
                                    new ODataProperty() { Name = "Three", Value = 3 },
                                }
                            })
                    },
                    JsonLight = "{\"p1\":{\"@odata.context\":\"http://odata.org/test/$metadata#TestModel.TestEntityType\",\"ID\":1,\"Name\":\"TestName\",\"ComplexProperty\":{}},\"p2\":{\"One\":1,\"Two\":2,\"Three\":3}}",
                },

                #endregion

                #region Single feed
                new
                {
                    DebugDescription = "Single feed parameter.",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", feed1)
                    },
                    JsonLight = "{\"p1\":[{\"@odata.type\":\"#TestModel.TestEntityType\",\"ID\":1,\"Name\":\"TestName\",\"ComplexProperty\":{}}]}",
                },
                new
                {
                    DebugDescription = "Single feed parameter with derived item.",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", feed2)
                    },
                    JsonLight = "{\"p1\":[{\"@odata.type\":\"#TestModel.TestEntityType\",\"ID\":1,\"Name\":\"TestName\",\"ComplexProperty\":{}}]}",
                },
                #endregion
            };

            var testConfigurations = this.WriterTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => tc.IsRequest);
            this.CombinatorialEngineProvider.RunCombinations(testConfigurations, testCases, (testConfiguration, testCase) =>
            {
                PayloadWriterTestDescriptor.WriterTestExpectedResultCallback resultCallback =
                    testConfig =>
                    {
                        if (testConfig.Format == ODataFormat.Json)
                        {
                            return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                            {
                                Json = testCase.JsonLight
                            };
                        }
                        else
                        {
                            throw new TaupoInvalidOperationException("Format '" + testConfig.Format.GetType().Name + "' not supported.");
                        }
                    };

                var testDescriptor = new PayloadWriterTestDescriptor<ODataParameters>(this.Settings, ObjectModelUtils.CreateDefaultParameter(), resultCallback)
                    {
                        DebugDescription = testCase.DebugDescription,
                        Model = withModel ? model : null
                    };

                TestWriterUtils.WriteActionAndVerifyODataPayload<ODataParameters>(
                    (messageWriter, writerDescriptor, feedWriter) =>
                    {
                        this.Settings.ObjectModelToMessageWriter.WriteMessage(messageWriter, ODataPayloadKind.Parameter, testCase.ParameterPayload);
                    },
                    testDescriptor,
                    testConfiguration,
                    this.Assert,
                    this.Logger);
            });
        }

        [TestMethod, Variation(Description = "Test different combinations of parameters in the payload with model.")]
        public void ParameterPayloadTest()
        {
            ParameterPayloadTest(true);
        }

        [TestMethod, Variation(Description = "Test different combinations of parameters in the payload without model.")]
        public void ParameterPayloadTest_withoutModel()
        {
            ParameterPayloadTest(false);
        }

        [TestMethod, Variation(Description = "Test different combinations of parameters in the payload.")]
        public void ParameterPayloadTestCollection()
        {
            EdmModel model = new EdmModel();

            var complexType = new EdmComplexType("TestModel", "ComplexType");
            complexType.AddStructuralProperty("One", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(complexType);

            var myComplex = new EdmComplexType("TestModel", "MyComplex");
            myComplex.AddStructuralProperty("OneProperty", EdmCoreModel.Instance.GetInt32(false));
            myComplex.AddStructuralProperty("OneCollectionProperty", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(myComplex);

            var container = new EdmEntityContainer("TestModel", "TestContainer");
            model.AddElement(container);

            var testCases = new[]
            {
                #region Collection of primitives
                new
                {
                    DebugDescription = "Collection of primitives: single string item.",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", this.GetPrimitiveStringCollection(1))
                    },
                    JsonLight = "{\"p1\":[\"foo\"]}",
                },
                new
                {
                    DebugDescription = "Collection of primitives: three string items.",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", this.GetPrimitiveStringCollection(3))
                    },
                    JsonLight = "{\"p1\":[\"foo\",\"bar\",\"collectionElement3\"]}",
                },
                new
                {
                    DebugDescription = "Collection of primitives: various string items.",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", this.GetPrimitiveStringCollection(-1))
                    },
                    JsonLight = "{\"p1\":[\"foo\",\"bar\",\"collectionElement3\",\"4\",\"collectionElement---5\",null,\"\",\"-8\",\"This is collectionElement9\",\"-collectionElement10-\"]}",
                },
                #endregion Collection of primitives
                #region Collection of complex values
                new
                {
                    DebugDescription = "Collection of complex values: single complex item.",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", this.GetComplexCollection(1))
                    },
                    JsonLight = "{\"p1\":[{\"One\":1}]}",
                },
                new
                {
                    DebugDescription = "Collection of complex values: multiple complex items.",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", this.GetComplexCollection(-1))
                    },
                    JsonLight = "{\"p1\":[{\"One\":1},{\"One\":2},{\"One\":3}]}",
                },
                #endregion Collection of complex values
                #region Collection of complex containing collection properties
                new
                {
                    DebugDescription = "Collection of complex values: complex items containing collection properties.",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", this.GetComplexCollectionContainingCollectionItem())
                    },
                    JsonLight = "{\"p1\":[{\"OneProperty\":1,\"OneCollectionProperty\":[15,20]},{\"OneProperty\":2,\"OneCollectionProperty\":[25]}]}",
                },
                #endregion Collection of complex containing collection properties
                #region Combination of primitive, complex and collection parameters
                new
                {
                    DebugDescription = "Combination of primitive, complex and collection parameters",
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", "foo"),
                        new KeyValuePair<string, object>("p2", 12345),
                        new KeyValuePair<string, object>("p3", new DateTimeOffset(2011, 12, 31, 0, 0, 0, TimeSpan.Zero)),
                        new KeyValuePair<string, object>("p4", new ODataResource(){ TypeName = "TestModel.ComplexType", Properties = new[] { new ODataProperty() { Name = "One", Value = 2 }}}),
                        new KeyValuePair<string, object>("p5", this.GetPrimitiveStringCollection(3)),
                        new KeyValuePair<string, object>("p6", this.GetComplexCollection(-1)),
                        new KeyValuePair<string, object>("p7", null),
                        new KeyValuePair<string, object>("p8", new ODataCollectionStart() { Name = "c" })
                    },
                    JsonLight = "{" +
                        "\"p1\":\"foo\"," +
                        "\"p2\":12345," + 
                        "\"p3\":\"2011-12-31T00:00:00Z\"," +
                        "\"p4\":{\"One\":2}," +
                        "\"p5\":[\"foo\",\"bar\",\"collectionElement3\"]," +
                        "\"p6\":[{\"One\":1},{\"One\":2},{\"One\":3}]," +
                        "\"p7\":null," +
                        "\"p8\":[]}",
                },
                #endregion Combination of primitive, complex and collection parameters
            };

            var testConfigurations = this.WriterTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => tc.IsRequest);
            this.CombinatorialEngineProvider.RunCombinations(testConfigurations, testCases, (testConfiguration, testCase) =>
            {
                PayloadWriterTestDescriptor.WriterTestExpectedResultCallback resultCallback =
                    testConfig =>
                    {
                        if (testConfig.Format == ODataFormat.Json)
                        {
                            return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                            {
                                Json = testCase.JsonLight
                            };
                        }
                        else
                        {
                            throw new TaupoInvalidOperationException("Format '" + testConfig.Format.GetType().Name + "' not supported.");
                        }
                    };

                var testDescriptor = new PayloadWriterTestDescriptor<ODataParameters>(this.Settings, ObjectModelUtils.CreateDefaultParameter(), resultCallback)
                {
                    DebugDescription = testCase.DebugDescription,
                    Model = model,
                };

                TestWriterUtils.WriteActionAndVerifyODataPayload<ODataParameters>(
                    (messageWriter, writerDescriptor, feedWriter) =>
                    {
                        this.Settings.ObjectModelToMessageWriter.WriteMessage(messageWriter, ODataPayloadKind.Parameter, testCase.ParameterPayload);
                    },
                    testDescriptor,
                    testConfiguration,
                    this.Assert,
                    this.Logger);
            });
        }

        [TestMethod, Variation(Description = "Verifies that in-Stream error is not supported when writing a parameter payload.")]
        public void ParameterWriterNotSupportInStreamErrorTest()
        {
            EdmModel model = new EdmModel();
            var container = new EdmEntityContainer("TestModel", "TestContainer");
            model.AddElement(container);

            var testConfigurations = this.WriterTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => tc.IsRequest);
            PayloadWriterTestDescriptor.WriterTestExpectedResultCallback resultCallback = testConfig =>
                new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                {
                    ExpectedException2 = ODataExpectedExceptions.ODataException("ODataMessageWriter_ErrorPayloadInRequest")
                };
            var testDescriptor = new PayloadWriterTestDescriptor<ODataParameters>(this.Settings, ObjectModelUtils.CreateDefaultParameter(), resultCallback)
                {
                    Model = model,
                };
            this.CombinatorialEngineProvider.RunCombinations(testConfigurations, new bool[] { true, false }, (testConfiguration, includeDebugInformation) =>
            {
                TestWriterUtils.WriteActionAndVerifyODataPayload<ODataParameters>(
                    (messageWriter, writerDescriptor, feedWriter) =>
                    {
                        ODataParameterWriter paramWriter = messageWriter.CreateODataParameterWriter(null /*functionImport*/);
                        messageWriter.WriteError(new ODataError(), includeDebugInformation);
                    },
                    testDescriptor,
                    testConfiguration,
                    this.Assert,
                    this.Logger);
            });
        }

        [TestMethod, Variation(Description = "Error test cases for writing parameter payloads.")]
        public void ParameterWriterErrorTests()
        {
            IEdmModel model = TestModels.BuildModelWithFunctionImport();
            IEdmOperationImport functionImport_Primitive = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Primitive").First();
            IEdmOperationImport functionImport_Complex = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Complex").First();
            IEdmOperationImport functionImport_PrimitiveCollection = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_PrimitiveCollection").First();
            IEdmOperationImport functionImport_NonNullablePrimitiveCollection = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_NonNullablePrimitiveCollection").First();
            IEdmOperationImport functionImport_ComplexCollection = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_ComplexCollection").First();
            IEdmOperationImport functionImport_Entry = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Entry").First();
            IEdmOperationImport functionImport_Feed = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Feed").First();
            IEdmOperationImport functionImport_Stream = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Stream").First();
            IEdmOperationImport iedmFunctionImport_PrimitiveTwoParameters = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_PrimitiveTwoParameters").First();
            IEdmOperationImport iedmFunctionImport_PrimitiveInt = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Int").First();
            IEdmOperationImport iedmFunctionImport_PrimitiveDouble = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Double").First();

            var testCases = new[]
            {
                // The function import has one more parameter then specified in the payload. Expect failure.
                new
                {
                    ParameterPayload = new ODataParameters() {  },
                    FunctionImport = functionImport_Primitive,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_MissingParameterInParameterPayload", "'primitive'", functionImport_Primitive.Name),
                },

                // The function import has more parameters then specified in the payload. Expect failure.
                new
                {
                    ParameterPayload = new ODataParameters() {  },
                    FunctionImport = iedmFunctionImport_PrimitiveTwoParameters,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_MissingParameterInParameterPayload", "'p1', 'p2'", iedmFunctionImport_PrimitiveTwoParameters.Name),
                },

                // Call WriteValue on parameter name that is not a parameter in the FunctionImport.
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", "foo")
                    },
                    FunctionImport = functionImport_Primitive,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_ParameterNameNotFoundInOperation", "p1", "FunctionImport_Primitive"),
                },

                // Call CreateCollectionWriter on parameter name that is not a parameter in the FunctionImport.
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", new ODataCollectionStart() { Name = "c" })
                    },
                    FunctionImport = functionImport_PrimitiveCollection,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_ParameterNameNotFoundInOperation", "p1", "FunctionImport_PrimitiveCollection"),
                },

                // Call WriteValue on a parameter that is not primitive or complex in the FunctionImport.
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("primitiveCollection", "foo")
                    },
                    FunctionImport = functionImport_PrimitiveCollection,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotWriteValueOnNonValueTypeKind", "primitiveCollection", "Collection"),
                },
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("complexCollection", "foo")
                    },
                    FunctionImport = functionImport_ComplexCollection,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotWriteValueOnNonValueTypeKind", "complexCollection", "Collection"),
                },
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("entry", "foo")
                    },
                    FunctionImport = functionImport_Entry,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotWriteValueOnNonValueTypeKind", "entry", "Entity"),
                },
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("feed", "foo")
                    },
                    FunctionImport = functionImport_Feed,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotWriteValueOnNonValueTypeKind", "feed", "Collection"),
                },

                // Call CreateCollectionWriter on a parameter that is not Collection in the FunctionImport.
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("primitive", new ODataCollectionStart() { Name = "c" })
                    },
                    FunctionImport = functionImport_Primitive,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotCreateCollectionWriterOnNonCollectionTypeKind", "primitive", "Primitive"),
                },
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("complex", new ODataCollectionStart() { Name = "c" })
                    },
                    FunctionImport = functionImport_Complex,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotCreateCollectionWriterOnNonCollectionTypeKind", "complex", "Complex"),
                },
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("entry", new ODataCollectionStart() { Name = "c" })
                    },
                    FunctionImport = functionImport_Entry,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotCreateCollectionWriterOnNonCollectionTypeKind", "entry", "Entity"),
                },
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("feed", new ODataCollectionStart() { Name = "c" })
                    },
                    FunctionImport = functionImport_Feed,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotCreateCollectionWriterOnNonCollectionTypeKind", "feed", "Collection"),
                },

                // Calling WriteValue with unsupported values
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", new MemoryStream())
                    },
                    FunctionImport = default(IEdmOperationImport),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotWriteValueOnNonSupportedValueType", "p1", "System.IO.MemoryStream"),
                },
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", new ODataCollectionValue())
                    },
                    FunctionImport = default(IEdmOperationImport),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotWriteValueOnNonSupportedValueType", "p1", "Microsoft.OData.ODataCollectionValue"),
                },
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", new ODataStreamReferenceValue())
                    },
                    FunctionImport = default(IEdmOperationImport),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotWriteValueOnNonSupportedValueType", "p1", "Microsoft.OData.ODataStreamReferenceValue"),
                },
                // Calling WriteValue and CreateCollectionWriter with null parameterName
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>(null, "string param")
                    },
                    FunctionImport = default(IEdmOperationImport),
                    ExpectedException = ODataExpectedExceptions.ArgumentNullOrEmptyException(),
                },
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>(string.Empty, "string param")
                    },
                    FunctionImport = default(IEdmOperationImport),
                    ExpectedException = ODataExpectedExceptions.ArgumentNullOrEmptyException(),
                },
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>(null, new ODataCollectionStart() { Name = "c" })
                    },
                    FunctionImport = default(IEdmOperationImport),
                    ExpectedException = ODataExpectedExceptions.ArgumentNullOrEmptyException(),
                },
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>(string.Empty, new ODataCollectionStart() { Name = "c" })
                    },
                    FunctionImport = default(IEdmOperationImport),
                    ExpectedException = ODataExpectedExceptions.ArgumentNullOrEmptyException(),
                },

                // the function import states that the type is a string, but we write an int.
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("primitive", 6),
                    },
                    FunctionImport = functionImport_Primitive,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncompatiblePrimitiveItemType", "Edm.Int32", "False", "Edm.String", "False"),
                },

                // function import states that the type is Int32 and we write a double.
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", 6.6),
                    },
                    FunctionImport = iedmFunctionImport_PrimitiveInt,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncompatiblePrimitiveItemType", "Edm.Double", "False", "Edm.Int32", "False"),
                },

                // function import states that the type is double and we write a int32.
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", 6),
                    },
                    FunctionImport = iedmFunctionImport_PrimitiveDouble,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncompatiblePrimitiveItemType", "Edm.Int32", "False", "Edm.Double", "False"),
                },

                // function import states that the type is complex, but we write primitive
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("complex", 6),
                    },
                    FunctionImport = functionImport_Complex,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotWriteValueOnNonValueTypeKind", "complex", "Complex"),
                },

                // complex type different in the payload and in the function import.
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("complex", 
                            new ODataResource() 
                            {
                                TypeName = "TestModel.SomeComplexType",
                                Properties = new[] { new ODataProperty() { Name = "One", Value = 1 } }
                            }),
                    },
                    FunctionImport = functionImport_Complex,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_UnrecognizedTypeName", "TestModel.SomeComplexType"),
                },

                // complex type without typename in the payload, primitive type in the function import.
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("primitive", 
                            new ODataResource() 
                            {
                                Properties = new[] { new ODataProperty() { Name = "One", Value = 1 } }
                            }),
                    },
                    FunctionImport = functionImport_Primitive,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotCreateResourceWriterOnNonEntityOrComplexTypeKind", "primitive", "Primitive"),
                },

                // primitive collection in the payload, but complex collection in the function import.
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("complexCollection", this.GetPrimitiveStringCollection(1))
                    },
                    FunctionImport = functionImport_ComplexCollection,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_NonPrimitiveTypeForPrimitiveValue", "TestModel.ComplexType")
                },

                // primitive collection of wrong type in the payload
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("primitiveCollection", this.GetPrimitiveIntCollection(1))
                    },
                    FunctionImport = functionImport_PrimitiveCollection,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncompatiblePrimitiveItemType", "Edm.Int32", "False", "Edm.String", "True"),
                },

                // complex collection in the payload, but primitive collection in the function import.
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("primitiveCollection", this.GetComplexCollection(1))
                    },
                    FunctionImport = functionImport_PrimitiveCollection,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotCreateResourceSetWriterOnNonStructuredCollectionTypeKind", "primitiveCollection", "Collection")
                },

                // complex collection without payload types in the payload, but primitive collection in the function import.
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("primitiveCollection", this.GetComplexCollectionNoTypeName(1))
                    },
                    FunctionImport = functionImport_PrimitiveCollection,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_CannotCreateResourceSetWriterOnNonStructuredCollectionTypeKind", "primitiveCollection", "Collection")
                },
            };

            var testConfigurations = this.WriterTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => tc.IsRequest);
            this.CombinatorialEngineProvider.RunCombinations(testConfigurations, testCases, (testConfiguration, testCase) =>
            {
                PayloadWriterTestDescriptor.WriterTestExpectedResultCallback resultCallback = testConfig => new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings) { ExpectedException2 = testCase.ExpectedException };
                var testDescriptor = new PayloadWriterTestDescriptor<ODataParameters>(this.Settings, ObjectModelUtils.CreateDefaultParameter(), resultCallback) { Model = model };

                TestWriterUtils.WriteActionAndVerifyODataPayload<ODataParameters>(
                    (messageWriter, writerDescriptor, feedWriter) =>
                    {
                        this.Settings.ObjectModelToMessageWriter.WriteMessage(messageWriter, ODataPayloadKind.Parameter, testCase.ParameterPayload, model, testCase.FunctionImport);
                    },
                    testDescriptor,
                    testConfiguration,
                    this.Assert,
                    this.Logger);
            });
        }

        [TestMethod, Variation(Description = "Test writing parameter payloads with function import present")]
        public void ParameterPayloadTestWithFunctionImport()
        {
            bool[] bindable = new bool[] { true, false };

            EdmModel model = new EdmModel();
            var complexType = new EdmComplexType("TestModel", "ComplexType");
            complexType.AddStructuralProperty("PrimitiveProperty", EdmPrimitiveTypeKind.String, isNullable: false);
            complexType.AddStructuralProperty("ComplexProperty", new EdmComplexTypeReference(complexType, isNullable: true));
            model.AddElement(complexType);
            var container = new EdmEntityContainer("TestModel", "TestContainer");
            model.AddElement(container);

            ODataResourceSet complexItemCollectionStart = new ODataResourceSet();
            var complexItemAnnotation = new ODataFeedEntriesObjectModelAnnotation();
            complexItemAnnotation.Add(
                new ODataResource()
                {
                    TypeName = "TestModel.ComplexType",
                    Properties = new[] { new ODataProperty() { Name = "PrimitiveProperty", Value = "foo" }, new ODataProperty() { Name = "ComplexProperty", Value = null } }
                });

            complexItemCollectionStart.SetAnnotation(complexItemAnnotation);

            foreach (bool value in bindable)
            {
                container.AddActionAndActionImport(model, "FunctionImport_NullablePrimitive_Bindable" + value, null /*returnType*/, null /*entitySet*/, value /*bindable*/).Action.AsEdmAction().AddParameter("primitive", EdmCoreModel.Instance.GetString(isNullable: true));

                container.AddActionAndActionImport(model, "FunctionImport_Primitive_Bindable" + value, null /*returnType*/, null /*entitySet*/, value /*bindable*/).Action.AsEdmAction().AddParameter("primitive", EdmCoreModel.Instance.GetString(isNullable: true));

                container.AddActionAndActionImport(model, "FunctionImport_PrimitiveCollection_Bindable" + value, null /*returnType*/, null /*entitySet*/, value /*bindable*/).Action.AsEdmAction().AddParameter("primitiveCollection", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(isNullable: true)));

                container.AddActionAndActionImport(model, "FunctionImport_Complex_Bindable" + value, null /*returnType*/, null /*entitySet*/, value /*bindable*/).Action.AsEdmAction().AddParameter("complex", new EdmComplexTypeReference(complexType, isNullable: true));

                container.AddActionAndActionImport(model, "FunctionImport_ComplexCollection_Bindable" + value, null /*returnType*/, null /*entitySet*/, value /*bindable*/).Action.AsEdmAction().AddParameter("complexCollection", EdmCoreModel.GetCollection(new EdmComplexTypeReference(complexType, isNullable: true)));

                EdmFunction function = new EdmFunction(container.Namespace, "FunctionImport_PrimitiveNoSideEffecting_Bindable_" + value, EdmCoreModel.Instance.GetInt32(true), value /*isBound*/, null, true /*isComposable*/);
                function.AddParameter("p1", EdmCoreModel.Instance.GetInt32(isNullable: true));
                model.AddElement(function);

                container.AddFunctionImport("FunctionImport_PrimitiveNoSideEffecting_Bindable_" + value, function);
            }

            var allTestCases = bindable.SelectMany(value =>
            {
                var testCases = new[]
                { 
                    // Nullable parameters can be ignored.
                    new
                    {
                        ParameterPayload = new ODataParameters() {  },
                        FunctionImport = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_NullablePrimitive_Bindable" + value).First(),
                        JsonLight = "{}",
                    },

                    // funtion import with side-effecting = false.
                    new
                    {
                        ParameterPayload = new ODataParameters()
                        {
                            new KeyValuePair<string, object>("p1", 5)
                        },
                        FunctionImport = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_PrimitiveNoSideEffecting_Bindable_" + value).First(),
                        JsonLight = "{\"p1\":5}",
                    },

                    // primitive with function import
                    new
                    {
                        ParameterPayload = new ODataParameters()
                        {
                            new KeyValuePair<string, object>("primitive", "foo")
                        },
                        FunctionImport = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Primitive_Bindable" + value).First(),
                        JsonLight = "{\"primitive\":\"foo\"}",
                    },

                    // complex with function import 
                    new
                    {
                        ParameterPayload = new ODataParameters()
                        {
                            new KeyValuePair<string, object>(
                                "complex", 
                                new ODataResource()
                                {
                                    TypeName = "TestModel.ComplexType",
                                    Properties = new[] { new ODataProperty() { Name = "PrimitiveProperty", Value = "foo" }, new ODataProperty() { Name="ComplexProperty", Value = null } }
                                })
                        },
                        FunctionImport = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Complex_Bindable" + value).First(),
                        JsonLight = "{\"complex\":{\"PrimitiveProperty\":\"foo\",\"ComplexProperty\":null}}",
                    },
 
                    // primitive collection with function import.
                    new
                    {
                        ParameterPayload = new ODataParameters()
                        {
                            new KeyValuePair<string, object>("primitiveCollection", this.GetPrimitiveStringCollection(1))
                        },
                        FunctionImport = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_PrimitiveCollection_Bindable" + value).First(),
                        JsonLight = "{\"primitiveCollection\":[\"foo\"]}",
                    },

                    // complex collection with function import.
                    new
                    {
                        ParameterPayload = new ODataParameters()
                        {
                            new KeyValuePair<string, object>("complexCollection", complexItemCollectionStart),
                        },
                        FunctionImport = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_ComplexCollection_Bindable" + value).First(),
                        JsonLight = "{\"complexCollection\":[{\"PrimitiveProperty\":\"foo\",\"ComplexProperty\":null}]}",
                    },
                };

                return testCases;

            });

            var testConfigurations = this.WriterTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => tc.IsRequest);
            this.CombinatorialEngineProvider.RunCombinations(testConfigurations, allTestCases, (testConfiguration, testCase) =>
            {
                PayloadWriterTestDescriptor.WriterTestExpectedResultCallback resultCallback = testConfig =>
                    {
                        if (testConfig.Format == ODataFormat.Json)
                        {
                            return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings) { Json = testCase.JsonLight };
                        }
                        else
                        {
                            throw new TaupoInvalidOperationException("Format '" + testConfig.Format.GetType().Name + "' is not supported.");
                        }
                    };
                var testDescriptor = new PayloadWriterTestDescriptor<ODataParameters>(this.Settings, ObjectModelUtils.CreateDefaultParameter(), resultCallback) { Model = model };

                TestWriterUtils.WriteActionAndVerifyODataPayload<ODataParameters>(
                    (messageWriter, writerDescriptor, feedWriter) =>
                    {
                        this.Settings.ObjectModelToMessageWriter.WriteMessage(messageWriter, ODataPayloadKind.Parameter, testCase.ParameterPayload, model, testCase.FunctionImport);
                    },
                    testDescriptor,
                    testConfiguration,
                    this.Assert,
                    this.Logger);
            });
        }

        [TestMethod, Variation(Description = "Test writing parameter payloads with duplicate parameter names.")]
        public void DuplicateParameterNamesTest()
        {
            EdmModel edmModel = new EdmModel();
            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            edmModel.AddElement(container);

            EdmAction action = new EdmAction("TestModel", "f1", null);
            action.AddParameter("p1", EdmCoreModel.Instance.GetString(isNullable: true));
            EdmOperationImport edmF1 = container.AddActionImport("f1", action);

            EdmAction action2 = new EdmAction("TestModel", "f2", null);
            action2.AddParameter("p1", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(isNullable: true)));
            EdmOperationImport edmF2 = container.AddActionImport("f2", action2);

            var testCases = new[]
            {
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", "foo"),
                        new KeyValuePair<string, object>("p1", new ODataResource())
                    },
                    FunctionImport = edmF1,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_DuplicatedParameterNameNotAllowed", "p1"),
                },
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", "foo"),
                        new KeyValuePair<string, object>("p1", new ODataCollectionStart() { Name = "c" })
                    },
                    FunctionImport = edmF1,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_DuplicatedParameterNameNotAllowed", "p1"),
                },
                new
                {
                    ParameterPayload = new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", new ODataCollectionStart() { Name = "c" }),
                        new KeyValuePair<string, object>("p1", new ODataCollectionStart() { Name = "c" })
                    },
                    FunctionImport = edmF2,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataParameterWriterCore_DuplicatedParameterNameNotAllowed", "p1"),
                }
            };

            var testConfigurations = this.WriterTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => tc.IsRequest);
            this.CombinatorialEngineProvider.RunCombinations(testConfigurations, testCases, (testConfiguration, testCase) =>
            {
                PayloadWriterTestDescriptor.WriterTestExpectedResultCallback resultCallback = testConfig => new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings) { ExpectedException2 = testCase.ExpectedException };
                var testDescriptor = new PayloadWriterTestDescriptor<ODataParameters>(this.Settings, testCase.ParameterPayload, resultCallback)
                    {
                        Model = edmModel,
                    };

                TestWriterUtils.WriteActionAndVerifyODataPayload<ODataParameters>(
                    (messageWriter, writerDescriptor, feedWriter) =>
                    {
                        this.Settings.ObjectModelToMessageWriter.WriteMessage(messageWriter, ODataPayloadKind.Parameter, testCase.ParameterPayload, edmModel, testCase.FunctionImport);
                    },
                    testDescriptor,
                    testConfiguration,
                    this.Assert,
                    this.Logger);
            });
        }

        [TestMethod, Variation(Description = "Function import does not need to be in the model provided in the ODataMessageWriterSettings.")]
        public void FunctionImportNotInModel()
        {
            IEdmModel writerModel = TestModels.BuildTestModel();
            IEdmModel model = TestModels.BuildModelWithFunctionImport();
            IEdmOperationImport functionImport_Primitive = model.FindEntityContainer("TestContainer").FindOperationImports("FunctionImport_Primitive").First();

            var parameterPayload = new ODataParameters() 
            {
                new KeyValuePair<string, object>("primitive", "Hi")
            };

            this.CombinatorialEngineProvider.RunCombinations(
                this.WriterTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => tc.IsRequest),
                (testConfiguration) =>
                {
                    PayloadWriterTestDescriptor.WriterTestExpectedResultCallback resultCallback = testConfig => new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings) { Json = "{\"primitive\":\"Hi\"}" };
                    var testDescriptor = new PayloadWriterTestDescriptor<ODataParameters>(this.Settings, ObjectModelUtils.CreateDefaultParameter(), resultCallback) { Model = writerModel };

                    TestWriterUtils.WriteActionAndVerifyODataPayload<ODataParameters>(
                        (messageWriter, writerDescriptor, feedWriter) =>
                        {
                            this.Settings.ObjectModelToMessageWriter.WriteMessage(messageWriter, ODataPayloadKind.Parameter, parameterPayload, writerModel, functionImport_Primitive);
                        },
                        testDescriptor,
                        testConfiguration,
                        this.Assert,
                        this.Logger);
                });
        }
    }
}

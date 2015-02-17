//---------------------------------------------------------------------
// <copyright file="CollectionReaderAtomTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;

    #endregion Namespaces

    /// <summary>
    /// Tests reading of various ATOM collection payloads.
    /// </summary>
    [TestClass, TestCase]
    public class CollectionReaderAtomTests : ODataReaderTestCase
    {
        [InjectDependency]
        public IPayloadGenerator PayloadGenerator { get; set; }

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings PayloadTestDescriptorSettings { get; set; }

        [InjectDependency]
        public PayloadReaderTestExpectedResult.PayloadReaderTestExpectedResultSettings PayloadExpectedResultSettings { get; set; }

        private const string ModelNamespace = "TestModel";

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Test the reader's behavior with ATOM collection payloads.")]
        public void CollectionReaderAtomTest()
        {
            EdmModel edmModel = new EdmModel();
            EdmComplexType edmComplexTypeCity = edmModel.ComplexType("CityType", ModelNamespace);
            edmComplexTypeCity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            edmModel.Fixup();

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
            {
                #region primitive collection

                // Empty primitive collection.
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new PrimitiveCollection().CollectionName(null),
                },

                // Yet another empty collection with extra elements not in the d namespace.
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new PrimitiveCollection().CollectionName(null)
                        .XmlRepresentation("<m:value><c:foo xmlns:c='customns'>bar</c:foo></m:value>"),
                },

                // Empty collection with text inside the collection element.
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new PrimitiveCollection().CollectionName(null)
                        .XmlRepresentation("<m:value>foo</m:value>"),
                },

                // Empty collection with insignificant nodes inside the collection element.
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new PrimitiveCollection().CollectionName(null)
                        .XmlRepresentation("<m:value><!-- some comment -->  </m:value>"),
                },

                // TODO:: Currently the test infrastructure drops any top level node other than the root element. 
                // Once the test infrstructure is fixed, we should add some test to verify that insignificant nodes 
                // before the collection start are discarded. 

                // Verify that anything which is not in the d namespace, after the collection start, is discarded.
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new PrimitiveCollection(
                        new PrimitiveValue[]
                        {
                            PayloadBuilder.PrimitiveValue("vineet"),
                        }
                        ).CollectionName(null)
                        .XmlRepresentation(@"<m:value>
                                            <!-- some comment -->                    
                                            <c:foo xmlns:c='customns'>bar</c:foo>
                                            <m:element>vineet</m:element>
                                            <c:foo xmlns:c='customns'>bar</c:foo>
                                           </m:value>"),
                },

                // Verify that insignificant nodes after the collection end are discarded.
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new PrimitiveCollection().CollectionName(null)
                        .XmlRepresentation(@"
                                            <m:value>
                                            </m:value>
                                            <!-- some comment -->                    
                                             some text
                                          "),
                },

                // Primitive collection with 'element' as collection name.
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new PrimitiveCollection(
                       new PrimitiveValue[]
                        {
                            PayloadBuilder.PrimitiveValue("vineet"),
                        }
                        ).CollectionName(null),
                },

                // Collection with 'element' as collection name.
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new PrimitiveCollection(
                       new PrimitiveValue[]
                        {
                            PayloadBuilder.PrimitiveValue("redmond"),
                            PayloadBuilder.PrimitiveValue(""),
                            PayloadBuilder.PrimitiveValue("seattle"),
                        }
                        ).XmlRepresentation(@"<m:value>            
                                             <m:element>redmond</m:element>
                                             <m:element/>
                                             <m:element>seattle</m:element>
                                            </m:value>")
                        .CollectionName(null),
                },

                #endregion      

                #region complex collection


                // yet another empty complex collection
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new ComplexInstanceCollection(
                        PayloadBuilder.ComplexValue("TestModel.CityType").PrimitiveProperty("Name", null)
                        ).CollectionName(null),
                },

                // Verify that anything which is not in the d namespace, after the collection start, is discarded.
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new ComplexInstanceCollection(
                       PayloadBuilder.ComplexValue("TestModel.CityType").Property("City", PayloadBuilder.PrimitiveValue("Vienna"))
                       ).CollectionName(null)
                       .XmlRepresentation(@"<m:value>
                                            <!-- some comment -->                    
                                            <c:foo xmlns:c='customns'>bar</c:foo>
                                            <m:element m:type='TestModel.CityType'>
                                                <c:foo xmlns:c='customns'>bar</c:foo>
                                                <d:City>Vienna</d:City>
                                                <!-- some more comments -->                            
                                            </m:element>
                                            <c:foo xmlns:c='customns'>bar</c:foo>
                                           </m:value>"),
                },

                // Verify that insignificant nodes after the collection end are discarded.
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new ComplexInstanceCollection(
                        PayloadBuilder.ComplexValue("TestModel.CityType").Property("City", PayloadBuilder.PrimitiveValue("Vienna")))
                        .CollectionName(null)
                        .XmlRepresentation(@"
                                            <m:value>
                                               <m:element m:type='TestModel.CityType'>
                                                <d:City>Vienna</d:City>
                                               </m:element>
                                            </m:value>
                                            <!-- some comment -->                    
                                             some text
                                         "),
                },

                #endregion

                #region no metadata

                // primitive collection with no expected type and no metadata.
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new PrimitiveCollection(
                        PayloadBuilder.PrimitiveValue(1),
                        PayloadBuilder.PrimitiveValue(2)
                        ).CollectionName(null),
                },

                // complex collection with no expected type and no metadata.
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new ComplexInstanceCollection(
                        PayloadBuilder.ComplexValue("TestModel.CityType").Property("Name", PayloadBuilder.PrimitiveValue("Vienna")),
                        PayloadBuilder.ComplexValue("TestModel.CityType").Property("Name", PayloadBuilder.PrimitiveValue("Am Euro Platz"))
                        ).CollectionName(null),
                },

                #endregion
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Test the reader's behavior with invalid ATOM collection payloads.")]
        public void CollectionReaderAtomErrorTest()
        {
            EdmModel edmModel = new EdmModel();

            EdmComplexType edmComplexTypeEmpty = new EdmComplexType(ModelNamespace, "EmptyComplexType");
            edmModel.AddElement(edmComplexTypeEmpty);

            EdmComplexType edmComplexTypeCity = new EdmComplexType(ModelNamespace, "CityType");
            edmComplexTypeCity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            edmModel.AddElement(edmComplexTypeCity);

            EdmComplexType edmComplexTypeAddress = new EdmComplexType(ModelNamespace, "AddressType");
            edmComplexTypeAddress.AddStructuralProperty("Street", EdmCoreModel.Instance.GetString(true));
            edmModel.AddElement(edmComplexTypeAddress);

            edmModel.Fixup();

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
            {
                // Verify that collections do not support top level collection type.
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new ComplexInstanceCollection().CollectionName(null)
                        .XmlRepresentation(@"<m:value>
                                           <m:element m:type='" + EntityModelUtils.GetCollectionTypeName("Edm.Int32") + @"'>
                                                <m:element>42</m:element>
                                           </m:element>
                                           </m:value>"),
                    PayloadEdmModel = edmModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_InvalidItemTypeKind", "Collection"),
                    SkipTestConfiguration = tc => tc.Version < Microsoft.OData.Core.ODataVersion.V4,
                },

                // Verify that collection inside complex type is not supported.
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new  ComplexInstanceCollection().CollectionName(null)
                        .XmlRepresentation(@"<m:value>
                                               <m:element>
                                                <m:element m:type='" + EntityModelUtils.GetCollectionTypeName("Edm.Int32") + @"'>
                                                 <m:element>42</m:element>
                                                </m:element>
                                               </m:element>
                                             </m:value>"),
                    PayloadEdmModel = null, // No model, since otherwise we would fail to read the top-level item as it has no type information
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_InvalidItemTypeKind", "Collection"),
                    SkipTestConfiguration = tc => tc.Version < Microsoft.OData.Core.ODataVersion.V4,
                },

                // Collection with m:type attribute in the root collection element.
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new PrimitiveCollection().CollectionName(null)
                        .XmlRepresentation("<m:value m:type='Edm.Int32'></m:value>"),
                    PayloadEdmModel = edmModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomCollectionDeserializer_TypeOrNullAttributeNotAllowed"),
                },

                // Collection with m:null attribute in the Collection element.
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new PrimitiveCollection().CollectionName(null)
                        .XmlRepresentation("<m:value m:null='true'></m:value>"),
                    PayloadEdmModel = edmModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomCollectionDeserializer_TypeOrNullAttributeNotAllowed"),
                },

                // Collection with both m:type and m:null attribute in the Collection element.
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new PrimitiveCollection().CollectionName(null)
                        .XmlRepresentation("<m:value m:null='true' m:type='Edm.Int32'></m:value>"),
                    PayloadEdmModel = edmModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomCollectionDeserializer_TypeOrNullAttributeNotAllowed"),
                },

                // root collection element not in the d namespace.
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new PrimitiveCollection().CollectionName("PrimitiveCollection")
                        .XmlRepresentation("<d:PrimitiveCollection></d:PrimitiveCollection>"),
                    PayloadEdmModel = edmModel,  
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomCollectionDeserializer_TopLevelCollectionElementWrongNamespace", "http://docs.oasis-open.org/odata/ns/data", "http://docs.oasis-open.org/odata/ns/metadata"),
                },

                // complex value instead of collection.
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new PrimitiveCollection().CollectionName(null)
                        .XmlRepresentation(@"<m:value>
                                              <m:city>Seattle</m:city>
                                             </m:value>"),
                    PayloadEdmModel = edmModel, 
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomCollectionDeserializer_WrongCollectionItemElementName", "city", "http://docs.oasis-open.org/odata/ns/data", "http://docs.oasis-open.org/odata/ns/data"),
                },

                // complex collection with expected type and no metadata.
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                     PayloadElement = new ComplexInstanceCollection(
                        PayloadBuilder.ComplexValue("TestModel.CityType").Property("Name", PayloadBuilder.PrimitiveValue("Vienna")),
                        PayloadBuilder.ComplexValue("TestModel.CityType").Property("Name", PayloadBuilder.PrimitiveValue("Am Euro Platz"))
                        ).ExpectedCollectionItemType(edmComplexTypeCity).CollectionName(null),

                     ExpectedResultCallback = tc =>
                        new PayloadReaderTestExpectedResult(this.PayloadExpectedResultSettings)
                        {
                            // There was no simple way to specify the complex type as the expected type, so specifying a primitive type as the
                            // expected type. Since the expected type is only used to trigger the exception any expected type works.
                            ReaderMetadata = new PayloadReaderTestDescriptor.ReaderMetadata(EdmCoreModel.Instance.GetInt32(false)),
                            ExpectedException = ODataExpectedExceptions.ArgumentException("ODataMessageReader_ExpectedTypeSpecifiedWithoutMetadata", "expectedItemTypeReference"),
                        }
                },

                // primitive collection with expected type and no metadata.
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new PrimitiveCollection(
                        PayloadBuilder.PrimitiveValue(1),
                        PayloadBuilder.PrimitiveValue(2)
                        ).ExpectedCollectionItemType(EdmDataTypes.Int32).CollectionName(null),
                    ExpectedException = ODataExpectedExceptions.ArgumentException("ODataMessageReader_ExpectedTypeSpecifiedWithoutMetadata", "expectedItemTypeReference"),
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Test the the reading of heterogeneous ATOM collection payloads.")]
        public void HeterogeneousCollectionReaderAtomTest()
        {
            EdmModel edmModel = new EdmModel();

            EdmComplexType edmComplexTypeEmpty = new EdmComplexType(ModelNamespace, "EmptyComplexType");
            edmModel.AddElement(edmComplexTypeEmpty);

            EdmComplexType edmComplexTypeCity = new EdmComplexType(ModelNamespace, "CityType");
            edmComplexTypeCity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            edmModel.AddElement(edmComplexTypeCity);

            EdmComplexType edmComplexTypeAddress = new EdmComplexType(ModelNamespace, "AddressType");
            edmComplexTypeAddress.AddStructuralProperty("Street", EdmCoreModel.Instance.GetString(true));
            edmModel.AddElement(edmComplexTypeAddress);

            edmModel.Fixup();

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
            {
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new ComplexInstanceCollection(
                        PayloadBuilder.ComplexValue("TestModel.CityType").Property("Name", PayloadBuilder.PrimitiveValue("Vienna")),
                        PayloadBuilder.ComplexValue("TestModel.AddressType").Property("Street", PayloadBuilder.PrimitiveValue("Am Euro Platz"))
                        ).ExpectedCollectionItemType(edmComplexTypeCity).CollectionName(null),
                    PayloadEdmModel = edmModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncompatibleType", "TestModel.AddressType", "TestModel.CityType"),
                },

                // primitive collection with primitive and complex elements (with metadata, with expected types)
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveCollection()
                        .ExpectedCollectionItemType(EdmDataTypes.String())
                        .XmlRepresentation(@"<m:value>
                                                <m:element m:type='Edm.String'>Foo</m:element>
                                                <m:element m:type='TestModel.CityType'>
                                                    <d:Name m:type='Edm.String'>Perth</d:Name>
                                                </m:element>
                                             </m:value>"),
                    PayloadEdmModel = edmModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKind", "TestModel.CityType", "Primitive", "Complex"),
                },

                // complex collection with complex and primitive elements (with metadata, with expected types)
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = PayloadBuilder.ComplexCollection()
                        .ExpectedCollectionItemType(edmComplexTypeCity)
                        .XmlRepresentation(@"<m:value>
                                                <m:element m:type='TestModel.CityType'>
                                                    <d:Name m:type='Edm.String'>Perth</d:Name>
                                                </m:element>
                                                <m:element m:type='Edm.Int32'>123</m:element>
                                             </m:value>"),
                    PayloadEdmModel = edmModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKind", "Edm.Int32", "Complex", "Primitive"),
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
            testDescriptors,
            this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
            (testDescriptor, testConfiguration) =>
            {
                testDescriptor.RunTest(testConfiguration);
            });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Test the the reading of heterogeneous ATOM collection payloads.")]
        public void HeterogeneousCollectionReaderWithoutMetadataAtomTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
            {
                // Collection with different item type kinds (complex instead of primitive)
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveCollection()
                        .XmlRepresentation(@"<m:value>
                                                <m:element>0</m:element>
                                                <m:element>
                                                    <d:bar>2</d:bar>
                                                </m:element>
                                            </m:value>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind", "Complex", "Primitive"),
                },

                // Collection where item type kind does not match item type name (primitive and complex items)
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = PayloadBuilder.ComplexCollection()
                        .XmlRepresentation(@"<m:value>
                                                <m:element></m:element>
                                                <m:element>
                                                    <d:bar>2</d:bar>
                                                </m:element>
                                            </m:value>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind", "Complex", "Primitive"),
                },

                // Collection where item type names don't match (Edm.String and Edm.Int32)
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = PayloadBuilder.ComplexCollection()
                        .XmlRepresentation(@"<m:value>
                                                <m:element></m:element>
                                                <m:element m:type='Edm.Int32'>2</m:element>
                                            </m:value>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "Edm.Int32", "Edm.String"),
                },

                // Collection where item type names don't match (Edm.String and Edm.Int32); including some null items
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = PayloadBuilder.ComplexCollection()
                        .XmlRepresentation(@"<m:value>
                                                <m:element m:null='true' />
                                                <m:element></m:element>
                                                <m:element m:null='true' />
                                                <m:element m:type='Edm.Int32'>2</m:element>
                                                <m:element m:null='true' />
                                            </m:value>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "Edm.Int32", "Edm.String"),
                },

                // Collection where item type names don't match (TestModel.SomeComplexType and TestModel.OtherComplexType)
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = PayloadBuilder.ComplexCollection()
                        .XmlRepresentation(@"<m:value>
                                                <m:element m:type='TestModel.SomeComplexType'><d:StringProperty>abc</d:StringProperty></m:element>
                                                <m:element m:type='TestModel.OtherComplexType'><d:IntProperty>42</d:IntProperty></m:element>
                                            </m:value>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "TestModel.OtherComplexType", "TestModel.SomeComplexType"),
                },

                // Collection where item type names don't match (TestModel.SomeComplexType and TestModel.OtherComplexType); including some null items
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = PayloadBuilder.ComplexCollection()
                        .XmlRepresentation(@"<m:value>
                                                <m:element m:null='true' />
                                                <m:element m:type='TestModel.SomeComplexType'><d:StringProperty>abc</d:StringProperty></m:element>
                                                <m:element m:null='true' />
                                                <m:element m:type='TestModel.OtherComplexType'><d:IntProperty>42</d:IntProperty></m:element>
                                                <m:element m:null='true' />
                                            </m:value>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "TestModel.OtherComplexType", "TestModel.SomeComplexType"),
                },

                // Collection where different item type kinds (primitive instead of complex)
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = PayloadBuilder.ComplexCollection()
                        .XmlRepresentation(@"<m:value>
                                                <m:element m:type='TestModel.SomeComplexType'>
                                                    <d:bar>2</d:bar>
                                                </m:element>
                                                <m:element>0</m:element>
                                            </m:value>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind", "Primitive", "Complex"),
                },

                // Collection with primitive and complex elements
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveCollection()
                        .XmlRepresentation(@"<m:value>
                                                <m:element m:type='Edm.String'>Foo</m:element>
                                                <m:element m:type='TestModel.CityType'>
                                                    <d:Name m:type='Edm.String'>Perth</d:Name>
                                                </m:element>
                                            </m:value>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind", "Complex", "Primitive"),
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
            testDescriptors,
            this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
            (testDescriptor, testConfiguration) =>
            {
                testDescriptor.RunTest(testConfiguration);
            });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Test the the reading of homogeneous ATOM collection payloads.")]
        public void HomogeneousCollectionReaderAtomTest()
        {
            EdmModel edmModel = new EdmModel();

            EdmComplexType edmComplexTypeEmpty = new EdmComplexType(ModelNamespace, "EmptyComplexType");
            edmModel.AddElement(edmComplexTypeEmpty);

            EdmComplexType edmComplexTypeCity = new EdmComplexType(ModelNamespace, "CityType");
            edmComplexTypeCity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            edmModel.AddElement(edmComplexTypeCity);

            EdmComplexType edmComplexTypeAddress = new EdmComplexType(ModelNamespace, "AddressType");
            edmComplexTypeAddress.AddStructuralProperty("Street", EdmCoreModel.Instance.GetString(true));
            edmModel.AddElement(edmComplexTypeAddress);

            edmModel.Fixup();

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                // complex collection with primitive expected type
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new ComplexInstanceCollection(
                        PayloadBuilder.ComplexValue("TestModel.CityType").Property("Name", PayloadBuilder.PrimitiveValue("Vienna")),
                        PayloadBuilder.ComplexValue("TestModel.CityType").Property("Name", PayloadBuilder.PrimitiveValue("Prague"))
                        ).ExpectedCollectionItemType(EdmDataTypes.Int32).CollectionName(null),
                    PayloadEdmModel = edmModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKind", "TestModel.CityType", "Primitive", "Complex"),
                },
                
                // primitive collection in XMLRepresentation with complex expected type. 
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new ComplexInstanceCollection(
                        PayloadBuilder.ComplexValue("TestModel.CityType"),
                        PayloadBuilder.ComplexValue("TestModel.CityType")
                        ).ExpectedCollectionItemType(edmComplexTypeCity).CollectionName(null)
                        .XmlRepresentation(@"
                                            <m:value>
                                                <m:element m:type='Edm.Int32'>1</m:element>
                                                <m:element m:type='Edm.Int32'>2</m:element>
                                            </m:value>"),
                    PayloadEdmModel = edmModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKind", "Edm.Int32", "Complex", "Primitive"),
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Test the the reading of homogeneous ATOM collection payloads.")]
        public void HomogeneousCollectionReaderWithoutMetadataAtomTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
            {
                // Primitive collection with only nulls
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new PrimitiveCollection(
                        PayloadBuilder.PrimitiveValue(null),
                        PayloadBuilder.PrimitiveValue(null)
                        ).CollectionName(null),
                },

                // Primitive collection with type names on string values
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new PrimitiveCollection(
                        PayloadBuilder.PrimitiveValue("foo"),
                        PayloadBuilder.PrimitiveValue("bar")
                        ).CollectionName(null),
                },

                // Primitive collection with type names on Int32 values
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new PrimitiveCollection(
                        PayloadBuilder.PrimitiveValue(1),
                        PayloadBuilder.PrimitiveValue(2)
                        ).CollectionName(null),
                },

                // Primitive collection without type names on some string values
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new PrimitiveCollection(
                        new PrimitiveValue("Edm.String", "foo"),
                        new PrimitiveValue(/*fullTypeName*/ null, "bar")
                        ).CollectionName(null),
                },

                // Primitive collection without type names on string values
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new PrimitiveCollection(
                        new PrimitiveValue(/*fullTypeName*/ null, "foo"),
                        new PrimitiveValue(/*fullTypeName*/ null, "bar")
                        ).CollectionName(null),
                },

                // Primitive collection with type names on Int32 values and null values
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new PrimitiveCollection(
                        PayloadBuilder.PrimitiveValue(1),
                        PayloadBuilder.PrimitiveValue(null),
                        PayloadBuilder.PrimitiveValue(2),
                        PayloadBuilder.PrimitiveValue(null)
                        ).CollectionName(null),
                },

                // Complex collection with type names on complex values
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new ComplexInstanceCollection(
                        PayloadBuilder.ComplexValue("TestModel.ComplexType").PrimitiveProperty("StringProperty", "abc"),
                        PayloadBuilder.ComplexValue("TestModel.ComplexType").PrimitiveProperty("StringProperty", "123")
                        ).CollectionName(null),
                },

                // Complex collection with type names on complex values and null values
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new ComplexInstanceCollection(
                        PayloadBuilder.ComplexValue("TestModel.ComplexType").PrimitiveProperty("StringProperty", "abc"),
                        PayloadBuilder.ComplexValue(null, /*isNull*/ true),
                        PayloadBuilder.ComplexValue("TestModel.ComplexType").PrimitiveProperty("StringProperty", "123")
                        ).CollectionName(null),
                },

                // Complex collection without type names on complex values
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new ComplexInstanceCollection(
                        PayloadBuilder.ComplexValue().PrimitiveProperty("Name", "Clemens"),
                        PayloadBuilder.ComplexValue().PrimitiveProperty("Name", "Vitek")
                        ).CollectionName(null),
                },

                // Complex collection without type names on complex values and null values
                new PayloadReaderTestDescriptor(this.PayloadTestDescriptorSettings)
                {
                    PayloadElement = new ComplexInstanceCollection(
                        PayloadBuilder.ComplexValue().PrimitiveProperty("FirstName", "Clemens"),
                        PayloadBuilder.ComplexValue(null, /*isNull*/ true),
                        PayloadBuilder.ComplexValue().PrimitiveProperty("LastName", "Kerer")
                        ).CollectionName(null),
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }
    }
}

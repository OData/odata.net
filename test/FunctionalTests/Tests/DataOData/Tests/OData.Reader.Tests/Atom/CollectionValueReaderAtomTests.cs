//---------------------------------------------------------------------
// <copyright file="CollectionValueReaderAtomTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Contracts.Atom;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of collections in ATOM.
    /// </summary>
    [TestClass, TestCase]
    public class CollectionValueReaderAtomTests : ODataReaderTestCase
    {
        private const string ModelNamespace = "TestModel";

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct parsing of ATOM collections with metadata or test which don't really care about metadata.")]
        public void CollectionValueTest()
        {
            EdmModel edmModel = new EdmModel();
            EdmComplexType edmComplexTypeMyComplexType = edmModel.ComplexType("MyComplexType", ModelNamespace);
            edmComplexTypeMyComplexType.Property("P1", EdmPrimitiveTypeKind.Int32,false).Property("P2",EdmPrimitiveTypeKind.String,true);
            edmModel.Fixup();

            var edmCollectionTypeOfIntegerType = new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false));
            var edmCollectionTypeOfMyComplexType = new EdmCollectionType(new EdmComplexTypeReference(edmComplexTypeMyComplexType,true));
            // Create payloads of the collection properties
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
            {
                // Empty element collection
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue(EntityModelUtils.GetCollectionTypeName("Edm.Int32"))
                        .XmlValueRepresentation(new XNode[0])
                        .WithTypeAnnotation(edmCollectionTypeOfIntegerType),
                    PayloadEdmModel = edmModel,
                },
                // Collections containing collections are not supported
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue()
                        .XmlValueRepresentation("<m:element />", EntityModelUtils.GetCollectionTypeName(EntityModelUtils.GetCollectionTypeName("Edm.String")), null)
                        .WithTypeAnnotation(edmCollectionTypeOfIntegerType),
                    PayloadEdmModel = edmModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_NestedCollectionsAreNotSupported"),
                },
            };

            var primitivePayloadsWithPadding = new CollectionPaddingPayloadTextCase<PrimitiveMultiValue>[]
            {
                new CollectionPaddingPayloadTextCase<PrimitiveMultiValue>
                {
                    XmlValue = "{0}",
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue(EntityModelUtils.GetCollectionTypeName("Edm.Int32"))
                        .WithTypeAnnotation(edmCollectionTypeOfIntegerType)
                },
                new CollectionPaddingPayloadTextCase<PrimitiveMultiValue>
                {
                    XmlValue = "{0}<m:element>42</m:element>",
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue(EntityModelUtils.GetCollectionTypeName("Edm.Int32"))
                        .Item(42)
                        .WithTypeAnnotation(edmCollectionTypeOfIntegerType)
                },
                new CollectionPaddingPayloadTextCase<PrimitiveMultiValue>
                {
                    XmlValue = "<m:element>42</m:element>{0}",
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue(EntityModelUtils.GetCollectionTypeName("Edm.Int32"))
                        .Item(42)
                        .WithTypeAnnotation(edmCollectionTypeOfIntegerType)
                },
            };

            var complexPayloadsWithPadding = new CollectionPaddingPayloadTextCase<ComplexMultiValue>[]
            {
                new CollectionPaddingPayloadTextCase<ComplexMultiValue>
                {
                    XmlValue = "<m:element><d:P1>42</d:P1></m:element>{0}<m:element><d:P2>test</d:P2></m:element>",
                    PayloadElement = PayloadBuilder.ComplexMultiValue(EntityModelUtils.GetCollectionTypeName("TestModel.MyComplexType"))
                        .Item(PayloadBuilder.ComplexValue("TestModel.MyComplexType").PrimitiveProperty("P1", 42).AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null }))
                        .Item(PayloadBuilder.ComplexValue("TestModel.MyComplexType").PrimitiveProperty("P2", "test").AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null }))
                        .WithTypeAnnotation(edmCollectionTypeOfMyComplexType)
                },
            };

            var xmlPadding = new CollectionXmlPadding[]
            {
                // Nothing
                new CollectionXmlPadding
                {
                    Padding = string.Empty,
                },
                // Whitespace only
                new CollectionXmlPadding
                {
                    Padding = "  \r\n\t",
                },
                // Insignificant nodes
                new CollectionXmlPadding
                {
                    Padding = "<!--s--> <?value?>",
                },
                // Element in no namespace
                new CollectionXmlPadding
                {
                    Padding = "<foo xmlns=''/>",
                },
                // Element in custom namespace
                new CollectionXmlPadding
                {
                    Padding = "<c:foo xmlns:c='uri' attr='1'><c:child/>text</c:foo>",
                },
                // Element in metadata namespace (should be ignored as well)
                new CollectionXmlPadding
                {
                    Padding = "<m:properties/>",
                },
                // Element in atom namespace (should also be ignored)
                new CollectionXmlPadding
                {
                    Padding = "<entry/>",
                },
                // Significant nodes - will be ignored
                new CollectionXmlPadding
                {
                    Padding = "some text <![CDATA[cdata]]>",
                },
                // Element in the d namespace should fail
                new CollectionXmlPadding
                {
                    Padding = "<d:foo/>",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomPropertyAndValueDeserializer_InvalidCollectionElement", "foo", "http://docs.oasis-open.org/odata/ns/metadata"),
                },
                // Element in the d namespace should fail (wrong name)
                new CollectionXmlPadding
                {
                    Padding = "<d:Element/>",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomPropertyAndValueDeserializer_InvalidCollectionElement", "Element", "http://docs.oasis-open.org/odata/ns/metadata"),
                },
            };

            testDescriptors = testDescriptors.Concat(this.CreatePayloadWithPaddingTestDescriptor(
                primitivePayloadsWithPadding, xmlPadding, edmModel));

            testDescriptors = testDescriptors.Concat(this.CreatePayloadWithPaddingTestDescriptor(
                complexPayloadsWithPadding, xmlPadding, edmModel));

            testDescriptors = testDescriptors.Select(td => td.InProperty());

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct parsing of ATOM collections without metadata.")]
        public void CollectionNoMetadataTest()
        {
            string collectionOfStringTypeName = EntityModelUtils.GetCollectionTypeName("Edm.String");
            string collectionOfInt32TypeName = EntityModelUtils.GetCollectionTypeName("Edm.Int32");
            string collectionOfComplexType = EntityModelUtils.GetCollectionTypeName("TestModel.ComplexType");

            // Create payloads of the collection properties
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
            {
                // Single empty m:element in >=V3
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue()
                        .Item(string.Empty)
                        .XmlValueRepresentation("<m:element/>", null, null),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // Single m:element with no content in >= V3
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue()
                        .Item(string.Empty)
                        .XmlValueRepresentation("<m:element></m:element>", null, null),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // Two m:element items in >= V3
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue()
                        .Item(string.Empty)
                        .Item("foo")
                        .XmlValueRepresentation("<m:element></m:element><m:element>foo</m:element>", null, null),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // Two m:element items with insiginificant nodes >= V3
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue()
                        .Item(string.Empty)
                        .Item("foo")
                        .XmlValueRepresentation("  <m:element></m:element>\r\n<?value?><m:element>foo</m:element><!--some-->", null, null),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // Two m:element items with text nodes >= V3
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue()
                        .Item(string.Empty)
                        .Item("foo")
                        .XmlValueRepresentation("foo<m:element></m:element>bar<m:element>foo</m:element>", null, null),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // Two m:element items with custom element >= V3
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue()
                        .Item(string.Empty)
                        .Item("foo")
                        .XmlValueRepresentation("<m:foo/><m:element></m:element><m:bar>some</m:bar><m:element>foo</m:element>", null, null),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // Two m:element items with custom element with m:element in it (which is to be ignored)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue()
                        .Item(string.Empty)
                        .Item("foo")
                        .XmlValueRepresentation("<m:foo/><m:element></m:element><m:bar><m:element/></m:bar><m:element>foo</m:element>", null, null),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // Two m:element items with custom element with d:prop in it (which is to be ignored)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue()
                        .Item(string.Empty)
                        .Item("foo")
                        .XmlValueRepresentation("<m:foo/><m:element></m:element><m:bar><d:prop/></m:bar><m:element>foo</m:element>", null, null),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // Nested m:element items (error)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue()
                        .XmlValueRepresentation("<m:element><m:element>0</m:element><m:element>1</m:element></m:element>", null, null),
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_InvalidItemTypeKind", "Collection"),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },

                #region No collection type name tests
                // No collection type name and string items (with and without type names)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue().Item("foo").Item(new PrimitiveValue(/*fullTypeName*/ null, "bar"))
                        .XmlValueRepresentation("<m:element m:type='Edm.String'>foo</m:element><m:element>bar</m:element>", null, null),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // No collection type name and Int32 items
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue().Item(1).Item(2)
                        .XmlValueRepresentation("<m:element m:type='Edm.Int32'>1</m:element><m:element m:type='Edm.Int32'>2</m:element>", null, null),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // No collection type name and complex items
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexMultiValue()
                                .Item(PayloadBuilder.ComplexValue("TestModel.ComplexType").PrimitiveProperty("StringProperty", "abc"))
                                .Item(PayloadBuilder.ComplexValue("TestModel.ComplexType").PrimitiveProperty("StringProperty", "123"))
                        .XmlValueRepresentation("<m:element m:type='TestModel.ComplexType'><d:StringProperty>abc</d:StringProperty></m:element><m:element m:type='TestModel.ComplexType'><d:StringProperty>123</d:StringProperty></m:element>", null, null),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // No collection type name and different item type kinds (complex instead of primitive)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue()
                        .XmlValueRepresentation("<m:element>0</m:element><m:element><d:bar>2</d:bar></m:element>", null, null),
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind", "Complex", "Primitive"),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // No collection type name and item type kind does not match item type name (primitive and complex items)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexMultiValue()
                        .XmlValueRepresentation("<m:element></m:element><m:element><d:bar>2</d:bar></m:element>", null, null),
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind", "Complex", "Primitive"),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // No collection type name and item type names don't match (Edm.String and Edm.Int32)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexMultiValue()
                        .XmlValueRepresentation("<m:element></m:element><m:element m:type='Edm.Int32'>2</m:element>", null, null),
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "Edm.Int32", "Edm.String"),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // No collection type name and item type names don't match (Edm.String and Edm.Int32); including some null items
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexMultiValue()
                        .XmlValueRepresentation("<m:element m:null='true' /><m:element></m:element><m:element m:null='true' /><m:element m:type='Edm.Int32'>2</m:element><m:element m:null='true' />", null, null),
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "Edm.Int32", "Edm.String"),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // No collection type name and item type names don't match (TestModel.SomeComplexType and TestModel.OtherComplexType)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexMultiValue()
                        .XmlValueRepresentation("<m:element m:type='TestModel.SomeComplexType'><d:SomeProperty /></m:element><m:element m:type='TestModel.OtherComplexType'><d:OtherProperty /></m:element>", null, null),
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "TestModel.OtherComplexType", "TestModel.SomeComplexType"),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // No collection type name and item type names don't match (TestModel.SomeComplexType and TestModel.OtherComplexType); including some null items
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexMultiValue()
                        .XmlValueRepresentation("<m:element m:null='true' /><m:element m:type='TestModel.SomeComplexType'></m:element><m:element m:null='true' /><m:element m:type='TestModel.OtherComplexType'></m:element><m:element m:null='true' />", null, null),
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "TestModel.OtherComplexType", "TestModel.SomeComplexType"),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // No collection type name and different item type kinds (primitive instead of complex)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexMultiValue()
                        .XmlValueRepresentation("<m:element m:type='TestModel.SomeComplexType'><m:bar>2</m:bar></m:element><m:element>0</m:element>", null, null),
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind", "Primitive", "Complex"),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },

                #endregion No collection type name tests

                #region Collection with type name tests
                // Collection with type name and string items (without type names) >= V3
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue(collectionOfStringTypeName).Item("test")
                        .XmlValueRepresentation("<m:element>test</m:element>", collectionOfStringTypeName, null),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // Collection with type name and string items (with and without type names)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue(collectionOfStringTypeName).Item("foo").Item(new PrimitiveValue(/*fullTypeName*/ null, "bar"))
                        .XmlValueRepresentation("<m:element m:type='Edm.String'>foo</m:element><m:element>bar</m:element>", collectionOfStringTypeName, null),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // Collection with type name and Int32 items (with type names)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue(collectionOfInt32TypeName).Item(1).Item(2)
                        .XmlValueRepresentation("<m:element m:type='Edm.Int32'>1</m:element><m:element m:type='Edm.Int32'>2</m:element>", collectionOfInt32TypeName, null),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // Collection with type name and Int32 items (without type names)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue(collectionOfInt32TypeName)
                        .Item(new PrimitiveValue(/*fullTypeName*/ null, 1))
                        .Item(new PrimitiveValue(/*fullTypeName*/ null, 2))
                        .XmlValueRepresentation("<m:element>1</m:element><m:element>2</m:element>", collectionOfInt32TypeName, null),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // Collection with type name and complex items (with type names)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexMultiValue(collectionOfComplexType)
                        .Item(PayloadBuilder.ComplexValue("TestModel.ComplexType").PrimitiveProperty("bar", "1"))
                        .Item(PayloadBuilder.ComplexValue("TestModel.ComplexType").PrimitiveProperty("bar", "2"))
                        .XmlValueRepresentation("<m:element m:type='TestModel.ComplexType'><d:bar>1</d:bar></m:element><m:element m:type='TestModel.ComplexType'><d:bar>2</d:bar></m:element>", collectionOfComplexType, null),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // Collection with type name and complex items (without type names)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexMultiValue(collectionOfComplexType)
                        .Item(PayloadBuilder.ComplexValue("TestModel.ComplexType").PrimitiveProperty("bar", "1").AddAnnotation(new SerializationTypeNameTestAnnotation { TypeName = null }))
                        .Item(PayloadBuilder.ComplexValue("TestModel.ComplexType").PrimitiveProperty("bar", "2").AddAnnotation(new SerializationTypeNameTestAnnotation { TypeName = null }))
                        .XmlValueRepresentation("<m:element><d:bar>1</d:bar></m:element><m:element><d:bar>2</d:bar></m:element>", collectionOfComplexType, null),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // Primitive collection with type name and different item type kinds (complex instead of primitive)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue(collectionOfInt32TypeName)
                        .XmlValueRepresentation("<m:element m:type='Edm.Int32'>0</m:element><m:element m:type='TestModel.SomeComplexType'><d:bar>2</d:bar></m:element>", collectionOfInt32TypeName, null),
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind", "Complex", "Primitive"),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // Primitive collection with type name and different item type kinds (invalid element in primitive)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue(collectionOfInt32TypeName)
                        .XmlValueRepresentation("<m:element m:type='Edm.Int32'>0</m:element><m:element><d:bar>2</d:bar></m:element>", collectionOfInt32TypeName, null),
                    ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element"),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // Complex collection type with type name and different item type kinds (primitive instead of complex)
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexMultiValue(collectionOfComplexType)
                        .XmlValueRepresentation("<m:element m:type='TestModel.ComplexType'><d:bar>2</d:bar></m:element><m:element m:type='Edm.Int32'>0</m:element>", collectionOfComplexType, null),
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind", "Primitive", "Complex"),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // Complex collection type with type name and mixed content - primitive value at the same level as complex value
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexMultiValue(collectionOfComplexType)
                        .Item(PayloadBuilder.ComplexValue("TestModel.ComplexType").PrimitiveProperty("bar", "2").AddAnnotation(new SerializationTypeNameTestAnnotation { TypeName = null }))
                        .Item(PayloadBuilder.ComplexValue("TestModel.ComplexType").PrimitiveProperty("bar", "3").AddAnnotation(new SerializationTypeNameTestAnnotation { TypeName = null }))
                        .XmlValueRepresentation("<m:element><d:bar>2</d:bar></m:element><m:element>0<d:bar>3</d:bar></m:element>", collectionOfComplexType, null),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // Primitive collection with type name and inconsistent item type names
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue(collectionOfInt32TypeName).Item(new PrimitiveValue(/*fullTypeName*/ null, 0)).Item(1)
                        .XmlValueRepresentation("<m:element>0</m:element><m:element m:type='Edm.Int32'>1</m:element>", collectionOfInt32TypeName, null),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                // Complex collection type with type name and inconsistent item type names
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexMultiValue(collectionOfComplexType)
                        .XmlValueRepresentation("<m:element m:type='TestModel.SomeComplexType'><d:foo>1</d:foo></m:element><m:element><d:bar>2</d:bar></m:element>", collectionOfComplexType, null),
                    ExpectedException = ODataExpectedExceptions.ODataException("CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName", "TestModel.SomeComplexType", "TestModel.ComplexType"),
                    SkipTestConfiguration = tc => tc.Version < ODataVersion.V4
                },
                #endregion Collection with type name and inconsistent payload items
            };

            // Wrap it in property (manually to prevent any type annotations)
            testDescriptors = testDescriptors.Select(td => td.InProperty());

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description="Verifies correct handling of ATOM collections containing heterogenous items")]
        public void CollectionWithHeterogenousItemsTest()
        {
            EdmModel edmModel = new EdmModel();
            EdmComplexType edmComplexTypeMyComplexType = edmModel.ComplexType("MyComplexType", ModelNamespace);
            edmComplexTypeMyComplexType.Property("P1", EdmPrimitiveTypeKind.Int32, false).Property("P2", EdmPrimitiveTypeKind.String, true);
            edmModel.Fixup();

            var edmCollectionTypeOfIntegerType = new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false));
            var edmCollectionTypeOfStringType = new EdmCollectionType(EdmCoreModel.Instance.GetString(true));

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
            {
                // Primitive collection with complex item
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue(EntityModelUtils.GetCollectionTypeName("Edm.String"))
                        .XmlValueRepresentation("<m:element><d:P1>0</d:P1><d:P2>Foo</d:P2></m:element>", EntityModelUtils.GetCollectionTypeName("Edm.String"), null)
                        .WithTypeAnnotation(edmCollectionTypeOfStringType),
                    PayloadEdmModel = edmModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element"),
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue(EntityModelUtils.GetCollectionTypeName("Edm.Int32"))
                        .XmlValueRepresentation("<m:element>0</m:element><m:element><d:P1>0</d:P1><d:P2>Foo</d:P2></m:element>", EntityModelUtils.GetCollectionTypeName("Edm.Int32"), null)
                        .WithTypeAnnotation(edmCollectionTypeOfIntegerType),
                    PayloadEdmModel = edmModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element"),
                },
                // Complex collection with primitive item
                // Note - the text nodes (of the primitive items) are ignored in complex values - leaving empty complex values
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexMultiValue(EntityModelUtils.GetCollectionTypeName("TestModel.MyComplexType"))
                        .Item(PayloadBuilder.ComplexValue("TestModel.MyComplexType").PrimitiveProperty("P1", 987).AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null }))
                        .Item(PayloadBuilder.ComplexValue("TestModel.MyComplexType").PrimitiveProperty("P1", 123).AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null }))
                        .XmlValueRepresentation("<m:element>Foo<d:P1>987</d:P1></m:element><m:element><d:P1>123</d:P1>Bar</m:element>", EntityModelUtils.GetCollectionTypeName("TestModel.MyComplexType"), null),
                    PayloadEdmModel = edmModel,
                },
                // Primitive collection containing a primitive and a nested collection
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.PrimitiveMultiValue(EntityModelUtils.GetCollectionTypeName("Edm.String"))
                        .XmlValueRepresentation("<m:element>Foo</m:element><m:element><m:element>0</m:element><m:element>1</m:element></m:element>", EntityModelUtils.GetCollectionTypeName("Edm.String"), null)
                        .WithTypeAnnotation(edmCollectionTypeOfStringType),
                    PayloadEdmModel = edmModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element"),
                },
            };

            testDescriptors = testDescriptors.Select(td => td.InProperty());

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        private class CollectionPaddingPayloadTextCase<T>
        {
            public string XmlValue { get; set; }
            public T PayloadElement { get; set; }
        }

        private class CollectionXmlPadding
        {
            public string Padding { get; set; }
            public ExpectedException ExpectedException { get; set; }
        }

        private IEnumerable<PayloadReaderTestDescriptor> CreatePayloadWithPaddingTestDescriptor<T>(
            IEnumerable<CollectionPaddingPayloadTextCase<T>> testCases,
            IEnumerable<CollectionXmlPadding> xmlPaddings,
            IEdmModel model)
            where T : ODataPayloadElement, ITypedValue
        {
            return testCases.SelectMany(testCase =>
                xmlPaddings.Select(padding =>
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = testCase.PayloadElement.DeepCopy()
                            .XmlValueRepresentation(string.Format(CultureInfo.InvariantCulture, testCase.XmlValue, padding.Padding)),
                        ExpectedException = padding.ExpectedException,
                        PayloadEdmModel = model
                    }));
        }
    }
}

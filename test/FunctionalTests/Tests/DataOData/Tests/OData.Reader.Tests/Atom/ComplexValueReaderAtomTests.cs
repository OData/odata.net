//---------------------------------------------------------------------
// <copyright file="ComplexValueReaderAtomTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts.Atom;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of complex values in JSON.
    /// </summary>
    [TestClass, TestCase]
    public class ComplexValueReaderAtomTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        // TODO: Add tests which parse complex values without metadata (once it's implemented)
        // TODO: Add tests which parse open complex value (do have metadata, but no expected type)
        // TODO: Add tests which verify the property validation against metadata
        // TODO: Add tests which verify expected versus actual type checks.
        // TODO: Add tests which do specify m:type but don't provider metadata model.
        //       We should be able to recognize primitive and collection and treat the rest as complex in that case.

        private const string DefaultNamespaceName = "TestModel";

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct parsing of ATOM complex values with metadata or test which don't really care about metadata.")]
        public void ComplexValueTest()
        {
            EdmModel model = new EdmModel();

            var emptyComplexType = new EdmComplexType(DefaultNamespaceName, "EmptyComplexType");
            model.AddElement(emptyComplexType);

            var complexTypeWithStringProperty = new EdmComplexType(DefaultNamespaceName, "ComplexTypeWithStringProperty");
            complexTypeWithStringProperty.AddStructuralProperty("stringProperty", EdmCoreModel.Instance.GetString(isNullable: true));
            complexTypeWithStringProperty.AddStructuralProperty("numberProperty", EdmCoreModel.Instance.GetInt32(isNullable: false));
            model.AddElement(complexTypeWithStringProperty);

            model.Fixup();
            

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
            {
                // Empty element is a valid complex value
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexValue("TestModel.EmptyComplexType")
                        .XmlValueRepresentation(new XNode[0])
                        .WithTypeAnnotation(emptyComplexType),
                    PayloadEdmModel = model
                },
            };

            testDescriptors = testDescriptors.Concat(
                PropertiesElementAtomValues.CreatePropertiesElementPaddingPayloads<ComplexInstance>(
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.ComplexValue("TestModel.ComplexTypeWithStringProperty")
                            .WithTypeAnnotation(complexTypeWithStringProperty),
                        PayloadEdmModel = model
                    },
                    (complexInstance, xmlValue) => complexInstance.XmlValueRepresentation(xmlValue)));

            testDescriptors = testDescriptors.Select(td => td.InProperty());

            testDescriptors = testDescriptors.Concat(new []
            {
                // Top-level property without expected type and no type name - this is read as primitive string!
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Property(null, PayloadBuilder.PrimitiveValue(string.Empty))
                        .XmlRepresentation("<m:value/>"),
                    PayloadEdmModel = model,
                },
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Tests reading of complex properties focusing on ordering of elements in XML.")]
        public void ComplexValuePropertyOrderingTest()
        {
            EdmModel model = new EdmModel();

            var complexType = new EdmComplexType(DefaultNamespaceName, "ComplexType");
            complexType.AddStructuralProperty("stringProperty", EdmCoreModel.Instance.GetString(isNullable: false));
            complexType.AddStructuralProperty("numberProperty", EdmCoreModel.Instance.GetInt32(isNullable: false));
            complexType.AddStructuralProperty("nullProperty", EdmCoreModel.Instance.GetString(isNullable: true));
            model.AddElement(complexType);

            model.Fixup();

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors =
                PropertiesElementAtomValues.CreatePropertiesElementOrderingPayloads<ComplexInstance>(
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.ComplexValue("TestModel.ComplexType"),
                        PayloadEdmModel = model
                    },
                    (complexInstance, xmlNodes) => complexInstance.XmlValueRepresentation(xmlNodes)).Select(td =>
                    new PayloadReaderTestDescriptor(td)
                    {
                        PayloadElement = PayloadBuilder.Property(null, td.PayloadElement).ExpectedPropertyType(new EdmComplexTypeReference(complexType, isNullable: false)),
                    });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct parsing of ATOM complex values without any metadata.")]
        public void ComplexValueNoMetadataTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
            {
                // Single property with string value
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexValue(null)
                        .PrimitiveProperty("prop", "foo")
                        .XmlValueRepresentation("<d:prop>foo</d:prop>", null, null)
                },
                // Single property - empty element
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexValue(null)
                        .PrimitiveProperty("prop", string.Empty)
                        .XmlValueRepresentation("<d:prop/>", null, null)
                },
                // Two properties
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexValue(null)
                        .PrimitiveProperty("prop", "foo")
                        .PrimitiveProperty("prop2", 42)
                        .XmlValueRepresentation("<d:prop m:type='Edm.String'>foo</d:prop><d:prop2 m:type='Edm.Int32'>42</d:prop2>", null, null)
                },
                // Elements in non-OData namespace
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexValue(null)
                        .PrimitiveProperty("prop", "foo")
                        .XmlValueRepresentation("<m:foo/><!----><?value?><bar>some</bar><nested><d:ignored/></nested><d:prop>foo</d:prop>", null, null)
                },
                // Two properties - first called "element"
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexValue(null)
                        .PrimitiveProperty("element", "foo")
                        .PrimitiveProperty("prop2", 42)
                        .XmlValueRepresentation("<d:element>foo</d:element><d:prop2 m:type='Edm.Int32'>42</d:prop2>", null, null)
                },
                // Two properties - second called "element"
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexValue(null)
                        .PrimitiveProperty("prop", "foo")
                        .PrimitiveProperty("element", 42)
                        .XmlValueRepresentation("<d:prop>foo</d:prop><d:element m:type='Edm.Int32'>42</d:element>", null, null)
                },
                // Single property - element in non-OData metadata namespace with OData element in it
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexValue(null)
                        .PrimitiveProperty("prop", string.Empty)
                        .XmlValueRepresentation("<d:prop/><bar><d:prop/></bar>", null, null)
                },
                // Single property - element in non-OData metadata namespace with OData element called "element" in it
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexValue(null)
                        .PrimitiveProperty("prop", string.Empty)
                        .XmlValueRepresentation("<d:prop/><bar><d:element/></bar>", null, null)
                },
                // Single property - element in OData metadata namespace with OData element in it
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexValue(null)
                        .PrimitiveProperty("prop", string.Empty)
                        .XmlValueRepresentation("<d:prop/><m:bar><d:prop/></m:bar>", null, null)
                },
                // Single property - element in OData metadata namespace with OData element called "element" in it
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexValue(null)
                        .PrimitiveProperty("prop", string.Empty)
                        .XmlValueRepresentation("<d:prop/><m:bar><d:element/></m:bar>", null, null)
                },
                // Complex with type name, but no model
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexValue("MyTypeName")
                        .PrimitiveProperty("prop", "foo")
                        .XmlValueRepresentation("<d:prop>foo</d:prop>", "MyTypeName", null)
                },
                // Empty complex with type name, but no model - still should recognize that the type name is not primitive and read as complex
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexValue("MyTypeName")
                        .XmlValueRepresentation(null, "MyTypeName", null)
                },
            };

            // Wrap it in property (manually to prevent any type annotations)
            testDescriptors = testDescriptors.Select(td =>
                new PayloadReaderTestDescriptor(td)
                {
                    PayloadElement = PayloadBuilder.Property(null, td.PayloadElement)
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct parsing of ATOM complex values with invalid property names.")]
        public void ComplexValueInvalidPropertyNameTest()
        {
            // NOTE: we are not testing the other invalid characters ('@', ':') since the Xml reader will already throw
            //       and there is no reasonable way to produce such a payload in the test infrastructure.
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
            {
                // Single property with invalid name
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexValue(null)
                        .PrimitiveProperty("Invalid.Name", "foo"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_PropertiesMustNotContainReservedChars", "Invalid.Name", "':', '.', '@'")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.ComplexValue(null)
                        .PrimitiveProperty("InvalidName.", "foo"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_PropertiesMustNotContainReservedChars", "InvalidName.", "':', '.', '@'")
                },
            };

            // Wrap it in property (manually to prevent any type annotations)
            testDescriptors = testDescriptors.Select(td =>
                new PayloadReaderTestDescriptor(td)
                {
                    PayloadElement = PayloadBuilder.Property(null, td.PayloadElement)
                });

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

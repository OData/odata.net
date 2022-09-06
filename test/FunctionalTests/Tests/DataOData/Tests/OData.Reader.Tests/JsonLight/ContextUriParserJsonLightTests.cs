//---------------------------------------------------------------------
// <copyright file="ContextUriParserJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    // We currently only support InternalsVisibleTo on the desktop.
    #region Namespaces
    using System;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.OData.JsonLight;
    using Microsoft.OData.UriParser;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// JSON Lite specific tests for the ODataContextUriParser class.
    /// </summary>
    [TestClass, TestCase]
    public class ContextUriParserJsonLightTests : ODataReaderTestCase
    {
        #region private fields and init
        private const string MetadataDocumentUri = "http://odata.org/odata.svc/$metadata";
        private IEdmModel testModel;
        private IEdmEntityContainer defaultContainer;
        private IEdmEntityType metropolitanCityType;
        private IEdmEntityType personType;
        private IEdmEntityType employeeType;
        private IEdmEntityType officeType;
        private IEdmComplexType addressType;
        private IEdmNavigationProperty containedOfficeNavigationProperty;
        private IEdmNavigationProperty containedMetropolitanCityNavigationProperty;
        private IEdmEntitySet personSet;
        private IEdmEntitySet metropolitanCitySet;
        private IEdmContainedEntitySet containedMetropolitanCitySet;
        private IEdmContainedEntitySet containedOfficeSet;
        private IEdmSingleton boss;
        private IEdmEntitySet officeSet;

        public override void Init()
        {
            base.Init();
            this.TestInitialize();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.testModel = Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            this.defaultContainer = this.testModel.EntityContainer;

            this.personSet = this.defaultContainer.FindEntitySet("Persons");
            this.personType = (IEdmEntityType)this.testModel.FindType("TestModel.Person");
            this.employeeType = (IEdmEntityType)this.testModel.FindType("TestModel.Employee");
            this.officeType = (IEdmEntityType)this.testModel.FindType("TestModel.OfficeType");
            this.addressType = (IEdmComplexType)this.testModel.FindType("TestModel.Address");
            this.metropolitanCitySet = this.defaultContainer.FindEntitySet("MetropolitanCities");
            this.metropolitanCityType = (IEdmEntityType)this.testModel.FindType("TestModel.MetropolitanCityType");
            this.boss = this.defaultContainer.FindSingleton("Boss");
            this.containedOfficeNavigationProperty = (IEdmNavigationProperty)this.metropolitanCityType.FindProperty("ContainedOffice");
            this.containedOfficeSet = (IEdmContainedEntitySet)metropolitanCitySet.FindNavigationTarget(this.containedOfficeNavigationProperty);
            this.containedMetropolitanCityNavigationProperty = (IEdmNavigationProperty)this.officeType.FindProperty("ContainedCity");
            this.containedMetropolitanCitySet = (IEdmContainedEntitySet)containedOfficeSet.FindNavigationTarget(this.containedMetropolitanCityNavigationProperty);
            this.officeSet = this.defaultContainer.FindEntitySet("Offices");
        }
        #endregion private and init

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Tests correct behavior of the ODataContextUriParser.Parse methods with invalid inputs.")]
        public void JsonLightContextUriParserErrorTest()
        {
            var testCases = new ContextUriParserTestCase[]
            {
                #region Cases
                new ContextUriParserTestCase
                {
                    DebugDescription = "null context URI is not allowed",
                    ContextUri = null,
                    Model = this.testModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightContextUriParser_NullMetadataDocumentUri")
                },
                new ContextUriParserTestCase
                {
                    DebugDescription = "empty string is a relative URI; context URIs have to absolute",
                    ContextUri = string.Empty,
                    Model = this.testModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightContextUriParser_TopLevelContextUrlShouldBeAbsolute", "")
                },
                new ContextUriParserTestCase
                {
                    DebugDescription = "another relative URI; context URIs have to absolute",
                    ContextUri = "relativeUri",
                    Model = this.testModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightContextUriParser_TopLevelContextUrlShouldBeAbsolute", "relativeUri")
                },
                new ContextUriParserTestCase
                {
                    DebugDescription = "context URI with invalid schema name",
                    PayloadKind = ODataPayloadKind.EntityReferenceLink,
                    ContextUri = MetadataDocumentUri + "#IncorrectModel.CityType",
                    Model = this.testModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightContextUriParser_InvalidEntitySetNameOrTypeName", MetadataDocumentUri + "#IncorrectModel.CityType", "IncorrectModel.CityType")
                },
                new ContextUriParserTestCase
                {
                    DebugDescription = "context URI with invalid type name",
                    PayloadKind = ODataPayloadKind.EntityReferenceLink,
                    ContextUri = MetadataDocumentUri + "#TestModel.NonExistingType",
                    Model = this.testModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightContextUriParser_InvalidEntitySetNameOrTypeName", MetadataDocumentUri + "#TestModel.NonExistingType", "TestModel.NonExistingType")
                },
                new ContextUriParserTestCase
                {
                    DebugDescription = "No model",
                    PayloadKind = ODataPayloadKind.ServiceDocument,
                    ContextUri = MetadataDocumentUri,
                    Model = EdmCoreModel.Instance,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightContextUriParser_NoModel")
                },
                new ContextUriParserTestCase
                {
                    DebugDescription = "$value appeared in wrong place",
                    PayloadKind = ODataPayloadKind.ServiceDocument,
                    ContextUri = MetadataDocumentUri + "#Cities(3)/Name/$value",
                    Model = this.testModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightContextUriParser_InvalidContextUrl" , MetadataDocumentUri + "#Cities(3)/Name/$value")
                },
                new ContextUriParserTestCase
                {
                    DebugDescription = "Key Segment in the end",
                    PayloadKind = ODataPayloadKind.Resource,
                    ContextUri = MetadataDocumentUri + "#Cities(3)/$entity",
                    Model = this.testModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightContextUriParser_LastSegmentIsKeySegment", MetadataDocumentUri + "#Cities(3)/$entity")
                },
                new ContextUriParserTestCase
                {
                    DebugDescription = "Octothorpe in front",
                    PayloadKind = ODataPayloadKind.Resource,
                    ContextUri = "outlookfeeds://host/$metadata#%23/?://localhost:50672/$metadata%23Cities(3)/Name",
                    Model = this.testModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightContextUriParser_InvalidContextUrl", "outlookfeeds://host/$metadata#%23/?://localhost:50672/$metadata%23Cities(3)/Name")
                },
                new ContextUriParserTestCase
                {
                    DebugDescription = "Octothorpe in front",
                    PayloadKind = ODataPayloadKind.Resource,
                    ContextUri = "outlookfeeds://host/$metadata#Cities(3)%23/?://localhost:50673/$metadata%23Cities(3)/Name",
                    Model = this.testModel,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightContextUriParser_InvalidContextUrl", "outlookfeeds://host/$metadata#Cities(3)%23/?://localhost:50673/$metadata%23Cities(3)/Name")
                },
                #endregion cases
            };

            this.CombinatorialEngineProvider.RunCombinations(testCases, this.RunTest);
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Tests correct behavior of the ODataContextUriParser for service documents.")]
        public void JsonLightContextUriParserServiceDocumentTest()
        {
            var testCases = new ContextUriParserTestCase[]
            {
                #region Cases
                new ContextUriParserTestCase
                {
                    DebugDescription = "Metadata document URI without fragment",
                    PayloadKind = ODataPayloadKind.ServiceDocument,
                    ContextUri = MetadataDocumentUri,
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri)
                    }
                },
                new ContextUriParserTestCase
                {
                    DebugDescription = "Metadata document URI with empty fragment",
                    PayloadKind = ODataPayloadKind.ServiceDocument,
                    ContextUri = MetadataDocumentUri + "#",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri)
                    }
                },
                new ContextUriParserTestCase
                {
                    DebugDescription = "Metadata document URI with non-empty fragment",
                    PayloadKind = ODataPayloadKind.ServiceDocument,
                    ContextUri = MetadataDocumentUri + "#SomeValue",
                    ExpectedException = ODataExpectedExceptions.ODataException(
                        "ODataJsonLightContextUriParser_InvalidEntitySetNameOrTypeName",
                        MetadataDocumentUri + "#SomeValue",
                        "SomeValue")
                },
                #endregion Cases
            };

            this.CombinatorialEngineProvider.RunCombinations(testCases, this.RunTest);
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Tests correct behavior of the ODataContextUriParser for properties.")]
        public void JsonLightContextUriParserPropertyTest()
        {
            var testCases = new ContextUriParserTestCase[]
            {
                #region Cases
                // Metadata document URI for complex type
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Property,
                    ContextUri = MetadataDocumentUri + "#TestModel.Address",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri + "#TestModel.Address"),
                        EdmType = this.addressType
                    }
                },
                // Metadata document URI for complex collection type
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Property,
                    ContextUri = MetadataDocumentUri + "#Collection(TestModel.Address)",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri + "#Collection(TestModel.Address)"),
                        EdmType = this.addressType
                    }
                },
                // Metadata document URI for primitive type
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Property,
                    ContextUri = MetadataDocumentUri + "#Edm.Int32",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri + "#Edm.Int32"),
                        EdmType = EdmCoreModel.Instance.GetInt32(false).Definition
                    }
                },
                // Metadata document URI for primitive collection type
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Property,
                    ContextUri = MetadataDocumentUri + "#Collection(Edm.Int32)",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri + "#Collection(Edm.Int32)"),
                        EdmType = new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))
                    }
                },
                // Metadata document URI for entity type
                new ContextUriParserTestCase
                {
                    ContextUri = MetadataDocumentUri + "#TestModel.CityTypeA",
                    ExpectedException = ODataExpectedExceptions.ODataException(
                        "ODataJsonLightContextUriParser_InvalidEntitySetNameOrTypeName",
                        MetadataDocumentUri + "#TestModel.CityTypeA",
                        "TestModel.CityTypeA"),
                },
                // Metadata document URI for entity collection type
                new ContextUriParserTestCase
                {
                    ContextUri = MetadataDocumentUri + "#Collection(TestModel.CityTypeA)",
                    ExpectedException = ODataExpectedExceptions.ODataException(
                        "ODataJsonLightContextUriParser_InvalidEntitySetNameOrTypeName",
                        MetadataDocumentUri + "#Collection(TestModel.CityTypeA)",
                        "Collection(TestModel.CityTypeA)"),
                },
                // Metadata document URI for property on entry type
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Property,
                    ContextUri = MetadataDocumentUri + "#TestModel.CityType/Name",
                    ExpectedException = ODataExpectedExceptions.ODataException(
                        "ODataJsonLightContextUriParser_InvalidContextUrl",
                        MetadataDocumentUri + "#TestModel.CityType/Name"),
                },
                // Metadata document URI with type name
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Property,
                    ContextUri = MetadataDocumentUri + "#TestModel.UnknownType",
                    ExpectedException = ODataExpectedExceptions.ODataException(
                        "ODataJsonLightContextUriParser_InvalidEntitySetNameOrTypeName",
                        MetadataDocumentUri + "#TestModel.UnknownType",
                        "TestModel.UnknownType")
                },
                #endregion Cases
                // TODO: add error tests expecting incorrect payload kinds.
            };

            this.CombinatorialEngineProvider.RunCombinations(testCases, this.RunTest);
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Tests correct behavior of the ODataContextUriParser for properties.")]
        public void JsonLightContextUriParserIndividualPropertyTest()
        {
            var testCases = new ContextUriParserTestCase[]
            {
                #region Cases
                // Metadata document URI for primitive type
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Property,
                    ContextUri = MetadataDocumentUri + "#Cities(932)/Name",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        EdmType = EdmCoreModel.Instance.GetString(false).Definition
                    }
                },
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Property,
                    ContextUri = MetadataDocumentUri + "#Offices(932)/TestModel.OfficeType/Address",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        EdmType = addressType,
                        NavigationSource = this.officeSet
                    }
                },
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Property,
                    ContextUri = MetadataDocumentUri + "#Offices(932)/Address/TestModel.Address",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        EdmType = addressType,
                        NavigationSource = this.officeSet
                    }
                },
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Property,
                    ContextUri = MetadataDocumentUri + "#Offices(932)/Address/Zip",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        EdmType = EdmCoreModel.Instance.GetInt32(false).Definition
                    }
                },
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Property,
                    ContextUri = MetadataDocumentUri + "#Offices(932)/Address/TestModel.Address/SubAddress/SubAddress/SubAddress/Zip",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        EdmType = EdmCoreModel.Instance.GetInt32(false).Definition
                    }
                },
                #endregion Cases
            };

            this.CombinatorialEngineProvider.RunCombinations(testCases, this.RunTest);
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Tests correct behavior of the ODataContextUriParser for top-level collections.")]
        public void JsonLightContextUriParserCollectionTest()
        {
            var testCases = new ContextUriParserTestCase[]
            {
                #region Cases
                // Metadata document URI for complex collection type
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Collection,
                    ContextUri = MetadataDocumentUri + "#Collection(TestModel.Address)",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri + "#Collection(TestModel.Address)"),
                        EdmType = this.addressType
                    }
                },
                // Metadata document URI for primitive collection type
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Collection,
                    ContextUri = MetadataDocumentUri + "#Collection(Edm.Int32)",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri + "#Collection(Edm.Int32)"),
                        EdmType = new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))
                    }
                },
                #endregion Cases
            };

            this.CombinatorialEngineProvider.RunCombinations(testCases, this.RunTest);
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Tests correct behavior of the ODataContextUriParser for entries.")]
        public void JsonLightContextUriParserEntryTest()
        {
            var testCases = new ContextUriParserTestCase[]
            {
                #region Cases
                // Metadata document URI for an entry of the same type as the entity set
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Resource,
                    ContextUri = MetadataDocumentUri + "#TestModel.DefaultContainer.Persons/$entity",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        NavigationSource = this.personSet,
                        EdmType = this.personType,
                    }
                },
                // Metadata document URI for an entry of a derived type from the base type of the entity set
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Resource,
                    ContextUri = MetadataDocumentUri + "#DefaultContainer.Persons/TestModel.Employee/$entity",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        NavigationSource = this.personSet,
                        EdmType = this.employeeType,
                    }
                },
                // Metadata document URI for an entry with a type cast to the base type of the entity set
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Resource,
                    ContextUri = MetadataDocumentUri + "#TestModel.DefaultContainer.Persons/TestModel.Person/$entity",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        NavigationSource = this.personSet,
                        EdmType = this.personType,
                    }
                },
                 new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Resource,
                    ContextUri = MetadataDocumentUri + "#MetropolitanCities(932)/ContainedOffice/$entity",
                    ResourcePath = "MetropolitanCities(932)/ContainedOffice",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        NavigationSource = containedOfficeSet,
                        EdmType = officeType
                    }
                },
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Resource,
                    ContextUri = MetadataDocumentUri + "#MetropolitanCities(932)/ContainedOffice(232)/ContainedCity/$entity",
                    ResourcePath = "MetropolitanCities(932)/ContainedOffice(232)/ContainedCity",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        NavigationSource = this.containedMetropolitanCitySet,
                        EdmType = metropolitanCityType
                    }
                },
                // Metadata document URI for an entry with an invalid entity container
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Resource,
                    ContextUri = MetadataDocumentUri + "#NonExistingContainer.Persons/TestModel.Employee/$entity",
                    ExpectedException = ODataExpectedExceptions.ODataException(
                        "ODataJsonLightContextUriParser_InvalidContextUrl", 
                        MetadataDocumentUri + "#NonExistingContainer.Persons/TestModel.Employee/$entity")
                },
                // Metadata document URI for an entry with a type cast to a non-existing entity type
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Resource,
                    ContextUri = MetadataDocumentUri + "#TestModel.DefaultContainer.Persons/TestModel.NonExistingType/$entity",
                    ExpectedException = ODataExpectedExceptions.ODataException(
                        "ODataJsonLightContextUriParser_InvalidContextUrl", 
                        MetadataDocumentUri + "#TestModel.DefaultContainer.Persons/TestModel.NonExistingType/$entity")
                },
                // Metadata document URI for an entry of a type incompatible with the base type of the entity set
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Resource,
                    ContextUri = MetadataDocumentUri + "#DefaultContainer.Persons/TestModel.OfficeType/$entity",
                    ExpectedException = ODataExpectedExceptions.ODataException(
                        "ODataJsonLightContextUriParser_InvalidContextUrl",
                        MetadataDocumentUri + "#DefaultContainer.Persons/TestModel.OfficeType/$entity")
                },
                // Metadata document URI for an entry with an incorrect $entity suffix
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Resource,
                    ContextUri = MetadataDocumentUri + "#TestModel.DefaultContainer.Persons/@WrongElement",
                    ExpectedException = ODataExpectedExceptions.ODataException(
                        "ODataJsonLightContextUriParser_InvalidContextUrl",
                        MetadataDocumentUri + "#TestModel.DefaultContainer.Persons/@WrongElement")
                },
                // Metadata document URI for an entry with a type cast and an incorrect $entity suffix
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Resource,
                    ContextUri = MetadataDocumentUri + "#DefaultContainer.Persons/TestModel.Employee/@WrongElement",
                    ExpectedException = ODataExpectedExceptions.ODataException(
                        "ODataJsonLightContextUriParser_InvalidContextUrl",
                        MetadataDocumentUri + "#DefaultContainer.Persons/TestModel.Employee/@WrongElement")
                },
                // Metadata document URI for an entry (invalid entity set)
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.ResourceSet,
                    ContextUri = MetadataDocumentUri + "#TestModel.DefaultContainer.WrongSet/$entity",
                    ExpectedException = ODataExpectedExceptions.ODataException(
                        "ODataJsonLightContextUriParser_InvalidContextUrl", 
                        MetadataDocumentUri + "#TestModel.DefaultContainer.WrongSet/$entity")
                },
                // Metadata document URI for an entry with type cast (invalid entity set)
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.ResourceSet,
                    ContextUri = MetadataDocumentUri + "#DefaultContainer.WrongSet/TestModel.Employee/$entity",
                    ExpectedException = ODataExpectedExceptions.ODataException(
                        "ODataJsonLightContextUriParser_InvalidContextUrl", 
                        MetadataDocumentUri + "#DefaultContainer.WrongSet/TestModel.Employee/$entity")
                },
                #endregion Cases
                // TODO: add error tests expecting incorrect payload kinds.
            };

            this.CombinatorialEngineProvider.RunCombinations(testCases, this.RunTest);
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Tests correct behavior of the ODataContextUriParser for singleton.")]
        public void JsonLightContextUriParserSingletonTest()
        {
            var testCases = new ContextUriParserTestCase[]
            {
                #region Cases
                // Metadata document URI for an entry of the same type as the singleton
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Resource,
                    ContextUri = MetadataDocumentUri + "#TestModel.DefaultContainer.Boss",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        NavigationSource = this.boss,
                        EdmType = this.personType,
                    }
                },
                // Metadata document URI for an entry of the same type as the singleton, name not qualified.
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Resource,
                    ContextUri = MetadataDocumentUri + "#Boss",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        NavigationSource = this.boss,
                        EdmType = this.personType,
                    }
                },
                // {schema.entity-container.singleton}/{type-cast}
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Resource,
                    ContextUri = MetadataDocumentUri + "#Boss/TestModel.Employee",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        NavigationSource = this.boss,
                        EdmType = this.employeeType,
                    }
                },
                // {schema.entity-container.singleton}/{property}
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Property,
                    ContextUri = MetadataDocumentUri + "#Boss/Id",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        EdmType = EdmCoreModel.Instance.GetInt32(false).Definition,
                    }
                },
                //  {schema.entity-container.singleton}/{type-cast}/{property}
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Property,
                    ContextUri = MetadataDocumentUri + "#Boss/TestModel.Employee/CompanyName",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        EdmType = EdmCoreModel.Instance.GetString(false).Definition,
                    }
                },
                #endregion Cases
            };

            this.CombinatorialEngineProvider.RunCombinations(testCases, this.RunTest);
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Tests correct behavior of the ODataContextUriParser for feeds.")]
        public void JsonLightContextUriParserFeedTest()
        {
            var testCases = new ContextUriParserTestCase[]
            {
                #region Cases
                // Metadata document URI for a feed of the same type as the entity set
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.ResourceSet,
                    ContextUri = MetadataDocumentUri + "#TestModel.DefaultContainer.Persons",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        NavigationSource = this.personSet,
                        EdmType = this.personType,
                    }
                },
                // Metadata document URI for a feed with a type cast
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.ResourceSet,
                    ContextUri = MetadataDocumentUri + "#DefaultContainer.Persons/TestModel.Employee",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        NavigationSource = this.personSet,
                        EdmType = this.employeeType,
                    }
                },
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.ResourceSet,
                    ContextUri = MetadataDocumentUri + "#MetropolitanCities(932)/ContainedOffice",
                    ResourcePath = "MetropolitanCities(932)/ContainedOffice",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        NavigationSource = containedOfficeSet,
                        EdmType = officeType
                    }
                },
                // Metadata document URI for a feed (invalid entity set)
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.ResourceSet,
                    ContextUri = MetadataDocumentUri + "#TestModel.DefaultContainer.WrongSet",
                    ExpectedException = ODataExpectedExceptions.ODataException(
                        "ODataJsonLightContextUriParser_InvalidEntitySetNameOrTypeName",
                        MetadataDocumentUri + "#TestModel.DefaultContainer.WrongSet", 
                        "TestModel.DefaultContainer.WrongSet")
                },
                // Metadata document URI for a feed with a type cast (invalid entity set)
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.ResourceSet,
                    ContextUri = MetadataDocumentUri + "#DefaultContainer.WrongSet/TestModel.Employee",
                    ExpectedException = ODataExpectedExceptions.ODataException(
                        "ODataJsonLightContextUriParser_InvalidContextUrl", 
                        MetadataDocumentUri + "#DefaultContainer.WrongSet/TestModel.Employee")
                },
                #endregion Cases
                // TODO: add error tests expecting incorrect payload kinds.
            };

            this.CombinatorialEngineProvider.RunCombinations(testCases, this.RunTest);
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Tests correct behavior of the ODataContextUriParser for delta respnses.")]
        public void JsonLightContextUriParserDeltaTest()
        {
            var testCases = new ContextUriParserTestCase[]
            {
                #region Cases
                // Delta feed response
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Delta,
                    ContextUri = MetadataDocumentUri + "#Persons/$delta",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        NavigationSource = this.personSet,
                        EdmType = this.personType,
                        DeltaKind = ODataDeltaKind.ResourceSet
                    }
                },
                 // Delta entry response
                new ContextUriParserTestCase()
                {
                    PayloadKind = ODataPayloadKind.Delta,
                    ContextUri = MetadataDocumentUri + "#Persons/$entity",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        NavigationSource = this.personSet,
                        EdmType = this.personType,
                        DeltaKind = ODataDeltaKind.Resource
                    }
                },
                // Delta deletedEntry response
                new ContextUriParserTestCase()
                {
                    PayloadKind = ODataPayloadKind.Delta,
                    ContextUri = MetadataDocumentUri + "#Persons/$deletedEntity",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        NavigationSource = this.personSet,
                        EdmType = this.personType,
                        DeltaKind = ODataDeltaKind.DeletedEntry
                    }
                },
                // Delta link response
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Delta,
                    ContextUri = MetadataDocumentUri + "#Persons/$link",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        NavigationSource = this.personSet,
                        EdmType = this.personType,
                        DeltaKind = ODataDeltaKind.Link
                    }
                },
                 // Delta deletedLink response
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.Delta,
                    ContextUri = MetadataDocumentUri + "#Persons/$deletedLink",
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        NavigationSource = this.personSet,
                        EdmType = this.personType,
                        DeltaKind = ODataDeltaKind.DeletedLink
                    }
                },
                #endregion Cases
            };

            this.CombinatorialEngineProvider.RunCombinations(testCases, this.RunTest);
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Tests correct behavior of the ODataContextUriParser for $select query options.")]
        public void JsonLightContextUriParserSelectQueryOptionTest()
        {
            const string contextUriSuffix = "#TestModel.DefaultContainer.Persons";
            // NOTE: testing of the parsing of the $select query option itself is already done 
            //       in the TDD tests (see ProjectedPropertiesHierarchyAnnotationTests.cs)
            var testCases = new ContextUriParserTestCase[]
            {
                #region Cases
                // $select as the only query option
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.ResourceSet,
                    ContextUri = BuildExpectedContextUri(contextUriSuffix, "Id,*"),
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        NavigationSource = this.personSet,
                        EdmType = this.personType,
                        SelectQueryOption = "Id,*",
                    }
                },
                // $select as the only query option (escaped)
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.ResourceSet,
                    ContextUri = BuildExpectedContextUri(contextUriSuffix, "Id,%20*"),
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri),
                        NavigationSource = this.personSet,
                        EdmType = this.personType,
                        SelectQueryOption = "Id, *",
                    }
                },
                // $select with another query option
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.ResourceSet,
                    ContextUri = BuildExpectedContextUri(MetadataDocumentUri + "?$other=value", contextUriSuffix, "Id,*"),
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri + "?$other=value"),
                        NavigationSource = this.personSet,
                        EdmType = this.personType,
                        SelectQueryOption = "Id,*",
                    }
                },
                // query option without $select
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.ResourceSet,
                    ContextUri = BuildExpectedContextUri(MetadataDocumentUri + "?$other=value", contextUriSuffix, null),
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri + "?$other=value"),
                        NavigationSource = this.personSet,
                        EdmType = this.personType,
                        SelectQueryOption = null,
                    }
                },
                // $select in both fragment and query
                new ContextUriParserTestCase
                {
                    PayloadKind = ODataPayloadKind.ResourceSet,
                    ContextUri = BuildExpectedContextUri(MetadataDocumentUri + "somethingElse", contextUriSuffix, "Id,*"),
                    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                    {
                        MetadataDocumentUri = new Uri(MetadataDocumentUri + "somethingElse"),
                        NavigationSource = this.personSet,
                        EdmType = this.personType,
                        SelectQueryOption = "Id,*",
                    }
                },
                ////[TODO]: layliu this test case is not correct, need to be fixed later
                //// $select in wrong place
                //new ContextUriParserTestCase
                //{
                //    PayloadKind = ODataPayloadKind.Property,
                //    ContextUri = BuildExpectedContextUri(MetadataDocumentUri + "#Offices(932)/Address", contextUriSuffix, "Id,*"),
                //    ExpectedResult = new ODataJsonLightContextUriParseResult(null)
                //    {
                //        MetadataDocumentUri = new Uri(MetadataDocumentUri + "somethingElse"),
                //        NavigationSource = this.personSet,
                //        EdmType = this.personType,
                //        SelectQueryOption = "Id,*",
                //    },
                //    ExpectedException = ODataExpectedExceptions.ODataException(
                //        "ODataJsonLightContextUriParser_InvalidPayloadKindWithSelectQueryOption", 
                //        "Property")
                //},
                #endregion Cases
            };

            this.CombinatorialEngineProvider.RunCombinations(testCases, this.RunTest);
        }

        private static string BuildExpectedContextUri(string expectedFragment, string selectValue)
        {
            return BuildExpectedContextUri(MetadataDocumentUri, expectedFragment, selectValue);
        }

        private static string BuildExpectedContextUri(string metadataDocumentUri, string expectedFragment, string selectValue)
        {
            if (selectValue != null)
            {
                expectedFragment += "(" + selectValue + ")";
            }

            return metadataDocumentUri + expectedFragment;
        }

        private void RunTest(ContextUriParserTestCase testCase)
        {
            this.Assert.ExpectedException(
                () =>
                {
                    ODataJsonLightContextUriParseResult parseResult = ODataJsonLightContextUriParser.Parse(
                        testCase.Model ?? this.testModel,
                        testCase.ContextUri,
                        testCase.PayloadKind,
                        null,
                        true);

                    this.CompareContextUriParseResults(testCase.ExpectedResult, parseResult, testCase.ResourcePath);
                },
                testCase.ExpectedException,
                this.ExceptionVerifier);
        }

        private void CompareContextUriParseResults(ODataJsonLightContextUriParseResult expected, ODataJsonLightContextUriParseResult actual, string expectedPathStr)
        {
            this.Assert.AreEqual(expected.MetadataDocumentUri, actual.MetadataDocumentUri, "Metadata document URIs don't match.");
            this.Assert.AreEqual(expected.NavigationSource, actual.NavigationSource, "Entity sets don't match.");
            if (expected.EdmType is IEdmCollectionType)
            {
                this.Assert.AreEqual(((IEdmCollectionType)expected.EdmType).ElementType.Definition, ((IEdmCollectionType)actual.EdmType).ElementType.Definition, "Entity types don't match.");
            }
            else
            {
                this.Assert.AreEqual(expected.EdmType, actual.EdmType, "Entity types don't match.");
            }

            this.Assert.AreEqual(expected.SelectQueryOption, actual.SelectQueryOption, "Select query option properties don't match.");
            if (expectedPathStr != null)
            {
                this.Assert.IsNotNull(actual.Path, "Path should not be null");
                this.Assert.AreEqual(expectedPathStr, actual.Path.ToContextUrlPathString(), "Path not equal");
            }

            this.Assert.AreEqual(expected.DeltaKind, actual.DeltaKind, "Delta kind doesn't match.");
        }

        private sealed class ContextUriParserTestCase
        {
            public string DebugDescription { get; set; }
            public string ContextUri { get; set; }
            public ODataPayloadKind PayloadKind { get; set; }
            public ODataJsonLightContextUriParseResult ExpectedResult { get; set; }
            public ExpectedException ExpectedException { get; set; }
            public IEdmModel Model { get; set; }
            public string ResourcePath { get; set; }

            public override string ToString()
            {
                return this.DebugDescription + "\r\n" + this.ContextUri;
            }
        }
    }
}

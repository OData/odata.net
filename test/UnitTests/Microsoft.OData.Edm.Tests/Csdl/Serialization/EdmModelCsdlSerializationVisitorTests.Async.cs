//---------------------------------------------------------------------
// <copyright file="EdmModelCsdlSerializationVisitorTests.Async.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Csdl.Serialization;
using Microsoft.OData.Edm.Vocabularies;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl.Serialization
{
    public partial class EdmModelCsdlSerializationVisitorTests
    {
        #region Complex Type
        [Fact]
        public async Task VerifyComplexTypeWrittenCorrectly_Async()
        {
            // Arrange
            EdmComplexType complexType = new EdmComplexType("NS", "Dimensions");
            complexType.AddStructuralProperty("Height", EdmCoreModel.Instance.GetDecimal(6, 2, true));
            complexType.AddStructuralProperty("Weight", EdmCoreModel.Instance.GetDecimal(6, null, true));
            complexType.AddStructuralProperty("Length", EdmCoreModel.Instance.GetDecimal(null, null, false));
            complexType.AddStructuralProperty("Breadth", EdmCoreModel.Instance.GetDecimal(6, 0, true));

            // Act & Assert for XML
            await VisitAndVerifyXmlAsync(v => v.VisitSchemaTypeAsync(complexType),
                @"<ComplexType Name=""Dimensions"">
  <Property Name=""Height"" Type=""Edm.Decimal"" Precision=""6"" Scale=""2"" />
  <Property Name=""Weight"" Type=""Edm.Decimal"" Precision=""6"" Scale=""Variable"" />
  <Property Name=""Length"" Type=""Edm.Decimal"" Nullable=""false"" Scale=""Variable"" />
  <Property Name=""Breadth"" Type=""Edm.Decimal"" Precision=""6"" Scale=""0"" />
</ComplexType>").ConfigureAwait(false);

            // Act & Assert for JSON
            await VisitAndVerifyJsonAsync(v => v.VisitSchemaTypeAsync(complexType), @"{
  ""Dimensions"": {
    ""$Kind"": ""ComplexType"",
    ""Height"": {
      ""$Type"": ""Edm.Decimal"",
      ""$Nullable"": true,
      ""$Precision"": 6,
      ""$Scale"": 2
    },
    ""Weight"": {
      ""$Type"": ""Edm.Decimal"",
      ""$Nullable"": true,
      ""$Precision"": 6
    },
    ""Length"": {
      ""$Type"": ""Edm.Decimal""
    },
    ""Breadth"": {
      ""$Type"": ""Edm.Decimal"",
      ""$Nullable"": true,
      ""$Precision"": 6,
      ""$Scale"": 0
    }
  }
}").ConfigureAwait(false);
        }
        #endregion

        #region Action
        [Fact]
        public async Task VerifySchemaWithActionsWrittenCorrectly_Async()
        {
            // Arrange
            EdmSchema schema = new EdmSchema("NS");
            EdmAction action = new EdmAction("NS", "DoStuff", EdmCoreModel.Instance.GetString(true), true, null);
            action.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            schema.AddSchemaElement(action);

            action = new EdmAction("NS", "DoStuff", EdmCoreModel.Instance.GetString(true), true, null);
            action.AddParameter("param1", EdmCoreModel.Instance.GetInt32(true));
            schema.AddSchemaElement(action);

            // Act & Assert for XML
            await VisitAndVerifyXmlAsync((v) => v.VisitEdmSchemaAsync(schema, null),
                @"<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Action Name=""DoStuff"" IsBound=""true"">
    <Parameter Name=""param1"" Type=""Edm.String"" />
    <ReturnType Type=""Edm.String"" />
  </Action>
  <Action Name=""DoStuff"" IsBound=""true"">
    <Parameter Name=""param1"" Type=""Edm.Int32"" />
    <ReturnType Type=""Edm.String"" />
  </Action>
</Schema>").ConfigureAwait(false);

            // Act & Assert for JSON
            await VisitAndVerifyJsonAsync((v) => v.VisitEdmSchemaAsync(schema, null), @"{
  ""NS"": {
    ""DoStuff"": [
      {
        ""$Kind"": ""Action"",
        ""$IsBound"": true,
        ""$Parameter"": [
          {
            ""$Name"": ""param1"",
            ""$Nullable"": true
          }
        ],
        ""$ReturnType"": {
          ""$Nullable"": true
        }
      },
      {
        ""$Kind"": ""Action"",
        ""$IsBound"": true,
        ""$Parameter"": [
          {
            ""$Name"": ""param1"",
            ""$Type"": ""Edm.Int32"",
            ""$Nullable"": true
          }
        ],
        ""$ReturnType"": {
          ""$Nullable"": true
        }
      }
    ]
  }
}").ConfigureAwait(false);
        }
        #endregion

        #region EntitySet
        [Fact]
        public async Task VerifyEntitySetWrittenCorrectly_Async()
        {
            // Arrange
            IEdmEntityType entityType = new EdmEntityType("NS", "EntityType");
            IEdmEntityContainer container = new EdmEntityContainer("NS", "Container");
            EdmEntitySet entitySet = new EdmEntitySet(container, "Set", entityType);

            // Act & Assert for XML
            await VisitAndVerifyXmlAsync((v) => v.VisitEntityContainerElementsAsync(new[] { entitySet }),
                @"<EntitySet Name=""Set"" EntityType=""NS.EntityType"" />").ConfigureAwait(false);

            // Act & Assert for JSON
            await VisitAndVerifyJsonAsync(async (v) => await v.VisitEntityContainerElementsAsync(new[] { entitySet }).ConfigureAwait(false),
                @"{
  ""Set"": {
    ""$Collection"": true,
    ""$Type"": ""NS.EntityType""
  }
}").ConfigureAwait(false);

            // Act & Assert for non-indent JSON
            await VisitAndVerifyJsonAsync((v) => v.VisitEntityContainerElementsAsync(new[] { entitySet }),
                @"{""Set"":{""$Collection"":true,""$Type"":""NS.EntityType""}}", false).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyEntitySetWithAllInformationsWrittenCorrectly_Async()
        {
            // Arrange
            var person = new EdmEntityType("NS", "Person");
            var entityId = person.AddStructuralProperty("UserName", EdmCoreModel.Instance.GetString(false));
            person.AddKeys(entityId);

            var city = new EdmEntityType("NS", "City");
            var cityId = city.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            city.AddKeys(cityId);

            var countryOrRegion = new EdmEntityType("NS", "CountryOrRegion");
            var countryId = countryOrRegion.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            countryOrRegion.AddKeys(countryId);

            var complex = new EdmComplexType("NS", "Address");
            complex.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            var navP = complex.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "City",
                    Target = city,
                    TargetMultiplicity = EdmMultiplicity.One,
                });

            var derivedComplex = new EdmComplexType("NS", "WorkAddress", complex);
            var navP2 = derivedComplex.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "CountryOrRegion",
                    Target = countryOrRegion,
                    TargetMultiplicity = EdmMultiplicity.One,
                });

            person.AddStructuralProperty("HomeAddress", new EdmComplexTypeReference(complex, false));
            person.AddStructuralProperty("WorkAddress", new EdmComplexTypeReference(complex, false));
            person.AddStructuralProperty("Addresses", new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(complex, false))));

            model.AddElement(person);
            model.AddElement(city);
            model.AddElement(countryOrRegion);
            model.AddElement(complex);
            model.AddElement(derivedComplex);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            model.AddElement(entityContainer);
            EdmEntitySet people = new EdmEntitySet(entityContainer, "People", person);
            EdmEntitySet cities = new EdmEntitySet(entityContainer, "City", city);
            EdmEntitySet countriesOrRegions = new EdmEntitySet(entityContainer, "CountryOrRegion", countryOrRegion);
            people.AddNavigationTarget(navP, cities, new EdmPathExpression("HomeAddress/City"));
            people.AddNavigationTarget(navP, cities, new EdmPathExpression("Addresses/City"));
            people.AddNavigationTarget(navP2, countriesOrRegions, new EdmPathExpression("WorkAddress/NS.WorkAddress/CountryOrRegion"));
            entityContainer.AddElement(people);
            entityContainer.AddElement(cities);
            entityContainer.AddElement(countriesOrRegions);

            EdmTerm term = new EdmTerm("UI", "ReadOnly", EdmPrimitiveTypeKind.Boolean);
            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(people, term, new EdmBooleanConstant(true));
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.SetVocabularyAnnotation(annotation);

            // Act & Assert for XML
            await VisitAndVerifyXmlAsync((v) => v.VisitEntityContainerElementsAsync(new[] { people }),
                @"<EntitySet Name=""People"" EntityType=""NS.Person"">
  <NavigationPropertyBinding Path=""Addresses/City"" Target=""City"" />
  <NavigationPropertyBinding Path=""HomeAddress/City"" Target=""City"" />
  <NavigationPropertyBinding Path=""WorkAddress/NS.WorkAddress/CountryOrRegion"" Target=""CountryOrRegion"" />
  <Annotation Term=""UI.ReadOnly"" Bool=""true"" />
</EntitySet>").ConfigureAwait(false);

            // Act & Assert for JSON
            await VisitAndVerifyJsonAsync((v) => v.VisitEntityContainerElementsAsync(new[] { people }),
                @"{
  ""People"": {
    ""$Collection"": true,
    ""$Type"": ""NS.Person"",
    ""$NavigationPropertyBinding"": {
      ""Addresses/City"": ""City"",
      ""HomeAddress/City"": ""City"",
      ""WorkAddress/NS.WorkAddress/CountryOrRegion"": ""CountryOrRegion""
    },
    ""@UI.ReadOnly"": true
  }
}").ConfigureAwait(false);
        }
        #endregion

        #region Singleton
        [Fact]
        public async Task VerifySingletonWrittenCorrectly_Async()
        {
            IEdmEntityType entityType = new EdmEntityType("NS", "EntityType");
            IEdmEntityContainer container = new EdmEntityContainer("NS", "Container");
            EdmSingleton singleton = new EdmSingleton(container, "Me", entityType);

            await VisitAndVerifyXmlAsync((v) => v.VisitEntityContainerElementsAsync(new[] { singleton }),
                @"<Singleton Name=""Me"" Type=""NS.EntityType"" />").ConfigureAwait(false);

            await VisitAndVerifyJsonAsync((v) => v.VisitEntityContainerElementsAsync(new[] { singleton }), @"{
  ""Me"": {
    ""$Type"": ""NS.EntityType""
  }
}").ConfigureAwait(false);

            await VisitAndVerifyJsonAsync((v) =>  v.VisitEntityContainerElementsAsync(new[] { singleton }),
                @"{""Me"":{""$Type"":""NS.EntityType""}}", false).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifySingletonWithAnnotationWrittenCorrectly_Async()
        {
            var entityType = new EdmEntityType("NS", "EntityType");
            var container = new EdmEntityContainer("NS", "Container");
            var singleton = new EdmSingleton(container, "Me", entityType);
            container.AddElement(singleton);
            this.model.AddElement(entityType);
            this.model.AddElement(container);

            var enumType = new EdmEnumType("NS", "Permission", true);
            var read = enumType.AddMember("Read", new EdmEnumMemberValue(0));
            var write = enumType.AddMember("Write", new EdmEnumMemberValue(1));
            var term = new EdmTerm("NS", "MyTerm", new EdmEnumTypeReference(enumType, true));
            this.model.AddElement(term);

            var enumMemberValue = new EdmEnumMemberExpression(read, write);
            var annotation = new EdmVocabularyAnnotation(singleton, term, enumMemberValue);
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            this.model.SetVocabularyAnnotation(annotation);

            await VisitAndVerifyXmlAsync((v) => v.VisitEntityContainerElementsAsync(new[] { singleton }),
                @"<Singleton Name=""Me"" Type=""NS.EntityType"">
  <Annotation Term=""NS.MyTerm"">
    <EnumMember>NS.Permission/Read NS.Permission/Write</EnumMember>
  </Annotation>
</Singleton>").ConfigureAwait(false);

            await VisitAndVerifyJsonAsync((v) => v.VisitEntityContainerElementsAsync(new[] { singleton }), @"{
  ""Me"": {
    ""$Type"": ""NS.EntityType"",
    ""@NS.MyTerm"": ""Read,Write""
  }
}").ConfigureAwait(false);
        }

        #endregion

        #region Action Import
        [Fact]
        public async Task VerifyActionImportWrittenCorrectly_Async()
        {
            // Arrange
            var actionImport = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, null);

            // Act & Assert for XML
            await VisitAndVerifyXmlAsync((v) => v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { actionImport }),
                @"<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" />").ConfigureAwait(false);

            // Act & Assert for JSON
            await VisitAndVerifyJsonAsync((v) => v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { actionImport }),
                @"{
  ""Checkout"": {
    ""$Kind"": ""ActionImport"",
    ""$Action"": ""Default.NameSpace2.CheckOut""
  }
}").ConfigureAwait(false);

            // Act & Assert for non-indent JSON
            await VisitAndVerifyJsonAsync((v) => v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { actionImport }),
                @"{""Checkout"":{""$Kind"":""ActionImport"",""$Action"":""Default.NameSpace2.CheckOut""}}", false).ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyTwoIdenticalNamedActionImportsWithNoEntitySetPropertyOnlyWrittenOnce_Async()
        {
            // Arrange
            var actionImport = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, null);
            var actionImport2 = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, null);

            // Act & Assert for XML
            await VisitAndVerifyXmlAsync((v) => v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { actionImport, actionImport2 }),
                @"<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" />").ConfigureAwait(false);

            // Act & Assert for JSON
            await VisitAndVerifyJsonAsync((v) => v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { actionImport, actionImport2 }),
                @"{
  ""Checkout"": {
    ""$Kind"": ""ActionImport"",
    ""$Action"": ""Default.NameSpace2.CheckOut""
  }
}").ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyTwoIdenticalNamedActionImportsWithSameEntitySetOnlyWrittenOnce_Async()
        {
            // Arrange
            var actionImport = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmPathExpression("Set"));
            var actionImport2 = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmPathExpression("Set"));

            // Act & Assert for XML
            await VisitAndVerifyXmlAsync((v) => v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { actionImport, actionImport2 }),
                @"<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" EntitySet=""Set"" />").ConfigureAwait(false);

            // Act & Assert for JSON
            await VisitAndVerifyJsonAsync((v) => v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { actionImport, actionImport2 }),
                @"{
  ""Checkout"": {
    ""$Kind"": ""ActionImport"",
    ""$Action"": ""Default.NameSpace2.CheckOut"",
    ""$EntitySet"": ""Set""
  }
}").ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyTwoIdenticalNamedActionImportsWithSameEdmPathOnlyWrittenOnce_Async()
        {
            // Arrange
            var actionImport = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmPathExpression("path1", "path2"));
            var actionImport2 = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmPathExpression("path1", "path2"));

            // Act & Assert for XML
            await VisitAndVerifyXmlAsync((v) => v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { actionImport, actionImport2 }),
                @"<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" EntitySet=""path1/path2"" />").ConfigureAwait(false);

            // Act & Assert for JSON
            await VisitAndVerifyJsonAsync((v) => v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { actionImport, actionImport2 }),
                @"{
  ""Checkout"": {
    ""$Kind"": ""ActionImport"",
    ""$Action"": ""Default.NameSpace2.CheckOut"",
    ""$EntitySet"": ""path1/path2""
  }
}").ConfigureAwait(false);
        }

        /// <summary>
        /// From OData Spec, it should be invalid to have overload action import.
        /// Need to check with OData TC.
        /// </summary>
        [Fact]
        public async Task VerifyIdenticalNamedActionImportsWithDifferentEntitySetPropertiesAreWritten_Async()
        {
            // Arrange
            var actionImportOnSet = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmPathExpression("Set"));
            var actionImportOnSet2 = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmPathExpression("Set2"));
            var actionImportWithNoEntitySet = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, null);
            var actionImportWithUniqueEdmPath = new EdmActionImport(defaultContainer, "Checkout", defaultCheckoutAction, new EdmPathExpression("path1", "path2"));

            // Act & Assert for XML
            await VisitAndVerifyXmlAsync((v) => v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { actionImportOnSet, actionImportOnSet2, actionImportWithNoEntitySet, actionImportWithUniqueEdmPath }),
                @"<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" EntitySet=""Set"" />
<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" EntitySet=""Set2"" />
<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" />
<ActionImport Name=""Checkout"" Action=""Default.NameSpace2.CheckOut"" EntitySet=""path1/path2"" />").ConfigureAwait(false);

            // Act & Assert for JSON
            await VisitAndVerifyJsonAsync((v) => v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { actionImportOnSet, actionImportOnSet2, actionImportWithNoEntitySet, actionImportWithUniqueEdmPath }),
    @"{
  ""Checkout"": {
    ""$Kind"": ""ActionImport"",
    ""$Action"": ""Default.NameSpace2.CheckOut"",
    ""$EntitySet"": ""Set""
  },
  ""Checkout"": {
    ""$Kind"": ""ActionImport"",
    ""$Action"": ""Default.NameSpace2.CheckOut"",
    ""$EntitySet"": ""Set2""
  },
  ""Checkout"": {
    ""$Kind"": ""ActionImport"",
    ""$Action"": ""Default.NameSpace2.CheckOut""
  },
  ""Checkout"": {
    ""$Kind"": ""ActionImport"",
    ""$Action"": ""Default.NameSpace2.CheckOut"",
    ""$EntitySet"": ""path1/path2""
  }
}").ConfigureAwait(false);
        }
        #endregion

        #region Function Import
        [Fact]
        public async Task VerifyFunctionImportWrittenCorrectly_Async()
        {
            // Arrange
            var functionImport = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, null, true);

            // Act & Assert for XML
            await VisitAndVerifyXmlAsync(async (v) => await v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { functionImport }).ConfigureAwait(false),
                @"<FunctionImport Name=""GetStuff"" Function=""Default.NameSpace2.GetStuff"" IncludeInServiceDocument=""true"" />").ConfigureAwait(false);

            // Act & Assert for JSON
            await VisitAndVerifyJsonAsync(async (v) => await v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { functionImport }).ConfigureAwait(false),
                @"{
  ""GetStuff"": {
    ""$Kind"": ""FunctionImport"",
    ""$Function"": ""Default.NameSpace2.GetStuff"",
    ""$IncludeInServiceDocument"": true
  }
}").ConfigureAwait(false);

            // Act & Assert for non-indent JSON
            await VisitAndVerifyJsonAsync(async (v) => await v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { functionImport }).ConfigureAwait(false),
                @"{""GetStuff"":{""$Kind"":""FunctionImport"",""$Function"":""Default.NameSpace2.GetStuff"",""$IncludeInServiceDocument"":true}}", false);
        }

        [Fact]
        public async Task VerifyTwoIdenticalNamedFunctionImportsWithoutEntitySetValueOnlyWrittenOnce_Async()
        {
            // Arrange
            var functionImport = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, null, true);
            var functionImport2 = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, null, true);

            // Act & Assert for XML
            await VisitAndVerifyXmlAsync(async (v) => await v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { functionImport, functionImport2 }).ConfigureAwait(false),
                @"<FunctionImport Name=""GetStuff"" Function=""Default.NameSpace2.GetStuff"" IncludeInServiceDocument=""true"" />").ConfigureAwait(false);

            // Act & Assert for JSON
            await VisitAndVerifyJsonAsync(async (v) => await v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { functionImport, functionImport2 }).ConfigureAwait(false),
                @"{
  ""GetStuff"": {
    ""$Kind"": ""FunctionImport"",
    ""$Function"": ""Default.NameSpace2.GetStuff"",
    ""$IncludeInServiceDocument"": true
  }
}").ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyTwoIdenticalNamedFunctionImportsWithSameEntitySetValueOnlyWrittenOnce_Async()
        {
            // Arrange
            var functionImport = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, new EdmPathExpression("Set"), true);
            var functionImport2 = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, new EdmPathExpression("Set"), true);

            // Act & Assert for XML
            await VisitAndVerifyXmlAsync(async (v) => await v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { functionImport, functionImport2 }).ConfigureAwait(false),
                @"<FunctionImport Name=""GetStuff"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""Set"" IncludeInServiceDocument=""true"" />").ConfigureAwait(false);

            // Act & Assert for JSON
            await VisitAndVerifyJsonAsync(async (v) => await v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { functionImport, functionImport2 }).ConfigureAwait(false),
                @"{
  ""GetStuff"": {
    ""$Kind"": ""FunctionImport"",
    ""$Function"": ""Default.NameSpace2.GetStuff"",
    ""$EntitySet"": ""Set"",
    ""$IncludeInServiceDocument"": true
  }
}").ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyTwoIdenticalNamedFunctionImportsWithSameEntitySetPathValueOnlyWrittenOnce_Async()
        {
            // Arrange
            var functionImport = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, new EdmPathExpression("path1", "path2"), true);
            var functionImport2 = new EdmFunctionImport(defaultContainer, "GetStuff", defaultGetStuffFunction, new EdmPathExpression("path1", "path2"), true);

            // Act & Assert for XML
            await VisitAndVerifyXmlAsync(async (v) => await v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { functionImport, functionImport2 }).ConfigureAwait(false),
                @"<FunctionImport Name=""GetStuff"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""path1/path2"" IncludeInServiceDocument=""true"" />").ConfigureAwait(false);

            // Act & Assert for JSON
            await VisitAndVerifyJsonAsync(async (v) => await v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { functionImport, functionImport2 }).ConfigureAwait(false),
                @"{
  ""GetStuff"": {
    ""$Kind"": ""FunctionImport"",
    ""$Function"": ""Default.NameSpace2.GetStuff"",
    ""$EntitySet"": ""path1/path2"",
    ""$IncludeInServiceDocument"": true
  }
}").ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyIdenticalNamedFunctionImportsWithDifferentEntitySetPropertiesAreWritten_Async()
        {
            // Arrange
            var functionImportOnSet = new EdmFunctionImport(defaultContainer, "Checkout", defaultGetStuffFunction, new EdmPathExpression("Set"), false);
            var functionImportOnSet2 = new EdmFunctionImport(defaultContainer, "Checkout", defaultGetStuffFunction, new EdmPathExpression("Set2"), false);
            var functionmportWithNoEntitySet = new EdmFunctionImport(defaultContainer, "Checkout", defaultGetStuffFunction, null, true);
            var functionImportWithUniqueEdmPath = new EdmFunctionImport(defaultContainer, "Checkout", defaultGetStuffFunction, new EdmPathExpression("path1", "path2"), false);

            // Act & Assert for XML
            await VisitAndVerifyXmlAsync(async (v) => await v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { functionImportOnSet, functionImportOnSet2, functionmportWithNoEntitySet, functionImportWithUniqueEdmPath }).ConfigureAwait(false),
                @"<FunctionImport Name=""Checkout"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""Set"" />
<FunctionImport Name=""Checkout"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""Set2"" />
<FunctionImport Name=""Checkout"" Function=""Default.NameSpace2.GetStuff"" IncludeInServiceDocument=""true"" />
<FunctionImport Name=""Checkout"" Function=""Default.NameSpace2.GetStuff"" EntitySet=""path1/path2"" />").ConfigureAwait(false);

            // Act & Assert for JSON
            await VisitAndVerifyJsonAsync(async (v) => await v.VisitEntityContainerElementsAsync(new IEdmEntityContainerElement[] { functionImportOnSet, functionImportOnSet2, functionmportWithNoEntitySet, functionImportWithUniqueEdmPath }).ConfigureAwait(false),
                @"{
  ""Checkout"": {
    ""$Kind"": ""FunctionImport"",
    ""$Function"": ""Default.NameSpace2.GetStuff"",
    ""$EntitySet"": ""Set""
  },
  ""Checkout"": {
    ""$Kind"": ""FunctionImport"",
    ""$Function"": ""Default.NameSpace2.GetStuff"",
    ""$EntitySet"": ""Set2""
  },
  ""Checkout"": {
    ""$Kind"": ""FunctionImport"",
    ""$Function"": ""Default.NameSpace2.GetStuff"",
    ""$IncludeInServiceDocument"": true
  },
  ""Checkout"": {
    ""$Kind"": ""FunctionImport"",
    ""$Function"": ""Default.NameSpace2.GetStuff"",
    ""$EntitySet"": ""path1/path2""
  }
}").ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifySchemaWithFunctionsWrittenCorrectly_Async()
        {
            // Arrange
            EdmSchema schema = new EdmSchema("NS");
            EdmFunction function = new EdmFunction("NS", "GetStuff", EdmCoreModel.Instance.GetString(true), true, null, false);
            function.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            schema.AddSchemaElement(function);

            function = new EdmFunction("NS", "GetStuff", EdmCoreModel.Instance.GetGuid(false), true, null, false);
            function.AddParameter("param1", EdmCoreModel.Instance.GetString(true));
            function.AddParameter("param2", EdmCoreModel.Instance.GetInt32(false));
            schema.AddSchemaElement(function);

            // Act & Assert for XML
            await VisitAndVerifyXmlAsync(async (v) => await v.VisitEdmSchemaAsync(schema, null).ConfigureAwait(false),
                @"<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Function Name=""GetStuff"" IsBound=""true"">
    <Parameter Name=""param1"" Type=""Edm.String"" />
    <ReturnType Type=""Edm.String"" />
  </Function>
  <Function Name=""GetStuff"" IsBound=""true"">
    <Parameter Name=""param1"" Type=""Edm.String"" />
    <Parameter Name=""param2"" Type=""Edm.Int32"" Nullable=""false"" />
    <ReturnType Type=""Edm.Guid"" Nullable=""false"" />
  </Function>
</Schema>").ConfigureAwait(false);

            // Act & Assert for JSON
            await VisitAndVerifyJsonAsync(async (v) => await v.VisitEdmSchemaAsync(schema, null).ConfigureAwait(false), @"{
  ""NS"": {
    ""GetStuff"": [
      {
        ""$Kind"": ""Function"",
        ""$IsBound"": true,
        ""$Parameter"": [
          {
            ""$Name"": ""param1"",
            ""$Nullable"": true
          }
        ],
        ""$ReturnType"": {
          ""$Nullable"": true
        }
      },
      {
        ""$Kind"": ""Function"",
        ""$IsBound"": true,
        ""$Parameter"": [
          {
            ""$Name"": ""param1"",
            ""$Nullable"": true
          },
          {
            ""$Name"": ""param2"",
            ""$Type"": ""Edm.Int32""
          }
        ],
        ""$ReturnType"": {
          ""$Type"": ""Edm.Guid""
        }
      }
    ]
  }
}").ConfigureAwait(false);
        }
        #endregion

        #region Out of line annotation
        [Fact]
        public async Task VerifyOutOfLineAnnotationWrittenCorrectly_Async()
        {
            // Arrange
            EdmSchema schema = new EdmSchema("NS");

            EdmComplexType complexType = new EdmComplexType("NS", "ComplexType");
            EdmProperty property = complexType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);

            EdmTerm term = new EdmTerm("UI", "Thumbnail", EdmPrimitiveTypeKind.Binary);
            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(complexType, term, new EdmBinaryConstant(new byte[] { 0x4f, 0x44, 0x61, 0x74, 0x61 }));
            schema.AddVocabularyAnnotation(annotation);

            EdmTerm displayName = new EdmTerm("UI", "DisplayName", EdmPrimitiveTypeKind.Int32);
            annotation = new EdmVocabularyAnnotation(complexType, displayName, new EdmIntegerConstant(42));
            schema.AddVocabularyAnnotation(annotation);

            annotation = new EdmVocabularyAnnotation(complexType, displayName, "Tablet", new EdmIntegerConstant(88));
            schema.AddVocabularyAnnotation(annotation);

            annotation = new EdmVocabularyAnnotation(property, displayName, "Tablet", new EdmIntegerConstant(42));
            schema.AddVocabularyAnnotation(annotation);

            // Act & Assert for XML
            await VisitAndVerifyXmlAsync(async (v) => await v.VisitEdmSchemaAsync(schema, null).ConfigureAwait(false),
                @"<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Annotations Target=""NS.ComplexType"">
    <Annotation Term=""UI.Thumbnail"" Binary=""4F44617461"" />
    <Annotation Term=""UI.DisplayName"" Int=""42"" />
    <Annotation Term=""UI.DisplayName"" Qualifier=""Tablet"" Int=""88"" />
  </Annotations>
  <Annotations Target=""NS.ComplexType/Name"">
    <Annotation Term=""UI.DisplayName"" Qualifier=""Tablet"" Int=""42"" />
  </Annotations>
</Schema>").ConfigureAwait(false);

            // Act & Assert for JSON
            await VisitAndVerifyJsonAsync(async (v) => await v.VisitEdmSchemaAsync(schema, null).ConfigureAwait(false), @"{
  ""NS"": {
    ""$Annotations"": {
      ""NS.ComplexType"": {
        ""@UI.Thumbnail"": ""T0RhdGE"",
        ""@UI.DisplayName"": 42,
        ""@UI.DisplayName#Tablet"": 88
      },
      ""NS.ComplexType/Name"": {
        ""@UI.DisplayName#Tablet"": 42
      }
    }
  }
}").ConfigureAwait(false);
        }

        [Fact]
        public async Task VerifyOutOfLineAnnotationForEnumMemberWrittenCorrectly_Async()
        {
            // Arrange
            EdmSchema schema = new EdmSchema("NS");

            EdmEnumType enumType = new EdmEnumType("NS", "Color", EdmPrimitiveTypeKind.Int16, true);
            var blue = enumType.AddMember("Blue", new EdmEnumMemberValue(0));

            EdmTerm term = new EdmTerm("UI", "Thumbnail", EdmPrimitiveTypeKind.Binary);
            IEdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(enumType, term, new EdmBinaryConstant(new byte[] { 0x4f, 0x44, 0x61, 0x74, 0x61 }));
            schema.AddVocabularyAnnotation(annotation);

            EdmTerm displayName = new EdmTerm("UI", "DisplayName", EdmPrimitiveTypeKind.Int32);
            annotation = new EdmVocabularyAnnotation(blue, displayName, new EdmIntegerConstant(42));
            schema.AddVocabularyAnnotation(annotation);

            annotation = new EdmVocabularyAnnotation(blue, displayName, "Tablet", new EdmIntegerConstant(88));
            schema.AddVocabularyAnnotation(annotation);

            // Act & Assert for XML
            await VisitAndVerifyXmlAsync(async (v) => await v.VisitEdmSchemaAsync(schema, null).ConfigureAwait(false),
                @"<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Annotations Target=""NS.Color"">
    <Annotation Term=""UI.Thumbnail"" Binary=""4F44617461"" />
  </Annotations>
  <Annotations Target=""NS.Color/Blue"">
    <Annotation Term=""UI.DisplayName"" Int=""42"" />
    <Annotation Term=""UI.DisplayName"" Qualifier=""Tablet"" Int=""88"" />
  </Annotations>
</Schema>").ConfigureAwait(false);

            // Act & Assert for JSON
            await VisitAndVerifyJsonAsync(async (v) => await v.VisitEdmSchemaAsync(schema, null).ConfigureAwait(false), @"{
  ""NS"": {
    ""$Annotations"": {
      ""NS.Color"": {
        ""@UI.Thumbnail"": ""T0RhdGE""
      },
      ""NS.Color/Blue"": {
        ""@UI.DisplayName"": 42,
        ""@UI.DisplayName#Tablet"": 88
      }
    }
  }
}").ConfigureAwait(false);
        }
        #endregion

        internal async Task VisitAndVerifyXmlAsync(Func<EdmModelCsdlSerializationVisitor, Task> testAction, string expected, bool indent = true)
        {
            XmlWriter xmlWriter;
            MemoryStream memStream;

            Version edmxVersion = model.GetEdmxVersion();
            memStream = new MemoryStream();
            xmlWriter = XmlWriter.Create(memStream, new XmlWriterSettings()
            {
                Indent = indent,
                ConformanceLevel = ConformanceLevel.Auto,
                Async = true
            });

            CsdlXmlWriterSettings writerSettings = new CsdlXmlWriterSettings
            {
                LibraryCompatibility = EdmLibraryCompatibility.UseLegacyVariableCasing
            };

            var schemaWriter = new EdmModelCsdlSchemaXmlWriter(model, xmlWriter, edmxVersion, writerSettings);
            var visitor = new EdmModelCsdlSerializationVisitor(model, schemaWriter);

            await testAction(visitor);
            await xmlWriter.FlushAsync().ConfigureAwait(false);

            memStream.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(memStream);

            // Remove extra xml header text as its not needed.
            string result = reader.ReadToEnd().Replace(@"<?xml version=""1.0"" encoding=""utf-8""?>", string.Empty);
            Assert.Equal(expected, result);
        }

        internal async Task VisitAndVerifyJsonAsync(Func<EdmModelCsdlSerializationVisitor, Task> testAction, string expected, bool indent = true, bool wrapper = true)
        {
            Version edmxVersion = this.model.GetEdmxVersion();

            using (MemoryStream memStream = new MemoryStream())
            {
                JsonWriterOptions options = new JsonWriterOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    Indented = indent,
                    SkipValidation = false
                };

                using (Utf8JsonWriter jsonWriter = new Utf8JsonWriter(memStream, options))
                {
                    var csdlSchemaWriter = new EdmModelCsdlSchemaJsonWriter(this.model, jsonWriter, edmxVersion);
                    var visitor = new EdmModelCsdlSerializationVisitor(this.model, csdlSchemaWriter);

                    // Use {} to wrapper the input.
                    if (wrapper)
                    {
                        jsonWriter.WriteStartObject();
                    }

                    await testAction(visitor);

                    if (wrapper)
                    {
                        jsonWriter.WriteEndObject();
                    }

                    await jsonWriter.FlushAsync().ConfigureAwait(false);
                }

                memStream.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(memStream);

                string result = reader.ReadToEnd();
                Assert.Equal(expected, result);
            }
        }
    }
}

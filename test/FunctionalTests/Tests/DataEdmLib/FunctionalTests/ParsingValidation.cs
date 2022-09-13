//---------------------------------------------------------------------
// <copyright file="ParsingValidation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ParsingValidation : EdmLibTestCaseBase
    {
        [TestMethod]
        public void ParseEmptyFile()
        {
            var csdl = @"";
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            XmlReader foo = XmlReader.Create(new StringReader(csdl), settings);
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { foo }, out model, out errors);

            Assert.IsFalse(parsed, "parsed");
            ExpectedEdmErrors expectedErrors = new ExpectedEdmErrors
            {
                {EdmErrorCode.EmptyFile, "XmlParser_EmptyFile"},
            };
            ValidationVerificationHelper.Verify(expectedErrors, errors);
        }

        [TestMethod]
        public void ParseNoNamespace()
        {
            var csdl =
@"<Schema Namespace=""NS1"">
    <EntityContainer Name=""C1"">
        <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
    </EntityContainer>
    <EntityType Name=""Customer"">
        <Key>
            <PropertyRef Name=""CustomerID"" />
        </Key>
        <Property Name=""CustomerID"" Type=""String"" Nullable=""false"" />
    </EntityType>
</Schema>";
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsFalse(parsed, "parsed");
            ExpectedEdmErrors expectedErrors = new ExpectedEdmErrors
            {
                {EdmErrorCode.UnexpectedXmlElement, "XmlParser_UnexpectedRootElementNoNamespace"},
            };
            ValidationVerificationHelper.Verify(expectedErrors, errors);
        }

        [TestMethod]
        public void ParseInvalidNamespace()
        {
            var csdl =
@"<Schema Namespace=""NS1"" xmlns=""http://borkborkbork"">
    <EntityContainer Name=""C1"">
        <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
    </EntityContainer>
    <EntityType Name=""Customer"">
        <Key>
            <PropertyRef Name=""CustomerID"" />
        </Key>
        <Property Name=""CustomerID"" Type=""String"" Nullable=""false"" />
    </EntityType>
</Schema>";
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsFalse(parsed, "parsed");
            ExpectedEdmErrors expectedErrors = new ExpectedEdmErrors
            {
                {EdmErrorCode.UnexpectedXmlElement, "XmlParser_UnexpectedRootElementWrongNamespace"},
            };
            ValidationVerificationHelper.Verify(expectedErrors, errors);
        }

        [TestMethod]
        public void ParseBadRootElement()
        {
            var csdl =
//invalid root element for namespace
@"<EntityContainer Name=""C1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
</EntityContainer>
<EntityType Name=""Customer"">
    <Key>
        <PropertyRef Name=""CustomerID"" />
    </Key>
    <Property Name=""CustomerID"" Type=""String"" Nullable=""false"" />
</EntityType>";
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsFalse(parsed, "parsed");
            ExpectedEdmErrors expectedErrors = new ExpectedEdmErrors
            {
                {EdmErrorCode.UnexpectedXmlElement, "XmlParser_UnexpectedRootElement"},
            };
            ValidationVerificationHelper.Verify(expectedErrors, errors);
        }

        [TestMethod]
        public void ParseUnexpectedAttribute()
        {
            var csdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""C1"" UNEXPECTED=""hello"">
        <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
    </EntityContainer>
    <EntityType Name=""Customer"">
        <Key>
            <PropertyRef Name=""CustomerID"" />
        </Key>
        <Property Name=""CustomerID"" Type=""String"" Nullable=""false"" />
    </EntityType>
</Schema>";
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsFalse(parsed, "parsed");
            ExpectedEdmErrors expectedErrors = new ExpectedEdmErrors
            {
                {EdmErrorCode.UnexpectedXmlAttribute, "XmlParser_UnexpectedAttribute"},
            };
            ValidationVerificationHelper.Verify(expectedErrors, errors);
        }

        [TestMethod]
        public void ParseUnexpectedElement()
        {
            var csdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""C1"">
        <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
        <UNEXPECTED />
    </EntityContainer>
    <EntityType Name=""Customer"">
        <Key>
            <PropertyRef Name=""CustomerID"" />
        </Key>
        <Property Name=""CustomerID"" Type=""String"" Nullable=""false"" />
    </EntityType>
</Schema>";
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsFalse(parsed, "parsed");
            ExpectedEdmErrors expectedErrors = new ExpectedEdmErrors
            {
                {EdmErrorCode.UnexpectedXmlElement, "XmlParser_UnexpectedElement"},
            };
            ValidationVerificationHelper.Verify(expectedErrors, errors);
        }

        [TestMethod]
        public void ParseUnexpectedElementWithClosingTag()
        {
            var csdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""C1"">
        <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
        <UNEXPECTED>
        </UNEXPECTED>
    </EntityContainer>
    <EntityType Name=""Customer"">
        <Key>
            <PropertyRef Name=""CustomerID"" />
        </Key>
        <Property Name=""CustomerID"" Type=""String"" Nullable=""false"" />
    </EntityType>
</Schema>";
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsFalse(parsed, "parsed");
            ExpectedEdmErrors expectedErrors = new ExpectedEdmErrors
            {
                {EdmErrorCode.UnexpectedXmlElement, "XmlParser_UnexpectedElement"},
            };
            ValidationVerificationHelper.Verify(expectedErrors, errors);
        }

        [TestMethod]
        public void ParseMissingTypeAttributeOrElementTest()
        {
            var csdl =
@"<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <Action Name=""FunctionImportsWithReturnTypePrimitiveDataType1""><ReturnType Type=""Edm.Binary""/>
        <Parameter Name=""PrimitiveParameter1"" />
      </Action>
      <EntityContainer Name=""MyContainer"">
        <ActionImport Name=""FunctionImportsWithReturnTypePrimitiveDataType1"" Action=""DefaultNamespace.FunctionImportsWithReturnTypePrimitiveDataType1"" />
      </EntityContainer>
</Schema>";
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsFalse(parsed, "parsed");
            ExpectedEdmErrors expectedErrors = new ExpectedEdmErrors
            {
                {EdmErrorCode.MissingType, "CsdlParser_MissingTypeAttributeOrElement"},
            };
            ValidationVerificationHelper.Verify(expectedErrors, errors);
        }

        //Duplicate dependent element
        [TestMethod]
        public void ParseDuplicateDependentElementForTheSameReferentialConstaint()
        {
            const string csdl =
                @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Hot"" Alias=""Fuzz"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Address"" Type=""String"" MaxLength=""100"" />
        <NavigationProperty Name=""ToPet"" Type=""Hot.Pet"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Pet"">
        <Key>
            <PropertyRef Name=""PetId"" />
        </Key>
        <Property Name=""PetId"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""OwnerId"" Type=""Int32"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""Hot.Person"" Partner=""ToPet"" >
            <ReferentialConstraint Property=""OwnerId"" Property=""OwnerId"" ReferencedProperty=""Id"" />
        </NavigationProperty>
    </EntityType>
    <EntityContainer Name=""Wild"">
        <EntitySet Name=""People"" EntityType=""Hot.Person"" />
    </EntityContainer>
    <EntityContainer Name=""Zero"" Extends=""Wild"">
        <EntitySet Name=""Pets"" EntityType=""Hot.Pet"" />
    </EntityContainer>
</Schema>";
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsFalse(parsed, "parsed");
            Assert.AreEqual(1, errors.Count(), "1 errors");
            Assert.AreEqual(EdmErrorCode.XmlError, errors.First().ErrorCode, "Correct error.");
            Assert.IsTrue(errors.First().ErrorMessage.Contains("'Property' is a duplicate attribute name."), "Correct error message");
        }

        //Duplicate principal element
        [TestMethod]
        public void ParseDuplicatePrincipalattributeForTheSameReferentialConstaint()
        {
            const string csdl =
           @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Hot"" Alias=""Fuzz"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Address"" Type=""String"" MaxLength=""100"" />
        <NavigationProperty Name=""ToPet"" Type=""Hot.Pet"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Pet"">
        <Key>
            <PropertyRef Name=""PetId"" />
        </Key>
        <Property Name=""PetId"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""OwnerId"" Type=""Int32"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""Hot.Person"" Partner=""ToPet"" >
            <ReferentialConstraint Property=""OwnerId"" ReferencedProperty=""Id"" ReferencedProperty=""Id""/>
        </NavigationProperty>
    </EntityType>
    <EntityContainer Name=""Wild"">
        <EntitySet Name=""People"" EntityType=""Hot.Person"" />
    </EntityContainer>
    <EntityContainer Name=""Zero"" Extends=""Wild"">
        <EntitySet Name=""Pets"" EntityType=""Hot.Pet"" />
    </EntityContainer>
</Schema>";
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsFalse(parsed, "parsed");
            Assert.AreEqual(1, errors.Count(), "1 errors");
            Assert.AreEqual(EdmErrorCode.XmlError, errors.First().ErrorCode, "Correct error code.");
            Assert.IsTrue(errors.First().ErrorMessage.Contains("'ReferencedProperty' is a duplicate attribute name."), "Correct error message");
        }
     
        [TestMethod]
        public void ParseUnexpectedText()
        {
            var csdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""C1"">
        <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" >
        UNEXPECTED TEXT!!!
        </EntitySet>
    </EntityContainer>
    <EntityType Name=""Customer"">
        <Key>
            <PropertyRef Name=""CustomerID"" />
        </Key>
        <Property Name=""CustomerID"" Type=""String"" Nullable=""false"" />
    </EntityType>
</Schema>";
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsFalse(parsed, "parsed");
            ExpectedEdmErrors expectedErrors = new ExpectedEdmErrors
            {
                {EdmErrorCode.TextNotAllowed, "XmlParser_TextNotAllowed"},
            };
            ValidationVerificationHelper.Verify(expectedErrors, errors);
        }

        [TestMethod]
        public void OptionalAttributeWithEmptyText()
        {
            var csdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""C1"">
        <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" >
        </EntitySet>
    </EntityContainer>
    <EntityType Name=""Customer"">
        <Key>
            <PropertyRef Name=""CustomerID"" />
        </Key>
        <Property Name=""CustomerID"" Type=""String"" Nullable="""" />
    </EntityType>
</Schema>";
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsFalse(parsed, "parsed");
            ExpectedEdmErrors expectedErrors = new ExpectedEdmErrors
            {
                {EdmErrorCode.InvalidBoolean, "ValueParser_InvalidBoolean"},
            };
            ValidationVerificationHelper.Verify(expectedErrors, errors);
        }

        [TestMethod]
        public void SchemaMissingRequiredAttribute()
        {
            var csdl =
@"<Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Customer"">
        <Key>
            <PropertyRef Name=""CustomerID"" />
        </Key>
        <Property Name=""CustomerID"" Type=""String"" Nullable=""false"" />
    </EntityType>
</Schema>";
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsTrue(parsed, "parsed");
            IEdmEntityType customer = model.SchemaElements.First() as IEdmEntityType;
            Assert.AreEqual(string.Empty, customer.Namespace, "Empty namespace");
            Assert.AreEqual("Customer", customer.Name, "Correct name");
        }

        [TestMethod]
        public void SchemaRequiredAttributeWithEmptyText()
        {
            var csdl =
@"<Schema Namespace="""" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""C1"">
        <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" >
        </EntitySet>
    </EntityContainer>
</Schema>";
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsTrue(parsed, "parsed");
            ExpectedEdmErrors expectedErrors = new ExpectedEdmErrors
            {
            };
            ValidationVerificationHelper.Verify(expectedErrors, errors);
        }

        [TestMethod]
        public void EntitySetMissingRequiredAttribute()
        {
            var csdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""C1"">
        <EntitySet EntityType=""NS1.Customer"">
        </EntitySet>
    </EntityContainer>
    <EntityType Name=""Customer"">
        <Key>
            <PropertyRef Name=""CustomerID"" />
        </Key>
        <Property Name=""CustomerID"" Type=""String"" Nullable=""false"" />
    </EntityType>
</Schema>";
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsFalse(parsed, "parsed");
            ExpectedEdmErrors expectedErrors = new ExpectedEdmErrors
            {
                {EdmErrorCode.MissingAttribute, "XmlParser_MissingAttribute"},
            };
            ValidationVerificationHelper.Verify(expectedErrors, errors);
        }

        [TestMethod]
        public void EntitySetRequiredAttributeWithEmptyText()
        {
            var csdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""C1"">
        <EntitySet Name="""" EntityType=""NS1.Customer"" >
        </EntitySet>
    </EntityContainer>
    <EntityType Name=""Customer"">
        <Key>
            <PropertyRef Name=""CustomerID"" />
        </Key>
        <Property Name=""CustomerID"" Type=""String"" Nullable=""false"" />
    </EntityType>
</Schema>";
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsTrue(parsed, "parsed");
            ExpectedEdmErrors expectedErrors = new ExpectedEdmErrors
            {
            };
            ValidationVerificationHelper.Verify(expectedErrors, errors);
        }

        // TODO: Make parser fail on invalid text
        [TestMethod]
        public void BooleanWithBadText()
        {
            var csdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""C1"">
        <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" >
        </EntitySet>
    </EntityContainer>
    <EntityType Name=""Customer"">
        <Key>
            <PropertyRef Name=""CustomerID"" />
        </Key>
        <Property Name=""CustomerID"" Type=""String"" Nullable=""asdf"" />
    </EntityType>
</Schema>";
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsFalse(parsed, "parsed");
            Assert.AreEqual(1, errors.Count(), "1 errors");
            Assert.AreEqual(EdmErrorCode.InvalidBoolean, errors.First().ErrorCode, "11: Text not allowed");
        }

        [TestMethod]
        public void EdmxParseUnexpectedAttribute()
        {
            var edmx =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
    <edmx:DataServices>
    <Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityContainer Name=""C1"" UNEXPECTED=""hello"">
            <EntitySet Name=""Customers"" EntityType=""NS1.Customer"" />
        </EntityContainer>
        <EntityType Name=""Customer"">
            <Key>
                <PropertyRef Name=""CustomerID"" />
            </Key>
            <Property Name=""CustomerID"" Type=""String"" Nullable=""false"" />
        </EntityType>
    </Schema>
    </edmx:DataServices>
</edmx:Edmx>";
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = CsdlReader.TryParse(XmlReader.Create(new StringReader(edmx)), out model, out errors);

            Assert.IsFalse(parsed, "parsed");
            ExpectedEdmErrors expectedErrors = new ExpectedEdmErrors
            {
                {EdmErrorCode.UnexpectedXmlAttribute, "XmlParser_UnexpectedAttribute"},
            };
            ValidationVerificationHelper.Verify(expectedErrors, errors);
        }

        [TestMethod]
        public void ParseUnexpectedElement_VocabularyAnnotation()
        {
            var csdl =
@"<Schema Namespace=""NS1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Customer"">
    <Annotation Term=""foo.Person"">
      <PropertyValue Property=""foo"" String=""zippy"" />
    </Annotation>
  </EntityType>
</Schema>";
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsFalse(parsed, "parsed");
            ExpectedEdmErrors expectedErrors = new ExpectedEdmErrors
            {
                {EdmErrorCode.UnexpectedXmlElement, "XmlParser_UnusedElement"},
            };
            ValidationVerificationHelper.Verify(expectedErrors, errors);
        }

        [TestMethod]
        public void ParseIfExpressionTooManyOperands()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Name"" />
    </Key>
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
    <Property Name=""Birthday"" Type=""Edm.DateTimeOffset"" />
    <Annotation Term=""Funk.RC"">
      <If>
        <Bool>true</Bool>
        <Int>123</Int>
        <Float>3.14</Float>
        <Bool>false</Bool>
      </If>
    </Annotation>
  </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsFalse(parsed, "parsed");
            ExpectedEdmErrors expectedErrors = new ExpectedEdmErrors
            {
                {EdmErrorCode.InvalidIfExpressionIncorrectNumberOfOperands, "CsdlParser_InvalidIfExpressionIncorrectNumberOfOperands"},
            };
            ValidationVerificationHelper.Verify(expectedErrors, errors);
        }

        [TestMethod]
        public void ParseIfExpressionTooFewOperands()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Name"" />
    </Key>
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
    <Property Name=""Birthday"" Type=""Edm.DateTimeOffset"" />
    <Annotation Term=""Funk.RC"">
      <If>
        <Bool>true</Bool>
        <Int>123</Int>
      </If>
    </Annotation>
  </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            Assert.IsFalse(parsed, "parsed");
            ExpectedEdmErrors expectedErrors = new ExpectedEdmErrors
            {
                {EdmErrorCode.InvalidIfExpressionIncorrectNumberOfOperands, "CsdlParser_InvalidIfExpressionIncorrectNumberOfOperands"},
            };
            ValidationVerificationHelper.Verify(expectedErrors, errors);
        }

        [TestMethod]
        public void EdmxParseEmptyFile()
        {
            var edmx = @"";
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            XmlReader foo = XmlReader.Create(new StringReader(edmx), settings);
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = CsdlReader.TryParse(foo, out model, out errors);

            Assert.IsFalse(parsed, "parsed");
            ExpectedEdmErrors expectedErrors = new ExpectedEdmErrors
            {
                {EdmErrorCode.EmptyFile, "XmlParser_EmptySchemaTextReader"},
            };
            ValidationVerificationHelper.Verify(expectedErrors, errors);
        }

        [TestMethod]
        public void EdmxParseUnexpectedRootElement()
        {
            var edmx = @"<edmx:BorkBorkBork xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx""/>";
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            XmlReader foo = XmlReader.Create(new StringReader(edmx), settings);
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = CsdlReader.TryParse(foo, out model, out errors);

            Assert.IsFalse(parsed, "parsed");
            ExpectedEdmErrors expectedErrors = new ExpectedEdmErrors
            {
                {EdmErrorCode.UnexpectedXmlElement, "XmlParser_UnexpectedRootElement"},
            };
            ValidationVerificationHelper.Verify(expectedErrors, errors);
        }

        [TestMethod]
        public void EdmxParseUnexpectedNamespace()
        {
            var edmx = @"<edmx:Edmx xmlns:edmx=""http://BorkBorkBork""/>";
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            XmlReader foo = XmlReader.Create(new StringReader(edmx), settings);
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = CsdlReader.TryParse(foo, out model, out errors);

            Assert.IsFalse(parsed, "parsed");
            ExpectedEdmErrors expectedErrors = new ExpectedEdmErrors
            {
                {EdmErrorCode.UnexpectedXmlElement, "XmlParser_UnexpectedRootElement"},
            };
            ValidationVerificationHelper.Verify(expectedErrors, errors);
        }

        [TestMethod]
        public void ParseInvalidFunctionImportParameterMode()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTime"" />
    </EntityType>
    <EntityContainer Name=""fooContainer"" >
        <EntitySet Name=""People"" EntityType=""foo.Person"" />
        <FunctionImport Name=""peopleWhoAreAwesome"" EntitySet=""People""><ReturnType Type=""Collection(foo.Person)""/>
            <Parameter Name=""awesomeName"" Mode=""Whargarble"" Type=""String"" />
        </FunctionImport>
    </EntityContainer>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsFalse(parsed, "parsed");
        }

        [TestMethod]
        public void BadPropertyTests()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""BadProp"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
    </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType entityType = (IEdmEntityType)model.SchemaElements.First();
            IEdmStructuralProperty badProp = entityType.DeclaredKey.First();
            Assert.AreEqual("BadProp", badProp.Name, "Bad property keeps name");
            Assert.AreEqual(entityType, badProp.DeclaringType, "Correct declaring type");
            Assert.AreEqual(EdmPropertyKind.None, badProp.PropertyKind, "Bad Prop has prop kind of none");
            Assert.IsNull(badProp.DefaultValueString, "Bad prop has no default value");
            IEdmTypeReference badType = badProp.Type;
            Assert.AreEqual(EdmTypeKind.None, badType.TypeKind(), "BadProp type set to none");
            Assert.IsTrue(badType.IsBad(), "Bad prop has bad type");
            Assert.AreEqual(EdmErrorCode.BadUnresolvedProperty, badProp.Errors().First().ErrorCode, "Bad prop is unresolved.");
            Assert.AreEqual(EdmErrorCode.BadUnresolvedProperty, badType.Definition.Errors().First().ErrorCode, "Bad type is bad by association.");
        }

        [TestMethod]
        public void NullReaderTest()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTime"" />
    </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)), null }, out model, out errors);
            Assert.IsFalse(parsed, "parsed");
            Assert.AreEqual(1, errors.Count(), "Error found");
            Assert.AreEqual(EdmErrorCode.NullXmlReader, errors.First().ErrorCode, "Correct error");
        }

        [TestMethod]
        public void TestVocabularyAnnotations()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Age"" Type=""Int32"" />
    <Term Name=""Subject"" Type=""foo.Person"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Name"" Type=""String"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
        <Annotation Term=""foo.BORKBORKBORK"" Qualifier=""First"" Int=""123"" />
        <Annotation Term=""foo.Age"" Qualifier=""Best"">
            <Int>456</Int>
        </Annotation>
        <Annotation Term=""Funk.Mage"" Int=""789"" />
    </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;

            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType person = (IEdmEntityType)model.FindType("foo.Person");
            IEnumerable<IEdmVocabularyAnnotation> personAnnotations = person.VocabularyAnnotations(model);
            Assert.AreEqual(personAnnotations.Count(), 3, "Annotations count");

            IEdmVocabularyAnnotation[] annotations = personAnnotations.ToArray<IEdmVocabularyAnnotation>();
            IEdmTerm unresolvedTerm = annotations.First().Term;
            Assert.IsTrue(unresolvedTerm.Type.TypeKind() == EdmTypeKind.None, "Bad term has bad type");
            Assert.AreEqual(EdmSchemaElementKind.Term, unresolvedTerm.SchemaElementKind, "Unresolved term has correct schema element kind.");
        }

        [TestMethod]
        public void NoReaderTest()
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { }, out model, out errors);
            Assert.IsFalse(parsed, "parsed");
            Assert.AreEqual(1, errors.Count(), "Error found");
            Assert.AreEqual(EdmErrorCode.NoReadersProvided, errors.First().ErrorCode, "Correct error");
        }

        [TestMethod]
        public void ParseBadInteger()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Person"">
        <Property Name=""Foo"" Type=""Decimal"" Precision=""BAD"" Nullable=""false"" />
    </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsFalse(parsed, "parsed");
            Assert.AreEqual(1, errors.Count(), "Correct number of errors");
            Assert.AreEqual(EdmErrorCode.InvalidInteger, errors.First().ErrorCode, "Correct error code.");
        }

        [TestMethod]
        public void ParseBadLong()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EnumType Name=""Color"">
      <Member Name=""Red"" Value=""BAD""/>
      <Member Name=""Green""/>
      <Member Name=""Blue"" Value=""5""/>
      <Member Name=""Yellow""/>
    </EnumType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsFalse(parsed, "parsed");
            Assert.AreEqual(1, errors.Count(), "Correct number of errors");
            Assert.AreEqual(EdmErrorCode.InvalidLong, errors.First().ErrorCode, "Correct error code.");
        }

        [TestMethod]
        public void ParseBadSrid()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Person"">
        <Property Name=""Foo"" Type=""Geometry"" SRID=""BAD"" Nullable=""false"" />
    </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsFalse(parsed, "parsed");
            Assert.AreEqual(1, errors.Count(), "Correct number of errors");
            Assert.AreEqual(EdmErrorCode.InvalidSrid, errors.First().ErrorCode, "Correct error code.");
        }

        [TestMethod]
        public void ParseBadMaxLength()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Person"">
        <Property Name=""Foo"" Type=""String"" MaxLength=""BAD"" Nullable=""false"" />
    </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsFalse(parsed, "parsed");
            Assert.AreEqual(1, errors.Count(), "Correct number of errors");
            Assert.AreEqual(EdmErrorCode.InvalidMaxLength, errors.First().ErrorCode, "Correct error code.");
        }

        [TestMethod]
        public void ParseMissingMaxLength()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Person"">
        <Property Name=""Foo"" Type=""String"" Nullable=""false"" />
    </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.AreEqual(0, errors.Count(), "Correct number of errors");
            Assert.IsNull(((IEdmComplexType)model.FindType("Grumble.Person")).Properties().First().Type.AsString().MaxLength, "MaxLength is null.");
        }

        [TestMethod]
        public void ParseNoneOnDelete()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Cold"" Alias=""Comfort"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Feckless"">
    <Key>
      <PropertyRef Name=""Id"" />
      <PropertyRef Name=""Ego"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Ego"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Bonus"" Type=""String"" MaxLength=""1001"" />
    <NavigationProperty Name=""MyReckless"" Type=""Collection(Comfort.MyFecklesses)"" Partner=""MyFecklesses"">
      <OnDelete Action=""None"" />
    </NavigationProperty>
  </EntityType>
  <EntityType Name=""Reckless"">
    <Key>
      <PropertyRef Name=""Id"" />
      <PropertyRef Name=""AlterEgo"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""AlterEgo"" Type=""Int32"" Nullable=""false"" />
    <NavigationProperty Name=""MyFecklesses"" Type=""Comfort.MyReckless"" Partner=""MyReckless"" >
      <ReferentialConstraint Property=""AlterEgo"" ReferencedProperty=""Ego"" />
    </NavigationProperty>
  </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No errors");

            IEdmEntityType feckless = (IEdmEntityType)model.SchemaElements.First();
            IEdmNavigationProperty end1 = feckless.NavigationProperties().First();
            Assert.AreEqual(EdmOnDeleteAction.None, end1.OnDelete, "End1 correct action");
        }

        [TestMethod]
        public void ParseBadOnDelete()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Cold"" Alias=""Comfort"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Feckless"">
    <Key>
      <PropertyRef Name=""Id"" />
      <PropertyRef Name=""Ego"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Ego"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Bonus"" Type=""String"" MaxLength=""1001"" />
    <NavigationProperty Name=""MyReckless"" Type=""Collection(Comfort.MyFecklesses)"" Partner=""MyFecklesses"">
      <OnDelete Action=""BADBADBAD"" />
    </NavigationProperty>
  </EntityType>
  <EntityType Name=""Reckless"">
    <Key>
      <PropertyRef Name=""Id"" />
      <PropertyRef Name=""AlterEgo"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""AlterEgo"" Type=""Int32"" Nullable=""false"" />
    <NavigationProperty Name=""MyFecklesses"" Type=""Comfort.MyReckless"" Partner=""MyReckless"" >
      <ReferentialConstraint Property=""AlterEgo"" ReferencedProperty=""Ego"" />
    </NavigationProperty>
  </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsFalse(parsed, "parsed");
            Assert.AreEqual(1, errors.Count(), "Correct number of errors");
            Assert.AreEqual(EdmErrorCode.InvalidOnDelete, errors.First().ErrorCode, "Correct error code.");
        }

        [TestMethod]
        public void ParseBadBool()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Person"" Abstract=""BorkBorkBork"">
        <Property Name=""Foo"" Type=""String"" Nullable=""false"" />
    </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsFalse(parsed, "parsed");
            Assert.AreEqual(1, errors.Count(), "Correct number of errors");
            Assert.AreEqual(EdmErrorCode.InvalidBoolean, errors.First().ErrorCode, "Correct error code.");
        }

        [TestMethod]
        public void ParseBadRefAsBadType()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Person"">
        <Property Name=""Foo"" Type=""Ref"" />
    </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsFalse(parsed, "parsed");
            Assert.AreEqual(1, errors.Count(), "Correct number of errors");
            Assert.AreEqual(EdmErrorCode.InvalidTypeName, errors.First().ErrorCode, "Correct error code.");
        }

        [TestMethod]
        public void ParseUnqualifiedType()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Person"" >
        <Property Name=""Foo"" Type=""Person"" />
    </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsFalse(parsed, "parsed");
            Assert.AreEqual(1, errors.Count(), "Correct number of errors");
            Assert.AreEqual(EdmErrorCode.InvalidTypeName, errors.First().ErrorCode, "Correct error code.");
        }

        [TestMethod]
        public void ParseDuplicateKeyElement()
        {
            string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Clod"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""ID"" Type=""Int32"" />
  </EntityType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsFalse(parsed, "parsed");
            Assert.AreEqual(1, errors.Count(), "Correct number of errors");
            Assert.AreEqual(EdmErrorCode.UnexpectedXmlElement, errors.First().ErrorCode, "Correct error code.");
        }

        [TestMethod]
        public void ParseDuplicateExpressionElement()
        {
            var csdl1 =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Vocab"" Alias=""Vocabulary"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Term Name=""PrimitiveTerm"" Type=""Edm.Int32"" />
</Schema>
";
            var csdl2 =
    @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CSDL"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplex"">
    <Property Name=""ID"" Type=""Edm.Int32"" />
    <Annotation Term=""Vocabulary.PrimitiveTerm"">
      <String>string 1</String>
      <String>string 2</String>
    </Annotation>
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl1)), XmlReader.Create(new StringReader(csdl2)) }, out model, out errors);
            Assert.IsFalse(parsed, "parsed");
            Assert.AreEqual(1, errors.Count(), "Correct number of errors");
            Assert.AreEqual(EdmErrorCode.UnexpectedXmlElement, errors.First().ErrorCode, "Correct error code.");
        }

        [TestMethod]
        public void ParseValidateODataTestModelBasicTest()
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(ODataTestModelBuilder.ODataTestModelBasicTest.Select(x => x.CreateReader()), out model, out errors);

            Assert.IsTrue(parsed, "Parsing failed.");
            Assert.IsTrue(errors.Count() == 0, "An error occured when parsing.");

            CheckModelElementCount(model, 1 /* entityContainerCount */, 9 /* schemaElementCount */, 8 /* typeDefinitionCount */, 0 /* operationCount */, 1 /* actionCount */);
            CheckModelEntityContainer(model.EntityContainer.Elements, 4 /* entityContainerElementCount */, 3 /* entitySetCount */, 0 /* operationImportCount */, 1 /* actionImportCount */);

            var person = model.FindEntityType("TestModel.Person");
            Assert.IsNotNull(person, "Invalid entity type.");
            Assert.IsNull(person.BaseType, "Entity type has invalid base type.");

            var employee = model.FindEntityType("TestModel.Employee");
            Assert.IsNotNull(employee, "Invalid entity type.");
            Assert.AreEqual(person, employee.BaseType, "Entity type has invalid base type.");

            var manager = model.FindEntityType("TestModel.Manager");
            Assert.IsNotNull(manager, "Invalid entity type.");
            Assert.AreEqual(employee, manager.BaseType, "Entity type has invalid base type.");

            var friend = person.FindProperty("Friend") as IEdmNavigationProperty;
            Assert.IsNotNull(friend, "Navigation property exists.");
        }

        [TestMethod]
        public void ParseValidateODataTestModelAnnotationTestWithAnnotations()
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(ODataTestModelBuilder.ODataTestModelAnnotationTestWithAnnotations.Select(x => x.CreateReader()), out model, out errors);

            Assert.IsTrue(parsed, "Parsing failed.");
            Assert.IsTrue(errors.Count() == 0, "An error occured when parsing.");

            CheckModelElementCount(model, 1, 3, 2, 0, 1);
            CheckModelEntityContainer(model.EntityContainer.Elements, 3, 2, 0, 1);

            var address = model.FindType("TestModel.Address") as IEdmComplexType;
            Assert.IsNotNull(address, "Invalid complex type");
            Assert.AreEqual(2, address.Properties().Count(), "Invalid property count for complex type.");

            var zipProperty = address.Properties().Where(x => x.Name == "Zip").SingleOrDefault();
            Assert.IsNotNull(zipProperty, "Invalid complex type property.");
            Assert.AreEqual(2, model.DirectValueAnnotations(zipProperty).Count(), "Invalid immediate annotation count for complex type property.");

            var firstEntityContainer = model.EntityContainer;
            Assert.IsNotNull(firstEntityContainer, "Invalid entity container.");
            Assert.AreEqual(0, model.DirectValueAnnotations(firstEntityContainer).Count(), "Invalid immediate annotation count of entity container.");

            var serviceOperation = firstEntityContainer.FindOperationImports("ServiceOperation1").SingleOrDefault();
            Assert.IsNotNull(serviceOperation, "Invalid import function.");
            Assert.AreEqual(2, model.DirectValueAnnotations(serviceOperation).Count(), "Invalid immediate annotation count of function import.");
            CheckImmediateAnnotation(model.DirectValueAnnotations(serviceOperation).First(), "MimeType", "img/jpeg");
        }

        [TestMethod]
        public void ParseValidateODataTestModelAnnotationTestWithoutAnnotations()
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(ODataTestModelBuilder.ODataTestModelAnnotationTestWithoutAnnotations.Select(x => x.CreateReader()), out model, out errors);

            Assert.IsTrue(parsed, "Parsing failed.");
            Assert.IsTrue(errors.Count() == 0, "An error occured when parsing.");

            CheckModelElementCount(model, 1, 3, 2, 0, 1);
            CheckModelEntityContainer(model.EntityContainer.Elements, 3, 2, 0, 1);

            var firstEntityContainer = model.EntityContainer;
            Assert.IsNotNull(firstEntityContainer, "Invalid entity container.");
            Assert.AreEqual(0, model.DirectValueAnnotations(firstEntityContainer).Count(), "Invalid immediate annotation count of entity container.");

            var serviceOperation = firstEntityContainer.FindOperationImports("ServiceOperation1").SingleOrDefault();
            Assert.IsNotNull(serviceOperation, "Invalid import function.");
            Assert.AreEqual(0, model.DirectValueAnnotations(serviceOperation).Count(), "Invalid immediate annotation count of function import.");
        }

        [TestMethod]
        public void ParseValidateODataTestModelWithOperationImport()
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(ODataTestModelBuilder.ODataTestModelWithFunctionImport.Select(x => x.CreateReader()), out model, out errors);

            Assert.IsTrue(parsed, "Parsing failed.");
            Assert.IsTrue(errors.Count() == 0, "An error occured when parsing.");

            CheckModelElementCount(model, 1, 11, 3, 0, 8);
            CheckModelEntityContainer(model.EntityContainer.Elements, 8, 0, 0, 8);

            var enumType = model.SchemaElements.Where(x => x.Name.Equals("EnumType")).SingleOrDefault() as IEdmEnumType;
            Assert.IsNotNull(enumType, "Invalid enum type.");

            var firstEntityContainer = model.EntityContainer;
            Assert.IsNotNull(firstEntityContainer, "Invalid entity container.");

            var enumOperation = model.FindOperations("TestNS.FunctionImport_Enum").SingleOrDefault();
            Assert.IsNotNull(enumOperation, "Invalid operation.");
            Assert.AreEqual(1, enumOperation.Parameters.Count(), "Invalid parameter count for operation");
            var enumParameter = enumOperation.Parameters.First();
            Assert.AreEqual(enumType, enumParameter.Type.Definition, "Invalid parameter type.");

            var enumOperationImport = firstEntityContainer.FindOperationImports("FunctionImport_Enum").SingleOrDefault();
            Assert.IsNotNull(enumOperationImport, "Invalid import operation.");
        }

        [TestMethod]
        public void ParseValidateODataTestModelDefaultModel()
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(ODataTestModelBuilder.ODataTestModelDefaultModel.Select(x => x.CreateReader()), out model, out errors);

            Assert.IsTrue(parsed, "Parsing failed.");
            Assert.IsTrue(errors.Count() == 0, "An error occured when parsing.");

            CheckModelElementCount(model, 1, 54, 41 + 7, 5, 1);
            CheckModelEntityContainer(model.EntityContainer.Elements, 35, 35, 0, 0);

            var plusOneOperation = model.SchemaElements.Where(x => x.SchemaElementKind.Equals(EdmSchemaElementKind.Function) && x.Name.Equals("GetArgumentPlusOne")).SingleOrDefault() as IEdmOperation;
            Assert.IsNotNull(plusOneOperation, "Invalid operation.");
            Assert.IsTrue(plusOneOperation.ReturnType.IsInt32(), "Invalid operation return type.");
            Assert.AreEqual(1, plusOneOperation.Parameters.Count(), "Invalid parameter count for operation.");
            Assert.AreEqual("arg1", plusOneOperation.Parameters.First().Name, "Invalid parameter name for operation.");
            Assert.IsTrue(plusOneOperation.Parameters.First().Type.IsInt32(), "Invalid parameter type for operation.");

            var stringOperation = model.SchemaElements.Where(x => x.SchemaElementKind.Equals(EdmSchemaElementKind.Function) && x.Name.Equals("GetPrimitiveString")).SingleOrDefault() as IEdmOperation;
            Assert.IsNotNull(stringOperation, "Invalid operation.");
            Assert.AreEqual(0, stringOperation.Parameters.Count(), "Invalid parameter count for operation.");
        }

        [TestMethod]
        public void ParseValidateODataTestModelEmptyModel()
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(ODataTestModelBuilder.ODataTestModelEmptyModel.Select(x => x.CreateReader()), out model, out errors);

            Assert.IsTrue(parsed, "Parsing failed.");
            Assert.IsTrue(errors.Count() == 0, "An error occured when parsing.");

            CheckModelElementCount(model, 1, 0, 0, 0);
            CheckModelEntityContainer(model.EntityContainer.Elements, 0, 0, 0);
        }

        [TestMethod]
        public void ParseValidateODataTestModelWithSingleEntityType()
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(ODataTestModelBuilder.ODataTestModelWithSingleEntityType.Select(x => x.CreateReader()), out model, out errors);

            Assert.IsTrue(parsed, "Parsing failed.");
            Assert.IsTrue(errors.Count() == 0, "An error occured when parsing.");

            CheckModelElementCount(model, 1, 1, 1, 0);
            CheckModelEntityContainer(model.EntityContainer.Elements, 1, 1, 0);
        }

        [TestMethod]
        public void ParseValidateODataTestModelWithSingleComplexType()
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(ODataTestModelBuilder.ODataTestModelWithSingleComplexType.Select(x => x.CreateReader()), out model, out errors);

            Assert.IsTrue(parsed, "Parsing failed.");
            Assert.IsTrue(errors.Count() == 0, "An error occured when parsing.");

            CheckModelElementCount(model, 1, 1, 1, 0);
            CheckModelEntityContainer(model.EntityContainer.Elements, 0, 0, 0);
        }

        [TestMethod]
        public void ParseValidateODataTestModelWithMultiValueProperty()
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(ODataTestModelBuilder.ODataTestModelWithCollectionProperty.Select(x => x.CreateReader()), out model, out errors);

            Assert.IsTrue(parsed, "Parsing failed.");
            Assert.IsTrue(errors.Count() == 0, "An error occured when parsing.");

            CheckModelElementCount(model, 1, 1, 1, 0);
            CheckModelEntityContainer(model.EntityContainer.Elements, 0, 0, 0);
        }

        [TestMethod]
        public void ParseValidateODataTestModelWithOpenType()
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(ODataTestModelBuilder.ODataTestModelWithOpenType.Select(x => x.CreateReader()), out model, out errors);

            Assert.IsTrue(parsed, "Parsing failed.");
            Assert.IsTrue(errors.Count() == 0, "An error occured when parsing.");

            CheckModelElementCount(model, 1, 1, 1, 0);
            CheckModelEntityContainer(model.EntityContainer.Elements, 1, 1, 0);

            var openEntityType = model.FindEntityType("TestModel.OpenEntityType");
            Assert.IsNotNull(openEntityType, "Invalid entity type.");
            Assert.AreEqual(true, openEntityType.IsOpen, "Invalid OpenType attribute value.");

            var openEntitySet = model.EntityContainer.FindEntitySet("OpenEntityType");
            Assert.IsNotNull(openEntityType, "Invalid entity set.");
            Assert.AreEqual(openEntityType, openEntitySet.EntityType(), "Invalid entity set type.");
        }

        [TestMethod]
        public void ParseValidateODataTestModelWithNamedStream()
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(ODataTestModelBuilder.ODataTestModelWithNamedStream.Select(x => x.CreateReader()), out model, out errors);

            Assert.IsTrue(parsed, "Parsing failed.");
            Assert.IsTrue(errors.Count() == 0, "An error occured when parsing.");

            CheckModelElementCount(model, 1, 1, 1, 0);
            CheckModelEntityContainer(model.EntityContainer.Elements, 1, 1, 0);

            var streamEntityType = model.FindEntityType("TestModel.NamedStreamEntityType");
            Assert.IsNotNull(streamEntityType, "Invalid entity type.");
            Assert.AreEqual(2, streamEntityType.Properties().Count(), "Invalid property count for entity type.");

            var streamProperty = streamEntityType.Properties().Where(x => x.Name.Equals("NamedStream")).SingleOrDefault();
            Assert.IsNotNull(streamProperty, "Invalid entity type property.");
            Assert.IsTrue(streamProperty.Type.IsStream(), "Invalid property type.");

            var streamSet = model.EntityContainer.FindEntitySet("NamedStreamEntityType");
            Assert.IsNotNull(streamSet, "Invalid entity set.");
            Assert.AreEqual(streamEntityType, streamSet.EntityType(), "Invalid element type.");
        }

        private void CheckImmediateAnnotation(IEdmDirectValueAnnotation immediateAnnotation, string termName, string value)
        {
            Assert.AreEqual(termName, immediateAnnotation.Name, "Invalid immediate annotation term.");
            Assert.AreEqual(value, ((EdmStringConstant)immediateAnnotation.Value).Value, "Invalid immediate annotation value.");
        }

        private void CheckModelEntityContainer(IEnumerable<IEdmEntityContainerElement> entityContainerElements, int entityContainerElementCount, int entitySetCount, int operationImportCount)
        {
            this.CheckModelEntityContainer(entityContainerElements, entityContainerElementCount, entitySetCount, operationImportCount, 0);
        }

        private void CheckModelEntityContainer(IEnumerable<IEdmEntityContainerElement> entityContainerElements, int entityContainerElementCount, int entitySetCount, int operationImportCount, int actionImportCount)
        {
            Assert.AreEqual(entityContainerElementCount, entityContainerElements.Count(), "Invalid entity container element count.");
            Assert.AreEqual(entitySetCount, entityContainerElements.Where(x => x.ContainerElementKind.Equals(EdmContainerElementKind.EntitySet)).Count(), "Invalid entity set count.");
            Assert.AreEqual(operationImportCount, entityContainerElements.Where(x => x.ContainerElementKind.Equals(EdmContainerElementKind.FunctionImport)).Count(), "Invalid function import count.");
            Assert.AreEqual(actionImportCount, entityContainerElements.Where(x => x.ContainerElementKind.Equals(EdmContainerElementKind.ActionImport)).Count(), "Invalid action import count.");
        }

        private void CheckModelElementCount(IEdmModel model, int entityContainerCount, int schemaElementCount, int typeDefinitionCount, int operationCount)
        {
            this.CheckModelElementCount(model, entityContainerCount, schemaElementCount, typeDefinitionCount, operationCount, 0);
        }

        private void CheckModelElementCount(IEdmModel model, int entityContainerCount, int schemaElementCount, int typeDefinitionCount, int operationCount, int actionCount)
        {
            Assert.AreEqual(entityContainerCount, 1, "Invalid entity container count.");

            var schemaElements = model.SchemaElements;
            Assert.AreEqual(schemaElementCount, schemaElements.Where(x => x.SchemaElementKind != EdmSchemaElementKind.EntityContainer).Count(), "Invalid schema element count.");
            Assert.AreEqual(typeDefinitionCount, schemaElements.Where(x => x.SchemaElementKind.Equals(EdmSchemaElementKind.TypeDefinition)).Count(), "Invalid type definition count.");
            Assert.AreEqual(operationCount, schemaElements.Where(x => x.SchemaElementKind.Equals(EdmSchemaElementKind.Function)).Count(), "Invalid function count.");
            Assert.AreEqual(actionCount, schemaElements.Where(x => x.SchemaElementKind.Equals(EdmSchemaElementKind.Action)).Count(), "Invalid action count.");
        }
    }
}

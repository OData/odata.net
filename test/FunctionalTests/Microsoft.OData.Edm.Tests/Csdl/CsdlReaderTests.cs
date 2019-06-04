﻿//---------------------------------------------------------------------
// <copyright file="CsdlReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using Xunit;
using ErrorStrings = Microsoft.OData.Edm.Strings;

namespace Microsoft.OData.Edm.Tests.Csdl
{
    public class CsdlReaderTests
    {
        private const string ValidEdmx = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""Container"" />
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
        private const string InvalidXml = "<fake/>";
        private const string ErrorMessage = "UnexpectedXmlElement : The element 'fake' was unexpected for the root element. The root element should be Edmx. : (0, 0)";

        private XmlReader validReader;
        private XmlReader invalidReader;

        public CsdlReaderTests()
        {
            this.validReader = XElement.Parse(ValidEdmx).CreateReader();
            this.invalidReader = XElement.Parse(InvalidXml).CreateReader();
        }

        [Fact]
        public void ReadNavigationPropertyPartnerTest()
        {
            var csdl =
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                    "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                        "<edmx:DataServices>" +
                            "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                                "<EntityType Name=\"EntityType1\">" +
                                    "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                    "<Property Name=\"ID\" Type=\"Edm.Int32\" />" +
                                    "<Property Name=\"ComplexProp\" Type=\"Collection(NS.ComplexType1)\" Nullable=\"false\" />" +
                                    "<NavigationProperty Name=\"OuterNavA\" Type=\"NS.EntityType2\" Nullable=\"false\" Partner=\"OuterNavA\" />" +
                                    "<NavigationProperty Name=\"OuterNavB\" Type=\"Collection(NS.EntityType2)\" Partner=\"NS.EntityType3/OuterNavC\" />" +
                                "</EntityType>" +
                                "<EntityType Name=\"EntityType2\">" +
                                    "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                    "<Property Name=\"ID\" Type=\"Edm.Int32\" />" +
                                    "<Property Name=\"ComplexProp\" Type=\"NS.ComplexType2\" Nullable=\"false\" />" +
                                    "<NavigationProperty Name=\"OuterNavA\" Type=\"NS.EntityType1\" Nullable=\"false\" Partner=\"OuterNavA\" />" +
                                    "<NavigationProperty Name=\"OuterNavB\" Type=\"NS.EntityType1\" Nullable=\"false\" Partner=\"ComplexProp/InnerNav\" />" +
                                "</EntityType>" +
                                "<EntityType Name=\"EntityType3\" BaseType=\"NS.EntityType2\">" +
                                    "<NavigationProperty Name=\"OuterNavC\" Type=\"Collection(NS.EntityType1)\" Partner=\"OuterNavB\" />" +
                                "</EntityType>" +
                                "<ComplexType Name=\"ComplexType1\">" +
                                    "<NavigationProperty Name=\"InnerNav\" Type=\"NS.EntityType2\" Nullable=\"false\" />" +
                                "</ComplexType>" +
                                "<ComplexType Name=\"ComplexType2\">" +
                                    "<NavigationProperty Name=\"InnerNav\" Type=\"NS.EntityType1\" Nullable=\"false\" />" +
                                "</ComplexType>" +
                            "</Schema>" +
                        "</edmx:DataServices>" +
                    "</edmx:Edmx>";
            var model = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
            var entityType1 = (IEdmEntityType)model.FindDeclaredType("NS.EntityType1");
            var entityType2 = (IEdmEntityType)model.FindDeclaredType("NS.EntityType2");
            var entityType3 = (IEdmEntityType)model.FindDeclaredType("NS.EntityType3");
            var complexType1 = (IEdmComplexType)model.FindDeclaredType("NS.ComplexType1");
            var complexType2 = (IEdmComplexType)model.FindDeclaredType("NS.ComplexType2");
            Assert.Equal("OuterNavA", ((IEdmNavigationProperty)entityType1.FindProperty("OuterNavA")).GetPartnerPath().Path);
            Assert.Equal(
                entityType2.FindProperty("OuterNavA"),
                ((IEdmNavigationProperty)entityType1.FindProperty("OuterNavA")).Partner);
            Assert.Equal("ComplexProp/InnerNav", ((IEdmNavigationProperty)entityType2.FindProperty("OuterNavB")).GetPartnerPath().Path);
            Assert.Equal(
                complexType1.FindProperty("InnerNav"),
                ((IEdmNavigationProperty)entityType2.FindProperty("OuterNavB")).Partner);
            Assert.Equal("NS.EntityType3/OuterNavC", ((IEdmNavigationProperty)entityType1.FindProperty("OuterNavB")).GetPartnerPath().Path);
            Assert.Equal(
                entityType3.FindProperty("OuterNavC"),
                ((IEdmNavigationProperty)entityType1.FindProperty("OuterNavB")).Partner);
        }

        [Fact]
        public void ValidateNavigationPropertyBindingPathTypeCast()
        {
            var csdl
                = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                    "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                      "<edmx:DataServices>" +
                        "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                          "<EntityType Name=\"EntityA\">" +
                            "<Key><PropertyRef Name=\"ID\" /></Key>" +
                              "<Property Name=\"ID\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                              "<Property Name=\"Complex\" Type=\"NS.ComplexA\" Nullable=\"false\" />" +
                            "</EntityType>" +
                          "<EntityType Name=\"EntityB\">" +
                            "<Key><PropertyRef Name=\"ID\" /></Key>" +
                            "<Property Name=\"ID\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                          "</EntityType>" +
                          "<ComplexType Name=\"ComplexA\" />" +
                          "<ComplexType Name=\"ComplexB\">" +
                            "<NavigationProperty Name=\"ComplexBNav\" Type=\"NS.EntityB\" Nullable=\"false\" />" +
                          "</ComplexType>" +
                          "<EntityContainer Name=\"Container\">" +
                            "<EntitySet Name=\"Set1\" EntityType=\"NS.EntityA\">" +
                              "<NavigationPropertyBinding Path=\"Complex/NS.ComplexB/ComplexBNav\" Target=\"Set2\" />" +
                            "</EntitySet>" +
                            "<EntitySet Name=\"Set2\" EntityType=\"NS.EntityB\" />" +
                          "</EntityContainer>" +
                        "</Schema>" +
                      "</edmx:DataServices>" +
                    "</edmx:Edmx>";
            var model = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
            var set1 = model.FindDeclaredNavigationSource("Set1");
            Assert.True(set1.NavigationPropertyBindings.First().NavigationProperty is UnresolvedNavigationPropertyPath);
        }

        [Fact]
        public void ValidateNavigationPropertyBindingPathTraversesNoNonContainmentNavigationProperties()
        {
            var csdl
                = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                    "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                      "<edmx:DataServices>" +
                        "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                          "<EntityType Name=\"EntityBase\">" +
                            "<Key><PropertyRef Name=\"ID\" /></Key>" +
                            "<Property Name=\"ID\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                          "</EntityType>" +
                          "<EntityType Name=\"EntityA\" BaseType=\"NS.EntityBase\">" +
                            "<NavigationProperty Name=\"EntityAToB\" Type=\"NS.EntityB\" Nullable=\"false\" />" +
                          "</EntityType>" +
                          "<EntityType Name=\"EntityB\" BaseType=\"NS.EntityBase\">" +
                            "<NavigationProperty Name=\"EntityBToC\" Type=\"NS.EntityC\" Nullable=\"false\" />" +
                          "</EntityType>" +
                          "<EntityType Name=\"EntityC\" BaseType=\"NS.EntityBase\" />" +
                          "<EntityContainer Name=\"Container\">" +
                            "<EntitySet Name=\"SetA\" EntityType=\"NS.EntityA\">" +
                              "<NavigationPropertyBinding Path=\"EntityAToB/EntityBToC\" Target=\"SetC\" />" +
                            "</EntitySet>" +
                            "<EntitySet Name=\"SetB\" EntityType=\"NS.EntityB\" />" +
                            "<EntitySet Name=\"SetC\" EntityType=\"NS.EntityC\" />" +
                          "</EntityContainer>" +
                        "</Schema>" +
                      "</edmx:DataServices>" +
                    "</edmx:Edmx>";
            var model = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
            var setA = model.FindDeclaredNavigationSource("SetA");
            Assert.True(setA.NavigationPropertyBindings.First().NavigationProperty is UnresolvedNavigationPropertyPath);
        }

        [Fact]
        public void ValidateNavigationPropertyBindingPathTraversesContainmentNavigationProperties()
        {
            var csdl
                = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                    "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                      "<edmx:DataServices>" +
                        "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                          "<EntityType Name=\"RootEntity\">" +
                            "<NavigationProperty Name=\"SetA\" Type=\"Collection(NS.EntityA)\" ContainsTarget=\"true\" />" +
                            "<NavigationProperty Name=\"SetB\" Type=\"Collection(NS.EntityB)\" ContainsTarget=\"true\" />" +
                          "</EntityType>" +
                          "<EntityType Name=\"EntityA\">" +
                            "<NavigationProperty Name=\"EntityAToB\" Type=\"Collection(NS.EntityB)\" />" +
                          "</EntityType>" +
                          "<EntityType Name=\"EntityB\"/>" +
                          "<EntityContainer Name=\"Container\">" +
                            "<Singleton Name=\"Root\" Type=\"NS.RootEntity\">" +
                              "<NavigationPropertyBinding Path=\"EntityA/EntityAToB\" Target=\"Root/SetB\" />" +
                            "</Singleton>" +
                          "</EntityContainer>" +
                        "</Schema>" +
                      "</edmx:DataServices>" +
                    "</edmx:Edmx>";
            var model = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
            var setA = model.FindDeclaredNavigationSource("Root");
            var target = setA.NavigationPropertyBindings.First().Target;
            Assert.True(target is IEdmContainedEntitySet);
            target.Name.Should().Be("SetB");
            var targetSegments = target.Path.PathSegments.ToList();
            targetSegments.Count().Should().Be(2);
            targetSegments[0].Should().Be("Root");
            targetSegments[1].Should().Be("SetB");
            var pathSegments = setA.NavigationPropertyBindings.First().Path.PathSegments.ToList();
            pathSegments.Count().Should().Be(2);
            pathSegments[0].Should().Be("EntityA");
            pathSegments[1].Should().Be("EntityAToB");
        }

        [Fact]
        public void ValidateEducationModel()
        {
            var csdl
                = "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                  "  <edmx:DataServices>" +
                  "    <Schema Namespace=\"Test.NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "      <EntityType Name=\"educationRoot\">" +
                  "        <NavigationProperty Name=\"classes\" Type=\"Collection(Test.NS.class)\" ContainsTarget=\"true\"/>" +
                  "        <NavigationProperty Name=\"users\" Type=\"Collection(Test.NS.user)\" ContainsTarget=\"true\"/>" +
                  "      </EntityType>" +
                  "      <EntityType Name=\"user\">" +
                  "        <Key><PropertyRef Name=\"name\"/></Key>" +
                  "        <Property Name =\"name\" Type=\"Edm.String\" Nullable=\"False\"/>" +
                  "        <NavigationProperty Name=\"classes\" Type=\"Collection(Test.NS.class)\"/>" +
                  "      </EntityType>" +
                  "      <EntityType Name=\"class\">" +
                  "        <Key><PropertyRef Name=\"classNumber\"/></Key>" +
                  "        <Property Name=\"classNumber\" Type=\"Edm.String\" Nullable=\"False\"/>" +
                  "        <Property Name=\"displayName\" Type=\"Edm.String\"/>" +
                  "        <Property Name=\"description\" Type=\"Edm.String\"/>" +
                  "        <Property Name=\"period\" Type=\"Edm.String\"/>" +
                  "        <NavigationProperty Name=\"members\" Type=\"Collection(Test.NS.user)\"/>" +
                  "      </EntityType>" +
                  "      <EntityContainer Name=\"EducationService\">" +
                  "        <Singleton Name=\"education\" Type=\"Test.NS.educationRoot\">" +
                  "          <NavigationPropertyBinding Path=\"classes/members\" Target=\"education/users\" />" +
                  "        </Singleton>" +
                  "      </EntityContainer>" +
                  "    </Schema>" +
                  "  </edmx:DataServices>" +
                  "</edmx:Edmx>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            CsdlReader.TryParse(XElement.Parse(csdl).CreateReader(), out model, out errors).Should().BeTrue();
            errors.Count().Should().Be(0);
            model.Validate(out errors);
            errors.Count().Should().Be(0);

            var educationSingleton = model.FindDeclaredNavigationSource("education");
            var navPropBinding = educationSingleton.NavigationPropertyBindings.First();
            var target = navPropBinding.Target;
            target.Should().NotBeNull();
            Assert.True(target is IEdmContainedEntitySet);
            target.Name.Should().Be("users");
            var targetSegments = target.Path.PathSegments.ToList();
            targetSegments.Count().Should().Be(2);
            targetSegments[0].Should().Be("education");
            targetSegments[1].Should().Be("users");
            var pathSegments = navPropBinding.Path.PathSegments.ToList();
            pathSegments.Count().Should().Be(2);
            pathSegments[0].Should().Be("classes");
            pathSegments[1].Should().Be("members");
        }

        [Fact]
        public void ReadNavigationPropertyPartnerTypeHierarchyTest()
        {
            var csdl =
                "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
                "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                    "<edmx:DataServices>" +
                        "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                            "<EntityType Name=\"EntityTypeA1\">" +
                                "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                "<Property Name=\"ID\" Type=\"Edm.Int32\" />" +
                                "<NavigationProperty Name=\"A1Nav\" Type=\"NS.EntityTypeB\" Nullable=\"false\" Partner=\"BNav1\" />" +
                            "</EntityType>" +
                            "<EntityType Name=\"EntityTypeA2\" BaseType=\"NS.EntityTypeA1\" />" +
                            "<EntityType Name=\"EntityTypeA3\" BaseType=\"NS.EntityTypeA2\">" +
                                "<NavigationProperty Name=\"A3Nav\" Type=\"NS.EntityTypeB\" Nullable=\"false\" Partner=\"BNav2\" />" +
                            "</EntityType>" +
                            "<EntityType Name=\"EntityTypeB\">" +
                                "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                "<Property Name=\"ID\" Type=\"Edm.Int32\" />" +
                                "<NavigationProperty Name=\"BNav1\" Type=\"NS.EntityTypeA2\" Nullable=\"false\" Partner=\"A1Nav\" />" +
                                "<NavigationProperty Name=\"BNav2\" Type=\"NS.EntityTypeA3\" Nullable=\"false\" Partner=\"NS.EntityTypeA3/A3Nav\" />" +
                            "</EntityType>" +
                        "</Schema>" +
                    "</edmx:DataServices>" +
                "</edmx:Edmx>";
            var model = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
            var entityTypeA1 = (IEdmEntityType)model.FindDeclaredType("NS.EntityTypeA1");
            var entityTypeA2 = (IEdmEntityType)model.FindDeclaredType("NS.EntityTypeA2");
            var entityTypeA3 = (IEdmEntityType)model.FindDeclaredType("NS.EntityTypeA3");
            var entityTypeB = (IEdmEntityType)model.FindDeclaredType("NS.EntityTypeB");
            Assert.Equal("BNav1", ((IEdmNavigationProperty)entityTypeA2.FindProperty("A1Nav")).GetPartnerPath().Path);
            Assert.Equal(entityTypeB.FindProperty("BNav1"), ((IEdmNavigationProperty)entityTypeA2.FindProperty("A1Nav")).Partner);
            Assert.Equal("BNav2", ((IEdmNavigationProperty)entityTypeA3.FindProperty("A3Nav")).GetPartnerPath().Path);
            Assert.Equal(entityTypeB.FindProperty("BNav2"), ((IEdmNavigationProperty)entityTypeA3.FindProperty("A3Nav")).Partner);
            Assert.Equal("A1Nav", ((IEdmNavigationProperty)entityTypeB.FindProperty("BNav1")).GetPartnerPath().Path);
            Assert.Equal(entityTypeA2.FindProperty("A1Nav"), ((IEdmNavigationProperty)entityTypeB.FindProperty("BNav1")).Partner);
            Assert.Equal("NS.EntityTypeA3/A3Nav", ((IEdmNavigationProperty)entityTypeB.FindProperty("BNav2")).GetPartnerPath().Path);
            Assert.Equal(entityTypeA3.FindProperty("A3Nav"), ((IEdmNavigationProperty)entityTypeB.FindProperty("BNav2")).Partner);
        }

        [Fact]
        public void ParsingValidXmlWithNoReferencesShouldSucceed()
        {
            this.RunValidTest(CsdlReader.Parse);
        }

        [Fact]
        public void ParsingInvalidXmlWithNoReferencesShouldThrow()
        {
            this.RunInvalidTest(CsdlReader.Parse);
        }

        [Fact]
        public void ParsingvalidXmlWithNavigationPropertyInComplex()
        {
            string complexWithNav =
"<?xml version=\"1.0\" encoding=\"utf-16\"?><edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                "<edmx:DataServices>" +
                "<Schema Namespace=\"DefaultNs\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                "<EntityType Name=\"Person\">" +
                    "<Key><PropertyRef Name=\"UserName\" /></Key>" +
                    "<Property Name=\"UserName\" Type=\"Edm.String\" Nullable=\"false\" />" +
                    "<Property Name=\"HomeAddress\" Type=\"DefaultNs.Address\" Nullable=\"false\" />" +
                    "<Property Name=\"WorkAddress\" Type=\"DefaultNs.Address\" Nullable=\"false\" />" +
                    "<Property Name=\"Addresses\" Type=\"Collection(DefaultNs.Address)\" Nullable=\"false\" />" +
                "</EntityType>" +
                "<EntityType Name=\"City\">" +
                    "<Key><PropertyRef Name=\"Name\" /></Key>" +
                    "<Property Name=\"Name\" Type=\"Edm.String\" Nullable=\"false\" />" +
                "</EntityType>" +
                "<EntityType Name=\"CountryOrRegion\">" +
                    "<Key><PropertyRef Name=\"Name\" /></Key>" +
                    "<Property Name=\"Name\" Type=\"Edm.String\" Nullable=\"false\" />" +
                "</EntityType>" +
                "<ComplexType Name=\"Address\">" +
                    "<Property Name=\"Id\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                    "<NavigationProperty Name=\"City\" Type=\"DefaultNs.City\" Nullable=\"false\" />" +
                "</ComplexType>" +
                "<ComplexType Name=\"WorkAddress\" BaseType=\"DefaultNs.Address\">" +
                    "<NavigationProperty Name=\"CountryOrRegion\" Type=\"DefaultNs.CountryOrRegion\" Nullable=\"false\" /><" +
                "/ComplexType>" +
                "<EntityContainer Name=\"Container\">" +
                "<EntitySet Name=\"People\" EntityType=\"DefaultNs.Person\">" +
                    "<NavigationPropertyBinding Path=\"HomeAddress/City\" Target=\"Cities\" />" +
                    "<NavigationPropertyBinding Path=\"Addresses/City\" Target=\"Cities\" />" +
                    "<NavigationPropertyBinding Path=\"WorkAddress/DefaultNs.WorkAddress/CountryOrRegion\" Target=\"CountriesOrRegions\" />" +
                "</EntitySet>" +
                "<EntitySet Name=\"Cities\" EntityType=\"DefaultNs.City\" />" +
                "<EntitySet Name=\"CountriesOrRegions\" EntityType=\"DefaultNs.CountryOrRegion\" />" +
                "</EntityContainer></Schema>" +
                "</edmx:DataServices>" +
                "</edmx:Edmx>";

            var model = CsdlReader.Parse(XElement.Parse(complexWithNav).CreateReader());
            var people = model.EntityContainer.FindEntitySet("People");
            var address = model.FindType("DefaultNs.Address") as IEdmStructuredType;
            var workAddress = model.FindType("DefaultNs.WorkAddress") as IEdmStructuredType;
            var city = address.FindProperty("City") as IEdmNavigationProperty;
            var countryOrRegion = workAddress.FindProperty("CountryOrRegion") as IEdmNavigationProperty;
            var cities = model.EntityContainer.FindEntitySet("Cities");
            var countriesOrRegions = model.EntityContainer.FindEntitySet("CountriesOrRegions");
            var navigationTarget = people.FindNavigationTarget(city, new EdmPathExpression("HomeAddress/City"));
            Assert.Equal(navigationTarget, cities);
            navigationTarget = people.FindNavigationTarget(city, new EdmPathExpression("Addresses/City"));
            Assert.Equal(navigationTarget, cities);
            navigationTarget = people.FindNavigationTarget(countryOrRegion, new EdmPathExpression("WorkAddress/DefaultNs.WorkAddress/CountryOrRegion"));
            Assert.Equal(navigationTarget, countriesOrRegions);
        }

        [Fact]
        public void ParsingXmlWithCollectionOfNavOnComplex()
        {
            string complexWithNav = "<?xml version=\"1.0\" encoding=\"utf-16\"?><edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                              "<edmx:DataServices><Schema Namespace=\"DefaultNs\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                              "<EntityType Name=\"EntityType\">" +
                                  "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                  "<Property Name=\"ID\" Type=\"Edm.String\" Nullable=\"false\" />" +
                                  "<Property Name=\"Complex\" Type=\"DefaultNs.ComplexType\" Nullable=\"false\" />" +
                              "</EntityType>" +
                              "<EntityType Name=\"NavEntityType\">" +
                                  "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                  "<Property Name=\"ID\" Type=\"Edm.String\" Nullable=\"false\" />" +
                              "</EntityType>" +
                              "<ComplexType Name=\"ComplexType\">" +
                                  "<Property Name=\"Prop1\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                                  "<NavigationProperty Name=\"CollectionOfNav\" Type=\"Collection(DefaultNs.NavEntityType)\" />" +
                              "</ComplexType>" +
                              "<EntityContainer Name=\"Container\">" +
                              "<EntitySet Name=\"Entities\" EntityType=\"DefaultNs.EntityType\">" +
                                "<NavigationPropertyBinding Path=\"Complex/CollectionOfNav\" Target=\"NavEntities\" />" +
                              "</EntitySet>" +
                              "<EntitySet Name=\"NavEntities\" EntityType=\"DefaultNs.NavEntityType\" />" +
                              "</EntityContainer>" +
                              "</Schema>" +
                              "</edmx:DataServices>" +
                              "</edmx:Edmx>";

            var model = CsdlReader.Parse(XElement.Parse(complexWithNav).CreateReader());
            var people = model.EntityContainer.FindEntitySet("Entities");
            var address = model.FindType("DefaultNs.ComplexType") as IEdmStructuredType;
            var city = address.FindProperty("CollectionOfNav") as IEdmNavigationProperty;
            var cities = model.EntityContainer.FindEntitySet("NavEntities");
            var navigationTarget = people.FindNavigationTarget(city, new EdmPathExpression("Complex/CollectionOfNav"));
            Assert.Equal(navigationTarget, cities);
        }

        [Fact]
        public void ParsingOptionalParametersShouldSucceed()
        {
            string csdl =
        "<edmx:Edmx xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\" Version=\"4.0\">" +
          "<edmx:DataServices>" +
            "<Schema xmlns=\"http://docs.oasis-open.org/odata/ns/edm\" Namespace=\"test\">" +
              "<Function Name=\"TestFunction\">" +
                "<Parameter Name = \"requiredParam\" Type=\"Edm.String\"/>" +
                "<Parameter Name = \"optionalParam\" Type=\"Edm.String\">" +
                    "<Annotation Term=\"Org.OData.Core.V1.OptionalParameter\"/>" +
                "</Parameter>" +
                "<Parameter Name = \"optionalParamWithDefault\" Type=\"Edm.String\">" +
                    "<Annotation Term=\"Org.OData.Core.V1.OptionalParameter\">" +
                      "<Record>" +
                        "<PropertyValue Property=\"DefaultValue\" String=\"Smith\"/>" +
                      "</Record>" +
                    "</Annotation>" +
                "</Parameter>" +
                "<ReturnType Type=\"Edm.Boolean\"/>" +
              "</Function>" +
              "<EntityContainer Name=\"Default\">" +
                "<FunctionImport Name=\"TestFunction\" Function=\"test.TestFunction\"/>" +
              "</EntityContainer>" +
            "</Schema>" +
          "</edmx:DataServices>" +
        "</edmx:Edmx>";

            var model = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
            var function = model.FindDeclaredOperations("test.TestFunction").FirstOrDefault();
            Assert.NotNull(function);
            var requiredParam = function.Parameters.Where(p => p.Name == "requiredParam").FirstOrDefault();
            Assert.NotNull(requiredParam);
            Assert.Null(requiredParam as IEdmOptionalParameter);
            IEdmOptionalParameter optionalParam = function.Parameters.Where(p => p.Name == "optionalParam").FirstOrDefault() as IEdmOptionalParameter;
            Assert.NotNull(optionalParam);
            Assert.True(String.IsNullOrEmpty(optionalParam.DefaultValueString));
            IEdmOptionalParameter optionalParamWithDefault = function.Parameters.Where(p => p.Name == "optionalParamWithDefault").FirstOrDefault() as IEdmOptionalParameter;
            Assert.NotNull(optionalParamWithDefault);
            Assert.Equal(optionalParamWithDefault.DefaultValueString, "Smith");
        }

        [Theory]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.Inline)]
        [InlineData(EdmVocabularyAnnotationSerializationLocation.OutOfLine)]
        public void ParsingReturnTypeAnnotationShouldSucceed(EdmVocabularyAnnotationSerializationLocation location)
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <Function Name=""TestFunction"">
        <ReturnType Type=""Edm.PrimitiveType"" Nullable=""false"" >
          {0}
        </ReturnType>
      </Function>
      <Annotations Target=""NS.TestFunction()/$ReturnType"">
         {1}
      </Annotations>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            string annotationString =
                "<Annotation Term =\"Org.OData.Validation.V1.DerivedTypeConstraint\">" +
                  "<Collection>" +
                     "<String>Edm.Int32</String>" +
                     "<String>Edm.Boolean</String>" +
                   "</Collection>" +
                 "</Annotation>";
            string inline = location == EdmVocabularyAnnotationSerializationLocation.Inline ? annotationString : "";
            string outLine = location == EdmVocabularyAnnotationSerializationLocation.Inline ? "" : annotationString;
            string csdl = String.Format(template, inline, outLine);

            var model = CsdlReader.Parse(XElement.Parse(csdl).CreateReader());
            var function = model.FindDeclaredOperations("NS.TestFunction").FirstOrDefault();
            Assert.NotNull(function);
            Assert.NotNull(function.ReturnType);
            IEdmOperationReturn returnType = function.GetReturn();
            Assert.NotNull(returnType);
            Assert.Same(returnType.DeclaringOperation, function);
            Assert.Equal("Edm.PrimitiveType", returnType.Type.FullName());

            var termType = model.FindTerm("Org.OData.Validation.V1.DerivedTypeConstraint");
            Assert.NotNull(termType);

            IEdmVocabularyAnnotation annotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(returnType, termType).FirstOrDefault();
            Assert.NotNull(annotation);

            Assert.True(annotation.GetSerializationLocation(model) == location);
            IEdmCollectionExpression collectConstant = annotation.Value as IEdmCollectionExpression;
            Assert.NotNull(collectConstant);

            Assert.Equal(new[] { "Edm.Int32", "Edm.Boolean" }, collectConstant.Elements.Select(e => ((IEdmStringConstantExpression)e).Value));
        }

        [Fact]
        public void ParsingValidXmlWithOneReferencesShouldSucceed()
        {
            this.RunValidTest(r => CsdlReader.Parse(r, EdmCoreModel.Instance));
        }

        [Fact]
        public void ParsingInvalidXmlWithOneReferencesShouldThrow()
        {
            this.RunInvalidTest(r => CsdlReader.Parse(r, EdmCoreModel.Instance));
        }

        [Fact]
        public void ParsingValidXmlWithManyReferencesShouldSucceed()
        {
            this.RunValidTest(r => CsdlReader.Parse(r, new IEdmModel[] { EdmCoreModel.Instance }));
        }

        [Fact]
        public void ParsingInvalidXmlWithManyReferencesShouldThrow()
        {
            this.RunInvalidTest(r => CsdlReader.Parse(r, new IEdmModel[] { EdmCoreModel.Instance }));
        }

        [Fact]
        public void ParsingInvalidXmlWithMultipleEntityContainersShouldThrow()
        {
            string EdmxwithMultipleEntityContainers = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""Container1"" />
      <EntityContainer Name=""Container2"" />
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            Action parseAction = () => CsdlReader.Parse(XElement.Parse(EdmxwithMultipleEntityContainers).CreateReader());
            parseAction.ShouldThrow<EdmParseException>().Where(e => e.Message.Contains(
                Strings.CsdlParser_MetadataDocumentCannotHaveMoreThanOneEntityContainer)).And.Errors.Should().HaveCount(1);
        }

        [Fact]
        public void ParsingNavigationPropertyWithEdmEntityTypeWorks()
        {
            string navigationProperty =
                @"<NavigationProperty Name=""EntityNavigationProperty"" Type=""Edm.EntityType"" />";

            IEdmModel model = GetEdmModel(properties: navigationProperty);
            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors));

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);
            var navProperty = customer.DeclaredNavigationProperties().FirstOrDefault(c => c.Name == "EntityNavigationProperty");
            Assert.NotNull(navProperty);
            Assert.True(navProperty.Type.IsNullable);
            Assert.Same(EdmCoreModel.Instance.GetEntityType(), navProperty.Type.Definition);

            var address = model.SchemaElements.OfType<IEdmComplexType>().FirstOrDefault(c => c.Name == "Address");
            Assert.NotNull(address);
            navProperty = address.DeclaredNavigationProperties().FirstOrDefault(c => c.Name == "EntityNavigationProperty");
            Assert.NotNull(navProperty);
            Assert.True(navProperty.Type.IsNullable);
            Assert.Same(EdmCoreModel.Instance.GetEntityType(), navProperty.Type.Definition);
        }

        [Fact]
        public void ParsingPropertyWithEdmComplexTypeWorks()
        {
            string complexProperty =
                @"<Property Name=""ComplexProperty"" Type=""Edm.ComplexType"" />";

            IEdmModel model = GetEdmModel(properties: complexProperty);
            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors));

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);
            var property = customer.DeclaredProperties.FirstOrDefault(c => c.Name == "ComplexProperty");
            Assert.NotNull(property);
            Assert.True(property.Type.IsNullable);
            Assert.Same(EdmCoreModel.Instance.GetComplexType(), property.Type.Definition);

            var address = model.SchemaElements.OfType<IEdmComplexType>().FirstOrDefault(c => c.Name == "Address");
            Assert.NotNull(address);
            property = address.DeclaredProperties.FirstOrDefault(c => c.Name == "ComplexProperty");
            Assert.NotNull(property);
            Assert.True(property.Type.IsNullable);
            Assert.Same(EdmCoreModel.Instance.GetComplexType(), property.Type.Definition);
        }

        [Fact]
        public void ParsingPropertyWithEdmPrimitiveTypeWorks()
        {
            string complexProperty =
                @"<Property Name=""PrimitiveProperty"" Type=""Edm.PrimitiveType"" Nullable=""false"" />";

            IEdmModel model = GetEdmModel(properties: complexProperty);
            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors));

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);
            var property = customer.DeclaredProperties.FirstOrDefault(c => c.Name == "PrimitiveProperty");
            Assert.NotNull(property);
            Assert.False(property.Type.IsNullable);
            Assert.Same(EdmCoreModel.Instance.GetPrimitiveType(), property.Type.Definition);

            var address = model.SchemaElements.OfType<IEdmComplexType>().FirstOrDefault(c => c.Name == "Address");
            Assert.NotNull(address);
            property = address.DeclaredProperties.FirstOrDefault(c => c.Name == "PrimitiveProperty");
            Assert.NotNull(property);
            Assert.False(property.Type.IsNullable);
            Assert.Same(EdmCoreModel.Instance.GetPrimitiveType(), property.Type.Definition);
        }

        [Fact]
        public void ParsingTermPropertyWithEdmPathTypeWorks()
        {
            string termTypes =
                @"<ComplexType Name=""SelectType"" >
                    <Property Name=""DefaultSelect"" Type=""Collection(Edm.PropertyPath)"" />
                    <Property Name=""DefaultHidden"" Type=""Collection(Edm.NavigationPropertyPath)"" Nullable=""false"" />
                  </ComplexType>
                  <Term Name=""MyTerm"" Type=""NS.SelectType"" />";

            IEdmModel model = GetEdmModel(types: termTypes);
            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors));

            var complex = model.SchemaElements.OfType<IEdmComplexType>().FirstOrDefault(c => c.Name == "SelectType");
            Assert.NotNull(complex);
            var property = complex.DeclaredProperties.FirstOrDefault(c => c.Name == "DefaultSelect");
            Assert.NotNull(property);
            Assert.True(property.Type.IsNullable);
            Assert.True(property.Type.IsCollection());
            Assert.Equal(EdmTypeKind.Path, property.Type.AsCollection().ElementType().TypeKind());
            Assert.Same(EdmCoreModel.Instance.GetPathType(EdmPathTypeKind.PropertyPath), property.Type.AsCollection().ElementType().Definition);

            property = complex.DeclaredProperties.FirstOrDefault(c => c.Name == "DefaultHidden");
            Assert.NotNull(property);
            Assert.False(property.Type.IsNullable);
            Assert.True(property.Type.IsCollection());
            Assert.Equal(EdmTypeKind.Path, property.Type.AsCollection().ElementType().TypeKind());
            Assert.Same(EdmCoreModel.Instance.GetPathType(EdmPathTypeKind.NavigationPropertyPath), property.Type.AsCollection().ElementType().Definition);
        }

        [Fact]
        public void ParsingPropertyWithEdmPathTypeWorksButValidationFailed()
        {
            string properties =
                @"<Property Name=""PathProperty"" Type=""Collection(Edm.PropertyPath)"" />";

            IEdmModel model = GetEdmModel(properties: properties);

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);
            var property = customer.DeclaredProperties.FirstOrDefault(c => c.Name == "PathProperty");
            Assert.NotNull(property);
            Assert.True(property.Type.IsNullable);
            Assert.True(property.Type.IsCollection());
            Assert.Equal(EdmTypeKind.Path, property.Type.AsCollection().ElementType().TypeKind());
            Assert.Same(EdmCoreModel.Instance.GetPathType(EdmPathTypeKind.PropertyPath), property.Type.AsCollection().ElementType().Definition);

            IEnumerable<EdmError> errors;
            Assert.False(model.Validate(out errors));
            var error = Assert.Single(errors);
            Assert.NotNull(error);
            Assert.Equal(EdmErrorCode.DeclaringTypeOfNavigationSourceCannotHavePathProperty, error.ErrorCode);
        }

        [Fact]
        public void ParsingRecursivePropertyWithEdmPathTypeWorksButValidationFailed()
        {
            string properties =
                @"<Property Name=""PathProperty"" Type=""Collection(Edm.PropertyPath)"" />
                  <Property Name=""ComplexProperty"" Type=""NS.Address"" />";

            IEdmModel model = GetEdmModel(properties: properties);

            var address = model.SchemaElements.OfType<IEdmComplexType>().FirstOrDefault(c => c.Name == "Address");
            Assert.NotNull(address);
            var property = address.DeclaredProperties.FirstOrDefault(c => c.Name == "ComplexProperty");
            Assert.NotNull(property);
            Assert.True(property.Type.IsNullable);
            Assert.Equal(EdmTypeKind.Complex, property.Type.TypeKind());

            IEnumerable<EdmError> errors;
            Assert.False(model.Validate(out errors));
            var error = Assert.Single(errors);
            Assert.NotNull(error);
            Assert.Equal(EdmErrorCode.DeclaringTypeOfNavigationSourceCannotHavePathProperty, error.ErrorCode);
        }

        [Fact]
        public void ParsingNavigationPropertyWithEdmPathTypeWorksButValidationFailed()
        {
            string properties =
                @"<NavigationProperty Name=""NavigationProperty"" Type=""Collection(NS.Customer)"" />
                  <Property Name=""PathProperty"" Type=""Collection(Edm.AnnotationPath)"" />";

            IEdmModel model = GetEdmModel(properties: properties);

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);
            var navProperty = customer.DeclaredNavigationProperties().FirstOrDefault(c => c.Name == "NavigationProperty");
            Assert.NotNull(navProperty);

            IEnumerable<EdmError> errors;
            Assert.False(model.Validate(out errors));
            Assert.NotNull(errors);
            Assert.Equal(3, errors.Count());

            Assert.Equal(new[] {EdmErrorCode.TypeOfNavigationPropertyCannotHavePathProperty,
                EdmErrorCode.TypeOfNavigationPropertyCannotHavePathProperty,
                EdmErrorCode.DeclaringTypeOfNavigationSourceCannotHavePathProperty}, errors.Select(e => e.ErrorCode));
        }

        [Fact]
        public void ParsingEnumMemberWithAnnotationsWorks()
        {
            string types =
@"<EnumType Name=""Color"" >
  <Member Name=""Red"" Value=""1"" >
    <Annotation String=""Inline Description"" Term=""Org.OData.Core.V1.LongDescription""/>
    <Annotation String=""Inline MyTerm Value"" Term=""NS.MyTerm""/>
  </Member>
  <Member Name=""Blue"" Value=""2"" />
</EnumType>
<Term Name=""MyTerm"" Type=""Edm.String""/>
<Annotations Target=""NS.Color/Blue"" >
  <Annotation String=""OutOfLine Description"" Term=""Org.OData.Core.V1.LongDescription""/>
  <Annotation String=""OutOfLine MyTerm Value"" Term=""NS.MyTerm""/>
</Annotations>";

            IEdmModel model = GetEdmModel(types: types);
            Assert.NotNull(model);

            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors), String.Format("Errors in validating model. {0}", String.Concat(errors.Select(e => e.ErrorMessage))));

            var color = model.SchemaElements.OfType<IEdmEnumType>().FirstOrDefault(c => c.Name == "Color");
            Assert.NotNull(color);

            IEdmTerm fooBarTerm = model.FindDeclaredTerm("NS.MyTerm");
            Assert.NotNull(fooBarTerm);

            // Red
            var red = color.Members.FirstOrDefault(c => c.Name == "Red");
            Assert.NotNull(red);
            string redAnnotation = GetStringAnnotation(model, red, CoreVocabularyModel.LongDescriptionTerm, EdmVocabularyAnnotationSerializationLocation.Inline);
            Assert.Equal("Inline Description", redAnnotation);

            redAnnotation = GetStringAnnotation(model, red, fooBarTerm, EdmVocabularyAnnotationSerializationLocation.Inline);
            Assert.Equal("Inline MyTerm Value", redAnnotation);

            // Blue
            var blue = color.Members.FirstOrDefault(c => c.Name == "Blue");
            Assert.NotNull(blue);
            string blueAnnotation = GetStringAnnotation(model, blue, CoreVocabularyModel.LongDescriptionTerm, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            Assert.Equal("OutOfLine Description", blueAnnotation);

            blueAnnotation = GetStringAnnotation(model, blue, fooBarTerm, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            Assert.Equal("OutOfLine MyTerm Value", blueAnnotation);
        }

        private string GetStringAnnotation(IEdmModel model, IEdmVocabularyAnnotatable target, IEdmTerm term, EdmVocabularyAnnotationSerializationLocation location)
        {
            IEdmVocabularyAnnotation annotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(target, term).FirstOrDefault();
            if (annotation != null)
            {
                Assert.True(annotation.GetSerializationLocation(model) == location);

                IEdmStringConstantExpression stringConstant = annotation.Value as IEdmStringConstantExpression;
                if (stringConstant != null)
                {
                    return stringConstant.Value;
                }
            }

            return null;
        }

        private void RunValidTest(Func<XmlReader, IEdmModel> parse)
        {
            var result = parse(this.validReader);
            result.Should().NotBeNull();
            result.EntityContainer.FullName().Should().Be("Test.Container");
        }

        private void RunInvalidTest(Func<XmlReader, IEdmModel> parse)
        {
            Action parseAction = () => parse(this.invalidReader);
            parseAction.ShouldThrow<EdmParseException>().WithMessage(ErrorStrings.EdmParseException_ErrorsEncounteredInEdmx(ErrorMessage)).And.Errors.Should().OnlyContain(e => e.ToString() == ErrorMessage);
        }

        private static IEdmModel GetEdmModel(string types = "", string properties = "")
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Customer"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
        {1}
      </EntityType>
      <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" />
        {1}
      </ComplexType>
      {0}
      <EntityContainer Name =""Default"">
         <EntitySet Name=""Customers"" EntityType=""NS.Customer"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string modelText = string.Format(template, types, properties);

            IEdmModel model;
            IEnumerable<EdmError> errors;

            bool result = CsdlReader.TryParse(XElement.Parse(modelText).CreateReader(), out model, out errors);
            Assert.True(result);
            return model;
        }

        [Fact]
        public void ParsingInLineOptionalParameterWorks()
        {
            string expected =
           "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
           "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
             "<edmx:DataServices>" +
               "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                 "<Function Name=\"TestFunction\">" +
                   "<Parameter Name=\"requiredParam\" Type=\"Edm.String\" Nullable=\"false\" />" +
                   "<Parameter Name=\"optionalParam\" Type=\"Edm.String\" Nullable=\"false\">" +
                       "<Annotation Term=\"Org.OData.Core.V1.OptionalParameter\" />" +
                   "</Parameter>" +
                   "<Parameter Name=\"optionalParamWithDefault\" Type=\"Edm.String\" Nullable=\"false\">" +
                       "<Annotation Term=\"Org.OData.Core.V1.OptionalParameter\">" +
                         "<Record>" +
                           "<PropertyValue Property=\"DefaultValue\" String=\"Smith\" />" +
                         "</Record>" +
                       "</Annotation>" +
                   "</Parameter>" +
                   "<ReturnType Type=\"Edm.String\" Nullable=\"false\" />" +
                 "</Function>" +
               "</Schema>" +
             "</edmx:DataServices>" +
           "</edmx:Edmx>";

            IEdmModel model;
            IEnumerable<EdmError> errors;

            bool result = CsdlReader.TryParse(XElement.Parse(expected).CreateReader(), out model, out errors);
            Assert.True(result);
            Assert.NotNull(model);

            var function = model.SchemaElements.OfType<IEdmFunction>().First();
            VerifyOptionalParameter(function);
        }

        [Theory]
        [InlineData("NS.TestFunction")] // non-overloads
        [InlineData("NS.TestFunction(Edm.String, Edm.String, Edm.String)")] // overloads
        public void ParsingOutOfLineOptionalParameterWorks(string fullName)
        {
            string expected = String.Format(
           "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
           "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
             "<edmx:DataServices>" +
               "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                 "<Function Name=\"TestFunction\">" +
                   "<Parameter Name=\"requiredParam\" Type=\"Edm.String\" Nullable=\"false\" />" +
                   "<Parameter Name=\"optionalParam\" Type=\"Edm.String\" Nullable=\"false\" />" +
                   "<Parameter Name=\"optionalParamWithDefault\" Type=\"Edm.String\" Nullable=\"false\" />" +
                   "<ReturnType Type=\"Edm.String\" Nullable=\"false\" />" +
                 "</Function>" +
                 "<EntityContainer Name=\"Default\">" +
                   "<FunctionImport Name=\"TestFunction\" Function=\"test.TestFunction\" />" +
                 "</EntityContainer>" +
                 "<Annotations Target=\"{0}/optionalParam\" >" +
                   "<Annotation Term=\"Org.OData.Core.V1.OptionalParameter\" >" +
                     "<Record />" +
                  "</Annotation> " +
                 "</Annotations>" +
                 "<Annotations Target=\"{0}/optionalParamWithDefault\" >" +
                   "<Annotation Term=\"Org.OData.Core.V1.OptionalParameter\" >" +
                     "<Record Type=\"Org.OData.Core.V1.OptionalParameterType\">" +
                       "<PropertyValue Property=\"DefaultValue\" String=\"Smith\" />" +
                     "</Record>" +
                  "</Annotation> " +
                 "</Annotations>" +
               "</Schema>" +
             "</edmx:DataServices>" +
           "</edmx:Edmx>", fullName);

            IEdmModel model;
            IEnumerable<EdmError> errors;

            bool result = CsdlReader.TryParse(XElement.Parse(expected).CreateReader(), out model, out errors);
            Assert.True(result);
            Assert.NotNull(model);

            var function = model.SchemaElements.OfType<IEdmFunction>().First();
            VerifyOptionalParameter(function);
        }

        private static void VerifyOptionalParameter(IEdmFunction function)
        {
            Assert.NotNull(function);
            Assert.Equal("TestFunction", function.Name);

            var requiredParam = function.Parameters.First(c => c.Name == "requiredParam");
            Assert.True(requiredParam is IEdmOperationParameter);
            Assert.False(requiredParam is IEdmOptionalParameter);

            var optionalParam = function.Parameters.First(c => c.Name == "optionalParam");
            Assert.True(optionalParam is IEdmOptionalParameter);

            var optionalParamWithDefault = function.Parameters.First(c => c.Name == "optionalParamWithDefault");
            Assert.True(optionalParamWithDefault is IEdmOptionalParameter);
            var parameter = optionalParamWithDefault as IEdmOptionalParameter;
            Assert.Equal("Smith", parameter.DefaultValueString);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ParsingUrlEscapeFunctionWorks(bool escaped)
        {
            string format = @"<?xml version=""1.0"" encoding=""utf-16""?>
            <edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" >
               <edmx:DataServices>
                <Schema Namespace=""NS"" xmlns =""http://docs.oasis-open.org/odata/ns/edm"" >
                   <EntityType Name=""Entity"" >
                    <Key>
                      <PropertyRef Name=""Id"" />
                    </Key>
                    <Property Name=""Id"" Type =""Edm.Int32"" Nullable =""false"" />
                  </EntityType>
                  <Function Name=""Function"" IsBound =""true"" >
                    <Parameter Name=""entity"" Type =""NS.Entity"" />
                    <Parameter Name=""path"" Type =""Edm.String"" />
                    <ReturnType Type=""Edm.Int32"" />
                    {0}
                  </Function>
                </Schema>
              </edmx:DataServices>
            </edmx:Edmx>";

            string annotation = escaped ? "<Annotation Term=\"Org.OData.Community.V1.UrlEscapeFunction\" Bool=\"true\" />" : "";
            string csdl = String.Format(format, annotation);

            IEdmModel model;
            IEnumerable<EdmError> errors;

            bool result = CsdlReader.TryParse(XElement.Parse(csdl).CreateReader(), out model, out errors);
            Assert.True(result);
            Assert.NotNull(model);

            var function = model.SchemaElements.OfType<IEdmFunction>().First();
            Assert.Equal(escaped, model.IsUrlEscapeFunction(function));
        }

        [Fact]
        public void ParsingBaseAndDerivedTypeWithSameAnnotationWorksButValidationSuccessful()
        {
            string annotations = @"
            <Annotations Target=""NS.Base"">
                <Annotation Term=""Org.OData.Core.V1.Description"" String=""Base description"" />
            </Annotations>
            <Annotations Target=""NS.Derived"">
                <Annotation Term=""Org.OData.Core.V1.Description"" String=""Derived description"" />
            </Annotations>";

            IEdmModel model = GetInheritanceEdmModel(annotations);

            var edmType = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Base");
            Assert.NotNull(edmType);
            Assert.Equal("Base description", model.GetDescriptionAnnotation(edmType));

            edmType = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Derived");
            Assert.NotNull(edmType);
            Assert.Equal("Derived description", model.GetDescriptionAnnotation(edmType));

            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors));
        }

        [Fact]
        public void ParsingDerivedTypeWithDuplicatedAnnotationsWorksButValidationFailed()
        {
            string annotations = @"
            <Annotations Target=""NS.Derived"">
                <Annotation Term=""Org.OData.Core.V1.Description"" String=""Derived description 1"" />
            </Annotations>
            <Annotations Target=""NS.Derived"">
                <Annotation Term=""Org.OData.Core.V1.Description"" String=""Derived description 2"" />
            </Annotations>";

            IEdmModel model = GetInheritanceEdmModel(annotations);

            var edmType = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Derived");
            Assert.NotNull(edmType);
            var descriptions = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(edmType, CoreVocabularyModel.DescriptionTerm);
            Assert.Equal(new[] { "Derived description 1", "Derived description 2" },
                descriptions.Select(d => d.Value as IEdmStringConstantExpression).Select(e => e.Value));

            IEnumerable<EdmError> errors;
            Assert.False(model.Validate(out errors));
            EdmError error = Assert.Single(errors);
            Assert.NotNull(error);
            Assert.Equal(EdmErrorCode.DuplicateAnnotation, error.ErrorCode);
            Assert.Equal("The annotated element 'NS.Derived' has multiple annotations with the term 'Org.OData.Core.V1.Description' and the qualifier ''.", error.ErrorMessage);
        }

        private static IEdmModel GetInheritanceEdmModel(string annotation)
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Base"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
      </EntityType>
      <EntityType Name=""Derived"" BaseType=""NS.Base"" />
      {0}
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string modelText = string.Format(template, annotation);

            IEdmModel model;
            IEnumerable<EdmError> errors;

            bool result = CsdlReader.TryParse(XElement.Parse(modelText).CreateReader(), out model, out errors);
            Assert.True(result);
            return model;
        }

        [Fact]
        public void ParsingPropertyWithCoreTypeDefinitionAsPropertyTypeWorks()
        {
            string csdl =
            "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
            "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
              "<edmx:DataServices>" +
                "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                  "<ComplexType Name=\"Complex\">" +
                    "<Property Name=\"ModifiedDate\" Type=\"Org.OData.Core.V1.LocalDateTime\" />" +
                  "</ComplexType>" +
                "</Schema>" +
              "</edmx:DataServices>" +
            "</edmx:Edmx>";

            IEdmModel model;
            IEnumerable<EdmError> errors;

            bool result = CsdlReader.TryParse(XElement.Parse(csdl).CreateReader(), out model, out errors);
            Assert.True(result);
            Assert.NotNull(model);

            var edmType = model.SchemaElements.OfType<IEdmComplexType>().First();
            Assert.NotNull(edmType);
            var property = edmType.Properties().FirstOrDefault(c => c.Name == "ModifiedDate");
            Assert.NotNull(property);

            Assert.Equal(EdmTypeKind.TypeDefinition, property.Type.TypeKind());
            Assert.Equal("Org.OData.Core.V1.LocalDateTime", property.Type.FullName());
        }

        [Theory]
        [InlineData("4.0")]
        [InlineData("4.01")]
        public void ValidateEdmxVersions(string odataVersion)
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-16\"?><edmx:Edmx Version=\"" + odataVersion + "\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\"><edmx:DataServices /></edmx:Edmx>";

            var stringReader = new System.IO.StringReader(xml);
            var xmlReader = System.Xml.XmlReader.Create(stringReader);

            IEdmModel edmModel = null;
            IEnumerable<EdmError> edmErrors;

            // Read in the CSDL and verify the version
            CsdlReader.TryParse(xmlReader, out edmModel, out edmErrors);
            Assert.Equal(edmErrors.Count(), 0);
            Assert.Equal(edmModel.GetEdmVersion(), odataVersion == "4.0" ? EdmConstants.EdmVersion4 : EdmConstants.EdmVersion401);
        }
    }
}
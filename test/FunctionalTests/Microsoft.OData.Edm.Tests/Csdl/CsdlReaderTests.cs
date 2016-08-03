//---------------------------------------------------------------------
// <copyright file="CsdlReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Xml;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OData.Edm.Csdl;
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
        public void ReadNavigationPropertyPartner()
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
            parseAction.ShouldThrow<EdmParseException>().WithMessage(
                Strings.CsdlParser_MetadataDocumentCannotHaveMoreThanOneEntityContainer, ComparisonMode.Substring).And.Errors.Should().HaveCount(1);
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
    }
}
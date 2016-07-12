//---------------------------------------------------------------------
// <copyright file="EdmxReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Xml;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm;
using Xunit;
using ErrorStrings = Microsoft.OData.Edm.Strings;

namespace Microsoft.OData.Edm.Tests.Csdl
{
    public class EdmxReaderTests
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

        public EdmxReaderTests()
        {
            this.validReader = XElement.Parse(ValidEdmx).CreateReader();
            this.invalidReader = XElement.Parse(InvalidXml).CreateReader();
        }

        [Fact]
        public void ParsingValidXmlWithNoReferencesShouldSucceed()
        {
            this.RunValidTest(EdmxReader.Parse);
        }

        [Fact]
        public void ParsingInvalidXmlWithNoReferencesShouldThrow()
        {
            this.RunInvalidTest(EdmxReader.Parse);
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
                "<EntityType Name=\"Country\">" +
                    "<Key><PropertyRef Name=\"Name\" /></Key>" +
                    "<Property Name=\"Name\" Type=\"Edm.String\" Nullable=\"false\" />" +
                "</EntityType>" +
                "<ComplexType Name=\"Address\">" +
                    "<Property Name=\"Id\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                    "<NavigationProperty Name=\"City\" Type=\"DefaultNs.City\" Nullable=\"false\" />" +
                "</ComplexType>" +
                "<ComplexType Name=\"WorkAddress\" BaseType=\"DefaultNs.Address\">" +
                    "<NavigationProperty Name=\"Country\" Type=\"DefaultNs.Country\" Nullable=\"false\" /><" +
                "/ComplexType>" +
                "<EntityContainer Name=\"Container\">" +
                "<EntitySet Name=\"People\" EntityType=\"DefaultNs.Person\">" +
                    "<NavigationPropertyBinding Path=\"HomeAddress/City\" Target=\"Cities\" />" +
                    "<NavigationPropertyBinding Path=\"Addresses/City\" Target=\"Cities\" />" +
                    "<NavigationPropertyBinding Path=\"WorkAddress/DefaultNs.WorkAddress/Country\" Target=\"Countries\" />" +
                "</EntitySet>" +
                "<EntitySet Name=\"Cities\" EntityType=\"DefaultNs.City\" />" +
                "<EntitySet Name=\"Countries\" EntityType=\"DefaultNs.Country\" />" +
                "</EntityContainer></Schema>" +
                "</edmx:DataServices>" +
                "</edmx:Edmx>";

            var model = EdmxReader.Parse(XElement.Parse(complexWithNav).CreateReader());
            var people = model.EntityContainer.FindEntitySet("People");
            var address = model.FindType("DefaultNs.Address") as IEdmStructuredType;
            var city = address.FindProperty("City") as IEdmNavigationProperty;
            var cities = model.EntityContainer.FindEntitySet("Cities");
            var navigationTarget = people.FindNavigationTarget(city);
            Assert.Equal(navigationTarget, cities);
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

            var model = EdmxReader.Parse(XElement.Parse(complexWithNav).CreateReader());
            var people = model.EntityContainer.FindEntitySet("Entities");
            var address = model.FindType("DefaultNs.ComplexType") as IEdmStructuredType;
            var city = address.FindProperty("CollectionOfNav") as IEdmNavigationProperty;
            var cities = model.EntityContainer.FindEntitySet("NavEntities");
            var navigationTarget = people.FindNavigationTarget(city);
            Assert.Equal(navigationTarget, cities);
        }

        [Fact]
        public void ParsingValidXmlWithOneReferencesShouldSucceed()
        {
            this.RunValidTest(r => EdmxReader.Parse(r, EdmCoreModel.Instance));
        }

        [Fact]
        public void ParsingInvalidXmlWithOneReferencesShouldThrow()
        {
            this.RunInvalidTest(r => EdmxReader.Parse(r, EdmCoreModel.Instance));
        }

        [Fact]
        public void ParsingValidXmlWithManyReferencesShouldSucceed()
        {
            this.RunValidTest(r => EdmxReader.Parse(r, new IEdmModel[] { EdmCoreModel.Instance }));
        }

        [Fact]
        public void ParsingInvalidXmlWithManyReferencesShouldThrow()
        {
            this.RunInvalidTest(r => EdmxReader.Parse(r, new IEdmModel[] { EdmCoreModel.Instance }));
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
            Action parseAction = () => EdmxReader.Parse(XElement.Parse(EdmxwithMultipleEntityContainers).CreateReader());
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
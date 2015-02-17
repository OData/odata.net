//---------------------------------------------------------------------
// <copyright file="EdmxReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Edm.TDD.Tests
{
    using System;
    using System.Xml;
    using System.Xml.Linq;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Library;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ErrorStrings = Microsoft.OData.Edm.Strings;

    [TestClass]
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

        [TestInitialize]
        public void Init()
        {
            this.validReader = XElement.Parse(ValidEdmx).CreateReader();
            this.invalidReader = XElement.Parse(InvalidXml).CreateReader();
        }

        [TestMethod]
        public void ParsingValidXmlWithNoReferencesShouldSucceed()
        {
            this.RunValidTest(EdmxReader.Parse);
        }

        [TestMethod]
        public void ParsingInvalidXmlWithNoReferencesShouldThrow()
        {
            this.RunInvalidTest(EdmxReader.Parse);
        }

        [TestMethod]
        public void ParsingValidXmlWithOneReferencesShouldSucceed()
        {
            this.RunValidTest(r => EdmxReader.Parse(r, EdmCoreModel.Instance));
        }

        [TestMethod]
        public void ParsingInvalidXmlWithOneReferencesShouldThrow()
        {
            this.RunInvalidTest(r => EdmxReader.Parse(r, EdmCoreModel.Instance));
        }

        [TestMethod]
        public void ParsingValidXmlWithManyReferencesShouldSucceed()
        {
            this.RunValidTest(r => EdmxReader.Parse(r, new IEdmModel[] { EdmCoreModel.Instance }));
        }

        [TestMethod]
        public void ParsingInvalidXmlWithManyReferencesShouldThrow()
        {
            this.RunInvalidTest(r => EdmxReader.Parse(r, new IEdmModel[] { EdmCoreModel.Instance }));
        }

        [TestMethod]
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
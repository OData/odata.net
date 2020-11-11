//---------------------------------------------------------------------
// <copyright file="ODataUrlValidationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//--------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Validation;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataUrlValidatorTests
    {
        private static IEdmModel model;
        private const string invalidRuleText = "This rule is invalid.";

        [Fact]
        public static void InvalidRule()
        {
            const string request = @"company";

            IEdmModel model = GetModel();
            Uri uri = new Uri(request, UriKind.Relative);

            IEnumerable<ODataUrlValidationMessage> errors;
            uri.ValidateODataUrl(model, new ODataUrlValidationRuleSet(new ODataUrlValidationRule[] { invalidRule }), out errors);
            Assert.Single(errors);
            var error = errors.Single();
            Assert.Equal("invalidRule", error.MessageCode);
            Assert.Contains(invalidRuleText, error.Message);
        }

        [Fact]
        public static void FailedParse()
        {
            const string request = @"myInvalidRequest";

            IEdmModel model = GetModel();
            Uri uri = new Uri(request, UriKind.Relative);

            IEnumerable<ODataUrlValidationMessage> errors;
            uri.ValidateODataUrl(model, ODataUrlValidationRuleSet.AllRules, out errors);
            Assert.Single(errors);
            var error = errors.Single();
            Assert.Equal("unableToParseUrl", error.MessageCode);
            Assert.Contains(request, error.Message);
        }

        [Fact]
        public static void MustUseSameModel()
        {
            const string request = @"company";

            IEdmModel model = CreateModel();
            IEdmModel model2 = CreateModel();

            ODataUrlValidator validator = new ODataUrlValidator(model2, ODataUrlValidationRuleSet.AllRules);
            ODataUriParser parser = new ODataUriParser(model, new Uri(request, UriKind.Relative));

            Action validate = () =>
                 {
                     IEnumerable<ODataUrlValidationMessage> errors;
                     parser.Validate(validator, out errors);
                 };

            validate.Throws<ODataException>(Strings.UriValidator_ValidatorMustUseSameModelAsParser);
        }

        [Fact]
        public static void PassingRulesSucceeds()
        {
            const string request = @"company";

            IEdmModel model = CreateModel();
            ODataUriParser parser = new ODataUriParser(model, new Uri(request, UriKind.Relative));

            IEnumerable<ODataUrlValidationMessage> errors;
            parser.Validate(dummyRuleset, out errors);
            Assert.Collection<ODataUrlValidationMessage>(errors, (error) =>
            {
                Assert.Equal("dummyCode", error.MessageCode);
                Assert.Equal("dummyMessage", error.Message);
                Assert.Equal(OData.UriParser.Validation.Severity.Warning, error.Severity);
            });
        }

        [Fact]
        public static void PassingValidatorSucceeds()
        {
            const string request = @"company";

            IEdmModel model = CreateModel();

            ODataUrlValidator validator = new ODataUrlValidator(model, dummyRuleset);
            ODataUriParser parser = new ODataUriParser(model, new Uri(request, UriKind.Relative));
            IEnumerable<ODataUrlValidationMessage> errors;
            parser.Validate(validator, out errors);
            Assert.Collection<ODataUrlValidationMessage>(errors, (error) =>
            {
                Assert.Equal("dummyCode", error.MessageCode);
                Assert.Equal("dummyMessage", error.Message);
                Assert.Equal(OData.UriParser.Validation.Severity.Warning, error.Severity);
            });
        }

        [Fact]
        public static void CallingOnValdiatorSucceeds()
        {
            const string request = @"company";

            IEdmModel model = CreateModel();

            ODataUrlValidator validator = new ODataUrlValidator(model, dummyRuleset);
            ODataUriParser parser = new ODataUriParser(model, new Uri(request, UriKind.Relative));
            ODataUri odataUri = parser.ParseUri();

            IEnumerable<ODataUrlValidationMessage> errors;
            validator.ValidateUrl(odataUri, out errors);
            Assert.Collection<ODataUrlValidationMessage>(errors, (error) =>
            {
                Assert.Equal("dummyCode", error.MessageCode);
                Assert.Equal("dummyMessage", error.Message);
                Assert.Equal(OData.UriParser.Validation.Severity.Warning, error.Severity);
            });
        }

        [Fact]
        public static void CallingOnUriSucceeds()
        {
            const string request = @"company";

            IEdmModel model = CreateModel();

            Uri uri = new Uri(request, UriKind.Relative);

            IEnumerable<ODataUrlValidationMessage> errors;
            uri.ValidateODataUrl(model, dummyRuleset, out errors);
            Assert.Collection<ODataUrlValidationMessage>(errors, (error) =>
            {
                Assert.Equal("dummyCode", error.MessageCode);
                Assert.Equal("dummyMessage", error.Message);
                Assert.Equal(OData.UriParser.Validation.Severity.Warning, error.Severity);
            });
        }

        private static IEdmModel GetModel()
        {
            if (model == null)
            {
                CreateModel();
            }

            return model;
        }

        private static IEdmModel CreateModel()
        {
            // Attempt to load the CSDL into an EdmModel 
            XmlReader reader = XmlReader.Create(new StringReader(JetsonsModel));
            IEnumerable<EdmError> errors;
            if (!CsdlReader.TryParse(reader, out model, out errors))
            {
                throw new Exception("Unable to parse Model");
            }

            return model;
        }

        private static ODataUrlValidationRule<ODataPathSegment> invalidRule = new ODataUrlValidationRule<ODataPathSegment>(
            "invalidRule",
            (context, segment) =>
            {
                throw new Exception(invalidRuleText);
            }
            );

        private static ODataUrlValidationRule dummyRule = new ODataUrlValidationRule<ODataPathSegment>(
            "dummyRule",
            (context, segment) =>
        {
            context.AddMessage("dummyCode", "dummyMessage", OData.UriParser.Validation.Severity.Warning);
        });

        private static ODataUrlValidationRuleSet dummyRuleset = new ODataUrlValidationRuleSet(new ODataUrlValidationRule[]{dummyRule});

        private static string JetsonsModel = @"
<edmx:Edmx xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.0"">
  <edmx:DataServices>
    <Schema xmlns = ""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""Jetsons.Models"">
      <EntityType Name = ""company"" >
        <Key>
          <PropertyRef Name=""stockSymbol""/>
        </Key>
        <Property Name = ""stockSymbol"" Type=""Edm.String"" Nullable=""false""/>
        <Property Name = ""name"" Type=""Edm.String""/>
      </EntityType>
      <EntityContainer Name=""Container"">
        <Singleton Name = ""company"" Type=""Jetsons.Models.company""/>
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

    }
}
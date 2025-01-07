//---------------------------------------------------------------------
// <copyright file="SelectRequiredTests.cs" company="Microsoft">
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
using Microsoft.OData.UriParser.Validation;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class SelectRequiredTests
    {
        private static IEdmModel model;

        [Theory]
        [InlineData(@"company", "company")]
        [InlineData(@"company?$select=*", "company")]
        [InlineData(@"company/employees", "employees")]
        [InlineData(@"company?$expand=employees", "company", "employees")]
        [InlineData(@"company?$expand=employees($select=*)", "company", "employees")]
        [InlineData(@"company/address", "address")]
        [InlineData(@"company?$select=name&$expand=employees", "employees")]
        public static void MissingSelectGeneratesErrors(String request, params string[] expectedErrors)
        {
            IEdmModel model = GetModel();
            Uri uri = new Uri(request, UriKind.Relative);

            IEnumerable<ODataUrlValidationMessage> errors;
            ODataUrlValidationRuleSet rules = new ODataUrlValidationRuleSet(new ODataUrlValidationRule[] { ODataUrlValidationRules.RequireSelectRule });
            uri.ValidateODataUrl(model, rules, out errors);
            int errorCount = errors.Count();
            Assert.Equal(expectedErrors.Count(), errorCount);
            int iError = 0;
            foreach (ODataUrlValidationMessage error in errors)
            {
                Assert.Equal("missingSelect", error.MessageCode);
                Assert.Contains(expectedErrors[iError++], error.Message);
            }
        }

        [Theory]
        [InlineData(@"company?$select=name")]
        [InlineData(@"company/employees?$select=firstName")]
        [InlineData(@"company/address?$select=city,state,zip")]
        [InlineData(@"company?$select=name&$expand=employees($select=firstName)")]
        public static void HasSelectGeneratesNoErrors(String request)
        {
            IEdmModel model = GetModel();
            Uri uri = new Uri(request, UriKind.Relative);

            IEnumerable<ODataUrlValidationMessage> errors;
            ODataUrlValidationRuleSet rules = new ODataUrlValidationRuleSet(new ODataUrlValidationRule[] { ODataUrlValidationRules.RequireSelectRule });
            uri.ValidateODataUrl(model, rules, out errors);
            Assert.Empty(errors);
        }


        private static IEdmModel GetModel()
        {
            if (model == null)
            {
                // Attempt to load the CSDL into an EdmModel 
                XmlReader reader = XmlReader.Create(new StringReader(JetsonsModel));
                IEnumerable<EdmError> errors;
                Assert.True(CsdlReader.TryParse(reader, out model, out errors), "Could not Parse CSDL");
            }

            return model;
        }

        private static string JetsonsModel = @"
<edmx:Edmx xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.0"">
  <edmx:DataServices>
    <Schema xmlns = ""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""Jetsons.Models"">
      <ComplexType Name = ""address"" >
        <Property Name=""city"" Type=""Edm.String""/>
        <Property Name = ""state"" Type=""Edm.String""/>
        <Property Name = ""zip"" Type=""Edm.String""/>
      </ComplexType>
      <EntityType Name = ""company"" >
        <Key>
          <PropertyRef Name=""stockSymbol""/>
        </Key>
        <Property Name = ""stockSymbol"" Type=""Edm.String"" Nullable=""false""/>
        <Property Name = ""name"" Type=""Edm.String""/>
        <Property Name = ""incorporated"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
        <Property Name = ""address"" Type=""Jetsons.Models.address""/>
        <NavigationProperty Name = ""employees"" Type=""Collection(Jetsons.Models.employee)"" ContainsTarget=""true""/>
        <NavigationProperty Name = ""directs"" Type=""Collection(Jetsons.Models.employee)"" ContainsTarget=""true""/>
      </EntityType>
      <EntityType Name = ""employee"" >
        <Key>
          <PropertyRef Name=""id""/>
        </Key>
        <Property Name = ""id"" Type=""Edm.Int32"" Nullable=""false""/>
        <Property Name = ""firstName"" Type=""Edm.String""/>
        <Property Name = ""lastName"" Type=""Edm.String""/>
        <Property Name = ""title"" Type=""Edm.String""/>
      </EntityType>
      <Action Name = ""ResetDataSource"" />
      <EntityContainer Name=""Container"">
        <EntitySet Name = ""competitors"" EntityType=""Jetsons.Models.company""/>
        <Singleton Name = ""company"" Type=""Jetsons.Models.company""/>
        <ActionImport Name = ""ResetDataSource"" Action=""Jetsons.Models.ResetDataSource""/>
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

    }
}

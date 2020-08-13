//---------------------------------------------------------------------
// <copyright file="DeprecationTests.cs" company="Microsoft">
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
    public class DeprecationTests
    {
        private static IEdmModel model;

        [Theory]
        [InlineData(@"company", "name", "state")]
        [InlineData(@"company/employees", "employees")]
        [InlineData(@"competitors", "competitors", "name", "state")]
        [InlineData(@"company/address", "state")]
        [InlineData(@"company/address/state", "state")]
        [InlineData(@"company/address?$select=state", "state")]
        [InlineData(@"company?$expand=employees", "name", "state", "employees")]
        [InlineData(@"company?$select=name", "name")]
        [InlineData(@"competitors?$filter=contains(name,'sprocket')", "competitors", "name", "state", "name")]
        public static void WithDeprecatedElementsGeneratesErrors(String request, params string[] expectedErrors)
        {
            string expectedDateAsString = "2020-03-30";
            Date expectedDate = Date.Parse(expectedDateAsString);
            string expectedRemovalDateAsString = "2022-03-30";
            Date expectedRemovalDate = Date.Parse(expectedRemovalDateAsString);

            IEdmModel model = GetModel();
            ODataUriParser parser = new ODataUriParser(model, new Uri(request, UriKind.Relative));
            
            IEnumerable<ODataUrlValidationMessage> errors;
            ODataUrlValidationRuleSet rules = new ODataUrlValidationRuleSet(new ODataUrlValidationRule[] 
            {
                ODataUrlValidationRules.DeprecatedNavigationSourceRule,
                ODataUrlValidationRules.DeprecatedPropertyRule, 
                ODataUrlValidationRules.DeprecatedTypeRule 
            });
            parser.Validate(rules, out errors);

            Assert.Equal(expectedErrors.Count(), errors.Count());
            foreach(ODataUrlValidationMessage error in errors)
            {
                Assert.Equal(ODataUrlValidationMessageCodes.DeprecatedElement, error.MessageCode);
                object elementName;
                Assert.True(error.ExtendedProperties.TryGetValue("ElementName", out elementName));
                object date;
                Assert.True(error.ExtendedProperties.TryGetValue("Date", out date));
                object removalDate;
                Assert.True(error.ExtendedProperties.TryGetValue("RemovalDate", out removalDate)); 
                object version;
                Assert.True(error.ExtendedProperties.TryGetValue("Version", out version));

                Assert.Contains(elementName as string, expectedErrors);
                Assert.Equal(date as Date?, expectedDate);
                Assert.Equal(version as string, expectedDateAsString);
                Assert.Equal(removalDate as Date?, expectedRemovalDate);
                Assert.Contains(elementName as string, error.Message);
                Assert.Contains(expectedRemovalDateAsString, error.Message);
            }
        }

        [Theory]
        [InlineData(@"company?$select=stockSymbol")]
        [InlineData(@"company/address?$select=city")]
        [InlineData(@"company/address/city")]
        public static void WithoutDeprecatedElementsDoesntGenerateErrors(String request)
        {
            IEdmModel model = GetModel();
            ODataUriParser parser = new ODataUriParser(model, new Uri(request, UriKind.Relative));
            ODataUri uri = parser.ParseUri();

            IEnumerable<ODataUrlValidationMessage> errors;
            ODataUrlValidationRuleSet rules = new ODataUrlValidationRuleSet(new ODataUrlValidationRule[] { ODataUrlValidationRules.DeprecatedPropertyRule, ODataUrlValidationRules.DeprecatedTypeRule });
            parser.Validate(rules, out errors);
            Assert.Empty(errors);
        }

        private static IEdmModel GetModel()
        {
            if (model == null)
            {
                // Attempt to load the CSDL into an EdmModel 
                XmlReader reader = XmlReader.Create(new StringReader(JetsonsModel));
                IEnumerable<EdmError> errors;
                Assert.True(CsdlReader.TryParse(reader, out model, out errors), "Failed Parsing Model");
            }

            return model;
        }

        private static string JetsonsModel = @"
<edmx:Edmx xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.0"">
  <edmx:DataServices>
    <Schema xmlns = ""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""Jetsons.Models"">
      <ComplexType Name = ""address"" >
        <Property Name=""city"" Type=""Edm.String""/>
        <Property Name=""subAddress"" Type=""Jetsons.Models.address""/>
        <Property Name = ""state"" Type=""Edm.String"">
          <Annotation Term = ""Core.Revisions"" >
            <Collection>
              <Record>
                <PropertyValue Property=""Version"" String=""2020-03-30""/>
                <PropertyValue Property = ""Kind"" EnumMember=""RevisionKind/Deprecated""/>
                <PropertyValue Property = ""Description"" String=""'state' is deprecated and will be retired on 2022-03-30. Please use 'region'.""/>
                <PropertyValue Property=""Date"" Date=""2020-03-30""/>
                <PropertyValue Property=""RemovalDate"" Date=""2022-03-30""/>
              </Record>
            </Collection>
          </Annotation>
        </Property>
        <Property Name = ""zip"" Type=""Edm.String""/>
      </ComplexType>
      <EntityType Name = ""company"" >
        <Key>
          <PropertyRef Name=""stockSymbol""/>
        </Key>
        <Property Name = ""stockSymbol"" Type=""Edm.String"" Nullable=""false""/>
        <Property Name = ""name_2"" Type=""Edm.String""/>
        <Property Name = ""name"" Type=""Edm.String"">
          <Annotation Term = ""Core.Revisions"" >
            <Collection>
              <Record>
                <PropertyValue Property=""Date"" Date=""2020-03-30""/>
                <PropertyValue Property=""RemovalDate"" Date=""2022-03-30""/>
                <PropertyValue Property=""Version"" String=""2020-03-30""/>
                <PropertyValue Property = ""Kind"" EnumMember=""RevisionKind/Deprecated""/>
                <PropertyValue Property = ""Description"" String=""'name' is deprecated and will be retired on 2022-03-30. Please use 'name2'.""/>
              </Record>
            </Collection>
          </Annotation>
        </Property>
        <Property Name = ""incorporated"" Type=""Edm.DateTimeOffset"" Nullable=""false""/>
        <Property Name = ""address"" Type=""Jetsons.Models.address""/>
        <NavigationProperty Name = ""employees"" Type=""Collection(Jetsons.Models.employee)"" ContainsTarget=""true"">
          <Annotation Term = ""Core.Revisions"" >
            <Collection>
              <Record>
                <PropertyValue Property=""Date"" Date=""2020-03-30""/>
                <PropertyValue Property=""RemovalDate"" Date=""2022-03-30""/>
                <PropertyValue Property=""Version"" String=""2020-03-30""/>
                <PropertyValue Property = ""Kind"" EnumMember=""RevisionKind/Deprecated""/>
                <PropertyValue Property = ""Description"" String=""'employees' is deprecated and will be retired on 2022-03-30. Please use 'directs'.""/>
              </Record>
            </Collection>
          </Annotation>
        </NavigationProperty>
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
        <EntitySet Name = ""competitors"" EntityType=""Jetsons.Models.company"">
          <Annotation Term = ""Core.Revisions"" >
            <Collection>
              <Record>
                <PropertyValue Property=""Date"" Date=""2020-03-30""/>
                <PropertyValue Property=""RemovalDate"" Date=""2022-03-30""/>
                <PropertyValue Property=""Version"" String=""2020-03-30""/>
                <PropertyValue Property = ""Kind"" EnumMember=""RevisionKind/Deprecated""/>
                <PropertyValue Property = ""Description"" String=""'competitors' is deprecated and will be retired on 2022-03-30.""/>
              </Record>
            </Collection>
          </Annotation>
        </EntitySet>
        <Singleton Name = ""company"" Type=""Jetsons.Models.company""/>
        <ActionImport Name = ""ResetDataSource"" Action=""Jetsons.Models.ResetDataSource""/>
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
    }
}


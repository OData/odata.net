//---------------------------------------------------------------------
// <copyright file="ExtendedContainerElementsFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using FluentAssertions;
using Microsoft.OData.Core.Tests.UriParser;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Library.Expressions;
using Microsoft.OData.Edm.Validation;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriParser
{
    public class ExtendedContainerElementsFunctionalTests
    {
        private static IEdmModel model;

        static ExtendedContainerElementsFunctionalTests()
        {
            string mainModelxml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:Reference Uri=""http://host/schema/Location.xml"">
    <edmx:Include Namespace=""Namespace1"" Alias=""A1"" />
  </edmx:Reference>
  <edmx:DataServices>
    <Schema Namespace=""Namespace0"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
        <EntityType Name=""Market"">
            <Key>
                <PropertyRef Name=""MarketID"" />
            </Key>
             <Property Name=""MarketID"" Type=""Edm.Int32"" Nullable=""false"" />
            <NavigationProperty Name=""Customers"" Type=""Collection(Namespace1.VipCustomer)"" />
        </EntityType>
        <EntityContainer Name=""DefaultContainer0"" Extends=""Namespace1.Container_sub"">
            <EntitySet Name=""EntitySet0"" EntityType=""Namespace0.Market"">
                <NavigationPropertyBinding Path=""Customers"" Target=""EntitySet1"" />
            </EntitySet>
            <FunctionImport Name=""FunctionImport0"" Function=""Namespace1.Function1"" EntitySet=""EntitySet1""/>
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string model1xml = @"<?xml version=""1.0"" encoding=""utf-16""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">  
        <EntityType Name=""VipCustomer"">
            <Key>
                <PropertyRef Name=""VipCustomerID"" />
            </Key>
            <Property Name=""VipCustomerID"" Type=""Edm.String"" Nullable=""false"" />
        </EntityType>
        <Function Name=""Function1"">
            <ReturnType Type=""Namespace1.VipCustomer"" />
        </Function>
        <EntityContainer Name=""Container_sub"">
            <Singleton Name=""Singleton1"" Type=""Namespace1.VipCustomer"" />
            <EntitySet Name=""EntitySet1"" EntityType=""Namespace1.VipCustomer"" />
            <FunctionImport Name=""FunctionImport1"" Function=""Namespace1.Function1"" EntitySet=""EntitySet1""/>
        </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            IEnumerable<EdmError> errors;
            IEdmModel model1;
            bool parsed = EdmxReader.TryParse(XmlReader.Create(new StringReader(model1xml)), out model1, out errors);
            Assert.True(parsed);

            parsed = EdmxReader.TryParse(XmlReader.Create(new StringReader(mainModelxml)), new IEdmModel[] { model1 }, out model, out errors);
            Assert.True(parsed);
        }

        [Fact]
        public void ParseEntitySetInExtendedContainer()
        {
            var path = RunParsePath("EntitySet1");
            path.LastSegment.ShouldBeEntitySetSegment(model.FindDeclaredEntitySet("EntitySet1"));
        }

        [Fact]
        public void ParseSingletonInExtendedContainer()
        {
            var path = RunParsePath("Singleton1");
            path.LastSegment.ShouldBeSingletonSegment(model.FindDeclaredSingleton("Singleton1"));
        }

        [Fact]
        public void ParseOperationImportsInExtendedContainer()
        {
            var path = RunParsePath("FunctionImport1");
            var operationImports = model.FindDeclaredOperationImports("FunctionImport1").ToArray();
            path.LastSegment.ShouldBeOperationImportSegment(operationImports);
        }

        [Fact]
        public void ParseOperationImportsWithEntitySetExpressionInExtendedContainer()
        {
            var path = RunParsePath("FunctionImport0");
            var operationImports = model.FindDeclaredOperationImports("FunctionImport0").ToArray();
            path.LastSegment.ShouldBeOperationImportSegment(operationImports);
            IEdmOperationImport operationImport = operationImports.Single();
            var expression = operationImport.EntitySet as EdmEntitySetReferenceExpression;
            var set1 = model.FindDeclaredEntitySet("EntitySet1");
            expression.ReferencedEntitySet.Should().Be(set1);
        }

        [Fact]
        public void ParseNavigationTargetInExtendedContainer()
        {
            var path = RunParsePath("EntitySet0(1)/Customers");
            var market = model.FindType("Namespace0.Market") as IEdmEntityType;
            var nav = market.FindProperty("Customers") as IEdmNavigationProperty;
            path.LastSegment.ShouldBeNavigationPropertySegment(nav);
            var set0 = model.FindDeclaredEntitySet("EntitySet0");
            var set1 = model.FindDeclaredEntitySet("EntitySet1");
            set0.FindNavigationTarget(nav).Should().Be(set1);
        }

        internal static ODataPath RunParsePath(string path)
        {
            return new ODataUriParser(model, new Uri(path, UriKind.Relative)).ParsePath();
        }
    }
}

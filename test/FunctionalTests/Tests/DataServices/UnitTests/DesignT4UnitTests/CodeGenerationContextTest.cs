//---------------------------------------------------------------------
// <copyright file="CodeGenerationContextTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client.Design.T4.UnitTests
{
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Linq;

    [TestClass]
    public class CodeGenerationContextTest
    {
        private const string EdmxSimple = @"<?xml version=""1.0"" standalone=""yes"" ?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" 
            xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" 
            xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""TestType"">
        <Key>
          <PropertyRef Name=""KeyProp"" />
        </Key>
        <Property Name=""KeyProp"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""ValueProp"" Type=""Edm.String"" Nullable=""false"" />
      </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string EdmxWithNamespaceInKeywords = @"<?xml version=""1.0"" standalone=""yes"" ?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""event.string.int"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" 
            xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" 
            xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""TestType"">
        <Key>
          <PropertyRef Name=""KeyProp"" />
        </Key>
        <Property Name=""KeyProp"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""ValueProp"" Type=""Edm.String"" Nullable=""false"" />
      </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string EdmxWithDifferentNamespaces = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""ComplexType"">
        <Property Name=""Value"" Type=""Edm.String"" Nullable=""false"" />
      </ComplexType>
      <EntityType Name=""EntityType"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Guid"" Nullable=""false"" />
        <Property Name=""Complex"" Type=""Namespace1.ComplexType"" Nullable=""true"" />
      </EntityType>
      <EntityContainer Name=""EntityContainer"">
        <EntitySet Name=""Set1"" EntityType=""Namespace1.EntityType"" />
        <EntitySet Name=""Set2"" EntityType=""Namespace2.EntityType"" />
      </EntityContainer>
    </Schema>
    <Schema Namespace=""Namespace2"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""ComplexType"">
        <Property Name=""Value"" Type=""Edm.String"" Nullable=""false"" />
      </ComplexType>
      <EntityType Name=""EntityType"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Guid"" Nullable=""false"" />
        <Property Name=""Complex"" Type=""Namespace2.ComplexType"" Nullable=""true"" />
      </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string EdmxWithEntityHierarchy = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""EntityBase"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
      </EntityType>
      <EntityType Name=""EntityType"" BaseType=""Namespace1.EntityBase"">
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
      </EntityType>
      <EntityContainer Name=""EntityContainer"">
        <EntitySet Name=""Set1"" EntityType=""Namespace1.EntityType"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string EdmxWithKeyAsSegmentAnnotaion = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Namespace1"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""EntityType"" />
      <EntityContainer Name=""EntityContainer"">
        <EntitySet Name=""Set1"" EntityType=""Namespace1.EntityType"" />
      </EntityContainer>
      <Annotations Target=""Namespace2.EntityContainer"">
        <Annotation Term=""Com.Microsoft.OData.Service.Conventions.V1.UrlConventions"" String=""KeyAsSegment"" />
      </Annotations>
    </Schema>
    <Schema Namespace=""Namespace2"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""EntityType"" />
      <Annotations Target=""Namespace1.EntityContainer"">
        <Annotation Term=""Com.Microsoft.OData.Service.Conventions.V1.UrlConventions"" String=""KeyAsSegment"" />
      </Annotations>
    </Schema>
    <Schema Namespace=""Namespace3"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""EntityType"" />
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        [TestMethod]
        public void NamespacesInModelShouldContainOneNamespaceInOneNamesapceModel()
        {
            ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(EdmxSimple, null);
            context.NamespacesInModel.Length.Should().Be(1);
        }

        [TestMethod]
        public void NamespacesInModelShouldContainTwoNamespacesInTwoNamesapceModel()
        {
            ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(EdmxWithDifferentNamespaces, null);
            context.NamespacesInModel.Length.Should().Be(2);
        }

        [TestMethod]
        public void NamespaceMapShouldBeEmptyIfNamespacePrefixIsNull()
        {
            ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(EdmxSimple, null);
            context.NamespaceMap.Count.Should().Be(0);
        }

        [TestMethod]
        public void NamespaceMapShouldNotBeEmptyIfNamespacePrefixIsNotNull()
        {
            ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(EdmxSimple, "Foo");
            context.NamespaceMap.Count.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public void ModelHasInheritanceShouldBeFalseIfEdmModelHasnotInheritance()
        {
            ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(EdmxSimple, null);
            context.ModelHasInheritance.Should().BeFalse();
        }

        [TestMethod]
        public void ModelHasInheritanceShouldBeTrueIfEdmModelHasInheritance()
        {
            ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(EdmxWithEntityHierarchy, null);
            context.ModelHasInheritance.Should().BeTrue();
        }

        [TestMethod]
        public void NeedResolveNameFromTypeShouldBeTrueIfEdmModelHasInheritance()
        {
            ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(EdmxWithEntityHierarchy, null);
            context.NeedResolveNameFromType.Should().BeTrue();
        }

        [TestMethod]
        public void NeedResolveNameFromTypeShouldBeTrueIfNamespacePrefixIsNotNull()
        {
            ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(EdmxSimple, "Foo");
            context.NeedResolveNameFromType.Should().BeTrue();
        }

        [TestMethod]
        public void NeedResolveNameFromTypeShouldFalseTrueIfEdmModeHasNotInheritanceAndNamespacePrefixIsIsNull()
        {
            ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(EdmxSimple, null);
            context.NeedResolveNameFromType.Should().BeFalse();
        }

        [TestMethod]
        public void NeedResolveTypeFromNameShouldBeTrueIfNamespacePrefixIsNotNull()
        {
            ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(EdmxSimple, "Foo");
            context.NeedResolveNameFromType.Should().BeTrue();
        }

        [TestMethod]
        public void NeedResolveTypeFromNameShouldBeFalseIfNamespacePrefixIsNull()
        {
            ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(EdmxSimple, null);
            context.NeedResolveNameFromType.Should().BeFalse();
        }

        [TestMethod]
        public void GetPrefixedNamespaceShouldReturnNamespaceWithPrefixIfNamespacePrefixIsNotNull()
        {
            ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(EdmxSimple, "Foo");
            context.GetPrefixedNamespace("Test", new ODataT4CodeGenerator.ODataClientCSharpTemplate(context), false, false).Should().Be("Foo.Test");
        }

        [TestMethod]
        public void GetPrefixedNamespaceShouldReturnNamespaceWithoutPrefixIfNamespacePrefixIsNull()
        {
            ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(EdmxSimple, null);
            context.GetPrefixedNamespace("Test", new ODataT4CodeGenerator.ODataClientCSharpTemplate(context), false, false).Should().Be("Test");
        }

        [TestMethod]
        public void GetPrefixedNamespaceShouldReturnFixedNamespaceWithoutPrefixIfNamespacePrefixIsNullAndNamespaceIsKeyword()
        {
            ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(EdmxWithNamespaceInKeywords, null);
            context.GetPrefixedNamespace("event.string.int", new ODataT4CodeGenerator.ODataClientCSharpTemplate(context), true, false).Should().Be("@event.@string.@int");
        }

        [TestMethod]
        public void GetPrefixedNamespaceShouldReturnFixedOriginalNamespaceWithoutPrefixIfNamespacePrefixIsNullAndNamespaceIsKeyword()
        {
            ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(EdmxWithNamespaceInKeywords, null);
            context.GetPrefixedNamespace("event.string.int", new ODataT4CodeGenerator.ODataClientCSharpTemplate(context), false, false).Should().Be("event.string.int");
        }

        [TestMethod]
        public void GetPrefixedNamespaceShouldReturnNamespaceWithoutGlobalAttached()
        {
            ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(EdmxSimple, "Test.Foo");
            context.GetPrefixedNamespace("Test", new ODataT4CodeGenerator.ODataClientCSharpTemplate(context), false, true).Should().Be("global::Test.Foo.Test");
        }

        [TestMethod]
        public void GetPrefixedFullNameShouldReturnExpectedFullName()
        {
            ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(EdmxWithEntityHierarchy, null);
            IEdmSchemaElement[] schemaElements = context.GetSchemaElements("Namespace1").ToArray();
            IEdmEntityContainer container = schemaElements.OfType<IEdmEntityContainer>().First();
            context.GetPrefixedFullName(container, container.Name, new ODataT4CodeGenerator.ODataClientCSharpTemplate(context)).Should().Be("global::Namespace1.EntityContainer");
        }

        [TestMethod]
        public void GetPrefixedFullNameShouldReturnFullNameWithPrefixNamespaceIfNamespacePrefixIsNotNull()
        {
            ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(EdmxWithKeyAsSegmentAnnotaion, "Foo");
            IEdmSchemaElement[] schemaElements = context.GetSchemaElements("Namespace1").ToArray();
            IEdmEntityContainer container = schemaElements.OfType<IEdmEntityContainer>().First();
            context.GetPrefixedFullName(container, container.Name, new ODataT4CodeGenerator.ODataClientCSharpTemplate(context)).Should().Be("global::Foo.Namespace1.EntityContainer");
        }

        [TestMethod]
        public void NeedToSetKeyAsSegmentShouldBeTureInAnnotaionTargetingNamespaces()
        {
            ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(EdmxWithKeyAsSegmentAnnotaion, null);
            IEdmSchemaElement[] schemaElements = context.GetSchemaElements("Namespace1").ToArray();
            context.UseKeyAsSegmentUrlConvention(schemaElements.OfType<IEdmEntityContainer>().First()).Should().BeTrue();
        }

        [TestMethod]
        public void GetSchemaElementsShouldReturnIEdmSchemaElementIfInputNamespaceExistInEdmModel()
        {
            ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(EdmxSimple, null);
            IEdmSchemaElement[] schemaElements = context.GetSchemaElements("Test").ToArray();
            schemaElements.Length.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public void GetSchemaElementsShouldReturnEmptyIfInputNamespaceNotExistInEdmModel()
        {
            ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(EdmxSimple, null);
            IEdmSchemaElement[] schemaElements = context.GetSchemaElements("Foo").ToArray();
            schemaElements.Length.Should().Be(0);
        }

        [TestMethod]
        public void GetEdmxStringFromMetadataPathShouldThrowExceptionIfInputIsInvalidUri()
        {
            try
            {
                Uri metadataUri = new Uri("ftp://testurl/");
                ODataT4CodeGenerator.CodeGenerationContext context = new ODataT4CodeGenerator.CodeGenerationContext(metadataUri, null);
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "Only file, http, https schemes are supported for paths to metadata source locations.");
            }
        }
    }
}

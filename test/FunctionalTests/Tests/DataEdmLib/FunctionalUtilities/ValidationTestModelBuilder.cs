//---------------------------------------------------------------------
// <copyright file="ValidationTestModelBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;
    using EdmLibTests.StubEdm;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class ValidationTestModelBuilder
    {
        public static XElement[] DuplicatePropertyName(EdmVersion edmVersion)
        {
            var csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CollectionAtomic"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""ComplexTypeA"">
    <Property Name=""Id"" Type=""Int32""/>
    <Property Name=""Collection"" Type=""Collection(Int32)""/>
    <Property Name=""Collection"" Type=""Collection(Int32)""/>
  </ComplexType>
  <EntityType Name=""ComplexTypeE"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <Property Name=""Collection"" Type=""Collection(Int32)""/>
    <Property Name=""Collection"" Type=""Collection(Int32)""/>
  </EntityType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] SystemNamespace(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Edm"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Clod"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""Bar"" Type=""Int32"" />
  </EntityType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] InvalidEntitySetNameReference(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherId"" Type=""Int32"" />
    <NavigationProperty Name=""Navigation"" Type=""Bork.Entity2"" Nullable=""false"" Partner=""Navigation"" /> 
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherId"" Type=""Int32"" Nullable=""false""/> 
    <NavigationProperty Name=""Navigation"" Type=""Collection(Bork.Entity1)"" Partner=""Navigation"" /> 
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Entity1_a"" EntityType=""Bork.Entity1"">
      <NavigationPropertyBinding Path=""Navigation"" Target=""Entity2_b"" />
    </EntitySet>
    <EntitySet Name=""Entity2_a"" EntityType=""Bork.Entity2"">
      <NavigationPropertyBinding Path=""Navigation"" Target=""Entity1_b"" />
    </EntitySet>
  </EntityContainer>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] ConcurrencyRedefinedOnSubTypeOfEntitySetType(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
  </EntityType>
  <EntityType Name=""Entity2"" BaseType=""Bork.Entity1"">
    <Property Name=""OtherFixed"" Type=""Int32"" /> 
    <Property Name=""OtherNone"" Type=""Int32"" /> 
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Entity1_a"" EntityType=""Bork.Entity1"" />
    <EntitySet Name=""Entity2_a"" EntityType=""Bork.Entity2"" />
  </EntityContainer>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] DuplicateEntityContainerMemberName(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <Function Name=""Function""><ReturnType Type=""Edm.String"" /></Function>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Entity1"" EntityType=""Bork.Entity1""/>
    <EntitySet Name=""Entity1"" EntityType=""Bork.Entity2""/>
    <FunctionImport Name=""Entity1"" Function=""Bork.Function""/>
  </EntityContainer>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] EdmEntityTypeDuplicatePropertyNameSpecifiedInEntityKey(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] InvalidKeyNullablePart(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""true""/>
  </EntityType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] EntityKeyMustBeScalar(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Bork.Foo"" Nullable=""false""/>
  </EntityType>
  <ComplexType Name=""Foo"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""true""/>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] KeyMissingOnEntityType(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity3"" BaseType=""Bork.Entity1""/>
  <EntityType Name=""Entity4"" BaseType=""Bork.Entity2""/>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] AbstractEntityTypeWithoutKey(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"" Abstract=""true"">
  </EntityType>
  <EntityType Name=""Entity2"" BaseType=""Bork.Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity3"" BaseType=""Bork.Entity2""/>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] InvalidMemberNameMatchesTypeName(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Entity1"" />
    </Key>
    <Property Name=""Entity1"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] PropertyNameAlreadyDefinedDuplicate(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""Dupe"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""Dupe"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""Dup"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity3"" BaseType=""Bork.Entity2"">
    <Property Name=""Dup"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity4"" BaseType=""Bork.Entity3"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] IEdmInvalidOperationMultipleEndsInAssociatedNavigationProperties(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <NavigationProperty Name=""ToEntity2"" Type=""Bork.Entity2"" Nullable=""false"" Partner=""ToEntity1"">
      <OnDelete Action=""Cascade"" />
    </NavigationProperty>
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherId"" Type=""Int32""  Nullable=""false""/> 
    <NavigationProperty Name=""ToEntity1"" Type=""Bork.Entity1"" Nullable=""false"" Partner=""ToEntity2"">
      <OnDelete Action=""Cascade"" />
    </NavigationProperty>
  </EntityType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] IEdmAssociationTypeEndWithManyMultiplicityCannotHaveOperationsSpecified(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <NavigationProperty Name=""ToEntity2"" Type=""Collection(Bork.Entity2)"" Partner=""ToEntity1"" />
    <NavigationProperty Name=""ToEntity2Again"" Type=""Collection(Bork.Entity2)"" Partner=""ToEntity1Again"" />
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherId"" Type=""Int32""  Nullable=""false""/> 
    <NavigationProperty Name=""ToEntity1"" Type=""Bork.Entity1"" Nullable=""false"" Partner=""ToEntity2"">
      <ReferentialConstraint Property=""OtherId"" ReferencedProperty=""Id"" />
      <OnDelete Action=""Cascade"" />
    </NavigationProperty>
    <NavigationProperty Name=""ToEntity1Again"" Type=""Bork.Entity1"" Nullable=""false"" Partner=""ToEntity2Again"">
      <ReferentialConstraint Property=""OtherId"" ReferencedProperty=""Id"" />
      <OnDelete Action=""Cascade"" />
    </NavigationProperty>
  </EntityType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] IEdmReferentialConstraintInvalidMultiplicityFromRoleToPropertyNullable(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <NavigationProperty Name=""ToEntity2"" Type=""Collection(Bork.Entity2)"" Partner=""ToEntity1"" />
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherId"" Type=""Int32""/> 
  <NavigationProperty Name=""ToEntity1"" Type=""Bork.Entity1"" Nullable=""false""  Partner=""ToEntity2"">
    <ReferentialConstraint Property=""OtherId"" ReferencedProperty=""Id"" />
  </NavigationProperty>
  </EntityType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] IEdmReferentialConstraintInvalidMultiplicityFromRoleUpperBoundMustBeOne(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherId"" Type=""Int32""  Nullable=""false""/> 
    <NavigationProperty Name=""ToEntity2"" Type=""Collection(Bork.Entity2)"" Partner=""ToEntity1"" />
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <NavigationProperty Name=""ToEntity1"" Type=""Bork.Entity1"" Partner=""ToEntity2"">
      <ReferentialConstraint Property=""Id"" ReferencedProperty=""Id"" />
    </NavigationProperty>
  </EntityType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] IEdmReferentialConstraintInvalidMultiplicityToRoleUpperBoundMustBeMany(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <NavigationProperty Name=""ToEntity2"" Type=""Bork.Entity2"" Nullable=""false"" Partner=""ToEntity1"" />
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherId"" Type=""Int32""  Nullable=""false""/> 
    <NavigationProperty Name=""ToEntity1"" Type=""Bork.Entity1"" Nullable=""false"" Partner=""ToEntity2"">
      <ReferentialConstraint Property=""OtherId"" ReferencedProperty=""Id"" />
    </NavigationProperty>
  </EntityType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] IEdmReferentialConstraintInvalidMultiplicityToRoleUpperBoundMustBeOne(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <NavigationProperty Name=""ToEntity2"" Type=""Bork.Entity2"" Partner=""ToEntity1"" />
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherId"" Type=""Int32""  Nullable=""true"" /> 
    <NavigationProperty Name=""ToEntity1"" Type=""Bork.Entity1"" Partner=""ToEntity2"">
      <ReferentialConstraint Property=""OtherId"" ReferencedProperty=""Id"" />
    </NavigationProperty>
  </EntityType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] IEdmReferentialConstraintTypeMismatchRelationshipConstraint(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <NavigationProperty Name=""ToEntity2"" Type=""Collection(Bork.Entity2)"" Partner=""ToEntity1"" />
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherId"" Type=""String""  Nullable=""false""/> 
    <NavigationProperty Name=""ToEntity1"" Type=""Bork.Entity1"" Nullable=""false"" Partner=""ToEntity2"">
      <ReferentialConstraint Property=""OtherId"" ReferencedProperty=""Id"" />
    </NavigationProperty>
  </EntityType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        [CustomCsdlSchemaCompliantTest]
        public static XElement[] ComplexTypeIsAbstractSupportInV40(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Complex"" Abstract=""true"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        [CustomCsdlSchemaCompliantTest]
        public static XElement[] EdmComplexTypeInvalidIsPolymorphic(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Complex1"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>
  <ComplexType Name=""Complex2"" BaseType=""Bork.Complex1"">
    <Property Name=""Id2"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] IEdmReferentialConstraintInvalidMultiplicityFromRoleToPropertyNonNullableV1(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <NavigationProperty Name=""ToEntity2"" Type=""Collection(Bork.Entity2)"" Partner=""ToEntity1"" />
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
        <PropertyRef Name=""OtherId"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherId"" Type=""Int32"" Nullable=""false""/> 
     <NavigationProperty Name=""ToEntity1"" Type=""Bork.Entity1"" Partner=""ToEntity2"">
      <ReferentialConstraint Property=""Id"" ReferencedProperty=""Id"" />
    </NavigationProperty>
  </EntityType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] IEdmReferentialConstraintInvalidToPropertyInRelationshipConstraintV1(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <NavigationProperty Name=""ToEntity2"" Type=""Collection(Bork.Entity2)"" Partner=""ToEntity1"" />
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherId"" Type=""Int32""/> 
    <NavigationProperty Name=""ToEntity1"" Type=""Bork.Entity1"" Partner=""ToEntity2"">
      <ReferentialConstraint Property=""OtherId"" ReferencedProperty=""Id"" />
    </NavigationProperty>
  </EntityType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] EdmPropertyNullableComplexType(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Complex1"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>
  <ComplexType Name=""Complex2"">
    <Property Name=""Id2"" Type=""Bork.Complex1"" Nullable=""true""/>
  </ComplexType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] EdmPropertyInvalidPropertyType(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <ComplexType Name=""Complex1"">
    <Property Name=""Id"" Type=""Collection(Int32)"" Nullable=""false""/>
    <Property Name=""Id2"" Type=""Bork.Entity1"" Nullable=""false""/>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] IEdmFunctionBaseParameterNameAlreadyDefinedDuplicate(EdmVersion edmVersion)
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
    </EntityType>
    <Function Name=""peopleWhoAreAwesome""><ReturnType Type=""Collection(foo.Person)""/>
      <Parameter Name=""awesomeName"" Type=""String"" />
      <Parameter Name=""awesomeName"" Type=""String"" />
    </Function>
    <EntityContainer Name=""fooContainer"" >
        <EntitySet Name=""People"" EntityType=""foo.Person"" />
    </EntityContainer>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] IEdmOperationImportOperationImportEntityTypeDoesNotMatchEntitySet(EdmVersion edmVersion)
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
    </EntityType>
    <EntityType Name=""OtherPerson"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
    </EntityType>
    <Function Name=""peopleWhoAreAwesome""><ReturnType Type=""Collection(foo.Person)"" /></Function>
    <EntityContainer Name=""fooContainer"" >
        <EntitySet Name=""People"" EntityType=""foo.OtherPerson"" />
        <FunctionImport Name=""peopleWhoAreAwesome"" Function=""foo.peopleWhoAreAwesome"" EntitySet=""People"" />
    </EntityContainer>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] IEdmOperationImportOperationImportSpecifiesEntitySetButDoesNotReturnEntityType(EdmVersion edmVersion)
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
    </EntityType>
    <Function Name=""peopleWhoAreAwesome""><ReturnType Type=""Collection(Int32)"" /></Function>
    <EntityContainer Name=""fooContainer"" >
        <EntitySet Name=""People"" EntityType=""foo.Person"" />
        <FunctionImport Name=""peopleWhoAreAwesome"" EntitySet=""People"" Function=""foo.peopleWhoAreAwesome"" />
    </EntityContainer>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] IEdmComposableFunctionNoReturnType(EdmVersion edmVersion)
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Function Name=""peopleWhoAreAwesome"" IsComposable=""true""/>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] IEdmEntityReferenceTypeEntityTypeNotBad(EdmVersion edmVersion)
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
        <Property Name=""Birthplace"" Type=""Bork.BadComplexType"" />
    </EntityType>
    <Function Name=""peopleWhoAreAwesome""><ReturnType Type=""Ref(Bork.BadEntityType)""/>
        <Parameter Name=""awesomeName"" Type=""String"" />
    </Function>
    <EntityContainer Name=""fooContainer"" >
        <EntitySet Name=""People"" EntityType=""foo.Person"" />
        <FunctionImport Name=""peopleWhoAreAwesome"" Function=""foo.peopleWhoAreAwesome"" />
    </EntityContainer>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] IEdmEntitySetElementTypeNotBad(EdmVersion edmVersion)
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
    </EntityType>
    <EntityContainer Name=""fooContainer"" >
        <EntitySet Name=""People"" EntityType=""foo.BadEntity"" />
    </EntityContainer>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static IEdmModel EdmModelNameMustNotBeWhiteSpace()
        {
            EdmModel model = new EdmModel();
            EdmComplexType t1 = new EdmComplexType("", " ");
            t1.AddStructuralProperty("Data", EdmCoreModel.Instance.GetString(true));
            model.AddElement(t1);
            return model;
        }

        public static IEdmModel NameIsTooLong()
        {
            EdmModel model = new EdmModel();
            EdmComplexType t1 = new EdmComplexType("Foo", new string('A', 481));
            t1.AddStructuralProperty("Data", EdmCoreModel.Instance.GetString(true));
            model.AddElement(t1);

            return model;
        }

        public static IEdmModel IEdmNamedElementNameIsNotAllowed()
        {
            var model = new EdmModel();
            var t1 = new EdmComplexType("Foo1", "_!@#$%^&*()");
            t1.AddStructuralProperty("Data", EdmCoreModel.Instance.GetString(true));
            model.AddElement(t1);
            var t2 = new EdmEntityType("Foo2", "_!@#$%^&*()");
            t2.AddStructuralProperty("Data", EdmCoreModel.Instance.GetString(true));
            var id = t2.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt16(false), null);
            t2.AddKeys(id);
            model.AddElement(t2);

            var t3 = new EdmEntityType(string.Empty, string.Empty);
            id = t3.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt16(false), null);
            t3.AddKeys(id);
            model.AddElement(t3);

            return model;
        }

        public static IEdmModel IEdmSchemaElementNameIsNotAllowed()
        {
            EdmModel model = new EdmModel();
            EdmComplexType t1 = new EdmComplexType("_!@#$%^&*()", "Bar");
            t1.AddStructuralProperty("Data", EdmCoreModel.Instance.GetString(true));
            model.AddElement(t1);

            return model;
        }

        public static XElement[] IEdmModelTypeNameAlreadyDefinedDuplicate(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Dupe"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>
  <ComplexType Name=""Dupe"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        private static XElement[] FixUpWithEdmVersion(string csdl, EdmVersion edmVersion)
        {
            return FixUpWithEdmVersion(new string[] { csdl }, edmVersion);
        }

        private static XElement[] FixUpWithEdmVersion(string[] csdls, EdmVersion edmVersion)
        {
            return csdls.Select(n => XElement.Parse(ModelBuilderHelpers.ReplaceCsdlNamespaceForEdmVersion(n, edmVersion), LoadOptions.SetLineInfo)).ToArray();
        }

        public static XElement[] OperationSupportInV40(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CollectionAtomic"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Function Name=""GetAge""><ReturnType Type=""Edm.Int32""/>
  </Function>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] InstantiateAbstractEntityType(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"" Abstract=""true"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity2"" BaseType=""Bork.Entity1"">
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Entity1_a"" EntityType=""Bork.Entity1"" />
    <EntitySet Name=""Entity2_a"" EntityType=""Bork.Entity2"" />
  </EntityContainer>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] InstantiateAbstractEntityTypeWithoutKey(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"" Abstract=""true"">
  </EntityType>
  <EntityType Name=""Entity2"" BaseType=""Bork.Entity1"" Abstract=""true"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Entity1_a"" EntityType=""Bork.Entity1"" />
    <EntitySet Name=""Entity2_a"" EntityType=""Bork.Entity2"" />
  </EntityContainer>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        [CustomCsdlSchemaCompliantTest]
        public static XElement[] OpenTypeSupportInV40(EdmVersion? edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"" OpenType=""true"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity2"" BaseType=""Bork.Entity1"" OpenType=""false"">
  </EntityType>
  <ComplexType Name=""Complex1"" OpenType=""true"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>
  <ComplexType Name=""Complex2"" OpenType=""true"">
  </ComplexType>
  <ComplexType Name=""Complex3"" OpenType=""false"">
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>
  <ComplexType Name=""Complex4"" OpenType=""false"">
  </ComplexType>
</Schema>";

            return edmVersion != null ? FixUpWithEdmVersion(csdl, edmVersion.Value) : new XElement[] { XElement.Parse(csdl) };
        }

        public static XElement[] BaseTypeStructuralIntegrity(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity2"" BaseType=""Bork.Entity1"">
  </EntityType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] CyclicEntityTypeBaseType(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"" BaseType=""Bork.Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity2"" BaseType=""Bork.Entity1"">
  </EntityType>
  <EntityType Name=""Entity3"" BaseType=""Bork.Entity3"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] DerivedEntityTypeKeyProperty(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Entity1Id"" />
    </Key>
    <Property Name=""Entity1Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity3"" BaseType=""Bork.Entity1"">
    <Key>
        <PropertyRef Name=""Entity3Id"" />
    </Key>
    <Property Name=""Entity3Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] SchemaElementShouldBeUnique(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Entity1"">
     <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity3"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityContainer Name=""Entity1"">
  </EntityContainer>
  <Function Name=""Entity1""><ReturnType Type=""Edm.Int32""/>
  </Function>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] ComplexTypePropertyWithNullableComplexType(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""DefaultNamespace"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""BaseType"">
     <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
  <ComplexType Name=""SimpleType"">
    <Property Name=""Type"" Type=""Self.BaseType"" />
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] EntityTypePropertyWithNullableComplexType(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""DefaultNamespace"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""ComplexType"">
     <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Property"" Type=""DefaultNamespace.ComplexType"" />
  </EntityType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] PropertyTypeShouldBeEdmSimpleTypeOrComplexType(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Complex1"">
    <Property Name=""Property"" Type=""Bork.Entity3"" Nullable=""false""/>
  </ComplexType>
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""Property"" Type=""Bork.Entity3"" Nullable=""false""/>
    <Property Name=""Name"" Type=""Bork.Complex1"" Nullable=""false""/>
    <Property Name=""Collection"" Type=""Collection(Bork.Entity3)"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity3"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        [CustomCsdlSchemaCompliantTest]
        public static XElement[] KeyAnnotationElementSupportInV40(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Hot"" Alias=""Fuzz"" xmlns:Bogus=""http://bogus.com/schema"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
      <Bogus:SchemaAnnotation Stuff=""Stuffed"" />
      <Bogus:AnotherSchemaAnnotation Fluff=""Fluffy"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] PropertyRefInteigrity(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Hot"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity2"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
  </EntityType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        [CustomConsistentValidationTest]
        public static IEdmModel PropertyRefShouldReferWithinDeclaringEntityType()
        {
            var model = new EdmModel();
            var type1 = new EdmEntityType("Foo", "EntityType1");
            var type2 = new EdmEntityType("Foo", "EntityType2");

            var id = new EdmStructuralProperty(type1, "Id", EdmCoreModel.Instance.GetInt16(false), null);

            type1.AddKeys(id);
            type2.AddKeys(id);

            model.AddElement(type1);
            model.AddElement(type2);

            return model;
        }

        [CustomCsdlSchemaCompliantTest]
        public static XElement[] ComplexTypeBaseTypeSupportInV11(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Complex2"" BaseType=""Bork.Complex1"">
    <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
  <ComplexType Name=""Complex1"">
    <Property Name=""End1"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""End2"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] CyclicComplexTypeBaseType(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Complex2"" BaseType=""Bork.Complex1"">
    <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
  <ComplexType Name=""Complex1"" BaseType=""Bork.Complex2"">
    <Property Name=""End1"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""End2"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>
  <ComplexType Name=""Complex3"" BaseType=""Bork.Complex3"">
    <Property Name=""Data2"" Type=""Edm.String"" />
  </ComplexType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] ComplexTypePropertyNameMustBeUnique(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Complex2"" BaseType=""Bork.Complex1"">
    <Property Name=""End2"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""End4"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>
  <ComplexType Name=""Complex3"" BaseType=""Bork.Complex1"">
    <Property Name=""End2"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>
  <ComplexType Name=""Complex1"">
    <Property Name=""End1"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""End1"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""End2"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""End3"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] ComplexTypePropertyNameShouldBeDifferentFromDeclaringType(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Complex2"" BaseType=""Bork.Complex1"">
    <Property Name=""Complex2"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""Complex1"" Type=""Int32"" Nullable=""false""/>
  </ComplexType>
  <ComplexType Name=""Complex1"">
    <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static IEdmModel AssociationNameSimpleIdentifier()
        {
            EdmModel model = new EdmModel();
            EdmEntityType t1 = new EdmEntityType("Bunk", "T1");
            EdmEntityType t2 = new EdmEntityType("Bunk", "T2");
            model.AddElement(t1);
            model.AddElement(t2);

            EdmStructuralProperty f11 = t1.AddStructuralProperty("F11", EdmCoreModel.Instance.GetInt16(false));
            EdmStructuralProperty f21 = t2.AddStructuralProperty("F21", EdmCoreModel.Instance.GetString(false));

            t1.AddKeys(f11);
            t2.AddKeys(f21);

            EdmNavigationProperty p101 = t1.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "P1", Target = t2, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "P1_Partner", TargetMultiplicity = EdmMultiplicity.Many });
            EdmNavigationProperty p102 = t1.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "P2", Target = t2, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
                new EdmNavigationPropertyInfo() { Name = "P2_Partner", TargetMultiplicity = EdmMultiplicity.Many });
            EdmNavigationProperty p201 = t2.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "P3", Target = t1, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
                new EdmNavigationPropertyInfo() { Name = "P3_Partner", TargetMultiplicity = EdmMultiplicity.Many });

            return model;
        }

        public static XElement[] ReferentialConstraintsCanExistBetweenKeyPropertiesInV12(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherId"" Type=""Int32"" />
    <NavigationProperty Name=""ToEntity2"" Type=""Collection(Bork.Entity2)"" Partner=""ToEntity1"" />
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherId"" Type=""Int32"" Nullable=""false""/> 
    <NavigationProperty Name=""ToEntity1"" Type=""Bork.Entity1"" Nullable=""false"" Partner=""ToEntity2"">
      <ReferentialConstraint Property=""OtherId"" ReferencedProperty=""Id"" />
    </NavigationProperty>
  </EntityType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] ReferentialConstraintsCanExistBetweenKeyPropertyAndPrimtiveTypeInV20(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Complex"">
     <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""OtherId"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherId"" Type=""Bork.Complex"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherId"" Type=""Int32"" Nullable=""false""/> 
    <Property Name=""OtherComplexId"" Type=""Bork.Complex"" Nullable=""false""/> 
    <NavigationProperty Name=""ToEntity1"" Type=""Bork.Entity1"" Nullable=""false"">
      <ReferentialConstraint Property=""OtherComplexId"" ReferencedProperty=""OtherId"" />
    </NavigationProperty>
  </EntityType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] PrincipalPropertyRefNameShouldBeUnique(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id1"" />
        <PropertyRef Name=""Id2"" />
    </Key>
    <Property Name=""Id1"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""Id2"" Type=""Int32"" Nullable=""false""/>
    <NavigationProperty Name=""ToEntity2"" Type=""Bork.Entity2"" Nullable=""false"">
      <ReferentialConstraint Property=""Id1"" ReferencedProperty=""Id"" />
      <ReferentialConstraint Property=""Id2"" ReferencedProperty=""Id"" />
    </NavigationProperty>
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherId1"" Type=""Int32"" Nullable=""false""/> 
    <Property Name=""OtherId2"" Type=""Int32"" Nullable=""false""/> 
    <NavigationProperty Name=""ToEntity1Again"" Type=""Bork.Entity1"" Nullable=""false"">
      <ReferentialConstraint Property=""OtherId1"" ReferencedProperty=""Id1"" />
      <ReferentialConstraint Property=""OtherId2"" ReferencedProperty=""Id1"" />
    </NavigationProperty>
  </EntityType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] PrincipalPropertyRefShouldCorrespondToDependentPropertyRef(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Complex"">
     <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id1"" />
        <PropertyRef Name=""Id2"" />
    </Key>
    <Property Name=""Id1"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""Id2"" Type=""String"" Nullable=""false""/>
    <NavigationProperty Name=""ToEntity2"" Type=""Collection(Bork.Entity2)"" Partner=""ToEntity1"" />
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherIdInt"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherIdString"" Type=""String"" Nullable=""false""/>
    <NavigationProperty Name=""ToEntity1"" Type=""Bork.Entity1"" Nullable=""false"" Partner=""ToEntity2"">
      <ReferentialConstraint Property=""OtherIdInt"" ReferencedProperty=""Id2"" />
      <ReferentialConstraint Property=""OtherIdString"" ReferencedProperty=""Id1"" />
    </NavigationProperty>
  </EntityType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] PrinciaplMustSpecifyAllTheKeyProperties(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id1"" />
        <PropertyRef Name=""Id2"" />
    </Key>
    <Property Name=""Id1"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""Id2"" Type=""Int32"" Nullable=""false""/>
    <NavigationProperty Name=""ToEntity2"" Type=""Collection(Bork.Entity2)"" Partner=""ToEntity1"" />
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherId1"" Type=""Int32"" Nullable=""false""/> 
    <Property Name=""OtherId2"" Type=""Int32"" Nullable=""false""/> 
    <NavigationProperty Name=""ToEntity1"" Type=""Bork.Entity1"" Nullable=""false"" Partner=""ToEntity2"">
      <ReferentialConstraint Property=""OtherId1"" ReferencedProperty=""Id1"" />
    </NavigationProperty>
  </EntityType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] DependentPropertyRefNameShouldBeUnique(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id1"" />
        <PropertyRef Name=""Id2"" />
    </Key>
    <Property Name=""Id1"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""Id2"" Type=""Int32"" Nullable=""false""/>
    <NavigationProperty Name=""ToEntity2"" Type=""Collection(Bork.Entity2)"" Partner=""ToEntity1"" />
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherId1"" Type=""Int32"" Nullable=""false""/> 
    <Property Name=""OtherId2"" Type=""Int32"" Nullable=""false""/> 
    <NavigationProperty Name=""ToEntity1"" Type=""Bork.Entity1"" Nullable=""false"" Partner=""ToEntity2"">
      <ReferentialConstraint Property=""OtherId1"" ReferencedProperty=""Id1"" />
      <ReferentialConstraint Property=""OtherId1"" ReferencedProperty=""Id2"" />
    </NavigationProperty>
  </EntityType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        [CustomCsdlSchemaCompliantTest]
        public static XElement[] EntityContainerAnnotationElementSupportInV40(EdmVersion edmVersion)
        {
            const string csdl =
 @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Grumble"" Alias=""Self"" xmlns:Bogus=""http://bogus.com/schema"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityContainer Name=""EC1"">
    <Bogus:SchemaAnnotation Stuff=""Stuffed1"" />
    <Bogus:SchemaAnnotation Stuff=""Stuffed2"" />
    <EntitySet Name=""Set1"" EntityType=""Grumble.Clod"" />
  </EntityContainer>
  <EntityType Name=""Clod"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""Bar"" Type=""Int32"" />
  </EntityType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static IEdmModel OperationImportNameSimpleIdentifier()
        {
            var model = new EdmModel();
            var container = new EdmEntityContainer("Default", "Container");

            var function1 = new EdmFunction("Default", "Function1", EdmCoreModel.Instance.GetInt32(false));
            var function2 = new EdmFunction("Default", "Function2", EdmCoreModel.Instance.GetInt32(false));
            var function3 = new EdmFunction("Default", "Function3", EdmCoreModel.Instance.GetInt32(false));
            var function4 = new EdmFunction("Default", "Function4", EdmCoreModel.Instance.GetInt32(false));
            container.AddFunctionImport("ValidName", function1);
            container.AddFunctionImport("_!@#$%^&*()", function2);
            container.AddFunctionImport("   ", function3);
            container.AddFunctionImport(string.Empty, function4);

            var action1 = new EdmAction("Default", "Action1", EdmCoreModel.Instance.GetInt32(false));
            var action2 = new EdmAction("Default", "Action2", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(action1);
            model.AddElement(action2);
            container.AddActionImport(new string('F', 480), action1);
            container.AddActionImport(new string('F', 481), action2);

            model.AddElement(container);
            model.AddElement(function1);
            model.AddElement(function2);
            model.AddElement(function3);
            model.AddElement(function4);
            return model;
        }

        public static XElement[] OperationImportReturnType(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <ComplexType Name=""ComplexType1"">
    <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Entity1"" EntityType=""Bork.Entity1"" />
    <EntitySet Name=""Entity2"" EntityType=""Bork.Entity2"" />
    <FunctionImport Name=""FunctionImport1"" EntitySet=""Entity1"" ReturnType=""Collection(Bork.Entity1)"">
    </FunctionImport>
    <FunctionImport Name=""FunctionImport2"" ReturnType=""Bork.Entity1"">
    </FunctionImport>
    <FunctionImport Name=""FunctionImport3"" ReturnType=""Bork.ComplexType1"">
    </FunctionImport>
    <FunctionImport Name=""FunctionImport4"" ReturnType=""Collection(Bork.ComplexType1)"">
    </FunctionImport>
    <FunctionImport Name=""FunctionImport5"" ReturnType=""Int32""/>
    <FunctionImport Name=""FunctionImport6"" ReturnType=""Collection(Int32)""/>
    <FunctionImport Name=""FunctionImport7"" ReturnType=""Bork.Association""/>
  </EntityContainer>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] FunctionImportReturnTypeV10(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <ComplexType Name=""ComplexType1"">
     <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Entity1"" EntityType=""Bork.Entity1"" />
    <EntitySet Name=""Entity2"" EntityType=""Bork.Entity2"" />
    <FunctionImport Name=""FunctionImport1"" EntitySet=""Entity1"" ReturnType=""Collection(Bork.Entity1)"">
    </FunctionImport>
    <FunctionImport Name=""FunctionImport2"" ReturnType=""Bork.Entity1"">
    </FunctionImport>
    <FunctionImport Name=""FunctionImport3"" ReturnType=""Bork.ComplexType1"">
    </FunctionImport>
    <FunctionImport Name=""FunctionImport4"" ReturnType=""Collection(Bork.ComplexType1)"">
    </FunctionImport>
    <FunctionImport Name=""FunctionImport5"" ReturnType=""Int32""/>
    <FunctionImport Name=""FunctionImport6"" ReturnType=""Collection(Int32)""/>
  </EntityContainer>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] OperationImportTypeIntegrity(EdmVersion edmVersion)
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
    </EntityType>
    <EntityContainer Name=""fooContainer"" >
        <EntitySet Name=""People"" EntityType=""foo.Person"" />
        <FunctionImport Name=""FunctionImport1"" ReturnType=""Collection(DoesNot.Exist)"">
            <Parameter Name=""parameter1"" Type=""String"" />
            <Parameter Name=""parameter2"" Type=""foo.DoesNotExist"" />
            <Parameter Name=""parameter3"" Type=""DoesNotExist.foo"" />
            <Parameter Name=""parameter4"" Type=""asdf.bar"" />
        </FunctionImport>
        <FunctionImport Name=""FunctionImport2"" ReturnType=""Collection(asdf.bar)"">
        </FunctionImport>
        <FunctionImport Name=""FunctionImport3"" ReturnType=""Collection(asdf.bar)"">
        </FunctionImport>
        <FunctionImport Name=""FunctionImport4"" ReturnType=""Collection(bad.type)"">
        </FunctionImport>
    </EntityContainer>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] OperationImportEntitySetMustNotBeSetWhenReturnTypeIsComplexOrSimple(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <ComplexType Name=""ComplexType1"">
    <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
    <Function Name=""Function1""><ReturnType Type=""Collection(Bork.Entity1)""/></Function>
    <Function Name=""Function2""><ReturnType Type=""Bork.Entity1""/></Function>
    <Function Name=""Function3""><ReturnType Type=""Bork.ComplexType1""/></Function>
    <Function Name=""Function4""><ReturnType Type=""Collection(Bork.ComplexType1)""/></Function>
    <Function Name=""Function5""><ReturnType Type=""Int32""/></Function>
    <Function Name=""Function6""><ReturnType Type=""Collection(Int32)""/></Function>
<EntityContainer Name=""Container"">
    <EntitySet Name=""Entity1"" EntityType=""Bork.Entity1""/>
    <EntitySet Name=""Entity2"" EntityType=""Bork.Entity2""/>
    <FunctionImport Name=""FunctionImport1"" EntitySet=""Entity1"" Function=""Bork.Function1""/>
    <FunctionImport Name=""FunctionImport2"" EntitySet=""Entity1"" Function=""Bork.Function2""/>
    <FunctionImport Name=""FunctionImport3"" EntitySet=""Entity1"" Function=""Bork.Function3""/>
    <FunctionImport Name=""FunctionImport4"" EntitySet=""Entity1"" Function=""Bork.Function4""/>
    <FunctionImport Name=""FunctionImport5"" EntitySet=""Entity1"" Function=""Bork.Function5""/>
    <FunctionImport Name=""FunctionImport6"" EntitySet=""Entity1"" Function=""Bork.Function6""/>
  </EntityContainer>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] ComposableFunctionImportCantBeSideEffecting(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""Entity1"" EntityType=""Bork.Entity1"" />
    <FunctionImport Name=""FunctionImport1"" EntitySet=""Entity1"" IsComposable=""true"" IsSideEffecting=""true"" ReturnType=""Collection(Bork.Entity1)"" />
  </EntityContainer>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] OperationImportIsComposableIsBindableIsSupportedInV4(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
    <Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Entity1"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
      </EntityType>
      <Function Name=""FunctionImport1""><ReturnType Type=""Collection(Bork.Entity1)"" /></Function>
      <Function Name=""FunctionImport2"" IsComposable=""true""><ReturnType Type=""Collection(Bork.Entity1)"" /></Function>
      <Action Name=""FunctionImport3"" IsBound=""true""><ReturnType Type=""Collection(Bork.Entity1)"">
        <Parameter Name=""p"" Type=""Collection(Bork.Entity1)"" />
      </Action>
      <EntityContainer Name=""Container"">
        <EntitySet Name=""Entity1"" EntityType=""Bork.Entity1"" />
      </EntityContainer>
    </Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] OperationImportParameterNameShouldBeUnique(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Function Name=""FunctionImport1""><ReturnType Type=""Collection(Int32)""/>
       <Parameter Name=""param1"" Type=""String"" />
       <Parameter Name=""param1"" Type=""String"" />
    </Function>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        [CustomCsdlSchemaCompliantTest]
        public static XElement[] OperationImportAnnotationElementSupportInV40(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns:Bogus=""http://bogus.com/schema"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Function Name=""Function1""><ReturnType Type=""Int32""/>
       <Bogus:SchemaAnnotation Stuff=""Stuffed"" />
       <Bogus:AnotherSchemaAnnotation Fluff=""Fluffy"" />
    </Function>
  <EntityContainer Name=""Container"">
    <FunctionImport Name=""FunctionImport1"" Function=""Bork.Function1"">
       <Bogus:SchemaAnnotation Stuff=""Stuffed"" />
       <Bogus:AnotherSchemaAnnotation Fluff=""Fluffy"" />
    </FunctionImport>
  </EntityContainer>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] OperationImportParameterTypeShouldBeSimpleOrComplex(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <ComplexType Name=""ComplexType1"">
    <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
  <ComplexType Name=""Complex"">
     <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
  <EntityContainer Name=""Container"">
    <FunctionImport Name=""FunctionImport1"" ReturnType=""Collection(Int32)"">
       <Parameter Name=""param1"" Type=""String"" />
       <Parameter Name=""param2"" Type=""Bork.ComplexType1"" />
       <Parameter Name=""param3"" Type=""Bork.Association"" />
    </FunctionImport>
    <FunctionImport Name=""FunctionImport2"" ReturnType=""Collection(Int32)"">
       <Parameter Name=""param1"" Type=""Foo.DoesNotExist"" />
       <Parameter Name=""param2"" Type=""Bork.Entity1"" />
    </FunctionImport>
  </EntityContainer>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] FunctionImportParamterShouldBeInAndOutAndInOut(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityContainer Name=""Container"">
    <FunctionImport Name=""FunctionImport1"" ReturnType=""Int32"">
       <Parameter Name=""param1"" Type=""String"" />
       <Parameter Name=""param2"" Type=""String"" />
       <Parameter Name=""param3"" Type=""String"" />
    </FunctionImport>
    <FunctionImport Name=""FunctionImport2"" ReturnType=""Int32"">
       <Parameter Name=""param4"" Type=""String"" />
       <Parameter Name=""param5"" Type=""String"" />
    </FunctionImport>
  </EntityContainer>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static IEdmModel EntitySetNameSimpleIdentifier()
        {
            var type = new EdmEntityType("Foo2", "Entity1");

            var container = new EdmEntityContainer("Foo", "EntityContainer");
            container.AddElement(new EdmEntitySet(container, "_!@#$%^&*()", type));
            container.AddElement(new EdmEntitySet(container, "", type));
            container.AddElement(new EdmEntitySet(container, "", type));
            container.AddElement(new EdmEntitySet(container, " ", type));
            container.AddElement(new EdmEntitySet(container, new string('E', 480), type));
            container.AddElement(new EdmEntitySet(container, new string('E', 481), type));

            type.AddKeys(type.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt16(false), null));
            var model = new EdmModel();
            model.AddElement(type);
            model.AddElement(container);

            return model;
        }

        [CustomCsdlSchemaCompliantTest]
        public static XElement[] AnnotationElementFullNameShouldBeUnique(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns:Bogus=""http://bogus.com/schema"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Function Name=""Function1""><ReturnType Type=""Int32"" /></Function>
  <EntityContainer Name=""Container"">
    <FunctionImport Name=""FunctionImport1"" Function=""Bork.Function1"">
       <Bogus:SchemaAnnotation Stuff=""Stuffed"" />
       <Bogus:SchemaAnnotation Stuff=""Fluffy"" />
       <Bogus:AnnotationScheme Stuff=""Fluffy"" />
    </FunctionImport>
  </EntityContainer>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static IEdmModel OperationNameSimpleIdentifier()
        {
            var model = new EdmModel();

           model.AddElement(new EdmAction("ValidNameSpace", "_!@#$%^&*()", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(new EdmAction("ValidNamespace", new string('F', 480), EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(new EdmAction(string.Empty, "ValidName", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(new EdmAction("_!@#$%^&*()", "ValidName", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(new EdmAction("ValidNamespace", " ", EdmCoreModel.Instance.GetInt32(false)));

            model.AddElement(new EdmAction(new string('F', 481), "ValidName", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(new EdmAction(new string('F', 512), "ValidName2", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(new EdmAction(new string('F', 513), "ValidName3", EdmCoreModel.Instance.GetInt32(false)));

            model.AddElement(new EdmAction(string.Empty, "ValidName", EdmCoreModel.Instance.GetInt64(false)));
            model.AddElement(new EdmAction("ValidNamespace", string.Empty, EdmCoreModel.Instance.GetInt32(false)));

            return model;
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEdmModel ModelWithCircularNavigationPartner()
        {
            EdmModel model = new EdmModel();

            var entity1 = new EdmEntityType("Foo", "Bar");
            var id1 = entity1.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt16(false), null);
            entity1.AddKeys(id1);

            var entity2 = new EdmEntityType("Foo", "Baz");
            var id2 = entity2.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt16(false), null);
            entity2.AddKeys(id2);

            var entity3 = new EdmEntityType("Foo", "Bas");
            var id3 = entity3.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt16(false), null);
            entity3.AddKeys(id3);

            StubEdmNavigationProperty entity1Navigation = new StubEdmNavigationProperty("ToBaz")
            {
                DeclaringType = entity1,
                Type = new EdmEntityTypeReference(entity2, true),
                Partner = new StubEdmNavigationProperty("ToBas") { DeclaringType = entity2, Type = new EdmEntityTypeReference(entity3, true) }
            };
            StubEdmNavigationProperty entity2Navigation = entity1Navigation.Partner as StubEdmNavigationProperty;
            entity2Navigation.Partner = entity1Navigation;
            StubEdmNavigationProperty entity3Navigation = new StubEdmNavigationProperty("ToBar")
            {
                DeclaringType = entity3,
                Type = new EdmEntityTypeReference(entity1, true),
            };
            entity3Navigation.Partner = entity1Navigation;
            entity1.AddProperty(entity1Navigation);
            entity2.AddProperty(entity2Navigation);
            entity3.AddProperty(entity3Navigation);

            model.AddElement(entity1);
            model.AddElement(entity2);
            model.AddElement(entity3);

            return model;
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEdmModel OperationParameterShouldBeInbound()
        {
            var model = new EdmModel();
            var operation = new EdmAction("namespace", "function1", EdmCoreModel.Instance.GetInt32(false));
            operation.AddParameter(new EdmOperationParameter(operation, "param1", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(operation);

            return model;
        }

        public static IEdmModel OperationParameterNameSimpleIdentifier()
        {
            var model = new EdmModel();

            var function = new EdmAction("namespace", "function1", EdmCoreModel.Instance.GetInt32(false));
            function.AddParameter(new EdmOperationParameter(function, "_!@#$%^&*()", EdmCoreModel.Instance.GetInt32(false)));
            function.AddParameter(new EdmOperationParameter(function, new string('P', 480), EdmCoreModel.Instance.GetInt32(false)));
            function.AddParameter(new EdmOperationParameter(function, new string('P', 481), EdmCoreModel.Instance.GetInt32(false)));
            function.AddParameter(new EdmOperationParameter(function, "param1", EdmCoreModel.Instance.GetInt32(false)));
            function.AddParameter(new EdmOperationParameter(function, "", EdmCoreModel.Instance.GetInt32(false)));
            function.AddParameter(new EdmOperationParameter(function, " ", EdmCoreModel.Instance.GetInt32(false)));
            function.AddParameter("", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(function);

            return model;
        }

        public static XElement[] OperationParamterValidTypeTest(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Complex"">
    <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <Function Name=""GetAge""><ReturnType Type=""Edm.Int32""/>
    <Parameter Name=""Person"" Type=""Bork.Association1"" />
  </Function>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] OperationReturnTypeValidTypeTest(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Complex"">
    <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <Function Name=""Function1""><ReturnType Type=""Edm.Int32""/>
  </Function>
  <Function Name=""Function2""><ReturnType Type=""Collection(Bork.Complex)""/></Function>
  <Function Name=""Function3""><ReturnType Type=""Bork.Association1""/></Function>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        [CustomCsdlSchemaCompliantTest]
        public static XElement[] CollectionTypeTypeRefSimpleTypeCanHaveFacets(EdmVersion edmVersion)
        {
            const string csdl = @"
<Schema Namespace='MyNamespace' xmlns='http://docs.oasis-open.org/odata/ns/edm'>
    <EntityType Name='MyEntityType'>
        <Key>
            <PropertyRef Name='Property1'/>
        </Key>
        <Property Name='Property1' Type='String' Nullable='false'/>
    </EntityType>
    <ComplexType Name='MyComplexType'>
        <Property Name='Property1' Type='String' />
    </ComplexType>
    <Function Name='MyFunction'><ReturnType Type='MyNamespace.MyEntityType'/>
        <Parameter Name='P2'>
            <CollectionType>
                <TypeRef Type='Edm.Int32' Nullable='false'/>
            </CollectionType>
        </Parameter>
        <Parameter Name='P3'>
            <CollectionType ElementType='Edm.Int32' Nullable='true'/>
        </Parameter>
        <Parameter Name='P4' Type='Collection(Binary)'/>
    </Function>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] TypeRefTypeIntegrity(EdmVersion edmVersion)
        {
            const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Complex"">
    <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <Function Name=""Function1""><ReturnType Type=""Edm.Int32""/>
        <Parameter Name='P3'>
            <CollectionType>
                <TypeRef Type='Int32'/>
            </CollectionType>
        </Parameter>
        <Parameter Name='P2'>
            <CollectionType>
                <TypeRef Type='Bork.Complex'/>
            </CollectionType>
        </Parameter>
        <Parameter Name='P1'>
            <CollectionType>
                <TypeRef Type='Bork.Entity1'/>
            </CollectionType>
        </Parameter>
        <Parameter Name='P4'>
            <CollectionType>
                <TypeRef Type='Bork.DoesNotExist'/>
            </CollectionType>
        </Parameter>
  </Function>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        [CustomCsdlSchemaCompliantTest]
        public static XElement[] TypeRefValidTypes(EdmVersion edmVersion)
        {
            const string csdl = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Complex"">
    <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <Function Name=""Function1""><ReturnType Type=""Edm.Int32""/>
        <Parameter Name='P2'>
            <CollectionType>
                <TypeRef Type='Bork.Entity1' Nullable='false'/>
            </CollectionType>
        </Parameter>
  </Function>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] InvalidKeyKeyDefinedInBaseClass(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
  <EntityType Name=""Entity2"" BaseType=""Bork.Entity1"">
    <Key>
        <PropertyRef Name=""Other""/>
    </Key>
    <Property Name=""Other"" Type=""Int32""  Nullable=""false""/> 
  </EntityType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        [CustomCsdlSchemaCompliantTest]
        public static XElement[] ValidDecimalTypePrecisionValue(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""Decimal1"" Type=""Decimal"" Nullable=""false"" Precision=""1""/>
    <Property Name=""Decimal2"" Type=""Decimal"" Nullable=""false"" Precision=""2""/>
    <Property Name=""Decimal3"" Type=""Decimal"" Nullable=""false"" Precision=""3""/>
    <Property Name=""Decimal4"" Type=""Decimal"" Nullable=""false"" Precision=""-1""/>
    <Property Name=""Decimal5"" Type=""Decimal"" Nullable=""false"" Precision=""0""/>
    <Property Name=""Decimal6"" Type=""Decimal"" Nullable=""false"" Precision=""4""/>
  </EntityType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        [CustomCsdlSchemaCompliantTest]
        public static XElement[] ValidDateTimeOffsetTypePrecisionValue(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""DateTimeOffset1"" Type=""DateTimeOffset"" Nullable=""false"" Precision=""1""/>
    <Property Name=""DateTimeOffset2"" Type=""DateTimeOffset"" Nullable=""false"" Precision=""2""/>
    <Property Name=""DateTimeOffset3"" Type=""DateTimeOffset"" Nullable=""false"" Precision=""3""/>
    <Property Name=""DateTimeOffset4"" Type=""DateTimeOffset"" Nullable=""false"" Precision=""-1""/>
    <Property Name=""DateTimeOffset5"" Type=""DateTimeOffset"" Nullable=""false"" Precision=""0""/>
    <Property Name=""DateTimeOffset6"" Type=""DateTimeOffset"" Nullable=""false"" Precision=""4""/>
  </EntityType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        [CustomCsdlSchemaCompliantTest]
        public static XElement[] ValidTimeTypePrecisionValue(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""Time1"" Type=""Duration"" Nullable=""false"" Precision=""1""/>
    <Property Name=""Time2"" Type=""Duration"" Nullable=""false"" Precision=""2""/>
    <Property Name=""Time3"" Type=""Duration"" Nullable=""false"" Precision=""3""/>
    <Property Name=""Time4"" Type=""Duration"" Nullable=""false"" Precision=""-1""/>
    <Property Name=""Time5"" Type=""Duration"" Nullable=""false"" Precision=""0""/>
    <Property Name=""Time6"" Type=""Duration"" Nullable=""false"" Precision=""4""/>
  </EntityType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] ValidDecimalTypeScaleValue(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""Decimal1"" Type=""Decimal"" Nullable=""false"" Precision=""1"" Scale=""0""/>
    <Property Name=""Decimal2"" Type=""Decimal"" Nullable=""false"" Precision=""1"" Scale=""1""/>
    <Property Name=""Decimal3"" Type=""Decimal"" Nullable=""false"" Precision=""1"" Scale=""2""/>
    <Property Name=""Decimal4"" Type=""Decimal"" Nullable=""false"" Precision=""1"" Scale=""-2""/>
  </EntityType>
</Schema>";

            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static IEdmModel ValidStringMaxLengthValue()
        {
            var complexType = new EdmComplexType("MyNamespace", "ComplexType");
            complexType.AddStructuralProperty("Prop1", EdmCoreModel.Instance.GetString(isUnbounded: false, maxLength: -1, isUnicode: true, isNullable: false));

            var model = new EdmModel();
            model.AddElement(complexType);
            return model;
        }

        public static IEdmModel MaxLengthAndUnbounded()
        {
            var complexType = new EdmComplexType("MyNamespace", "ComplexType");
            complexType.AddStructuralProperty("Prop1", new InconsistentStringTypeReference());

            var model = new EdmModel();
            model.AddElement(complexType);
            return model;
        }

        class InconsistentStringTypeReference : EdmPrimitiveTypeReference, IEdmStringTypeReference
        {
            public InconsistentStringTypeReference()
            : base(EdmCoreModel.Instance.GetString(true).PrimitiveDefinition(), false)
            { }

            public bool IsUnbounded
            {
                get { return true; }
            }

            public int? MaxLength
            {
                get { return 10; }
            }

            public bool? IsUnicode
            {
                get { return false; }
            }
        }

        public static XElement[] BinaryKeyTypeSupportInV40(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplexType"">
    <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Binary"" Nullable=""false""/>
  </EntityType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] BinaryKeyTypeWithNegativeMaxLength(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""MyComplexType"">
    <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Binary"" Nullable=""false"" MaxLength=""-1"" />
  </EntityType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        [CustomCsdlSchemaCompliantTest]
        public static XElement[] PropertyRefAnnotationElementSupportInV40(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Hot"" Alias=""Fuzz"" xmlns:Bogus=""http://bogus.com/schema"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"">
        <Bogus:SchemaAnnotation Stuff=""Stuffed"" />
        <Bogus:AnotherSchemaAnnotation Fluff=""Fluffy"" />
      </PropertyRef>
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
  </EntityType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static IEdmModel EdmFunctionNamespaceIsEmpty()
        {
            var model = new EdmModel();
            EdmComplexType complex = new EdmComplexType("", "ComplexType");
            complex.AddStructuralProperty("Data", EdmCoreModel.Instance.GetString(true));
            model.AddElement(complex);
            model.AddElement(new EdmFunction(string.Empty, "ValidName", EdmCoreModel.Instance.GetString(false)));
            return model;
        }

        public static XElement[] InvalidBinaryValues(EdmVersion edmVersion)
        {
            var csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""CollectionAtomic"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""ComplexTypeA"">
    <Property Name=""Binary1"" Type=""Binary"" DefaultValue=""0x1234""/>
    <Property Name=""Binary2"" Type=""Binary"" DefaultValue=""BAADF00d""/>
    <Property Name=""Binary3"" Type=""Binary"" DefaultValue=""OxDEADBEEFCAFE1""/>
    <Property Name=""Binary4"" Type=""Binary"" DefaultValue=""OxNotHexCharacters""/>
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static IEdmModel EntitySetInaccessibleEntityType()
        {
            var model = new EdmModel();
            var entity = new EdmEntityType("Foo", "Bar");
            var id = new EdmStructuralProperty(entity, "Id", EdmCoreModel.Instance.GetInt16(false), null);
            entity.AddKeys(id);
            var entityContainer = new EdmEntityContainer("Foo", "Container");
            entityContainer.AddEntitySet("Baz", entity);
            model.AddElement(entityContainer);

            return model;
        }

        public static IEdmModel SingletonInaccessibleEntityType()
        {
            var model = new EdmModel();
            var entity = new EdmEntityType("Foo", "Bar");
            var id = new EdmStructuralProperty(entity, "Id", EdmCoreModel.Instance.GetInt16(false), null);
            entity.AddKeys(id);
            var entityContainer = new EdmEntityContainer("Foo", "Container");
            entityContainer.AddSingleton("Baz", entity);
            model.AddElement(entityContainer);

            return model;
        }

        public static IEdmModel EntityBaseTypeInaccessibleEntityType()
        {
            var model = new EdmModel();
            var entity1 = new EdmEntityType("Foo", "Bar");
            var id = new EdmStructuralProperty(entity1, "Id", EdmCoreModel.Instance.GetInt16(false), null);
            entity1.AddKeys(id);

            var entity2 = new EdmEntityType("Foo", "Baz", entity1);

            model.AddElement(entity2);
            return model;
        }

        public static IEdmModel EntityReferenceInaccessibleEntityType()
        {
            var model = new EdmModel();
            var entity1 = new EdmEntityType("Foo", "Bar");
            var id = new EdmStructuralProperty(entity1, "Id", EdmCoreModel.Instance.GetInt16(false), null);
            entity1.AddKeys(id);

            var function = new EdmFunction("namespace", "function1", new EdmEntityReferenceTypeReference(new EdmEntityReferenceType(entity1), false));
            model.AddElement(function);
            return model;
        }

        public static IEdmModel TypeReferenceInaccessibleEntityType()
        {
            var model = new EdmModel();
            var entity1 = new EdmEntityType("Foo", "Bar");
            var id = new EdmStructuralProperty(entity1, "Id", EdmCoreModel.Instance.GetInt16(false), null);
            entity1.AddKeys(id);

            var function = new EdmFunction("namespace", "function1", new EdmEntityTypeReference(entity1, false));
            model.AddElement(function);
            return model;
        }

        public static IEdmModel DeclaringTypeIncorrect()
        {
            var model = new EdmModel();
            var entity1 = new EdmEntityType("Foo", "Bar");
            var id = new EdmStructuralProperty(entity1, "Id", EdmCoreModel.Instance.GetInt16(false), null);
            entity1.AddKeys(id);
            var entity2 = new StubEdmEntityType("Foo", "Baz");
            var id2 = new EdmStructuralProperty(entity1, "badProp", EdmCoreModel.Instance.GetInt16(false), null);
            entity2.SetKey(id2);
            entity2.Add(id2);

            model.AddElement(entity1);
            model.AddElement(entity2);

            return model;
        }

        public static IEdmModel ModelWithEnums()
        {
            EdmModel model = new EdmModel();
            EdmEnumType colors = new EdmEnumType("Foo", "Colors", EdmPrimitiveTypeKind.Int64, false);
            colors.AddMember("Red", new EdmEnumMemberValue(1));
            colors.AddMember("Blue", new EdmEnumMemberValue(2));
            colors.AddMember("Green", new EdmEnumMemberValue(3));
            colors.AddMember("Orange", new EdmEnumMemberValue(4));

            EdmEnumType gender = new EdmEnumType("Foo", "Gender", EdmPrimitiveTypeKind.Int64, true);
            gender.AddMember("Male", new EdmEnumMemberValue(1));
            gender.AddMember("Female", new EdmEnumMemberValue(2));

            model.AddElement(gender);
            model.AddElement(colors);
            return model;
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEdmModel ModelWithMismatchedEnumMemberTypes()
        {
            EdmModel model = new EdmModel();
            EdmEnumType colors = new EdmEnumType("Foo", "Colors", EdmPrimitiveTypeKind.Byte, false);
            colors.AddMember("Red", new EdmEnumMemberValue(1));
            colors.AddMember("Blue", new EdmEnumMemberValue(2));
            colors.AddMember("Green", new EdmEnumMemberValue(3));
            colors.AddMember("Orange", new EdmEnumMemberValue(4));

            EdmEnumType gender = new EdmEnumType("Foo", "Gender", EdmPrimitiveTypeKind.Byte, true);
            gender.AddMember("Male", new EdmEnumMemberValue(512L));
            gender.AddMember("Female", new EdmEnumMemberValue(512L));

            model.AddElement(gender);
            return model;
        }

        [CustomCsdlSchemaCompliantTest]
        [CustomConsistentValidationTest]
        public static IEdmModel ModelWithInconsistentNavigationPropertyPartner()
        {
            EdmModel model = new EdmModel();

            var entity1 = new EdmEntityType("Foo", "Bar");
            var id1 = entity1.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt16(false), null);
            entity1.AddKeys(id1);

            var entity2 = new EdmEntityType("Foo", "Baz");
            var id2 = entity2.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt16(false), null);
            entity2.AddKeys(id2);

            var entity3 = new EdmEntityType("Foo", "Bas");
            var id3 = entity3.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt16(false), null);
            entity3.AddKeys(id3);

            StubEdmNavigationProperty entity1Navigation = new StubEdmNavigationProperty("ToBaz")
            {
                DeclaringType = entity1,
                Type = new EdmEntityTypeReference(entity1, true),
                Partner = new StubEdmNavigationProperty("Parner") { DeclaringType = entity2, Type = new EdmEntityTypeReference(entity1, true) }
            };
            ((StubEdmNavigationProperty)entity1Navigation.Partner).Partner = entity1Navigation;
            entity1.AddProperty(entity1Navigation);
            entity2.AddProperty(entity1Navigation.Partner);
            entity3.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "ToBar", Target = entity1, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });

            model.AddElement(entity1);
            model.AddElement(entity2);
            model.AddElement(entity3);

            return model;
        }

        [CustomCsdlSchemaCompliantTest, CustomConsistentValidationTest]
        public static IEdmModel ModelWithInvalidDependentProperties()
        {
            EdmModel model = new EdmModel();

            var entity1 = new EdmEntityType("Foo", "Bar");
            var id1 = new EdmStructuralProperty(entity1, "Id", EdmCoreModel.Instance.GetInt16(false), null);
            entity1.AddKeys(id1);

            var entity2 = new EdmEntityType("Foo", "Baz");
            var id2 = new EdmStructuralProperty(entity2, "Id", EdmCoreModel.Instance.GetInt16(false), null);
            entity2.AddKeys(id2);

            EdmNavigationProperty entity1Navigation = entity1.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "ToBaz", Target = entity2, TargetMultiplicity = EdmMultiplicity.One, DependentProperties = new[] { id2 }, PrincipalProperties = entity2.Key() },
                new EdmNavigationPropertyInfo() { Name = "ToBar", TargetMultiplicity = EdmMultiplicity.Many }); 

            model.AddElement(entity1);

            return model;
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEdmModel ModelWithAssociationEndAsInaccessibleEntityType()
        {
            EdmModel model = new EdmModel();

            var entity1 = new EdmEntityType("Foo", "Bar");
            entity1.AddKeys(entity1.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt16(false), null));


            var entity2 = new EdmEntityType("Foo", "Baz");
            entity2.AddKeys(entity2.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt16(false), null));

            EdmNavigationProperty entity1Navigation = entity1.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo() { Name = "ToBaz", Target = entity2, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
                new EdmNavigationPropertyInfo() { Name = "ToBar", TargetMultiplicity = EdmMultiplicity.ZeroOrOne });

            model.AddElement(entity1);

            return model;
        }

        public static XElement[] IEdmReferentialConstraintInvalidMultiplicityPrincipleEndMany(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <NavigationProperty Name=""ToEntity2"" Type=""Collection(Bork.Entity2)"" Partner=""ToEntity1"" />
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherId"" Type=""Int32""/> 
    <NavigationProperty Name=""ToEntity1"" Type=""Collection(Bork.Entity1)"" Partner=""ToEntity2"">
      <ReferentialConstraint Property=""OtherId"" ReferencedProperty=""Id"" />
    </NavigationProperty>
  </EntityType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEdmModel ModelWithComplexTypeWithEntityBaseType()
        {
            EdmModel model = new EdmModel();

            var entity1 = new StubEdmEntityType("Foo", "Bar");
            var id1 = new EdmStructuralProperty(entity1, "Id", EdmCoreModel.Instance.GetInt16(false), null);
            entity1.Add(id1);
            entity1.SetKey(id1);

            StubEdmComplexType complex1 = new StubEdmComplexType("Foo", "Baz");
            complex1.Add(new EdmStructuralProperty(complex1,"Data",EdmCoreModel.Instance.GetString(true))); 

            complex1.BaseType = entity1;

            model.AddElement(entity1);
            model.AddElement(complex1);

            return model;
        }

        public static XElement[] ModelWithComplexTypeWithEntityBaseTypeCsdl(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Bar"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int16"" Nullable=""false"" />
  </EntityType>
  <ComplexType Name=""Baz"" BaseType=""Foo.Bar"">
     <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        [CustomCsdlSchemaCompliantTest, CustomConsistentValidationTest]
        public static IEdmModel ModelWithEntityTypeWithComplexBaseType()
        {
            EdmModel model = new EdmModel();

            var entity1 = new StubEdmEntityType("Foo", "Bar");
            var id1 = new EdmStructuralProperty(entity1, "Id", EdmCoreModel.Instance.GetInt16(false), null);
            entity1.Add(id1);
            entity1.SetKey(id1);

            StubEdmComplexType complex1 = new StubEdmComplexType("Foo", "Baz");
            complex1.Add(new EdmStructuralProperty(complex1, "Data", EdmCoreModel.Instance.GetString(true)));
            entity1.BaseType = complex1;

            model.AddElement(entity1);
            model.AddElement(complex1);

            return model;
        }

        public static XElement[] ModelWithEntityTypeWithComplexBaseTypeCsdl(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Bar"" BaseType=""Foo.Baz"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int16"" Nullable=""false"" />
  </EntityType>
  <ComplexType Name=""Baz"">
     <Property Name=""Data"" Type=""Edm.String"" />
  </ComplexType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        public static XElement[] InvalidMultiplicityFromRoleToPropertyNonNullableV1(EdmVersion edmVersion)
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Bork"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Entity1"">
    <Key>
        <PropertyRef Name=""Id"" />
        <PropertyRef Name=""OtherId"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherId"" Type=""Int32"" Nullable=""false""/> 
    <NavigationProperty Name=""ToEntity2"" Type=""Bork.Entity2"" Nullable=""false"" Partner=""ToEntity1"" />
  </EntityType>
  <EntityType Name=""Entity2"">
    <Key>
        <PropertyRef Name=""Id"" />
        <PropertyRef Name=""OtherId"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false""/>
    <Property Name=""OtherId"" Type=""Int32"" Nullable=""false""/> 
    <NavigationProperty Name=""ToEntity1"" Type=""Bork.Entity1"" Partner=""ToEntity2"">
      <ReferentialConstraint Property=""Id"" ReferencedProperty=""OtherId"" />
      <ReferentialConstraint Property=""OtherId"" ReferencedProperty=""OtherId"" />
    </NavigationProperty>
  </EntityType>
</Schema>";
            return FixUpWithEdmVersion(csdl, edmVersion);
        }

        [CustomConsistentValidationTest]
        public static IEdmModel ModelWithReferentialConstraintWhoseDependentPropertiesNotPartOfDependentEntity()
        {
            EdmModel model = new EdmModel();

            var entity1 = new EdmEntityType("Foo", "Bar");
            var id1 = new EdmStructuralProperty(entity1, "Id", EdmCoreModel.Instance.GetInt16(false), null);
            entity1.AddKeys(id1);

            var entity2 = new EdmEntityType("Foo", "Baz");
            var id2 = new EdmStructuralProperty(entity2, "Id", EdmCoreModel.Instance.GetInt16(false), null);
            entity2.AddKeys(id2);

            StubEdmNavigationProperty navProp1 = new StubEdmNavigationProperty("ToBaz");
            StubEdmNavigationProperty navProp2 = new StubEdmNavigationProperty("ToBar");
            navProp1.Type = new EdmEntityTypeReference(entity2, false);
            navProp1.DeclaringType = entity1;
            navProp1.ReferentialConstraint = EdmReferentialConstraint.Create(new[] { new EdmStructuralProperty(entity2, "Id1", EdmCoreModel.Instance.GetInt16(false), null) }, entity2.Key());
            navProp1.Partner = navProp2;

            navProp2.Type = new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(entity1, false)));
            navProp2.DeclaringType = entity2;
            navProp2.Partner = navProp1;

            entity1.AddProperty(navProp1);
            entity2.AddProperty(navProp2);

            model.AddElement(entity1);
            model.AddElement(entity2);

            return model;
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEdmModel ModelWithNavigationPropertyWithBadType()
        {
            EdmModel model = new EdmModel();

            var entity1 = new EdmEntityType("Foo", "Bar");
            var id1 = entity1.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt16(false), null);
            entity1.AddKeys(id1);

            var entity2 = new EdmEntityType("Foo", "Baz");
            var id2 = entity2.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt16(false), null);
            entity2.AddKeys(id2);

            var complexType = new EdmComplexType("Foo", "Bas");
            complexType.AddStructuralProperty("Data", EdmCoreModel.Instance.GetString(true));

            StubEdmNavigationProperty navProp = new StubEdmNavigationProperty("NavProp")
            {
                DeclaringType = entity2,
                Type = EdmCoreModel.Instance.GetSingle(false),
                Partner = new StubEdmNavigationProperty("Partner") { DeclaringType = entity1, Type = new EdmEntityTypeReference(entity2, false) }
            };
            ((StubEdmNavigationProperty)navProp.Partner).Partner = navProp;
            entity2.AddProperty(navProp);

            EdmNavigationProperty navProp2 = entity2.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "CollectionNavProp", Target = entity1, TargetMultiplicity = EdmMultiplicity.Many });

            StubEdmNavigationProperty navProp3 = new StubEdmNavigationProperty("ComplexNavProp")
            {
                DeclaringType = entity2,
                Type = new EdmComplexTypeReference(complexType, false),
                Partner = new StubEdmNavigationProperty("Partner") { DeclaringType = entity1, Type = new EdmEntityTypeReference(entity2, false) }
            };
            ((StubEdmNavigationProperty)navProp3.Partner).Partner = navProp3;
            entity2.AddProperty(navProp3);

            model.AddElement(entity1);
            model.AddElement(entity2);
            model.AddElement(complexType);

            return model;
        }

        [CustomCsdlSchemaCompliantTest, CustomConsistentValidationTest]
        public static IEdmModel ModelWithNavigationPropertyWithContainsTargetBadMultiplicity()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""ParentId"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
        <NavigationProperty Name=""Orders"" Type=""Collection(foo.Order)"" Partner=""Customer"" />
        <NavigationProperty Name=""Parent"" Type=""foo.Person"" Nullable=""false"" Partner=""Children"" />
        <!-- this nav prop represents a recursive containment and its source must be zero-or-one, however according to the model it is one and it's target must be optional. -->
        <NavigationProperty Name=""Children"" ContainsTarget=""true"" Type=""foo.Person"" Nullable=""false"" Partner=""Parent"">
          <ReferentialConstraint Property=""Id"" ReferencedProperty=""Id"" />
        </NavigationProperty>
    </EntityType>
    <EntityType Name=""Order"">
      <Key>
        <PropertyRef Name=""ID"" />
      </Key>
      <Property Name=""ID"" Type=""Edm.String"" Nullable=""false"" />
      <Property Name=""PersonId"" Type=""Int32"" Nullable=""false"" />
      <!-- this nav prop has source that must be one -->
      <NavigationProperty Name=""Customer"" Type=""foo.Person"" Nullable=""false"" Partner=""Orders"" ContainsTarget=""true"">
        <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
      </NavigationProperty>
    </EntityType>
    <EntityContainer Name=""fooContainer"" >
        <EntitySet Name=""People"" EntityType=""foo.Person"">
          <NavigationPropertyBinding Path=""Orders"" Target=""Orders"" />
          <NavigationPropertyBinding Path=""Parent"" Target=""People"" />
          <NavigationPropertyBinding Path=""Children"" Target=""People"" />
        </EntitySet>
        <EntitySet Name=""Orders"" EntityType=""foo.Order"">
          <NavigationPropertyBinding Path=""Customer"" Target=""People"" />
        </EntitySet>
    </EntityContainer>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No parsing errors");

            return model;
        }

        [CustomCsdlSchemaCompliantTest, CustomConsistentValidationTest]
        public static IEdmModel ModelWithNavigationPropertyWithRecursiveContainsNonSameEntitySet()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""ParentId"" Type=""Int32"" Nullable=""true"" />
        <Property Name=""ManagerId"" Type=""Int32"" Nullable=""true"" />

        <NavigationProperty Name=""Employees"" ContainsTarget=""true"" Type=""Collection(foo.Person)"" Partner=""Manager"" />
        <NavigationProperty Name=""Manager"" Type=""foo.Person"" Partner=""Employees"" Nullable=""true"" >
          <ReferentialConstraint Property=""ManagerId"" ReferencedProperty=""Id"" />
        </NavigationProperty>
    </EntityType>
    <EntityContainer Name=""fooContainer"" >
        <EntitySet Name=""Set1"" EntityType=""foo.Person"">
          <NavigationPropertyBinding Path=""Employees"" Target=""Set2"" />
          <NavigationPropertyBinding Path=""Manager"" Target=""Set2"" />
        </EntitySet>
        <EntitySet Name=""Set2"" EntityType=""foo.Person"">
          <NavigationPropertyBinding Path=""Employees"" Target=""Set1"" />
          <NavigationPropertyBinding Path=""Manager"" Target=""Set1"" />
        </EntitySet>
    </EntityContainer>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No parsing errors");

            return model;
        }

        public static IEdmModel ModelWithNavigationPropertyWithContainsTarget()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
        <NavigationProperty Name=""Orders"" Type=""Collection(foo.Order)"" Partner=""Customer"" ContainsTarget=""true"" />
    </EntityType>
    <EntityType Name=""Order"">
      <Key>
        <PropertyRef Name=""ID"" />
        <PropertyRef Name=""PersonId"" />
      </Key>
      <Property Name=""ID"" Type=""Edm.String"" Nullable=""false"" />
      <Property Name=""PersonId"" Type=""Int32"" Nullable=""false"" />
      <NavigationProperty Name=""Customer"" Type=""foo.Person"" Nullable=""false"" Partner=""Orders"">
        <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
      </NavigationProperty>
    </EntityType>
    <EntityContainer Name=""fooContainer"" >
        <EntitySet Name=""People"" EntityType=""foo.Person"">
          <NavigationPropertyBinding Path=""Orders"" Target=""Orders"" />
        </EntitySet>
        <EntitySet Name=""Orders"" EntityType=""foo.Order"">
          <NavigationPropertyBinding Path=""Customer"" Target=""People"" />
        </EntitySet>
    </EntityContainer>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);

            return parsed ? model : null;
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEdmModel ModelWithInvalidOperationReturnType()
        {
            EdmModel model = new EdmModel();

            IEdmTypeReference badTypeRef = new StubEdmTypeReference { Definition = new StubEdmType(), IsNullable = false };

            EdmOperation func1 = new EdmFunction("Foo", "Bar", badTypeRef);

            model.AddElement(func1);

            return model;
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEdmModel ModelWithInvalidOperationParameterType()
        {
            EdmModel model = new EdmModel();

            EdmOperation a = new EdmAction("Foo", "Bar", EdmCoreModel.Instance.GetString(false));

            IEdmTypeReference badTypeRef = new StubEdmTypeReference { Definition = new StubEdmType(), IsNullable = false };
            EdmOperationParameter par1 = new EdmOperationParameter(a, "Parameter", badTypeRef);
            a.AddParameter(par1);

            model.AddElement(a);

            return model;
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEdmModel ModelWithInvalidFunctionReturnType()
        {
            EdmModel model = new EdmModel();
            
            IEdmTypeReference badTypeRef = new StubEdmTypeReference { Definition = new StubEdmType(), IsNullable = false };

            EdmFunction func1 = new EdmFunction("Default", "Foo", badTypeRef);

            EdmOperationParameter par1 = new EdmOperationParameter(func1, "Parameter", badTypeRef);
            func1.AddParameter(par1);

            model.AddElement(func1);

            return model;
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEdmModel ModelWithInvalidActionImportEntitySet()
        {
            EdmModel model = new EdmModel();

            var entity = new EdmEntityType("Foo", "Bar");
            var id1 = new EdmStructuralProperty(entity, "Id", EdmCoreModel.Instance.GetInt16(false), null);
            entity.AddKeys(id1);

            EdmEntityContainer container = new EdmEntityContainer("Default", "Container");

            EdmAction action = new EdmAction("Default", "Foo", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(entity, false))));
            model.AddElement(action);
            EdmOperationImport actionImport1 = new EdmActionImport(container, "Foo", action, new EdmIntegerConstant(EdmCoreModel.Instance.GetInt32(false), 123));

            container.AddElement(actionImport1);
            model.AddElement(container);
            model.AddElement(entity);

            return model;
        }

        [CustomCsdlSchemaCompliantTest, CustomConsistentValidationTest]
        public static IEdmModel ModelWithInvalidOperationImportEntitySet2()
        {
            const string csdl =
@"<Schema Namespace=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
        <NavigationProperty Name=""Orders"" Type=""Collection(foo.Order)"" Partner=""Customer"" />
    </EntityType>
    <EntityType Name=""Person2"" BaseType=""foo.Person"">
        <NavigationProperty Name=""Person2Orders"" Type=""Collection(foo.Order)"" />
    </EntityType>
    <EntityType Name=""Person3"" BaseType=""foo.Person2"">
        <NavigationProperty Name=""Person3Orders"" Type=""Collection(foo.Order)"" />
    </EntityType>
    <EntityType Name=""Order"">
      <Key>
        <PropertyRef Name=""ID"" />
      </Key>
      <Property Name=""ID"" Type=""Edm.String"" Nullable=""false"" />
      <Property Name=""PersonId"" Type=""Int32"" Nullable=""false"" />
      <Property Name=""Person2Id"" Type=""Int32"" Nullable=""false"" />
      <Property Name=""Person3Id"" Type=""Int32"" Nullable=""false"" />
      <NavigationProperty Name=""Customer"" Type=""foo.Person"" Nullable=""false"" Partner=""Orders"">
        <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
      </NavigationProperty>
    </EntityType>
    <ComplexType Name=""PersonCT"">
        <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
        <Property Name=""Birthday"" Type=""DateTimeOffset"" />
    </ComplexType>
    <EntityContainer Name=""fooContainer"" >
        <EntitySet Name=""People"" EntityType=""foo.Person"">
          <NavigationPropertyBinding Path=""Orders"" Target=""Orders""/>
          <NavigationPropertyBinding Path=""foo.Person2/Person2Orders"" Target=""Orders2""/>
          <NavigationPropertyBinding Path=""foo.Person3/Person3Orders"" Target=""Orders3""/>
        </EntitySet>
        <EntitySet Name=""Orders"" EntityType=""foo.Order"">
          <NavigationPropertyBinding Path=""Customer"" Target=""People""/>
        </EntitySet>
        <EntitySet Name=""Orders2"" EntityType=""foo.Order"">
          <NavigationPropertyBinding Path=""Customer"" Target=""People""/>
        </EntitySet>
        <EntitySet Name=""Orders3"" EntityType=""foo.Order"">
          <NavigationPropertyBinding Path=""Customer"" Target=""People""/>
        </EntitySet>

        <FunctionImport Name=""EntitySetPathCastToABadType"" Function=""foo.EntitySetPathCastToABadType"" EntitySet=""Purchases/Customer/foo.PPP/Person3Orders""/>

        <FunctionImport Name=""EntitySetPathCastToANonNavProp"" Function=""foo.EntitySetPathCastToANonNavProp"" EntitySet=""Purchases/Customer/foo.Person3/Birthday""/>

        <FunctionImport Name=""EntitySetPathCastToANonNavProp2"" Function=""foo.EntitySetPathCastToANonNavProp2"" EntitySet=""Purchases/ID/foo.Person3/Orders""/>

        <FunctionImport Name=""EntitySetPathCastToABadProp"" Function=""foo.EntitySetPathCastToABadProp"" EntitySet=""Purchases/QOO/foo.Person3/Orders""/>

        <FunctionImport Name=""EntitySetPathCastToABadProp2"" Function=""foo.EntitySetPathCastToABadProp2"" EntitySet=""Purchases/Customer/foo.Person3/QOO""/>

    </EntityContainer>
    <Function Name=""EntitySetPathStartsWithBadParameter"" IsComposable=""true"" IsBound=""true"" EntitySetPath=""qwerty""><ReturnType Type=""Collection(foo.Order)""/>
        <Parameter Name=""Purchases"" Type=""Collection(foo.Order)"" />
    </Function>

    <Function Name=""EntitySetPathStartsWithEmptySegment"" IsComposable=""true"" IsBound=""true"" EntitySetPath=""/Purchases""><ReturnType Type=""Collection(foo.Order)""/>
        <Parameter Name=""Purchases"" Type=""Collection(foo.Order)"" />
    </Function>

    <Function Name=""EntitySetPathEndsWithEmptySegment"" IsComposable=""true"" IsBound=""true"" EntitySetPath=""Purchases/""><ReturnType Type=""Collection(foo.Order)""/>
        <Parameter Name=""Purchases"" Type=""Collection(foo.Order)"" />
    </Function>

    <Function Name=""EntitySetPathEndsWithCast"" IsComposable=""true"" IsBound=""true"" EntitySetPath=""Purchases/Customer/foo.Person2""><ReturnType Type=""Collection(foo.Order)""/>
        <Parameter Name=""Purchases"" Type=""Collection(foo.Order)"" />
    </Function>

    <!-- the wrong type exists, but it does not derive from the type of the previous segment -->
    <Function Name=""EntitySetPathCastToAWrongType"" IsComposable=""true"" IsBound=""true"" EntitySetPath=""Purchases/Customer/foo.Order/Customer""><ReturnType Type=""Collection(foo.Order)""/>
        <Parameter Name=""Purchases"" Type=""Collection(foo.Order)"" />
    </Function>

    <Function Name=""EntitySetPathCastToABadType"" IsComposable=""true"" ><ReturnType Type=""Collection(foo.Order)""/>
        <Parameter Name=""Purchases"" Type=""Collection(foo.Order)"" />
    </Function>

    <Function Name=""EntitySetPathCastToANonNavProp"" IsComposable=""true"" ><ReturnType Type=""Collection(foo.Order)""/>
        <Parameter Name=""Purchases"" Type=""Collection(foo.Order)"" />
    </Function>

    <Function Name=""EntitySetPathCastToANonNavProp2"" IsComposable=""true"" ><ReturnType Type=""Collection(foo.Order)""/>
        <Parameter Name=""Purchases"" Type=""Collection(foo.Order)"" />
    </Function>

    <Function Name=""EntitySetPathCastToABadProp"" IsComposable=""true"" ><ReturnType Type=""Collection(foo.Order)""/>
        <Parameter Name=""Purchases"" Type=""Collection(foo.Order)"" />
    </Function>

    <Function Name=""EntitySetPathCastToABadProp2"" IsComposable=""true"" ><ReturnType Type=""Collection(foo.Order)""/>
        <Parameter Name=""Purchases"" Type=""Collection(foo.Order)"" />
    </Function>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            bool parsed = SchemaReader.TryParse(new XmlReader[] { XmlReader.Create(new StringReader(csdl)) }, out model, out errors);
            Assert.IsTrue(parsed, "parsed");
            Assert.IsTrue(errors.Count() == 0, "No parsing errors");

            return model;
        }

        [CustomCsdlSchemaCompliantTest]
        public static IEnumerable<XElement> ModelWithEnumTerm()
        {
            const string csdl =
@"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""ElementCollectionAtomic"" Alias=""Self"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Age"" Type=""Edm.Int64"" />
    <EnumType Name=""Popularity"" UnderlyingType=""Edm.Int32"">
        <Annotation Term=""ElementCollectionAtomic.Age"" Int=""22"" />
    </EnumType>
</Schema>";

            return new XElement[] { XElement.Parse(csdl) };
        }

        public static IEdmModel ModelWithNegativeMaxLengthBinaryType()
        {
            EdmModel model = new EdmModel();

            EdmComplexType complex = new EdmComplexType("Foo", "Bar");
            new EdmStructuralProperty(complex, "Baz", EdmCoreModel.Instance.GetBinary(false, -1, false), null);
            model.AddElement(complex);
            return model;
        }

        public static IEdmModel ModelWithBadElementAnnotation()
        {
            EdmModel model = new EdmModel();
            EdmEntityType customer = new EdmEntityType("Westwind", "Customer");
            EdmStructuralProperty customerID = customer.AddStructuralProperty("IDC", EdmCoreModel.Instance.GetInt32(false));
            customer.AddKeys(customerID);
            model.AddElement(customer);

            EdmEntityContainer container = new EdmEntityContainer("Westwind", "Gunk");
            model.AddElement(container);
            EdmEntitySet customers = new EdmEntitySet(container, "Customers", customer);
            container.AddElement(customers);

            XElement contacts =
                new XElement("{http://msn}Contacts",
                    new XElement("{http://msn}Contact",
                        new XElement("{http://msn}Name", "Patrick Hines"),
                        new XElement("{http://msn}Phone", "206-555-0144"),
                        new XElement("{http://msn}Address",
                            new XElement("{http://msn}Street1", "123 Main St"),
                            new XElement("{http://msn}City", "Mercer Island"),
                            new XElement("{http://msn}State", "WA"),
                            new XElement("{http://msn}Postal", "68042")
                        )
                    )
                );

            var annotation = new EdmStringConstant(EdmCoreModel.Instance.GetString(false), contacts.ToString());
            annotation.SetIsSerializedAsElement(model, true);
            model.SetAnnotationValue(customer, "http://other", "Contacts", annotation);
            return model;
        }

        public static IEdmModel ReferenceBasicTestMainModel()
        {
            var model = new EdmModel();

            var customerType = new EdmEntityType("NS1", "Customer");
            var customerId = customerType.AddStructuralProperty("CustomerID", EdmCoreModel.Instance.GetString(false));
            customerType.AddKeys(customerId);
            model.AddElement(customerType);

            return model;
        }

        public static IEdmModel ReferenceBasicTestReferencedModel()
        {
            var model = new EdmModel();
            var title = new EdmTerm("NS1", "Title", EdmCoreModel.Instance.GetString(true));
            model.AddElement(title);

            var personType = new EdmEntityType("NS1", "Person");
            var personId = personType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetString(false));
            personType.AddKeys(personId);
            model.AddElement(personType);

            var regionType = new EdmComplexType("NS1", "Region");
            regionType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            model.AddElement(regionType);

            return model;
        }

        public static IEdmModel BidirectionalContainmentModel()
        {
            var model = new EdmModel();
            var person = new EdmEntityType("NS", "Person");
            var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            var pet = new EdmEntityType("NS", "Pet");
            var petId = new EdmStructuralProperty(pet, "Id", EdmCoreModel.Instance.GetInt32(false));
            pet.AddProperty(petId);
            pet.AddKeys(petId);
            model.AddElement(pet);

            var petToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true, OnDelete = EdmOnDeleteAction.None },
                new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true, OnDelete = EdmOnDeleteAction.None });
            pet.AddProperty(petToPerson);
            person.AddProperty(petToPerson.Partner);

            return model;
        }

        public interface ITestEdmCheckableElement : IEdmElement
        {

        }

        public class TestEdmCheckableElement : ITestEdmCheckableElement, IEdmCheckable
        {
            protected bool IsBad { get; set; }

            public TestEdmCheckableElement(bool isBad) { this.IsBad = isBad; }

            public TestEdmCheckableElement() : this(false) { }

            public IEnumerable<IEdmDirectValueAnnotation> AttachedAnnotations
            {
                get { return Enumerable.Empty<IEdmDirectValueAnnotation>(); }
            }

            public void SetAnnotation(string namespaceName, string localName, object value)
            {
                throw new NotImplementedException();
            }

            public object GetAnnotation(string namespaceName, string localName)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<EdmError> Errors
            {
                get
                {
                    if (this.IsBad)
                    {
                        return new EdmError[] 
                        { 
                            new EdmError(null, EdmErrorCode.AlreadyDefined, null) 
                        };
                    }
                    return new EdmError[] { };
                }
            }
        }

        public interface ITestEdmCheckableSpecialChildElement : ITestEdmCheckableElement
        {

        }

        public class TestEdmCheckableSpecialChildElement : TestEdmCheckableElement, ITestEdmCheckableSpecialChildElement, IEdmCheckable
        {
            public TestEdmCheckableSpecialChildElement(bool isBad) : base(isBad) { }

            public TestEdmCheckableSpecialChildElement() : base(false) { }

            IEnumerable<EdmError> IEdmCheckable.Errors
            {
                get
                {
                    if (this.IsBad)
                    {
                        return new EdmError[] 
                        { 
                            new EdmError(null, EdmErrorCode.AlreadyDefined, null) 
                        };
                    }
                    return new EdmError[] { };
                }
            }
        }

        public interface ITestEdmCheckableChildElement : ITestEdmCheckableElement
        {
            ITestEdmAssociatedCheckableElement AssociatedProperty
            {
                get;
                set;
            }
        }

        public class TestEdmCheckableChildElement : TestEdmCheckableElement, ITestEdmCheckableChildElement
        {
            public TestEdmCheckableChildElement(bool isBad) : base(isBad) { }

            public TestEdmCheckableChildElement() : base(false) { }

            public ITestEdmAssociatedCheckableElement AssociatedProperty
            {
                get;
                set;
            }
        }

        public interface ITestEdmNonCheckableChildElement : ITestEdmCheckableElement
        {

        }

        public class TestEdmNonCheckableChildElement : TestEdmCheckableElement, ITestEdmNonCheckableChildElement
        {
            public TestEdmNonCheckableChildElement(bool isBad) : base(isBad) { }
            public TestEdmNonCheckableChildElement() : base(false) { }
        }

        public class TestEdmAssociatedElement : IEdmElement
        {
            protected bool IsBad { get; set; }

            public TestEdmAssociatedElement(bool isBad)
            {
                this.IsBad = isBad;
            }

            public TestEdmAssociatedElement() : this(false) { }

            public IEnumerable<Microsoft.OData.Edm.Vocabularies.IEdmDirectValueAnnotation> AttachedAnnotations
            {
                get { return Enumerable.Empty<IEdmDirectValueAnnotation>(); }
            }

            public void SetAnnotation(string namespaceName, string localName, object value)
            {
                throw new NotImplementedException();
            }

            public object GetAnnotation(string namespaceName, string localName)
            {
                throw new NotImplementedException();
            }
        }

        public interface ITestEdmAssociatedCheckableElement : IEdmElement
        {

        }

        public class TestEdmAssociatedCheckableElement : TestEdmAssociatedElement, ITestEdmAssociatedCheckableElement, IEdmCheckable
        {
            public TestEdmAssociatedCheckableElement(bool isBad) : base(isBad) { }

            public TestEdmAssociatedCheckableElement() : base() { }

            IEnumerable<EdmError> IEdmCheckable.Errors
            {
                get
                {
                    if (this.IsBad)
                    {
                        return new EdmError[] 
                        { 
                            new EdmError(null, EdmErrorCode.AlreadyDefined, null) 
                        };
                    }
                    return new EdmError[] { };
                }
            }
        }

        public class TestEdmAssociatedCheckableNullErrorElement : TestEdmAssociatedElement, IEdmCheckable, IEdmEnumType
        {
            public IEnumerable<EdmError> Errors
            {
                get { return Enumerable.Empty<EdmError>(); }
            }

            public EdmSchemaElementKind SchemaElementKind
            {
                get { return EdmSchemaElementKind.TypeDefinition; }
            }

            public string Namespace
            {
                get { return "MyNamespace"; }
            }

            public string Name
            {
                get { return "MyName"; }
            }

            public EdmTypeKind TypeKind
            {
                get { return EdmTypeKind.Enum; }
            }

            public IEdmPrimitiveType UnderlyingType
            {
                get { return EdmCoreModel.Instance.GetInt32(false).PrimitiveDefinition(); }
            }

            public IEnumerable<IEdmEnumMember> Members
            {
                get { return Enumerable.Empty<IEdmEnumMember>(); }
            }

            public bool IsFlags
            {
                get { throw new NotImplementedException(); }
            }
        }
    }

    public static class TestExtension
    {
        public static bool IsBad(this ValidationTestModelBuilder.ITestEdmCheckableChildElement element)
        {
            if (element == null)
            {
                return true;
            }
            var results = new List<bool>();
            results.Add(((IEdmElement)element).IsBad());
            results.Add(((IEdmElement)element.AssociatedProperty).IsBad());
            return results.Any(n => n);
        }
    }
}

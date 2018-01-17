//---------------------------------------------------------------------
// <copyright file="NavigationTestModelBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData.Edm;

    public static class NavigationTestModelBuilder
    {
        public static IEnumerable<XElement> NavigationOneKeywithOneNonKeyPrincipalPropertyRefCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""PersonId"" />
            <PropertyRef Name=""PersonName"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> NavigationTwoNonKeyPrincipalPropertyRefCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""AdditionalId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""PersonId"" />
            <PropertyRef Name=""PersonName"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""AdditionalId"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> NavigationOneNonKeyPrincipalPropertyRefCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""AdditionalId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""PersonName"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> NavigationDuplicatePrincipalPropertyRefCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""PersonId"" />
            <PropertyRef Name=""PersonName"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId2"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonId2"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> NavigationDuplicateReferentialConstraintCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""PersonId"" />
            <PropertyRef Name=""PersonName"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId2"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> NavigationDuplicateDependentPropertyRefCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id1"" />
            <PropertyRef Name=""Id2"" />
        </Key>
        <Property Name=""Id1"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Id2"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""PersonId"" />
            <PropertyRef Name=""PersonName"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id1"" />
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id2"" />
        </NavigationProperty>
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> NavigationPrincipalPropertyRefDoesNotCorrespondToDependentPropertyRefCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""PersonId"" />
            <PropertyRef Name=""PersonName"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Name"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Id"" />
        </NavigationProperty>
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> NavigationSinglePrincipalWithNotNullableDependentCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> NavigationSinglePrincipalWithNotNullableKeyDependentCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""PersonId"" />
            <PropertyRef Name=""PersonName"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> NavigationSinglePrincipalWithMixNullableDependentCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""true"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> NavigationSinglePrincipalWithAllNullableDependentCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""true"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""true"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> NavigationMultiplePrincipalWithNotNullableDependentCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""Collection(DefaultNamespace.Person)"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> NavigationMultiplePrincipalWithMixNullableDependentCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""true"" />
        <NavigationProperty Name=""ToPerson"" Type=""Collection(DefaultNamespace.Person)"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> NavigationMultiplePrincipalWithAllNullableDependentCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""true"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""true"" />
        <NavigationProperty Name=""ToPerson"" Type=""Collection(DefaultNamespace.Person)"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>");
        }
        
        public static IEnumerable<XElement> NavigationZeroOnePrincipalWithNotNullableDependentCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> NavigationZeroOnePrincipalWithNotNullableKeyDependentCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""PersonId"" />
            <PropertyRef Name=""PersonName"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> NavigationZeroOnePrincipalWithMixNullableDependentCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""true"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> NavigationZeroOnePrincipalWithAllNullableDependentCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""true"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""true"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> NavigationZeroOnePrincipalWithAllNullableKeyDependentCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""Name"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />
        <NavigationProperty Name=""ToBadge"" Type=""Collection(DefaultNamespace.Badge)"" Partner=""ToPerson"" />
    </EntityType>
    <EntityType Name=""Badge"">
        <Key>
            <PropertyRef Name=""Id"" />
            <PropertyRef Name=""PersonId"" />
            <PropertyRef Name=""PersonName"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""true"" />
        <Property Name=""PersonName"" Type=""Edm.String"" Nullable=""true"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Partner=""ToBadge"">
          <ReferentialConstraint Property=""PersonId"" ReferencedProperty=""Id"" />
          <ReferentialConstraint Property=""PersonName"" ReferencedProperty=""Name"" />
        </NavigationProperty>
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> NonDirectRecursiveContainment()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""A"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""ToB"" Type=""DefaultNamespace.B"" Nullable=""false"" ContainsTarget=""True""/>
    </EntityType>
    <EntityType Name=""B"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""C"" BaseType=""DefaultNamespace.A"">
    </EntityType>
    <EntityType Name=""D"" BaseType=""DefaultNamespace.B"">
        <NavigationProperty Name=""ToC"" Type=""DefaultNamespace.C"" Nullable=""false"" ContainsTarget=""True"" />
    </EntityType>
</Schema>");
        }

        public static IEdmModel NavigationWithBothContainmentEnds()
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
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
                new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            pet.AddProperty(petToPerson);
            person.AddProperty(petToPerson.Partner);

            return model;
        }

        public static IEnumerable<XElement> NavigationWithBothContainmentEndsCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToPet"" ContainsTarget=""true"" />
  </EntityType>
</Schema>");
        }

        public static IEdmModel NavigationWithOneMultiplicityContainmentEnd()
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

            var onePetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "OnePetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToOnePet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            pet.AddProperty(onePetToPerson);
            person.AddProperty(onePetToPerson.Partner);

            var zeroOrOnePetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ZeroOrOnePetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToZeroOrOnePet", Target = pet, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true });
            pet.AddProperty(zeroOrOnePetToPerson);
            person.AddProperty(zeroOrOnePetToPerson.Partner);

            var manyPetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ManyPetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToManyPet", Target = pet, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true });
            pet.AddProperty(manyPetToPerson);
            person.AddProperty(manyPetToPerson.Partner);

            return model;
        }

        public static IEnumerable<XElement> NavigationWithOneMultiplicityContainmentEndCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToOnePet"" Type=""NS.Pet"" Nullable=""false"" Partner=""OnePetToPerson"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToZeroOrOnePet"" Type=""NS.Pet"" Partner=""ZeroOrOnePetToPerson""  ContainsTarget=""true"" />
    <NavigationProperty Name=""ToManyPet"" Type=""Collection(NS.Pet)"" Partner=""ManyPetToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""OnePetToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToOnePet""  />
    <NavigationProperty Name=""ZeroOrOnePetToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToZeroOrOnePet"" />
    <NavigationProperty Name=""ManyPetToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToManyPet"" />
  </EntityType>
</Schema>");
        }

        public static IEdmModel NavigationWithManyMultiplicityContainmentEnd()
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

            var onePetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "OnePetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "ToOnePet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            pet.AddProperty(onePetToPerson);
            person.AddProperty(onePetToPerson.Partner);

            var zeroOrOnePetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ZeroOrOnePetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "ToZeroOrOnePet", Target = pet, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true });
            pet.AddProperty(zeroOrOnePetToPerson);
            person.AddProperty(zeroOrOnePetToPerson.Partner);

            var manyPetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ManyPetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "ToManyPet", Target = pet, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true });
            pet.AddProperty(manyPetToPerson);
            person.AddProperty(manyPetToPerson.Partner);

            return model;
        }

        public static IEnumerable<XElement> NavigationWithManyMultiplicityContainmentEndCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToOnePet"" Type=""NS.Pet"" Nullable=""false"" Partner=""OnePetToPerson""  ContainsTarget=""true"" />
    <NavigationProperty Name=""ToZeroOrOnePet"" Type=""NS.Pet"" Partner=""ZeroOrOnePetToPerson"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToManyPet"" Type=""Collection(NS.Pet)"" Partner=""ManyPetToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""OnePetToPerson"" Type=""Collection(NS.Person)"" Partner=""ToOnePet"" />
    <NavigationProperty Name=""ZeroOrOnePetToPerson"" Type=""Collection(NS.Person)"" Partner=""ToZeroOrOnePet"" />
    <NavigationProperty Name=""ManyPetToPerson"" Type=""Collection(NS.Person)"" Partner=""ToManyPet"" />
  </EntityType>
</Schema>");
        }

        public static IEdmModel NavigationWithZeroOrOneMultiplicityContainmentEnd()
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

            var onePetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "OnePetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
                new EdmNavigationPropertyInfo() { Name = "ToOnePet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            pet.AddProperty(onePetToPerson);
            person.AddProperty(onePetToPerson.Partner);

            var zeroOrOnePetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ZeroOrOnePetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
                new EdmNavigationPropertyInfo() { Name = "ToZeroOrOnePet", Target = pet, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true });
            pet.AddProperty(zeroOrOnePetToPerson);
            person.AddProperty(zeroOrOnePetToPerson.Partner);

            var manyPetToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ManyPetToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne },
                new EdmNavigationPropertyInfo() { Name = "ToManyPet", Target = pet, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true });
            pet.AddProperty(manyPetToPerson);
            person.AddProperty(manyPetToPerson.Partner);

            return model;
        }

        public static IEnumerable<XElement> NavigationWithZeroOrOneMultiplicityContainmentEndCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToOnePet"" Type=""NS.Pet"" Nullable=""false"" Partner=""OnePetToPerson"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToZeroOrOnePet"" Type=""NS.Pet"" Partner=""ZeroOrOnePetToPerson"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToManyPet"" Type=""Collection(NS.Pet)"" Partner=""ManyPetToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""OnePetToPerson"" Type=""NS.Person"" Partner=""ToOnePet"" />
    <NavigationProperty Name=""ZeroOrOnePetToPerson"" Type=""NS.Person"" Partner=""ToZeroOrOnePet"" />
    <NavigationProperty Name=""ManyPetToPerson"" Type=""NS.Person"" Partner=""ToManyPet""  />
  </EntityType>
</Schema>");
        }

        public static IEdmModel NavigationWithValidZeroOrOneMultiplicityRecursiveContainmentEnd()
        {
            var model = new EdmModel();

            var person = new EdmEntityType("NS", "Person");
            var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            var friendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true },
                new EdmNavigationPropertyInfo() { Name = "ToFriend", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
            person.AddProperty(friendToPerson);
            person.AddProperty(friendToPerson.Partner);

            var parentToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToSelf", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true },
                new EdmNavigationPropertyInfo() { Name = "ToParent", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
            person.AddProperty(parentToPerson);
            person.AddProperty(parentToPerson.Partner);

            return model;
        }

        public static IEnumerable<XElement> NavigationWithValidZeroOrOneMultiplicityRecursiveContainmentEndCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""Collection(NS.Person)"" Partner=""ToFriend"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToFriend"" Type=""NS.Person"" Partner=""ToPerson"" />
    <NavigationProperty Name=""ToSelf"" Type=""NS.Person"" Partner=""ToParent"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToParent"" Type=""NS.Person"" Partner=""ToSelf"" />
  </EntityType>
</Schema>");
        }

        public static IEdmModel NavigationWithInvaliZeroOrOnedMultiplicityRecursiveContainmentEnd()
        {
            var model = new EdmModel();

            var person = new EdmEntityType("NS", "Person");
            var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            var friendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
                new EdmNavigationPropertyInfo() { Name = "ToFriend", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
            person.AddProperty(friendToPerson);
            person.AddProperty(friendToPerson.Partner);
            
            return model;
        }

        public static IEnumerable<XElement> NavigationWithInvaliZeroOrOnedMultiplicityRecursiveContainmentEndCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToFriend"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToFriend"" Type=""NS.Person"" Partner=""ToPerson""  />
  </EntityType>
</Schema>");
        }

        public static IEdmModel NavigationWithOneMultiplicityRecursiveContainmentEnd()
        {
            var model = new EdmModel();

            var person = new EdmEntityType("NS", "Person");
            var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            var manyFriendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ManyFriendToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToManyFriend", Target = person, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true });
            person.AddProperty(manyFriendToPerson);
            person.AddProperty(manyFriendToPerson.Partner);

            var zeroOrOneFriendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ZeroOrOneFriendToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToZeroOrOneFriend", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true });
            person.AddProperty(zeroOrOneFriendToPerson);
            person.AddProperty(zeroOrOneFriendToPerson.Partner);

            var oneFriendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "OneFriendToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToOneFriend", Target = person, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            person.AddProperty(oneFriendToPerson);
            person.AddProperty(oneFriendToPerson.Partner);

            return model;
        }

        public static IEnumerable<XElement> NavigationWithOneMultiplicityRecursiveContainmentEndCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ManyFriendToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToManyFriend"" />
    <NavigationProperty Name=""ToManyFriend"" Type=""Collection(NS.Person)"" Partner=""ManyFriendToPerson"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ZeroOrOneFriendToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToZeroOrOneFriend"" />
    <NavigationProperty Name=""ToZeroOrOneFriend"" Type=""NS.Person"" Partner=""ZeroOrOneFriendToPerson"" ContainsTarget=""true"" />
    <NavigationProperty Name=""OneFriendToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToOneFriend"" />
    <NavigationProperty Name=""ToOneFriend"" Type=""NS.Person"" Nullable=""false"" Partner=""OneFriendToPerson"" ContainsTarget=""true"" />
  </EntityType>
</Schema>");
        }

        public static IEdmModel NavigationWithManyMultiplicityRecursiveContainmentEnd()
        {
            var model = new EdmModel();

            var person = new EdmEntityType("NS", "Person");
            var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            var manyFriendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ManyFriendToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "ToManyFriend", Target = person, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true });
            person.AddProperty(manyFriendToPerson);
            person.AddProperty(manyFriendToPerson.Partner);

            var zeroOrOneFriendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ZeroOrOneFriendToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "ToZeroOrOneFriend", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true });
            person.AddProperty(zeroOrOneFriendToPerson);
            person.AddProperty(zeroOrOneFriendToPerson.Partner);

            var oneFriendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "OneFriendToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "ToOneFriend", Target = person, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            person.AddProperty(oneFriendToPerson);
            person.AddProperty(oneFriendToPerson.Partner);

            return model;
        }

        public static IEnumerable<XElement> NavigationWithManyMultiplicityRecursiveContainmentEndCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ManyFriendToPerson"" Type=""Collection(NS.Person)"" Partner=""ToManyFriend""  />
    <NavigationProperty Name=""ToManyFriend"" Type=""Collection(NS.Person)"" Partner=""ManyFriendToPerson"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ZeroOrOneFriendToPerson"" Type=""Collection(NS.Person)"" Partner=""ToZeroOrOneFriend"" />
    <NavigationProperty Name=""ToZeroOrOneFriend"" Type=""NS.Person"" Partner=""ZeroOrOneFriendToPerson""  ContainsTarget=""true"" />
    <NavigationProperty Name=""OneFriendToPerson"" Type=""Collection(NS.Person)"" Partner=""ToOneFriend"" />
    <NavigationProperty Name=""ToOneFriend"" Type=""NS.Person"" Nullable=""false"" Partner=""OneFriendToPerson""  ContainsTarget=""true"" />
  </EntityType>
</Schema>");
        }

        public static IEdmModel SingleSimpleContainmentNavigation()
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

            var home = new EdmEntityType("NS", "Home");
            var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            home.AddKeys(homeId);
            model.AddElement(home);

            var personToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One });
            pet.AddProperty(personToPet.Partner);
            person.AddProperty(personToPet);

            var homeToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
                new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One });
            pet.AddProperty(homeToPet.Partner);
            home.AddProperty(homeToPet);

            var container = new EdmEntityContainer("NS", "Container");
            var personSet = container.AddEntitySet("PersonSet", person);
            var petSet = container.AddEntitySet("PetSet", pet);
            var homeSet = container.AddEntitySet("HomeSet", home);

            petSet.AddNavigationTarget(personToPet.Partner, personSet);
            personSet.AddNavigationTarget(personToPet, petSet);

            model.AddElement(container);

            return model;
        }

        public static IEnumerable<XElement> SingleSimpleContainmentNavigationCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToPet"" />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPet"" />
  </EntityType>
  <EntityType Name=""Home"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToHome""  ContainsTarget=""true"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""PersonSet"" EntityType=""NS.Person"">
      <NavigationPropertyBinding Path=""ToPet"" Target=""PetSet"" />
    </EntitySet>
    <EntitySet Name=""PetSet"" EntityType=""NS.Pet"">
      <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
    </EntitySet>
    <EntitySet Name=""HomeSet"" EntityType=""NS.Home"" />
  </EntityContainer>
</Schema>");
        }

        public static IEdmModel TwoContainmentNavigationWithSameEnd()
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

            var home = new EdmEntityType("NS", "Home");
            var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            home.AddKeys(homeId);
            model.AddElement(home);

            var personToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One });
            pet.AddProperty(personToPet.Partner);
            person.AddProperty(personToPet);

            var homeToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
                new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One });
            pet.AddProperty(homeToPet.Partner);
            home.AddProperty(homeToPet);

            var container = new EdmEntityContainer("NS", "Container");
            var personSet = container.AddEntitySet("PersonSet", person);
            var petSet = container.AddEntitySet("PetSet", pet);
            var homeSet = container.AddEntitySet("HomeSet", home);

            petSet.AddNavigationTarget(personToPet.Partner, personSet);
            petSet.AddNavigationTarget(homeToPet.Partner, homeSet);

            personSet.AddNavigationTarget(personToPet, petSet);
            homeSet.AddNavigationTarget(homeToPet, petSet);

            model.AddElement(container);

            return model;
        }

        public static IEnumerable<XElement> TwoContainmentNavigationWithSameEndCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToPet"" />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPet"" />
  </EntityType>
  <EntityType Name=""Home"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToHome"" ContainsTarget=""true"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""PersonSet"" EntityType=""NS.Person"">
      <NavigationPropertyBinding Path=""ToPet"" Target=""PetSet"" />
    </EntitySet>
    <EntitySet Name=""PetSet"" EntityType=""NS.Pet"">
      <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
      <NavigationPropertyBinding Path=""ToHome"" Target=""HomeSet"" />
    </EntitySet>
    <EntitySet Name=""HomeSet"" EntityType=""NS.Home"">
      <NavigationPropertyBinding Path=""ToPet"" Target=""PetSet"" />
    </EntitySet>
  </EntityContainer>
</Schema>");
        }

        [CustomConsistentValidationTest]
        public static IEdmModel TwoContainmentNavigationWithSameEndAddedDifferently()
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

            var home = new EdmEntityType("NS", "Home");
            var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            home.AddKeys(homeId);
            model.AddElement(home);

            var personToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One });
            pet.AddProperty(personToPet.Partner);
            person.AddProperty(personToPet);

            var homeToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
                new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One });
            pet.AddProperty(homeToPet.Partner);
            home.AddProperty(homeToPet);

            var container = new EdmEntityContainer("NS", "Container");
            var personSet = container.AddEntitySet("PersonSet", person);
            var petSet = container.AddEntitySet("PetSet", pet);
            var homeSet = container.AddEntitySet("HomeSet", home);

            // [EdmLib] EntitySet.AddNavigationTarget() ordering matters and results into some validation not appearing
            personSet.AddNavigationTarget(personToPet, petSet);
            homeSet.AddNavigationTarget(homeToPet, petSet);
            petSet.AddNavigationTarget(personToPet.Partner, personSet);
            petSet.AddNavigationTarget(homeToPet.Partner, homeSet);

            model.AddElement(container);

            return model;
        }

        public static IEdmModel ContainmentNavigationWithDifferentEnds()
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

            var home = new EdmEntityType("NS", "Home");
            var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            home.AddKeys(homeId);
            model.AddElement(home);

            var petToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            pet.AddProperty(petToPerson);
            person.AddProperty(petToPerson.Partner);

            var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            person.AddProperty(homeToPerson.Partner);
            home.AddProperty(homeToPerson);

            var container = new EdmEntityContainer("NS", "Container");
            var personSet = container.AddEntitySet("PersonSet", person);
            var petSet = container.AddEntitySet("PetSet", pet);
            var homeSet = container.AddEntitySet("HomeSet", home);

            personSet.AddNavigationTarget(petToPerson.Partner, petSet);
            personSet.AddNavigationTarget(homeToPerson.Partner, homeSet);

            petSet.AddNavigationTarget(petToPerson, personSet);
            homeSet.AddNavigationTarget(homeToPerson, personSet);
            model.AddElement(container);

            return model;
        }

        public static IEnumerable<XElement> ContainmentNavigationWithDifferentEndsCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToPerson"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToPet"" />
  </EntityType>
  <EntityType Name=""Home"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToHome"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""PersonSet"" EntityType=""NS.Person"">
      <NavigationPropertyBinding Path=""ToPet"" Target=""PetSet"" />
      <NavigationPropertyBinding Path=""ToHome"" Target=""HomeSet"" />
    </EntitySet>
    <EntitySet Name=""PetSet"" EntityType=""NS.Pet"">
      <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
    </EntitySet>
    <EntitySet Name=""HomeSet"" EntityType=""NS.Home"">
      <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
    </EntitySet>
  </EntityContainer>
</Schema>");
        }

        public static IEdmModel RecursiveThreeContainmentNavigations()
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

            var home = new EdmEntityType("NS", "Home");
            var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            home.AddKeys(homeId);
            model.AddElement(home);

            var personToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One });
            pet.AddProperty(personToPet.Partner);
            person.AddProperty(personToPet);

            var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
                new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One });
            person.AddProperty(homeToPerson.Partner);
            home.AddProperty(homeToPerson);

            var petToHome = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
                new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One });
            pet.AddProperty(petToHome);
            home.AddProperty(petToHome.Partner);

            return model;
        }

        public static IEnumerable<XElement> RecursiveThreeContainmentNavigationsCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToPerson""  ContainsTarget=""true"" />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPerson"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToPet""  />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPet""  ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Home"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToHome"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToHome"" />
  </EntityType>
</Schema>");
        }

        public static IEdmModel RecursiveThreeContainmentNavigationsWithEntitySet()
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

            var home = new EdmEntityType("NS", "Home");
            var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            home.AddKeys(homeId);
            model.AddElement(home);

            var personToPet = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One });
            pet.AddProperty(personToPet.Partner);
            person.AddProperty(personToPet);

            var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
                new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One });
            person.AddProperty(homeToPerson.Partner);
            home.AddProperty(homeToPerson);

            var petToHome = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true },
                new EdmNavigationPropertyInfo() { Name = "ToPet", Target = pet, TargetMultiplicity = EdmMultiplicity.One });
            pet.AddProperty(petToHome);
            home.AddProperty(petToHome.Partner);

            var container = new EdmEntityContainer("NS", "Container");
            model.AddElement(container);

            var personSet = container.AddEntitySet("PersonSet", person);
            var petSet = container.AddEntitySet("PetSet", pet);
            var homeSet = container.AddEntitySet("HomeSet", home);

            personSet.AddNavigationTarget(personToPet, petSet);
            petSet.AddNavigationTarget(personToPet.Partner, personSet);
            petSet.AddNavigationTarget(petToHome, homeSet);
            homeSet.AddNavigationTarget(petToHome.Partner, petSet);
            homeSet.AddNavigationTarget(homeToPerson, personSet);
            personSet.AddNavigationTarget(homeToPerson.Partner, homeSet);

            return model;
        }

        public static IEnumerable<XElement> RecursiveThreeContainmentNavigationsWithEntitySetCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToPerson"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPerson"" />
  </EntityType>
  <EntityType Name=""Pet"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToPet"" />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPet"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Home"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToHome"" ContainsTarget=""true"" />
    <NavigationProperty Name=""ToPet"" Type=""NS.Pet"" Nullable=""false"" Partner=""ToHome"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""PersonSet"" EntityType=""NS.Person"">
      <NavigationPropertyBinding Path=""ToPet"" Target=""PetSet"" />
      <NavigationPropertyBinding Path=""ToHome"" Target=""HomeSet"" />
    </EntitySet>
    <EntitySet Name=""PetSet"" EntityType=""NS.Pet"">
      <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
      <NavigationPropertyBinding Path=""ToHome"" Target=""HomeSet"" />
    </EntitySet>
    <EntitySet Name=""HomeSet"" EntityType=""NS.Home"">
      <NavigationPropertyBinding Path=""ToPet"" Target=""PetSet"" />
      <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
    </EntitySet>
  </EntityContainer>
</Schema>");
        }

        public static IEdmModel RecursiveOneContainmentNavigationSelfPointingEntitySet()
        {
            var model = new EdmModel();

            var person = new EdmEntityType("NS", "Person");
            var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            var personToFriends = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToFriend", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true },
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
            person.AddProperty(personToFriends.Partner);
            person.AddProperty(personToFriends);

            var container = new EdmEntityContainer("NS", "Container");
            model.AddElement(container);
            var personSet = container.AddEntitySet("PersonSet", person);
            personSet.AddNavigationTarget(personToFriends, personSet);
            personSet.AddNavigationTarget(personToFriends.Partner, personSet);

            return model;
        }

        public static IEdmModel RecursiveOneContainmentNavigationInheritedSelfPointingEntitySet()
        {
            var model = new EdmModel();

            var person = new EdmEntityType("NS", "Person");
            var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            var employee = new EdmEntityType("NS", "Employee", person);
            model.AddElement(employee);

            var personToFriends = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToFriend", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true },
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
            person.AddProperty(personToFriends.Partner);
            person.AddProperty(personToFriends);

            var container = new EdmEntityContainer("NS", "Container");
            model.AddElement(container);
            var personSet = container.AddEntitySet("PersonSet", person);
            var employeeSet = container.AddEntitySet("EmployeeSet", employee);
            personSet.AddNavigationTarget(personToFriends, employeeSet);
            employeeSet.AddNavigationTarget(personToFriends.Partner, personSet);

            return model;
        }

        public static IEnumerable<XElement> RecursiveOneContainmentNavigationSelfPointingEntitySetCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Partner=""ToFriend""  />
    <NavigationProperty Name=""ToFriend"" Type=""NS.Person"" Partner=""ToPerson""  ContainsTarget=""true"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""PersonSet"" EntityType=""NS.Person"">
      <NavigationPropertyBinding Path=""ToFriend"" Target=""PersonSet"" />
      <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
    </EntitySet>
  </EntityContainer>
</Schema>");
        }

        public static IEdmModel RecursiveOneContainmentNavigationWithTwoEntitySet()
        {
            var model = new EdmModel();

            var person = new EdmEntityType("NS", "Person");
            var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            var personToFriends = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToFriend", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne, ContainsTarget = true },
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
            person.AddProperty(personToFriends.Partner);
            person.AddProperty(personToFriends);

            var container = new EdmEntityContainer("NS", "Container");
            model.AddElement(container);
            var personSet = container.AddEntitySet("PersonSet", person);
            var friendSet = container.AddEntitySet("FriendSet", person);
            personSet.AddNavigationTarget(personToFriends, friendSet);
            friendSet.AddNavigationTarget(personToFriends.Partner, personSet);

            return model;
        }

        public static IEnumerable<XElement> RecursiveOneContainmentNavigationWithTwoEntitySetCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Partner=""ToFriend"" />
    <NavigationProperty Name=""ToFriend"" Type=""NS.Person"" Partner=""ToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""FriendSet"" EntityType=""NS.Person"">
      <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
    </EntitySet>
    <EntitySet Name=""PersonSet"" EntityType=""NS.Person"">
      <NavigationPropertyBinding Path=""ToFriend"" Target=""FriendSet"" />
    </EntitySet>
  </EntityContainer>
</Schema>");
        }

        public static IEdmModel DerivedContainmentNavigationWithBaseAssociationSet()
        {
            var model = new EdmModel();

            var person = new EdmEntityType("NS", "Person");
            var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            var employee = new EdmEntityType("NS", "Employee", person);
            model.AddElement(employee);

            var home = new EdmEntityType("NS", "Home");
            var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            home.AddKeys(homeId);
            model.AddElement(home);

            var office = new EdmEntityType("NS", "Office", home);
            model.AddElement(office);

            var officeToEmployee = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToEmployee", Target = employee, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToOffice", Target = office, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            employee.AddProperty(officeToEmployee.Partner);
            office.AddProperty(officeToEmployee);

            var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            person.AddProperty(homeToPerson.Partner);
            home.AddProperty(homeToPerson);

            var container = new EdmEntityContainer("NS", "Container");
            model.AddElement(container);

            var personSet = container.AddEntitySet("PersonSet", person);
            var employeeSet = container.AddEntitySet("EmployeeSet", employee);
            var homeSet = container.AddEntitySet("HomeSet", home);
            var officeSet = container.AddEntitySet("OfficeSet", office);

            homeSet.AddNavigationTarget(officeToEmployee, personSet);
            personSet.AddNavigationTarget(officeToEmployee.Partner, homeSet);

            return model;
        }

        public static IEnumerable<XElement> DerivedContainmentNavigationWithBaseAssociationSetCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPerson""  ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Employee"" BaseType=""NS.Person"">
    <NavigationProperty Name=""ToOffice"" Type=""NS.Office"" Nullable=""false"" Partner=""ToEmployee"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Home"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToHome""  />
  </EntityType>
  <EntityType Name=""Office"" BaseType=""NS.Home"">
    <NavigationProperty Name=""ToEmployee"" Type=""NS.Employee"" Nullable=""false"" Partner=""ToOffice""  />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""PersonSet"" EntityType=""NS.Person"">
      <NavigationPropertyBinding Path=""NS.Employee/ToOffice"" Target=""HomeSet"" />
    </EntitySet>
    <EntitySet Name=""EmployeeSet"" EntityType=""NS.Employee"" />
    <EntitySet Name=""HomeSet"" EntityType=""NS.Home"">
      <NavigationPropertyBinding Path=""NS.Office/ToEmployee"" Target=""PersonSet"" />
    </EntitySet>
    <EntitySet Name=""OfficeSet"" EntityType=""NS.Office"" />
  </EntityContainer>
</Schema>");
        }

        public static IEdmModel DerivedContainmentNavigationWithDerivedAssociationSet()
        {
            var model = new EdmModel();

            var person = new EdmEntityType("NS", "Person");
            var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            var employee = new EdmEntityType("NS", "Employee", person);
            model.AddElement(employee);

            var home = new EdmEntityType("NS", "Home");
            var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            home.AddKeys(homeId);
            model.AddElement(home);

            var office = new EdmEntityType("NS", "Office", home);
            model.AddElement(office);

            var officeToEmployee = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToEmployee", Target = employee, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToOffice", Target = office, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            employee.AddProperty(officeToEmployee.Partner);
            office.AddProperty(officeToEmployee);

            var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            person.AddProperty(homeToPerson.Partner);
            home.AddProperty(homeToPerson);

            var container = new EdmEntityContainer("NS", "Container");
            model.AddElement(container);

            var personSet = container.AddEntitySet("PersonSet", person);
            var employeeSet = container.AddEntitySet("EmployeeSet", employee);
            var homeSet = container.AddEntitySet("HomeSet", home);
            var officeSet = container.AddEntitySet("OfficeSet", office);

            officeSet.AddNavigationTarget(homeToPerson, employeeSet);
            employeeSet.AddNavigationTarget(homeToPerson.Partner, officeSet);

            return model;
        }

        public static IEnumerable<XElement> DerivedContainmentNavigationWithDerivedAssociationSetCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Employee"" BaseType=""NS.Person"">
    <NavigationProperty Name=""ToOffice"" Type=""NS.Office"" Nullable=""false"" Partner=""ToEmployee"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Home"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToHome"" />
  </EntityType>
  <EntityType Name=""Office"" BaseType=""NS.Home"">
    <NavigationProperty Name=""ToEmployee"" Type=""NS.Employee"" Nullable=""false"" Partner=""ToOffice"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""PersonSet"" EntityType=""NS.Person"" />
    <EntitySet Name=""EmployeeSet"" EntityType=""NS.Employee"">
      <NavigationPropertyBinding Path=""ToHome"" Target=""OfficeSet"" />
    </EntitySet>
    <EntitySet Name=""HomeSet"" EntityType=""NS.Home"" />
    <EntitySet Name=""OfficeSet"" EntityType=""NS.Office"">
      <NavigationPropertyBinding Path=""ToPerson"" Target=""EmployeeSet"" />
    </EntitySet>
  </EntityContainer>
</Schema>");
        }

        public static IEdmModel DerivedContainmentNavigationWithDerivedAndBaseAssociationSet()
        {
            var model = new EdmModel();

            var person = new EdmEntityType("NS", "Person");
            var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            var employee = new EdmEntityType("NS", "Employee", person);
            model.AddElement(employee);

            var home = new EdmEntityType("NS", "Home");
            var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            home.AddKeys(homeId);
            model.AddElement(home);

            var office = new EdmEntityType("NS", "Office", home);
            model.AddElement(office);

            var officeToEmployee = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToEmployee", Target = employee, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToOffice", Target = office, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            employee.AddProperty(officeToEmployee.Partner);
            office.AddProperty(officeToEmployee);

            var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            person.AddProperty(homeToPerson.Partner);
            home.AddProperty(homeToPerson);

            var container = new EdmEntityContainer("NS", "Container");
            model.AddElement(container);

            var personSet = container.AddEntitySet("PersonSet", person);
            var employeeSet = container.AddEntitySet("EmployeeSet", employee);
            var homeSet = container.AddEntitySet("HomeSet", home);
            var officeSet = container.AddEntitySet("OfficeSet", office);

            officeSet.AddNavigationTarget(officeToEmployee, personSet);
            personSet.AddNavigationTarget(officeToEmployee.Partner, officeSet);
            homeSet.AddNavigationTarget(homeToPerson, employeeSet);
            employeeSet.AddNavigationTarget(homeToPerson.Partner, homeSet);

            return model;
        }

        public static IEnumerable<XElement> DerivedContainmentNavigationWithDerivedAndBaseAssociationSetCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""Person"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToHome"" Type=""NS.Home"" Nullable=""false"" Partner=""ToPerson"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Employee"" BaseType=""NS.Person"">
    <NavigationProperty Name=""ToOffice"" Type=""NS.Office"" Nullable=""false"" Partner=""ToEmployee"" ContainsTarget=""true"" />
  </EntityType>
  <EntityType Name=""Home"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    <NavigationProperty Name=""ToPerson"" Type=""NS.Person"" Nullable=""false"" Partner=""ToHome"" />
  </EntityType>
  <EntityType Name=""Office"" BaseType=""NS.Home"">
    <NavigationProperty Name=""ToEmployee"" Type=""NS.Employee"" Nullable=""false"" Partner=""ToOffice"" />
  </EntityType>
  <EntityContainer Name=""Container"">
    <EntitySet Name=""PersonSet"" EntityType=""NS.Person"">
      <NavigationPropertyBinding Path=""NS.Employee/ToOffice"" Target=""OfficeSet"" />
    </EntitySet>
    <EntitySet Name=""EmployeeSet"" EntityType=""NS.Employee"">
      <NavigationPropertyBinding Path=""ToHome"" Target=""HomeSet"" />
    </EntitySet>
    <EntitySet Name=""HomeSet"" EntityType=""NS.Home"">
      <NavigationPropertyBinding Path=""ToPerson"" Target=""EmployeeSet"" />
    </EntitySet>
    <EntitySet Name=""OfficeSet"" EntityType=""NS.Office"">
      <NavigationPropertyBinding Path=""ToEmployee"" Target=""PersonSet"" />
    </EntitySet>
  </EntityContainer>
</Schema>");
        }

        [CustomConsistentValidationTestAttribute]
        public static IEdmModel NavigationWithInvalidAssociationSet()
        {
            var model = new EdmModel();

            var person = new EdmEntityType("NS", "Person");
            var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            var home = new EdmEntityType("NS", "Home");
            var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            home.AddKeys(homeId);
            model.AddElement(home);

            var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            person.AddProperty(homeToPerson.Partner);
            home.AddProperty(homeToPerson);

            // Has not been added to model
            var employee = new EdmEntityType("NS", "Employee", person);
            var office = new EdmEntityType("NS", "Office", home);

            var officeToEmployee = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToEmployee", Target = employee, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToOffice", Target = office, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            employee.AddProperty(officeToEmployee.Partner);
            office.AddProperty(officeToEmployee);

            var container = new EdmEntityContainer("NS", "Container");
            model.AddElement(container);

            var personSet = container.AddEntitySet("PersonSet", person);
            var homeSet = container.AddEntitySet("HomeSet", home);

            homeSet.AddNavigationTarget(officeToEmployee, personSet);
            personSet.AddNavigationTarget(officeToEmployee.Partner, homeSet);

            return model;
        }

        public static IEdmModel NavigationWithInvalidEntitySet()
        {
            var model = new EdmModel();

            var person = new EdmEntityType("NS", "Person");
            var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            var home = new EdmEntityType("NS", "Home");
            var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            home.AddKeys(homeId);
            model.AddElement(home);

            var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            person.AddProperty(homeToPerson.Partner);
            home.AddProperty(homeToPerson);

            // Has not been added to model
            var employee = new EdmEntityType("NS", "Employee", person);
            var office = new EdmEntityType("NS", "Office", home);

            var officeToEmployee = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToEmployee", Target = employee, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToOffice", Target = office, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            employee.AddProperty(officeToEmployee.Partner);
            office.AddProperty(officeToEmployee);

            var container = new EdmEntityContainer("NS", "Container");
            model.AddElement(container);

            var personSet = container.AddEntitySet("PersonSet", person);
            var homeSet = container.AddEntitySet("HomeSet", home);
            var employeeSet = container.AddEntitySet("EmployeeSet", employee);
            var officeSet = container.AddEntitySet("OfficeSet", office);
            
            return model;
        }

        public static IEdmModel NavigationAssociationSetWithInvalidEntitySet()
        {
            var model = new EdmModel();

            var person = new EdmEntityType("NS", "Person");
            var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            var home = new EdmEntityType("NS", "Home");
            var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            home.AddKeys(homeId);
            model.AddElement(home);

            var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            person.AddProperty(homeToPerson.Partner);
            home.AddProperty(homeToPerson);

            var employee = new EdmEntityType("NS", "Employee");
            var employeeId = employee.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            employee.AddKeys(employeeId);
            model.AddElement(employee);

            var office = new EdmEntityType("NS", "Office");
            var officeId = office.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            office.AddKeys(officeId);
            model.AddElement(office);

            var officeToEmployee = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToEmployee", Target = employee, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToOffice", Target = office, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            employee.AddProperty(officeToEmployee.Partner);
            office.AddProperty(officeToEmployee);

            var container = new EdmEntityContainer("NS", "Container");
            model.AddElement(container);

            var personSet = container.AddEntitySet("PersonSet", person);
            var homeSet = container.AddEntitySet("HomeSet", home);

            homeSet.AddNavigationTarget(officeToEmployee, personSet);
            personSet.AddNavigationTarget(officeToEmployee.Partner, homeSet);

            return model;
        }

        public static IEdmModel NavigationAssociationSetWithInvalidEntitySetInSingleton()
        {
            var model = new EdmModel();

            var person = new EdmEntityType("NS", "Person");
            var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            var home = new EdmEntityType("NS", "Home");
            var homeId = home.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            home.AddKeys(homeId);
            model.AddElement(home);

            var homeToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToHome", Target = home, TargetMultiplicity = EdmMultiplicity.One, ContainsTarget = true });
            person.AddProperty(homeToPerson.Partner);
            home.AddProperty(homeToPerson);

            var employee = new EdmEntityType("NS", "Employee");
            var employeeId = employee.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            employee.AddKeys(employeeId);
            model.AddElement(employee);

            var office = new EdmEntityType("NS", "Office");
            var officeId = office.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            office.AddKeys(officeId);
            model.AddElement(office);

            var officeToEmployee = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToEmployee", Target = employee, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToOffice", Target = office, TargetMultiplicity = EdmMultiplicity.One});
            employee.AddProperty(officeToEmployee.Partner);
            office.AddProperty(officeToEmployee);

            var container = new EdmEntityContainer("NS", "Container");
            model.AddElement(container);

            var boss = container.AddSingleton("Boss", person);
            var homeSet = container.AddEntitySet("HomeSet", home);

            homeSet.AddNavigationTarget(officeToEmployee, boss);
            boss.AddNavigationTarget(officeToEmployee.Partner, homeSet);

            return model;
        }

        public static IEdmModel NavigationPropertyOfCollectionTypeTargetToSingleton()
        {
            var model = new EdmModel();

            var person = new EdmEntityType("NS", "Person");
            var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            var address = new EdmEntityType("NS", "Address");
            var addressId = address.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            address.AddKeys(addressId);
            model.AddElement(address);

            var personToAddress = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "Addresses", Target = address, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo() { Name = "Owner", Target = person, TargetMultiplicity = EdmMultiplicity.One});
            person.AddProperty(personToAddress);
            address.AddProperty(personToAddress.Partner);

            var container = new EdmEntityContainer("NS", "Container");
            model.AddElement(container);

            var people = container.AddEntitySet("People", person);
            var singleAddress = container.AddSingleton("SingleAddress", address);

            people.AddNavigationTarget(personToAddress, singleAddress);
            singleAddress.AddNavigationTarget(personToAddress.Partner, people);

            return model;
        }

        [CustomConsistentValidationTest]
        public static IEdmModel NavigationWithUnknownMultiplicity()
        {
            var model = new EdmModel();

            var person = new EdmEntityType("NS", "Person");
            var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            var friendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.Unknown },
                new EdmNavigationPropertyInfo() { Name = "ToFriend", Target = person, TargetMultiplicity = EdmMultiplicity.One });
            person.AddProperty(friendToPerson);
            person.AddProperty(friendToPerson.Partner);

            return model;
        }

        [CustomConsistentValidationTest]
        public static IEdmModel NavigationWithUnknownMultiplicityPartner()
        {
            var model = new EdmModel();

            var person = new EdmEntityType("NS", "Person");
            var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            var friendToPerson = EdmNavigationProperty.CreateNavigationPropertyWithPartner(
                new EdmNavigationPropertyInfo() { Name = "ToPerson", Target = person, TargetMultiplicity = EdmMultiplicity.One },
                new EdmNavigationPropertyInfo() { Name = "ToFriend", Target = person, TargetMultiplicity = EdmMultiplicity.Unknown});
            person.AddProperty(friendToPerson);
            person.AddProperty(friendToPerson.Partner);

            return model;
        }

        public static IEdmModel NavigationWithEmptyNameModel()
        {
            var model = new EdmModel();

            var entityType = new EdmEntityType("NS", "Entity");
            var entityId = entityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            entityType.AddKeys(entityId);
            model.AddElement(entityType);

            var person = new EdmEntityType("NS", "Person");
            var personId = person.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            person.AddKeys(personId);
            model.AddElement(person);

            var navigation = entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "", Target = person, TargetMultiplicity = EdmMultiplicity.One });
            
            return model;
        }

        public static IEdmModel NavigationSinglePrincipalWithNotNullableDependentModel()
        {
            return NavigationPrincipalEndAndDependentPropertyModelBuilder(false, false, false, false, false, false);
        }

        public static IEdmModel NavigationSinglePrincipalWithNotNullableKeyDependentModel()
        {
            return NavigationPrincipalEndAndDependentPropertyModelBuilder(true, true, false, false, false, false);
        }

        public static IEdmModel NavigationSinglePrincipalWithMixNullableDependentModel()
        {
            return NavigationPrincipalEndAndDependentPropertyModelBuilder(false, false, false, true, false, false);
        }

        public static IEdmModel NavigationSinglePrincipalWithAllNullableDependentModel()
        {
            return NavigationPrincipalEndAndDependentPropertyModelBuilder(false, false, true, true, false, false);
        }

        public static IEdmModel NavigationMultiplePrincipalWithNotNullableDependentModel()
        {
            return NavigationPrincipalEndAndDependentPropertyModelBuilder(false, false, false, false, true, true);
        }

        public static IEdmModel NavigationMultiplePrincipalWithMixNullableDependentModel()
        {
            return NavigationPrincipalEndAndDependentPropertyModelBuilder(false, false, false, true, true, true);
        }

        public static IEdmModel NavigationMultiplePrincipalWithAllNullableDependentModel()
        {
            return NavigationPrincipalEndAndDependentPropertyModelBuilder(false, false, true, true, true, true);
        }

        public static IEdmModel NavigationZeroOnePrincipalWithNotNullableDependentModel()
        {
            return NavigationPrincipalEndAndDependentPropertyModelBuilder(false, false, false, false, true, false);
        }

        public static IEdmModel NavigationZeroOnePrincipalWithNotNullableKeyDependentModel()
        {
            return NavigationPrincipalEndAndDependentPropertyModelBuilder(true, true, false, false, true, false);
        }

        public static IEdmModel NavigationZeroOnePrincipalWithMixNullableDependentModel()
        {
            return NavigationPrincipalEndAndDependentPropertyModelBuilder(false, false, false, true, true, false);
        }

        public static IEdmModel NavigationZeroOnePrincipalWithAllNullableDependentModel()
        {
            return NavigationPrincipalEndAndDependentPropertyModelBuilder(false, false, true, true, true, false);
        }

        public static IEdmModel NavigationZeroOnePrincipalWithAllNullableKeyDependentModel()
        {
            return NavigationPrincipalEndAndDependentPropertyModelBuilder(true, true, true, true, true, false);
        }

        public static IEdmModel MultiNavigationBindingModel()
        {
            var model = new EdmModel();

            var entityType = new EdmEntityType("NS", "EntityType");
            var id = entityType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false));
            entityType.AddKeys(id);

            var containedEntityType = new EdmEntityType("NS", "ContainedEntityType");
            var containedId = containedEntityType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false));
            containedEntityType.AddKeys(containedId);

            var containedNav1 = entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "ContainedNav1",
                Target = containedEntityType,
                TargetMultiplicity = EdmMultiplicity.One,
                ContainsTarget = true
            });

            var containedNav2 = entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "ContainedNav2",
                Target = containedEntityType,
                TargetMultiplicity = EdmMultiplicity.One,
                ContainsTarget = true
            });

            var navEntityType = new EdmEntityType("NS", "NavEntityType");
            var navEntityId = navEntityType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false));
            navEntityType.AddKeys(navEntityId);

            var complex = new EdmComplexType("NS", "ComplexType");
            complex.AddStructuralProperty("Prop1", EdmCoreModel.Instance.GetInt32(false));

            var complxNavP = complex.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "CollectionOfNavOnComplex",
                    Target = navEntityType,
                    TargetMultiplicity = EdmMultiplicity.Many,
                });

            entityType.AddStructuralProperty("complexProp1", new EdmComplexTypeReference(complex, false));
            entityType.AddStructuralProperty("complexProp2", new EdmComplexTypeReference(complex, false));

            var navOnContained = containedEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "NavOnContained",
                    Target = navEntityType,
                    TargetMultiplicity = EdmMultiplicity.One,
                });

            model.AddElement(entityType);
            model.AddElement(containedEntityType);
            model.AddElement(navEntityType);
            model.AddElement(complex);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            model.AddElement(entityContainer);
            var entitySet = new EdmEntitySet(entityContainer, "EntitySet", entityType);
            var navEntitySet1 = new EdmEntitySet(entityContainer, "NavEntitySet1", navEntityType);
            var navEntitySet2 = new EdmEntitySet(entityContainer, "NavEntitySet2", navEntityType);
            entitySet.AddNavigationTarget(complxNavP, navEntitySet1, new EdmPathExpression("complexProp1/CollectionOfNavOnComplex"));
            entitySet.AddNavigationTarget(complxNavP, navEntitySet2, new EdmPathExpression("complexProp2/CollectionOfNavOnComplex"));
            entitySet.AddNavigationTarget(navOnContained, navEntitySet1, new EdmPathExpression("ContainedNav1/NavOnContained"));
            entitySet.AddNavigationTarget(navOnContained, navEntitySet2, new EdmPathExpression("ContainedNav2/NavOnContained"));
            entityContainer.AddElement(entitySet);
            entityContainer.AddElement(navEntitySet1);
            entityContainer.AddElement(navEntitySet2);

            return model;
        }

        public static IEnumerable<XElement> MultiNavigationBindingModelCsdl()
        {
            const string csdl = "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                                "<EntityType Name=\"EntityType\">" +
                                    "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                    "<Property Name=\"ID\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                                    "<Property Name=\"complexProp1\" Type=\"NS.ComplexType\" Nullable=\"false\" />" +
                                    "<Property Name=\"complexProp2\" Type=\"NS.ComplexType\" Nullable=\"false\" />" +
                                    "<NavigationProperty Name=\"ContainedNav1\" Type=\"NS.ContainedEntityType\" Nullable=\"false\" ContainsTarget=\"true\" />" +
                                    "<NavigationProperty Name=\"ContainedNav2\" Type=\"NS.ContainedEntityType\" Nullable=\"false\" ContainsTarget=\"true\" />" +
                                "</EntityType>" +
                                "<EntityType Name=\"ContainedEntityType\">" +
                                    "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                    "<Property Name=\"ID\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                                    "<NavigationProperty Name=\"NavOnContained\" Type=\"NS.NavEntityType\" Nullable=\"false\" />" +
                                "</EntityType>" +
                                "<EntityType Name=\"NavEntityType\">" +
                                    "<Key><PropertyRef Name=\"ID\" /></Key>" +
                                    "<Property Name=\"ID\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                                "</EntityType>" +
                                "<ComplexType Name=\"ComplexType\">" +
                                    "<Property Name=\"Prop1\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                                    "<NavigationProperty Name=\"CollectionOfNavOnComplex\" Type=\"Collection(NS.NavEntityType)\" />" +
                                "</ComplexType>" +
                                "<EntityContainer Name=\"Container\">" +
                                "<EntitySet Name=\"EntitySet\" EntityType=\"NS.EntityType\">" +
                                    "<NavigationPropertyBinding Path=\"complexProp1/CollectionOfNavOnComplex\" Target=\"NavEntitySet1\" />" +
                                    "<NavigationPropertyBinding Path=\"complexProp2/CollectionOfNavOnComplex\" Target=\"NavEntitySet2\" />" +
                                    "<NavigationPropertyBinding Path=\"ContainedNav1/NavOnContained\" Target=\"NavEntitySet1\" />" +
                                    "<NavigationPropertyBinding Path=\"ContainedNav2/NavOnContained\" Target=\"NavEntitySet2\" />" +
                                "</EntitySet>" +
                                "<EntitySet Name=\"NavEntitySet1\" EntityType=\"NS.NavEntityType\" />" +
                                "<EntitySet Name=\"NavEntitySet2\" EntityType=\"NS.NavEntityType\" />" +
                                "</EntityContainer>" +
                                "</Schema>";

            return ConvertCsdlsToXElements(csdl);
        }

        private static IEdmModel NavigationPrincipalEndAndDependentPropertyModelBuilder(bool isPersonIdKey, bool isPersonNameKey, bool isPersonIdNullable, bool isPersonNameNullable, bool isPrincipalEndNullable, bool isPrincipalEndCollection)
        {
            var model = new EdmModel();
            var badgeEntity = GetBadgeEntity(isPersonIdKey, isPersonNameKey, isPersonIdNullable, isPersonNameNullable);
            var personEntity = GetPersonEntity();

            model.AddElement(badgeEntity);
            model.AddElement(personEntity);

            EdmNavigationProperty toPerson = ((EdmEntityType)badgeEntity).AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "ToPerson",
                    Target = (EdmEntityType)personEntity,
                    TargetMultiplicity = isPrincipalEndCollection ? EdmMultiplicity.Many : isPrincipalEndNullable ? EdmMultiplicity.ZeroOrOne : EdmMultiplicity.One,
                    DependentProperties = new[]
                    {
                        badgeEntity.FindProperty("PersonId") as IEdmStructuralProperty,
                        badgeEntity.FindProperty("PersonName") as IEdmStructuralProperty
                    },
                    PrincipalProperties = personEntity.Key()
                },
                new EdmNavigationPropertyInfo()
                {
                    Name = "ToBadge",
                    TargetMultiplicity = EdmMultiplicity.Many
                });

            return model;
        }

        private static IEdmEntityType GetBadgeEntity(bool isPersonIdKey, bool isPersonNameKey, bool isPersonIdNullable, bool isPersonNameNullable)
        {
            var badgeEntity = new EdmEntityType("DefaultNamespace", "Badge");
            var badgeId = badgeEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            var badgePersonId = badgeEntity.AddStructuralProperty("PersonId", EdmCoreModel.Instance.GetInt32(isPersonIdNullable));
            var badgePersonName = badgeEntity.AddStructuralProperty("PersonName", EdmCoreModel.Instance.GetString(isPersonNameNullable));

            var badgeKey = new List<IEdmStructuralProperty> { badgeId };
            if (isPersonIdKey)
            {
                badgeKey.Add(badgePersonId);
            }
            if (isPersonNameKey)
            {
                badgeKey.Add(badgePersonName);
            }
            badgeEntity.AddKeys(badgeKey.ToArray());

            return badgeEntity;
        }

        private static IEdmEntityType GetPersonEntity()
        {
            var personEntity = new EdmEntityType("DefaultNamespace", "Person");
            var personId = personEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            var personName = personEntity.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            personEntity.AddKeys(personId, personName);
            return personEntity;
        }

        private static IEnumerable<XElement> ConvertCsdlsToXElements(params string[] csdls)
        {
            return csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        }
    }
}

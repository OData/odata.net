//---------------------------------------------------------------------
// <copyright file="FindMethodsTestModelBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.OData.Utils.Metadata;

    public static class FindMethodsTestModelBuilder
    {
        public static IEdmModel MultipleSchemasWithDifferentNamespacesEdm()
        {
            var namespaces = new string[] 
                { 
                    "FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespaces.first", 
                    "FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespaces.second" 
                };

            var model = new EdmModel();
            foreach (var namespaceName in namespaces)
            {
                var entityType1 = new EdmEntityType(namespaceName, "validEntityType1");
                entityType1.AddKeys(entityType1.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
                var entityType2 = new EdmEntityType(namespaceName, "VALIDeNTITYtYPE2");
                entityType2.AddKeys(entityType2.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
                var entityType3 = new EdmEntityType(namespaceName, "VALIDeNTITYtYPE3");
                entityType3.AddKeys(entityType3.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));

                entityType1.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo {Name = "Mumble", Target = entityType2, TargetMultiplicity = EdmMultiplicity.Many});

                var complexType = new EdmComplexType(namespaceName, "ValidNameComplexType1");
                complexType.AddStructuralProperty("aPropertyOne", new EdmComplexTypeReference(complexType, false));
                model.AddElements(new IEdmSchemaElement[] { entityType1, entityType2, entityType3, complexType });

                var function1 = new EdmFunction(namespaceName, "ValidFunction1", EdmCoreModel.Instance.GetSingle(false)); 
                var function2 = new EdmFunction(namespaceName, "ValidFunction1", EdmCoreModel.Instance.GetSingle(false));
                function2.AddParameter("param1", new EdmEntityTypeReference(entityType1, false));
                var function3 = new EdmFunction(namespaceName, "ValidFunction1", EdmCoreModel.Instance.GetSingle(false));
                function3.AddParameter("param1", EdmCoreModel.Instance.GetSingle(false));

                model.AddElements(new IEdmSchemaElement[] {function1, function2, function3});
            }

            return model;
        }


        [ValidationTestInvalidModel]
        public static IEnumerable<XElement> MultipleSchemasWithSameNamespace(EdmVersion csdlVersion) // M3
        {
            var csdl = new[] {
                #region MultipleSchemasWithSameNamespace CSDL
                @"<Schema Namespace=""FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace"" p1:UseStrongSpatialTypes=""false"" xmlns:p1=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""validEntityType1"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <NavigationProperty Name=""Grumble"" Type=""Collection(FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace.VALIDeNTITYtYPE2)""/>
  </EntityType>
  <EntityType Name=""VALIDeNTITYtYPE2"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
  </EntityType>
  <ComplexType Name=""ValidNameComplexType1"">
    <Property Name=""aPropertyOne"" Type=""FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace.ValidNameComplexType1"" Nullable=""false"" />
  </ComplexType>
  <Function Name=""ValidFunction1"">
    <ReturnType Type=""Single"" Nullable=""false"" />
  </Function>
  <Function Name=""ValidFunction1"">
    <ReturnType Type=""Single"" Nullable=""false"" />
    <Parameter Name=""param1"" Type=""FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace.validEntityType1"" />
  </Function>
  <Function Name=""ValidFunction1"">
    <ReturnType Type=""Single"" Nullable=""false"" />
    <Parameter Name=""param1"" Type=""Single"" Nullable=""false"" />
  </Function>
</Schema>", @"<Schema Namespace=""FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace"" p1:UseStrongSpatialTypes=""false"" xmlns:p1=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityContainer Name=""vALIDeNTITYcONTAINER2"">
    <EntitySet Name=""AValidEntitySet2"" EntityType=""FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace.VALIDeNTITYtYPE2"" />
  </EntityContainer>
  <EntityType Name=""validEntityTypeA"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
    <NavigationProperty Name=""Mumble"" Type=""Collection(FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace.VALIDeNTITYtYPEB)"" />
  </EntityType>
  <EntityType Name=""VALIDeNTITYtYPEB"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
  </EntityType>
  <ComplexType Name=""ValidNameComplexTypeA"">
    <Property Name=""aPropertyOne"" Type=""FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace.ValidNameComplexTypeA"" Nullable=""false"" />
  </ComplexType>
  <Function Name=""ValidFunctionA"">
    <ReturnType Type=""Single"" Nullable=""false"" />
  </Function>
  <Function Name=""ValidFunctionA"">
    <ReturnType Type=""Single"" Nullable=""false"" />
    <Parameter Name=""param1"" Type=""FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace.validEntityTypeA"" />
  </Function>
  <Function Name=""ValidFunctionA"">
    <ReturnType Type=""Single"" Nullable=""false"" />
    <Parameter Name=""param1"" Type=""Single"" Nullable=""false"" />
  </Function>
</Schema>",
                #endregion
            };

            return csdl.Select(XElement.Parse);
        }

        public static IEnumerable<XElement> MultipleSchemasWithDerivedTypes(EdmVersion csdlVersion) // M4
        {
            var csdl = new[]
            {
               @"<Schema Namespace=""FindMethodsTestModelBuilder.MultipleSchemasWithUsings.first"" p1:UseStrongSpatialTypes=""false"" xmlns:p1=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""validEntityType1"">
    <Key>
      <PropertyRef Name=""Id"" />
    </Key>
    <Property Name=""Id"" Type=""Int32"" Nullable=""false"" />
  </EntityType>
  <ComplexType Name=""V1alidcomplexType"">
    <Property Name=""aPropertyOne"" Type=""Int32"" Nullable=""false"" />
  </ComplexType>
</Schema>",
                @"<Schema Namespace=""FindMethodsTestModelBuilder.MultipleSchemasWithUsings.second"" p1:UseStrongSpatialTypes=""false"" xmlns:p1=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""VALIDeNTITYtYPE2"" BaseType=""FindMethodsTestModelBuilder.MultipleSchemasWithUsings.first.validEntityType1"" />
  <ComplexType Name=""V1alidcomplexType"">
    <Property Name=""aPropertyOne"" Type=""FindMethodsTestModelBuilder.MultipleSchemasWithUsings.first.V1alidcomplexType"" Nullable=""false"" />
  </ComplexType>
  <EntityContainer Name=""ValidEntityContainer1"" />
</Schema>",
                @"<Schema Namespace=""FindMethodsTestModelBuilder.MultipleSchemasWithUsings.third"" p1:UseStrongSpatialTypes=""false"" xmlns:p1=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""VALIDeNTITYtYPE1"" BaseType=""FindMethodsTestModelBuilder.MultipleSchemasWithUsings.second.VALIDeNTITYtYPE2"" />
  <ComplexType Name=""V1alidcomplexType"">
    <Property Name=""aPropertyOne"" Type=""FindMethodsTestModelBuilder.MultipleSchemasWithUsings.second.V1alidcomplexType"" Nullable=""false"" />
  </ComplexType>
</Schema>",
            };

            return csdl.Select(XElement.Parse);
        }

        [ValidationTestInvalidModel]
        public static IEnumerable<XElement> MultipleSchemasWithDifferentNamespacesCyclicInvalid() // M9
        {
            var csdl = new[]
            {
                @"<Schema Namespace=""FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesCyclicInvalid.third"" p1:UseStrongSpatialTypes=""false"" xmlns:p1=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""VALIDeNTITYtYPE1"" BaseType=""FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesCyclicInvalid.second.VALIDeNTITYtYPE2"" />
  <ComplexType Name=""V1alidcomplexType"">
    <Property Name=""aPropertyOne"" Type=""FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesCyclicInvalid.second.V1alidcomplexType"" Nullable=""false"" />
  </ComplexType>
</Schema>",
                @"<Schema Namespace=""FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesCyclicInvalid.second"" p1:UseStrongSpatialTypes=""false"" xmlns:p1=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""VALIDeNTITYtYPE2"" BaseType=""FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesCyclicInvalid.first.validEntityType1"" />
  <ComplexType Name=""V1alidcomplexType"">
    <Property Name=""aPropertyOne"" Type=""FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesCyclicInvalid.first.V1alidcomplexType"" Nullable=""false"" />
  </ComplexType>
</Schema>",
                @"<Schema Namespace=""FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesCyclicInvalid.first"" p1:UseStrongSpatialTypes=""false"" xmlns:p1=""http://schemas.microsoft.com/ado/2009/02/edm/annotation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityType Name=""validEntityType1"" BaseType=""FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesCyclicInvalid.third.VALIDeNTITYtYPE1"" />
  <ComplexType Name=""V1alidcomplexType"">
    <Property Name=""aPropertyOne"" Type=""FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesCyclicInvalid.third.V1alidcomplexType"" Nullable=""false"" />
  </ComplexType>
</Schema>",
            };

            return csdl.Select(XElement.Parse);
        }

        public static IEnumerable<XElement> TermAndFunctionCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""Note"" Type=""Edm.String"" />
    <Term Name=""NotePerson"" Type=""DefaultNamespace.Person"" />
    <Term Name=""NoteSimpleType"" Type=""DefaultNamespace.SimpleType"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <ComplexType Name=""SimpleType"">
      <Property Name=""Data"" Type=""Edm.String"" />
    </ComplexType>
    <Function Name=""SimpleFunction""><ReturnType Type=""Edm.String""/>
        <Parameter Name=""Person"" Type=""Edm.String"" />
    </Function>
</Schema>");
        }


        public static IEnumerable<XElement> FunctionOverloadingWithDifferentParametersCountCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Function Name=""SimpleFunction""><ReturnType Type=""Edm.String""/>
        <Parameter Name=""Person"" Type=""Edm.String"" />
    </Function>
    <Function Name=""SimpleFunction""><ReturnType Type=""Edm.String""/>
        <Parameter Name=""Person"" Type=""Edm.String"" />
        <Parameter Name=""Count"" Type=""Edm.Int32"" />
    </Function>
</Schema>");
        }

        public static IEnumerable<XElement> FunctionImportOverloadingWithDifferentParametersCountCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <Function Name=""SimpleFunction""><ReturnType Type=""Edm.String""/>
    <Parameter Name=""Person"" Type=""Edm.Int32"" />
  </Function>
  <Function Name=""SimpleFunction""><ReturnType Type=""Edm.String""/>
    <Parameter Name=""Person"" Type=""Edm.Int32"" />
    <Parameter Name=""Count"" Type=""Edm.Int32"" />
  </Function>
  <EntityContainer Name=""Container"">
    <FunctionImport Name=""SimpleFunction"" Function=""DefaultNamespace.SimpleFunction"" />
  </EntityContainer>
</Schema>");
        }

        public static IEnumerable<XElement> FunctionImportOverloadingWithComplexParameterCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name='MyEntityType'>
        <Key>
          <PropertyRef Name='Id' />
        </Key>
        <Property Name='Id' Type='Edm.Int32' Nullable='false' />
      </EntityType>
      <ComplexType Name='MyComplexType'>
        <Property Name=""Data"" Type=""Edm.String"" />
      </ComplexType>
      <Function Name='MyFunction'><ReturnType Type='Edm.String'/>
        <Parameter Name='P1' Type='Collection(DefaultNamespace.MyComplexType)' />
        <Parameter Name='P2' Type='DefaultNamespace.MyEntityType' />
        <Parameter Name='P3' Type='Collection(DefaultNamespace.MyEntityType)' />
        <Parameter Name='P4' Type='Edm.Int32' />
      </Function>
      <Function Name='MyFunction'><ReturnType Type='Edm.String'/>
        <Parameter Name='P1' Type='Collection(DefaultNamespace.MyComplexType)' />
        <Parameter Name='P2' Type='DefaultNamespace.MyEntityType' />
        <Parameter Name='P3b' Type='DefaultNamespace.MyComplexType' />
        <Parameter Name='P4' Type='Edm.Int32' />
      </Function>
      <EntityContainer Name='Container'>
        <FunctionImport Name='MyFunction' Function='DefaultNamespace.MyFunction' />
      </EntityContainer>
</Schema>");
        }

        public static IEnumerable<XElement> EntitySetWithSingleNavigationCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""Container"">
        <EntitySet Name=""BuyerSet"" EntityType=""DefaultNamespace.Person"">
          <NavigationPropertyBinding Path=""ItemPurchased"" Target=""ItemSet"" />
        </EntitySet>
        <EntitySet Name=""ItemSet"" EntityType=""DefaultNamespace.Item"">
          <NavigationPropertyBinding Path=""Purchaser"" Target=""BuyerSet"" />
        </EntitySet>
    </EntityContainer>
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""ItemPurchased"" Type=""DefaultNamespace.Item"" Nullable=""false"" Partner=""Purchaser"" />
    </EntityType>
    <EntityType Name=""Item"">
        <Key>
            <PropertyRef Name=""PersonId"" />
        </Key>
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""Purchaser"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ItemPurchased"" />
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> EntitySetNavigationUsedTwiceCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""Container"">
        <EntitySet Name=""BuyerSet"" EntityType=""DefaultNamespace.Person"">
          <NavigationPropertyBinding Path=""ItemPurchased"" Target=""ItemSet"" />
        </EntitySet>
        <EntitySet Name=""ItemSet"" EntityType=""DefaultNamespace.Item"">
          <NavigationPropertyBinding Path=""Purchaser"" Target=""BuyerSet"" />
        </EntitySet>
        <EntitySet Name=""SecondBuyerSet"" EntityType=""DefaultNamespace.Person"">
          <NavigationPropertyBinding Path=""ItemPurchased"" Target=""SecondItemSet"" />
        </EntitySet>
        <EntitySet Name=""SecondItemSet"" EntityType=""DefaultNamespace.Item"">
          <NavigationPropertyBinding Path=""Purchaser"" Target=""SecondBuyerSet"" />
        </EntitySet>
    </EntityContainer>
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""ItemPurchased"" Type=""DefaultNamespace.Item"" Nullable=""false"" Partner=""Purchaser"" />
    </EntityType>
    <EntityType Name=""Item"">
        <Key>
            <PropertyRef Name=""PersonId"" />
        </Key>
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""Purchaser"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ItemPurchased"" />
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> EntitySetWithTwoNavigationCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""Container"">
        <EntitySet Name=""BuyerSet"" EntityType=""DefaultNamespace.Person"">
          <NavigationPropertyBinding Path=""ItemPurchased"" Target=""ItemSet"" />
          <NavigationPropertyBinding Path=""Pet"" Target=""PetSet"" />
        </EntitySet>
        <EntitySet Name=""ItemSet"" EntityType=""DefaultNamespace.Item"">
          <NavigationPropertyBinding Path=""Purchaser"" Target=""BuyerSet"" />
        </EntitySet>
        <EntitySet Name=""PetSet"" EntityType=""DefaultNamespace.Pet"">
          <NavigationPropertyBinding Path=""Owner"" Target=""BuyerSet"" />
        </EntitySet>
    </EntityContainer>
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""ItemPurchased"" Type=""DefaultNamespace.Item"" Nullable=""false"" Partner=""Purchaser"" />
        <NavigationProperty Name=""Pet"" Type=""DefaultNamespace.Pet"" Nullable=""false"" Partner=""Owner"" />
    </EntityType>
    <EntityType Name=""Item"">
        <Key>
            <PropertyRef Name=""PersonId"" />
        </Key>
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""Purchaser"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ItemPurchased"" />
    </EntityType>
    <EntityType Name=""Pet"">
        <Key>
            <PropertyRef Name=""PersonId"" />
        </Key>
        <Property Name=""PersonId"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""Owner"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""Pet"" />
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> EntitySetRecursiveNavigationCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""Container"">
        <EntitySet Name=""PersonSet"" EntityType=""DefaultNamespace.Person"">
          <NavigationPropertyBinding Path=""ToPerson"" Target=""PersonSet"" />
          <NavigationPropertyBinding Path=""ToFriend"" Target=""PersonSet"" />
        </EntitySet>
    </EntityContainer>
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""ToFriend"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToPerson"" />
        <NavigationProperty Name=""ToPerson"" Type=""DefaultNamespace.Person"" Nullable=""false"" Partner=""ToFriend"" />
    </EntityType>
</Schema>");
        }

        public static IEnumerable<XElement> SimpleTypesCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name='MyEntityType'>
        <Key>
            <PropertyRef Name='Id' />
        </Key>
        <Property Name='Id' Type='Int32' Nullable='false' />
    </EntityType>
    <ComplexType Name='MyComplexType'>
        <Property Name='Data' Type='Edm.String' />
    </ComplexType>
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest]
        [ValidationTestInvalidModelAttribute] // BadUnresolvedProperty 'FakeId'
        public static IEnumerable<XElement> FindPropertyBindingAnnotationCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Pet"">
        <Property Name=""OwnerId"" Type=""Edm.Int32"" />
        <Property Name=""Name"" Type=""Edm.String"" />
    </ComplexType>
    <Annotations Target=""DefaultNamespace.Pet"">
        <Annotation Term=""AnnotationNamespace.AddressObject"">
            <Record>
                <PropertyValue Property=""Street"" String=""foo"" />
                <PropertyValue Property=""City"" String=""bar"" />
            </Record>
        </Annotation>
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""AddressObject"" Type=""AnnotationNamespace.Address"" />
    <EntityType Name=""PersonAddress"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Postal"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""Address"" Type=""AnnotationNamespace.Address"" />
    </EntityType>
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""FakeId"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest, ValidationTestInvalidModel]
        public static IEnumerable<XElement> FindVocabularyAnnotationWithTermCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Pet"">
        <Property Name=""OwnerId"" Type=""Edm.Int32"" />
        <Property Name=""Name"" Type=""Edm.String"" />
    </ComplexType>
    <Annotations Target=""DefaultNamespace.Pet"">
        <Annotation Term=""AnnotationNamespace.AddressObject"">
        </Annotation>
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""AddressObject"" Type=""AnnotationNamespace.Address"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""FakeId"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest, ValidationTestInvalidModel]
        public static IEnumerable<XElement> FindVocabularyAnnotationWithEntityTypeCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Pet"">
        <Property Name=""OwnerId"" Type=""Edm.Int32"" />
        <Property Name=""Name"" Type=""Edm.String"" />
    </ComplexType>
    <Annotations Target=""DefaultNamespace.Pet"">
        <Annotation Term=""AnnotationNamespace.Person"">
        </Annotation>
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""AddressObject"" Type=""AnnotationNamespace.Address"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""FakeId"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest, ValidationTestInvalidModel]
        public static IEnumerable<XElement> FindVocabularyAnnotationWithComplexTypeCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Pet"">
        <Property Name=""OwnerId"" Type=""Edm.Int32"" />
        <Property Name=""Name"" Type=""Edm.String"" />
    </ComplexType>
    <Annotations Target=""DefaultNamespace.Pet"">
        <Annotation Term=""AnnotationNamespace.Address"">
        </Annotation>
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""AddressObject"" Type=""AnnotationNamespace.Address"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""FakeId"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest, ValidationTestInvalidModel]
        public static IEnumerable<XElement> FindVocabularyAnnotationWithMultipleTermsCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Pet"">
        <Property Name=""OwnerId"" Type=""Edm.Int32"" />
        <Property Name=""Name"" Type=""Edm.String"" />
    </ComplexType>
    <Annotations Target=""DefaultNamespace.Pet"">
        <Annotation Term=""AnnotationNamespace.Address"">
        </Annotation>
        <Annotation Term=""AnnotationNamespace.AddressObject"">
        </Annotation>
        <Annotation Term=""AnnotationNamespace.AddressObject"">
        </Annotation>
    </Annotations>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""AddressObject"" Type=""AnnotationNamespace.Address"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""FakeId"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
</Schema>");
        }

        [CustomCsdlSchemaCompliantTest, ValidationTestInvalidModel]
        public static IEnumerable<XElement> FindVocabularyAnnotationAcrossModelOutOfLineAnnotationCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""ValueAnnotationType"">
    </ComplexType>
    <Annotations Target=""DefaultNamespace.ValueAnnotationType"">
        <Annotation Term=""DefaultNamespace.ValueTermInModel"">
        </Annotation>
        <Annotation Term=""AnnotationNamespace.ValueTermOutOfModel"">
        </Annotation>
        <Annotation Term=""fooNamespace.ValueTermDoesNotExist"">
        </Annotation>
    </Annotations>
    <Term Name=""ValueTermInModel"" Type=""Edm.String"" />
    <EntityType Name=""PersonInMode"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ValueTermOutOfModel"" Type=""Edm.String"" />
    <EntityType Name=""PersonOutOfMode"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
</Schema>");
        }

        [ValidationTestInvalidModel]
        public static IEnumerable<XElement> FindVocabularyAnnotationAcrossModelAnnotationCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""ContainerThree"">
        <Annotation Term=""DefaultNamespace.TermThree"" String=""33"" />
    </EntityContainer>
    <Term Name=""TermThree"" Type=""Edm.String"" />
    <Annotations Target=""DefaultNamespace.ContainerTwo"">
        <Annotation Term=""DefaultNamespace.TermTwo"" String=""22"" />
    </Annotations>
</Schema>");
        }

        public static EdmModel FindVocabularyAnnotationAcrossModelAnnotationModel()
        {
            var model = new EdmModel();

            var containerOne = new EdmEntityContainer("DefaultNamespace", "ContainerOne");
            model.AddElement(containerOne);

            var termOne = new EdmTerm("DefaultNamespace", "TermOne", EdmCoreModel.Instance.GetString(true));
            model.AddElement(termOne);
            var termTwo = new EdmTerm("DefaultNamespace", "TermTwo", EdmCoreModel.Instance.GetString(true));
            model.AddElement(termTwo);

            var valueAnnotationOne = new EdmVocabularyAnnotation(
                containerOne,
                termOne,
                new EdmStringConstant("1"));
            valueAnnotationOne.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);
            model.AddVocabularyAnnotation(valueAnnotationOne);

            return model;
        }

        [CustomCsdlSchemaCompliantTest, ValidationTestInvalidModel]
        public static IEnumerable<XElement> FindVocabularyAnnotationAcrossModelInLineAnnotationCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""ValueAnnotationType"">
        <Annotation Term=""DefaultNamespace.ValueTermInModel"">
        </Annotation>
        <Annotation Term=""AnnotationNamespace.ValueTermOutOfModel"">
        </Annotation>
        <Annotation Term=""fooNamespace.ValueTermDoesNotExist"">
        </Annotation>
    </ComplexType>
    <Term Name=""ValueTermInModel"" Type=""Edm.String"" />
    <EntityType Name=""PersonInMode"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ValueTermOutOfModel"" Type=""Edm.String"" />
    <EntityType Name=""PersonOutOfMode"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
</Schema>");
        }

        [ValidationTestInvalidModel]
        public static IEnumerable<XElement> FindTermCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ValueTermInModel"" Type=""Edm.String"" />
    <Term Name=""AmbigousValueTerm"" Type=""Edm.String"" />
    <Term Name=""AmbigousValueTerm"" Type=""Edm.String"" />
    <Term Name=""ReferenceAmbigousValueTerm"" Type=""Edm.String"" />
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""ValueTerm"" Type=""Edm.String"" />
</Schema>");
        }

        public static EdmModel FindTermModel()
        {
            var model = new EdmModel();

            var secondValueTerm = new EdmTerm("DefaultNamespace", "SecondValueTermInModel", EdmCoreModel.Instance.GetString(true));
            model.AddElement(secondValueTerm);

            var referenceAmbigous = new EdmTerm("DefaultNamespace", "ReferenceAmbigousValueTerm", EdmCoreModel.Instance.GetString(true));
            model.AddElement(referenceAmbigous);

            return model;
        }

        [ValidationTestInvalidModel]
        public static IEnumerable<XElement> FindTypeComplexTypeCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""SimpleType"" />
    <ComplexType Name=""AmbiguousType"" />
    <ComplexType Name=""AmbiguousType"" />
    <ComplexType Name=""ReferenceAmbiguousType"" />
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""SimpleType"" />
</Schema>");
        }

        [ValidationTestInvalidModel]
        public static IEnumerable<XElement> FindTypeEntityTypeCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""SimpleEntity"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""AmbiguousEntity"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""AmbiguousEntity"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <EntityType Name=""ReferenceAmbiguousEntity"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityType Name=""SimpleEntity"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
</Schema>");
        }

        [ValidationTestInvalidModel]
        public static IEnumerable<XElement> FindTypeTypeDefinitionCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <TypeDefinition Name=""Length"" UnderlyingType=""Edm.String"" />
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <TypeDefinition Name=""Length"" UnderlyingType=""Edm.Int32"" />
</Schema>");
        }

        public static EdmModel FindTypeModel()
        {
            var model = new EdmModel();

            var secondSimpleType = new EdmComplexType("DefaultNamespace", "SecondSimpleType");
            model.AddElement(secondSimpleType);

            var referenceAmbiguousType = new EdmComplexType("DefaultNamespace", "ReferenceAmbiguousType");
            model.AddElement(referenceAmbiguousType);

            var secondSimpleEntity = new EdmEntityType("DefaultNamespace", "SecondSimpleEntity");
            var secondSimpleEntityId = secondSimpleEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(secondSimpleEntity);

            var referenceAmbiguousEntity = new EdmEntityType("DefaultNamespace", "ReferenceAmbiguousEntity");
            var referenceAmbiguousEntityId = referenceAmbiguousEntity.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(referenceAmbiguousEntity);

            return model;
        }

        [ValidationTestInvalidModel]
        public static IEnumerable<XElement> FindFunctionAcrossModelsCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Function Name=""SimpleFunction""><ReturnType Type=""Edm.Int32"" /></Function>
    <Function Name=""AmbiguousFunction""><ReturnType Type=""Edm.Int32"" /></Function>
    <Function Name=""AmbiguousFunction""><ReturnType Type=""Edm.Int32"" /></Function>
    <Function Name=""ReferenceAmbiguousFunction""><ReturnType Type=""Edm.Int32"" /></Function>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Function Name=""SimpleFunction""><ReturnType Type=""Edm.Int32"" /></Function>
</Schema>");
        }

        public static EdmModel FindFunctionAcrossModelsModel()
        {
            var model = new EdmModel();

            var simpleFunction = new EdmFunction("DefaultNamespace", "SecondSimpleFunction", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(simpleFunction);

            var referenceAmbigiousAction = new EdmFunction("DefaultNamespace", "ReferenceAmbiguousFunction", EdmCoreModel.Instance.GetInt32(true));
            model.AddElement(referenceAmbigiousAction);

            return model;
        }

        [ValidationTestInvalidModel]
        public static IEnumerable<XElement> FindEntityContainerCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""Default"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <EntityContainer Name=""SimpleContainer"" />
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
</Schema>");
        }

        [ValidationTestInvalidModel]
        public static IEnumerable<XElement> FindVocabularyAnnotationsIncludingInheritedAnnotationsCsdl()
        {
            return ConvertCsdlsToXElements(@"
<Schema Namespace=""DefaultNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <ComplexType Name=""Pet"" BaseType=""foo.Animal"">
        <Property Name=""OwnerId"" Type=""Edm.Int32"" />
        <Property Name=""Name"" Type=""Edm.String"" />
    </ComplexType>
    <Annotations Target=""Unknown.UnresolvedTypeBase"">
        <Annotation Term=""AnnotationNamespace.AddressObject"">
        </Annotation>
    </Annotations>
    <Annotations Target=""DefaultNamespace.Pet"">
        <Annotation Term=""AnnotationNamespace.AddressObject"">
        </Annotation>
    </Annotations>
    <EntityType Name=""Child"" BaseType=""AnnotationNamespace.Person"" />
    <EntityType Name=""UnresolvedType"" BaseType=""Unknown.UnresolvedTypeBase""/>
</Schema>", @"
<Schema Namespace=""AnnotationNamespace"" Alias=""foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
    <Term Name=""AddressObject"" Type=""Edm.Int32"" />
    <Term Name=""Habitation"" Type=""Edm.Int32"" />
    <EntityType Name=""Person"">
        <Key>
            <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
    </EntityType>
    <ComplexType Name=""Animal"" BaseType=""AnnotationNamespace.Life"">
        <Property Name=""Gender"" Type=""Edm.String"" Nullable=""false"" />
    </ComplexType>
    <ComplexType Name=""Address"">
        <Property Name=""Street"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""City"" Type=""Edm.String"" Nullable=""true"" />
    </ComplexType>
    <Annotations Target=""AnnotationNamespace.Animal"">
        <Annotation Term=""AnnotationNamespace.Habitation"" Int=""5"">
        </Annotation>
    </Annotations>
    <Annotations Target=""AnnotationNamespace.Life"">
        <Annotation Term=""AnnotationNamespace.Unknown"" Int=""5"">
        </Annotation>
    </Annotations>
    <Annotations Target=""AnnotationNamespace.Derived"">
        <Annotation Term=""AnnotationNamespace.Unknown"" Int=""5"">
        </Annotation>
    </Annotations>
    <ComplexType Name=""Derived"" BaseType=""AnnotationNamespace.Base"">
    </ComplexType>
    <ComplexType Name=""Base"" BaseType=""AnnotationNamespace.Derived"">
        <Annotation Term=""AnnotationNamespace.Habitation"" Int=""5"">
        </Annotation>
    </ComplexType>
    <ComplexType Name=""ComplexTypeWithEntityTypeBase"" BaseType=""DefaultNamespace.Child"">
        <Annotation Term=""AnnotationNamespace.Habitation"" Int=""5"">
        </Annotation>
    </ComplexType>
</Schema>");
        }

        public static EdmModel FindEntityContainerModel()
        {
            var model = new EdmModel();

            var secondSimpleContainer = new EdmEntityContainer("", "SecondSimpleContainer");
            model.AddElement(secondSimpleContainer);

            return model;
        }

        private static IEnumerable<XElement> ConvertCsdlsToXElements(params string[] csdls)
        {
            return csdls.Select(e => XElement.Parse(e, LoadOptions.SetLineInfo));
        }
    }
}

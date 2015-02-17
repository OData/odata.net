//---------------------------------------------------------------------
// <copyright file="AnnotationTestModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Evaluation
{
    using System.Collections.Generic;
    using System.Xml.Linq;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Library.Values;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Values;

    internal static class AnnotationTestModel
    {
        private const string ModelCsdl =
            @"<Schema Namespace=""TestModel"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <EntityContainer Name=""WithAnnotations"">
    <EntitySet Name=""Customers"" EntityType=""TestModel.Customer"">
      <NavigationPropertyBinding Path=""Orders"" Target=""Orders"">
    </EntitySet>
    <EntitySet Name=""Orders"" EntityType=""TestModel.Orders"">
      <NavigationPropertyBinding Path=""Customer"" Target=""Customers"">
    </EntitySet>

    <FunctionImport Name=""CallOnCustomer"" IsBindable=""true"">
      <Parameter Name=""Customer"" Type=""TestModel.Customer"" />
    </FunctionImport>
    <FunctionImport Name=""CallOnOrder"" IsBindable=""true"">
      <Parameter Name=""Order"" Type=""TestModel.Orders"" />
    </FunctionImport>
  </EntityContainer>

<EntityContainer Name=""WithoutAnnotations"">
    <EntitySet Name=""Customers"" EntityType=""TestModel.Customer"" />
    
    <FunctionImport Name=""CallOnCustomer"" IsBindable=""true"">
      <Parameter Name=""Customer"" Type=""TestModel.Customer"" />
    </FunctionImport>
  </EntityContainer>

  <EntityType Name=""Customer"">
    <Key>
      <PropertyRef Name=""ID"" />
    </Key>
    <Property Name=""ID"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""Name"" Type=""String"" />
    <Property Name=""Image"" Type=""Stream"" />
    <NavigationProperty Name=""Orders"" ToRole=""Order_Customer"" FromRole=""Customer_Orders"" Relationship=""TestModel.Customer_Orders"" />
  </EntityType>

  <EntityType Name=""Orders"">
    <Key>
      <PropertyRef Name=""KeyA"" />
      <PropertyRef Name=""KeyB"" />
    </Key>
    <Property Name=""KeyA"" Type=""Int32"" Nullable=""false"" />
    <Property Name=""KeyB"" Type=""String"" Nullable=""false"" />
    <Property Name=""Image"" Type=""Stream"" />
    <NavigationProperty Name=""Customer"" ToRole=""Customer_Orders"" FromRole=""Order_Customer"" Relationship=""TestModel.Customer_Orders"" />
  </EntityType>

  <Association Name=""Customer_Orders"">
    <End Type=""TestModel.Customer"" Multiplicity=""0..1"" Role=""Customer_Orders"" />
    <End Type=""TestModel.Orders"" Multiplicity=""*"" Role=""Order_Customer"" />
  </Association>

  <Annotations Target='TestModel.WithAnnotations'>
    <Annotation Term='OData.BaseUri' String='http://odata.org/service/' />
  </Annotations>

  <Annotations Target='TestModel.WithAnnotations/Customers'>
    <Annotation Term='OData.EntitySetUri' String='GetSomeCustomers/' />
    <Annotation Term='OData.EntitySetUriSuffix' String='CustomerKeyLookup/' />
    <Annotation Term='OData.NavigationLinkUri' Qualifier='Orders' String='_Orders_Navigation/' />
    <Annotation Term='OData.AssociationLinkUri' Qualifier='Orders' String='_Orders_Association/' />
    <Annotation Term='OData.OperationTargetUri' Qualifier='TestModel.WithAnnotations/CallOnCustomer' String='_Customers_Call/' />
    <Annotation Term='OData.StreamEditLinkUri' Qualifier='Image' String='_CustomerImage_EditLink/' />
    <Annotation Term='OData.StreamReadLinkUri' Qualifier='Image' String='_CustomerImage_ReadLink/' />
    <Annotation Term='OData.ETag' String='_Customer_ETag_' />
    <Annotation Term='OData.Id' String='_Customer_ID_' />
    <Annotation Term='OData.OperationTitle' Qualifier='TestModel.WithAnnotations/CallOnCustomer' String='_Call_On_Customer_' />
    <Annotation Term='OData.StreamContentType' Qualifier='Image' String='_Image_Content_Type_' />
    <Annotation Term='OData.StreamETag' Qualifier='Image' String='_Image_ETag_' />
  </Annotations>

  <Annotations Target='TestModel.WithAnnotations/Orders'>
    <Annotation Term='OData.EntitySetUri' String='http://fake.org/_Orders' />
    <Annotation Term='OData.NavigationLinkUri' Qualifier='Customer' String='http://fake.org/_Customer_Navigation/' />
    <Annotation Term='OData.AssociationLinkUri' Qualifier='Customer' String='http://fake.org/_Customer_Association' />
    <Annotation Term='OData.OperationTargetUri' Qualifier='TestModel.WithAnnotations/CallOnOrder' String='http://fake.org/_Orders_Call/' />
    <Annotation Term='OData.StreamEditLinkUri' Qualifier='Image' String='http://fake.org/_OrderImage_EditLink' />
    <Annotation Term='OData.StreamReadLinkUri' Qualifier='Image' String='http://fake.org/_OrderImage_ReadLink/' />
  </Annotations>
</Schema>";

        static AnnotationTestModel()
        {
            IEnumerable<EdmError> errors;

            var parsedModel = XElement.Parse(ModelCsdl);

            IEdmModel model;
            bool parsed = CsdlReader.TryParse(new[] { parsedModel.CreateReader() }, new IEdmModel[0], out model, out errors);
            parsed.Should().BeTrue();

            Model = model;
            ContainerWithAnnotations = model.FindEntityContainer("TestModel.WithAnnotations");
            ContainerWithoutAnnotations = model.FindEntityContainer("TestModel.WithoutAnnotations");

            var customerValues = new Dictionary<string, IEdmValue>
            {
                { "ID", new EdmIntegerConstant(null, 1) },
            };
            Customer = new EdmStructuredValueSimulator((IEdmEntityType)model.FindDeclaredType("TestModel.Customer"), customerValues);

            var orderValue = new Dictionary<string, IEdmValue>
            {
                { "KeyA", new EdmIntegerConstant(null, 1) },
                { "KeyB", new EdmStringConstant(null, "foo") },
            };
            Order = new EdmStructuredValueSimulator((IEdmEntityType)model.FindDeclaredType("TestModel.Orders"), orderValue);
        }

        public static IEdmModel Model { get; private set; }

        public static IEdmEntityContainer ContainerWithAnnotations { get; private set; }

        public static IEdmEntityContainer ContainerWithoutAnnotations { get; private set; }

        public static IEdmStructuredValue Customer { get; private set; }

        public static IEdmStructuredValue Order { get; private set; }
    }
}
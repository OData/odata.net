//---------------------------------------------------------------------
// <copyright file="CsdlReaderTests.TargetPath.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl
{
    public partial class CsdlReaderTests
    {
        [Fact]
        public void ParsingEntitySetPropertyOutOfLineAnnotationsWorks()
        {
            // Test for MySchema.MyEntityContainer/MyEntitySet/MyProperty

            string types =
@"<Term Name=""MyTerm"" Type=""Edm.String""/>
<Annotations Target=""NS.Default/Customers/Name"" >
  <Annotation Term=""NS.MyTerm"" String=""Name OutOfLine MyTerm Value"" />
</Annotations>";

            string properties =
                @"<Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />";

            IEdmModel model = GetModel(types: types, properties: properties);
            Assert.NotNull(model);

            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors), String.Format("Errors in validating model. {0}", String.Concat(errors.Select(e => e.ErrorMessage))));

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);

            IEdmTerm myTerm = model.FindDeclaredTerm("NS.MyTerm");
            Assert.NotNull(myTerm);

            // Name
            IEdmProperty nameProperty = customer.DeclaredProperties.Where(x => x.Name == "Name").FirstOrDefault();
            Assert.NotNull(nameProperty);

            IEdmTargetPath targetPath = model.GetTargetPath("NS.Default/Customers/Name");

            string nameAnnotation = GetAnnotation(model, targetPath, myTerm, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            Assert.Equal("Name OutOfLine MyTerm Value", nameAnnotation);
        }

        [Fact]
        public void ParsingEntitySetNavigationPropertyOutOfLineAnnotationsWorks()
        {
            // Test for MySchema.MyEntityContainer/MyEntitySet/MyNavigationProperty

            string types =
@"<EntityType Name=""Order"">
    <Key>
        <PropertyRef Name=""OrderId""/>
    </Key>
    <Property Name=""OrderId"" Type=""Edm.String"" Nullable=""false""/>
    <Property Name=""OrderName"" Type=""Edm.String"" Nullable=""false""/>
</EntityType>
<Term Name=""MyTerm"" Type=""Edm.String""/>
<Annotations Target=""NS.Default/Customers/Orders"" >
  <Annotation Term=""NS.MyTerm"" String=""Name OutOfLine MyTerm Value"" />
</Annotations>";

            string properties =
                @"<NavigationProperty Name=""Orders"" Type=""Collection(NS.Order)"" ContainsTarget=""true"" />";

            IEdmModel model = GetModel(types: types, properties: properties);
            Assert.NotNull(model);

            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors), String.Format("Errors in validating model. {0}", String.Concat(errors.Select(e => e.ErrorMessage))));

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);

            IEdmTerm myTerm = model.FindDeclaredTerm("NS.MyTerm");
            Assert.NotNull(myTerm);

            // Name
            IEdmProperty nameProperty = customer.DeclaredNavigationProperties().Where(x => x.Name == "Orders").FirstOrDefault();
            Assert.NotNull(nameProperty);

            IEdmTargetPath targetPath = model.GetTargetPath("NS.Default/Customers/Orders");

            string nameAnnotation = GetAnnotation(model, targetPath, myTerm, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            Assert.Equal("Name OutOfLine MyTerm Value", nameAnnotation);
        }

        [Fact]
        public void ParsingEntitySetNavigationPropertyOnComplexTypeOutOfLineAnnotationsWorks()
        {
            // Test for MySchema.MyEntityContainer/MyEntitySet/MyComplexProperty/MyNavigationProperty

            string types =
@"<EntityType Name=""City"">
    <Key>
        <PropertyRef Name=""Name""/>
    </Key>
    <Property Name=""Name"" Nullable=""false"" Type=""Edm.String""/>
</EntityType>
<ComplexType Name=""Address"">
    <Property Name=""Road"" Type=""Edm.String"" Nullable=""false""/>
    <NavigationProperty Name=""City"" Type=""NS.City"" Nullable=""false"" ContainsTarget=""true"" />
</ComplexType>
<Term Name=""MyTerm"" Type=""Edm.String""/>
<Annotations Target=""NS.Default/Customers/Address/City"" >
  <Annotation Term=""NS.MyTerm"" String=""Name OutOfLine MyTerm Value"" />
</Annotations>";

            string properties =
                @"<Property Name=""Address"" Type=""NS.Address"" />";

            IEdmModel model = GetModel(types: types, properties: properties);
            Assert.NotNull(model);

            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors), String.Format("Errors in validating model. {0}", String.Concat(errors.Select(e => e.ErrorMessage))));

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);

            IEdmTerm myTerm = model.FindDeclaredTerm("NS.MyTerm");
            Assert.NotNull(myTerm);

            // Name
            IEdmProperty nameProperty = customer.DeclaredProperties.Where(x => x.Name == "Address").FirstOrDefault();
            Assert.NotNull(nameProperty);

            IEdmTargetPath targetPath = model.GetTargetPath("NS.Default/Customers/Address/City");

            string nameAnnotation = GetAnnotation(model, targetPath, myTerm, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            Assert.Equal("Name OutOfLine MyTerm Value", nameAnnotation);
        }

        [Fact]
        public void ParsingEntitySetNavigationPropertySubPropertyOnComplexTypeOutOfLineAnnotationsWorks()
        {
            // Test for MySchema.MyEntityContainer/MyEntitySet/MyComplexProperty/MyNavigationProperty/MyProperty

            string types =
@"<EntityType Name=""City"">
    <Key>
        <PropertyRef Name=""Name""/>
    </Key>
    <Property Name=""Name"" Nullable=""false"" Type=""Edm.String""/>
</EntityType>
<ComplexType Name=""Address"">
    <Property Name=""Road"" Type=""Edm.String"" Nullable=""false""/>
    <NavigationProperty Name=""City"" Type=""NS.City"" Nullable=""false"" ContainsTarget=""true"" />
</ComplexType>
<Term Name=""MyTerm"" Type=""Edm.String""/>
<Annotations Target=""NS.Default/Customers/Address/City/Name"" >
  <Annotation Term=""NS.MyTerm"" String=""Name OutOfLine MyTerm Value"" />
</Annotations>";

            string properties =
                @"<Property Name=""Address"" Type=""NS.Address"" />";

            IEdmModel model = GetModel(types: types, properties: properties);
            Assert.NotNull(model);

            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors), String.Format("Errors in validating model. {0}", String.Concat(errors.Select(e => e.ErrorMessage))));

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);

            IEdmTerm myTerm = model.FindDeclaredTerm("NS.MyTerm");
            Assert.NotNull(myTerm);

            // Name
            IEdmProperty nameProperty = customer.DeclaredProperties.Where(x => x.Name == "Address").FirstOrDefault();
            Assert.NotNull(nameProperty);

            IEdmTargetPath targetPath = model.GetTargetPath("NS.Default/Customers/Address/City/Name");

            string nameAnnotation = GetAnnotation(model, targetPath, myTerm, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            Assert.Equal("Name OutOfLine MyTerm Value", nameAnnotation);
        }

        [Fact]
        public void ParsingEntitySetPropertyOnComplexTypeOutOfLineAnnotationsWorks()
        {
            // Test for MySchema.MyEntityContainer/MyEntitySet/MyComplexProperty/MyProperty

            string types =
@"<EntityType Name=""City"">
    <Key>
        <PropertyRef Name=""Name""/>
    </Key>
    <Property Name=""Name"" Nullable=""false"" Type=""Edm.String""/>
</EntityType>
<ComplexType Name=""Address"">
    <Property Name=""Road"" Type=""Edm.String"" Nullable=""false""/>
</ComplexType>
<Term Name=""MyTerm"" Type=""Edm.String""/>
<Annotations Target=""NS.Default/Customers/Address/Road"" >
  <Annotation Term=""NS.MyTerm"" String=""Name OutOfLine MyTerm Value"" />
</Annotations>";

            string properties =
                @"<Property Name=""Address"" Type=""NS.Address"" />";

            IEdmModel model = GetModel(types: types, properties: properties);
            Assert.NotNull(model);

            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors), String.Format("Errors in validating model. {0}", String.Concat(errors.Select(e => e.ErrorMessage))));

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);

            IEdmTerm myTerm = model.FindDeclaredTerm("NS.MyTerm");
            Assert.NotNull(myTerm);

            // Name
            IEdmProperty nameProperty = customer.DeclaredProperties.Where(x => x.Name == "Address").FirstOrDefault();
            Assert.NotNull(nameProperty);

            IEdmTargetPath targetPath = model.GetTargetPath("NS.Default/Customers/Address/Road");

            string nameAnnotation = GetAnnotation(model, targetPath, myTerm, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            Assert.Equal("Name OutOfLine MyTerm Value", nameAnnotation);
        }

        [Fact]
        public void ParsingEntitySetPropertyOnDerivedComplexTypeCollectionOutOfLineAnnotationsWorks()
        {
            // Test for MySchema.MyEntityContainer/MyEntitySet/MyComplexCollectionProperty/MySchema.DerivedType/MyProperty

            string types =
@"<EntityType Name=""City"">
    <Key>
        <PropertyRef Name=""Name""/>
    </Key>
    <Property Name=""Name"" Nullable=""false"" Type=""Edm.String""/>
</EntityType>
<ComplexType Name=""Address"">
    <Property Name=""Road"" Type=""Edm.String"" Nullable=""false""/>
</ComplexType>
<ComplexType Name=""LocalAddress"" BaseType=""NS.Address"">
    <Property Name=""BuildingInfo"" Type=""Edm.String"" Nullable=""false""/>
</ComplexType>
<Term Name=""MyTerm"" Type=""Edm.String""/>
<Annotations Target=""NS.Default/Customers/AddressInfo/NS.LocalAddress/Road"" >
  <Annotation Term=""NS.MyTerm"" String=""Name OutOfLine MyTerm Value"" />
</Annotations>";

            string properties =
                @"<Property Name=""AddressInfo"" Type=""Collection(NS.Address)"" />";

            IEdmModel model = GetModel(types: types, properties: properties);
            Assert.NotNull(model);

            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors), String.Format("Errors in validating model. {0}", String.Concat(errors.Select(e => e.ErrorMessage))));

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);

            IEdmTerm myTerm = model.FindDeclaredTerm("NS.MyTerm");
            Assert.NotNull(myTerm);

            // Name
            IEdmProperty nameProperty = customer.DeclaredProperties.Where(x => x.Name == "AddressInfo").FirstOrDefault();
            Assert.NotNull(nameProperty);

            IEdmTargetPath targetPath = model.GetTargetPath("NS.Default/Customers/AddressInfo/NS.LocalAddress/Road");

            string nameAnnotation = GetAnnotation(model, targetPath, myTerm, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            Assert.Equal("Name OutOfLine MyTerm Value", nameAnnotation);
        }

        [Fact]
        public void ParsingEntitySetPropertyOnDerivedTypeOutOfLineAnnotationsWorks()
        {
            // Test for MySchema.MyEntityContainer/MyEntitySet/MySchema.DerivedType/MyProperty

            string types =
@"<EntityType Name =""VipCustomer"" BaseType=""NS.Customer"">
 <Property Name=""VipNumber"" Type=""Edm.String"" Nullable=""false"" />
</EntityType>
<Term Name=""MyTerm"" Type=""Edm.String""/>
<Annotations Target=""NS.Default/Customers/NS.VipCustomer/VipNumber"" >
  <Annotation Term=""NS.MyTerm"" String=""Name OutOfLine MyTerm Value"" />
</Annotations>";

            string properties = "";

            IEdmModel model = GetModel(types: types, properties: properties);
            Assert.NotNull(model);

            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors), String.Format("Errors in validating model. {0}", String.Concat(errors.Select(e => e.ErrorMessage))));

            var vipCustomer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "VipCustomer");
            Assert.NotNull(vipCustomer);

            IEdmTerm myTerm = model.FindDeclaredTerm("NS.MyTerm");
            Assert.NotNull(myTerm);

            // Name
            IEdmProperty vipNumberProperty = vipCustomer.DeclaredProperties.Where(x => x.Name == "VipNumber").FirstOrDefault();
            Assert.NotNull(vipNumberProperty);

            IEdmTargetPath targetPath = model.GetTargetPath("NS.Default/Customers/NS.VipCustomer/VipNumber");

            string nameAnnotation = GetAnnotation(model, targetPath, myTerm, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            Assert.Equal("Name OutOfLine MyTerm Value", nameAnnotation);
        }

        [Fact]
        public void ParsingSingletonNavigationPropertyOutOfLineAnnotationsWorks()
        {
            // Test for MySchema.MyEntityContainer/MySingleton/MyNavigationProperty

            string types =
@"<EntityType Name=""Order"">
    <Key>
        <PropertyRef Name=""OrderId""/>
    </Key>
    <Property Name=""OrderId"" Type=""Edm.String"" Nullable=""false""/>
    <Property Name=""OrderName"" Type=""Edm.String"" Nullable=""false""/>
</EntityType>
<Term Name=""MyTerm"" Type=""Edm.String""/>
<Annotations Target=""NS.Default/Me/Orders"" >
  <Annotation Term=""NS.MyTerm"" String=""Name OutOfLine MyTerm Value"" />
</Annotations>";

            string properties =
                @"<NavigationProperty Name=""Orders"" Type=""Collection(NS.Order)"" ContainsTarget=""true"" />";

            IEdmModel model = GetModel(types: types, properties: properties);
            Assert.NotNull(model);

            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors), String.Format("Errors in validating model. {0}", String.Concat(errors.Select(e => e.ErrorMessage))));

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);

            IEdmTerm myTerm = model.FindDeclaredTerm("NS.MyTerm");
            Assert.NotNull(myTerm);

            // Name
            IEdmProperty nameProperty = customer.DeclaredNavigationProperties().Where(x => x.Name == "Orders").FirstOrDefault();
            Assert.NotNull(nameProperty);

            IEdmTargetPath targetPath = model.GetTargetPath("NS.Default/Me/Orders");

            string nameAnnotation = GetAnnotation(model, targetPath, myTerm, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            Assert.Equal("Name OutOfLine MyTerm Value", nameAnnotation);
        }

        [Fact]
        public void ParsingInvalidAnnotationTargetValidationFails()
        {
            string types =
@"
<Annotations Target=""NS.Default/Me/Orders/NS.VipOrder/OrderId"" >
  <Annotation Term=""NS.MyTerm"" String=""Name OutOfLine MyTerm Value"" />
</Annotations>";

            string properties ="";

            IEdmModel model = GetModel(types: types, properties: properties);
            Assert.NotNull(model);

            IEnumerable<EdmError> errors;
            Assert.False(model.Validate(out errors));
            var error1 = errors.First();
            var error2 = errors.ElementAt(1);
            Assert.Equal(EdmErrorCode.BadUnresolvedProperty, error1.ErrorCode);
            Assert.Equal(EdmErrorCode.BadUnresolvedTarget, error2.ErrorCode);
        }

        [Fact]
        public void ParsingMultipleOutOfLineAnnotationsWorks()
        {
            string types =
@"<EntityType Name=""Order"">
    <Key>
        <PropertyRef Name=""OrderId""/>
    </Key>
    <Property Name=""OrderId"" Type=""Edm.String"" Nullable=""false""/>
    <Property Name=""OrderName"" Type=""Edm.String"" Nullable=""false""/>
</EntityType>
<Term Name=""MyTerm"" Type=""Edm.String""/>
<Term Name=""MyTerm2"" Type=""Edm.String""/>
<Annotations Target=""NS.Default/Me/Orders"" >
  <Annotation Term=""NS.MyTerm"" String=""Name OutOfLine MyTerm Value"" />
</Annotations>
<Annotations Target=""NS.Default/Me/Orders"" >
  <Annotation Term=""NS.MyTerm2"" String=""Name OutOfLine MyTerm Value 2"" />
</Annotations>
<Annotations Target=""NS.Default/Customers/Orders"" >
  <Annotation Term=""NS.MyTerm"" String=""Name OutOfLine MyTerm Value"" />
</Annotations>";

            string properties =
                @"<NavigationProperty Name=""Orders"" Type=""Collection(NS.Order)"" ContainsTarget=""true"" />";

            IEdmModel model = GetModel(types: types, properties: properties);
            Assert.NotNull(model);

            IEnumerable<EdmError> errors;
            Assert.True(model.Validate(out errors), String.Format("Errors in validating model. {0}", String.Concat(errors.Select(e => e.ErrorMessage))));

            var customer = model.SchemaElements.OfType<IEdmEntityType>().FirstOrDefault(c => c.Name == "Customer");
            Assert.NotNull(customer);

            IEdmTerm myTerm = model.FindDeclaredTerm("NS.MyTerm");
            Assert.NotNull(myTerm);

            // Name
            IEdmProperty nameProperty = customer.DeclaredNavigationProperties().Where(x => x.Name == "Orders").FirstOrDefault();
            Assert.NotNull(nameProperty);

            // Mark model as Immutable so as to cache annotations.
            model.MarkAsImmutable();

            IEdmTargetPath targetPath = model.GetTargetPath("NS.Default/Me/Orders");

            string nameAnnotation = GetAnnotation(model, targetPath, myTerm, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            Assert.Equal("Name OutOfLine MyTerm Value", nameAnnotation);

            IEnumerable<IEdmVocabularyAnnotation> allAnnotations = model.VocabularyAnnotations;
            Assert.Equal(3, allAnnotations.Count()); // 2 NS.Default/Me/Orders and 1 NS.Default/Customers/Orders

            // Call FindXXX(...) multiple times to test there is no memory leak in VocabularyAnnotationsCache
            IEnumerable<IEdmVocabularyAnnotation> edmTargetPathAnnotations = model.FindVocabularyAnnotations(targetPath);
            edmTargetPathAnnotations = model.FindVocabularyAnnotations(targetPath);
            edmTargetPathAnnotations = model.FindVocabularyAnnotations(targetPath);
            IEnumerable<IEdmVocabularyAnnotation> targetPathStringAnnotations = model.GetTargetPathAnnotations("NS.Default/Me/Orders");
            targetPathStringAnnotations = model.GetTargetPathAnnotations("NS.Default/Me/Orders");
            targetPathStringAnnotations = model.GetTargetPathAnnotations("NS.Default/Me/Orders");

            Assert.Equal(2, edmTargetPathAnnotations.Count());
            Assert.Equal(2, targetPathStringAnnotations.Count());

            Assert.IsAssignableFrom<IEdmTargetPath>(edmTargetPathAnnotations.First().Target);
            Assert.IsAssignableFrom<IEdmTargetPath>(edmTargetPathAnnotations.Last().Target);
            Assert.IsAssignableFrom<IEdmTargetPath>(targetPathStringAnnotations.First().Target);
            Assert.IsAssignableFrom<IEdmTargetPath>(targetPathStringAnnotations.Last().Target);

            IEdmVocabularyAnnotation annotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>("NS.Default/Me/Orders", myTerm).FirstOrDefault();
            IEdmVocabularyAnnotation annotation1 = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>("NS.Default/Me/Orders", myTerm).FirstOrDefault();
            IEdmVocabularyAnnotation annotation2 = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>("NS.Default/Me/Orders", myTerm).FirstOrDefault();
            Assert.NotNull(annotation);
        }

        private string GetAnnotation(IEdmModel model, IEdmVocabularyAnnotatable target, IEdmTerm term, EdmVocabularyAnnotationSerializationLocation location)
        {
            IEdmVocabularyAnnotation annotation = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(target, term).FirstOrDefault();
            if (annotation != null)
            {
                Assert.True(typeof(IEdmTargetPath).IsAssignableFrom(annotation.Target.GetType()));

                Assert.True(annotation.GetSerializationLocation(model) == location);

                IEdmStringConstantExpression stringConstant = annotation.Value as IEdmStringConstantExpression;
                if (stringConstant != null)
                {
                    return stringConstant.Value;
                }
            }

            return null;
        }

        private static IEdmModel GetModel(string types = "", string properties = "")
        {
            const string template = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""NS"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Customer"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
        {1}
      </EntityType>
      {0}
      <EntityContainer Name =""Default"">
         <EntitySet Name=""Customers"" EntityType=""NS.Customer"" />
         <Singleton Name=""Me"" Type=""NS.Customer"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            string modelText = string.Format(template, types, properties);

            IEdmModel model;
            IEnumerable<EdmError> errors;

            bool result = CsdlReader.TryParse(XElement.Parse(modelText).CreateReader(), out model, out errors);
            Assert.True(result);
            return model;
        }
    }
}

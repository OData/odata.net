//---------------------------------------------------------------------
// <copyright file="OasisActionsFunctionsRelationshipChangesAcceptanceTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Xunit;

namespace Microsoft.OData.Edm.Tests.ScenarioTests
{
    public partial class OasisActionsFunctionsRelationshipChangesAcceptanceTest
    {
        private static DefaultTestModel Model;

        public class DefaultTestModel
        {
            public const string RepresentativeEdmxDocument = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Person"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""Children"" Type=""Collection(Test.Person)"" Partner=""Parent"" ContainsTarget=""true"" />
        <NavigationProperty Name=""Parent"" Type=""Test.Person"" Partner=""Children"" />
        <NavigationProperty Name=""Cars"" Type=""Collection(Test.Car)"" Partner=""Owner"" />
      </EntityType>
      <EntityType Name=""Car"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
        <NavigationProperty Name=""Owner"" Type=""Test.Person"" Partner=""Cars"" />
      </EntityType>
      <Function Name=""Get1"">
        <Parameter Name=""People"" Type=""Collection(Test.Person)"" />
        <ReturnType Type=""Test.Person"" />
      </Function>
      <Function Name=""Get2"" IsComposable=""true"">
        <ReturnType Type=""Edm.String"" />
      </Function>
      <Function Name=""Get3"">
        <Parameter Name=""Foo"" Type=""Collection(Edm.String)"" />
        <ReturnType Type=""Test.Person"" />
      </Function>
      <Function Name=""Get3"">
        <ReturnType Type=""Test.Person"" />
      </Function>
      <Action Name=""Add"">
        <Parameter Name=""People"" Type=""Collection(Test.Person)"" />
        <ReturnType Type=""Test.Person"" />
      </Action>
      <Action Name=""Other"">
        <ReturnType Type=""Edm.String"" />
      </Action>
      <Action Name=""RemoveBadCar"" IsBound=""true"" EntitySetPath=""People/Cars"">
        <Parameter Name=""People"" Type=""Collection(Test.Person)"" />
        <ReturnType Type=""Collection(Test.Car)"" />
      </Action>
      <EntityContainer Name=""Container"">
        <EntitySet Name=""People"" EntityType=""Test.Person"">
          <NavigationPropertyBinding Path=""Children"" Target=""People"" />
          <NavigationPropertyBinding Path=""Parent"" Target=""People"" />
          <NavigationPropertyBinding Path=""Cars"" Target=""Cars"" />
        </EntitySet>
        <EntitySet Name=""Cars"" EntityType=""Test.Car"">
          <NavigationPropertyBinding Path=""Owner"" Target=""People"" />
        </EntitySet>
        <ActionImport Name=""Add"" Action=""Test.Add"" EntitySet=""People"" />
        <ActionImport Name=""Other"" Action=""Test.Other"" />
        <FunctionImport Name=""Get1"" Function=""Test.Get1"" EntitySet=""People"" />
        <FunctionImport Name=""Get2"" Function=""Test.Get2"" IncludeInServiceDocument=""true"" />
        <FunctionImport Name=""Get3"" Function=""Test.Get3"" EntitySet=""People"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            public DefaultTestModel()
            {
                this.RepresentativeModel = CsdlReader.Parse(XElement.Parse(RepresentativeEdmxDocument).CreateReader());
                this.EntityContainer = this.RepresentativeModel.EntityContainer;
                Assert.NotNull(this.EntityContainer);
                Assert.Equal("Container", this.EntityContainer.Name);

                this.AddAction = (IEdmAction)this.RepresentativeModel.FindDeclaredOperations("Test.Add").Single();
                this.OtherAction = (IEdmAction)this.RepresentativeModel.FindDeclaredOperations("Test.Other").Single();
                this.RemoveBadCarAction = (IEdmAction)this.RepresentativeModel.FindDeclaredOperations("Test.RemoveBadCar").Single();

                this.Get1Function = (IEdmFunction)this.RepresentativeModel.FindDeclaredOperations("Test.Get1").Single();
                this.Get2Function = (IEdmFunction)this.RepresentativeModel.FindDeclaredOperations("Test.Get2").Single();
                var operations = this.RepresentativeModel.FindDeclaredOperations("Test.Get3").ToList();
                this.Get3FunctionOneParam = (IEdmFunction)operations[0];
                this.Get3FunctionNoParams = (IEdmFunction)operations[1];

                this.Get1FunctionImport = (IEdmFunctionImport)this.EntityContainer.FindOperationImports("Get1").Single();
                this.Get2FunctionImport = (IEdmFunctionImport)this.EntityContainer.FindOperationImports("Get2").Single();
                var get3Imports = this.EntityContainer.FindOperationImports("Get3").ToList();
                this.Get3FunctionImportWithOneParam = (IEdmFunctionImport)get3Imports[0];
                this.Get3FunctionImportWithNoParams = (IEdmFunctionImport)get3Imports[1];

                this.PersonType = this.RepresentativeModel.FindDeclaredType("Test.Person") as IEdmEntityType;
                Assert.NotNull(this.PersonType);

                this.CarType = this.RepresentativeModel.FindDeclaredType("Test.Car") as IEdmEntityType;
                Assert.NotNull(this.CarType);
            }

            public IEdmModel RepresentativeModel { get; private set; }

            public IEdmEntityContainer EntityContainer { get; private set; }

            public IEdmAction AddAction { get; private set; }

            public IEdmAction OtherAction { get; private set; }

            public IEdmAction RemoveBadCarAction { get; private set; }

            public IEdmFunction Get1Function { get; private set; }

            public IEdmFunction Get2Function { get; private set; }

            public IEdmFunction Get3FunctionOneParam { get; private set; }
            public IEdmFunction Get3FunctionNoParams { get; private set; }

            public IEdmFunctionImport Get1FunctionImport { get; private set; }

            public IEdmFunctionImport Get2FunctionImport { get; private set; }

            public IEdmFunctionImport Get3FunctionImportWithOneParam { get; private set; }
            
            public IEdmFunctionImport Get3FunctionImportWithNoParams { get; private set; }

            public IEdmEntityType PersonType { get; private set; }

            public IEdmEntityType CarType { get; private set; }
        }

        // Moved this to create this on a delayed property because if testing parsing in other tests 
        // its best not to invoke reading this as debugging gets harder.
        public DefaultTestModel TestModel
        {
            get
            {
                if (Model == null)
                {
                    Model = new DefaultTestModel();
                }

                return Model;
            }
        }
        [Fact]
        public void RepresentativeFunctionsModelShouldBeValid()
        {
            IEnumerable<EdmError> errors;
            bool valid = this.TestModel.RepresentativeModel.Validate(out errors);
            Assert.True(valid);
            Assert.Empty(errors);
        }

        [Fact]
        public void EnsureReadingCsdlResultsInTwoFunctionImportsAndActionImports()
        {
            string testCsdl = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""Person"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
      </EntityType>
      <Function Name=""Get1"">
        <ReturnType Type=""Test.Person"" />
      </Function>
      <Function Name=""Get1"">
        <Parameter Name=""Emails"" Type=""Collection(Edm.String)"" />
        <ReturnType Type=""Edm.Person"" />
      </Function>
      <Action Name=""Get1"">
        <ReturnType Type=""Test.Person"" />
      </Action>
      <Action Name=""Post1"">
        <ReturnType Type=""Test.Person"" />
      </Action>
      <Action Name=""Post1"">
        <Parameter Name=""Emails"" Type=""Collection(Edm.String)"" />
        <ReturnType Type=""Edm.Person"" />
      </Action>
      <Function Name=""Post1"">
        <ReturnType Type=""Test.Person"" />
      </Function>
      <EntityContainer Name=""Container"">
        <EntitySet Name=""People"" EntityType=""Test.Person"" />
        <ActionImport Name=""Post1"" Action=""Test.Post1"" />
        <FunctionImport Name=""Get1"" Function=""Test.Get1"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            var edmModel = CsdlReader.Parse(XElement.Parse(testCsdl).CreateReader());
            var operations = edmModel.SchemaElements.OfType<IEdmOperation>().ToList();
            Assert.Equal(6, operations.Count());
            var container = edmModel.EntityContainer;
            var operationImports = container.OperationImports().ToList();

            // Notice there are only two defined above but there are 4, this is by design.
            Assert.Equal(4, operationImports.Count);
            Assert.Equal("Post1", operationImports[0].Name);
            Assert.Equal("Post1", operationImports[0].Operation.Name);
            Assert.Empty(operationImports[0].Operation.Parameters);

            Assert.Equal("Post1", operationImports[1].Name);
            Assert.Equal("Post1", operationImports[1].Operation.Name);
            Assert.Single(operationImports[1].Operation.Parameters);

            Assert.Equal("Get1", operationImports[2].Name);
            Assert.Equal("Get1", operationImports[2].Operation.Name);
            Assert.Empty(operationImports[2].Operation.Parameters);

            Assert.Equal("Get1", operationImports[3].Name);
            Assert.Equal("Get1", operationImports[3].Operation.Name);
            Assert.Single(operationImports[3].Operation.Parameters);
        }

        [Fact]
        public void EnsureOperationReturnTypesAreReadCorrectlyFromCsdl()
        {
            string testCsdl = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <ComplexType Name=""Complex"" />
      <Function Name=""FunctionWithComplexReturnType"">
        <ReturnType Type=""Test.Complex"" Nullable=""false"" />
      </Function>
      <Function Name=""FunctionWithComplexCollectionReturnType"">
        <ReturnType Type=""Collection(Test.Complex)"" Nullable=""false"" />
      </Function>
      <Function Name=""FunctionWithPrimitiveReturnType"">
        <ReturnType Type=""Edm.String"" Nullable=""false"" MaxLength=""10"" />
      </Function>
      <Function Name=""FunctionWithPrimitiveCollectionReturnType"">
        <ReturnType Type=""Collection(Edm.String)"" Nullable=""false"" MaxLength=""20"" />
      </Function>
      <Action Name=""ActionWithComplexReturnType"">
        <ReturnType Type=""Test.Complex"" Nullable=""false"" />
      </Action>
      <Action Name=""ActionWithComplexCollectionReturnType"">
        <ReturnType Type=""Collection(Test.Complex)"" Nullable=""false"" />
      </Action>
      <Action Name=""ActionWithPrimitiveReturnType"">
        <ReturnType Type=""Edm.String"" Nullable=""false"" MaxLength=""10"" />
      </Action>
      <Action Name=""ActionWithPrimitiveCollectionReturnType"">
        <ReturnType Type=""Collection(Edm.String)"" Nullable=""false"" MaxLength=""20"" />
      </Action>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            var edmModel = CsdlReader.Parse(XElement.Parse(testCsdl).CreateReader());
            var operations = edmModel.SchemaElements.OfType<IEdmOperation>().ToList();
            Assert.Equal(8, operations.Count());

            // Functions
            Assert.True(operations[0] is IEdmFunction);
            Assert.Equal("FunctionWithComplexReturnType", operations[0].Name);
            Assert.Equal("Test.Complex", operations[0].ReturnType.FullName());
            Assert.False(operations[0].ReturnType.IsNullable);

            Assert.True(operations[1] is IEdmFunction);
            Assert.Equal("FunctionWithComplexCollectionReturnType", operations[1].Name);
            Assert.Equal("Collection(Test.Complex)", operations[1].ReturnType.FullName());
            Assert.False(operations[1].ReturnType.AsCollection().ElementType().IsNullable);

            Assert.True(operations[2] is IEdmFunction);
            Assert.Equal("FunctionWithPrimitiveReturnType", operations[2].Name);
            Assert.Equal("Edm.String", operations[2].ReturnType.FullName());
            Assert.False(operations[2].ReturnType.IsNullable);
            Assert.Equal(10, operations[2].ReturnType.AsString().MaxLength);

            Assert.True(operations[3] is IEdmFunction);
            Assert.Equal("FunctionWithPrimitiveCollectionReturnType", operations[3].Name);
            Assert.Equal("Collection(Edm.String)", operations[3].ReturnType.FullName());
            Assert.False(operations[3].ReturnType.AsCollection().ElementType().IsNullable);
            Assert.Equal(20, operations[3].ReturnType.AsCollection().ElementType().AsString().MaxLength);

            // Actions
            Assert.True(operations[4] is IEdmAction);
            Assert.Equal("ActionWithComplexReturnType", operations[4].Name);
            Assert.Equal("Test.Complex", operations[4].ReturnType.FullName());
            Assert.False(operations[4].ReturnType.IsNullable);

            Assert.True(operations[5] is IEdmAction);
            Assert.Equal("ActionWithComplexCollectionReturnType", operations[5].Name);
            Assert.Equal("Collection(Test.Complex)", operations[5].ReturnType.FullName());
            Assert.False(operations[5].ReturnType.AsCollection().ElementType().IsNullable);

            Assert.True(operations[6] is IEdmAction);
            Assert.Equal("ActionWithPrimitiveReturnType", operations[6].Name);
            Assert.Equal("Edm.String", operations[6].ReturnType.FullName());
            Assert.False(operations[6].ReturnType.IsNullable);
            Assert.Equal(10, operations[6].ReturnType.AsString().MaxLength);

            Assert.True(operations[7] is IEdmAction);
            Assert.Equal("ActionWithPrimitiveCollectionReturnType", operations[7].Name);
            Assert.Equal("Collection(Edm.String)", operations[7].ReturnType.FullName());
            Assert.False(operations[7].ReturnType.AsCollection().ElementType().IsNullable);
            Assert.Equal(20, operations[7].ReturnType.AsCollection().ElementType().AsString().MaxLength);
        }

        [Fact]
        public void ReturnTypePathReturnsCorrectly()
        {
            Assert.NotNull(this.TestModel.RemoveBadCarAction.ReturnType);
            var personType = this.TestModel.RemoveBadCarAction.ReturnType.AsCollection().ElementType().Definition;
            Assert.Equal(this.TestModel.CarType, personType);
        }

        [Fact]
        public void EntitySetPathReturnsCorrectly()
        {
            var paths = this.TestModel.RemoveBadCarAction.EntitySetPath.PathSegments.ToList();
            Assert.Equal("People", paths[0]);
            Assert.Equal("Cars", paths[1]);
        }

        [Fact]
        public void ValidateEntitySetPathExpressionCannotHaveValueWhenActionNonBound()
        {
            const string errorDocument = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <Action Name=""Add"" EntitySetPath=""Param/Thing"">
        <Parameter Name=""Param"" Type=""Edm.String"" />
        <ReturnType Type=""Test.Person"" />
      </Action>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            IEdmModel model = null;
            IEnumerable<EdmError> errors = null;
            var errorParsing = CsdlReader.TryParse(XElement.Parse(errorDocument).CreateReader(), out model, out errors);
            Assert.False(errorParsing);
            var errorsList = errors.ToList();
            var error = Assert.Single(errorsList);
            Assert.Equal(EdmErrorCode.InvalidEntitySetPath, error.ErrorCode);
            Assert.Equal(Strings.CsdlParser_InvalidEntitySetPathWithUnboundAction(CsdlConstants.Element_Action, "Add"), error.ErrorMessage);
        }

        [Fact]
        public void ValidateActionImportReferencingNonExistingActionShouldReturnError()
        {
            const string errorDocument = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""Container"">
        <ActionImport Name=""Add"" Action=""Test.Add""/>
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";
            
            var model = CsdlReader.Parse(XElement.Parse(errorDocument).CreateReader());
            IEnumerable<EdmError> errors = null;
            model.Validate(out errors);

            var errorsList = errors.ToList();
            var error = Assert.Single(errorsList);
            Assert.Equal(EdmErrorCode.BadUnresolvedOperation, error.ErrorCode);
        }

        [Fact]
        public void ValidateActionImportMissingActionAttributeShouldReturnError()
        {
            const string errorDocument = @"<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Namespace=""Test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityContainer Name=""Container"">
        <ActionImport Name=""Add"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            Action test = () => CsdlReader.Parse(XElement.Parse(errorDocument).CreateReader());
            var exception = Assert.Throws<EdmParseException>(test);
            Assert.Contains(Strings.XmlParser_MissingAttribute("Action", "ActionImport"), exception.Message);
        }

        [Fact]
        public void VerifyRepresentativeModelWrittenOutCorrectly()
        {
            var builder = new StringBuilder();
            using (var writer = XmlWriter.Create(builder))
            {
                IEnumerable<EdmError> errors;
                Assert.True(CsdlWriter.TryWriteCsdl(this.TestModel.RepresentativeModel, writer, CsdlTarget.OData, out errors));
                Assert.Empty(errors);
                writer.Flush();
            }

            string actual = builder.ToString();
            var actualXml = XElement.Parse(actual);
            var actualNormalized = actualXml.ToString();

            Assert.Equal(DefaultTestModel.RepresentativeEdmxDocument, actualNormalized);
        }
    }
}

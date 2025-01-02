//---------------------------------------------------------------------
// <copyright file="OperationImportSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class OperationImportSegmentTests
    {
        private readonly EdmPrimitiveTypeReference nullableIntType;
        private readonly EdmPrimitiveTypeReference nullableDecimalType;
        private readonly EdmPrimitiveTypeReference nullableStringType;
        private readonly EdmPrimitiveTypeReference nullableBinaryType;

        private readonly EdmEntityContainer container;

        private readonly EdmFunctionImport functionImportIntToInt;
        private readonly EdmFunctionImport functionImportDecimalToInt;
        private readonly EdmFunctionImport functionImportBinaryToInt;
        private readonly EdmFunctionImport functionImportIntToString;

        private readonly EdmFunction functionIntToInt;
        private readonly EdmFunction functionDecimalToInt;
        private readonly EdmFunction functionBinaryToInt;
        private readonly EdmFunction functionIntToString;
        private EdmModel model;

        public OperationImportSegmentTests()
        {
            nullableIntType = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), true);
            nullableDecimalType = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true);
            nullableBinaryType = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), true);
            nullableStringType = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true);

            container = ModelBuildingHelpers.BuildValidEntityContainer();
            model = new EdmModel();
            model.AddElement(container);

            this.functionIntToInt = new EdmFunction("Name.Space", "Function", this.nullableIntType);
            this.functionIntToInt.AddParameter("Parameter1", this.nullableIntType);
            this.functionImportIntToInt = new EdmFunctionImport(this.container, "Function", this.functionIntToInt);

            this.functionDecimalToInt = new EdmFunction("Name.Space", "Function", this.nullableIntType);
            this.functionDecimalToInt.AddParameter("Parameter1", this.nullableDecimalType);
            this.functionImportDecimalToInt = new EdmFunctionImport(this.container, "Function", this.functionDecimalToInt);

            this.functionBinaryToInt = new EdmFunction("Name.Space", "Function", this.nullableIntType);
            this.functionBinaryToInt.AddParameter("Parameter1", this.nullableBinaryType);
            this.functionImportBinaryToInt = new EdmFunctionImport(this.container, "Function", this.functionBinaryToInt);

            this.functionIntToString = new EdmFunction("Name.Space", "Function", this.nullableStringType);
            this.functionIntToString.AddParameter("Parameter1", this.nullableIntType);
            this.functionImportIntToString = new EdmFunctionImport(this.container, "Function", this.functionIntToString);

            model.AddElement(functionIntToInt);
            model.AddElement(functionDecimalToInt);
            model.AddElement(functionBinaryToInt);
            model.AddElement(functionIntToString);
        }

        [Fact]
        public void OperationCannotBeNull()
        {
            Action createWithNullServiceOperations = () => new OperationImportSegment((IEdmOperationImport)null, HardCodedTestModel.GetPeopleSet());
            Assert.Throws<ArgumentNullException>("operationImport", createWithNullServiceOperations);
        }

        [Fact]
        public void OperationsCannotBeNull()
        {
            Action createWithNullServiceOperations = () => new OperationImportSegment((List<IEdmOperationImport>)null, HardCodedTestModel.GetPeopleSet());

            // bad error message as a result of making OperationSegment take IEnumerable... but its still thrown by us so I don't care.
            Assert.Throws<ArgumentNullException>("operationImports", createWithNullServiceOperations);
        }

        [Fact]
        public void ParametersSetCorrectly()
        {
            OperationImportSegment segment = new OperationImportSegment(new List<IEdmOperationImport>() { HardCodedTestModel.GetFunctionImportForGetCoolestPerson() },
                HardCodedTestModel.GetPeopleSet(),
                new[] { new OperationSegmentParameter("stuff", new ConstantNode(new ODataPrimitiveValue(true))), });

            OperationSegmentParameter parameter = segment.Parameters.SingleOrDefault(p => p.Name == "stuff");
            Assert.NotNull(parameter);

            ConstantNode constantNode = Assert.IsType<ConstantNode>(parameter.Value);

            var actual = Assert.IsType<ODataPrimitiveValue>(constantNode.Value);
            Assert.Equal(true, actual.Value);
        }

        [Fact]
        public void SingleOperationSetCorrectly()
        {
            OperationImportSegment segment = new OperationImportSegment(HardCodedTestModel.GetFunctionImportForGetCoolestPerson(), HardCodedTestModel.GetPeopleSet());
            Assert.Same(HardCodedTestModel.GetFunctionImportForGetCoolestPerson(), segment.OperationImports.Single());
        }

        [Fact]
        public void CandidateServiceOperationsSetCorrectly()
        {
            OperationImportSegment segment = new OperationImportSegment(new List<IEdmOperationImport>() { HardCodedTestModel.GetFunctionImportForGetCoolestPerson() }, HardCodedTestModel.GetPeopleSet());
            var operationImport = Assert.Single(segment.OperationImports);
            Assert.Equal(HardCodedTestModel.GetFunctionImportForGetCoolestPerson().Name, operationImport.Name);
        }

        [Fact]
        public void EntitySetIsCorrect()
        {
            OperationImportSegment segment = new OperationImportSegment(new List<IEdmOperationImport>() { HardCodedTestModel.GetFunctionImportForGetCoolestPerson() }, HardCodedTestModel.GetPeopleSet());
            Assert.Same(HardCodedTestModel.GetPeopleSet(), segment.EntitySet);
        }

        [Fact]
        public void EmptyListOfOperationsShouldThrow()
        {
            var operations = new List<IEdmOperationImport>();
            Action create = () => new OperationImportSegment(operations, null);

            Assert.Throws<ArgumentException>("operations", create);
        }

        [Fact]
        public void EdmTypeComputedFromOperationReturnTypeForSingleOperation()
        {
            OperationImportSegment segment = new OperationImportSegment(HardCodedTestModel.GetFunctionImportForGetCoolestPerson(), HardCodedTestModel.GetPeopleSet());
            Assert.Same(HardCodedTestModel.GetFunctionImportForGetCoolestPerson().Operation.ReturnType.Definition, segment.EdmType);
        }

        [Fact]
        public void IfOperationsAllHaveSameReturnTypeThenReturnTypeIsSet()
        {
            var operations = new List<IEdmOperationImport> {
                this.functionImportIntToInt,
                this.functionImportDecimalToInt,
                this.functionImportIntToInt,
            };

            var segment = new OperationImportSegment(operations, null);

            // All operations in the list return int, so we can set the return type to that.
            Assert.Same(operations.First().Operation.ReturnType.Definition, segment.EdmType);
        }

        [Fact]
        public void IfOperationsHaveDifferentReturnTypeThenWeThrowInEdmTypeProperty()
        {
            var operations = new List<IEdmOperationImport> {
                this.functionImportIntToInt,
                this.functionImportDecimalToInt,
                this.functionImportIntToString,
            };

            var segment = new OperationImportSegment(operations, null);

            try
            {
                // Dummy code, just need to access property to get the exception
                var type = segment.EdmType;
                Assert.True(false, "The EdmType getter returned '" + type + "', but should have thrown.");
            }
            catch (ODataException e)
            {
                Assert.Equal("No type could be computed for this Segment since there were multiple possible operations with varying return types.", e.Message);
            }
        }

        [Fact]
        public void IfEntitySetSpecifiedThenComputedtypeMustBeEntityOrUnknown()
        {
            Action create = () => new OperationImportSegment(HardCodedTestModel.GetAllFunctionImportsForGetMostImportantPerson()[0], HardCodedTestModel.GetDogsSet());
            create.Throws<ODataException>("The return type from the operation is not possible with the given entity set.");
        }

        [Fact]
        public void ComputedTypeMustBeRelatedToEntitySet()
        {
            Action create = () => new OperationImportSegment(HardCodedTestModel.GetFunctionImportForGetCoolestPerson(), HardCodedTestModel.GetDogsSet());
            create.Throws<ODataException>("The return type from the operation is not possible with the given entity set.");
        }

        [Fact]
        public void ComputedTypeCanBeRelatedToEntitySetByInheritance()
        {
            OperationImportSegment segment = new OperationImportSegment(HardCodedTestModel.GetFunctionImportForGetBestManager(), HardCodedTestModel.GetPeopleSet());
            Assert.Same(HardCodedTestModel.GetManagerType(), segment.EdmType);
            Assert.Same(HardCodedTestModel.GetPeopleSet(), segment.EntitySet);
        }

        [Fact]
        public void ComputedTypeInsideCollectionShouldBeUsedWhenComparingToSet()
        {
            Action create = () => new OperationImportSegment(HardCodedTestModel.GetFunctionImportForGetCoolPeople(), HardCodedTestModel.GetPeopleSet());
            create.DoesNotThrow();
        }

        [Fact]
        public void OperationWithTypeShouldNotAllowEntitySet()
        {
            Action create = () => new OperationImportSegment(HardCodedTestModel.GetFunctionImportForResetAllData(), HardCodedTestModel.GetPeopleSet());
            create.Throws<ODataException>("The return type from the operation is not possible with the given entity set.");
        }

        [Fact]
        public void EqualityIsCorrect()
        {
            OperationImportSegment segment1 = new OperationImportSegment(new List<IEdmOperationImport>() { HardCodedTestModel.GetFunctionImportForGetCoolestPerson() }, HardCodedTestModel.GetPeopleSet());
            OperationImportSegment segment2 = new OperationImportSegment(new List<IEdmOperationImport>() { HardCodedTestModel.GetFunctionImportForGetCoolestPerson() }, HardCodedTestModel.GetPeopleSet());
            Assert.True(segment1.Equals(segment2));
        }

        [Fact]
        public void InequalityIsCorrect()
        {
            OperationImportSegment operationSegment1 = new OperationImportSegment(new List<IEdmOperationImport>() { HardCodedTestModel.GetFunctionImportForGetCoolestPerson() }, HardCodedTestModel.GetPeopleSet());
            OperationImportSegment operationSegment2 = new OperationImportSegment(new List<IEdmOperationImport>() { HardCodedTestModel.GetFunctionImportForGetCoolestPerson() }, null);
            OperationImportSegment operationSegment3 = new OperationImportSegment(new List<IEdmOperationImport>() { HardCodedTestModel.GetFunctionImportForGetCoolPeople() }, HardCodedTestModel.GetPeopleSet());
            BatchSegment segment = BatchSegment.Instance;
            Assert.False(operationSegment1.Equals(operationSegment2));
            Assert.False(operationSegment1.Equals(operationSegment3));
            Assert.False(operationSegment1.Equals(segment));
        }
    }
}

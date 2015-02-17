//---------------------------------------------------------------------
// <copyright file="SelectPathSegmentTokenBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    [TestClass]
    public class SelectPathSegmentTokenBinderTests
    {
        // Bind a EndPathToken
        [TestMethod]
        public void PropertyNameResultsInStructuralPropertySelectionItem()
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Shoe", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            segment.ShouldBePropertySegment(HardCodedTestModel.GetPersonShoeProp());
        }

        [TestMethod]
        public void NavPropNameResultsInNavigationPropertyLinkSelectionItem()
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("MyDog", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            segment.ShouldBeNavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp());
        }

        [TestMethod]
        public void ActionNameResultsInOperationSelectionItem()
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Fully.Qualified.Namespace.Walk", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType());
            segment.ShouldBeOperationSegment(HardCodedTestModel.GetDogWalkAction());
        }

        [TestMethod]
        public void FunctionNameResultsInOperationSelectionItemWithAllMatchingOverloads()
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Fully.Qualified.Namespace.HasDog", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            segment.ShouldBeOperationSegment(HardCodedTestModel.GetAllHasDogFunctionOverloadsForPeople() /* TODO: parameters */);
        }

        [TestMethod]
        public void FunctionNameResultsInOperationSelectionItemWithOnlyOverloadsBoundToTheGivenType()
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Fully.Qualified.Namespace.HasDog", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetEmployeeType());
            segment.ShouldBeOperationSegment(HardCodedTestModel.GetHasDogOverloadForEmployee());
        }

        [TestMethod]
        public void ActionNameResultsInOperationSelectionForDerivedBindingTypeItemWithAllMatchingOverloads()
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Fully.Qualified.Namespace.Move", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetEmployeeType());
            segment.ShouldBeOperationSegment(HardCodedTestModel.GetMoveOverloadForEmployee() /* TODO: parameters */);
        }

        [TestMethod]
        public void ActionNameResultsInOperationSelectionItemWithAllMatchingOverloadsMatchingBindingType()
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Fully.Qualified.Namespace.Move", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            segment.ShouldBeOperationSegment(HardCodedTestModel.GetMoveOverloadForPerson());
        }

        [TestMethod]
        public void ContainerQualifiedActionNameShouldWork()
        {
            BindQualifiedOperationToPerson("Fully.Qualified.Namespace.Move").ShouldBeOperationSegment(HardCodedTestModel.GetMoveOverloadForPerson() /* TODO: parameters */);
        }

        [TestMethod]
        public void NamespaceQualifiedActionNameShouldWork()
        {
            BindQualifiedOperationToPerson("Fully.Qualified.Namespace.Move").ShouldBeOperationSegment(HardCodedTestModel.GetMoveOverloadForPerson() /* TODO: parameters */);
        }

        [TestMethod]
        public void DifferentQualifiedActionNameShouldWork()
        {
            BindQualifiedOperationToPerson("Fully.Qualified.Namespace.Move").ShouldBeOperationSegment(HardCodedTestModel.GetMoveOverloadForPerson());
        }

        [TestMethod]
        public void OperationWithParenthesesShouldNotWork()
        {
            Action action = () => SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Move()", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType()).Should();
            action.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_PropertyNotDeclared("Fully.Qualified.Namespace.Person", "Move()"));
        }

        [TestMethod]
        public void UnboundOperationShouldIgnore()
        {
            var segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Fully.Qualified.Namespace.GetPet1", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            segment.Should().BeNull();
        }

        [TestMethod]
        public void QualifiedNameShouldTreatAsDynamicPropertyInOpenType()
        {
            var segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("A.B", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetOpenEmployeeType());
            segment.ShouldBeOpenPropertySegment("A.B");
        }

        [TestMethod]
        public void OperationWithQualifiedParameterTypeNamesShouldIgnore()
        {
            var segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Move2(Fully.Qualified.Namespace.Person,Edm.String)", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            segment.Should().BeNull();
        }

        [TestMethod]
        public void UnqualifiedActionNameOnOpenTypeShouldBeInterpretedAsAnOpenProperty()
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Restore", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());
            segment.ShouldBeOpenPropertySegment("Restore");
        }

        [TestMethod]
        public void QualifiedActionNameOnOpenTypeShouldBeInterpretedAsAnOperation()
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Fully.Qualified.Namespace.Restore", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());
            segment.ShouldBeOperationSegment(HardCodedTestModel.GetRestoreAction());
        }

        [TestMethod]
        public void UnfoundProperyOnOpenTypeResultsInOpenPropertySelectionItem()
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Fully.Qualified.Namespace.Effervescence", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType());
            segment.ShouldBeOpenPropertySegment("Fully.Qualified.Namespace.Effervescence");
        }

        [TestMethod]
        public void UnfoundProperyOnClosedTypeThrows()
        {
            Action visit = () => SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Missing", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType());
            visit.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_PropertyNotDeclared(HardCodedTestModel.GetPersonType(), "Missing"));
        }

        [TestMethod]
        public void UnQualifiedActionNameShouldThrow()
        {
            Action visit = () => SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Walk", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType());
            visit.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_PropertyNotDeclared(HardCodedTestModel.GetDogType(), "Walk"));
        }


        [TestMethod]
        public void ValidateThrowInFindOperationsByBindingParameterTypeHierarchyExceptionDoesNotSurface()
        {
            var model = new FindOperationsByBindingParameterTypeHierarchyThrowingCatchableExceptionEdmModel();
            IEdmEntityType entityType = new EdmEntityType("n", "EntityType");
            ODataPathSegment foundPathSegment = null;
            SelectPathSegmentTokenBinder.TryBindAsOperation(new SystemToken("foo", null), model, entityType, out foundPathSegment).Should().BeFalse();
        }

        [Ignore]
        [TestMethod]
        public void ValidateNonCatchableExceptionsThrowInFindOperationsByBindingParameterTypeHierarchyExceptionAndSurfaces()
        {
            var model = new FindOperationsByBindingParameterTypeHierarchyThrowingStackOverflowEdmModel();
            IEdmEntityType entityType = new EdmEntityType("n", "EntityType");
            ODataPathSegment foundPathSegment = null;
            Action test = () => SelectPathSegmentTokenBinder.TryBindAsOperation(new SystemToken("foo", null), model, entityType, out foundPathSegment);
            test.ShouldThrow<StackOverflowException>();
        }

        [TestMethod]
        public void SelectBindingShouldCheckForExtendedModelInterfaceAndLookupOperationsByBindingType()
        {
            IEdmEntityType dummyBindingType = new EdmEntityType("Fake", "Type");
            IEdmEntityTypeReference dummyTypeReference = new EdmEntityTypeReference(dummyBindingType, false);
            var function1 = new EdmFunction("Name.Space", "FakeFunction1", new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), true), true, null, false);
            function1.AddParameter("bindingParameter", dummyTypeReference);
            var function2 = new EdmFunction("Name.Space", "FakeFunction1", new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int64), true), true, null, false);
            function2.AddParameter("bindingParameter", dummyTypeReference);

            var dummyOperations = new[] { function1, function2 };
            var resolver = new DummyFunctionImportResolver
                {
                    FindOperations = (bindingType) =>
                        {
                            bindingType.Should().BeSameAs(dummyBindingType);
                            return dummyOperations;
                        }
                };
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Name.Space.FakeFunction1", null, null), resolver, dummyBindingType);
            segment.ShouldBeOperationSegment(dummyOperations /* TODO: parameters */);
        }

        private static ODataPathSegment BindQualifiedOperationToPerson(string identifier)
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(
                new NonSystemToken(identifier, null, null), 
                HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType());
            return segment;
        }

        private class DummyFunctionImportResolver : EdmModel
        {
            internal Func<IEdmType, IEnumerable<IEdmOperation>> FindOperations { get; set; }

            public override IEnumerable<IEdmOperation> FindDeclaredBoundOperations(IEdmType bindingType)
            {
                if (this.FindOperations != null)
                {
                    return this.FindOperations(bindingType);
                }

                throw new NotImplementedException();
            }

            public IEdmOperationImport FindServiceOperation(string serviceOperationName)
            {
                throw new NotImplementedException();
            }
        }

        private class FindOperationsByBindingParameterTypeHierarchyThrowingCatchableExceptionEdmModel : EdmModel
        {
            public override IEnumerable<IEdmOperation> FindDeclaredBoundOperations(IEdmType bindingType)
            {
                throw new Exception("Oh no!");
            }
        }

        private class FindOperationsByBindingParameterTypeHierarchyThrowingStackOverflowEdmModel : EdmModel
        {
            public override IEnumerable<IEdmOperation> FindDeclaredBoundOperations(IEdmType bindingType)
            {
                throw new StackOverflowException( "Oh no!");
            }
        }
    }
}

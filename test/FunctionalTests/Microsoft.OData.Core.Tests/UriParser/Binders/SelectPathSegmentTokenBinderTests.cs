//---------------------------------------------------------------------
// <copyright file="SelectPathSegmentTokenBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser.Binders
{
    public class SelectPathSegmentTokenBinderTests
    {
        private static readonly ODataUriResolver DefaultUriResolver = ODataUriResolver.GetUriResolver(null);

        // Bind a EndPathToken
        [Fact]
        public void PropertyNameResultsInStructuralPropertySelectionItem()
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Shoe", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), DefaultUriResolver);
            segment.ShouldBePropertySegment(HardCodedTestModel.GetPersonShoeProp());
        }

        [Fact]
        public void NavPropNameResultsInNavigationPropertyLinkSelectionItem()
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("MyDog", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), DefaultUriResolver);
            segment.ShouldBeNavigationPropertySegment(HardCodedTestModel.GetPersonMyDogNavProp());
        }

        [Fact]
        public void ActionNameResultsInOperationSelectionItem()
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Fully.Qualified.Namespace.Walk", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType(), DefaultUriResolver);
            segment.ShouldBeOperationSegment(HardCodedTestModel.GetDogWalkAction());
        }

        [Fact]
        public void FunctionNameResultsInOperationSelectionItemWithAllMatchingOverloads()
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Fully.Qualified.Namespace.HasDog", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), DefaultUriResolver);
            segment.ShouldBeOperationSegment(HardCodedTestModel.GetAllHasDogFunctionOverloadsForPeople() /* TODO: parameters */);
        }

        [Fact]
        public void FunctionNameResultsInOperationSelectionItemWithOnlyOverloadsBoundToTheGivenType()
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Fully.Qualified.Namespace.HasDog", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetEmployeeType(), DefaultUriResolver);
            segment.ShouldBeOperationSegment(HardCodedTestModel.GetHasDogOverloadForEmployee());
        }

        [Fact]
        public void ActionNameResultsInOperationSelectionForDerivedBindingTypeItemWithAllMatchingOverloads()
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Fully.Qualified.Namespace.Move", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetEmployeeType(), DefaultUriResolver);
            segment.ShouldBeOperationSegment(HardCodedTestModel.GetMoveOverloadForEmployee() /* TODO: parameters */);
        }

        [Fact]
        public void ActionNameResultsInOperationSelectionItemWithAllMatchingOverloadsMatchingBindingType()
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Fully.Qualified.Namespace.Move", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), DefaultUriResolver);
            segment.ShouldBeOperationSegment(HardCodedTestModel.GetMoveOverloadForPerson());
        }

        [Fact]
        public void ContainerQualifiedActionNameShouldWork()
        {
            BindQualifiedOperationToPerson("Fully.Qualified.Namespace.Move").ShouldBeOperationSegment(HardCodedTestModel.GetMoveOverloadForPerson() /* TODO: parameters */);
        }

        [Fact]
        public void NamespaceQualifiedActionNameShouldWork()
        {
            BindQualifiedOperationToPerson("Fully.Qualified.Namespace.Move").ShouldBeOperationSegment(HardCodedTestModel.GetMoveOverloadForPerson() /* TODO: parameters */);
        }

        [Fact]
        public void DifferentQualifiedActionNameShouldWork()
        {
            BindQualifiedOperationToPerson("Fully.Qualified.Namespace.Move").ShouldBeOperationSegment(HardCodedTestModel.GetMoveOverloadForPerson());
        }

        [Fact]
        public void OperationWithParenthesesShouldNotWork()
        {
            Action action = () => SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Move()", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), DefaultUriResolver).Should();
            action.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_PropertyNotDeclared("Fully.Qualified.Namespace.Person", "Move()"));
        }

        [Fact]
        public void UnboundOperationShouldIgnore()
        {
            var segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Fully.Qualified.Namespace.GetPet1", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), DefaultUriResolver);
            segment.Should().BeNull();
        }

        [Fact]
        public void QualifiedNameShouldTreatAsDynamicPropertyInOpenType()
        {
            var segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("A.B", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetOpenEmployeeType(), DefaultUriResolver);
            segment.ShouldBeDynamicPathSegment("A.B");
        }

        [Fact]
        public void OperationWithQualifiedParameterTypeNamesShouldIgnore()
        {
            var segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Move2(Fully.Qualified.Namespace.Person,Edm.String)", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), DefaultUriResolver);
            segment.Should().BeNull();
        }

        [Fact]
        public void UnqualifiedActionNameOnOpenTypeShouldBeInterpretedAsAnOpenProperty()
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Restore", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType(), DefaultUriResolver);
            segment.ShouldBeDynamicPathSegment("Restore");
        }

        [Fact]
        public void QualifiedActionNameOnOpenTypeShouldBeInterpretedAsAnOperation()
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Fully.Qualified.Namespace.Restore", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType(), DefaultUriResolver);
            segment.ShouldBeOperationSegment(HardCodedTestModel.GetRestoreAction());
        }

        [Fact]
        public void UnfoundProperyOnOpenTypeResultsInOpenPropertySelectionItem()
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Fully.Qualified.Namespace.Effervescence", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPaintingType(), DefaultUriResolver);
            segment.ShouldBeDynamicPathSegment("Fully.Qualified.Namespace.Effervescence");
        }

        [Fact]
        public void UnfoundProperyOnClosedTypeThrows()
        {
            Action visit = () => SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Missing", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetPersonType(), DefaultUriResolver);
            visit.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_PropertyNotDeclared(HardCodedTestModel.GetPersonType(), "Missing"));
        }

        [Fact]
        public void UnQualifiedActionNameShouldThrow()
        {
            Action visit = () => SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Walk", null, null), HardCodedTestModel.TestModel, HardCodedTestModel.GetDogType(), DefaultUriResolver);
            visit.ShouldThrow<ODataException>().WithMessage(Strings.MetadataBinder_PropertyNotDeclared(HardCodedTestModel.GetDogType(), "Walk"));
        }


        [Fact]
        public void ValidateThrowInFindOperationsByBindingParameterTypeHierarchyExceptionDoesNotSurface()
        {
            var model = new FindOperationsByBindingParameterTypeHierarchyThrowingCatchableExceptionEdmModel();
            IEdmEntityType entityType = new EdmEntityType("n", "EntityType");
            ODataPathSegment foundPathSegment = null;
            SelectPathSegmentTokenBinder.TryBindAsOperation(new SystemToken("foo", null), model, entityType, out foundPathSegment).Should().BeFalse();
        }

        [Fact]
        public void BindAsOperationWithWildcardInPathTokenAndBindingParameterTypeThrows()
        {
            var model = new FindOperationsByBindingParameterTypeHierarchyThrowingCatchableExceptionEdmModel();
            IEdmEntityType entityType = new EdmEntityType("n", "EntityType");
            ODataPathSegment foundPathSegment = null;
            SelectPathSegmentTokenBinder.TryBindAsOperation(new SystemToken("f*oo", null), model, entityType, out foundPathSegment).Should().BeFalse();
        }

        public void ValidateNonCatchableExceptionsThrowInFindOperationsByBindingParameterTypeHierarchyExceptionAndSurfaces()
        {
            var model = new FindOperationsByBindingParameterTypeHierarchyThrowingNonCatchableExceptionEdmModel();
            IEdmEntityType entityType = new EdmEntityType("n", "EntityType");
            ODataPathSegment foundPathSegment = null;
            Action test = () => SelectPathSegmentTokenBinder.TryBindAsOperation(new SystemToken("foo", null), model, entityType, out foundPathSegment);
            test.ShouldThrow<OutOfMemoryException>();
        }

        [Fact]
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
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(new NonSystemToken("Name.Space.FakeFunction1", null, null), resolver, dummyBindingType, DefaultUriResolver);
            segment.ShouldBeOperationSegment(dummyOperations /* TODO: parameters */);
        }

        private static ODataPathSegment BindQualifiedOperationToPerson(string identifier)
        {
            ODataPathSegment segment = SelectPathSegmentTokenBinder.ConvertNonTypeTokenToSegment(
                new NonSystemToken(identifier, null, null), 
                HardCodedTestModel.TestModel,
                HardCodedTestModel.GetPersonType(),
                DefaultUriResolver);
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

        private class FindOperationsByBindingParameterTypeHierarchyThrowingNonCatchableExceptionEdmModel : EdmModel
        {
            public override IEnumerable<IEdmOperation> FindDeclaredBoundOperations(IEdmType bindingType)
            {
                throw new OutOfMemoryException("OutOfMemoryException");
            }
        }
    }
}

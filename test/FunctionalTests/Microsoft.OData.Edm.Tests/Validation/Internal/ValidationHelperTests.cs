//---------------------------------------------------------------------
// <copyright file="ValidationHelperTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Validation.Internal;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Validation.Internal
{
    /// <summary>
    /// ValidationRulesTests tests
    /// </summary>
    public class ValidationHelperTests
    {
        [Fact]
        public void EnsureDuplicateEntityTypeAndFunctionReturnTrue()
        {
            EdmModel otherModel = new EdmModel();
            var entityType = new EdmEntityType("n.s", "GetStuff");
            otherModel.AddElement(entityType);

            var edmFunction = new EdmFunction("n.s", "GetStuff", new EdmEntityTypeReference(entityType, false), false /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddReferencedModel(otherModel);

            model.OperationOrNameExistsInReferencedModel(edmFunction, edmFunction.FullName()).Should().BeTrue();
        }

        [Fact]
        public void EnsureDuplicateTermAndFunctionReturnTrue()
        {
            EdmModel model = new EdmModel();
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), false /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            model.AddElement(edmFunction);

            EdmModel otherModel = new EdmModel();
            var edmTerm = new EdmTerm("n.s", "GetStuff", EdmPrimitiveTypeKind.Int32);
            otherModel.AddElement(edmTerm);
            model.AddReferencedModel(otherModel);

            model.OperationOrNameExistsInReferencedModel(edmFunction, edmFunction.FullName()).Should().BeTrue();
        }

        [Fact]
        public void EnsureDuplicateContainerFunctionReturnTrue()
        {
            EdmModel model = new EdmModel();
            var edmFunction = new EdmFunction("n.s", "GetStuff", EdmCoreModel.Instance.GetString(true), false /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            model.AddElement(edmFunction);

            EdmModel otherModel = new EdmModel();
            EdmEntityContainer container = new EdmEntityContainer("n.s", "GetStuff");
            otherModel.AddElement(container);
            model.AddReferencedModel(otherModel);

            model.OperationOrNameExistsInReferencedModel(edmFunction, edmFunction.FullName()).Should().BeTrue();
        }

        [Fact]
        public void EnsureNoDuplicateFoundForFunctionShouldReturnFalse()
        {
            EdmModel otherModel = new EdmModel();
            var entityType = new EdmEntityType("n.s", "GetStuff2");
            otherModel.AddElement(entityType);

            var edmFunction = new EdmFunction("n.s", "GetStuff", new EdmEntityTypeReference(entityType, false), false /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            EdmModel model = new EdmModel();
            model.AddElement(edmFunction);
            model.AddReferencedModel(otherModel);

            model.OperationOrNameExistsInReferencedModel(edmFunction, edmFunction.FullName()).Should().BeFalse();
        }
    }
}

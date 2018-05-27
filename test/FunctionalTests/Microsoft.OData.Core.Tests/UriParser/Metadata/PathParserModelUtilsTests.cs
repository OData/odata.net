//---------------------------------------------------------------------
// <copyright file="PathParserModelUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Metadata;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Metadata
{
    public class PathParserModelUtilsTests
    {
        private readonly IEdmEntityType nonOpenEdmEntityType = new EdmEntityType("Fake", "Type", null, false, false);
        private readonly IEdmEntityType openEdmEntityType = new EdmEntityType("Fake", "OpenType", null, false, true);
        private readonly IEdmComplexType nonOpenEdmComplexType = new EdmComplexType("Fake", "Complex", null, false);
        private readonly IEdmComplexType openEdmComplexType = new OpenComplexType("Fake", "OpenComplex");
        private readonly IEdmPrimitiveType edmPrimitiveType = (IEdmPrimitiveType)EdmCoreModel.Instance.GetInt32(false).Definition;
        
        [Fact]
        public void PrimitiveTypeIsNotOpen()
        {
            this.edmPrimitiveType.IsOpen().Should().BeFalse();
        }

        [Fact]
        public void NonOpenEntityTypeIsNotOpen()
        {
            this.nonOpenEdmEntityType.IsOpen().Should().BeFalse();
        }

        [Fact]
        public void NonOpenComplexTypeIsNotOpen()
        {
            this.nonOpenEdmComplexType.IsOpen().Should().BeFalse();
        }

        [Fact]
        public void OpenEntityTypeIsOpen()
        {
            this.openEdmEntityType.IsOpen().Should().BeTrue();
        }

        [Fact]
        public void OpenComplexTypeIsOpen()
        {
            this.openEdmComplexType.IsOpen().Should().BeTrue();
        }

        [Fact]
        public void PrimitiveCollectionTypeIsNotOpen()
        {
            this.ToCollection(this.edmPrimitiveType).IsOpen().Should().BeFalse();
        }

        [Fact]
        public void NonOpenEntityCollectionTypeIsNotOpen()
        {
            this.ToCollection(this.nonOpenEdmEntityType).IsOpen().Should().BeFalse();
        }

        [Fact]
        public void NonOpenComplexCollectionTypeIsNotOpen()
        {
            this.ToCollection(this.nonOpenEdmComplexType).IsOpen().Should().BeFalse();
        }

        [Fact]
        public void OpenEntityCollectionTypeIsOpen()
        {
            // TODO: when SingleResult is removed from the semantic path parser, change this to return false.
             this.ToCollection(this.openEdmEntityType).IsOpen().Should().BeTrue();
        }

        [Fact]
        public void OpenComplexCollectionTypeIsOpen()
        {
            this.ToCollection(this.openEdmComplexType).IsOpen().Should().BeTrue();
        }

        [Fact]
        public void EntityTypeIsAnEntityType()
        {
            IEdmEntityType result;
            this.nonOpenEdmEntityType.IsEntityOrEntityCollectionType(out result).Should().BeTrue();
            result.Should().BeSameAs(this.nonOpenEdmEntityType);
        }

        [Fact]
        public void EntityCollectionTypeIsAnEntityType()
        {
            IEdmEntityType result;
            this.ToCollection(this.nonOpenEdmEntityType).IsEntityOrEntityCollectionType(out result).Should().BeTrue();
            result.Should().BeSameAs(this.nonOpenEdmEntityType);
        }

        [Fact]
        public void ComplexTypeIsNotAnEntityType()
        {
            IEdmEntityType result;
            this.nonOpenEdmComplexType.IsEntityOrEntityCollectionType(out result).Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void ComplexCollectionTypeIsNotAnEntityType()
        {
            IEdmEntityType result;
            this.ToCollection(this.nonOpenEdmComplexType).IsEntityOrEntityCollectionType(out result).Should().BeFalse();
            result.Should().BeNull();
        }

        [Fact]
        public void GetTargetEntitySetForFunctionWithStaticSet()
        {
            var model = new EdmModel();
            var container = new EdmEntityContainer("Fake", "Container");
            model.AddElement(container);
            var entitySet = container.AddEntitySet("EntitySet", new EdmEntityType("Fake", "EntityType"));
            var function = new EdmFunction("Fake", "FakeFunction", new EdmEntityTypeReference(entitySet.EntityType(), false));
            var operationImport = container.AddFunctionImport("FakeAction", function, new EdmPathExpression(entitySet.Name));
            operationImport.GetTargetEntitySet(null, model).Should().BeSameAs(entitySet);
        }

        [Fact]
        public void GetTargetEntitySetForFunctionWithNoStaticSetOrSourceSetShouldBeNull()
        {
            var model = new EdmModel();
            var container = new EdmEntityContainer("Fake", "Container");
            model.AddElement(container);
            var function = new EdmFunction("Fake", "FakeFunction", new EdmEntityTypeReference(new EdmEntityType("Fake", "EntityType"), false));
            var expression = new EdmPathExpression("p1/Navigation1");
            var operationImport = container.AddFunctionImport("FakeAction", function, expression);
            operationImport.GetTargetEntitySet(null, model).Should().BeNull();
        }

        [Fact]
        public void GetTargetEntitySetForFunctionWithPath()
        {
            IEdmEntitySet targetEntitySet;
            GetTargetEntitySet(new EdmPathExpression("p1/Navigation1/Navigation2"), out targetEntitySet, addParameters: true).Should().BeSameAs(targetEntitySet);
        }

        [Fact]
        public void GetTargetEntitySetForFunctionWithPathThatDoesntHaveASetShouldBeNull()
        {
            IEdmEntitySet targetEntitySet;
            Assert.True(GetTargetEntitySet(new EdmPathExpression("p1/Navigation2"), out targetEntitySet, addParameters: true) is IEdmUnknownEntitySet);
        }

        [Fact]
        public void GetTargetEntitySetForFunctionWithPathThatStartsFromNonBindingParameterShouldFail()
        {
            IEdmEntitySet targetEntitySet;
            Action getTargetSet = () => GetTargetEntitySet(new EdmPathExpression("p2/Navigation1"), out targetEntitySet, addParameters: true);
            getTargetSet.ShouldThrow<ODataException>();
        }

        [Fact]
        public void GetTargetEntitySetForFunctionWithNeitherPathNorStaticSetShouldBeNull()
        {
            IEdmEntitySet targetEntitySet;
            GetTargetEntitySet(null, out targetEntitySet).Should().BeNull();
        }

        [Fact]
        public void GetTargetEntitySetForFunctionWithPathButNoParametersShouldBeNull()
        {
            IEdmEntitySet targetEntitySet;
            GetTargetEntitySet(new EdmPathExpression("p2/Navigation1"), out targetEntitySet).Should().BeNull();
        }

        [Fact]
        public void GetTargetEntitySetForNonBinadbleFunctionWithPathShouldBeNull()
        {
            IEdmEntitySet targetEntitySet;
            GetTargetEntitySet(new EdmPathExpression("p1/Navigation1/Navigation2"), out targetEntitySet, isBindable: false, addParameters: true).Should().BeNull();
        }

        [Fact]
        public void GetTargetEntitySetForSingleton()
        {
            var model = new EdmModel();
            var container = new EdmEntityContainer("Fake", "Container");
            model.AddElement(container);
            var edmEntityType = new EdmEntityType("Fake", "EntityType");

            var singleton = container.AddSingleton("Singleton", edmEntityType);

            var function = new EdmFunction("Fake", "FakeFunction", new EdmEntityTypeReference(edmEntityType, false), true, new EdmPathExpression("bindingparameter"), false);
            function.AddParameter("bindingparameter", new EdmEntityTypeReference(edmEntityType, false));
            var target = function.GetTargetEntitySet(singleton, model);
            target.Should().BeNull();
        }

        private static IEdmEntitySetBase GetTargetEntitySet(EdmPathExpression edmPathExpression, out IEdmEntitySet targetEntitySet, bool addParameters = false, bool isBindable = true)
        {
            var model = new EdmModel();
            var container = new EdmEntityContainer("Fake", "Container");
            model.AddElement(container);
            var edmEntityType = new EdmEntityType("Fake", "EntityType");

            var sourceEntitySet = container.AddEntitySet("SourceEntitySet", edmEntityType);
            var middleEntitySet = container.AddEntitySet("MiddleEntitySet", edmEntityType);
            targetEntitySet = container.AddEntitySet("TargetEntitySet", edmEntityType);

            var nav1 = edmEntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "Navigation1", Target = edmEntityType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
            var nav2 = edmEntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "Navigation2", Target = edmEntityType, TargetMultiplicity = EdmMultiplicity.Many });

            sourceEntitySet.AddNavigationTarget(nav1, middleEntitySet);
            middleEntitySet.AddNavigationTarget(nav2, targetEntitySet);

            var action = new EdmAction("Fake", "FakeAction", new EdmEntityTypeReference(edmEntityType, false), isBindable, null /*EntitySetPath*/);
            if (addParameters)
            {
                action.AddParameter("p1", new EdmEntityTypeReference(edmEntityType, false));
                action.AddParameter("p2", new EdmEntityTypeReference(edmEntityType, false));
            }

            var actionImport = container.AddActionImport("FakeAction", action, edmPathExpression);
          
            return actionImport.GetTargetEntitySet(sourceEntitySet, model);
        }

        private IEdmCollectionType ToCollection(IEdmType type)
        {
            switch (type.TypeKind)
            {
                case EdmTypeKind.Entity:
                    return new EdmCollectionType(new EdmEntityTypeReference((IEdmEntityType)type, false));
                case EdmTypeKind.Complex:
                    return new EdmCollectionType(new EdmComplexTypeReference((IEdmComplexType)type, false));
                case EdmTypeKind.Primitive:
                    return new EdmCollectionType(new EdmPrimitiveTypeReference((IEdmPrimitiveType)type, false));
            }

            throw new NotImplementedException();
        }

        private class OpenComplexType : EdmComplexType, IEdmStructuredType
        {
            public OpenComplexType(string namespaceName, string name)
                : base(namespaceName, name)
            { 
            }

            bool IEdmStructuredType.IsOpen { get { return true; } }
        }
    }
}
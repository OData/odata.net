//---------------------------------------------------------------------
// <copyright file="PathParserModelUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
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
            Assert.False(this.edmPrimitiveType.IsOpen());
        }

        [Fact]
        public void NonOpenEntityTypeIsNotOpen()
        {
            Assert.False(this.nonOpenEdmEntityType.IsOpen());
        }

        [Fact]
        public void NonOpenComplexTypeIsNotOpen()
        {
            Assert.False(this.nonOpenEdmComplexType.IsOpen());
        }

        [Fact]
        public void OpenEntityTypeIsOpen()
        {
            Assert.True(this.openEdmEntityType.IsOpen());
        }

        [Fact]
        public void OpenComplexTypeIsOpen()
        {
            Assert.True(this.openEdmComplexType.IsOpen());
        }

        [Fact]
        public void PrimitiveCollectionTypeIsNotOpen()
        {
            Assert.False(this.ToCollection(this.edmPrimitiveType).IsOpen());
        }

        [Fact]
        public void NonOpenEntityCollectionTypeIsNotOpen()
        {
            Assert.False(this.ToCollection(this.nonOpenEdmEntityType).IsOpen());
        }

        [Fact]
        public void NonOpenComplexCollectionTypeIsNotOpen()
        {
            Assert.False(this.ToCollection(this.nonOpenEdmComplexType).IsOpen());
        }

        [Fact]
        public void OpenEntityCollectionTypeIsOpen()
        {
            // TODO: when SingleResult is removed from the semantic path parser, change this to return false.
            Assert.True(this.ToCollection(this.openEdmEntityType).IsOpen());
        }

        [Fact]
        public void OpenComplexCollectionTypeIsOpen()
        {
            Assert.True(this.ToCollection(this.openEdmComplexType).IsOpen());
        }

        [Fact]
        public void EntityTypeIsAnEntityType()
        {
            IEdmEntityType result;
            Assert.True(this.nonOpenEdmEntityType.IsEntityOrEntityCollectionType(out result));
            Assert.Same(this.nonOpenEdmEntityType, result);
        }

        [Fact]
        public void EntityCollectionTypeIsAnEntityType()
        {
            IEdmEntityType result;
            Assert.True(this.ToCollection(this.nonOpenEdmEntityType).IsEntityOrEntityCollectionType(out result));
            Assert.Same(this.nonOpenEdmEntityType, result);
        }

        [Fact]
        public void ComplexTypeIsNotAnEntityType()
        {
            IEdmEntityType result;
            Assert.False(this.nonOpenEdmComplexType.IsEntityOrEntityCollectionType(out result));
            Assert.Null(result);
        }

        [Fact]
        public void ComplexCollectionTypeIsNotAnEntityType()
        {
            IEdmEntityType result;
            Assert.False(this.ToCollection(this.nonOpenEdmComplexType).IsEntityOrEntityCollectionType(out result));
            Assert.Null(result);
        }

        [Fact]
        public void GetTargetEntitySetForFunctionWithStaticSet()
        {
            var model = new EdmModel();
            var container = new EdmEntityContainer("Fake", "Container");
            model.AddElement(container);
            var entitySet = container.AddEntitySet("EntitySet", new EdmEntityType("Fake", "EntityType"));
            var function = new EdmFunction("Fake", "FakeFunction", new EdmEntityTypeReference(entitySet.EntityType, false));
            var operationImport = container.AddFunctionImport("FakeAction", function, new EdmPathExpression(entitySet.Name));
            Assert.Same(entitySet, operationImport.GetTargetEntitySet(null, model));
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
            Assert.Null(operationImport.GetTargetEntitySet(null, model));
        }

        [Fact]
        public void GetTargetEntitySetForFunctionWithPath()
        {
            IEdmEntitySet targetEntitySet;
            Assert.Same(GetTargetEntitySet(new EdmPathExpression("p1/Navigation1/Navigation2"), out targetEntitySet, addParameters: true), targetEntitySet);
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
            Assert.Throws<ODataException>(getTargetSet);
        }

        [Fact]
        public void GetTargetEntitySetForFunctionWithNeitherPathNorStaticSetShouldBeNull()
        {
            IEdmEntitySet targetEntitySet;
            Assert.Null(GetTargetEntitySet(null, out targetEntitySet));
        }

        [Fact]
        public void GetTargetEntitySetForFunctionWithPathButNoParametersShouldBeNull()
        {
            IEdmEntitySet targetEntitySet;
            Assert.Null(GetTargetEntitySet(new EdmPathExpression("p2/Navigation1"), out targetEntitySet));
        }

        [Fact]
        public void GetTargetEntitySetForNonBinadbleFunctionWithPathShouldBeNull()
        {
            IEdmEntitySet targetEntitySet;
            Assert.Null(GetTargetEntitySet(new EdmPathExpression("p1/Navigation1/Navigation2"), out targetEntitySet, isBindable: false, addParameters: true));
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
            Assert.Null(target);
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
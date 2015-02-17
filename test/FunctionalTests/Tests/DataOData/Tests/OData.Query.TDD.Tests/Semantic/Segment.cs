//---------------------------------------------------------------------
// <copyright file="Segment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    #region Namespaces
    using Microsoft.OData.Core;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Query;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData.Core.Query.SemanticAst;
    #endregion Namespaces

    /// <summary>
    /// Test code for semantic parsing of multiple segments and complex types
    /// </summary>
    [TestClass]
    public class Complex
    {
        private IEdmModel edmModel = ModelMaker.GetEdmModel();

        [TestMethod]
        public void NavigationPropertiesShouldCreateCorrectSemanticNode()
        {
            var semanticTree = ModelMaker.GetSemanticTree("People?$filter=MyDog/Color eq 'Magenta'", this.edmModel);

            var cmpDog = this.edmModel.FindType("MvcApplication3.Dog");
            semanticTree.Query.As<FilterQueryNode>().Expression.As<BinaryOperatorQueryNode>().Left.As
                <PropertyAccessQueryNode>().Source.As<SingletonNavigationNode>().TypeReference.Definition.Should().Be(cmpDog);
        }

        [TestMethod]
        public void NavigationPropertiesShouldHaveAssociatedTargetMultiplicity()
        {
            var semanticTree = ModelMaker.GetSemanticTree("People?$filter=MyDog/Color eq 'Magenta'", this.edmModel);

            semanticTree.Query.As<FilterQueryNode>().Expression.As<BinaryOperatorQueryNode>().Left.As
                <PropertyAccessQueryNode>().Source.As<SingletonNavigationNode>().TargetMultiplicity.Should().Be(EdmMultiplicity.ZeroOrOne);
        }

        [TestMethod]
        public void WhenAnyIsLedBySingletonNavigationPropertyShouldFail()
        {
            try
            {
                ModelMaker.GetSemanticTree("People?$filter=MyDog/MyPeople/MyDog/MyPeople/any(a: a/MyDog/Color eq 'Simba' )", this.edmModel);
            }
            catch (ODataException)
            {
                return;
            }
            throw new ODataException();
        }

        [TestMethod]
        public void ComplexTypeShouldCreateCorrectSemanticNode()
        {
            var semanticTree = ModelMaker.GetSemanticTree("People?$filter=MyAddress/City eq 'Tacoma'", this.edmModel);

            var cmpAddress = this.edmModel.FindType("MvcApplication3.Address");
            semanticTree.Query.As<FilterQueryNode>().Expression.As<BinaryOperatorQueryNode>().
                Left.As<PropertyAccessQueryNode>().Source.As<PropertyAccessQueryNode>().TypeReference.Definition.Should().Be(cmpAddress);
        }

        [TestMethod]
        public void ComplexTypesShouldBeNestable()
        {
            var semanticTree = ModelMaker.GetSemanticTree("People?$filter=MyAddress/NextHome/NextHome/City eq 'Tacoma'", this.edmModel);

            var cmpAddress = this.edmModel.FindType("MvcApplication3.Address");
            semanticTree.Query.As<FilterQueryNode>().Expression.As<BinaryOperatorQueryNode>().
                Left.As<PropertyAccessQueryNode>().Source.As<PropertyAccessQueryNode>().TypeReference.Definition.Should().Be(cmpAddress);
            semanticTree.Query.As<FilterQueryNode>().Expression.As<BinaryOperatorQueryNode>().
                Left.As<PropertyAccessQueryNode>().Source.As<PropertyAccessQueryNode>().Source.As<PropertyAccessQueryNode>().TypeReference.Definition.Should().Be(cmpAddress);
        }

        [TestMethod]
        public void ComplexTypesShouldBeAllowedInAny()
        {
            var semanticTree = ModelMaker.GetSemanticTree("People?$filter=MyDog/MyPeople/any(a: a/MyAddress/City eq 'Renton')", this.edmModel);

            var cmpAddress = this.edmModel.FindType("MvcApplication3.Address");
            semanticTree.Query.As<FilterQueryNode>().Expression.As<AnyQueryNode>().Body.As<BinaryOperatorQueryNode>().
                Left.As<PropertyAccessQueryNode>().Source.As<PropertyAccessQueryNode>().TypeReference.Definition.Should().Be(cmpAddress);
        }

        [TestMethod]
        public void ComplexTypesShouldBeCastable()
        {
            var semanticTree = ModelMaker.GetSemanticTree("People?$filter=MyDog/MyPeople/any(a: a/MyAddress/MvcApplication3.Address/City eq 'Renton')", this.edmModel);

            var cmpAddress = this.edmModel.FindType("MvcApplication3.Address");
            semanticTree.Query.As<FilterQueryNode>().Expression.As<AnyQueryNode>().Body.As<BinaryOperatorQueryNode>().
                Left.As<PropertyAccessQueryNode>().Source.As<CastNode>().TypeReference.Definition.Should().Be(cmpAddress);
        }

        [TestMethod]
        public void PrimitiveCollectionTypeShouldBeAllowedUnderComplexType()
        {
            var semanticTree = ModelMaker.GetSemanticTree("People?$filter=MyAddress/MyNeighbors/any(a: a eq 'Christy' )", this.edmModel);

            var cmp = this.edmModel.FindType("Edm.String");
            semanticTree.Query.As<FilterQueryNode>().Expression.As<AnyQueryNode>().Body.As<BinaryOperatorQueryNode>().
                Left.As<ParameterQueryNode>().TypeReference.Definition.Should().Be(cmp);
        }
    }
}

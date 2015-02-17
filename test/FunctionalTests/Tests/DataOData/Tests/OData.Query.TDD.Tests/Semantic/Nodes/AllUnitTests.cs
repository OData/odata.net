//---------------------------------------------------------------------
// <copyright file="AllUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    #region Namespaces
    using System.Collections.ObjectModel;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.Parsers;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Unit tests for the AllNode class
    /// </summary>
    [TestClass]
    public class AllUnitTests
    {
        [TestMethod]
        public void RangeVariablesShouldBeSetCorrectly()
        {
            EntityRangeVariable rangeVariable = (EntityRangeVariable)NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            Collection<RangeVariable> rangeVariables = new Collection<RangeVariable>
                {
                    rangeVariable
                };
            AllNode allNode = new AllNode(rangeVariables, rangeVariable);
            allNode.RangeVariables.Count.Should().Be(1);
            allNode.RangeVariables[0].Name.Should().Be(ExpressionConstants.It);
            allNode.RangeVariables[0].Kind.Should().Be(RangeVariableKind.Entity);
            EntityRangeVariable returnedRangeVariable = (EntityRangeVariable)allNode.RangeVariables[0];
            returnedRangeVariable.NavigationSource.Should().Be(HardCodedTestModel.GetDogsSet());
        }

        [TestMethod]
        public void RangeVariableShouldBeSetCorrectly()
        {
            EntityRangeVariable rangeVariable = (EntityRangeVariable)NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            Collection<RangeVariable> rangeVariables = new Collection<RangeVariable>
                {
                    rangeVariable
                };
            AllNode allNode = new AllNode(rangeVariables, rangeVariable);
            allNode.CurrentRangeVariable.Name.Should().Be(ExpressionConstants.It);
            allNode.CurrentRangeVariable.Kind.Should().Be(RangeVariableKind.Entity);
            allNode.CurrentRangeVariable.TypeReference.FullName().Should().Be(HardCodedTestModel.GetDogTypeReference().FullName());
        }

        [TestMethod]
        public void TypeReferenceShouldBeBoolean()
        {
            EntityRangeVariable rangeVariable = (EntityRangeVariable)NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            Collection<RangeVariable> rangeVariables = new Collection<RangeVariable>
                {
                    rangeVariable
                };
            AllNode allNode = new AllNode(rangeVariables, rangeVariable);
            allNode.TypeReference.FullName().Should().Be("Edm.Boolean");
        }

        [TestMethod]
        public void KindShouldBeAllNode()
        {
            EntityRangeVariable rangeVariable = (EntityRangeVariable)NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            Collection<RangeVariable> rangeVariables = new Collection<RangeVariable>
                {
                    rangeVariable
                };
            AllNode allNode = new AllNode(rangeVariables, rangeVariable);
            allNode.InternalKind.Should().Be(InternalQueryNodeKind.All);
        }
    }
}

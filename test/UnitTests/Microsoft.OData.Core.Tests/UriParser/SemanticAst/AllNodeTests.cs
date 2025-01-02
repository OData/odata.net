//---------------------------------------------------------------------
// <copyright file="AllNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.ObjectModel;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the AllNode class
    /// </summary>
    public class AllNodeTests
    {
        [Fact]
        public void RangeVariablesShouldBeSetCorrectly()
        {
            ResourceRangeVariable rangeVariable = (ResourceRangeVariable)NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            Collection<RangeVariable> rangeVariables = new Collection<RangeVariable>
                {
                    rangeVariable
                };
            AllNode allNode = new AllNode(rangeVariables, rangeVariable);
            RangeVariable rangeVar = Assert.Single(allNode.RangeVariables);
            Assert.Equal(ExpressionConstants.It, rangeVar.Name);
            Assert.Equal(RangeVariableKind.Resource, rangeVar.Kind);
            ResourceRangeVariable returnedRangeVariable = Assert.IsType<ResourceRangeVariable>(rangeVar);
            Assert.Same(HardCodedTestModel.GetDogsSet(), returnedRangeVariable.NavigationSource);
        }

        [Fact]
        public void RangeVariableShouldBeSetCorrectly()
        {
            ResourceRangeVariable rangeVariable = (ResourceRangeVariable)NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            Collection<RangeVariable> rangeVariables = new Collection<RangeVariable>
                {
                    rangeVariable
                };
            AllNode allNode = new AllNode(rangeVariables, rangeVariable);
            Assert.Equal(ExpressionConstants.It, allNode.CurrentRangeVariable.Name);
            Assert.Equal(RangeVariableKind.Resource, allNode.CurrentRangeVariable.Kind);
            Assert.Equal(HardCodedTestModel.GetDogTypeReference().FullName(), allNode.CurrentRangeVariable.TypeReference.FullName());
        }

        [Fact]
        public void TypeReferenceShouldBeBoolean()
        {
            ResourceRangeVariable rangeVariable = (ResourceRangeVariable)NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            Collection<RangeVariable> rangeVariables = new Collection<RangeVariable>
                {
                    rangeVariable
                };
            AllNode allNode = new AllNode(rangeVariables, rangeVariable);
            Assert.Equal("Edm.Boolean", allNode.TypeReference.FullName());
        }

        [Fact]
        public void KindShouldBeAllNode()
        {
            ResourceRangeVariable rangeVariable = (ResourceRangeVariable)NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            Collection<RangeVariable> rangeVariables = new Collection<RangeVariable>
                {
                    rangeVariable
                };
            AllNode allNode = new AllNode(rangeVariables, rangeVariable);
            Assert.Equal(InternalQueryNodeKind.All, allNode.InternalKind);
        }
    }
}

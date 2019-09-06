//---------------------------------------------------------------------
// <copyright file="AnyNodeTests.cs" company="Microsoft">
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
    /// Unit tests for the AnyNode class
    /// </summary>
    public class AnyNodeTests
    {
        [Fact]
        public void RangeVariablesShouldBeSetCorrectly()
        {
            ResourceRangeVariable rangeVariable = (ResourceRangeVariable)NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            Collection<RangeVariable> rangeVariables = new Collection<RangeVariable>
                {
                    rangeVariable
                };
            AnyNode anyNode = new AnyNode(rangeVariables, rangeVariable);
            Assert.Single(anyNode.RangeVariables);
            Assert.Equal(ExpressionConstants.It, anyNode.RangeVariables[0].Name);
            Assert.Equal(RangeVariableKind.Resource, anyNode.RangeVariables[0].Kind);
            Assert.Equal(HardCodedTestModel.GetDogTypeReference().FullName(), anyNode.RangeVariables[0].TypeReference.FullName());
            ResourceRangeVariable returnedRangeVariable = (ResourceRangeVariable)anyNode.RangeVariables[0];
            Assert.Same(HardCodedTestModel.GetDogsSet(), returnedRangeVariable.NavigationSource);
        }

        [Fact]
        public void RangeVariableShouldBeSetCorrectly()
        {
            ResourceRangeVariable rangeVariable =
                (ResourceRangeVariable)
                NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            Collection<RangeVariable> rangeVariables = new Collection<RangeVariable>
                {
                    rangeVariable
                };
            AnyNode anyNode = new AnyNode(rangeVariables, rangeVariable);
            Assert.Equal(ExpressionConstants.It, anyNode.CurrentRangeVariable.Name);
            Assert.Equal(RangeVariableKind.Resource, anyNode.CurrentRangeVariable.Kind);
            Assert.Equal(HardCodedTestModel.GetDogTypeReference().FullName(), anyNode.CurrentRangeVariable.TypeReference.FullName());
        }

        [Fact]
        public void TypeReferenceShouldBeBoolean()
        {
            ResourceRangeVariable rangeVariable = (ResourceRangeVariable)NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            Collection<RangeVariable> rangeVariables = new Collection<RangeVariable>
                {
                    rangeVariable
                };
            AnyNode anyNode = new AnyNode(rangeVariables, rangeVariable);
            Assert.Equal("Edm.Boolean", anyNode.TypeReference.FullName());
        }

        [Fact]
        public void KindShouldBeAnyNode()
        {
            ResourceRangeVariable rangeVariable = (ResourceRangeVariable)NodeFactory.CreateImplicitRangeVariable(HardCodedTestModel.GetDogTypeReference(), HardCodedTestModel.GetDogsSet());
            Collection<RangeVariable> rangeVariables = new Collection<RangeVariable>
                {
                    rangeVariable
                };
            AnyNode anyNode = new AnyNode(rangeVariables, rangeVariable);
            Assert.Equal(InternalQueryNodeKind.Any, anyNode.InternalKind);
        }
    }
}

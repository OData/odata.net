//---------------------------------------------------------------------
// <copyright file="AllNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.ObjectModel;
using FluentAssertions;
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
            allNode.RangeVariables.Count.Should().Be(1);
            allNode.RangeVariables[0].Name.Should().Be(ExpressionConstants.It);
            allNode.RangeVariables[0].Kind.Should().Be(RangeVariableKind.Resource);
            ResourceRangeVariable returnedRangeVariable = (ResourceRangeVariable)allNode.RangeVariables[0];
            returnedRangeVariable.NavigationSource.Should().Be(HardCodedTestModel.GetDogsSet());
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
            allNode.CurrentRangeVariable.Name.Should().Be(ExpressionConstants.It);
            allNode.CurrentRangeVariable.Kind.Should().Be(RangeVariableKind.Resource);
            allNode.CurrentRangeVariable.TypeReference.FullName().Should().Be(HardCodedTestModel.GetDogTypeReference().FullName());
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
            allNode.TypeReference.FullName().Should().Be("Edm.Boolean");
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
            allNode.InternalKind.Should().Be(InternalQueryNodeKind.All);
        }
    }
}

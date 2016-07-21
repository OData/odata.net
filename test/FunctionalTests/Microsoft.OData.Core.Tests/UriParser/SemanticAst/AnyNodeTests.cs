//---------------------------------------------------------------------
// <copyright file="AnyNodeTests.cs" company="Microsoft">
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
            anyNode.RangeVariables.Count.Should().Be(1);
            anyNode.RangeVariables[0].Name.Should().Be(ExpressionConstants.It);
            anyNode.RangeVariables[0].Kind.Should().Be(RangeVariableKind.Resource);
            anyNode.RangeVariables[0].TypeReference.FullName().Should().Be(HardCodedTestModel.GetDogTypeReference().FullName());
            ResourceRangeVariable returnedRangeVariable = (ResourceRangeVariable)anyNode.RangeVariables[0];
            returnedRangeVariable.NavigationSource.Should().Be(HardCodedTestModel.GetDogsSet());
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
            anyNode.CurrentRangeVariable.Name.Should().Be(ExpressionConstants.It);
            anyNode.CurrentRangeVariable.Kind.Should().Be(RangeVariableKind.Resource);
            anyNode.CurrentRangeVariable.TypeReference.FullName().Should().Be(HardCodedTestModel.GetDogTypeReference().FullName());
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
            anyNode.TypeReference.FullName().Should().Be("Edm.Boolean");
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
            anyNode.InternalKind.Should().Be(InternalQueryNodeKind.Any);
        }
    }
}

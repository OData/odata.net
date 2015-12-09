//---------------------------------------------------------------------
// <copyright file="AllNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.ObjectModel;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the AllNode class
    /// </summary>
    public class AllNodeTests
    {
        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

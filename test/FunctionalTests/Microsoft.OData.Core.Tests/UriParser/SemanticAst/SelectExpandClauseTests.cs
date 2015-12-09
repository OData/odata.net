//---------------------------------------------------------------------
// <copyright file="SelectExpandClauseTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the SelectExpandClause class.
    /// </summary>
    public class SelectExpandClauseTests
    {
        [Fact]
        public void ConstructorSetsAllSeletedProperty()
        {
            var clause = new SelectExpandClause(null, false);
            clause.AllSelected.Should().BeFalse();
        }
    
        [Fact]
        public void ConstructorSetsSelectedItemsProperty()
        {
            var expansions = new List<ExpandedNavigationSelectItem>();
            var clause = new SelectExpandClause(expansions, true);
            clause.SelectedItems.Should().NotBeNull();
        }

        [Fact]
        public void SuccessfullyAddSelectionItems()
        {
            ODataSelectPath personNamePath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonNameProp()));
            ODataSelectPath personShoePath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonShoeProp()));
            var clause = new SelectExpandClause(new List<ExpandedNavigationSelectItem>(), false);
            clause.AddToSelectedItems(new PathSelectItem(personShoePath));
            clause.AddToSelectedItems(new PathSelectItem(personNamePath));
            clause.SelectedItems.Should().HaveCount(2).And.Contain(x => x is PathSelectItem && x.As<PathSelectItem>().SelectedPath == personNamePath).And.Contain(x => x is PathSelectItem && x.As<PathSelectItem>().SelectedPath == personShoePath);
        }
    }
}

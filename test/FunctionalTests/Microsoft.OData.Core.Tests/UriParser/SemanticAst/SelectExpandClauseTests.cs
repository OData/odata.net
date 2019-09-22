//---------------------------------------------------------------------
// <copyright file="SelectExpandClauseTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
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
            Assert.False(clause.AllSelected);
        }
    
        [Fact]
        public void ConstructorSetsSelectedItemsProperty()
        {
            var expansions = new List<ExpandedNavigationSelectItem>();
            var clause = new SelectExpandClause(expansions, true);
            Assert.NotNull(clause.SelectedItems);
        }

        [Fact]
        public void SuccessfullyAddSelectionItems()
        {
            ODataSelectPath personNamePath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonNameProp()));
            ODataSelectPath personShoePath = new ODataSelectPath(new PropertySegment(HardCodedTestModel.GetPersonShoeProp()));
            var clause = new SelectExpandClause(new List<ExpandedNavigationSelectItem>(), false);
            clause.AddToSelectedItems(new PathSelectItem(personShoePath));
            clause.AddToSelectedItems(new PathSelectItem(personNamePath));
            Assert.Equal(2, clause.SelectedItems.Count());
            Assert.Contains(clause.SelectedItems, x => x is PathSelectItem && ((PathSelectItem)x).SelectedPath == personNamePath);
            Assert.Contains(clause.SelectedItems, x => x is PathSelectItem && ((PathSelectItem)x).SelectedPath == personShoePath);
        }
    }
}

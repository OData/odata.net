//---------------------------------------------------------------------
// <copyright file="SelectExpandClauseFinisherTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Binders
{
    public class SelectExpandClauseFinisherTests
    {
        [Fact]
        public void ExpandedNavigationPropertiesAreImplicitlyAddedAsPathSelectionItemsIfSelectIsPopulated()
        {
            IEdmNavigationProperty navigationProperty = ModelBuildingHelpers.BuildValidNavigationProperty();
            IEdmStructuralProperty structuralProperty = ModelBuildingHelpers.BuildValidPrimitiveProperty();
            SelectExpandClause clause = new SelectExpandClause(new SelectItem[]
            {
                new PathSelectItem(new ODataSelectPath(new PropertySegment(structuralProperty))),
                new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(navigationProperty, ModelBuildingHelpers.BuildValidEntitySet())), ModelBuildingHelpers.BuildValidEntitySet(), new SelectExpandClause(new List<SelectItem>(), false)),
            },
            false /*allSelected*/);
            SelectExpandClauseFinisher.AddExplicitNavPropLinksWhereNecessary(clause);
            clause.SelectedItems.Should().HaveCount(2)
                .And.Contain(x => x is PathSelectItem && x.As<PathSelectItem>().SelectedPath.LastSegment is PropertySegment && x.As<PathSelectItem>().SelectedPath.LastSegment.As<PropertySegment>().Property.Name == structuralProperty.Name);
        }

        [Fact]
        public void ExpandedNavigationPropertiesAreNotAddedAsPathSelectionItemsIfSelectIsNotPopulated()
        {
            SelectExpandClause clause = new SelectExpandClause(new SelectItem[]
            {
                new ExpandedNavigationSelectItem(new ODataExpandPath(new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), ModelBuildingHelpers.BuildValidEntitySet())), ModelBuildingHelpers.BuildValidEntitySet(), new SelectExpandClause(new List<SelectItem>(), false)),
            },
            false /*allSelected*/);
            SelectExpandClauseFinisher.AddExplicitNavPropLinksWhereNecessary(clause);
            clause.SelectedItems.Should().HaveCount(1);
        }
    }
}

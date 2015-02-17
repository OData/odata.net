//---------------------------------------------------------------------
// <copyright file="SelectionAssertions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Test.OData.Query.TDD.Tests.TestUtilities
{
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;

    /// <summary>
    /// Contains fluent assertion APIs for testing Selection objects.
    /// </summary>
    internal static class SelectionAssertions
    {
        public static AndConstraint<T> ShouldBeSelectedItemOfType<T>(this SelectItem actual) where T : SelectItem
        {
            actual.Should().BeOfType<T>();
            return new AndConstraint<T>(actual as T);
        }

        public static AndConstraint<PathSelectItem> ShouldBePathSelectionItem(this SelectItem actual, ODataPath path)
        {
            var andConstraint = actual.ShouldBeSelectedItemOfType<PathSelectItem>();
            andConstraint.And.SelectedPath.Equals(path).Should().BeTrue();
            return andConstraint;
        }

        public static AndConstraint<WildcardSelectItem> ShouldBeWildcardSelectionItem(this SelectItem actual)
        {
            var andConstraint = actual.ShouldBeSelectedItemOfType<WildcardSelectItem>();
            return andConstraint;
        }

        public static AndConstraint<ExpandedNavigationSelectItem> ShouldBeExpansionFor(this SelectItem item, IEdmNavigationProperty navigationProperty)
        {
            item.Should().BeOfType<ExpandedNavigationSelectItem>();
            var expansion = item.As<ExpandedNavigationSelectItem>();
            expansion.PathToNavigationProperty.LastSegment.As<NavigationPropertySegment>().NavigationProperty.Should().BeSameAs(navigationProperty);
            return new AndConstraint<ExpandedNavigationSelectItem>(expansion);
        }

        public static AndConstraint<ExpandedNavigationSelectItem> ShouldBeExpansionFor(this SelectItem item, ODataPath path)
        {
            item.Should().BeOfType<ExpandedNavigationSelectItem>();
            var expansion = item.As<ExpandedNavigationSelectItem>();
            expansion.PathToNavigationProperty.Equals(path).Should().BeTrue();
            return new AndConstraint<ExpandedNavigationSelectItem>(expansion);
        }
    }
}

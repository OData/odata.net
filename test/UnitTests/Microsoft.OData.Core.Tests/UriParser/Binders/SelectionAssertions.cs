//---------------------------------------------------------------------
// <copyright file="SelectionAssertions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Binders
{
    /// <summary>
    /// Contains fluent assertion APIs for testing Selection objects.
    /// </summary>
    internal static class SelectionAssertions
    {
        public static T ShouldBeSelectedItemOfType<T>(this SelectItem actual) where T : SelectItem
        {
            Assert.NotNull(actual);
            return Assert.IsType<T>(actual);
        }

        public static PathSelectItem ShouldBePathSelectionItem(this SelectItem actual, ODataPath path)
        {
            PathSelectItem pathSeleteItem = actual.ShouldBeSelectedItemOfType<PathSelectItem>();
            Assert.True(pathSeleteItem.SelectedPath.Equals(path));
            return pathSeleteItem;
        }

        public static WildcardSelectItem ShouldBeWildcardSelectionItem(this SelectItem actual)
        {
            WildcardSelectItem wildSelectItem = actual.ShouldBeSelectedItemOfType<WildcardSelectItem>();
            return wildSelectItem;
        }

        public static ExpandedNavigationSelectItem ShouldBeExpansionFor(this SelectItem item, IEdmNavigationProperty navigationProperty)
        {
            Assert.NotNull(item);
            ExpandedNavigationSelectItem expanded = Assert.IsType<ExpandedNavigationSelectItem>(item);
            Assert.NotNull(expanded.PathToNavigationProperty.LastSegment);
            NavigationPropertySegment navigationSegment = Assert.IsType<NavigationPropertySegment>(expanded.PathToNavigationProperty.LastSegment);
            Assert.Same(navigationProperty, navigationSegment.NavigationProperty);
            return expanded;
        }

        public static ExpandedReferenceSelectItem ShouldBeExpansionWithRefFor(this SelectItem item, IEdmNavigationProperty navigationProperty)
        {
            Assert.NotNull(item);
            ExpandedReferenceSelectItem expanded = Assert.IsType<ExpandedReferenceSelectItem>(item);
            Assert.NotNull(expanded.PathToNavigationProperty.LastSegment);
            NavigationPropertySegment navigationSegment = Assert.IsType<NavigationPropertySegment>(expanded.PathToNavigationProperty.LastSegment);
            Assert.Same(navigationProperty, navigationSegment.NavigationProperty);
            return expanded;
        }

        public static ExpandedNavigationSelectItem ShouldBeExpansionFor(this SelectItem item, ODataPath path)
        {
            Assert.NotNull(item);
            ExpandedNavigationSelectItem expanded = Assert.IsType<ExpandedNavigationSelectItem>(item);
            Assert.True(expanded.PathToNavigationProperty.Equals(path));
            return expanded;
        }
    }
}

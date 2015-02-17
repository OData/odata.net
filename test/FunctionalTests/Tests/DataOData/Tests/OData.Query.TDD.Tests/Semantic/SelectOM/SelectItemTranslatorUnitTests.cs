//---------------------------------------------------------------------
// <copyright file="SelectItemTranslatorUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic.SelectOM
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SelectItemTranslatorUnitTests
    {
        private class FakeTranslator : SelectItemTranslator<string>
        {
        }

        [TestMethod]
        public void WildcardSelectItemIsNotImplemented()
        {
            FakeTranslator translator = new FakeTranslator();
            WildcardSelectItem item = new WildcardSelectItem();
            Action visitWildcard = () => item.TranslateWith(translator);
            visitWildcard.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void PathSelectionItemIsNotImplemented()
        {
            FakeTranslator translator = new FakeTranslator();
            PathSelectItem item = new PathSelectItem(new ODataSelectPath() { new PropertySegment(ModelBuildingHelpers.BuildValidPrimitiveProperty()) });
            Action visitPath = () => item.TranslateWith(translator);
            visitPath.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void ContainerQualifiedWildcardSelectItemIsNotImplemented()
        {
            FakeTranslator translator = new FakeTranslator();
            ContainerQualifiedWildcardSelectItem item = new ContainerQualifiedWildcardSelectItem(ModelBuildingHelpers.BuildValidEntityContainer());
            Action visitContainerQualifiedWildcard = () => item.TranslateWith(translator);
            visitContainerQualifiedWildcard.ShouldThrow<NotImplementedException>();
        }

        [TestMethod]
        public void ExpandedNavigationPropertySelectItemIsNotImplemented()
        {
            FakeTranslator translator = new FakeTranslator();
            ExpandedNavigationSelectItem item = new ExpandedNavigationSelectItem(new ODataExpandPath() { new NavigationPropertySegment(ModelBuildingHelpers.BuildValidNavigationProperty(), ModelBuildingHelpers.BuildValidEntitySet()) }, null, null);
            Action visitExpandedNavigationSelectItem = () => item.TranslateWith(translator);
            visitExpandedNavigationSelectItem.ShouldThrow<NotImplementedException>();
        }
    }
}

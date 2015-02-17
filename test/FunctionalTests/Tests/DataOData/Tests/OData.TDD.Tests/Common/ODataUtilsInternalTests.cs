//---------------------------------------------------------------------
// <copyright file="ODataUtilsInternalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Common
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataUtilsInternalTests
    {
        [TestMethod]
        public void SelectedPropertiesShouldReturnEntireSubtreeWhenMetadataDocumentUriIsNull()
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();
            settings.SelectedProperties.Should().BeSameAs(SelectedPropertiesNode.EntireSubtree);
        }

        [Ignore] //TODO: Update SelectedPropertiesNode class to adapt to V4, fix EntireSubtree logic for V4
        [TestMethod]
        public void SelectedPropertiesShouldReturnEntireSubTreeWhenSelectExpandClauseIsNull()
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();
            settings.SetServiceDocumentUri(new Uri("http://service/$metadata"));
            settings.SelectedProperties.Should().BeSameAs(SelectedPropertiesNode.EntireSubtree);
        }
    }
}

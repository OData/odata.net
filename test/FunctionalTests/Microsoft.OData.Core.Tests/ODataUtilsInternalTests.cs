//---------------------------------------------------------------------
// <copyright file="ODataUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataUtilsInternalTests
    {
        [Fact]
        public void SelectedPropertiesShouldReturnEntireSubtreeWhenMetadataDocumentUriIsNull()
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();
            settings.SelectedProperties.Should().BeSameAs(SelectedPropertiesNode.EntireSubtree);
        }

        [Fact(Skip = "TODO: Update SelectedPropertiesNode class to adapt to V4, fix EntireSubtree logic for V4")]
        public void SelectedPropertiesShouldReturnEntireSubTreeWhenSelectExpandClauseIsNull()
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();
            settings.SetServiceDocumentUri(new Uri("http://service/$metadata"));
            settings.SelectedProperties.Should().BeSameAs(SelectedPropertiesNode.EntireSubtree);
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="ODataEntityMetadataBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.Evaluation;
using Xunit;

namespace Microsoft.OData.Core.Tests.Evaluation
{
    public class ODataEntityMetadataBuilderTests
    {
        private readonly ODataEntityMetadataBuilder builder = new TestBuilder();

        [Fact]
        public void GetStreamEditLinkShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestsUtils.GetStreamEditLinkShouldValidateArguments(this.builder);
        }

        [Fact]
        public void GetStreamReadLinkShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestsUtils.GetStreamReadLinkShouldValidateArguments(this.builder);
        }

        [Fact]
        public void GetNavigationLinkUriShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestsUtils.GetNavigationLinkUriShouldValidateArguments(this.builder);
        }

        [Fact]
        public void GetAssociationLinkUriShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestsUtils.GetAssociationLinkUriShouldValidateArguments(this.builder);
        }

        [Fact]
        public void GetOperationTargetUriShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestsUtils.GetOperationTargetUriShouldValidateArguments(this.builder);
        }

        [Fact]
        public void GetOperationTitleShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestsUtils.GetOperationTitleShouldValidateArguments(this.builder);
        }

        [Fact]
        public void BaseBuilderShouldReturnNullAssociationLink()
        {
            this.builder.GetAssociationLinkUri("Fake", null, false).Should().BeNull();
            this.builder.GetAssociationLinkUri("Fake", null, true).Should().BeNull();
        }

        [Fact]
        public void BaseBuilderShouldReturnNullNavigationLink()
        {
            this.builder.GetNavigationLinkUri("Fake", null, false).Should().BeNull();
            this.builder.GetNavigationLinkUri("Fake", null, true).Should().BeNull();
        }
        [Fact]
        public void BaseBuilderShouldReturnNullAssociationLinkEvenWhenNonComputedLinkIsAvailable()
        {
            this.builder.GetAssociationLinkUri("Fake", new Uri("http://example.com/override"), false).Should().BeNull();
            this.builder.GetAssociationLinkUri("Fake", new Uri("http://example.com/override"), true).Should().BeNull();
        }

        [Fact]
        public void BaseBuilderShouldReturnNullNavigationLinkEvenWhenNonComputedLinkIsAvailable()
        {
            this.builder.GetNavigationLinkUri("Fake", new Uri("http://example.com/override"), false).Should().BeNull();
            this.builder.GetNavigationLinkUri("Fake", new Uri("http://example.com/override"), true).Should().BeNull();
        }

        [Fact]
        public void BaseBuilderShouldReturnNullOperationTarget()
        {
            this.builder.GetOperationTargetUri("Fake", null, null).Should().BeNull();
        }

        [Fact]
        public void BaseBuilderShouldReturnNullOperationTitle()
        {
            this.builder.GetOperationTitle("Fake").Should().BeNull();
        }

        [Fact]
        public void BaseBuilderShouldReturnNullStreamEditLinkForStreamProperty()
        {
            this.builder.GetStreamEditLink("Fake").Should().BeNull();
        }

        [Fact]
        public void BaseBuilderShouldReturnNullStreamEditLinkForDefaultStream()
        {
            this.builder.GetStreamEditLink(null).Should().BeNull();
        }

        [Fact]
        public void BaseBuilderShouldReturnNullStreamReadLinkForStreamProperty()
        {
            this.builder.GetStreamReadLink("Fake").Should().BeNull();
        }

        [Fact]
        public void BaseBuilderShouldReturnNullStreamReadLinkForDefaultStream()
        {
            this.builder.GetStreamReadLink(null).Should().BeNull();
        }

        private class TestBuilder : ODataEntityMetadataBuilder
        {
            internal override System.Uri GetEditLink()
            {
                return new Uri("http://example.com/edit/link");
            }

            internal override System.Uri GetReadLink()
            {
                return new Uri("http://example.com/read/link");
            }

            internal override string GetETag()
            {
                return "etag value";
            }

            internal override Uri GetId()
            {
                return new Uri("http://idvalue");
            }

            internal override bool TryGetIdForSerialization(out Uri id)
            {
                id = null;
                return false;
            }
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="ODataEntityMetadataBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Evaluation;
using Xunit;

namespace Microsoft.OData.Tests.Evaluation
{
    public class ODataEntityMetadataBuilderTests
    {
        private readonly ODataResourceMetadataBuilder builder = new TestBuilder();

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
            Assert.Null(this.builder.GetAssociationLinkUri("Fake", null, false));
            Assert.Null(this.builder.GetAssociationLinkUri("Fake", null, true));
        }

        [Fact]
        public void BaseBuilderShouldReturnNullNavigationLink()
        {
            Assert.Null(this.builder.GetNavigationLinkUri("Fake", null, false));
            Assert.Null(this.builder.GetNavigationLinkUri("Fake", null, true));
        }
        [Fact]
        public void BaseBuilderShouldReturnNullAssociationLinkEvenWhenNonComputedLinkIsAvailable()
        {
            Assert.Null(this.builder.GetAssociationLinkUri("Fake", new Uri("http://example.com/override"), false));
            Assert.Null(this.builder.GetAssociationLinkUri("Fake", new Uri("http://example.com/override"), true));
        }

        [Fact]
        public void BaseBuilderShouldReturnNullNavigationLinkEvenWhenNonComputedLinkIsAvailable()
        {
            Assert.Null(this.builder.GetNavigationLinkUri("Fake", new Uri("http://example.com/override"), false));
            Assert.Null(this.builder.GetNavigationLinkUri("Fake", new Uri("http://example.com/override"), true));
        }

        [Fact]
        public void BaseBuilderShouldReturnNullOperationTarget()
        {
            Assert.Null(this.builder.GetOperationTargetUri("Fake", null, null));
        }

        [Fact]
        public void BaseBuilderShouldReturnNullOperationTitle()
        {
            Assert.Null(this.builder.GetOperationTitle("Fake"));
        }

        [Fact]
        public void BaseBuilderShouldReturnNullStreamEditLinkForStreamProperty()
        {
            Assert.Null(this.builder.GetStreamEditLink("Fake"));
        }

        [Fact]
        public void BaseBuilderShouldReturnNullStreamEditLinkForDefaultStream()
        {
            Assert.Null(this.builder.GetStreamEditLink(null));
        }

        [Fact]
        public void BaseBuilderShouldReturnNullStreamReadLinkForStreamProperty()
        {
            Assert.Null(this.builder.GetStreamReadLink("Fake"));
        }

        [Fact]
        public void BaseBuilderShouldReturnNullStreamReadLinkForDefaultStream()
        {
            Assert.Null(this.builder.GetStreamReadLink(null));
        }

        private class TestBuilder : ODataResourceMetadataBuilder
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

//---------------------------------------------------------------------
// <copyright file="ODataUriBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using FluentAssertions;
using Microsoft.OData.Core.Evaluation;
using Microsoft.OData.Edm.Values;
using Xunit;

namespace Microsoft.OData.Core.Tests.Evaluation
{
    public class ODataUriBuilderTests : ODataUriBuilderTestsBase
    {
        private static readonly IEdmStructuredValue Entity = new EdmStructuredValueSimulator();
        private readonly ODataUriBuilder builder = new TestBuilder();

        [Fact]
        public void BuildEntitySetUriShouldValidateArguments()
        {
            BuildEntitySetUriShouldValidateArguments(this.builder);
        }

        [Fact]
        public void BuildStreamEditLinkUriShouldValidateArguments()
        {
            BuildStreamEditLinkUriShouldValidateArguments(this.builder);
        }

        [Fact]
        public void BuildStreamReadLinkUriShouldValidateArguments()
        {
            BuildStreamReadLinkUriShouldValidateArguments(this.builder);
        }

        [Fact]
        public void BuildNavigationLinkUriShouldValidateArguments()
        {
            BuildNavigationLinkUriShouldValidateArguments(this.builder);
        }

        [Fact]
        public void BuildAssociationLinkUriShouldValidateArguments()
        {
            BuildAssociationLinkUriShouldValidateArguments(this.builder);
        }

        [Fact]
        public void BuildOperationTargetUriShouldValidateArguments()
        {
            BuildOperationTargetUriShouldValidateArguments(this.builder);
        }

        [Fact]
        public void BaseBuilderShouldReturnNullForContainer()
        {
            this.builder.BuildBaseUri().Should().BeNull();
        }

        [Fact]
        public void BaseBuilderShouldReturnNullForSet()
        {
            this.builder.BuildEntitySetUri(null, "Fake").Should().BeNull();
        }

        [Fact]
        public void BaseBuilderShouldReturnNullForEntityKey()
        {
            this.builder.BuildEntityInstanceUri(null, new Collection<KeyValuePair<string, object>>(), "TypeName").Should().BeNull();
        }

        [Fact]
        public void BaseBuilderShouldReturnNullForNavigation()
        {
            this.builder.BuildNavigationLinkUri(null, "Fake").Should().BeNull();
        }

        [Fact]
        public void BaseBuilderShouldReturnNullForAssociation()
        {
            this.builder.BuildAssociationLinkUri(null, "Fake").Should().BeNull();
        }

        [Fact]
        public void BaseBuilderShouldReturnNullForOperation()
        {
            this.builder.BuildOperationTargetUri(null, "Fake", null, null).Should().BeNull();
        }

        [Fact]
        public void BaseBuilderShouldReturnNullForStreamEdit()
        {
            this.builder.BuildStreamEditLinkUri(null, "Fake").Should().BeNull();
        }

        [Fact]
        public void BaseBuilderShouldReturnNullForStreamRead()
        {
            this.builder.BuildStreamReadLinkUri(null, "Fake").Should().BeNull();
        }

        [Fact]
        public void BaseBuilderShouldReturnNullForTypeSegment()
        {
            this.builder.AppendTypeSegment(null, "NS.TypeName").Should().BeNull();
        }

        private class TestBuilder : ODataUriBuilder
        {
        }
    }
}

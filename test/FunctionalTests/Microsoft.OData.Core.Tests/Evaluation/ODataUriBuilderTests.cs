//---------------------------------------------------------------------
// <copyright file="ODataUriBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Evaluation;
using Xunit;

namespace Microsoft.OData.Tests.Evaluation
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
            Assert.Null(this.builder.BuildBaseUri());
        }

        [Fact]
        public void BaseBuilderShouldReturnNullForSet()
        {
            Assert.Null(this.builder.BuildEntitySetUri(null, "Fake"));
        }

        [Fact]
        public void BaseBuilderShouldReturnNullForEntityKey()
        {
            Assert.Null(this.builder.BuildEntityInstanceUri(null, new Collection<KeyValuePair<string, object>>(), "TypeName"));
        }

        [Fact]
        public void BaseBuilderShouldReturnNullForNavigation()
        {
            Assert.Null(this.builder.BuildNavigationLinkUri(null, "Fake"));
        }

        [Fact]
        public void BaseBuilderShouldReturnNullForAssociation()
        {
            Assert.Null(this.builder.BuildAssociationLinkUri(null, "Fake"));
        }

        [Fact]
        public void BaseBuilderShouldReturnNullForOperation()
        {
            Assert.Null(this.builder.BuildOperationTargetUri(null, "Fake", null, null));
        }

        [Fact]
        public void BaseBuilderShouldReturnNullForStreamEdit()
        {
            Assert.Null(this.builder.BuildStreamEditLinkUri(null, "Fake"));
        }

        [Fact]
        public void BaseBuilderShouldReturnNullForStreamRead()
        {
            Assert.Null(this.builder.BuildStreamReadLinkUri(null, "Fake"));
        }

        [Fact]
        public void BaseBuilderShouldReturnNullForTypeSegment()
        {
            Assert.Null(this.builder.AppendTypeSegment(null, "NS.TypeName"));
        }

        private class TestBuilder : ODataUriBuilder
        {
        }
    }
}

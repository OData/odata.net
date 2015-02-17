//---------------------------------------------------------------------
// <copyright file="ODataUriBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Evaluation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using FluentAssertions;
    using Microsoft.OData.Edm.Values;
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataUriBuilderTests : ODataUriBuilderBaseTests
    {
        private static readonly IEdmStructuredValue Entity = new EdmStructuredValueSimulator();
        private readonly ODataUriBuilder builder = new TestBuilder();

        [TestMethod]
        public void BuildEntitySetUriShouldValidateArguments()
        {
            BuildEntitySetUriShouldValidateArguments(this.builder);
        }

        [TestMethod]
        public void BuildStreamEditLinkUriShouldValidateArguments()
        {
            BuildStreamEditLinkUriShouldValidateArguments(this.builder);
        }

        [TestMethod]
        public void BuildStreamReadLinkUriShouldValidateArguments()
        {
            BuildStreamReadLinkUriShouldValidateArguments(this.builder);
        }

        [TestMethod]
        public void BuildNavigationLinkUriShouldValidateArguments()
        {
            BuildNavigationLinkUriShouldValidateArguments(this.builder);
        }

        [TestMethod]
        public void BuildAssociationLinkUriShouldValidateArguments()
        {
            BuildAssociationLinkUriShouldValidateArguments(this.builder);
        }

        [TestMethod]
        public void BuildOperationTargetUriShouldValidateArguments()
        {
            BuildOperationTargetUriShouldValidateArguments(this.builder);
        }

        [TestMethod]
        public void BaseBuilderShouldReturnNullForContainer()
        {
            this.builder.BuildBaseUri().Should().BeNull();
        }

        [TestMethod]
        public void BaseBuilderShouldReturnNullForSet()
        {
            this.builder.BuildEntitySetUri(null, "Fake").Should().BeNull();
        }

        [TestMethod]
        public void BaseBuilderShouldReturnNullForEntityKey()
        {
            this.builder.BuildEntityInstanceUri(null, new Collection<KeyValuePair<string, object>>(), "TypeName").Should().BeNull();
        }

        [TestMethod]
        public void BaseBuilderShouldReturnNullForNavigation()
        {
            this.builder.BuildNavigationLinkUri(null, "Fake").Should().BeNull();
        }

        [TestMethod]
        public void BaseBuilderShouldReturnNullForAssociation()
        {
            this.builder.BuildAssociationLinkUri(null, "Fake").Should().BeNull();
        }

        [TestMethod]
        public void BaseBuilderShouldReturnNullForOperation()
        {
            this.builder.BuildOperationTargetUri(null, "Fake", null, null).Should().BeNull();
        }

        [TestMethod]
        public void BaseBuilderShouldReturnNullForStreamEdit()
        {
            this.builder.BuildStreamEditLinkUri(null, "Fake").Should().BeNull();
        }

        [TestMethod]
        public void BaseBuilderShouldReturnNullForStreamRead()
        {
            this.builder.BuildStreamReadLinkUri(null, "Fake").Should().BeNull();
        }

        [TestMethod]
        public void BaseBuilderShouldReturnNullForTypeSegment()
        {
            this.builder.AppendTypeSegment(null, "NS.TypeName").Should().BeNull();
        }

        private class TestBuilder : ODataUriBuilder
        {
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="NamespaceQualifiedWildcardSelectionItemTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class NamespaceQualifiedWildcardSelectionItemTests
    {
        [Fact]
        public void ContainerIsSetFromConstructor()
        {
            var item = new NamespaceQualifiedWildcardSelectItem("name.space");
            item.Namespace.Should().Be("name.space");
        }

        [Fact]
        public void ContainerCannotBeNull()
        {
            Action ctor = () => new NamespaceQualifiedWildcardSelectItem(null);
            ctor.ShouldThrow<ArgumentNullException>();
        }
    }
}

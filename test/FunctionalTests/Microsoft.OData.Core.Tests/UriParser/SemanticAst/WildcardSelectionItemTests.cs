//---------------------------------------------------------------------
// <copyright file="WildcardSelectionItemTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    public class WildcardSelectionItemTests
    {
        [Fact]
        public void ConstructorDoesNotThrow()
        {
            Action ctor = () => new WildcardSelectItem();
            ctor.ShouldNotThrow();
        }

        [Fact]
        public void TwoInstanceAreNotReferenceEquals()
        {
            var item1 = new WildcardSelectItem();
            var item2 = new WildcardSelectItem();
            item1.Should().NotBeSameAs(item2);
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="WildcardSelectionItemTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class WildcardSelectionItemTests
    {
        [Fact]
        public void ConstructorDoesNotThrow()
        {
            Action ctor = () => new WildcardSelectItem();
            ctor.DoesNotThrow();
        }

        [Fact]
        public void TwoInstanceAreNotReferenceEquals()
        {
            var item1 = new WildcardSelectItem();
            var item2 = new WildcardSelectItem();
            Assert.NotSame(item2, item1);
        }
    }
}

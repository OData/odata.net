//---------------------------------------------------------------------
// <copyright file="PathSelectItemTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class PathSelectItemTests
    {
        [Fact]
        public void ConstructorShouldSetPropertyName()
        {
            var item = new PathSelectItem(new ODataSelectPath(new DynamicPathSegment("abc")));
            item.SelectedPath.FirstSegment.ShouldBeDynamicPathSegment("abc");
        }

        [Fact]
        public void PathCannotBeNull()
        {
            Action ctor = () => new PathSelectItem(null);
            ctor.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void NullNameIsNotAllowed()
        {
            Action ctor = () => new PathSelectItem(null);
            ctor.ShouldThrow<ArgumentNullException>();
        }
    }
}

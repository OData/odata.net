//---------------------------------------------------------------------
// <copyright file="PathSelectItemTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    public class PathSelectItemTests
    {
        [Fact]
        public void ConstructorShouldSetPropertyName()
        {
            var item = new PathSelectItem(new ODataSelectPath(new OpenPropertySegment("abc")));
            item.SelectedPath.FirstSegment.ShouldBeOpenPropertySegment("abc");
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

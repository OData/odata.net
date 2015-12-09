//---------------------------------------------------------------------
// <copyright file="ConvertNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    /// <summary>
    /// Unit tests for the ConvertNode class
    /// </summary>
    public class ConvertNodeTests
    {
        [Fact]
        public void SourceCannotBeNull()
        {
            Action createWithNullSource = () => new ConvertNode(null, EdmCoreModel.Instance.GetInt32(true));
            createWithNullSource.ShouldThrow<Exception>(Error.ArgumentNull("source").ToString());
        }
        
        [Fact]
        public void TypeReferenceCannotBeNull()
        {
            ConstantNode source = new ConstantNode(1);
            Action createWithNullTargetType = () => new ConvertNode(source, null);
            createWithNullTargetType.ShouldThrow<Exception>(Error.ArgumentNull("typeReference").ToString());
        }

        [Fact]
        public void SourceIsSetCorrectly()
        {
            ConstantNode source = new ConstantNode(1);
            ConvertNode convertNode = new ConvertNode(source, source.TypeReference);
            convertNode.Source.As<ConstantNode>().Value.As<int>().Should().Be(1);
        }

        [Fact]
        public void TypeReferenceIsSetCorrectly()
        {
            ConstantNode source = new ConstantNode(1);
            ConvertNode convertNode = new ConvertNode(source, EdmCoreModel.Instance.GetInt64(true));
            convertNode.TypeReference.FullName().Should().Be("Edm.Int64");
        }

        [Fact]
        public void KindIsSetToConvertNode()
        {
            ConstantNode source = new ConstantNode(1);
            ConvertNode convertNode = new ConvertNode(source, EdmCoreModel.Instance.GetInt64(true));
            convertNode.InternalKind.Should().Be(InternalQueryNodeKind.Convert);
        }
    }
}

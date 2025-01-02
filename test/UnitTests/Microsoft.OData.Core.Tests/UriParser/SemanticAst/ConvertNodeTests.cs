//---------------------------------------------------------------------
// <copyright file="ConvertNodeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
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
            Assert.Throws<ArgumentNullException>("source", createWithNullSource);
        }

        [Fact]
        public void TypeReferenceCannotBeNull()
        {
            ConstantNode source = new ConstantNode(1);
            Action createWithNullTargetType = () => new ConvertNode(source, null);
            Assert.Throws<ArgumentNullException>("typeReference", createWithNullTargetType);
        }

        [Fact]
        public void SourceIsSetCorrectly()
        {
            ConstantNode source = new ConstantNode(1);
            ConvertNode convertNode = new ConvertNode(source, source.TypeReference);

            ConstantNode conNode = Assert.IsType<ConstantNode>(convertNode.Source);
            Assert.Equal(1, Assert.IsType<int>(conNode.Value));
        }

        [Fact]
        public void TypeReferenceIsSetCorrectly()
        {
            ConstantNode source = new ConstantNode(1);
            ConvertNode convertNode = new ConvertNode(source, EdmCoreModel.Instance.GetInt64(true));
            Assert.Equal("Edm.Int64", convertNode.TypeReference.FullName());
        }

        [Fact]
        public void KindIsSetToConvertNode()
        {
            ConstantNode source = new ConstantNode(1);
            ConvertNode convertNode = new ConvertNode(source, EdmCoreModel.Instance.GetInt64(true));
            Assert.Equal(InternalQueryNodeKind.Convert, convertNode.InternalKind);
        }
    }
}

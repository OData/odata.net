//---------------------------------------------------------------------
// <copyright file="ConvertUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    #region Namespaces

    using System;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Unit tests for the ConvertNode class
    /// </summary>
    [TestClass]
    public class ConvertUnitTests
    {
        [TestMethod]
        public void SourceCannotBeNull()
        {
            Action createWithNullSource = () => new ConvertNode(null, EdmCoreModel.Instance.GetInt32(true));
            createWithNullSource.ShouldThrow<Exception>(Error.ArgumentNull("source").ToString());
        }
        
        [TestMethod]
        public void TypeReferenceCannotBeNull()
        {
            ConstantNode source = new ConstantNode(1);
            Action createWithNullTargetType = () => new ConvertNode(source, null);
            createWithNullTargetType.ShouldThrow<Exception>(Error.ArgumentNull("typeReference").ToString());
        }

        [TestMethod]
        public void SourceIsSetCorrectly()
        {
            ConstantNode source = new ConstantNode(1);
            ConvertNode convertNode = new ConvertNode(source, source.TypeReference);
            convertNode.Source.As<ConstantNode>().Value.As<int>().Should().Be(1);
        }

        [TestMethod]
        public void TypeReferenceIsSetCorrectly()
        {
            ConstantNode source = new ConstantNode(1);
            ConvertNode convertNode = new ConvertNode(source, EdmCoreModel.Instance.GetInt64(true));
            convertNode.TypeReference.FullName().Should().Be("Edm.Int64");
        }

        [TestMethod]
        public void KindIsSetToConvertNode()
        {
            ConstantNode source = new ConstantNode(1);
            ConvertNode convertNode = new ConvertNode(source, EdmCoreModel.Instance.GetInt64(true));
            convertNode.InternalKind.Should().Be(InternalQueryNodeKind.Convert);
        }
    }
}

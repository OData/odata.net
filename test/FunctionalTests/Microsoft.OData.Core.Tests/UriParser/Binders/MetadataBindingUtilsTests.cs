//---------------------------------------------------------------------
// <copyright file="MetadataBindingUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.Binders
{
    /// <summary>
    /// Unit and Short-Span Integration tests for MetadataBindingUtils methods.
    /// </summary>
    public class MetadataBindingUtilsTests
    {
        #region MetadataBindingUtils.ConvertToType Tests
        [Fact]
        public void IfTypePromotionNeededConvertNodeIsCreatedAndSourcePropertySet()
        {
            SingleValueNode node = new ConstantNode(7);
            var result = MetadataBindingUtils.ConvertToTypeIfNeeded(node, EdmCoreModel.Instance.GetDouble(false));
            result.ShouldBeConstantQueryNode(7d);
        }

        [Fact]
        public void IfTypeReferenceIsNullNoConvertIsInjected()
        {
            SingleValueNode node = new ConstantNode(7);
            var result = MetadataBindingUtils.ConvertToTypeIfNeeded(node, null);
            result.ShouldBeConstantQueryNode(7);
        }

        [Fact]
        public void IfTypesMatchIsNullNoConvertIsInjected()
        {
            SingleValueNode node = new ConstantNode(7);
            var result = MetadataBindingUtils.ConvertToTypeIfNeeded(node, EdmCoreModel.Instance.GetInt32(false));
            result.ShouldBeConstantQueryNode(7);
        }

        [Fact]
        public void IfTypesCannotPromoteErrorIsThrown()
        {
            SingleValueNode node = new ConstantNode(7);
            var targetType = EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiLineString, false);
            Action convertMethod = () => MetadataBindingUtils.ConvertToTypeIfNeeded(node, targetType);
            convertMethod.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.MetadataBinder_CannotConvertToType(node.TypeReference.FullName(), targetType.FullName()));
        }
        #endregion
    }
}

//---------------------------------------------------------------------
// <copyright file="ValidationUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ValidationUtilsTests
    {
        #region Test ValidateServiceDocumentElement
        [Fact]
        public void ServiceDocumentElementIsNotNullShouldThrow()
        {
            Action test = () => ValidationUtils.ValidateServiceDocumentElement(null, ODataFormat.Json);
            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_WorkspaceResourceMustNotContainNullItem);
        }
        
        [Fact]
        public void ServiceDocumentElementHasNullUrlShouldThrow()
        {
            Action test = () => ValidationUtils.ValidateServiceDocumentElement(new ODataFunctionImportInfo(){Name="good"}, ODataFormat.Json);
            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_ResourceMustSpecifyUrl);
        }
        
        [Fact]
        public void ServiceDocumentElementHasNullNameShouldThrowIfJson()
        {
            var uri = new Uri("http://link/foo");
            Action test = () => ValidationUtils.ValidateServiceDocumentElement(new ODataFunctionImportInfo() { Url = uri }, ODataFormat.Json);
            test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_ResourceMustSpecifyName(uri.OriginalString));
        }

        [Fact]
        public void ServiceDocumentElementHasNullNameShouldNotThrowIfAtom()
        {
            var uri = new Uri("http://link/foo");
            Action test = () => ValidationUtils.ValidateServiceDocumentElement(new ODataFunctionImportInfo() { Url = uri }, ODataFormat.Atom);
            test.ShouldNotThrow();
        }
        
        [Fact]
        public void ServiceDocumentElementShouldNotThrow()
        {
            Action test = () => ValidationUtils.ValidateServiceDocumentElement(new ODataFunctionImportInfo() { Url = new Uri("http://link/foo"), Name = "GoodName" }, ODataFormat.Json);
            test.ShouldNotThrow();
        }
        #endregion

        #region Test ValidateTypeAssignable
        [Fact]
        public void ValidateComplexTypeIsAssignableTest()
        {
            EdmComplexType type1 = new EdmComplexType("NS", "Type1", null, false);
            EdmComplexType type2 = new EdmComplexType("NS", "Type2", type1, false);
            EdmComplexType type3 = new EdmComplexType("NS", "Type3", null, false);

            Action test1 = () => ValidationUtils.ValidateComplexTypeIsAssignable(type1, type2);
            test1.ShouldNotThrow();

            Action test2 = () => ValidationUtils.ValidateComplexTypeIsAssignable(type1, type1);
            test2.ShouldNotThrow();

            Action test3 = () => ValidationUtils.ValidateComplexTypeIsAssignable(type1, type3);
            test3.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_IncompatibleType("NS.Type3", "NS.Type1"));
        }
        #endregion
    }
}

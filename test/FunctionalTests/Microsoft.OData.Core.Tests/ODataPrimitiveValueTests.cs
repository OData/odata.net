//---------------------------------------------------------------------
// <copyright file="ODataPrimitiveValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Xunit;
using ErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests
{
    public class ODataPrimitiveValueTests
    {
        [Fact]
        public void CreatingNullPrimitiveValueShouldFail()
        {
            Action testSubject = () => new ODataPrimitiveValue(null);
            testSubject.ShouldThrow<ArgumentNullException>().WithMessage(ErrorStrings.ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromNull);
        }

        [Fact]
        public void CreatingNestedODataPrimitiveValueShouldFail()
        {
            ODataPrimitiveValue innerPrimitiveValue = new ODataPrimitiveValue(42);
            Action testSubject = () => new ODataPrimitiveValue(innerPrimitiveValue);
            testSubject.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromUnsupportedValueType("Microsoft.OData.Core.ODataPrimitiveValue"));
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="ODataPrimitiveValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.Test.OData.TDD.Tests.ObjecetModel
{
    using Microsoft.OData.Core;
    using FluentAssertions;

    [TestClass]
    public class ODataPrimitiveValueTests
    {
        [TestMethod]
        public void CreatingNullPrimitiveValueShouldFail()
        {
            Action testSubject = () => new ODataPrimitiveValue(null);
            testSubject.ShouldThrow<ArgumentNullException>().WithMessage(ErrorStrings.ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromNull);
        }

        [TestMethod]
        public void CreatingNestedODataPrimitiveValueShouldFail()
        {
            ODataPrimitiveValue innerPrimitiveValue = new ODataPrimitiveValue(42);
            Action testSubject = () => new ODataPrimitiveValue(innerPrimitiveValue);
            testSubject.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromUnsupportedValueType("Microsoft.OData.Core.ODataPrimitiveValue"));
        }
    }
}

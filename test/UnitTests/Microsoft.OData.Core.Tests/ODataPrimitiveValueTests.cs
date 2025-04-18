﻿//---------------------------------------------------------------------
// <copyright file="ODataPrimitiveValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Core;
using System;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataPrimitiveValueTests
    {
        [Fact]
        public void CreatingNullPrimitiveValueShouldFail()
        {
            Action testSubject = () => new ODataPrimitiveValue(null);
            testSubject.Throws<ArgumentNullException>(SRResources.ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromNull);
        }

        [Fact]
        public void CreatingNestedODataPrimitiveValueShouldFail()
        {
            ODataPrimitiveValue innerPrimitiveValue = new ODataPrimitiveValue(42);
            Action testSubject = () => new ODataPrimitiveValue(innerPrimitiveValue);
            testSubject.Throws<ODataException>(Error.Format(SRResources.ODataPrimitiveValue_CannotCreateODataPrimitiveValueFromUnsupportedValueType, "Microsoft.OData.ODataPrimitiveValue"));
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="ODataNullValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Test.OData.TDD.Tests.ObjecetModel
{
    using Microsoft.OData.Core;
    using FluentAssertions;

    [TestClass]
    public class ODataNullValueTests
    {
        private ODataNullValue nullValue;

        [TestInitialize]
        public void Initialize()
        {
            this.nullValue = new ODataNullValue();
        }

        [TestMethod]
        public void IsNullMethodShouldBeTrueForNullValue()
        {
            this.nullValue.IsNullValue.Should().BeTrue();
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="WebUtilTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using FluentAssertions;
    using Microsoft.OData.Service;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class WebUtilTests
    {
        [TestMethod]
        public void UnspecifiedDateTimeConversion()
        {
            var dt = new DateTime(2012, 5, 5);
            var dto = WebUtil.ConvertDateTimeToDateTimeOffset(dt);
            dt.Ticks.ShouldBeEquivalentTo(dto.Ticks, "Ticks value should remain the same");
            dto.Offset.ShouldBeEquivalentTo(TimeSpan.Zero, "Unspecified DateTime should be converted to UTC DateTimeOffset");
        }

        [TestMethod]
        public void LocalDateTimeConversion()
        {
            var dt = DateTime.Now.ToLocalTime();
            var dto = WebUtil.ConvertDateTimeToDateTimeOffset(dt);
            dt.ShouldBeEquivalentTo(dto.DateTime, "For local DateTime values, the value of DateTime within DateTimeOffset must be equal");
        }

        [TestMethod]
        public void UtcDateTimeConversion()
        {
            var dt = DateTime.UtcNow;
            var dto = WebUtil.ConvertDateTimeToDateTimeOffset(dt);
            dt.ShouldBeEquivalentTo(dto.DateTime, "For utc DateTime values, the value of DateTime within DateTimeOffset must be equal");
        }

        [TestMethod]
        public void UtcDateTimeOffsetConversion()
        {
            var dto = DateTimeOffset.UtcNow;
            var dt = WebUtil.ConvertDateTimeOffsetToDateTime(dto);
            dto.DateTime.ShouldBeEquivalentTo(dt, "For utc DateTimeOffset values, the value of DateTime within DateTimeOffset must be equal to the result DateTime value");
        }

        [TestMethod]
        public void LocalDateTimeOffsetConversion()
        {
            var dto = DateTimeOffset.Now;
            var dt = WebUtil.ConvertDateTimeOffsetToDateTime(dto);
            dto.DateTime.ShouldBeEquivalentTo(dt, "For local DateTimeOffset values, the value of DateTime within DateTimeOffset must be equal to the result DateTime value");
        }
    }
}

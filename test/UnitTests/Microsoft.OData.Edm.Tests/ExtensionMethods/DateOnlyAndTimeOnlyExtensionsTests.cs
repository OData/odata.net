//---------------------------------------------------------------------
// <copyright file="DateOnlyAndTimeOnlyExtensionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Globalization;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Edm.Tests.ExtensionMethods
{
    public class DateOnlyAndTimeOnlyExtensionsTests
    {
        [Fact]
        public void ToODataString_DateOnly_DefaultProvider_ReturnsIso8601()
        {
            var date = new DateOnly(2025, 11, 27);
            var result = date.ToODataString();
            Assert.Equal("2025-11-27", result);
        }

        [Fact]
        public void ToODataString_DateOnly_CustomProvider_ReturnsIso8601()
        {
            var date = new DateOnly(2025, 1, 9);
            var result = date.ToODataString(CultureInfo.InvariantCulture);
            Assert.Equal("2025-01-09", result);
        }

        [Fact]
        public void ToODataString_TimeOnly_DefaultProvider_ReturnsIso8601Extended()
        {
            var time = new TimeOnly(13, 45, 59, 123);
            var result = time.ToODataString();
            Assert.Equal("13:45:59.1230000", result);
        }

        [Fact]
        public void ToODataString_TimeOnly_CustomProvider_ReturnsIso8601Extended()
        {
            var time = new TimeOnly(0, 0, 0, 1, 2);
            var result = time.ToODataString(CultureInfo.InvariantCulture);
            Assert.Equal("00:00:00.0010020", result);
        }

        [Fact]
        public void ToODataString_TimeOnly_DifferentProviders_ProduceSameString()
        {
            var time = new TimeOnly(8, 15, 30, 456, 23);
            var invariant = time.ToODataString(CultureInfo.InvariantCulture);
            var french = time.ToODataString(new CultureInfo("fr-FR"));
            Assert.Equal(invariant, french);
            Assert.Equal("08:15:30.4560230", invariant);
        }

        [Fact]
        public void ToODataString_DateOnly_DifferentProviders_ProduceSameString()
        {
            var date = new DateOnly(2025, 12, 5);
            var invariant = date.ToODataString(CultureInfo.InvariantCulture);
            var german = date.ToODataString(new CultureInfo("de-DE"));
            Assert.Equal(invariant, german);
            Assert.Equal("2025-12-05", german);
        }
    }
}

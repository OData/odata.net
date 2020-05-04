//---------------------------------------------------------------------
// <copyright file="PlatformHelperTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Client;
using System.Globalization;
using System.Reflection;
using AstoriaUnitTests;
using FluentAssertions;
using Xunit;

namespace AstoriaUnitTests.Tests
{
    /// <summary>
    /// This test class is also used by the Astoria TDDUnitTests. The reason for this is that the methods being tested here have different
    /// source on Modern versus other platforms, so by running the tests in both places but expecting the same output (except minor differences
    /// as noted in the tests), we can ensure that the Modern behavior is compatible with other platforms.
    /// </summary>
    public class PlatformHelperTests
    {
        #region Type System testing

        [Fact]
        public void AreMethodsEqualShouldBeTrue()
        {
            Type t = typeof(Type);
            MethodInfo methodInfo = PlatformHelper.GetMethod(t, "GetType", new Type[] { }, true, false);
            MethodInfo methodInfo2 = PlatformHelper.GetMethod(t, "GetType", new Type[] { }, true, false);

            PlatformHelper.AreMembersEqual(methodInfo, methodInfo2).Should().BeTrue();
        }

        [Fact]
        public void GetPublicInstanceMethodWithNoTypes()
        {
            MethodInfo methodInfo = PlatformHelper.GetMethod(typeof (Type), "GetType",new Type[]{}, true, false);
            Assert.NotNull(methodInfo);
            Assert.Equal("GetType", methodInfo.Name);
        }

        [Fact]
        public void GetPublicInstanceMethod()
        {
            MethodInfo methodInfo = PlatformHelper.GetMethod(typeof(Type), "GetDefaultMembers", true, false);
            Assert.NotNull(methodInfo);
            Assert.Equal("GetDefaultMembers", methodInfo.Name);
        }

        [Fact]
        public void GetNonPublicStaticMethod()
        {
            MethodInfo methodInfo = PlatformHelper.GetMethod(typeof(TestClassWithInternalStaticProperty), "MyFact", false, true);
            Assert.NotNull(methodInfo);
            Assert.Equal("MyFact", methodInfo.Name);
        }

        [Fact]
        public void GetConstructorInfo()
        {
            ConstructorInfo constructorInfo = PlatformHelper.GetInstanceConstructor(typeof(Version), true, new Type[] { typeof(int), typeof(int) });
            Assert.NotNull(constructorInfo);
        }

        #endregion

        internal class TestClassWithInternalStaticProperty
        {
            internal static int MyFact()
            {
                return 1;
            }
        }

        [Fact]
        public void ConvertStringToDateTimeOffsetShouldThrowIfTimeZoneIsMissing()
        {
            string dateTimeOffsetStr = "2013-11-04T19:09:26";
            Action test = () => PlatformHelper.ConvertStringToDateTimeOffset(dateTimeOffsetStr);
            test.ShouldThrow<FormatException>(Strings.PlatformHelper_DateTimeOffsetMustContainTimeZone(dateTimeOffsetStr));
        }

        [Fact]
        public void ConvertStringToDateTimeOffsetShouldConvertDateTimeStringEndsWithUpperCaseZ()
        {
            PlatformHelper.ConvertStringToDateTimeOffset("2013-11-04T19:09:26Z").Offset.Should().Be(new TimeSpan(0));
        }

        [Fact]
        public void ConvertStringToDateTimeOffsetShouldConvertDateTimeStringEndsWithLowerCaseZ()
        {
            PlatformHelper.ConvertStringToDateTimeOffset("2013-11-04T19:09:26z").Offset.Should().Be(new TimeSpan(0));
        }

        [Fact]
        public void ConvertStringToDateTimeOffsetShouldConvertDateTimeStringWithPlusOffset()
        {
            PlatformHelper.ConvertStringToDateTimeOffset("2013-11-04T19:09:26+08:00").Offset.Should().Be(new TimeSpan(8, 0, 0));
        }

        [Fact]
        public void ConvertStringToDateTimeOffsetShouldConvertDateTimeStringWithMinusOffset()
        {
            PlatformHelper.ConvertStringToDateTimeOffset("2013-11-04T19:09:26-08:00").Offset.Should().Be(new TimeSpan(-8, 0, 0));
        }
    }
}

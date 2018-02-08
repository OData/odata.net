//---------------------------------------------------------------------
// <copyright file="ClrValueToEdmValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
    using System.Data.Linq;
#endif
    using Microsoft.OData.Client;
    using System.Xml.Linq;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ClrValueToEdmValueTests
    {
        [TestMethod]
        public void UriValueShouldBeConvertable()
        {
            const string testUri = "http://fake.org/";
            ConvertAndValidateString(new Uri(testUri), testUri);
        }

        [TestMethod]
        public void CharValueShouldBeConvertable()
        {
            ConvertAndValidateString('c', "c");
        }

        [TestMethod]
        public void CharArrayValueShouldBeConvertable()
        {
            const string testString = "foo";
            ConvertAndValidateString(testString.ToCharArray(), testString);
        }

        [TestMethod]
        public void TypeValueShouldBeConvertable()
        {
            ConvertAndValidateString(typeof(int), typeof(int).AssemblyQualifiedName);
        }

        [TestMethod]
        public void XmlValueShouldBeConvertable()
        {
            const string testXml = "<fake />";
            ConvertAndValidateString(XDocument.Parse(testXml), testXml);
            ConvertAndValidateString(XElement.Parse(testXml), testXml);
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [TestMethod]
        public void L2SBinaryValueShouldBeConvertable()
        {
            byte[] testBytes = new byte[] { 1, 2, 3 };
            ConvertAndValidateBinary(new Binary(testBytes), testBytes);
        }
#endif

        [TestMethod]
        public void UnsignedIntegerValueShouldBeConvertable()
        {
            ConvertAndValidateString((ulong)1, "1");
            ConvertAndValidateString((uint)1, "1");
            ConvertAndValidateString((ushort)1, "1");
        }

        private static void ConvertAndValidateString(object valueToConvert, string expectedValue)
        {
            ConvertAndValidate<IEdmStringValue, string>(valueToConvert, EdmPrimitiveTypeKind.String, v => v.Value.Should().Be(expectedValue));
        }

        private static void ConvertAndValidateBinary(object valueToConvert, byte[] expectedValue)
        {
            ConvertAndValidate<IEdmBinaryValue, byte[]>(valueToConvert, EdmPrimitiveTypeKind.Binary, v => v.Value.Should().BeEquivalentTo(expectedValue));
        }

        private static void ConvertAndValidate<TExpected, TValue>(object valueToConvert, EdmPrimitiveTypeKind expectedKind, Action<TExpected> validate)
        {
            var converted = EdmValueUtils.ConvertPrimitiveValue(valueToConvert, null);
            converted.Should().NotBeNull();
            converted.Value.Should().BeAssignableTo<TExpected>();
            validate(converted.Value.As<TExpected>());
            converted.Value.Type.Should().NotBeNull();
            converted.Value.Type.PrimitiveKind().Should().Be(expectedKind);
        }
    }
}
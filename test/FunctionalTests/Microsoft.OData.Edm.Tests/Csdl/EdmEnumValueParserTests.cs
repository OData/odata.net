//---------------------------------------------------------------------
// <copyright file="EdmEnumValueParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Values;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl
{
    public class EdmEnumValueParserTests
    {
        [Fact]
        public void TryParseEnumMemberOfOneValueShouldBeTrue()
        {
            var enumType = new EdmEnumType("Ns", "Color");
            var blue = enumType.AddMember("Blue", new EdmIntegerConstant(0));
            enumType.AddMember("White", new EdmIntegerConstant(1));
            var complexType = new EdmComplexType("Ns", "Address");
            string enumPath = "  Ns.Color/Blue  ";
            List<IEdmSchemaType> types = new List<IEdmSchemaType> { enumType, complexType };
            IEnumerable<IEdmEnumMember> parsedMember;
            EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember).Should().BeTrue();
            parsedMember.Single().Should().Be(blue);
        }

        [Fact]
        public void TryParseEnumMemberOfInvalidStringsShouldBeFalse()
        {
            var enumType = new EdmEnumType("Ns", "Color");
            enumType.AddMember("Blue", new EdmIntegerConstant(0));
            enumType.AddMember("White", new EdmIntegerConstant(1));
            var complexType = new EdmComplexType("Ns", "Address");
            string enumPath = "       ";
            List<IEdmSchemaType> types = new List<IEdmSchemaType> { enumType, complexType };
            IEnumerable<IEdmEnumMember> parsedMember;
            EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember).Should().BeFalse();

            enumPath = "        /   ";
            EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember).Should().BeFalse();
        }

        [Fact]
        public void TryParseEnumMemberOfMultipleInvalidTypeShouldBeTrue()
        {
            var complexType = new EdmComplexType("Ns", "Address");
            string enumPath = "Ns.Color/Blue Ns.Color/Red";
            List<IEdmSchemaType> types = new List<IEdmSchemaType> { complexType };
            IEnumerable<IEdmEnumMember> parsedMember;
            EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember).Should().BeTrue();
            parsedMember.Count().Should().Be(2);
            parsedMember.First().Name.Should().Be("Blue");
            parsedMember.ElementAt(1).Name.Should().Be("Red");
        }

        [Fact]
        public void TryParseEnumMemberOfInvalidPathShouldBeFalse()
        {
            var enumType = new EdmEnumType("Ns", "Color");
            enumType.AddMember("Blue", new EdmIntegerConstant(0));
            enumType.AddMember("White", new EdmIntegerConstant(1));
            var complexType = new EdmComplexType("Ns", "Address");
            string enumPath = "Ns.Color//Blue";
            List<IEdmSchemaType> types = new List<IEdmSchemaType> { enumType, complexType };
            IEnumerable<IEdmEnumMember> parsedMember;
            EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember).Should().BeFalse();
        }

        [Fact]
        public void TryParseEnumMemberOfInvalidEnumTypeShouldBeTrue()
        {
            var enumType = new EdmEnumType("Ns", "Color");
            enumType.AddMember("Blue", new EdmIntegerConstant(0));
            enumType.AddMember("White", new EdmIntegerConstant(1));
            var complexType = new EdmComplexType("Ns", "Address");
            string enumPath = "Ns.Colors/Blue";
            List<IEdmSchemaType> types = new List<IEdmSchemaType> { enumType, complexType };
            IEnumerable<IEdmEnumMember> parsedMember;
            EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember).Should().BeTrue();
            parsedMember.Count().Should().Be(1);
            parsedMember.First().Name.Should().Be("Blue");
        }

        [Fact]
        public void TryParseEnumMemberOfInvalidEnumMemberShouldBeFalse()
        {
            var enumType = new EdmEnumType("Ns", "Color");
            enumType.AddMember("Blue", new EdmIntegerConstant(0));
            enumType.AddMember("White", new EdmIntegerConstant(1));
            var complexType = new EdmComplexType("Ns", "Address");
            string enumPath = "Ns.Color/Green";
            List<IEdmSchemaType> types = new List<IEdmSchemaType> { enumType, complexType };
            IEnumerable<IEdmEnumMember> parsedMember;
            EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember).Should().BeFalse();
        }

        [Fact]
        public void TryParseEnumMemberWithFlagsOfTwoValuesShouldBeTrue()
        {
            var enumType = new EdmEnumType("Ns", "Permission", true);
            var read = enumType.AddMember("Read", new EdmIntegerConstant(0));
            var write = enumType.AddMember("Write", new EdmIntegerConstant(1));
            var complexType = new EdmComplexType("Ns", "Address");
            string enumPath = " Ns.Permission/Read   Ns.Permission/Write ";
            List<IEdmSchemaType> types = new List<IEdmSchemaType> { enumType, complexType };
            IEnumerable<IEdmEnumMember> parsedMember;
            EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember).Should().BeTrue();
            parsedMember.Count().Should().Be(2);
            parsedMember.First().Should().Be(read);
            parsedMember.Last().Should().Be(write);
        }

        [Fact]
        public void TryParseEnumMemberWithUnderlyingTypeNotIntegerOfTwoValuesShouldBeFalse()
        {
            var enumType = new EdmEnumType("Ns", "Permission", EdmPrimitiveTypeKind.String, true);
            enumType.AddMember("Read", new EdmStringConstant("1"));
            enumType.AddMember("Write", new EdmStringConstant("2"));
            var complexType = new EdmComplexType("Ns", "Address");
            string enumPath = "Ns.Permission/Read Ns.Permission/Write";
            List<IEdmSchemaType> types = new List<IEdmSchemaType> { enumType, complexType };
            IEnumerable<IEdmEnumMember> parsedMember;
            EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember).Should().BeFalse();
        }

        [Fact]
        public void TryParseEnumMemberOfTwoValuesWithInvalidEnumTypeShouldBeFalse()
        {
            var enumType = new EdmEnumType("Ns", "Permission", EdmPrimitiveTypeKind.String, true);
            enumType.AddMember("Read", new EdmStringConstant("1"));
            enumType.AddMember("Write", new EdmStringConstant("2"));
            var complexType = new EdmComplexType("Ns", "Address");
            string enumPath = "Ns.Permission/Read Ns.Permissions/Write";
            List<IEdmSchemaType> types = new List<IEdmSchemaType> { enumType, complexType };
            IEnumerable<IEdmEnumMember> parsedMember;
            EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember).Should().BeFalse();
        }

        [Fact]
        public void TryParseEnumMemberWithoutFlagsOfTwoValueShouldBeFalse()
        {
            var enumType = new EdmEnumType("Ns", "Permission");
            enumType.AddMember("Read", new EdmIntegerConstant(0));
            enumType.AddMember("Write", new EdmIntegerConstant(1));
            var complexType = new EdmComplexType("Ns", "Address");
            string enumPath = "Ns.Permission/Read Ns.Permission/Write";
            List<IEdmSchemaType> types = new List<IEdmSchemaType> { enumType, complexType };
            IEnumerable<IEdmEnumMember> parsedMember;
            EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember).Should().BeFalse();
        }

        [Fact]
        public void TryParseEnumMemberWithFlagsOfMultiValueShouldBeTrue()
        {
            var enumType = new EdmEnumType("Ns", "Permission", true);
            var read = enumType.AddMember("Read", new EdmIntegerConstant(1));
            var write = enumType.AddMember("Write", new EdmIntegerConstant(2));
            var readwrite = enumType.AddMember("ReadWrite", new EdmIntegerConstant(3));
            var complexType = new EdmComplexType("Ns", "Address");
            string enumPath = "Ns.Permission/Read  Ns.Permission/Write  Ns.Permission/ReadWrite";
            List<IEdmSchemaType> types = new List<IEdmSchemaType> { enumType, complexType };
            IEnumerable<IEdmEnumMember> parsedMember;
            EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember).Should().BeTrue();
            parsedMember.Count().Should().Be(3);
            parsedMember.Should().Contain(read).And.Contain(write).And.Contain(readwrite);
        }

        private static IEdmModel BuildModelFromTypes(IEnumerable<IEdmSchemaType> types)
        {
            var model = new EdmModel();
            model.AddElements(types);
            return model;
        }
    }
}

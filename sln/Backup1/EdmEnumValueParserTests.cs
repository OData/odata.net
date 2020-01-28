//---------------------------------------------------------------------
// <copyright file="EdmEnumValueParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Csdl;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl
{
    public class EdmEnumValueParserTests
    {
        [Fact]
        public void TryParseEnumMemberOfOneValueShouldBeTrue()
        {
            var enumType = new EdmEnumType("Ns", "Color");
            var blue = enumType.AddMember("Blue", new EdmEnumMemberValue(0));
            enumType.AddMember("White", new EdmEnumMemberValue(1));
            var complexType = new EdmComplexType("Ns", "Address");
            string enumPath = "  Ns.Color/Blue  ";
            List<IEdmSchemaType> types = new List<IEdmSchemaType> { enumType, complexType };
            IEnumerable<IEdmEnumMember> parsedMember;
            Assert.True(EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember));
            Assert.Equal(blue, parsedMember.Single());
        }

        [Fact]
        public void TryParseEnumMemberOfInvalidStringsShouldBeFalse()
        {
            var enumType = new EdmEnumType("Ns", "Color");
            enumType.AddMember("Blue", new EdmEnumMemberValue(0));
            enumType.AddMember("White", new EdmEnumMemberValue(1));
            var complexType = new EdmComplexType("Ns", "Address");
            string enumPath = "       ";
            List<IEdmSchemaType> types = new List<IEdmSchemaType> { enumType, complexType };
            IEnumerable<IEdmEnumMember> parsedMember;
            Assert.False(EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember));

            enumPath = "        /   ";
            Assert.False(EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember));
        }

        [Fact]
        public void TryParseEnumMemberOfMultipleInvalidTypeShouldBeTrue()
        {
            var complexType = new EdmComplexType("Ns", "Address");
            string enumPath = "Ns.Color/Blue Ns.Color/Red";
            List<IEdmSchemaType> types = new List<IEdmSchemaType> { complexType };
            IEnumerable<IEdmEnumMember> parsedMember;
            Assert.True(EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember));
            Assert.Equal(2, parsedMember.Count());
            Assert.Equal("Blue", parsedMember.First().Name);
            Assert.Equal("Red", parsedMember.ElementAt(1).Name);
        }

        [Fact]
        public void TryParseEnumMemberOfInvalidPathShouldBeFalse()
        {
            var enumType = new EdmEnumType("Ns", "Color");
            enumType.AddMember("Blue", new EdmEnumMemberValue(0));
            enumType.AddMember("White", new EdmEnumMemberValue(1));
            var complexType = new EdmComplexType("Ns", "Address");
            string enumPath = "Ns.Color//Blue";
            List<IEdmSchemaType> types = new List<IEdmSchemaType> { enumType, complexType };
            IEnumerable<IEdmEnumMember> parsedMember;
            Assert.False(EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember));
        }

        [Fact]
        public void TryParseEnumMemberOfInvalidEnumTypeShouldBeTrue()
        {
            var enumType = new EdmEnumType("Ns", "Color");
            enumType.AddMember("Blue", new EdmEnumMemberValue(0));
            enumType.AddMember("White", new EdmEnumMemberValue(1));
            var complexType = new EdmComplexType("Ns", "Address");
            string enumPath = "Ns.Colors/Blue";
            List<IEdmSchemaType> types = new List<IEdmSchemaType> { enumType, complexType };
            IEnumerable<IEdmEnumMember> parsedMember;
            Assert.True(EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember));
            var mem = Assert.Single(parsedMember);
            Assert.Equal("Blue", mem.Name);
        }

        [Fact]
        public void TryParseEnumMemberOfInvalidEnumMemberShouldBeFalse()
        {
            var enumType = new EdmEnumType("Ns", "Color");
            enumType.AddMember("Blue", new EdmEnumMemberValue(0));
            enumType.AddMember("White", new EdmEnumMemberValue(1));
            var complexType = new EdmComplexType("Ns", "Address");
            string enumPath = "Ns.Color/Green";
            List<IEdmSchemaType> types = new List<IEdmSchemaType> { enumType, complexType };
            IEnumerable<IEdmEnumMember> parsedMember;
            Assert.False(EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember));
        }

        [Fact]
        public void TryParseEnumMemberWithFlagsOfTwoValuesShouldBeTrue()
        {
            var enumType = new EdmEnumType("Ns", "Permission", true);
            var read = enumType.AddMember("Read", new EdmEnumMemberValue(0));
            var write = enumType.AddMember("Write", new EdmEnumMemberValue(1));
            var complexType = new EdmComplexType("Ns", "Address");
            string enumPath = " Ns.Permission/Read   Ns.Permission/Write ";
            List<IEdmSchemaType> types = new List<IEdmSchemaType> { enumType, complexType };
            IEnumerable<IEdmEnumMember> parsedMember;
            Assert.True(EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember));
            Assert.Equal(2, parsedMember.Count());
            Assert.Equal(read, parsedMember.First());
            Assert.Equal(write, parsedMember.Last());
        }

        [Fact]
        public void TryParseEnumMemberWithoutFlagsOfTwoValueShouldBeFalse()
        {
            var enumType = new EdmEnumType("Ns", "Permission");
            enumType.AddMember("Read", new EdmEnumMemberValue(0));
            enumType.AddMember("Write", new EdmEnumMemberValue(1));
            var complexType = new EdmComplexType("Ns", "Address");
            string enumPath = "Ns.Permission/Read Ns.Permission/Write";
            List<IEdmSchemaType> types = new List<IEdmSchemaType> { enumType, complexType };
            IEnumerable<IEdmEnumMember> parsedMember;
            Assert.False(EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember));
        }

        [Fact]
        public void TryParseEnumMemberWithFlagsOfMultiValueShouldBeTrue()
        {
            var enumType = new EdmEnumType("Ns", "Permission", true);
            var read = enumType.AddMember("Read", new EdmEnumMemberValue(1));
            var write = enumType.AddMember("Write", new EdmEnumMemberValue(2));
            var readwrite = enumType.AddMember("ReadWrite", new EdmEnumMemberValue(3));
            var complexType = new EdmComplexType("Ns", "Address");
            string enumPath = "Ns.Permission/Read  Ns.Permission/Write  Ns.Permission/ReadWrite";
            List<IEdmSchemaType> types = new List<IEdmSchemaType> { enumType, complexType };
            IEnumerable<IEdmEnumMember> parsedMember;
            Assert.True(EdmEnumValueParser.TryParseEnumMember(enumPath, BuildModelFromTypes(types), null, out parsedMember));
            Assert.Equal(3, parsedMember.Count());
            Assert.Contains(read, parsedMember);
            Assert.Contains(write, parsedMember);
            Assert.Contains(readwrite, parsedMember);
        }

        private static IEdmModel BuildModelFromTypes(IEnumerable<IEdmSchemaType> types)
        {
            var model = new EdmModel();
            model.AddElements(types);
            return model;
        }
    }
}

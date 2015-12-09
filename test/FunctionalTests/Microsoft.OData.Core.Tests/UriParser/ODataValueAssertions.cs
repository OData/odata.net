//---------------------------------------------------------------------
// <copyright file="ODataValueAssertions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace Microsoft.OData.Core.Tests.UriParser
{
    public static class ODataValueAssertions
    {
        public static AndConstraint<ODataCollectionValue> ShouldBeODataCollectionValue(this object value)
        {
            value.Should().BeAssignableTo<ODataCollectionValue>();
            return new AndConstraint<ODataCollectionValue>(value.As<ODataCollectionValue>());
        }

        public static AndConstraint<IEnumerable<TValue>> ItemsShouldBeAssignableTo<TValue>(this ODataCollectionValue value)
        {
            value.Items.Cast<object>().Should().ContainItemsAssignableTo<TValue>();
            return new AndConstraint<IEnumerable<TValue>>(value.Items.Cast<TValue>());
        }

        public static AndConstraint<ODataComplexValue> ShouldBeODataComplexValue(this object value)
        {
            value.Should().BeAssignableTo<ODataComplexValue>();
            return new AndConstraint<ODataComplexValue>(value.As<ODataComplexValue>());
        }

        public static AndConstraint<ODataEnumValue> ShouldBeODataEnumValue(this object value, string typeName, string val)
        {
            value.Should().BeAssignableTo<ODataEnumValue>();
            var enumVal = value.As<ODataEnumValue>();
            enumVal.TypeName.Should().Be(typeName);
            enumVal.Value.Should().Be(val);
            return new AndConstraint<ODataEnumValue>(enumVal);
        }

        public static AndConstraint<ODataComplexValue> WithTypeName(this AndConstraint<ODataComplexValue> value, string expectedTypeName)
        {
            value.And.TypeName.Should().BeEquivalentTo(expectedTypeName);
            return new AndConstraint<ODataComplexValue>(value.And);
        }
    }
}

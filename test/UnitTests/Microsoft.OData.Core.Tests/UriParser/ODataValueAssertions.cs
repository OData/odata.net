//---------------------------------------------------------------------
// <copyright file="ODataValueAssertions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Microsoft.OData.Tests.UriParser
{
    public static class ODataValueAssertions
    {
        public static ODataCollectionValue ShouldBeODataCollectionValue(this object value)
        {
            Assert.NotNull(value);
            var collectionValue = value as ODataCollectionValue;
            return Assert.IsType<ODataCollectionValue>(value);
        }

        public static IEnumerable<TValue> ItemsShouldBeAssignableTo<TValue>(this ODataCollectionValue value)
        {
            Assert.NotNull(value);
            Assert.NotNull(value.Items);
            foreach (var item in value.Items)
            {
                Assert.IsType<TValue>(item);
            }

            return value.Items.Cast<TValue>();
        }

        public static ODataEnumValue ShouldBeODataEnumValue(this object value, string typeName, string val)
        {
            Assert.NotNull(value);
            var enumVal = Assert.IsType<ODataEnumValue>(value);
            Assert.Equal(typeName, enumVal.TypeName);
            Assert.Equal(val, enumVal.Value);
            return enumVal;
        }
    }
}

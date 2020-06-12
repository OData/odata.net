//---------------------------------------------------------------------
// <copyright file="ODataAnnotationNamesTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.OData.JsonLight;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    using BindingFlags = System.Reflection.BindingFlags;

    public class ODataAnnotationNamesTests
    {
        private static readonly string[] ReservedODataAnnotationNames =
            typeof(ODataAnnotationNames)
#if NETCOREAPP1_1
            .GetFields()
#else
            .GetFields(BindingFlags.NonPublic | BindingFlags.Static)
#endif
            .Where(f => f.FieldType == typeof(string))
            .Select(f => (string)f.GetValue(null)).ToArray();

#if !NETCOREAPP1_1 && !NETCOREAPP2_1&& !NETCOREAPP3_1
        // Not applicable to .NET Core due to changes in framework
        [Fact]
        public void ReservedODataAnnotationNamesHashSetShouldContainAllODataAnnotationNamesSpecialToODataLib()
        {
            Assert.Equal(ReservedODataAnnotationNames.Length, ODataAnnotationNames.KnownODataAnnotationNames.Count);
            foreach (string annotationName in ReservedODataAnnotationNames)
            {
                Assert.True(ODataAnnotationNames.IsODataAnnotationName(annotationName));

                Assert.Contains(annotationName, ODataAnnotationNames.KnownODataAnnotationNames);
                Assert.DoesNotContain(annotationName.ToUpperInvariant(), ODataAnnotationNames.KnownODataAnnotationNames);
            }
        }
#endif

        [Fact]
        public void IsODataAnnotationNameShouldReturnTrueForAnnotationNamesUnderODataNamespace()
        {
            Assert.True(ODataAnnotationNames.IsODataAnnotationName("odata.unknown"));
        }

        [Theory]
        [InlineData("odataa.unknown")]
        [InlineData("oodata.unknown")]
        [InlineData("custom.unknown")]
        [InlineData("OData.unknown")]
        public void IsODataAnnotationNameShouldReturnFalseForAnnotationNamesNotUnderODataNamespace(string annotationName)
        {
            Assert.False(ODataAnnotationNames.IsODataAnnotationName(annotationName));
        }

        [Fact]
        public void IsUnknownODataAnnotationNameShouldReturnFalseOnPropertyName()
        {
            Assert.False(ODataAnnotationNames.IsUnknownODataAnnotationName("odataPropertyName"));
        }

        [Fact]
        public void IsUnknownODataAnnotationNameShouldReturnFalseOnCustomAnnotationName()
        {
            Assert.False(ODataAnnotationNames.IsUnknownODataAnnotationName("custom.annotation"));
            foreach (string annotationName in ReservedODataAnnotationNames)
            {
                // We match the "odata." namespace case sensitive.
                Assert.False(ODataAnnotationNames.IsUnknownODataAnnotationName(annotationName.ToUpperInvariant()));
            }
        }

        [Fact]
        public void IsUnknownODataAnnotationNameShouldReturnFalseOnReservedODataAnnotationName()
        {
            foreach (string annotationName in ReservedODataAnnotationNames)
            {
                Assert.False(ODataAnnotationNames.IsUnknownODataAnnotationName(annotationName));
            }
        }

        [Fact]
        public void IsUnknownODataAnotationNameShouldReturnTrueOnUnknownODataAnnotationName()
        {
            Assert.True(ODataAnnotationNames.IsUnknownODataAnnotationName("odata.unknown"));
        }

        [Fact]
        public void ValidateCustomAnnotationNameShouldThrowOnReservedODataAnnotationName()
        {
            foreach(string annotationName in ReservedODataAnnotationNames)
            {
                Action test = () => ODataAnnotationNames.ValidateIsCustomAnnotationName(annotationName);
                test.Throws<ODataException>(Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties(annotationName));
            }
        }
    }
}

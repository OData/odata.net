//---------------------------------------------------------------------
// <copyright file="ODataAnnotationNamesTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.JsonLight;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    using BindingFlags = System.Reflection.BindingFlags;

    public class ODataAnnotationNamesTests
    {
        
        private static readonly string[] ReservedODataAnnotationNames =
            typeof(ODataAnnotationNames)
#if NETCOREAPP1_0
            .GetFields()
#else
            .GetFields(BindingFlags.NonPublic | BindingFlags.Static)
#endif
            .Where(f => f.FieldType == typeof(string))
            .Select(f => (string)f.GetValue(null)).ToArray();

#if !NETCOREAPP1_0
        // Not applicable to .NET Core due to changes in framework
        [Fact]
        public void ReservedODataAnnotationNamesHashSetShouldContainAllODataAnnotationNamesSpecialToODataLib()
        {
            Assert.Equal(ReservedODataAnnotationNames.Length, ODataAnnotationNames.KnownODataAnnotationNames.Count);
            foreach (string annotationName in ReservedODataAnnotationNames)
            {
                ODataAnnotationNames.IsODataAnnotationName(annotationName).Should().BeTrue();
                ODataAnnotationNames.KnownODataAnnotationNames.Contains(annotationName).Should().BeTrue();
                ODataAnnotationNames.KnownODataAnnotationNames.Contains(annotationName.ToUpperInvariant()).Should().BeFalse();
            }
        }
#endif

        [Fact]
        public void IsODataAnnotationNameShouldReturnTrueForAnnotationNamesUnderODataNamespace()
        {
            ODataAnnotationNames.IsODataAnnotationName("odata.unknown").Should().BeTrue();
        }

        [Fact]
        public void IsODataAnnotationNameShouldReturnFalseForAnnotationNamesNotUnderODataNamespace()
        {
            ODataAnnotationNames.IsODataAnnotationName("odataa.unknown").Should().BeFalse();
            ODataAnnotationNames.IsODataAnnotationName("oodata.unknown").Should().BeFalse();
            ODataAnnotationNames.IsODataAnnotationName("custom.unknown").Should().BeFalse();
            ODataAnnotationNames.IsODataAnnotationName("OData.unknown").Should().BeFalse();
        }

        [Fact]
        public void IsUnknownODataAnnotationNameShouldReturnFalseOnPropertyName()
        {
            ODataAnnotationNames.IsUnknownODataAnnotationName("odataPropertyName").Should().BeFalse();
        }

        [Fact]
        public void IsUnknownODataAnnotationNameShouldReturnFalseOnCustomAnnotationName()
        {
            ODataAnnotationNames.IsUnknownODataAnnotationName("custom.annotation").Should().BeFalse();
            foreach (string annotationName in ReservedODataAnnotationNames)
            {
                // We match the "odata." namespace case sensitive.
                ODataAnnotationNames.IsUnknownODataAnnotationName(annotationName.ToUpperInvariant()).Should().BeFalse();
            }
        }

        [Fact]
        public void IsUnknownODataAnnotationNameShouldReturnFalseOnReservedODataAnnotationName()
        {
            foreach (string annotationName in ReservedODataAnnotationNames)
            {
                ODataAnnotationNames.IsUnknownODataAnnotationName(annotationName).Should().BeFalse();
            }            
        }

        [Fact]
        public void IsUnknownODataAnotationNameShouldReturnTrueOnUnknownODataAnnotationName()
        {
            ODataAnnotationNames.IsUnknownODataAnnotationName("odata.unknown").Should().BeTrue();
        }

        [Fact]
        public void ValidateCustomAnnotationNameShouldThrowOnReservedODataAnnotationName()
        {
            foreach(string annotationName in ReservedODataAnnotationNames)
            {
                Action test = () => ODataAnnotationNames.ValidateIsCustomAnnotationName(annotationName);
                test.ShouldThrow<ODataException>().WithMessage(Strings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties(annotationName));
            }
        }
    }
}

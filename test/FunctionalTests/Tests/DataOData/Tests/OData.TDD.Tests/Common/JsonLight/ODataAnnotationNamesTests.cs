//---------------------------------------------------------------------
// <copyright file="ODataAnnotationNamesTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Common.JsonLight
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.JsonLight;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataAnnotationNamesTests
    {
        private static readonly string[] ReservedODataAnnotationNames =
            typeof(ODataAnnotationNames)
            .GetFields(BindingFlags.NonPublic | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(string))
            .Select(f => (string)f.GetValue(null)).ToArray();

        [TestMethod]
        public void ReservedODataAnnotationNamesHashSetShouldContainAllODataAnnotationNamesSpecialToODataLib()
        {
            Assert.AreEqual(ReservedODataAnnotationNames.Length, ODataAnnotationNames.KnownODataAnnotationNames.Count);
            foreach (string annotationName in ReservedODataAnnotationNames)
            {
                ODataAnnotationNames.IsODataAnnotationName(annotationName).Should().BeTrue();
                ODataAnnotationNames.KnownODataAnnotationNames.Contains(annotationName).Should().BeTrue();
                ODataAnnotationNames.KnownODataAnnotationNames.Contains(annotationName.ToUpperInvariant()).Should().BeFalse();
            }
        }

        [TestMethod]
        public void IsODataAnnotationNameShouldReturnTrueForAnnotationNamesUnderODataNamespace()
        {
            ODataAnnotationNames.IsODataAnnotationName("odata.unknown").Should().BeTrue();
        }

        [TestMethod]
        public void IsODataAnnotationNameShouldReturnFalseForAnnotationNamesNotUnderODataNamespace()
        {
            ODataAnnotationNames.IsODataAnnotationName("odataa.unknown").Should().BeFalse();
            ODataAnnotationNames.IsODataAnnotationName("oodata.unknown").Should().BeFalse();
            ODataAnnotationNames.IsODataAnnotationName("custom.unknown").Should().BeFalse();
            ODataAnnotationNames.IsODataAnnotationName("OData.unknown").Should().BeFalse();
        }

        [TestMethod]
        public void IsUnknownODataAnnotationNameShouldReturnFalseOnPropertyName()
        {
            ODataAnnotationNames.IsUnknownODataAnnotationName("odataPropertyName").Should().BeFalse();
        }

        [TestMethod]
        public void IsUnknownODataAnnotationNameShouldReturnFalseOnCustomAnnotationName()
        {
            ODataAnnotationNames.IsUnknownODataAnnotationName("custom.annotation").Should().BeFalse();
            foreach (string annotationName in ReservedODataAnnotationNames)
            {
                // We match the "odata." namespace case sensitive.
                ODataAnnotationNames.IsUnknownODataAnnotationName(annotationName.ToUpperInvariant()).Should().BeFalse();
            }
        }

        [TestMethod]
        public void IsUnknownODataAnnotationNameShouldReturnFalseOnReservedODataAnnotationName()
        {
            foreach (string annotationName in ReservedODataAnnotationNames)
            {
                ODataAnnotationNames.IsUnknownODataAnnotationName(annotationName).Should().BeFalse();
            }            
        }

        [TestMethod]
        public void IsUnknownODataAnotationNameShouldReturnTrueOnUnknownODataAnnotationName()
        {
            ODataAnnotationNames.IsUnknownODataAnnotationName("odata.unknown").Should().BeTrue();
        }

        [TestMethod]
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

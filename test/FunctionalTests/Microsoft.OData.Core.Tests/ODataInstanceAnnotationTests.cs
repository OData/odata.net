//---------------------------------------------------------------------
// <copyright file="ODataInstanceAnnotationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataInstanceAnnotationTests
    {
        [Fact]
        public void SetNameToNullOrEmptyShouldThrow()
        {
            foreach(string name in new[] {null, string.Empty, ""})
            {
                Action test = () => new ODataInstanceAnnotation(name, new ODataPrimitiveValue("value"));
                test.ShouldThrow<ArgumentNullException>().WithMessage(Strings.ExceptionUtils_ArgumentStringNullOrEmpty + "\r\nParameter name: name");
            }
        }

        [Fact]
        public void InstanceAnnotationNameWithoutPeriodInTheMiddleShouldThrowArgumentException()
        {
            foreach (string name in new[] { "foo", ".foo", "foo." })
            {
                Action test = () => new ODataInstanceAnnotation(name, new ODataPrimitiveValue("value"));
                test.ShouldThrow<ArgumentException>().WithMessage(Strings.ODataInstanceAnnotation_NeedPeriodInName(name));
            }
        }

        [Fact]
        public void AtInInstanceAnnotationNameShouldThrowArgumentException()
        {
            foreach (string name in new[] { "@foo.bar", "foo.b@ar", "foo.bar@" })
            {
                Action test = () => new ODataInstanceAnnotation(name, new ODataPrimitiveValue("value"));
                test.ShouldThrow<ArgumentException>().WithMessage(Strings.ODataInstanceAnnotation_BadTermName(name));
            }
        }

        [Fact]
        public void HashInInstanceAnnotationNameShouldThrowArgumentException()
        {
            foreach (string name in new[] { "#foo.bar", "foo.b#ar", "foo.bar#" })
            {
                Action test = () => new ODataInstanceAnnotation(name, new ODataPrimitiveValue("value"));
                test.ShouldThrow<ArgumentException>().WithMessage(Strings.ODataInstanceAnnotation_BadTermName(name));
            }
        }

        [Fact]
        public void ReservedNameAsInstanceAnnotationNameShouldThrowArgumentException()
        {
            const string annotationName = "odata.unknown";
            Action add = () => new ODataInstanceAnnotation(annotationName, new ODataPrimitiveValue("value"));
            add.ShouldThrow<ArgumentException>().WithMessage(Strings.ODataInstanceAnnotation_ReservedNamesNotAllowed(annotationName, "odata."));
        }

        [Fact]
        public void TheNamePropertyShouldReturnTheAnnotationName()
        {
            const string name = "instance.annotation";
            var annotation = new ODataInstanceAnnotation(name, new ODataPrimitiveValue("value"));
            annotation.Name.Should().Be(name);
        }

        [Fact]
        public void SetValueToNullShouldThrowArgumentNullException()
        {
            Action test = () => new ODataInstanceAnnotation("namespace.name", null); 
            test.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: value");
        }

        [Fact]
        public void SetValueToODataStreamReferenceValueShouldThrowArgumentException()
        {
            Action test = () => new ODataInstanceAnnotation("namespace.name", new ODataStreamReferenceValue());
            test.ShouldThrow<ArgumentException>().WithMessage(Strings.ODataInstanceAnnotation_ValueCannotBeODataStreamReferenceValue + "\r\nParameter name: value");
        }

        [Fact]
        public void TheValuePropertyShouldReturnTheAnnotationValue()
        {
            foreach(ODataValue value in new ODataValue[] {new ODataNullValue(), new ODataPrimitiveValue(1), new ODataComplexValue(), new ODataCollectionValue()})
            {
                var annotation = new ODataInstanceAnnotation("namespace.name", value);
                annotation.Value.Should().BeSameAs(value);
            }
        }
    }
}

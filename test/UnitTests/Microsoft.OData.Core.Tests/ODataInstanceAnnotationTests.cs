//---------------------------------------------------------------------
// <copyright file="ODataInstanceAnnotationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Core;
using System;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataInstanceAnnotationTests
    {
        [Fact]
        public void SetNameToNullOrEmptyShouldThrow()
        {
            foreach (string name in new[] { null, string.Empty, "" })
            {
                Action test = () => new ODataInstanceAnnotation(name, new ODataPrimitiveValue("value"));
                Assert.Throws<ArgumentNullException>("annotationName", test);
            }
        }

        [Fact]
        public void InstanceAnnotationNameWithoutPeriodInTheMiddleShouldThrowArgumentException()
        {
            foreach (string name in new[] { "foo", ".foo", "foo." })
            {
                Action test = () => new ODataInstanceAnnotation(name, new ODataPrimitiveValue("value"));
                test.Throws<ArgumentException>(Error.Format(SRResources.ODataInstanceAnnotation_NeedPeriodInName, name));
            }
        }

        [Fact]
        public void AtInInstanceAnnotationNameShouldThrowArgumentException()
        {
            foreach (string name in new[] { "@foo.bar", "foo.b@ar", "foo.bar@" })
            {
                Action test = () => new ODataInstanceAnnotation(name, new ODataPrimitiveValue("value"));
                test.Throws<ArgumentException>(Error.Format(SRResources.ODataInstanceAnnotation_BadTermName, name));
            }
        }

        [Fact]
        public void HashInInstanceAnnotationNameShouldThrowArgumentException()
        {
            foreach (string name in new[] { "#foo.bar", "foo.b#ar", "foo.bar#" })
            {
                Action test = () => new ODataInstanceAnnotation(name, new ODataPrimitiveValue("value"));
                test.Throws<ArgumentException>(Error.Format(SRResources.ODataInstanceAnnotation_BadTermName, name));
            }
        }

        [Fact]
        public void ReservedNameAsInstanceAnnotationNameShouldThrowArgumentException()
        {
            const string annotationName = "odata.unknown";
            Action add = () => new ODataInstanceAnnotation(annotationName, new ODataPrimitiveValue("value"));
            add.Throws<ArgumentException>(Error.Format(SRResources.ODataInstanceAnnotation_ReservedNamesNotAllowed, annotationName, "odata."));
        }

        [Fact]
        public void TheNamePropertyShouldReturnTheAnnotationName()
        {
            const string name = "instance.annotation";
            var annotation = new ODataInstanceAnnotation(name, new ODataPrimitiveValue("value"));
            Assert.Equal(name, annotation.Name);
        }

        [Fact]
        public void SetValueToNullShouldThrowArgumentNullException()
        {
            Action test = () => new ODataInstanceAnnotation("namespace.name", null);
            Assert.Throws<ArgumentNullException>("value", test);
        }

        [Fact]
        public void SetValueToODataStreamReferenceValueShouldThrowArgumentException()
        {
            Action test = () => new ODataInstanceAnnotation("namespace.name", new ODataStreamReferenceValue());
            Assert.Throws<ArgumentException>("value", test);
        }

        [Fact]
        public void TheValuePropertyShouldReturnTheAnnotationValue()
        {
            foreach (ODataValue value in new ODataValue[] { ODataNullValue.Instance, new ODataPrimitiveValue(1), new ODataResourceValue(), new ODataCollectionValue() })
            {
                var annotation = new ODataInstanceAnnotation("namespace.name", value);
                Assert.Same(value, annotation.Value);
            }
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="PropertyAndAnnotationCollectorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.JsonLight;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests
{
    public class PropertyAndAnnotationCollectorTests
    {
        [Fact]
        public void DuplicateInstanceODataAnnotationShouldFail()
        {
            PropertyAndAnnotationCollector duplicateChecker = new PropertyAndAnnotationCollector(true);
            Action action = () => duplicateChecker.MarkPropertyAsProcessed(JsonLightConstants.ODataAnnotationNamespacePrefix + "name");
            action.ShouldNotThrow();
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.DuplicateAnnotationNotAllowed(JsonLightConstants.ODataAnnotationNamespacePrefix + "name"));
        }

        [Fact]
        public void DuplicatePropertyODataAnnotationShouldFail()
        {
            PropertyAndAnnotationCollector duplicateChecker = new PropertyAndAnnotationCollector(true);
            Action action = () => duplicateChecker.AddODataPropertyAnnotation("property", JsonLightConstants.ODataAnnotationNamespacePrefix + "name", "SomeValue");
            action.ShouldNotThrow();
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.DuplicateAnnotationForPropertyNotAllowed(JsonLightConstants.ODataAnnotationNamespacePrefix + "name", "property"));
        }

        [Fact]
        public void DuplicatePropertyCustomAnnotationShouldNotFail()
        {
            PropertyAndAnnotationCollector duplicateChecker = new PropertyAndAnnotationCollector(true);
            Action action = () => duplicateChecker.AddCustomPropertyAnnotation("property", "custom.name", "value");
            action.ShouldNotThrow();
            action.ShouldNotThrow();
        }

        [Fact]
        public void NoAnnotationsForObjectScopeAddedShouldReturnEmpty()
        {
            PropertyAndAnnotationCollector duplicateChecker = new PropertyAndAnnotationCollector(true);
            duplicateChecker.GetODataPropertyAnnotations(string.Empty).Should().BeEmpty();
        }

        [Fact]
        public void NoAnnotationsForPropertyAddedShouldReturnEmpty()
        {
            PropertyAndAnnotationCollector duplicateChecker = new PropertyAndAnnotationCollector(true);
            duplicateChecker.GetODataPropertyAnnotations("property").Should().BeEmpty();
        }

        [Fact]
        public void OnlyCustomAnnotationsForPropertyAddedShouldReturnEmpty()
        {
            PropertyAndAnnotationCollector duplicateChecker = new PropertyAndAnnotationCollector(true);
            duplicateChecker.AddCustomPropertyAnnotation("property", "custom.annotation", "value");
            duplicateChecker.AddCustomPropertyAnnotation("property", "custom.annotation2", "value");
            duplicateChecker.GetODataPropertyAnnotations("property").Should().BeEmpty();
        }

        [Fact]
        public void AnnotationsForPropertyShouldBeStored()
        {
            PropertyAndAnnotationCollector duplicateChecker = new PropertyAndAnnotationCollector(true);
            duplicateChecker.AddODataPropertyAnnotation("property", JsonLightConstants.ODataAnnotationNamespacePrefix + "one", 1);
            duplicateChecker.AddODataPropertyAnnotation("property", JsonLightConstants.ODataAnnotationNamespacePrefix + "two", "Two");
            duplicateChecker.GetODataPropertyAnnotations("property").Should().Equal(new Dictionary<string, object>()
            {
                { JsonLightConstants.ODataAnnotationNamespacePrefix + "one", 1 },
                { JsonLightConstants.ODataAnnotationNamespacePrefix + "two", "Two" }
            });
        }

        [Fact]
        public void MarkPropertyAsProcessedWithNoPropertyShouldWork()
        {
            PropertyAndAnnotationCollector duplicateChecker = new PropertyAndAnnotationCollector(true);
            duplicateChecker.MarkPropertyAsProcessed("property");
            Action odataAction = () => duplicateChecker.AddODataPropertyAnnotation("property", JsonLightConstants.ODataAnnotationNamespacePrefix + "name", "value");
            odataAction.ShouldThrow<ODataException>().WithMessage(ErrorStrings.PropertyAnnotationAfterTheProperty(JsonLightConstants.ODataAnnotationNamespacePrefix + "name", "property"));
            Action customAction = () => duplicateChecker.AddCustomPropertyAnnotation("property", "custom.name", "value");
            customAction.ShouldThrow<ODataException>().WithMessage(ErrorStrings.PropertyAnnotationAfterTheProperty("custom.name", "property"));
        }

        [Fact]
        public void MarkPropertyAsProcessedWithSomeAnnotationsShouldWork()
        {
            PropertyAndAnnotationCollector duplicateChecker = new PropertyAndAnnotationCollector(true);
            duplicateChecker.AddODataPropertyAnnotation("property", JsonLightConstants.ODataAnnotationNamespacePrefix + "first", 42);
            duplicateChecker.MarkPropertyAsProcessed("property");
            Action odataAction = () => duplicateChecker.AddODataPropertyAnnotation("property", JsonLightConstants.ODataAnnotationNamespacePrefix + "name", "value");
            odataAction.ShouldThrow<ODataException>().WithMessage(ErrorStrings.PropertyAnnotationAfterTheProperty(JsonLightConstants.ODataAnnotationNamespacePrefix + "name", "property"));
            Action customAction = () => duplicateChecker.AddCustomPropertyAnnotation("property", "custom.name", "value");
            customAction.ShouldThrow<ODataException>().WithMessage(ErrorStrings.PropertyAnnotationAfterTheProperty("custom.name", "property"));
        }
    }
}

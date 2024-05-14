//---------------------------------------------------------------------
// <copyright file="PropertyAndAnnotationCollectorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Json;
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
            Action action = () => duplicateChecker.MarkPropertyAsProcessed(ODataJsonConstants.ODataAnnotationNamespacePrefix + "name");
            action.DoesNotThrow();
            action.Throws<ODataException>(ErrorStrings.DuplicateAnnotationNotAllowed(ODataJsonConstants.ODataAnnotationNamespacePrefix + "name"));
        }

        [Fact]
        public void DuplicatePropertyODataAnnotationShouldFail()
        {
            PropertyAndAnnotationCollector duplicateChecker = new PropertyAndAnnotationCollector(true);
            Action action = () => duplicateChecker.AddODataPropertyAnnotation("property", ODataJsonConstants.ODataAnnotationNamespacePrefix + "name", "SomeValue");
            action.DoesNotThrow();
            action.Throws<ODataException>(ErrorStrings.DuplicateAnnotationForPropertyNotAllowed(ODataJsonConstants.ODataAnnotationNamespacePrefix + "name", "property"));
        }

        [Fact]
        public void DuplicatePropertyCustomAnnotationShouldNotFail()
        {
            PropertyAndAnnotationCollector duplicateChecker = new PropertyAndAnnotationCollector(true);
            Action action = () => duplicateChecker.AddCustomPropertyAnnotation("property", "custom.name", "value");
            action.DoesNotThrow();
            action.DoesNotThrow();
        }

        [Fact]
        public void NoAnnotationsForObjectScopeAddedShouldReturnEmpty()
        {
            PropertyAndAnnotationCollector duplicateChecker = new PropertyAndAnnotationCollector(true);
            Assert.Empty(duplicateChecker.GetODataPropertyAnnotations(string.Empty));
        }

        [Fact]
        public void NoAnnotationsForPropertyAddedShouldReturnEmpty()
        {
            PropertyAndAnnotationCollector duplicateChecker = new PropertyAndAnnotationCollector(true);
            Assert.Empty(duplicateChecker.GetODataPropertyAnnotations("property"));
        }

        [Fact]
        public void OnlyCustomAnnotationsForPropertyAddedShouldReturnEmpty()
        {
            PropertyAndAnnotationCollector duplicateChecker = new PropertyAndAnnotationCollector(true);
            duplicateChecker.AddCustomPropertyAnnotation("property", "custom.annotation", "value");
            duplicateChecker.AddCustomPropertyAnnotation("property", "custom.annotation2", "value");
            Assert.Empty(duplicateChecker.GetODataPropertyAnnotations("property"));
        }

        [Fact]
        public void AnnotationsForPropertyShouldBeStored()
        {
            PropertyAndAnnotationCollector duplicateChecker = new PropertyAndAnnotationCollector(true);
            duplicateChecker.AddODataPropertyAnnotation("property", ODataJsonConstants.ODataAnnotationNamespacePrefix + "one", 1);
            duplicateChecker.AddODataPropertyAnnotation("property", ODataJsonConstants.ODataAnnotationNamespacePrefix + "two", "Two");
            var annotations = duplicateChecker.GetODataPropertyAnnotations("property");
            Assert.Equal(new Dictionary<string, object>()
            {
                { ODataJsonConstants.ODataAnnotationNamespacePrefix + "one", 1 },
                { ODataJsonConstants.ODataAnnotationNamespacePrefix + "two", "Two" }
            }, annotations);
        }

        [Fact]
        public void MarkPropertyAsProcessedWithNoPropertyShouldWork()
        {
            PropertyAndAnnotationCollector duplicateChecker = new PropertyAndAnnotationCollector(true);
            duplicateChecker.MarkPropertyAsProcessed("property");
            Action odataAction = () => duplicateChecker.AddODataPropertyAnnotation("property", ODataJsonConstants.ODataAnnotationNamespacePrefix + "name", "value");
            odataAction.Throws<ODataException>(ErrorStrings.PropertyAnnotationAfterTheProperty(ODataJsonConstants.ODataAnnotationNamespacePrefix + "name", "property"));
            Action customAction = () => duplicateChecker.AddCustomPropertyAnnotation("property", "custom.name", "value");
            customAction.Throws<ODataException>(ErrorStrings.PropertyAnnotationAfterTheProperty("custom.name", "property"));
        }

        [Fact]
        public void MarkPropertyAsProcessedWithSomeAnnotationsShouldWork()
        {
            PropertyAndAnnotationCollector duplicateChecker = new PropertyAndAnnotationCollector(true);
            duplicateChecker.AddODataPropertyAnnotation("property", ODataJsonConstants.ODataAnnotationNamespacePrefix + "first", 42);
            duplicateChecker.MarkPropertyAsProcessed("property");
            Action odataAction = () => duplicateChecker.AddODataPropertyAnnotation("property", ODataJsonConstants.ODataAnnotationNamespacePrefix + "name", "value");
            odataAction.Throws<ODataException>(ErrorStrings.PropertyAnnotationAfterTheProperty(ODataJsonConstants.ODataAnnotationNamespacePrefix + "name", "property"));
            Action customAction = () => duplicateChecker.AddCustomPropertyAnnotation("property", "custom.name", "value");
            customAction.Throws<ODataException>(ErrorStrings.PropertyAnnotationAfterTheProperty("custom.name", "property"));
        }
    }
}

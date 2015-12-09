//---------------------------------------------------------------------
// <copyright file="DuplicatePropertyNamesCheckerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Core.JsonLight;
using Xunit;
using ErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests
{
    public class DuplicatePropertyNamesCheckerTests
    {
        [Fact]
        public void DuplicateInstanceODataAnnotationShouldFail()
        {
            DuplicatePropertyNamesChecker duplicateChecker = new DuplicatePropertyNamesChecker(false, true);
            Action action = () => duplicateChecker.MarkPropertyAsProcessed(JsonLightConstants.ODataAnnotationNamespacePrefix + "name");
            action.ShouldNotThrow();
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.DuplicatePropertyNamesChecker_DuplicateAnnotationNotAllowed(JsonLightConstants.ODataAnnotationNamespacePrefix + "name"));
        }

        [Fact]
        public void DuplicateInstanceCustomAnnotationShouldFail()
        {
            DuplicatePropertyNamesChecker duplicateChecker = new DuplicatePropertyNamesChecker(false, true);
            Action action = () => duplicateChecker.MarkPropertyAsProcessed("custom.name");
            action.ShouldNotThrow();
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.DuplicatePropertyNamesChecker_DuplicateAnnotationNotAllowed("custom.name"));
        }

        [Fact]
        public void DuplicatePropertyODataAnnotationShouldFail()
        {
            DuplicatePropertyNamesChecker duplicateChecker = new DuplicatePropertyNamesChecker(false, true);
            Action action = () => duplicateChecker.AddODataPropertyAnnotation("property", JsonLightConstants.ODataAnnotationNamespacePrefix + "name", null);
            action.ShouldNotThrow();
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.DuplicatePropertyNamesChecker_DuplicateAnnotationForPropertyNotAllowed(JsonLightConstants.ODataAnnotationNamespacePrefix + "name", "property"));
        }

        [Fact]
        public void DuplicatePropertyCustomAnnotationShouldFail()
        {
            DuplicatePropertyNamesChecker duplicateChecker = new DuplicatePropertyNamesChecker(false, true);
            Action action = () => duplicateChecker.AddCustomPropertyAnnotation("property", "custom.name");
            action.ShouldNotThrow();
            action.ShouldThrow<ODataException>().WithMessage(ErrorStrings.DuplicatePropertyNamesChecker_DuplicateAnnotationForPropertyNotAllowed("custom.name", "property"));
        }

        [Fact]
        public void NoAnnotationsForObjectScopeAddedShouldReturnNull()
        {
            DuplicatePropertyNamesChecker duplicateChecker = new DuplicatePropertyNamesChecker(false, true);
            duplicateChecker.GetODataPropertyAnnotations(string.Empty).Should().BeNull();
        }

        [Fact]
        public void NoAnnotationsForPropertyAddedShouldReturnNull()
        {
            DuplicatePropertyNamesChecker duplicateChecker = new DuplicatePropertyNamesChecker(false, true);
            duplicateChecker.GetODataPropertyAnnotations("property").Should().BeNull();
        }

        [Fact]
        public void OnlyCustomAnnotationsForPropertyAddedShouldReturnNull()
        {
            DuplicatePropertyNamesChecker duplicateChecker = new DuplicatePropertyNamesChecker(false, true);
            duplicateChecker.AddCustomPropertyAnnotation("property", "custom.annotation");
            duplicateChecker.AddCustomPropertyAnnotation("property", "custom.annotation2");
            duplicateChecker.GetODataPropertyAnnotations("property").Should().BeNull();
        }

        [Fact]
        public void AnnotationsForPropertyShouldBeStored()
        {
            DuplicatePropertyNamesChecker duplicateChecker = new DuplicatePropertyNamesChecker(false, true);
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
            DuplicatePropertyNamesChecker duplicateChecker = new DuplicatePropertyNamesChecker(false, true);
            duplicateChecker.MarkPropertyAsProcessed("property");
            Action odataAction = () => duplicateChecker.AddODataPropertyAnnotation("property", JsonLightConstants.ODataAnnotationNamespacePrefix + "name", null);
            odataAction.ShouldThrow<ODataException>().WithMessage(ErrorStrings.DuplicatePropertyNamesChecker_PropertyAnnotationAfterTheProperty(JsonLightConstants.ODataAnnotationNamespacePrefix + "name", "property"));
            Action customAction = () => duplicateChecker.AddCustomPropertyAnnotation("property", "custom.name");
            customAction.ShouldThrow<ODataException>().WithMessage(ErrorStrings.DuplicatePropertyNamesChecker_PropertyAnnotationAfterTheProperty("custom.name", "property"));
        }

        [Fact]
        public void MarkPropertyAsProcessedWithSomeAnnotationsShouldWork()
        {
            DuplicatePropertyNamesChecker duplicateChecker = new DuplicatePropertyNamesChecker(false, true);
            duplicateChecker.AddODataPropertyAnnotation("property", JsonLightConstants.ODataAnnotationNamespacePrefix + "first", 42);
            duplicateChecker.MarkPropertyAsProcessed("property");
            Action odataAction = () => duplicateChecker.AddODataPropertyAnnotation("property", JsonLightConstants.ODataAnnotationNamespacePrefix + "name", null);
            odataAction.ShouldThrow<ODataException>().WithMessage(ErrorStrings.DuplicatePropertyNamesChecker_PropertyAnnotationAfterTheProperty(JsonLightConstants.ODataAnnotationNamespacePrefix + "name", "property"));
            Action customAction = () => duplicateChecker.AddCustomPropertyAnnotation("property", "custom.name");
            customAction.ShouldThrow<ODataException>().WithMessage(ErrorStrings.DuplicatePropertyNamesChecker_PropertyAnnotationAfterTheProperty("custom.name", "property"));
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="ServerUrlConventionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using Microsoft.OData.Client;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using AstoriaUnitTests.TDD.Tests.Server.Simulators;
    using AstoriaUnitTests.Tests.Server.Simulators;
    using FluentAssertions;
    using Microsoft.OData.Core.Tests;
    using Microsoft.OData.Edm.Annotations;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm.Library.Values;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData.Service;
    using UrlConvention = Microsoft.OData.Service.UrlConvention;

    [TestClass]
    public class ServerUrlConventionTests
    {
        private readonly DataServiceBehavior defaultBehavior = new DataServiceBehavior { UrlConventions = DataServiceUrlConventions.Default };
        private readonly DataServiceBehavior behaviorWithKeysAsSegments = new DataServiceBehavior { UrlConventions = DataServiceUrlConventions.KeyAsSegment };
        
        [TestMethod]
        public void UrlConventionShouldNotBeKeysAsSegmentsIfKnobIsNotFlipped()
        {
            UrlConvention.CreateFromUserInput(this.defaultBehavior, ShouldNotBeCalled).GenerateKeyAsSegment.Should().BeFalse();
        }

        [TestMethod]
        public void UrlConventionShouldNotBeKeysAsSegmentsIfKnobIsFlippedButHeaderIsDefault()
        {
            UrlConvention.CreateFromUserInput(this.behaviorWithKeysAsSegments, DefaultUrlConventionHeaderValue).GenerateKeyAsSegment.Should().BeFalse();
        }

        [TestMethod]
        public void UrlConventionShouldNotBeKeysAsSegmentsIfKnobIsFlippedButHeaderIsMissing()
        {
            UrlConvention.CreateFromUserInput(this.behaviorWithKeysAsSegments, NoHeader).GenerateKeyAsSegment.Should().BeFalse();
        }

        [TestMethod]
        public void UrlConventionShouldShouldThrowIfKnobIsFlippedAndHeaderIsWrongValue()
        {
            Action callOverload = () => UrlConvention.CreateFromUserInput(this.behaviorWithKeysAsSegments, WrongHeader);
            callOverload.ShouldThrow<DataServiceException>().WithMessage(Microsoft.OData.Service.Strings.UrlConvention_BadRequestIfUnknown("wrong", "DataServiceUrlConventions", "Default", "KeyAsSegment"));
        }

        [TestMethod]
        public void UrlConventionShouldBeKeysAsSegmentsIfKnobIsFlippedAndHeaderIsCorrectValue()
        {
            UrlConvention.CreateFromUserInput(this.behaviorWithKeysAsSegments, KeysAsSegmentsHeaderValue).GenerateKeyAsSegment.Should().BeTrue();
        }

        [TestMethod]
        public void UrlConventionIntegrationTest()
        {
            var host = new DataServiceHost2Simulator
                       {
                           RequestHeaders = new WebHeaderCollection { { UrlConventionsConstants.UrlConventionHeaderName, "KeyAsSegment" } },
                       };
            var service = new DataServiceSimulator
            {
                Configuration = new DataServiceConfiguration(new DataServiceProviderSimulator()), 
                OperationContext = new DataServiceOperationContext(host)
            };

            service.OperationContext.InitializeAndCacheHeaders(service);

            service.Configuration.DataServiceBehavior.UrlConventions = DataServiceUrlConventions.KeyAsSegment;
            UrlConvention.Create(service).GenerateKeyAsSegment.Should().BeTrue();
        }

        [TestMethod]
        public void DefaultUrlConventionShouldNotAddAnnotationsToModel()
        {
            EdmEntityContainer container;
            GetMetadataAnnotations(new DataServiceBehavior(), out container).Should().BeEmpty();
        }

        [TestMethod]
        public void KeyAsSegmenttUrlConventionShouldAddAnnotationsToModel()
        {
            EdmEntityContainer container;

            var annotations = GetMetadataAnnotations(new DataServiceBehavior { UrlConventions = DataServiceUrlConventions.KeyAsSegment }, out container).ToList();
            annotations.Should().HaveCount(1);

            var annotation = annotations.Single();
            annotation.Should().BeAssignableTo<IEdmValueAnnotation>();
            annotation.Target.Should().BeSameAs(container);
            annotation.Term.Name.Should().Be(UrlConventionsConstants.ConventionTermName);
            annotation.Term.Namespace.Should().Be(UrlConventionsConstants.ConventionTermNamespace);
            annotation.As<IEdmValueAnnotation>().Value.Should().BeAssignableTo<EdmStringConstant>();
            annotation.As<IEdmValueAnnotation>().Value.As<EdmStringConstant>().Value.Should().Be(UrlConventionsConstants.KeyAsSegmentAnnotationValueString);
        }

        private static IEnumerable<IEdmVocabularyAnnotation> GetMetadataAnnotations(DataServiceBehavior dataServiceBehavior, out EdmEntityContainer container)
        {
            container = new EdmEntityContainer("Fake", "Container");
            var primaryModel = new EdmModel();
            primaryModel.AddElement(container);
            
            return UrlConvention.BuildMetadataAnnotations(dataServiceBehavior, primaryModel);
        }

        private static string ShouldNotBeCalled(string option)
        {
            throw new Exception();
        }

        private static string DefaultUrlConventionHeaderValue(string option)
        {
            option.Should().Be(UrlConventionsConstants.UrlConventionHeaderName);
            return "Default";
        }

        private static string NoHeader(string option)
        {
            option.Should().Be(UrlConventionsConstants.UrlConventionHeaderName);
            return null;
        }

        private static string WrongHeader(string option)
        {
            option.Should().Be(UrlConventionsConstants.UrlConventionHeaderName);
            return "wrong";
        }

        private static string KeysAsSegmentsHeaderValue(string option)
        {
            option.Should().Be(UrlConventionsConstants.UrlConventionHeaderName);
            return "KeyAsSegment";
        }
    }
}

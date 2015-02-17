//---------------------------------------------------------------------
// <copyright file="DataServiceConfigurationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Service;
using Microsoft.OData.Service.Providers;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AstoriaUnitTests.Tests.Server
{
    using Microsoft.OData.Client;
    using AstoriaUnitTests.Tests.Server.Simulators;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Unit tests for DataServiceConfiguration
    ///</summary>
    [TestClass]
    public class DataServiceConfigurationTests
    {
        [TestMethod()]
        public void ValidateGenerateKeyAsSegmentIndicatesMetadataHasAnnotation()
        {
            var metadataSimulator = new DataServiceProviderSimulator();
            var dataServiceConfiguration = new DataServiceConfiguration(metadataSimulator);
            dataServiceConfiguration.DataServiceBehavior.UrlConventions = DataServiceUrlConventions.KeyAsSegment;
            Assert.IsTrue(dataServiceConfiguration.HasAnnotations());
        }

        [TestMethod()]
        public void ValidateSettingAnnotationBuilderIndicatesMetadataHasAnnotation()
        {
            var metadataSimulator = new DataServiceProviderSimulator();
            var dataServiceConfiguration = new DataServiceConfiguration(metadataSimulator);
            dataServiceConfiguration.AnnotationsBuilder = (IEdmModel model) => new IEdmModel[] {model};
            Assert.IsTrue(dataServiceConfiguration.HasAnnotations());
        }

        [TestMethod()]
        public void ValidateWhenUrlAndSettingAnnotationBuilderNotSetIndicatesMetadataHasNoAnnotation()
        {
            var metadataSimulator = new DataServiceProviderSimulator();
            var dataServiceConfiguration = new DataServiceConfiguration(metadataSimulator);
            Assert.IsFalse(dataServiceConfiguration.HasAnnotations());
        }
    }
}

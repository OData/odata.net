//---------------------------------------------------------------------
// <copyright file="DataServiceConfigurationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AstoriaUnitTests.Tests.Server
{
    using AstoriaUnitTests.Tests.Server.Simulators;
    using Microsoft.OData.Client;
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
            dataServiceConfiguration.DataServiceBehavior.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
            Assert.IsTrue(dataServiceConfiguration.HasAnnotations());
        }

        [TestMethod()]
        public void ValidateSettingAnnotationBuilderIndicatesMetadataHasAnnotation()
        {
            var metadataSimulator = new DataServiceProviderSimulator();
            var dataServiceConfiguration = new DataServiceConfiguration(metadataSimulator);
            dataServiceConfiguration.AnnotationsBuilder = (IEdmModel model) => new IEdmModel[] { model };
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

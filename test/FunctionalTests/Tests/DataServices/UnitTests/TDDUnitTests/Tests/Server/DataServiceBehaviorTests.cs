//---------------------------------------------------------------------
// <copyright file="DataServiceBehaviorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service.Configuration;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData.Service;
    
    [TestClass]
    public class DataServiceBehaviorTests
    {
        [TestMethod]
        public void ReplaceShouldBeDisabledByDefault()
        {
            new DataServiceBehavior().AcceptReplaceFunctionInQuery.Should().BeFalse();
        }

        [TestMethod]
        public void ConfigurationFileShouldSupercedeApiForReplace()
        {
            var testSubject = new DataServiceBehavior { AcceptReplaceFunctionInQuery = false };
            DataServicesReplaceFunctionFeature replaceFeature = new TestReplaceFeature(true) { Enable = true };
            DataServicesFeaturesSection dataServicesFeaturesSection = new DataServicesFeaturesSection { ReplaceFunction = replaceFeature };
            testSubject.ApplySettingsFromConfiguration(dataServicesFeaturesSection);
            testSubject.AcceptReplaceFunctionInQuery.Should().BeTrue();

            replaceFeature.Enable = false;
            testSubject.ApplySettingsFromConfiguration(dataServicesFeaturesSection);
            testSubject.AcceptReplaceFunctionInQuery.Should().BeFalse();
        }

        [TestMethod]
        public void ConfigurationFileShouldForReplaceShouldOnlyBeAppliedIfPresent()
        {
            var testSubject = new DataServiceBehavior { AcceptReplaceFunctionInQuery = true };
            testSubject.ApplySettingsFromConfiguration(new DataServicesFeaturesSection { ReplaceFunction = new TestReplaceFeature(false) { Enable = false } });
            testSubject.AcceptReplaceFunctionInQuery.Should().BeTrue();
        }

        [TestMethod]
        public void GenerateKeyAsSegmentsShouldBeOffByDefault()
        {
            new DataServiceBehavior().UrlConventions.Should().BeSameAs(DataServiceUrlConventions.Default);
        }

        [TestMethod]
        public void UrlConventionShouldNotAllowNull()
        {
            Action setToNull = () => new DataServiceBehavior().UrlConventions = null;
            setToNull.ShouldThrow<ArgumentNullException>().WithMessage("UrlConventions", ComparisonMode.Substring);
        }

        private class TestReplaceFeature : DataServicesReplaceFunctionFeature
        {
            private readonly bool isPresent;

            public TestReplaceFeature(bool isPresent)
            {
                this.isPresent = isPresent;
            }

            internal override bool IsPresent
            {
                get { return this.isPresent; }
            }
        }
    }
}
//---------------------------------------------------------------------
// <copyright file="DataServiceBehaviorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            new DataServiceBehavior().UrlKeyDelimiter.Should().BeSameAs(DataServiceUrlKeyDelimiter.Parentheses);
        }

        [TestMethod]
        public void UrlConventionShouldNotAllowNull()
        {
            Action setToNull = () => new DataServiceBehavior().UrlKeyDelimiter = null;
            setToNull.ShouldThrow<ArgumentNullException>().WithMessage("UrlKeyDelimiter", ComparisonMode.Substring);
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
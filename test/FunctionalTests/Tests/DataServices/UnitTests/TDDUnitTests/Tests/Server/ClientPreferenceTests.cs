//---------------------------------------------------------------------
// <copyright file="ClientPreferenceTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using Microsoft.OData.Service;
    using FluentAssertions;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ClientPreferenceTests
    {
        [TestMethod]
        public void AnnotationFilterShouldNotBeSetAndRequiredResponseVersionShouldBe10WhenTheODataAnnotationsPreferenceIsMissing()
        {
            RequestDescription descrption = new RequestDescription(RequestTargetKind.Link, RequestTargetSource.ServiceOperation, new Uri("http://service/set"));
            IODataRequestMessage requestMessage = new ODataRequestMessageSimulator();
            ClientPreference preference = new ClientPreference(descrption, HttpVerbs.None, requestMessage, effectiveMaxResponseVersion: VersionUtil.Version4Dot0);
            preference.AnnotationFilter.Should().BeNull();
            preference.RequiredResponseVersion.Should().Be(VersionUtil.Version4Dot0);
        }

        [TestMethod]
        public void AnnotationFilterShouldBeSetWithODataAnnotationsPreferenceAndRequiredResponseVersionShouldBe30WhenEffectiveMaxResponseVersionIs30()
        {
            RequestDescription descrption = new RequestDescription(RequestTargetKind.Link, RequestTargetSource.ServiceOperation, new Uri("http://service/set"));
            IODataRequestMessage requestMessage = new ODataRequestMessageSimulator();
            requestMessage.PreferHeader().AnnotationFilter = "*";
            ClientPreference preference = new ClientPreference(descrption, HttpVerbs.None, requestMessage, effectiveMaxResponseVersion: VersionUtil.Version4Dot0);
            preference.AnnotationFilter.Should().Be("*");
            preference.RequiredResponseVersion.Should().Be(VersionUtil.Version4Dot0);
        }
    }
}

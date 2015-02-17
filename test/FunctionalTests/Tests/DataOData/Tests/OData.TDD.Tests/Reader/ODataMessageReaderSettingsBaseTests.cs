//---------------------------------------------------------------------
// <copyright file="ODataMessageReaderSettingsBaseTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Reader
{
    #region Namespaces
    using System;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Atom;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;
    #endregion Namespaces

    /// <summary>
    /// Tests for ODataMessageReaderSettingsBase class.
    /// </summary>
    [TestClass]
    public class ODataMessageReaderSettingsBaseTests
    {
        [TestMethod]
        public void DefaultSettingsBaseTest()
        {
            CustomODataMessageReaderSettings settings = new CustomODataMessageReaderSettings();
            settings.CheckCharacters.Should().BeFalse();
            settings.EnableAtomMetadataReading.Should().BeFalse();
            settings.InstanceAnnotationFilter.Should().BeNull();
        }

        [TestMethod]
        public void SettingsShouldBeCopiedCorrectlyOnNewSettings()
        {
            CustomODataMessageReaderSettings settings = new CustomODataMessageReaderSettings();
            var copyOfSettings = new CustomODataMessageReaderSettings(settings);
            // Compare original and settings created from copy constructor without setting values
            this.CompareMessageReaderSettings(settings, copyOfSettings);
        }

        [TestMethod]
        public void ODataMessageReaderCopyConstructorTest()
        {
            CustomODataMessageReaderSettings settings = new CustomODataMessageReaderSettings();
            settings.CheckCharacters = true;
            settings.EnableAtomMetadataReading = true;
            settings.MessageQuotas.MaxPartsPerBatch = 100;
            settings.MessageQuotas.MaxOperationsPerChangeset = 200;
            settings.MessageQuotas.MaxNestingDepth = 20;
            settings.MessageQuotas.MaxReceivedMessageSize = 2000;
            settings.MessageQuotas.MaxEntityPropertyMappingsPerType = 30;
            settings.InstanceAnnotationFilter = (name) => name != null ? true : false;
            CustomODataMessageReaderSettings copyOfSettings = new CustomODataMessageReaderSettings(settings);
            this.CompareMessageReaderSettings(settings, copyOfSettings);
        }

        private void CompareMessageReaderSettings(ODataMessageReaderSettingsBase expected, ODataMessageReaderSettingsBase actual)
        {
            if (expected == null && actual == null)
            {
                return;
            }

            Assert.IsNotNull(expected, "expected settings cannot be null");
            Assert.IsNotNull(actual, "actual settings cannot be null");
            Assert.AreEqual(expected.CheckCharacters, actual.CheckCharacters, "CheckCharacters does not match");
            Assert.AreEqual(expected.EnableAtomMetadataReading, actual.EnableAtomMetadataReading, "EnableAtomMetadataReading does not match");
            Assert.AreEqual(expected.MessageQuotas.MaxPartsPerBatch, actual.MessageQuotas.MaxPartsPerBatch, "MaxPartsPerBatch does not match");
            Assert.AreEqual(expected.MessageQuotas.MaxOperationsPerChangeset, actual.MessageQuotas.MaxOperationsPerChangeset, "MaxOperationsPerChangeset does not match");
            Assert.AreEqual(expected.MessageQuotas.MaxNestingDepth, actual.MessageQuotas.MaxNestingDepth, "MaxNestingDepth does not match");
            Assert.AreEqual(expected.MessageQuotas.MaxReceivedMessageSize, actual.MessageQuotas.MaxReceivedMessageSize, "MaxMessageSize does not match");
            Assert.AreEqual(expected.MessageQuotas.MaxEntityPropertyMappingsPerType, actual.MessageQuotas.MaxEntityPropertyMappingsPerType, "MaxEntityPropertyMappingsPerType does not match");
            Assert.AreEqual(expected.InstanceAnnotationFilter, actual.InstanceAnnotationFilter, "InstanceAnnotationFilter does not match");
        }

        public class CustomODataMessageReaderSettings : ODataMessageReaderSettingsBase
        {
            public CustomODataMessageReaderSettings()
                : base()
            {
            }

            public CustomODataMessageReaderSettings(CustomODataMessageReaderSettings other)
                : base(other)
            {
            }
        }
    }
}

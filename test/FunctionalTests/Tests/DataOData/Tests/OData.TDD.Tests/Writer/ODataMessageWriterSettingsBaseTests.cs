//---------------------------------------------------------------------
// <copyright file="ODataMessageWriterSettingsBaseTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Writer
{
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataMessageWriterSettingsBaseTests
    {
        [TestMethod]
        public void SettingsShouldbeCopied()
        {
            CustomODataMessageWriterSettings settings = new CustomODataMessageWriterSettings();
            settings.CheckCharacters = true;
            settings.Indent = true;
            settings.MessageQuotas.MaxReceivedMessageSize = 20;

            CustomODataMessageWriterSettings otherSettings = new CustomODataMessageWriterSettings(settings);
            otherSettings.CheckCharacters.Should().BeTrue();
            otherSettings.Indent.Should().BeTrue();
            settings.MessageQuotas.MaxReceivedMessageSize.Should().Be(20);
        }

        [TestMethod]
        public void DefaultSettingsBaseTest()
        {
            CustomODataMessageWriterSettings settings = new CustomODataMessageWriterSettings();
            settings.CheckCharacters.Should().BeFalse();
            settings.Indent.Should().BeFalse();
        }

        public class CustomODataMessageWriterSettings : ODataMessageWriterSettingsBase
        {
            public CustomODataMessageWriterSettings()
                : base()
            {
            }

            public CustomODataMessageWriterSettings(CustomODataMessageWriterSettings other)
                : base(other)
            {
            }
        }
    }
}
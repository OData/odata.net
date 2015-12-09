//---------------------------------------------------------------------
// <copyright file="ODataMessageWriterSettingsBaseTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataMessageWriterSettingsBaseTests
    {
        [Fact]
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

        [Fact]
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
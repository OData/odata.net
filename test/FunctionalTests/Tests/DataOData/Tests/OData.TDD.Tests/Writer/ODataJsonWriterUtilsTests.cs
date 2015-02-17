//---------------------------------------------------------------------
// <copyright file="ODataJsonWriterUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Writer
{
    using System;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Json;
    using Microsoft.OData.Core.JsonLight;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit and short-span integration tests for the ODataJsonWriterUtils.
    /// TODO: Create an interface and make a JsonWriter simulator so we don't have to double test its writing functionality.
    //  TODO: Write unit tests the remaining methods on ODataJsonWriterUtils.
    /// </summary>
    [TestClass]
    public class ODataJsonWriterUtilsTests
    {
        private StringWriter stringWriter;
        private IJsonWriter jsonWriter;
        private ODataMessageWriterSettings settings;

        [TestInitialize]
        public void Initialize()
        {
            this.stringWriter = new StringWriter();
            this.jsonWriter = new JsonWriter(this.stringWriter, /*indent*/ false, ODataFormat.Json, isIeee754Compatible: true);
            this.settings = new ODataMessageWriterSettings();
        }

        [TestMethod]
        public void StartJsonPaddingIfRequiredWillDoNothingIfNullFunctionName()
        {
            settings.JsonPCallback = null;
            ODataJsonWriterUtils.StartJsonPaddingIfRequired(this.jsonWriter, settings);
            stringWriter.GetStringBuilder().ToString().Should().BeEmpty();
        }

        [TestMethod]
        public void StartJsonPaddingIfRequiredWillDoNothingIfEmptyFunctionName()
        {
            settings.JsonPCallback = "";
            ODataJsonWriterUtils.StartJsonPaddingIfRequired(this.jsonWriter, settings);
            stringWriter.GetStringBuilder().ToString().Should().BeEmpty();
        }

        [TestMethod]
        public void EndJsonPaddingIfRequiredWillDoNothingIfNullFunctionName()
        {
            settings.JsonPCallback = null;
            ODataJsonWriterUtils.EndJsonPaddingIfRequired(this.jsonWriter, settings);
            stringWriter.GetStringBuilder().ToString().Should().BeEmpty();
        }

        [TestMethod]
        public void EndJsonPaddingIfRequiredWillDoNothingIfEmptyFunctionName()
        {
            settings.JsonPCallback = "";
            ODataJsonWriterUtils.EndJsonPaddingIfRequired(this.jsonWriter, settings);
            stringWriter.GetStringBuilder().ToString().Should().BeEmpty();
        }

        [TestMethod]
        public void StartAndEndJsonPaddingSuccessTest()
        {
            settings.JsonPCallback = "functionName";
            ODataJsonWriterUtils.StartJsonPaddingIfRequired(this.jsonWriter, settings);
            ODataJsonWriterUtils.EndJsonPaddingIfRequired(this.jsonWriter, settings);
            stringWriter.GetStringBuilder().ToString().Should().Be("functionName()");
        }
    }
}

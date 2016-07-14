//---------------------------------------------------------------------
// <copyright file="ODataVCardScenarioTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace Microsoft.Test.OData.PluggableFormat.VCard.Test
{
    using System;
    using System.IO;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.DependencyInjection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataVCardScenarioTests
    {
        private const string ResModel = "Microsoft.Test.OData.PluggableFormat.Tests.VCard.Test.VCardModel.xml";
        private const string ResSimpleSampleVcf = "Microsoft.Test.OData.PluggableFormat.Tests.VCard.Test.SimpleSample.vcf";
        private const string ResSimpleSampleJson = "Microsoft.Test.OData.PluggableFormat.Tests.VCard.Test.SimpleSample.json";

        private static readonly IEdmModel VCardModel;
        private static readonly IEdmComplexType VCardType;

        private static IServiceProvider container;

        static ODataVCardScenarioTests()
        {
            VCardModel = TestHelper.GetModel(ResModel);
            VCardType = (IEdmComplexType)VCardModel.FindType("VCard21.VCard");

            container = ContainerBuilderHelper.BuildContainer(builder =>
                builder.AddService<ODataMediaTypeResolver, VCardMediaTypeResolver>(ServiceLifetime.Singleton));
        }

        [TestMethod]
        public void TestReadSimpleSampleVCard()
        {
            VCardBaseLineTest(ResSimpleSampleVcf, ResSimpleSampleJson);
        }

        private void VCardBaseLineTest(string resVcf, string resJson)
        {
            foreach (var async in new[] { false, true })
            {
                var valueFromVcf = GetTopLevelProperty(resVcf, "text/x-vCard", null, async);
                TestBaseLine(valueFromVcf, resVcf, resJson);
                var valueFromJson = GetTopLevelProperty(resJson, "application/json", VCardModel, async);
                TestBaseLine(valueFromJson, resVcf, resJson);
            }
        }

        private ODataResource GetTopLevelProperty(string res, string contentType, IEdmModel model = null, bool async = false)
        {
            Stream stream = null;

            // Read vcf
            try
            {
                stream = TestHelper.GetResourceStream(res);
                ODataResource val = null;

                using (var reader = TestHelper.CreateMessageReader(stream, container, contentType, model))
                {
                    stream = null;
                    if (async)
                    {
                        var task = reader.CreateODataResourceReaderAsync();
                        task.Wait();
                        var odataReader = task.Result;
                        while (true)
                        {
                            var readTask = odataReader.ReadAsync();
                            readTask.Wait();
                            if (!readTask.Result)
                            {
                                break;
                            }

                            if (odataReader.State == ODataReaderState.ResourceEnd)
                            {
                                val = odataReader.Item as ODataResource;
                                break;
                            }
                        }
                    }
                    else
                    {
                        var task = reader.CreateODataResourceReaderAsync();
                        task.Wait();
                        var odataReader = task.Result;
                        while (true)
                        {
                            var readTask = odataReader.ReadAsync();
                            readTask.Wait();
                            if (!readTask.Result)
                            {
                                break;
                            }

                            if (odataReader.State == ODataReaderState.ResourceEnd)
                            {
                                val = odataReader.Item as ODataResource;
                                break;
                            }
                        }
                    }
                }

                Assert.IsNotNull(val);
                return val;
            }
            finally
            {
                if (stream != null) stream.Dispose();
            }
        }

        private void TestBaseLine(ODataResource val, string resVcf, string resJson)
        {
            // Write json, compare with baseline
            Assert.AreEqual(
                Regex.Replace(TestHelper.GetResourceString(resJson), @"\r\n\s*([""{}\]])", "$1"),
                TestHelper.GetToplevelPropertyPayloadString(val, container),
                "Json baseline");

            // Write vcf, compare with baseline
            Assert.AreEqual(
                TestHelper.GetResourceString(resVcf),
                TestHelper.GetToplevelPropertyPayloadString(val, container, "text/x-vCard"),
                "Vcf baseline");
        }
    }
}

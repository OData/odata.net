//---------------------------------------------------------------------
// <copyright file="EncodingAtomTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests to ensure we use the proper encoding when reading ATOM payloads.
    /// </summary>
    [TestClass, TestCase]
    public class EncodingAtomTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        // The encodings used in this test don't exist on Silverlight or the phone.
        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Tests use of proper encoding when reading top-level properties.")]
        public void EncodingPropertyTest()
        {
            // NOTE: the XmlReader (when created via XmlReader.Create) will use the Xml declaration and/or scan
            //       ahead to detect the encoding from the payload. Only if both fail, do we have the opportunity
            //       to make a difference by specifying the encoding in the content type.
            //
            // Create an non-standard character in the iso-8859-9 (Turkish) encoding
            Encoding iso88599Encoding = Encoding.GetEncoding("iso-8859-9");
            char[] chars = iso88599Encoding.GetChars(new byte[] { 250 });
            string payloadValue = new string(chars);

            var testCases = new[]
            {
                new
                {
                    ContentType = "application/xml",
                    ResultString = payloadValue,
                    ExpectedException = new ExpectedException(typeof(XmlException)),
                },
                new
                {
                    ContentType = "application/xml;charset=iso-8859-9",
                    ResultString = payloadValue,
                    ExpectedException = (ExpectedException)null,
                },
            };

            var testDescriptors = testCases.Select(testCase =>
                new ReaderEncodingTestDescriptor(this.Settings, testCase.ContentType)
                {
                    PayloadElement = PayloadBuilder.PrimitiveProperty(null, payloadValue)
                        .SerializationEncoding("iso-8859-9", /*omitDeclaration*/ true),
                    ExpectedResultPayloadElement = tc => PayloadBuilder.PrimitiveProperty(null, testCase.ResultString),
                    ExpectedException = testCase.ExpectedException,
                }
            );

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = new ReaderTestConfiguration(testConfiguration);
                    testConfiguration.MessageReaderSettings.DisableMessageStreamDisposal = true;

                    testDescriptor.RunTest(testConfiguration);
                });
        }
      
        private sealed class ReaderEncodingTestDescriptor : PayloadReaderTestDescriptor
        {
            private readonly string contentType;

            internal ReaderEncodingTestDescriptor(PayloadReaderTestDescriptor.Settings settings, string contentType)
                : base(settings)
            {
                this.contentType = contentType;
            }

            protected override TestMessage CreateInputMessage(ReaderTestConfiguration testConfiguration)
            {
                TestMessage testMessage =  base.CreateInputMessage(testConfiguration);
                testMessage.SetHeader(ODataConstants.ContentTypeHeader, contentType);
                return testMessage;
            }
        }
    }
}

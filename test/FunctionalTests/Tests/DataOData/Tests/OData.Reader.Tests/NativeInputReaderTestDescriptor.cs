//---------------------------------------------------------------------
// <copyright file="NativeInputReaderTestDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System;
    using System.IO;
    using System.Text;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Reader test descriptor for tests which need to specify the input as exact ATOM or JSON payload.
    /// </summary>
    public class NativeInputReaderTestDescriptor : ReaderTestDescriptor
    {
        /// <summary>
        /// The input for ATOM payloads.
        /// </summary>
        private XElement atomInput;

        /// <summary>
        /// The input for JSON payloads.
        /// </summary>
        private JsonValue jsonInput;

        /// <summary>
        /// The metadata in the form of entity model for the payload.
        /// Can be null in which case the payload will execute without metadata.
        /// </summary>
        public IEdmModel PayloadEdmModel
        {
            get
            {
                return this.Model;
            }
            set
            {
                this.Model = value;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NativeInputReaderTestDescriptor()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The other test descriptor to copy values from.</param>
        public NativeInputReaderTestDescriptor(NativeInputReaderTestDescriptor other)
            : base(other)
        {
            this.AtomInput = other.AtomInput;
            this.AtomInputXml = other.AtomInputXml;
            this.JsonInputText = other.JsonInputText;
            this.JsonInput = other.JsonInput;
            this.DefaultInput = other.DefaultInput;
            this.InputCreator = other.InputCreator;
        }

        /// <summary>
        /// The input for ATOM payloads as text XML.
        /// </summary>
        public string AtomInputXml { get; set; }

        /// <summary>
        /// The input for ATOM payloads. This overrides AtomInputXml if both are specified.
        /// </summary>
        public XElement AtomInput
        {
            get
            {
                if (this.atomInput == null && this.AtomInputXml != null)
                {
                    return XElement.Parse(this.AtomInputXml);
                }
                else
                {
                    return this.atomInput;
                }
            }

            set
            {
                this.atomInput = value;
            }
        }

        /// <summary>
        /// The input for JSON payloads as text.
        /// </summary>
        public string JsonInputText { get; set; }

        /// <summary>
        /// The input for JSON payloads. This overrides JsonInputText if both are specified.
        /// </summary>
        public JsonValue JsonInput
        {
            get
            {
                if (this.jsonInput == null && this.JsonInputText != null)
                {
                    return JsonTextPreservingParser.ParseValue(new StringReader(this.JsonInputText));
                }
                else
                {
                    return this.jsonInput;
                }
            }

            set
            {
                this.jsonInput = value;
            }
        }

        /// <summary>
        /// The input for default payloads.
        /// </summary>
        public string DefaultInput { get; set; }

        /// <summary>
        /// Func which if specified overrides all of the above inputs.
        /// It's called to get the input for the test.
        /// </summary>
        public Func<ReaderTestConfiguration, string> InputCreator { get; set; }

        /// <summary>
        /// Returns description of the test case.
        /// </summary>
        /// <returns>Humanly readable description of the test. Used for debugging.</returns>
        public override string ToString()
        {
            string result = base.ToString();
            XElement atomInput = this.AtomInput;
            if (atomInput != null)
            {
                result += "\r\nATOM: " + atomInput.ToString();
            }

            JsonValue jsonInput = this.JsonInput;
            if (jsonInput != null)
            {
                result += "\r\nJSON: " + jsonInput.ToString();
            }

            if (this.DefaultInput != null)
            {
                result += "\r\nDefault: " + this.DefaultInput;
            }

            return result;
        }

        /// <summary>
        /// Called to create the input message for the reader test.
        /// </summary>
        /// <param name="testConfiguration">The test configuration.</param>
        /// <returns>The newly created test message to use.</returns>
        protected override TestMessage CreateInputMessage(ReaderTestConfiguration testConfiguration)
        {
            MemoryStream memoryStream = new MemoryStream();
            this.WriteInputMessageContent(testConfiguration, new TestStream(memoryStream, ignoreDispose: true));
            memoryStream.Seek(0, SeekOrigin.Begin);
            TestStream messageStream = new TestStream(memoryStream);
            TestMessage testMessage = TestReaderUtils.CreateInputMessageFromStream(
                messageStream,
                testConfiguration,
                this.PayloadKind,
                /*customContentTypeHeader*/ null,
                /*urlResolver*/ null);
            return testMessage;
        }

        /// <summary>
        /// If overriden dumps the content of an input message which would be created for the specified test configuration
        /// into a string and returns it. This is used only for debugging purposes.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The string content of the input message.</returns>
        protected override string DumpInputMessageContent(ReaderTestConfiguration testConfiguration)
        {
            MemoryStream stream = new MemoryStream();
            TestStream testStream = new TestStream(stream, true /*ignoreDispose*/);
            this.WriteInputMessageContent(testConfiguration, testStream);
            return Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
        }

        /// <summary>
        /// Writes the content of the input message into a stream.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <param name="stream">The stream to write to.</param>
        private void WriteInputMessageContent(ReaderTestConfiguration testConfiguration, Stream stream)
        {
            if (this.InputCreator != null)
            {
                // Note that by default the StreamWriter uses UTF8 encoding which is exactly what we want.
                using (StreamWriter textWriter = new StreamWriter(stream))
                {
                    textWriter.Write(this.InputCreator(testConfiguration));
                }
            }
            else
            {
                if (testConfiguration.Format == null)
                {
                    ExceptionUtilities.CheckObjectNotNull(this.DefaultInput, "The test tries to use Default format, thus the DefaultInput must be specified.");

                    // Note that by default the StreamWriter uses UTF8 encoding which is exactly what we want.
                    using (StreamWriter textWriter = new StreamWriter(stream))
                    {
                        textWriter.Write(this.DefaultInput);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a copy of this NativeInputReaderTestDescriptor
        /// </summary>
        public override object Clone()
        {
            return new NativeInputReaderTestDescriptor(this);
        }
    }
}

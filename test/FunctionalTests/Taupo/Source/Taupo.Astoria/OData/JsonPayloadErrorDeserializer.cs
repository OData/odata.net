//---------------------------------------------------------------------
// <copyright file="JsonPayloadErrorDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Json;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Deserializer for OData payloads for the atom/xml wire format
    /// </summary>
    public class JsonPayloadErrorDeserializer : IPayloadErrorDeserializer
    {
        /// <summary>
        /// Gets or sets the assertion handler to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public AssertionHandler Assert { get; set; }

        /// <summary>
        /// Deserializes given HTTP payload as a json error payload or returns false
        /// </summary>
        /// <param name="serialized">The payload that was sent over HTTP</param>
        /// <param name="encodingName">Optional name of an encoding to use if it is relevant to the current format. May be null if no character-set information was known.</param>
        /// <param name="errorPayload">Error payload that is found</param>
        /// <returns>True if it finds and error, false if not</returns>
        public bool TryDeserializeErrorPayload(byte[] serialized, string encodingName, out ODataPayloadElement errorPayload)
        {
            ExceptionUtilities.CheckArgumentNotNull(serialized, "serialized");
            errorPayload = null;

            Encoding encoding = HttpUtilities.GetEncodingOrDefault(encodingName);
            string payload = encoding.GetString(serialized, 0, serialized.Length);
            
            // sample json error
            // {
            //   “error”: {
            //     “code”: <string>
            //     “message”: {
            //       “lang”: <string>,
            //       “value“: <string>
            //     },
            //     “innererror”: {
            //       “message”: <string>,
            //       “type”: <string>,
            //       “stacktrace”: <string>,
            //       “internalexception”: {
            //         … another inner error structure …
            //       }
            //     }
            //   }
            // }
            string errorName = "\"" + ODataConstants.ErrorElementName + "\":";
            int errorElementPos = payload.IndexOf(errorName, StringComparison.Ordinal);

            // Look in the payload and see if an error exists
            if (errorElementPos > -1)
            {
                ODataErrorPayload topLevelError = new ODataErrorPayload();
                errorPayload = topLevelError;
                ODataInternalExceptionPayload innerError = null;
                bool inTopLevelMessage = false;
                bool inError = false;

                using (StringReader reader = new StringReader(payload.Substring(errorElementPos)))
                {
                    JsonTokenizer tokenizer = new JsonTokenizer(reader);
                    
                    while (tokenizer.HasMoreTokens())
                    {
                        if (tokenizer.TokenType == JsonTokenType.String)
                        {
                            string tokenValue = null;
                            if (tokenizer != null)
                            {
                                tokenValue = tokenizer.Value.ToString();
                            }

                            // don't start processing other properties until we're definitely inside the error itself
                            if (!inError)
                            {
                                inError = tokenValue == ODataConstants.ErrorElementName;
                            }
                            else
                            {
                                if (tokenValue == ODataConstants.InnerErrorElementName)
                                {
                                    this.Assert.IsNull(innerError, "Should not find 'innererror' property on inner errors");
                                    topLevelError.InnerError = innerError = new ODataInternalExceptionPayload();
                                }
                                else if (tokenValue == ODataConstants.InternalExceptionElementName)
                                {
                                    this.Assert.IsNotNull(innerError, "Should not find 'internalexception' property on top-level errors");
                                    ExceptionUtilities.Assert(innerError.InternalException == null, "Inner error should have been null");
                                    innerError.InternalException = new ODataInternalExceptionPayload();
                                    innerError = innerError.InternalException;
                                }
                                else if (tokenValue == ODataConstants.MessageElementName)
                                {
                                    if (innerError == null)
                                    {
                                        inTopLevelMessage = true;
                                    }
                                    else
                                    {
                                        innerError.Message = this.GetIdentifierValue(tokenizer);
                                    }
                                }
                                else if (tokenValue == ODataConstants.ValueElementName)
                                {
                                    this.Assert.IsTrue(inTopLevelMessage, "Should not find 'value' property outside top-level message");
                                    this.Assert.IsNull(innerError, "Should not find 'value' property on inner errors");
                                    topLevelError.Message = this.GetIdentifierValue(tokenizer);
                                }
                                else if (tokenValue == ODataConstants.CodeElementName)
                                {
                                    this.Assert.IsNull(innerError, "Should not find 'code' property on inner errors");
                                    topLevelError.Code = this.GetIdentifierValue(tokenizer);
                                }
                                else if (tokenValue == ODataConstants.TypeNameElementName)
                                {
                                    this.Assert.IsNotNull(innerError, "Should not find 'type' on top-level errors");
                                    innerError.TypeName = this.GetIdentifierValue(tokenizer);
                                }
                                else if (tokenValue == ODataConstants.StackTraceElementName)
                                {
                                    this.Assert.IsNotNull(innerError, "Should not find 'stacktrace' on top-level errors");
                                    innerError.StackTrace = this.GetIdentifierValue(tokenizer);
                                }
                            }
                        }
                        else if (tokenizer.TokenType == JsonTokenType.RightCurly)
                        {
                            inTopLevelMessage = false;
                        }

                        tokenizer.GetNextToken();
                    }
                }

                return true;
            }

            return false;
        }

        private string GetIdentifierValue(JsonTokenizer tokenizer)
        {
            tokenizer.GetNextToken();
            ExceptionUtilities.Assert(tokenizer.TokenType == JsonTokenType.Colon, "Error parsing JSON Error: Expected colon token next");

            tokenizer.GetNextToken();
            ExceptionUtilities.Assert(tokenizer.TokenType == JsonTokenType.String, "Error parsing JSON Error: Expected string token next");

            ExceptionUtilities.CheckObjectNotNull(tokenizer.Value, "Expected tokenizer to have a value and not be null");
            return tokenizer.Value.ToString();
        }
    }
}

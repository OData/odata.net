//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Json
{
    #region Namespaces
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// OData JSON deserializer for errors.
    /// </summary>
    internal sealed class ODataJsonErrorDeserializer : ODataJsonDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonInputContext">The JSON input context to read from.</param>
        internal ODataJsonErrorDeserializer(ODataJsonInputContext jsonInputContext)
            : base(jsonInputContext)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Read a top-level error.
        /// </summary>
        /// <returns>An <see cref="ODataError"/> representing the read error.</returns>
        internal ODataError ReadTopLevelError()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            Debug.Assert(!this.JsonReader.DisableInStreamErrorDetection, "!JsonReader.DisableInStreamErrorDetection");
            this.JsonReader.AssertNotBuffering();

            // prevent the buffering JSON reader from detecting in-stream errors - we read the error ourselves
            // to throw proper exceptions
            this.JsonReader.DisableInStreamErrorDetection = true;

            ODataError error = new ODataError();

            try
            {
                // Read the start of the payload (no "d" wrapper for top-level errors)
                this.ReadPayloadStart(/*isReadingNestedPayload*/ false, /*expectResponseWrapper*/ false);

                // Read the start of the error object
                this.JsonReader.ReadStartObject();

                ODataJsonReaderUtils.ErrorPropertyBitMask propertiesFoundBitField = ODataJsonReaderUtils.ErrorPropertyBitMask.None;
                while (this.JsonReader.NodeType == JsonNodeType.Property)
                {
                    string propertyName = this.JsonReader.ReadPropertyName();
                    if (string.CompareOrdinal(JsonConstants.ODataErrorName, propertyName) != 0)
                    {
                        // we only allow a single 'error' property for a top-level error object
                        throw new ODataException(Strings.ODataJsonErrorDeserializer_TopLevelErrorWithInvalidProperty(propertyName));
                    }

                    ODataJsonReaderUtils.VerifyErrorPropertyNotFound(ref propertiesFoundBitField, ODataJsonReaderUtils.ErrorPropertyBitMask.Error, JsonConstants.ODataErrorName);

                    this.JsonReader.ReadStartObject();

                    while (this.JsonReader.NodeType == JsonNodeType.Property)
                    {
                        propertyName = this.JsonReader.ReadPropertyName();
                        switch (propertyName)
                        {
                            case JsonConstants.ODataErrorCodeName:
                                ODataJsonReaderUtils.VerifyErrorPropertyNotFound(ref propertiesFoundBitField, ODataJsonReaderUtils.ErrorPropertyBitMask.Code, JsonConstants.ODataErrorCodeName);
                                error.ErrorCode = this.JsonReader.ReadStringValue(JsonConstants.ODataErrorCodeName);
                                break;

                            case JsonConstants.ODataErrorMessageName:
                                ODataJsonReaderUtils.VerifyErrorPropertyNotFound(ref propertiesFoundBitField, ODataJsonReaderUtils.ErrorPropertyBitMask.Message, JsonConstants.ODataErrorMessageName);
                                this.JsonReader.ReadStartObject();

                                while (this.JsonReader.NodeType == JsonNodeType.Property)
                                {
                                    propertyName = this.JsonReader.ReadPropertyName();
                                    switch (propertyName)
                                    {
                                        case JsonConstants.ODataErrorMessageLanguageName:
                                            ODataJsonReaderUtils.VerifyErrorPropertyNotFound(ref propertiesFoundBitField, ODataJsonReaderUtils.ErrorPropertyBitMask.MessageLanguage, JsonConstants.ODataErrorMessageLanguageName);
                                            error.MessageLanguage = this.JsonReader.ReadStringValue(JsonConstants.ODataErrorMessageLanguageName);
                                            break;

                                        case JsonConstants.ODataErrorMessageValueName:
                                            ODataJsonReaderUtils.VerifyErrorPropertyNotFound(ref propertiesFoundBitField, ODataJsonReaderUtils.ErrorPropertyBitMask.MessageValue, JsonConstants.ODataErrorMessageValueName);
                                            error.Message = this.JsonReader.ReadStringValue(JsonConstants.ODataErrorMessageValueName);
                                            break;

                                        default:
                                            // we only allow a 'lang' and 'value' properties in the value of the 'message' property
                                            throw new ODataException(Strings.ODataJsonErrorDeserializer_TopLevelErrorMessageValueWithInvalidProperty(propertyName));
                                    }
                                }

                                this.JsonReader.ReadEndObject();
                                break;

                            case JsonConstants.ODataErrorInnerErrorName:
                                ODataJsonReaderUtils.VerifyErrorPropertyNotFound(ref propertiesFoundBitField, ODataJsonReaderUtils.ErrorPropertyBitMask.InnerError, JsonConstants.ODataErrorInnerErrorName);
                                error.InnerError = this.ReadInnerError(0 /* recursionDepth */);
                                break;

                            default:
                                // we only allow a 'code', 'message' and 'innererror' properties in the value of the 'error' property
                                throw new ODataException(Strings.ODataJsonErrorDeserializer_TopLevelErrorValueWithInvalidProperty(propertyName));
                        }
                    }

                    this.JsonReader.ReadEndObject();
                }

                // Read the end of the error object
                this.JsonReader.ReadEndObject();

                // Read the end of the response (no "d" wrapper for top-level errors)
                this.ReadPayloadEnd(/*isReadingNestedPayload*/ false, /*expectResponseWrapper*/ false);
            }
            finally
            {
                this.JsonReader.DisableInStreamErrorDetection = false;
            }

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            return error;
        }

        /// <summary>
        /// Reads an inner error payload.
        /// </summary>
        /// <param name="recursionDepth">The number of times this method has been called recursively.</param>
        /// <returns>An <see cref="ODataInnerError"/> representing the read inner error.</returns>
        /// <remarks>
        /// Pre-Condition:  any                         - will throw if not StartObject
        /// Post-Condition: JsonNodeType.Property       - The next property in the error value
        ///                 JsonNodeType.EndObject      - The end of the error value
        /// </remarks>
        private ODataInnerError ReadInnerError(int recursionDepth)
        {
            Debug.Assert(this.JsonReader.DisableInStreamErrorDetection, "JsonReader.DisableInStreamErrorDetection");
            this.JsonReader.AssertNotBuffering();

            ValidationUtils.IncreaseAndValidateRecursionDepth(ref recursionDepth, this.MessageReaderSettings.MessageQuotas.MaxNestingDepth);

            this.JsonReader.ReadStartObject();

            ODataInnerError innerError = new ODataInnerError();

            ODataJsonReaderUtils.ErrorPropertyBitMask propertiesFoundBitField = ODataJsonReaderUtils.ErrorPropertyBitMask.None;
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.ReadPropertyName();
                switch (propertyName)
                {
                    case JsonConstants.ODataErrorInnerErrorMessageName:
                        ODataJsonReaderUtils.VerifyErrorPropertyNotFound(
                            ref propertiesFoundBitField,
                            ODataJsonReaderUtils.ErrorPropertyBitMask.MessageValue,
                            JsonConstants.ODataErrorInnerErrorMessageName);
                        innerError.Message = this.JsonReader.ReadStringValue(JsonConstants.ODataErrorInnerErrorMessageName);
                        break;

                    case JsonConstants.ODataErrorInnerErrorTypeNameName:
                        ODataJsonReaderUtils.VerifyErrorPropertyNotFound(
                            ref propertiesFoundBitField,
                            ODataJsonReaderUtils.ErrorPropertyBitMask.TypeName,
                            JsonConstants.ODataErrorInnerErrorTypeNameName);
                        innerError.TypeName = this.JsonReader.ReadStringValue(JsonConstants.ODataErrorInnerErrorTypeNameName);
                        break;

                    case JsonConstants.ODataErrorInnerErrorStackTraceName:
                        ODataJsonReaderUtils.VerifyErrorPropertyNotFound(
                            ref propertiesFoundBitField,
                            ODataJsonReaderUtils.ErrorPropertyBitMask.StackTrace,
                            JsonConstants.ODataErrorInnerErrorStackTraceName);
                        innerError.StackTrace = this.JsonReader.ReadStringValue(JsonConstants.ODataErrorInnerErrorStackTraceName);
                        break;

                    case JsonConstants.ODataErrorInnerErrorInnerErrorName:
                        ODataJsonReaderUtils.VerifyErrorPropertyNotFound(
                            ref propertiesFoundBitField,
                            ODataJsonReaderUtils.ErrorPropertyBitMask.InnerError,
                            JsonConstants.ODataErrorInnerErrorInnerErrorName);
                        innerError.InnerError = this.ReadInnerError(recursionDepth);
                        break;

                    default:
                        // skip any unsupported properties in the inner error
                        this.JsonReader.SkipValue();
                        break;
                }
            }

            this.JsonReader.ReadEndObject();
            this.JsonReader.AssertNotBuffering();
            return innerError;
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="ODataJsonPayloadKindDetectionDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    using System;
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    #endregion Namespaces

    /// <summary>
    /// OData Json deserializer for detecting the payload kind of a Json payload.
    /// </summary>
    internal sealed class ODataJsonPayloadKindDetectionDeserializer : ODataJsonPropertyAndValueDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonInputContext">The Json input context to read from.</param>
        internal ODataJsonPayloadKindDetectionDeserializer(ODataJsonInputContext jsonInputContext)
            : base(jsonInputContext)
        {
        }

        /// <summary>
        /// Detects the payload kind(s).
        /// </summary>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>An enumerable of zero, one or more payload kinds that were detected from looking at the payload in the message stream.</returns>
        internal IEnumerable<ODataPayloadKind> DetectPayloadKind(ODataPayloadKindDetectionInfo detectionInfo)
        {
            Debug.Assert(detectionInfo != null, "detectionInfo != null");
            Debug.Assert(this.ReadingResponse, "Payload kind detection is only supported in responses.");

            // prevent the buffering JSON reader from detecting in-stream errors - we read the error ourselves
            this.JsonReader.DisableInStreamErrorDetection = true;

            try
            {
                this.ReadPayloadStart(
                    ODataPayloadKind.Unsupported,
                    /*propertyAndAnnotationCollector*/null,
                    /*isReadingNestedPayload*/false,
                    /*allowEmptyPayload*/false);
                return this.DetectPayloadKindImplementation(detectionInfo);
            }
            catch (ODataException)
            {
                // If we are not able to read the payload in the expected JSON format/structure
                // return no detected payload kind below.
                return Enumerable.Empty<ODataPayloadKind>();
            }
            finally
            {
                this.JsonReader.DisableInStreamErrorDetection = false;
            }
        }

        /// <summary>
        /// Detects the payload kind(s).
        /// </summary>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>A task which returns an enumerable of zero, one or more payload kinds that were detected from looking at the payload in the message stream.</returns>
        internal async Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(ODataPayloadKindDetectionInfo detectionInfo)
        {
            Debug.Assert(detectionInfo != null, "detectionInfo != null");
            Debug.Assert(this.ReadingResponse, "Payload kind detection is only supported in responses.");

            // prevent the buffering JSON reader from detecting in-stream errors - we read the error ourselves
            this.JsonReader.DisableInStreamErrorDetection = true;

            try
            {

                await this.ReadPayloadStartAsync(
                    ODataPayloadKind.Unsupported,
                    propertyAndAnnotationCollector: null,
                    isReadingNestedPayload: false,
                    allowEmptyPayload: false).ConfigureAwait(false);

                return await this.DetectPayloadKindImplementationAsync(detectionInfo)
                    .ConfigureAwait(false);
            }
            catch (ODataException)
            {
                // If we are not able to read the payload in the expected JSON format/structure
                // return no detected payload kind below.
                return Enumerable.Empty<ODataPayloadKind>();
            }
            finally
            {
                this.JsonReader.DisableInStreamErrorDetection = false;
            }
        }

        /// <summary>
        /// Detects the payload kind(s).
        /// </summary>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>An enumerable of zero, one or more payload kinds that were detected from looking at the payload in the message stream.</returns>
        private IEnumerable<ODataPayloadKind> DetectPayloadKindImplementation(ODataPayloadKindDetectionInfo detectionInfo)
        {
            Debug.Assert(detectionInfo != null, "detectionInfo != null");
            Debug.Assert(this.JsonReader.DisableInStreamErrorDetection, "The in-stream error detection should be disabled for payload kind detection.");

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If we found a context URI and parsed it, look at the detected payload kind and return it.
            if (this.ContextUriParseResult != null)
            {
                return this.ContextUriParseResult.DetectedPayloadKinds;
            }

            // Otherwise this is a payload without context URI and we have to start sniffing; only error payloads
            // don't have a context URI so check for a single 'error' property (ignoring custom annotations).
            ODataError error = null;
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.ReadPropertyName();
                string annotatedPropertyName, annotationName;
                if (!ODataJsonDeserializer.TryParsePropertyAnnotation(propertyName, out annotatedPropertyName, out annotationName))
                {
                    if (ODataJsonReaderUtils.IsAnnotationProperty(propertyName))
                    {
                        if (propertyName != null && propertyName.StartsWith(ODataJsonConstants.ODataPropertyAnnotationSeparatorChar + ODataJsonConstants.ODataAnnotationNamespacePrefix, System.StringComparison.Ordinal))
                        {
                            // Any @odata.* instance annotations are not allowed for errors.
                            return Enumerable.Empty<ODataPayloadKind>();
                        }
                        else
                        {
                            // Skip custom instance annotations
                            this.JsonReader.SkipValue();
                        }
                    }
                    else
                    {
                        if (string.Equals(ODataJsonConstants.ODataErrorPropertyName, propertyName, StringComparison.Ordinal))
                        {
                            // If we find multiple errors or an invalid error value, this is not an error payload.
                            if (error != null || !this.JsonReader.StartBufferingAndTryToReadInStreamErrorPropertyValue(out error))
                            {
                                return Enumerable.Empty<ODataPayloadKind>();
                            }

                            // At this point we successfully read the first error property.
                            // Skip the error value and check whether there are more properties.
                            this.JsonReader.SkipValue();
                        }
                        else
                        {
                            // if it contains non-annotation property, it is not an error payload.
                            return Enumerable.Empty<ODataPayloadKind>();
                        }
                    }
                }
                else
                {
                    // Property annotation
                    return Enumerable.Empty<ODataPayloadKind>();
                }
            }

            // If we got here without finding a context URI or an error payload, we don't know what this is.
            if (error == null)
            {
                return Enumerable.Empty<ODataPayloadKind>();
            }

            return new ODataPayloadKind[] { ODataPayloadKind.Error };
        }

        /// <summary>
        /// Asynchronously detects the payload kind(s).
        /// </summary>
        /// <param name="detectionInfo">Additional information available for the payload kind detection.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains an enumerable of zero,
        /// one or more payload kinds that were detected from looking at the payload in the message stream.
        /// </returns>
        private async Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindImplementationAsync(ODataPayloadKindDetectionInfo detectionInfo)
        {
            Debug.Assert(detectionInfo != null, "detectionInfo != null");
            Debug.Assert(this.JsonReader.DisableInStreamErrorDetection, "The in-stream error detection should be disabled for payload kind detection.");

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            // If we found a context URI and parsed it, look at the detected payload kind and return it.
            if (this.ContextUriParseResult != null)
            {
                return this.ContextUriParseResult.DetectedPayloadKinds;
            }

            // Otherwise this is a payload without context URI and we have to start sniffing; only error payloads
            // don't have a context URI so check for a single 'error' property (ignoring custom annotations).
            ODataError error = null;
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = await this.JsonReader.ReadPropertyNameAsync()
                    .ConfigureAwait(false);
                if (!TryParsePropertyAnnotation(propertyName, out _, out _))
                {
                    if (ODataJsonReaderUtils.IsAnnotationProperty(propertyName))
                    {
                        if (propertyName != null && propertyName.StartsWith(ODataJsonConstants.ODataPropertyAnnotationSeparatorChar + ODataJsonConstants.ODataAnnotationNamespacePrefix, StringComparison.Ordinal))
                        {
                            // Any @odata.* instance annotations are not allowed for errors.
                            return Enumerable.Empty<ODataPayloadKind>();
                        }
                        else
                        {
                            // Skip custom instance annotations
                            await this.JsonReader.SkipValueAsync()
                                .ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        if (string.Equals(ODataJsonConstants.ODataErrorPropertyName, propertyName, StringComparison.Ordinal))
                        {
                            // If we find multiple errors or an invalid error value, this is not an error payload.
                            if (error != null)
                            {
                                return Enumerable.Empty<ODataPayloadKind>();
                            }
                            else
                            {
                                (bool IsReadSuccessfully, ODataError Error) readInStreamErrorPropertyResult = await this.JsonReader.StartBufferingAndTryToReadInStreamErrorPropertyValueAsync()
                                    .ConfigureAwait(false);
                                if (!readInStreamErrorPropertyResult.IsReadSuccessfully)
                                {
                                    return Enumerable.Empty<ODataPayloadKind>();
                                }

                                error = readInStreamErrorPropertyResult.Error;
                            }

                            // At this point we successfully read the first error property.
                            // Skip the error value and check whether there are more properties.
                            await this.JsonReader.SkipValueAsync()
                                .ConfigureAwait(false);
                        }
                        else
                        {
                            // if it contains non-annotation property, it is not an error payload.
                            return Enumerable.Empty<ODataPayloadKind>();
                        }
                    }
                }
                else
                {
                    // Property annotation
                    return Enumerable.Empty<ODataPayloadKind>();
                }
            }

            // If we got here without finding a context URI or an error payload, we don't know what this is.
            if (error == null)
            {
                return Enumerable.Empty<ODataPayloadKind>();
            }

            return new ODataPayloadKind[] { ODataPayloadKind.Error };
        }
    }
}

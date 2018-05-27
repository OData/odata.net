//---------------------------------------------------------------------
// <copyright file="ODataUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Utility methods used with the OData library.
    /// </summary>
    public static class ODataUtils
    {
        /// <summary>String representation of the version 4.0 of the OData protocol.</summary>
        private const string Version4NumberString = "4.0";

        /// <summary>String representation of the version 4.01 of the OData protocol.</summary>
        private const string Version401NumberString = "4.01";

        /// <summary>Sets the content-type and OData-Version headers on the message used by the message writer.</summary>
        /// <returns>The content-type and OData-Version headers on the message used by the message writer.</returns>
        /// <param name="messageWriter">The message writer to set the headers for.</param>
        /// <param name="payloadKind">The kind of payload to be written with the message writer.</param>
        /// <remarks>
        /// This method can be called if it is important to set all the message headers before calling any of the
        /// write methods on the <paramref name="messageWriter"/>.
        /// If it is sufficient to set the headers when the write methods on the <paramref name="messageWriter"/>
        /// are called, you don't have to call this method and setting the headers will happen automatically.
        /// </remarks>
        public static ODataFormat SetHeadersForPayload(ODataMessageWriter messageWriter, ODataPayloadKind payloadKind)
        {
            ExceptionUtils.CheckArgumentNotNull(messageWriter, "messageWriter");

            if (payloadKind == ODataPayloadKind.Unsupported)
            {
                throw new ArgumentException(Strings.ODataMessageWriter_CannotSetHeadersWithInvalidPayloadKind(payloadKind), "payloadKind");
            }

            return messageWriter.SetHeaders(payloadKind);
        }

        /// <summary>Returns the format used by the message reader for reading the payload.</summary>
        /// <returns>The format used by the messageReader for reading the payload.</returns>
        /// <param name="messageReader">The <see cref="T:Microsoft.OData.ODataMessageReader" /> to get the read format from.</param>
        /// <remarks>This method must only be called once reading has started.
        /// This means that a read method has been called on the <paramref name="messageReader"/> or that a reader (for entries, resource sets, collections, etc.) has been created.
        /// If the method is called prior to that it will throw.</remarks>
        public static ODataFormat GetReadFormat(ODataMessageReader messageReader)
        {
            ExceptionUtils.CheckArgumentNotNull(messageReader, "messageReader");
            return messageReader.GetFormat();
        }


        /// <summary>
        /// Gets the reader behavior for null property value on the specified property.
        /// </summary>
        /// <param name="model">The model containing the annotation.</param>
        /// <param name="property">The property to check.</param>
        /// <returns>The behavior to use when reading null value for this property.</returns>
        public static ODataNullValueBehaviorKind NullValueReadBehaviorKind(this IEdmModel model, IEdmProperty property)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(property, "property");

            ODataEdmPropertyAnnotation annotation = model.GetAnnotationValue<ODataEdmPropertyAnnotation>(property);
            return annotation == null ? ODataNullValueBehaviorKind.Default : annotation.NullValueReadBehaviorKind;
        }

        /// <summary>
        /// Adds a transient annotation to indicate how null values for the specified property should be read.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotations.</param>
        /// <param name="property">The <see cref="IEdmProperty"/> to modify.</param>
        /// <param name="nullValueReadBehaviorKind">The new behavior for reading null values for this property.</param>
        public static void SetNullValueReaderBehavior(this IEdmModel model, IEdmProperty property, ODataNullValueBehaviorKind nullValueReadBehaviorKind)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(property, "property");

            ODataEdmPropertyAnnotation annotation = model.GetAnnotationValue<ODataEdmPropertyAnnotation>(property);
            if (annotation == null)
            {
                if (nullValueReadBehaviorKind != ODataNullValueBehaviorKind.Default)
                {
                    annotation = new ODataEdmPropertyAnnotation
                    {
                        NullValueReadBehaviorKind = nullValueReadBehaviorKind
                    };
                    model.SetAnnotationValue(property, annotation);
                }
            }
            else
            {
                annotation.NullValueReadBehaviorKind = nullValueReadBehaviorKind;
            }
        }

        /// <summary>Displays the OData version to string representation.</summary>
        /// <returns>The OData version.</returns>
        /// <param name="version">The OData version.</param>
        public static string ODataVersionToString(ODataVersion version)
        {
            switch (version)
            {
                case ODataVersion.V4:
                    return Version4NumberString;

                case ODataVersion.V401:
                    return Version401NumberString;

                default:
                    // invalid enum value - unreachable.
                    throw new ODataException(Strings.ODataUtils_UnsupportedVersionNumber);
            }
        }

        /// <summary>Displays a string to OData version representation.</summary>
        /// <returns>The OData version.</returns>
        /// <param name="version">The OData version.</param>
        public static ODataVersion StringToODataVersion(string version)
        {
            // don't want to edit the string later.
            string modifiedVersion = version;

            // version must not be null or empty
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(version, "version");

            // removes the ";" and the user agent string from the version.
            int ix = modifiedVersion.IndexOf(';');
            if (ix >= 0)
            {
                modifiedVersion = modifiedVersion.Substring(0, ix);
            }

            switch (modifiedVersion.Trim())
            {
                case Version4NumberString:
                    return ODataVersion.V4;

                case Version401NumberString:
                    return ODataVersion.V401;

                default:
                    // invalid version string
                    throw new ODataException(Strings.ODataUtils_UnsupportedVersionHeader(version));
            }
        }

        /// <summary>
        /// Translates the <paramref name="annotationFilter"/> to a func that would evalutate whether the filter would match a given annotation name.
        /// The func would evaluate to true if the <paramref name="annotationFilter"/> matches the annotation name that's passed to the it, and false otherwise.
        /// </summary>
        /// <param name="annotationFilter">
        /// The filter string may be a comma delimited list of any of the following supported patterns:
        ///   "*"        -- Matches all annotation names.
        ///   "ns.*"     -- Matches all annotation names under the namespace "ns".
        ///   "ns.name"  -- Matches only the annotation name "ns.name".
        ///   "-"        -- The exclude operator may be used with any of the supported pattern, for example:
        ///                 "-ns.*"    -- Excludes all annotation names under the namespace "ns".
        ///                 "-ns.name" -- Excludes only the annotation name "ns.name".
        /// Null or empty filter is equivalent to "-*".
        ///
        /// The relative priority of the pattern is base on the relative specificity of the patterns being compared. If pattern1 is under the namespace pattern2,
        /// pattern1 is more specific than pattern2 because pattern1 matches a subset of what pattern2 matches. We give higher priority to the pattern that is more specific.
        /// For example:
        ///  "ns.*" has higher priority than "*"
        ///  "ns.name" has higher priority than "ns.*"
        ///  "ns1.name" has same priority as "ns2.*"
        ///
        /// Patterns with the exclude operator takes higher precedence than the same pattern without.
        /// For example: "-ns.name" has higher priority than "ns.name".
        ///
        /// Examples:
        ///   "ns1.*,ns.name"       -- Matches any annotation name under the "ns1" namespace and the "ns.name" annotation.
        ///   "*,-ns.*,ns.name"     -- Matches any annotation name outside of the "ns" namespace and only "ns.name" under the "ns" namespace.
        /// </param>
        /// <returns>Returns a func which would evaluate to true if the <paramref name="annotationFilter"/> matches the annotation name that's passed to the it,
        /// and false otherwise.</returns>
        public static Func<string, bool> CreateAnnotationFilter(string annotationFilter)
        {
            AnnotationFilter filter = AnnotationFilter.Create(annotationFilter);
            return filter.Matches;
        }

        /// <summary>
        /// Generate a default ODataServiceDocument instance from model.
        /// </summary>
        /// <param name="model">The Edm Model frm which to generate the service document.</param>
        /// <returns>The generated service document.</returns>
        public static ODataServiceDocument GenerateServiceDocument(this IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            if (model.EntityContainer == null)
            {
                throw new ODataException(Strings.ODataUtils_ModelDoesNotHaveContainer);
            }

            ODataServiceDocument serviceDocument = new ODataServiceDocument();
            serviceDocument.EntitySets = model.EntityContainer.EntitySets()
                .Select(entitySet => new ODataEntitySetInfo() { Name = entitySet.Name, Title = entitySet.Name, Url = new Uri(entitySet.Name, UriKind.RelativeOrAbsolute) }).ToList();
            serviceDocument.Singletons = model.EntityContainer.Singletons()
                .Select(singleton => new ODataSingletonInfo() { Name = singleton.Name, Title = singleton.Name, Url = new Uri(singleton.Name, UriKind.RelativeOrAbsolute) }).ToList();
            serviceDocument.FunctionImports = model.EntityContainer.OperationImports().OfType<IEdmFunctionImport>().Where(functionImport => functionImport.IncludeInServiceDocument && !functionImport.Function.Parameters.Any())
                .Select(functionImport => new ODataFunctionImportInfo() { Name = functionImport.Name, Title = functionImport.Name, Url = new Uri(functionImport.Name, UriKind.RelativeOrAbsolute) }).ToList();

            return serviceDocument;
        }

        /// <summary>
        /// Append default values required by OData to specified HTTP header.
        ///
        /// When header name is ODataConstants.ContentTypeHeader:
        ///     If header value is application/json, append the following default values:
        ///         (odata.)metadata=minimal
        ///         (odata.)streaming=true
        ///         IEEE754Compatible=false
        /// </summary>
        /// <param name="headerName">The name of the header to append default values.</param>
        /// <param name="headerValue">The original header value string.</param>
        /// <returns>The header value string with appended default values.</returns>
        public static string AppendDefaultHeaderValue(string headerName, string headerValue)
        {
            return AppendDefaultHeaderValue(headerName, headerValue, ODataVersion.V4);
        }

        /// <summary>
        /// Append default values required by OData to specified HTTP header.
        ///
        /// When header name is ODataConstants.ContentTypeHeader, if header value is application/json
        ///     append the following default values for 4.0:
        ///         odata.metadata=minimal
        ///         odata.streaming=true
        ///         IEEE754Compatible=false
        ///     append the following default values for 4.01:
        ///         metadata=minimal
        ///         streaming=true
        ///         IEEE754Compatible=false
        /// </summary>
        /// <param name="headerName">The name of the header to append default values.</param>
        /// <param name="headerValue">The original header value string.</param>
        /// <param name="version">The ODataVersion for which to create the default header value</param>
        /// <returns>The header value string with appended default values.</returns>
        public static string AppendDefaultHeaderValue(string headerName, string headerValue, ODataVersion version)
        {
            if (string.CompareOrdinal(headerName, ODataConstants.ContentTypeHeader) != 0)
            {
                return headerValue;
            }

            if (headerValue == null)
            {
                return null;
            }

            var mediaTypeList = HttpUtils.MediaTypesFromString(headerValue);
            var mediaType = mediaTypeList.Single().Key;
            var encoding = HttpUtils.GetEncodingFromCharsetName(mediaTypeList.Single().Value);

            if (string.CompareOrdinal(mediaType.FullTypeName, MimeConstants.MimeApplicationJson) != 0)
            {
                return headerValue;
            }

            var extendedParameters = new List<KeyValuePair<string, string>>();
            var extendedMediaType = new ODataMediaType(mediaType.Type, mediaType.SubType, extendedParameters);

            var hasMetadata = false;
            var hasStreaming = false;
            var hasIeee754Compatible = false;

            if (mediaType.Parameters != null)
            {
                foreach (var parameter in mediaType.Parameters)
                {
                    extendedParameters.Add(parameter);

                    if (HttpUtils.IsMetadataParameter(parameter.Key))
                    {
                        hasMetadata = true;
                    }

                    if (HttpUtils.IsStreamingParameter(parameter.Key))
                    {
                        hasStreaming = true;
                    }

                    if (string.Compare(parameter.Key, MimeConstants.MimeIeee754CompatibleParameterName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        hasIeee754Compatible = true;
                    }
                }
            }

            if (!hasMetadata)
            {
                extendedParameters.Add(new KeyValuePair<string, string>(
                    version < ODataVersion.V401 ? MimeConstants.MimeMetadataParameterName : MimeConstants.MimeShortMetadataParameterName,
                    MimeConstants.MimeMetadataParameterValueMinimal));
            }

            if (!hasStreaming)
            {
                extendedParameters.Add(new KeyValuePair<string, string>(
                    version < ODataVersion.V401 ? MimeConstants.MimeStreamingParameterName : MimeConstants.MimeShortStreamingParameterName,
                    MimeConstants.MimeParameterValueTrue));
            }

            if (!hasIeee754Compatible)
            {
                extendedParameters.Add(new KeyValuePair<string, string>(
                    MimeConstants.MimeIeee754CompatibleParameterName,
                    MimeConstants.MimeParameterValueFalse));
            }

            return extendedMediaType.ToText(encoding);
        }
    }
}

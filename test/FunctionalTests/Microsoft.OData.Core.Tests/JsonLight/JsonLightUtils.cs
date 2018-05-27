//---------------------------------------------------------------------
// <copyright file="JsonLightUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Tests.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.JsonLight;
    #endregion Namespaces

    public static class JsonLightUtils
    {
        /// <summary>The default streaming Json Light media type.</summary>
        internal static readonly ODataMediaType JsonLightStreamingMediaType = new ODataMediaType(
            MimeConstants.MimeApplicationType,
            MimeConstants.MimeJsonSubType,
            new[]{
                new KeyValuePair<string, string>(MimeConstants.MimeMetadataParameterName, MimeConstants.MimeMetadataParameterValueMinimal),
                new KeyValuePair<string, string>(MimeConstants.MimeStreamingParameterName, MimeConstants.MimeParameterValueTrue),
                new KeyValuePair<string, string>(MimeConstants.MimeIeee754CompatibleParameterName, MimeConstants.MimeParameterValueFalse)
            });

        /// <summary>
        /// Gets the name of the property annotation property.
        /// </summary>
        /// <param name="propertyName">The name of the property to annotate.</param>
        /// <param name="annotationName">The name of the annotation.</param>
        /// <returns>The property name for the annotation property.</returns>
        public static string GetPropertyAnnotationName(string propertyName, string annotationName)
        {
            return propertyName + JsonLightConstants.ODataPropertyAnnotationSeparatorChar + annotationName;
        }


        /// <summary>
        /// Get the Json string with special characters escaped.
        /// </summary>
        /// <param name="text">The original string containing Json special characters that need to apply the escape rules.</param>
        /// <returns>The encoded Json string.</returns>
        public static string GetJsonEncodedString(string text)
        {
            return text.Replace("\\", "\\\\")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("\f", "\\f")
                .Replace("\t", "\\t")
                .Replace("\"", "\\\"")
                .Replace("\b", "\\b");
        }

        /// <summary>
        /// Get the Base64Url encoded string for the binary.
        /// </summary>
        /// <param name="bytes">The binary bytes to be encoded.</param>
        /// <returns>The Base64Url encoded string.</returns>
        public static string GetBase64UrlEncodedString(byte[] bytes)
        {
            return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_');
        }
    }
}
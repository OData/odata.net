//---------------------------------------------------------------------
// <copyright file="JsonLightUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Core.JsonLight;

namespace Microsoft.OData.Core.Tests.JsonLight
{
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
    }
}
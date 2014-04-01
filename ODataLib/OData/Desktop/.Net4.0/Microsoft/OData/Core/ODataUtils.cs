//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Utility methods used with the OData library.
    /// </summary>
    public static class ODataUtils
    {
        /// <summary>String representation of the version 4.0 of the OData protocol.</summary>
        private const string Version4NumberString = "4.0";

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
        /// <param name="messageReader">The <see cref="T:Microsoft.OData.Core.ODataMessageReader" /> to get the read format from.</param>
        /// <remarks>This method must only be called once reading has started.
        /// This means that a read method has been called on the <paramref name="messageReader"/> or that a reader (for entries, feeds, collections, etc.) has been created.
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
    }
}

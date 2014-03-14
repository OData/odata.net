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

namespace Microsoft.Data.OData
{
    /// <summary>
    /// Constant values related to media types.
    /// </summary>
    internal static class MimeConstants
    {
        /// <summary>Media type for requesting any media type.</summary>
        internal const string MimeAny = "*/*";

        /// <summary>'application' - media type for application types.</summary>
        internal const string MimeApplicationType = "application";

        /// <summary>'text' - media type for text subtypes.</summary>
        internal const string MimeTextType = "text";

        /// <summary>'multipart' - media type.</summary>
        internal const string MimeMultipartType = "multipart";

        /// <summary>'atom+xml' - constant for atom+xml subtypes.</summary>
        internal const string MimeAtomXmlSubType = "atom+xml";

        /// <summary>'atomsvc+xml' - constant for atomsvc+xml subtypes.</summary>
        internal const string MimeAtomSvcXmlSubType = "atomsvc+xml";

        /// <summary>'xml' - constant for xml subtypes.</summary>
        internal const string MimeXmlSubType = "xml";

        /// <summary>'json' - constant for JSON subtypes.</summary>
        internal const string MimeJsonSubType = "json";

        /// <summary>'plain' - constant for text subtypes.</summary>
        internal const string MimePlainSubType = "plain";

        /// <summary>'javascript' - constant for javascript subtypes.</summary>
        internal const string MimeJavaScriptType = "javascript";

        /// <summary>'octet-stream' subtype.</summary>
        internal const string MimeOctetStreamSubType = "octet-stream";

        /// <summary>'mixed' subtype.</summary>
        internal const string MimeMixedSubType = "mixed";

        /// <summary>'http' subtype.</summary>
        internal const string MimeHttpSubType = "http";

        /// <summary>Parameter name for 'type' parameters.</summary>
        internal const string MimeTypeParameterName = "type";

        /// <summary>Parameter value for type 'entry'.</summary>
        internal const string MimeTypeParameterValueEntry = "entry";

        /// <summary>Parameter value for type 'feed'.</summary>
        internal const string MimeTypeParameterValueFeed = "feed";

        /// <summary>Parameter name for 'odata' parameters.</summary>
        internal const string MimeODataParameterName = "odata";

        /// <summary>Parameter value for 'verbose' JSON.</summary>
        internal const string MimeODataParameterValueVerbose = "verbose";

        /// <summary>JSON Light parameter value 'fullmetadata'.</summary>
        internal const string MimeODataParameterValueFullMetadata = "fullmetadata";

        /// <summary>JSON Light parameter value 'minimalmetadata'.</summary>
        internal const string MimeODataParameterValueMinimalMetadata = "minimalmetadata";

        /// <summary>JSON Light parameter value 'nometadata'.</summary>
        internal const string MimeODataParameterValueNoMetadata = "nometadata";

        /// <summary>Parameter name for 'streaming' parameter.</summary>
        internal const string MimeStreamingParameterName = "streaming";

        /// <summary>JSON Light streaming parameter value 'true'.</summary>
        internal const string MimeStreamingParameterValueTrue = "true";

        /// <summary>JSON Light streaming parameter value 'false'.</summary>
        internal const string MimeStreamingParameterValueFalse = "false";

        /// <summary>Media type for XML bodies.</summary>
        internal const string MimeApplicationXml = MimeApplicationType + Separator + MimeXmlSubType;

        /// <summary>Media type for ATOM payloads.</summary>
        internal const string MimeApplicationAtomXml = MimeApplicationType + Separator + MimeAtomXmlSubType;

        /// <summary>Media type for links referencing a single entry.</summary>
        internal const string MimeApplicationAtomXmlTypeEntry = MimeApplicationAtomXml + ";" + MimeTypeParameterName + "=" + MimeTypeParameterValueEntry;

        /// <summary>Media type for links referencing a collection of entries.</summary>
        internal const string MimeApplicationAtomXmlTypeFeed = MimeApplicationAtomXml + ";" + MimeTypeParameterName + "=" + MimeTypeParameterValueFeed;

        /// <summary>Media type for JSON payloads.</summary>
        internal const string MimeApplicationJson = MimeApplicationType + Separator + MimeJsonSubType;

        /// <summary>Media type for binary raw content.</summary>
        internal const string MimeApplicationOctetStream = MimeApplicationType + Separator + MimeOctetStreamSubType;

        /// <summary>Media type for batch parts.</summary>
        internal const string MimeApplicationHttp = MimeApplicationType + Separator + MimeHttpSubType;

        /// <summary>Media type for Xml bodies (deprecated).</summary>
        internal const string MimeTextXml = MimeTextType + Separator + MimeXmlSubType;

        /// <summary>Media type for raw content (except binary).</summary>
        internal const string MimeTextPlain = MimeTextType + Separator + MimePlainSubType;
        
        /// <summary>Media type for javascript content.</summary>
        internal const string TextJavaScript = MimeTextType + Separator + MimeJavaScriptType;

        /// <summary>Media type for raw content (except binary).</summary>
        internal const string MimeMultipartMixed = MimeMultipartType + Separator + MimeMixedSubType;

        /// <summary>The '*' wildcard usable in type names and subtype names.</summary>
        internal const string MimeStar = "*";

        /// <summary>Separator between mediat type and subtype.</summary>
        private const string Separator = "/";
    }
}

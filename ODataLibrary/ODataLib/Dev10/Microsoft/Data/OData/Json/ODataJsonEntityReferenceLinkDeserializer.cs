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
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.Edm.Library;
    #endregion Namespaces

    /// <summary>
    /// OData JSON deserializer for entity reference links.
    /// </summary>
    internal sealed class ODataJsonEntityReferenceLinkDeserializer : ODataJsonDeserializer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonInputContext">The JSON input context to read from.</param>
        internal ODataJsonEntityReferenceLinkDeserializer(ODataJsonInputContext jsonInputContext)
            : base(jsonInputContext)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Read a set of top-level entity reference links.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLinks"/> representing the read links.</returns>
        internal ODataEntityReferenceLinks ReadEntityReferenceLinks()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            this.JsonReader.AssertNotBuffering();

            // Set to true if the entity reference links are expected to have the 'results' wrapper.
            // Entity reference links are only expected to have a results wrapper if
            // (a) the protocol version is >= 2 AND
            // (b) we are reading a response
            // NOTE: OIPI does not specify a format for >= v2 entity reference links in requests; we thus use the v1 format and consequently do not expect a result wrapper.
            bool isResultsWrapperExpected = this.Version >= ODataVersion.V2 && this.ReadingResponse;

            ODataJsonReaderUtils.EntityReferenceLinksWrapperPropertyBitMask propertiesFoundBitField =
                ODataJsonReaderUtils.EntityReferenceLinksWrapperPropertyBitMask.None;
            ODataEntityReferenceLinks entityReferenceLinks = new ODataEntityReferenceLinks();

            // Read the response wrapper "d" if expected.
            this.ReadDataWrapperStart();

            if (isResultsWrapperExpected)
            {
                // Read the start object of the results wrapper object
                this.JsonReader.ReadStartObject();

                bool foundResultsProperty = this.ReadEntityReferenceLinkProperties(entityReferenceLinks, ref propertiesFoundBitField);

                if (!foundResultsProperty)
                {
                    throw new ODataException(Strings.ODataJsonEntityReferenceLinkDeserializer_ExpectedEntityReferenceLinksResultsPropertyNotFound);
                }
            }

            // Read the start of the content array of the links
            this.JsonReader.ReadStartArray();

            List<ODataEntityReferenceLink> links = new List<ODataEntityReferenceLink>();

            while (this.JsonReader.NodeType != JsonNodeType.EndArray)
            {
                // read another link
                ODataEntityReferenceLink entityReferenceLink = this.ReadSingleEntityReferenceLink();
                links.Add(entityReferenceLink);
            }

            // Read over the end object - note that this might be the last node in the input (in case there's no response wrapper)
            this.JsonReader.ReadEndArray();

            if (isResultsWrapperExpected)
            {
                this.ReadEntityReferenceLinkProperties(entityReferenceLinks, ref propertiesFoundBitField);

                // Read the end object of the results wrapper object
                this.JsonReader.ReadEndObject();
            }

            entityReferenceLinks.Links = new ReadOnlyEnumerable<ODataEntityReferenceLink>(links);

            // Read the end of the response wrapper "d" if expected.
            this.ReadDataWrapperEnd();

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            return entityReferenceLinks;
        }

        /// <summary>
        /// Reads a top-level entity reference link - implementation of the actual functionality.
        /// </summary>
        /// <returns>An <see cref="ODataEntityReferenceLink"/> representing the read entity reference link.</returns>
        internal ODataEntityReferenceLink ReadEntityReferenceLink()
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            this.JsonReader.AssertNotBuffering();

            // Read the response wrapper "d" if expected.
            this.ReadDataWrapperStart();

            ODataEntityReferenceLink entityReferenceLink = this.ReadSingleEntityReferenceLink();

            // Read the end of the response wrapper "d" if expected.
            this.ReadDataWrapperEnd();

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            return entityReferenceLink;
        }

        /// <summary>
        /// Reads the properties of an entity reference link.
        /// </summary>
        /// <param name="entityReferenceLinks">The <see cref="ODataEntityReferenceLinks"/> instance to set the read property values on.</param>
        /// <param name="propertiesFoundBitField">The bit field with all the properties already read.</param>
        /// <returns>true if the method found the 'results' property; otherwise false.</returns>
        private bool ReadEntityReferenceLinkProperties(
            ODataEntityReferenceLinks entityReferenceLinks,
            ref ODataJsonReaderUtils.EntityReferenceLinksWrapperPropertyBitMask propertiesFoundBitField)
        {
            Debug.Assert(entityReferenceLinks != null, "entityReferenceLinks != null");
            this.JsonReader.AssertNotBuffering();

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                string propertyName = this.JsonReader.ReadPropertyName();
                switch (propertyName)
                {
                    case JsonConstants.ODataResultsName:
                        ODataJsonReaderUtils.VerifyEntityReferenceLinksWrapperPropertyNotFound(
                            ref propertiesFoundBitField,
                            ODataJsonReaderUtils.EntityReferenceLinksWrapperPropertyBitMask.Results,
                            JsonConstants.ODataResultsName);
                        this.JsonReader.AssertNotBuffering();
                        return true;

                    case JsonConstants.ODataCountName:
                        ODataJsonReaderUtils.VerifyEntityReferenceLinksWrapperPropertyNotFound(
                            ref propertiesFoundBitField,
                            ODataJsonReaderUtils.EntityReferenceLinksWrapperPropertyBitMask.InlineCount,
                            JsonConstants.ODataCountName);
                        object inlineCountValue = this.JsonReader.ReadPrimitiveValue();
                        long? inlineCount = (long?)ODataJsonReaderUtils.ConvertValue(inlineCountValue, EdmCoreModel.Instance.GetInt64(true), /*usesV1ProviderBehavior*/ false);
                        ODataJsonReaderUtils.ValidateCountPropertyInEntityReferenceLinks(inlineCount);
                        entityReferenceLinks.InlineCount = inlineCount;
                        break;

                    case JsonConstants.ODataNextLinkName:
                        ODataJsonReaderUtils.VerifyEntityReferenceLinksWrapperPropertyNotFound(
                            ref propertiesFoundBitField,
                            ODataJsonReaderUtils.EntityReferenceLinksWrapperPropertyBitMask.NextPageLink,
                            JsonConstants.ODataNextLinkName);
                        string nextLinkString = this.JsonReader.ReadStringValue(JsonConstants.ODataNextLinkName);
                        ODataJsonReaderUtils.ValidateEntityReferenceLinksStringProperty(nextLinkString, JsonConstants.ODataNextLinkName);
                        entityReferenceLinks.NextLink = this.ProcessUriFromPayload(nextLinkString);
                        break;

                    default:
                        // Skip all unrecognized properties
                        this.JsonReader.SkipValue();
                        break;
                }
            }

            this.JsonReader.AssertNotBuffering();
            return false;
        }

        /// <summary>
        /// Read an entity reference link.
        /// </summary>
        /// <returns>An instance of <see cref="ODataEntityReferenceLink"/> which was read.</returns>
        /// <remarks>
        /// Pre-Condition:  any node   - This method will throw if the node type is not a StartObject node
        /// Post-Condition: any node
        /// </remarks>
        private ODataEntityReferenceLink ReadSingleEntityReferenceLink()
        {
            this.JsonReader.AssertNotBuffering();

            if (this.JsonReader.NodeType != JsonNodeType.StartObject)
            {
                // entity reference link has to be an object
                throw new ODataException(Strings.ODataJsonEntityReferenceLinkDeserializer_EntityReferenceLinkMustBeObjectValue(this.JsonReader.NodeType));
            }

            this.JsonReader.ReadStartObject();

            ODataEntityReferenceLink entityReferenceLink = new ODataEntityReferenceLink();

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                // read the 'uri' property
                string propertyName = this.JsonReader.ReadPropertyName();
                if (string.CompareOrdinal(JsonConstants.ODataUriName, propertyName) == 0)
                {
                    if (entityReferenceLink.Url != null)
                    {
                        throw new ODataException(Strings.ODataJsonEntityReferenceLinkDeserializer_MultipleUriPropertiesInEntityReferenceLink);
                    }

                    // read the value of the 'uri' property
                    string uriString = this.JsonReader.ReadStringValue(JsonConstants.ODataUriName);
                    if (uriString == null)
                    {
                        throw new ODataException(Strings.ODataJsonEntityReferenceLinkDeserializer_EntityReferenceLinkUriCannotBeNull);
                    }

                    entityReferenceLink.Url = this.ProcessUriFromPayload(uriString);
                }
                else
                {
                    // Skip unrecognized properties
                    this.JsonReader.SkipValue();
                }
            }

            ReaderValidationUtils.ValidateEntityReferenceLink(entityReferenceLink);

            // end of the entity reference link object
            this.JsonReader.ReadEndObject();

            this.JsonReader.AssertNotBuffering();
            return entityReferenceLink;
        }
    }
}

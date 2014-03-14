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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System.Diagnostics;
    using System.Xml;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Responsible for reading the annotation element in ATOM.
    /// </summary>
    internal sealed class ODataAtomAnnotationReader
    {
        /// <summary>
        /// The input context to use when parsing the annotation element.
        /// </summary>
        private readonly ODataAtomInputContext inputContext;

        #region Atomized strings
        /// <summary>Atomized string representation of the URI used for the OData metadata namespace.</summary>
        private readonly string odataMetadataNamespace;

        /// <summary>Atomized name of the decimal attribute of an annotation element.</summary>
        private readonly string attributeElementName;

        /// <summary>The property and value deserializer used to read values in ATOM.</summary>
        private readonly ODataAtomPropertyAndValueDeserializer propertyAndValueDeserializer;
        #endregion Atomized strings

        /// <summary>
        /// Creates a new ATOM annotation parser.
        /// </summary>
        /// <param name="inputContext">The input context this annotation reader should use to read annotation elements.</param>
        /// <param name="propertyAndValueDeserializer">The property and value deserializer to use to read the value of an annotation element.</param>
        internal ODataAtomAnnotationReader(ODataAtomInputContext inputContext, ODataAtomPropertyAndValueDeserializer propertyAndValueDeserializer)
        {
            DebugUtils.CheckNoExternalCallers();

            this.inputContext = inputContext;
            this.propertyAndValueDeserializer = propertyAndValueDeserializer;
            BufferingXmlReader xmlReader = this.inputContext.XmlReader;

            Debug.Assert(xmlReader != null, "xmlReader != null");
            Debug.Assert(xmlReader.NameTable != null, "xmlReader.NameTable != null");
            xmlReader.NameTable.Add(AtomConstants.ODataAnnotationTargetAttribute);
            xmlReader.NameTable.Add(AtomConstants.ODataAnnotationTermAttribute);
            xmlReader.NameTable.Add(AtomConstants.AtomTypeAttributeName);
            xmlReader.NameTable.Add(AtomConstants.ODataNullAttributeName);
            xmlReader.NameTable.Add(AtomConstants.ODataAnnotationStringAttribute);
            xmlReader.NameTable.Add(AtomConstants.ODataAnnotationBoolAttribute);
            xmlReader.NameTable.Add(AtomConstants.ODataAnnotationDecimalAttribute);
            xmlReader.NameTable.Add(AtomConstants.ODataAnnotationIntAttribute);
            xmlReader.NameTable.Add(AtomConstants.ODataAnnotationFloatAttribute);
            this.odataMetadataNamespace = xmlReader.NameTable.Add(AtomConstants.ODataMetadataNamespace);
            this.attributeElementName = xmlReader.NameTable.Add(AtomConstants.ODataAnnotationElementName);
        }

        /// <summary>
        /// Attempts to read the current element as an annotation element.
        /// </summary>
        /// <param name="annotation">If this method returned true, this is the instance annotation information from the parsed element.</param>
        /// <returns>true if the element was an annotation element, false if it wasn't.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element    - The element to read.
        /// Post-Condition:  XmlNodeType.EndElement - The end tag of the element (if the element was a non-empty annotation element).
        ///                  XmlNodeType.Element    - The same element as the pre-condition if this method returned false, or an empty annotation element.
        /// </remarks>
        internal bool TryReadAnnotation(out AtomInstanceAnnotation annotation)
        {
            DebugUtils.CheckNoExternalCallers();

            BufferingXmlReader xmlReader = this.inputContext.XmlReader;
            Debug.Assert(xmlReader != null, "xmlReader != null");
            Debug.Assert(xmlReader.NodeType == XmlNodeType.Element, "xmlReader must be positioned on an Element");

            annotation = null;

            if (this.propertyAndValueDeserializer.MessageReaderSettings.ShouldIncludeAnnotation != null
                && xmlReader.NamespaceEquals(this.odataMetadataNamespace)
                && xmlReader.LocalNameEquals(this.attributeElementName))
            {
                annotation = AtomInstanceAnnotation.CreateFrom(this.inputContext, this.propertyAndValueDeserializer);
            }

            return annotation != null;
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="CsdlReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies.Community.V1;
using Microsoft.OData.Edm.Vocabularies.V1;

namespace Microsoft.OData.Edm.Csdl.Reader
{
    /// <summary>
    /// Provides CSDL parsing services for EDM models.
    /// </summary>
    internal class CsdlXmlReader : CsdlReader
    {
        private static readonly Dictionary<string, Action> EmptyParserLookup = new Dictionary<string, Action>();
        private readonly Dictionary<string, Action> edmxParserLookup;
        private readonly Dictionary<string, Action> runtimeParserLookup;
        private readonly Dictionary<string, Action> conceptualModelsParserLookup;
        private readonly Dictionary<string, Action> dataServicesParserLookup;
        private readonly XmlReader reader;
        private readonly List<EdmError> errors;
        private readonly List<IEdmReference> edmReferences;
        private readonly CsdlParser csdlParser;
        private readonly Func<Uri, XmlReader> getReferencedModelReaderFunc; // Url -> XmlReader

        /// <summary>
        /// True when either Runtime or DataServices node have been processed.
        /// </summary>
        private bool targetParsed;

        /// <summary>
        /// Ignore the unexpected attributes and elements.
        /// </summary>
        private bool ignoreUnexpectedAttributesAndElements;

        /// <summary>
        /// Indicates where the document comes from.
        /// </summary>
        private string source;

        public CsdlXmlReader(XmlReader reader, Func<Uri, XmlReader> getReferencedModelReaderFunc)
            : this(reader, getReferencedModelReaderFunc, false)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reader">The XmlReader for current CSDL doc</param>
        /// <param name="getReferencedModelReaderFunc">The function to load referenced model xml. If null, will stop loading the referenced model.</param>
        public CsdlXmlReader(XmlReader reader, Func<Uri, XmlReader> getReferencedModelReaderFunc, bool ignoreUnexpectedAttributesAndElements)
        {
            this.reader = reader;
            this.getReferencedModelReaderFunc = getReferencedModelReaderFunc;
            this.ignoreUnexpectedAttributesAndElements = ignoreUnexpectedAttributesAndElements;
            this.errors = new List<EdmError>();
            this.edmReferences = new List<IEdmReference>();
            this.csdlParser = new CsdlParser();

            // Setup the edmx parser.
            this.edmxParserLookup = new Dictionary<string, Action>
            {
                { CsdlConstants.Element_DataServices, this.ParseDataServicesElement },
                { CsdlConstants.Element_Reference, this.ParseReferenceElement },
                { CsdlConstants.Element_Runtime, this.ParseRuntimeElement }
            };
            this.dataServicesParserLookup = new Dictionary<string, Action>
            {
                { CsdlConstants.Element_Schema, this.ParseSchemaElement }
            };
            this.runtimeParserLookup = new Dictionary<string, Action>
            {
                { CsdlConstants.Element_ConceptualModels, this.ParseConceptualModelsElement }
            };
            this.conceptualModelsParserLookup = new Dictionary<string, Action>
            {
                { CsdlConstants.Element_Schema, this.ParseSchemaElement }
            };
        }

        protected bool TryParse(IEnumerable<IEdmModel> referencedModels, bool includeDefaultVocabularies, out IEdmModel model, out IEnumerable<EdmError> parsingErrors)
        {
            Version edmxVersion;
            CsdlModel astModel;

            TryParseCsdlFileToCsdlModel(out edmxVersion, out astModel);

            if (!this.HasIntolerableError())
            {
                List<CsdlModel> referencedAstModels = this.LoadAndParseReferencedCsdlFiles(edmxVersion);

                IEnumerable<EdmError> csdlErrors;
                this.csdlParser.GetResult(out astModel, out csdlErrors);
                if (csdlErrors != null)
                {
                    this.errors.AddRange(csdlErrors.Except(this.errors));
                }

                if (!this.HasIntolerableError())
                {
                    CsdlSemanticsModel tmp = new CsdlSemanticsModel(astModel, new CsdlSemanticsDirectValueAnnotationsManager(), referencedAstModels, includeDefaultVocabularies);

                    // add more referenced IEdmModels in addition to the above loaded CsdlModels.
                    tmp.AddToReferencedModels(referencedModels);

                    model = tmp;
                    Debug.Assert(edmxVersion != null, "edmxVersion != null");
                    model.SetEdmxVersion(edmxVersion);
                    Version edmVersion;
                    if (CsdlConstants.EdmxToEdmVersions.TryGetValue(edmxVersion, out edmVersion))
                    {
                        model.SetEdmVersion(edmVersion);
                    }
                }
                else
                {
                    model = null;
                }
            }
            else
            {
                model = null;
            }

            parsingErrors = this.errors;

            return !this.HasIntolerableError();
        }

        /// <summary>
        /// <see cref="Version"/>TryParse does not exist on all platforms, so implementing it here.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <param name="version">Parsed version.</param>
        /// <returns>False in case of failure.</returns>
        private static bool TryParseVersion(string input, out Version version)
        {
            version = null;

            if (String.IsNullOrEmpty(input))
            {
                return false;
            }

            input = input.Trim();

            var parts = input.Split('.');
            if (parts.Length != 2)
            {
                return false;
            }

            int major;
            int minor;
            if (!int.TryParse(parts[0], out major) || !int.TryParse(parts[1], out minor))
            {
                return false;
            }

            version = new Version(major, minor);
            return true;
        }


        /// <summary>
        /// Load and parse the referenced model but ignored any further referenced model.
        /// </summary>
        /// <param name="mainCsdlVersion">The main CSDL version.</param>
        /// <returns>A list of CsdlModel (no semantics) of the referenced models.</returns>
        private List<CsdlModel> LoadAndParseReferencedCsdlFiles(Version mainCsdlVersion)
        {
            List<CsdlModel> referencedAstModels = new List<CsdlModel>();
            if (this.getReferencedModelReaderFunc == null)
            {
                // don't try to load Edm xml doc, but this.edmReferences's namespace-alias need to be used later.
                return referencedAstModels;
            }

            foreach (var edmReference in this.edmReferences)
            {
                if (!edmReference.Includes.Any() && !edmReference.IncludeAnnotations.Any())
                {
                    this.RaiseError(EdmErrorCode.ReferenceElementMustContainAtLeastOneIncludeOrIncludeAnnotationsElement, Strings.EdmxParser_InvalidReferenceIncorrectNumberOfIncludes);
                    continue;
                }

                if (edmReference.Uri != null && (edmReference.Uri.OriginalString.EndsWith(CoreVocabularyConstants.VocabularyUrlSuffix, StringComparison.Ordinal) ||
                    edmReference.Uri.OriginalString.EndsWith(CapabilitiesVocabularyConstants.VocabularyUrlSuffix, StringComparison.Ordinal) ||
                    edmReference.Uri.OriginalString.EndsWith("/Org.OData.Authorization.V1.xml", StringComparison.Ordinal) ||
                    edmReference.Uri.OriginalString.EndsWith("/Org.OData.Validation.V1.xml", StringComparison.Ordinal) ||
                    edmReference.Uri.OriginalString.EndsWith("/Org.OData.Community.V1.xml", StringComparison.Ordinal) ||
                    edmReference.Uri.OriginalString.EndsWith(AlternateKeysVocabularyConstants.VocabularyUrlSuffix, StringComparison.Ordinal)))
                {
                    continue;
                }

                XmlReader referencedXmlReader = this.getReferencedModelReaderFunc(edmReference.Uri);
                if (referencedXmlReader == null)
                {
                    this.RaiseError(EdmErrorCode.UnresolvedReferenceUriInEdmxReference, Strings.EdmxParser_UnresolvedReferenceUriInEdmxReference);
                    continue;
                }

                // recursively use CsdlReader to parse sub edm:
                CsdlXmlReader referencedEdmxReader = new CsdlXmlReader(referencedXmlReader, /*getReferencedModelReaderFunc*/ null, this.ignoreUnexpectedAttributesAndElements);
                referencedEdmxReader.source = edmReference.Uri != null ? edmReference.Uri.OriginalString : null;
                Version referencedEdmxVersion;
                CsdlModel referencedAstModel;
                if (referencedEdmxReader.TryParseCsdlFileToCsdlModel(out referencedEdmxVersion, out referencedAstModel))
                {
                    if (!mainCsdlVersion.Equals(referencedEdmxVersion))
                    {
                        // TODO: REF add exception message
                        this.errors.Add(null);
                    }

                    referencedAstModel.AddParentModelReferences(edmReference);
                    referencedAstModels.Add(referencedAstModel);
                }

                this.errors.AddRange(referencedEdmxReader.errors);
            }

            return referencedAstModels;
        }

        /// <summary>
        /// Parse CSDL xml doc into CsdlModel, error messages are stored in this.errors.
        /// </summary>
        /// <param name="csdlVersion">The csdlVersion out.</param>
        /// <param name="csdlModel">The CsdlModel out.</param>
        /// <returns>True if succeeded.</returns>
        internal bool TryParseCsdlFileToCsdlModel(out Version csdlVersion, out CsdlModel csdlModel)
        {
            csdlVersion = null;
            csdlModel = null;
            try
            {
                // Advance to root element
                if (this.reader.NodeType != XmlNodeType.Element)
                {
                    while (this.reader.Read() && this.reader.NodeType != XmlNodeType.Element)
                    {
                    }
                }

                // There must be a root element for all current artifacts
                if (this.reader.EOF)
                {
                    this.RaiseEmptyFile();
                    return false;
                }

                if (this.reader.LocalName != CsdlConstants.Element_Edmx ||
                    !CsdlConstants.SupportedEdmxNamespaces.TryGetValue(this.reader.NamespaceURI, out csdlVersion))
                {
                    this.RaiseError(EdmErrorCode.UnexpectedXmlElement, Edm.Strings.XmlParser_UnexpectedRootElement(this.reader.Name, CsdlConstants.Element_Edmx));
                    return false;
                }

                csdlVersion = this.ParseEdmxElement(csdlVersion);
                IEnumerable<EdmError> err;
                if (!this.csdlParser.GetResult(out csdlModel, out err))
                {
                    this.errors.AddRange(err);
                    if (this.HasIntolerableError())
                    {
                        return false;
                    }
                }
            }
            catch (XmlException e)
            {
                this.errors.Add(new EdmError(new CsdlLocation(this.source, e.LineNumber, e.LinePosition), EdmErrorCode.XmlError, e.Message));
                return false;
            }

            csdlModel.AddCurrentModelReferences(this.edmReferences);
            return true;
        }

        /// <summary>
        /// Determine if there is any error that could not be ignored.
        /// </summary>
        /// <returns>True if there is any error that could not be ignored.</returns>
        private bool HasIntolerableError()
        {
            if (this.ignoreUnexpectedAttributesAndElements)
            {
                return this.errors.Any(error => error.ErrorCode != EdmErrorCode.UnexpectedXmlElement && error.ErrorCode != EdmErrorCode.UnexpectedXmlAttribute);
            }

            return this.errors.Any();
        }

        /// <summary>
        /// All parse functions start with the reader pointing at the start tag of an element, and end after consuming the ending tag for the element.
        /// </summary>
        /// <param name="elementName">The current element name to be parsed.</param>
        /// <param name="elementParsers">The parsers for child elements of the current element.</param>
        private void ParseElement(string elementName, Dictionary<string, Action> elementParsers)
        {
            Debug.Assert(this.reader.LocalName == elementName, "Must call ParseElement on correct element type");
            if (this.reader.IsEmptyElement)
            {
                // Consume the tag.
                this.reader.Read();
            }
            else
            {
                // Consume the start tag.
                this.reader.Read();
                while (this.reader.NodeType != XmlNodeType.EndElement)
                {
                    if (this.reader.NodeType == XmlNodeType.Element)
                    {
                        if (elementParsers.ContainsKey(this.reader.LocalName))
                        {
                            elementParsers[this.reader.LocalName]();
                        }
                        else
                        {
                            this.ParseElement(this.reader.LocalName, EmptyParserLookup);
                        }
                    }
                    else
                    {
                        if (!this.reader.Read())
                        {
                            break;
                        }
                    }
                }

                Debug.Assert(elementName == this.reader.LocalName, "The XmlReader should have thrown an error if the opening and closing tags do not match");

                // Consume the ending tag.
                this.reader.Read();
            }
        }

        private Version ParseEdmxElement(Version edmxVersion)
        {
            Debug.Assert(this.reader.LocalName == CsdlConstants.Element_Edmx, "this.reader.LocalName == CsdlConstants.Element_Edmx");
            Debug.Assert(edmxVersion != null, "edmxVersion != null");

            string edmxVersionString = this.GetAttributeValue(null, CsdlConstants.Attribute_Version);
            Version edmxVersionFromAttribute = null;
            if (edmxVersionString != null && (!TryParseVersion(edmxVersionString, out edmxVersionFromAttribute) || edmxVersionFromAttribute.Major != edmxVersion.Major))
            {
                this.RaiseError(EdmErrorCode.InvalidVersionNumber, Edm.Strings.EdmxParser_EdmxVersionMismatch);
            }

            this.ParseElement(CsdlConstants.Element_Edmx, this.edmxParserLookup);

            return edmxVersionFromAttribute ?? edmxVersion;
        }

        private string GetAttributeValue(string namespaceUri, string localName)
        {
            //// OData BufferingXmlReader does not support <see cref="XmlReader.GetAttribute(string)"/> API, so implementing it here.

            string elementNamespace = this.reader.NamespaceURI;
            Debug.Assert(!String.IsNullOrEmpty(elementNamespace), "!String.IsNullOrEmpty(elementNamespace)");

            string value = null;
            bool hasAttributes = this.reader.MoveToFirstAttribute();
            while (hasAttributes)
            {
                if ((namespaceUri != null && this.reader.NamespaceURI == namespaceUri || (String.IsNullOrEmpty(this.reader.NamespaceURI) || this.reader.NamespaceURI == elementNamespace)) &&
                    this.reader.LocalName == localName)
                {
                    value = this.reader.Value;
                    break;
                }

                hasAttributes = this.reader.MoveToNextAttribute();
            }

            // Move back to the element.
            this.reader.MoveToElement();
            return value;
        }

        private void ParseRuntimeElement()
        {
            this.ParseTargetElement(CsdlConstants.Element_Runtime, this.runtimeParserLookup);
        }

        private void ParseDataServicesElement()
        {
            this.ParseTargetElement(CsdlConstants.Element_DataServices, this.dataServicesParserLookup);
        }

        private void ParseTargetElement(string elementName, Dictionary<string, Action> elementParsers)
        {
            if (!this.targetParsed)
            {
                this.targetParsed = true;
            }
            else
            {
                // Edmx should contain at most one element - either <DataServices> or <Runtime>.
                this.RaiseError(EdmErrorCode.UnexpectedXmlElement, Edm.Strings.EdmxParser_BodyElement(CsdlConstants.Element_DataServices));

                // Read to the end of the element anyway, to let the caller move on to the rest of the document.
                elementParsers = EmptyParserLookup;
            }

            this.ParseElement(elementName, elementParsers);
        }

        private void ParseConceptualModelsElement()
        {
            this.ParseElement(CsdlConstants.Element_ConceptualModels, this.conceptualModelsParserLookup);
        }

        /// <summary>
        /// TODO: use XmlDocumentParser
        /// </summary>
        private void ParseReferenceElement()
        {
            // read 'Uri' attribute
            EdmReference result = new EdmReference(new Uri(this.GetAttributeValue(null, CsdlConstants.Attribute_Uri), UriKind.RelativeOrAbsolute));
            if (this.reader.IsEmptyElement)
            {
                this.reader.Read();
                this.edmReferences.Add(result);
                return;
            }

            this.reader.Read();
            while (this.reader.NodeType != XmlNodeType.EndElement)
            {
                while (this.reader.NodeType == XmlNodeType.Whitespace && this.reader.Read())
                { // read white spaces. can be an extension method.
                }

                if (this.reader.NodeType != XmlNodeType.Element)
                {
                    break;
                }

                if (this.reader.LocalName == CsdlConstants.Element_Include)
                {
                    // parse: <edmx:Include Alias="IoTDeviceModel" Namespace="Microsoft.IntelligentSystems.DeviceModel.Vocabulary.V1"/>
                    IEdmInclude tmp = new EdmInclude(this.GetAttributeValue(null, CsdlConstants.Attribute_Alias), this.GetAttributeValue(null, CsdlConstants.Attribute_Namespace));
                    result.AddInclude(tmp);
                }
                else if (this.reader.LocalName == CsdlConstants.Element_IncludeAnnotations)
                {
                    // parse: <edmx:IncludeAnnotations TermNamespace="org.example.hcm" Qualifier="Tablet" TargetNamespace="com.contoso.Person" />
                    IEdmIncludeAnnotations tmp = new EdmIncludeAnnotations(this.GetAttributeValue(null, CsdlConstants.Attribute_TermNamespace), this.GetAttributeValue(null, CsdlConstants.Attribute_Qualifier), this.GetAttributeValue(null, CsdlConstants.Attribute_TargetNamespace));
                    result.AddIncludeAnnotations(tmp);
                }
                else if (this.reader.LocalName == CsdlConstants.Element_Annotation)
                {
                    this.reader.Skip();
                    this.RaiseError(EdmErrorCode.UnexpectedXmlElement, Edm.Strings.XmlParser_UnexpectedElement(this.reader.LocalName));
                    continue;
                }
                else
                {
                    this.RaiseError(EdmErrorCode.UnexpectedXmlElement, Edm.Strings.XmlParser_UnexpectedElement(this.reader.LocalName));
                }

                if (!this.reader.IsEmptyElement)
                {
                    this.reader.Read();
                    while (this.reader.NodeType == XmlNodeType.Whitespace && this.reader.Read())
                    { // read white spaces. can be an extension method.
                    }

                    Debug.Assert(this.reader.NodeType == XmlNodeType.EndElement, "The XmlReader should be at the end of element");
                }

                this.reader.Read();
            }

            Debug.Assert(this.reader.NodeType == XmlNodeType.EndElement, "The XmlReader should be at the end of element");
            this.reader.Read();
            this.edmReferences.Add(result);
        }

        private void ParseSchemaElement()
        {
            Debug.Assert(this.reader.LocalName == CsdlConstants.Element_Schema, "Must call ParseCsdlSchemaElement on Schema Element");

            XmlReaderSettings settings = new XmlReaderSettings();
            IXmlLineInfo lineInfo = this.reader as IXmlLineInfo;
            if (lineInfo != null && lineInfo.HasLineInfo())
            {
                settings.LineNumberOffset = lineInfo.LineNumber - 1;
                settings.LinePositionOffset = lineInfo.LinePosition - 2;
            }

            using (StringReader sr = new StringReader(this.reader.ReadOuterXml()))
            {
                using (XmlReader xr = XmlReader.Create(sr, settings))
                {
                    this.csdlParser.AddReader(xr, this.source);
                }
            }
        }

        private void RaiseEmptyFile()
        {
            this.RaiseError(EdmErrorCode.EmptyFile, Edm.Strings.XmlParser_EmptySchemaTextReader);
        }

        private CsdlLocation Location()
        {
            int lineNumber = 0;
            int linePosition = 0;

            IXmlLineInfo xmlLineInfo = this.reader as IXmlLineInfo;
            if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
            {
                lineNumber = xmlLineInfo.LineNumber;
                linePosition = xmlLineInfo.LinePosition;
            }

            return new CsdlLocation(this.source, lineNumber, linePosition);
        }

        private void RaiseError(EdmErrorCode errorCode, string errorMessage)
        {
            this.errors.Add(new EdmError(this.Location(), errorCode, errorMessage));
        }
    }
}

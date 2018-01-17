//---------------------------------------------------------------------
// <copyright file="XmlDocumentParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.Parsing.Common
{
    /// <summary>
    /// Base class for parsers of XML documents
    /// </summary>
    internal abstract class XmlDocumentParser
    {
        private readonly string docPath;
        private readonly Stack<ElementScope> currentBranch = new Stack<ElementScope>();

        private XmlReader reader;
        private IXmlLineInfo xmlLineInfo;
        private List<EdmError> errors;
        private StringBuilder currentText;
        private CsdlLocation currentTextLocation;
        private ElementScope currentScope;

        protected XmlDocumentParser(XmlReader underlyingReader, string documentPath)
        {
            this.reader = underlyingReader;
            this.docPath = documentPath;
            this.errors = new List<EdmError>();
        }

        internal string DocumentPath
        {
            get { return this.docPath; }
        }

        internal string DocumentNamespace
        {
            get; private set;
        }

        internal Version DocumentVersion
        {
            get; private set;
        }

        internal CsdlLocation DocumentElementLocation
        {
            get; private set;
        }

        internal bool HasErrors
        {
            get; private set;
        }

        internal XmlElementValue Result
        {
            get; private set;
        }

        internal CsdlLocation Location
        {
            get
            {
                int lineNumber = 0;
                int linePosition = 0;

                if (this.xmlLineInfo != null && this.xmlLineInfo.HasLineInfo())
                {
                    lineNumber = this.xmlLineInfo.LineNumber;
                    linePosition = this.xmlLineInfo.LinePosition;
                }

                return new CsdlLocation(DocumentPath, lineNumber, linePosition);
            }
        }

        internal IEnumerable<EdmError> Errors
        {
            get { return this.errors; }
        }

        private bool IsTextNode
        {
            get
            {
                switch (this.reader.NodeType)
                {
                    case XmlNodeType.CDATA:
                    case XmlNodeType.Text:
                    case XmlNodeType.SignificantWhitespace:
                        return true;

                    default:
                        return false;
                }
            }
        }

        internal void ParseDocumentElement()
        {
            Debug.Assert(this.DocumentNamespace == null && this.xmlLineInfo == null, "Calling MoveToDocumentElement more than once?");

            this.reader = this.InitializeReader(this.reader);
            this.xmlLineInfo = this.reader as IXmlLineInfo;

            // To make life simpler, we skip down to the first/root element, unless we're
            // already there
            if (this.reader.NodeType != XmlNodeType.Element)
            {
                while (this.reader.Read() && this.reader.NodeType != XmlNodeType.Element)
                {
                }
            }

            // There must be a root element for all current artifacts
            if (this.reader.EOF)
            {
                this.ReportEmptyFile();
                return;
            }

            // The root element must be in an expected namespace that maps to a version of the input artifact type.
            this.DocumentNamespace = this.reader.NamespaceURI;
            Version discoveredVersion;
            string[] expectedNamespaces;
            if (this.TryGetDocumentVersion(this.DocumentNamespace, out discoveredVersion, out expectedNamespaces))
            {
                this.DocumentVersion = discoveredVersion;
            }
            else
            {
                this.ReportUnexpectedRootNamespace(this.reader.LocalName, this.DocumentNamespace, expectedNamespaces);
                return;
            }

            this.DocumentElementLocation = this.Location;

            // At this point the root element is in one of the expected namespaces but may not be the expected root element for that namespace
            bool emptyElement = this.reader.IsEmptyElement;
            XmlElementInfo rootElement = this.ReadElement(this.reader.LocalName, this.DocumentElementLocation);
            XmlElementParser currentParser;
            if (!this.TryGetRootElementParser(this.DocumentVersion, rootElement, out currentParser))
            {
                this.ReportUnexpectedRootElement(rootElement.Location, rootElement.Name, this.DocumentNamespace);
                return;
            }

            this.BeginElement(currentParser, rootElement);
            if (emptyElement)
            {
                this.EndElement();
            }
            else
            {
                this.Parse();
            }
        }

        protected void ReportError(CsdlLocation errorLocation, EdmErrorCode errorCode, string errorMessage)
        {
            this.errors.Add(new EdmError(errorLocation, errorCode, errorMessage));
            this.HasErrors = true;
        }

        protected abstract XmlReader InitializeReader(XmlReader inputReader);

        protected abstract bool TryGetDocumentVersion(string xmlNamespaceName, out Version version, out string[] expectedNamespaces);

        protected abstract bool TryGetRootElementParser(Version artifactVersion, XmlElementInfo rootElement, out XmlElementParser parser);

        protected virtual bool IsOwnedNamespace(string namespaceName)
        {
            return this.DocumentNamespace.EqualsOrdinal(namespaceName);
        }

        protected virtual XmlElementParser<TResult> Element<TResult>(string elementName, Func<XmlElementInfo, XmlElementValueCollection, TResult> parserFunc, params XmlElementParser[] childParsers)
        {
            return XmlElementParser.Create(elementName, parserFunc, childParsers, null);
        }

        private void Parse()
        {
            Debug.Assert(this.DocumentNamespace != null && !this.HasErrors, "Calling Parse when MoveToDocumentElement failed?");
            Debug.Assert(this.Result == null, "Calling Parse more than once?");

            while (this.currentBranch.Count > 0 &&
                this.reader.Read())
            {
                this.ProcessNode();
            }

            if (this.reader.EOF)
            {
                Debug.Assert(!(this.currentBranch.Count > 0), "XmlParser has consumed tags in an imbalanced fashion");
            }
            else
            {
                // this forces the reader to look beyond this top
                // level element, and complain if there is another one.
                this.reader.Read();
            }
        }

        private void EndElement()
        {
            ElementScope scope = this.currentBranch.Pop();
            this.currentScope = this.currentBranch.Count > 0 ? this.currentBranch.Peek() : null;

            XmlElementParser parser = scope.Parser;
            XmlElementValue resultValue = parser.Parse(scope.Element, scope.ChildValues);

            if (resultValue != null)
            {
                if (this.currentScope != null)
                {
                    this.currentScope.AddChildValue(resultValue);
                }
                else
                {
                    this.Result = resultValue;
                }
            }

            foreach (var unused in scope.Element.Attributes.Unused)
            {
                // there's no handler for (namespace,name) and there wasn't a validation error.
                // Report an error of our own if the node is in no namespace or if it is in one of our xml schemas target namespace.
                this.ReportUnexpectedAttribute(unused.Location, unused.Name);
            }

            // For text nodes, one may be expected but additional text should cause an error.
            var textNodes = scope.ChildValues.Where(v => v.IsText);
            var unusedText = textNodes.Where(t => !t.IsUsed);
            if (unusedText.Any())
            {
                XmlTextValue firstInvalidText;
                if (unusedText.Count() == textNodes.Count())
                {
                    // Text is not expected at all for this element
                    firstInvalidText = (XmlTextValue)textNodes.First();
                }
                else
                {
                    // Additional text was unexpected
                    firstInvalidText = (XmlTextValue)unusedText.First();
                }

                this.ReportTextNotAllowed(firstInvalidText.Location, firstInvalidText.Value);
            }

            // If any elements were unused, the csdl is not properly formed. This could be a result of an entirely unexpected element
            // or, it could be an expected but superfluous element.
            // Consider:
            // <ReferentialConstraint>
            //     <Principal>... </Principal>
            //     <Dependent>... </Dependent>
            //     <Principal>... </Principal>
            // </ReferentialConstraint>
            //
            // The second occurrence of 'Principal' will be successfully parsed, but the element parser for ReferentialConstraint will not use its value because only the first occurence is expected.
            // This will also catch if only a single type reference (Collection, EntityReference) element was expected but multiple are provided
            foreach (var unusedChildValue in scope.ChildValues.Where(v => !v.IsText && !v.IsUsed))
            {
                this.ReportUnusedElement(unusedChildValue.Location, unusedChildValue.Name);
            }
        }

        private void BeginElement(XmlElementParser elementParser, XmlElementInfo element)
        {
            ElementScope newScope = new ElementScope(elementParser, element);
            this.currentBranch.Push(newScope);
            this.currentScope = newScope;
        }

        private void ProcessNode()
        {
            Debug.Assert(!this.reader.EOF, "ProcessNode should not be called after reader reaches EOF");

            // If this is a text node, accumulate the text and return so that sequences of text nodes are coalesced into a single text value.
            if (this.IsTextNode)
            {
                if (this.currentText == null)
                {
                    this.currentText = new StringBuilder();
                    this.currentTextLocation = this.Location;
                }

                this.currentText.Append(this.reader.Value);
                return;
            }

            // If this is not a text node and text has been accumulated, set the text on the current parser before control moves to the parser for this new sub-element.
            if (this.currentText != null)
            {
                string textValue = this.currentText.ToString();
                CsdlLocation textLocation = this.currentTextLocation;
                this.currentText = null;
                this.currentTextLocation = default(CsdlLocation);

                if (!EdmUtil.IsNullOrWhiteSpaceInternal(textValue) &&  !string.IsNullOrEmpty(textValue))
                {
                    this.currentScope.AddChildValue(new XmlTextValue(textLocation, textValue));
                }
            }

            switch (this.reader.NodeType)
            {
                // we ignore these childless elements
                case XmlNodeType.Whitespace:
                case XmlNodeType.XmlDeclaration:
                case XmlNodeType.Comment:
                case XmlNodeType.Notation:
                case XmlNodeType.ProcessingInstruction:
                    {
                        return;
                    }

                // we ignore these elements that can have children
                case XmlNodeType.DocumentType:
                case XmlNodeType.EntityReference:
                    {
                        this.reader.Skip();
                        return;
                    }

                case XmlNodeType.Element:
                    {
                        this.ProcessElement();
                        return;
                    }

                case XmlNodeType.EndElement:
                    {
                        this.EndElement();
                        return;
                    }

                default:
                    {
                        this.ReportUnexpectedNodeType(this.reader.NodeType);
                        this.reader.Skip();
                        return;
                    }
            }
        }

        private void ProcessElement()
        {
            bool emptyElement = this.reader.IsEmptyElement;
            string elementNamespace = this.reader.NamespaceURI;
            string elementName = this.reader.LocalName;

            if (elementNamespace == this.DocumentNamespace)
            {
                XmlElementParser newParser;

                if (!this.currentScope.Parser.TryGetChildElementParser(elementName, out newParser))
                {
                    // Don't error on unexpected annotations, just ignore
                    if (elementName != CsdlConstants.Element_Annotation)
                    {
                        this.ReportUnexpectedElement(this.Location, this.reader.Name);
                    }

                    if (!emptyElement)
                    {
                        int depth = reader.Depth;
                        do
                        {
                            reader.Read();
                        }
                        while (reader.Depth > depth);
                    }

                    return;
                }

                XmlElementInfo newElement = this.ReadElement(elementName, this.Location);
                this.BeginElement(newParser, newElement);
                if (emptyElement)
                {
                    this.EndElement();
                }
            }
            else
            {
                // This element is not in the expected XML namespace for this artifact.
                // we need to report an error if the namespace for this element is a target namespace for the xml schemas we are parsing against.
                // otherwise we assume that this is either a valid 'any' element or that the xsd validator has generated an error
                if (string.IsNullOrEmpty(elementNamespace) || this.IsOwnedNamespace(elementNamespace))
                {
                    // Don't error on unexpected annotations, just ignore
                    if (elementName != CsdlConstants.Element_Annotation)
                    {
                        this.ReportUnexpectedElement(this.Location, this.reader.Name);
                    }

                    this.reader.Skip();
                }
                else
                {
                    XmlReader elementReader = this.reader.ReadSubtree();
                    elementReader.MoveToContent();
                    string annotationValue = elementReader.ReadOuterXml();
                    this.currentScope.Element.AddAnnotation(new XmlAnnotationInfo(this.Location, elementNamespace, elementName, annotationValue, false));
                }
            }
        }

        private XmlElementInfo ReadElement(string elementName, CsdlLocation elementLocation)
        {
            Debug.Assert(this.reader.NodeType == XmlNodeType.Element, "Retrieving attributes from non-element node?");

            List<XmlAttributeInfo> ownedAttributes = null;
            List<XmlAnnotationInfo> annotationAttributes = null;

            bool hasAttributes = this.reader.MoveToFirstAttribute();
            while (hasAttributes)
            {
                string attributeNamespace = this.reader.NamespaceURI;
                if (string.IsNullOrEmpty(attributeNamespace) || attributeNamespace.EqualsOrdinal(this.DocumentNamespace))
                {
                    if (ownedAttributes == null)
                    {
                        ownedAttributes = new List<XmlAttributeInfo>();
                    }

                    ownedAttributes.Add(new XmlAttributeInfo(this.reader.LocalName, this.reader.Value, this.Location));
                }
                else
                {
                    if (this.IsOwnedNamespace(attributeNamespace))
                    {
                        this.ReportUnexpectedAttribute(this.Location, this.reader.Name);
                    }
                    else
                    {
                        if (annotationAttributes == null)
                        {
                            annotationAttributes = new List<XmlAnnotationInfo>();
                        }

                        annotationAttributes.Add(new XmlAnnotationInfo(this.Location, this.reader.NamespaceURI, this.reader.LocalName, this.reader.Value, true));
                    }
                }

                hasAttributes = this.reader.MoveToNextAttribute();
            }

            return new XmlElementInfo(elementName, elementLocation, ownedAttributes, annotationAttributes);
        }

        #region Errors

        private void ReportEmptyFile()
        {
            string errorMessage = this.DocumentPath == null ?
                Edm.Strings.XmlParser_EmptySchemaTextReader :
                Edm.Strings.XmlParser_EmptyFile(this.DocumentPath);

            this.ReportError(
                this.Location,
                EdmErrorCode.EmptyFile,
                errorMessage);
        }

        private void ReportUnexpectedRootNamespace(string elementName, string namespaceUri, string[] expectedNamespaces)
        {
            string expectedNamespacesString = string.Join(", ", expectedNamespaces);
            string errorMessage = string.IsNullOrEmpty(namespaceUri)
                    ? Edm.Strings.XmlParser_UnexpectedRootElementNoNamespace(expectedNamespacesString)
                    : Edm.Strings.XmlParser_UnexpectedRootElementWrongNamespace(namespaceUri, expectedNamespacesString);
            this.ReportError(
                this.Location,
                EdmErrorCode.UnexpectedXmlElement,
                errorMessage);
        }

        private void ReportUnexpectedRootElement(CsdlLocation elementLocation, string elementName, string expectedNamespace)
        {
            Debug.Assert(!string.IsNullOrEmpty(expectedNamespace), "UnexpectedRootElementInExpectedNamespace requires a valid expected namespace");
            this.ReportError(elementLocation, EdmErrorCode.UnexpectedXmlElement, Edm.Strings.XmlParser_UnexpectedRootElement(elementName, CsdlConstants.Element_Schema));
        }

        private void ReportUnexpectedAttribute(CsdlLocation errorLocation, string attributeName)
        {
            this.ReportError(errorLocation, EdmErrorCode.UnexpectedXmlAttribute, Edm.Strings.XmlParser_UnexpectedAttribute(attributeName));
        }

        private void ReportUnexpectedNodeType(XmlNodeType nodeType)
        {
            this.ReportError(this.Location, EdmErrorCode.UnexpectedXmlNodeType, Edm.Strings.XmlParser_UnexpectedNodeType(nodeType));
        }

        private void ReportUnexpectedElement(CsdlLocation errorLocation, string elementName)
        {
            this.ReportError(errorLocation, EdmErrorCode.UnexpectedXmlElement, Edm.Strings.XmlParser_UnexpectedElement(elementName));
        }

        private void ReportUnusedElement(CsdlLocation errorLocation, string elementName)
        {
            this.ReportError(errorLocation, EdmErrorCode.UnexpectedXmlElement, Edm.Strings.XmlParser_UnusedElement(elementName));
        }

        private void ReportTextNotAllowed(CsdlLocation errorLocation, string textValue)
        {
            this.ReportError(errorLocation, EdmErrorCode.TextNotAllowed, Edm.Strings.XmlParser_TextNotAllowed(textValue));
        }

        #endregion

        #region Scope Management

        private class ElementScope
        {
            private static readonly IList<XmlElementValue> EmptyValues = new System.Collections.ObjectModel.ReadOnlyCollection<XmlElementValue>(new XmlElementValue[] { });

            private List<XmlElementValue> childValues;

            internal ElementScope(XmlElementParser parser, XmlElementInfo element)
            {
                this.Parser = parser;
                this.Element = element;
            }

            internal XmlElementParser Parser
            {
                get;
                private set;
            }

            internal XmlElementInfo Element
            {
                get;
                private set;
            }

            internal IList<XmlElementValue> ChildValues
            {
                get { return this.childValues ?? EmptyValues; }
            }

            internal void AddChildValue(XmlElementValue value)
            {
                if (this.childValues == null)
                {
                    this.childValues = new List<XmlElementValue>();
                }

                this.childValues.Add(value);
            }
        }

        #endregion
    }

    internal abstract class XmlDocumentParser<TResult> : XmlDocumentParser
    {
        internal XmlDocumentParser(XmlReader underlyingReader, string documentPath)
            : base(underlyingReader, documentPath)
        {
        }

        internal new XmlElementValue<TResult> Result
        {
            get
            {
                if (base.Result != null)
                {
                    Debug.Assert(base.Result is XmlElementValue<TResult> && base.Result.UntypedValue is TResult, "DocumentParser<TResult> without ElementParser<TResult> as root element parser?");
                    return (XmlElementValue<TResult>)base.Result;
                }

                return null;
            }
        }

        protected override sealed bool TryGetRootElementParser(Version artifactVersion, XmlElementInfo rootElement, out XmlElementParser parser)
        {
            XmlElementParser<TResult> typedParser;
            if (this.TryGetDocumentElementParser(artifactVersion, rootElement, out typedParser))
            {
                parser = typedParser;
                return true;
            }

            parser = null;
            return false;
        }

        protected abstract bool TryGetDocumentElementParser(Version artifactVersion, XmlElementInfo rootElement, out XmlElementParser<TResult> parser);
    }
}

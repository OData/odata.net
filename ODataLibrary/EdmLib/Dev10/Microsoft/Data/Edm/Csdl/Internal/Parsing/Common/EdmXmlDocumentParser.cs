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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.Parsing.Common
{
    internal abstract class EdmXmlDocumentParser<TResult> : XmlDocumentParser<TResult>
    {
        private HashSetInternal<string> parseableXmlNamespaces;
        private readonly Stack<List<KeyValuePair<string, CsdlLocation>>> currentProps = new Stack<List<KeyValuePair<string, CsdlLocation>>>();
        private readonly Stack<XmlElementInfo> elementStack = new Stack<XmlElementInfo>();
        protected XmlElementInfo currentElement;

        internal EdmXmlDocumentParser(string artifactLocation, XmlReader reader)
            : base(reader, artifactLocation)
        {
        }

        internal abstract IEnumerable<KeyValuePair<Version, string>> SupportedVersions { get; }

        protected override XmlReader InitializeReader(XmlReader reader)
        {
            XmlReaderSettings readerSettings = new XmlReaderSettings
                                                   {
                                                       CheckCharacters = true,
                                                       CloseInput = false,
                                                       IgnoreWhitespace = true,
                                                       ConformanceLevel = ConformanceLevel.Auto,
                                                       IgnoreComments = true,
                                                       IgnoreProcessingInstructions = true,
#if !ORCAS
                                                       DtdProcessing = DtdProcessing.Prohibit
#endif
                                                   };

            // user specified a stream to read from, read from it.
            // The Uri is just used to identify the stream in errors.
            return XmlReader.Create(reader, readerSettings);
        }

        protected override bool TryGetDocumentVersion(string xmlNamespaceName, out Version version, out string[] expectedNamespaces)
        {
            expectedNamespaces = this.SupportedVersions.Select(v => v.Value).ToArray();
            version = this.SupportedVersions.Where(v => v.Value == xmlNamespaceName).Select(v => v.Key).FirstOrDefault();
            return version != null;
        }

        protected override bool IsOwnedNamespace(string namespaceName)
        {
            return this.IsEdmNamespace(namespaceName);
        }

        private bool IsEdmNamespace(string xmlNamespaceUri)
        {
            Debug.Assert(!string.IsNullOrEmpty(xmlNamespaceUri), "Ensure namespace URI is not null or empty before calling IsEdmNamespace");

            if (this.parseableXmlNamespaces == null)
            {
                this.parseableXmlNamespaces = new HashSetInternal<string>();
                foreach (var schemaResource in XmlSchemaResource.GetMetadataSchemaResourceMap(this.DocumentVersion).Values)
                {
                    this.parseableXmlNamespaces.Add(schemaResource.NamespaceUri);
                }
            }

            return this.parseableXmlNamespaces.Contains(xmlNamespaceUri);
        }

        #region Utility Methods for derived document parsers

        internal XmlAttributeInfo GetOptionalAttribute(XmlElementInfo element, string attributeName)
        {
            var attr = element.Attributes[attributeName];
            if (attr.IsMissing || !attr.IsEmpty)
            {
                return attr;
            }
            
            // TODO_EDM: SOM raised invalid name for this case, which does not seem correct now.
            this.ReportError(attr.Location, EdmErrorCode.InvalidName,
                             Edm.Strings.XmlParser_InvalidName(attributeName, attr.Value));
            return XmlAttributeInfo.Missing;
        }

        internal XmlAttributeInfo GetRequiredAttribute(XmlElementInfo element, string attributeName)
        {
            var attr = element.Attributes[attributeName];
            if (attr.IsMissing)
            {
                this.ReportError(element.Location, EdmErrorCode.MissingAttribute, Edm.Strings.XmlParser_MissingAttribute(attributeName, element.Name));
                return attr;
            }
            
            if (attr.IsEmpty)
            {
                // TODO_SOM: SOM raised invalid name for this case, which does not seem correct now.
                this.ReportError(attr.Location, EdmErrorCode.InvalidName, Edm.Strings.XmlParser_InvalidName(attributeName, attr.Value));
                return XmlAttributeInfo.Missing;
            }

            return attr;
        }

        protected XmlElementParser<TItem> CsdlElement<TItem>(string elementName, Func<XmlElementInfo, XmlElementValueCollection, TItem> initializer, params XmlElementParser[] childParsers)
           where TItem : class
        {
            return Element<TItem>(elementName, (element, childValues) =>
            {
                BeginItem(element);
                TItem result = initializer(element, childValues);
                AnnotateItem(result, childValues);
                EndItem();

                return result;
            },
            childParsers);
        }

        protected void BeginItem(XmlElementInfo element)
        {
            this.elementStack.Push(element);
            this.currentElement = element;
            this.currentProps.Push(new List<KeyValuePair<string, CsdlLocation>>());
        }

        protected abstract void AnnotateItem(object result, XmlElementValueCollection childValues);
        
        protected void EndItem()
        {
            this.elementStack.Pop();
            this.currentElement = this.elementStack.Count == 0 ? null : this.elementStack.Peek();
            this.currentProps.Pop();
        }

        protected int? OptionalInteger(string attributeName)
        {
            XmlAttributeInfo attr = this.GetOptionalAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                int? value = attr.ToInt32();
                if (value == null)
                {
                    this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidInteger, Edm.Strings.CsdlParser_InvalidInteger(attr.Value));
                }

                return value;
            }

            return null;
        }

        protected long? OptionalLong(string attributeName)
        {
            XmlAttributeInfo attr = this.GetOptionalAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                long? value = attr.ToInt64();
                if (value == null)
                {
                    this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidLong, Edm.Strings.CsdlParser_InvalidLong(attr.Value));
                }

                return value;
            }

            return null;
        }

        protected int? OptionalSrid(string attributeName, int defaultSrid)
        {
            XmlAttributeInfo attr = this.GetOptionalAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                int? srid;
                if (attr.Value.EqualsOrdinalIgnoreCase(CsdlConstants.Value_SridVariable))
                {
                    srid = null;
                }
                else
                {
                    srid = attr.ToInt32();
                    if (srid == null)
                    {
                        this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidSrid, Edm.Strings.CsdlParser_InvalidSrid(attr.Value));
                    }
                }

                return srid;
            }

            return defaultSrid;
        }

        protected int? OptionalMaxLength(string attributeName)
        {
            XmlAttributeInfo attr = this.GetOptionalAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                int? value = attr.ToInt32();
                if (value == null)
                {
                    this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidMaxLength, Edm.Strings.CsdlParser_InvalidMaxLength(attr.Value));
                }

                return value;
            }

            return null;
        }

        protected EdmFunctionParameterMode? OptionalFunctionParameterMode(string attributeName)
        {
            XmlAttributeInfo attr = this.GetOptionalAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                switch (attr.Value)
                {
                    case CsdlConstants.Value_ModeIn:
                        return EdmFunctionParameterMode.In;
                    case CsdlConstants.Value_ModeInOut:
                        return EdmFunctionParameterMode.InOut;
                    case CsdlConstants.Value_ModeOut:
                        return EdmFunctionParameterMode.Out;
                    default:
                        this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidParameterMode, Edm.Strings.CsdlParser_InvalidParameterMode(attr.Value));
                        return EdmFunctionParameterMode.None;
                }
            }

            return null;
        }

        protected EdmConcurrencyMode? OptionalConcurrencyMode(string attributeName)
        {
            XmlAttributeInfo attr = this.GetOptionalAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                switch (attr.Value)
                {
                    case CsdlConstants.Value_None:
                        return EdmConcurrencyMode.None;
                    case CsdlConstants.Value_Fixed:
                        return EdmConcurrencyMode.Fixed;
                    default:
                        this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidConcurrencyMode, Edm.Strings.CsdlParser_InvalidConcurrencyMode(attr.Value));
                        break;
                }
            }

            return null;
        }

        protected EdmAssociationMultiplicity RequiredMultiplicity(string attributeName)
        {
            XmlAttributeInfo attr = this.GetRequiredAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                switch (attr.Value)
                {
                    case CsdlConstants.Value_EndRequired:
                        return EdmAssociationMultiplicity.One;
                    case CsdlConstants.Value_EndOptional:
                        return EdmAssociationMultiplicity.ZeroOrOne;
                    case CsdlConstants.Value_EndMany:
                        return EdmAssociationMultiplicity.Many;
                    default:
                        this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidMultiplicity, Edm.Strings.CsdlParser_InvalidMultiplicity(attr.Value));
                        break;
                }
            }

           return EdmAssociationMultiplicity.One;
        }

        protected EdmOnDeleteAction RequiredOnDeleteAction(string attributeName)
        {
            XmlAttributeInfo attr = this.GetRequiredAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                switch (attr.Value)
                {
                    case CsdlConstants.Value_None:
                        return EdmOnDeleteAction.None;
                    case CsdlConstants.Value_Cascade:
                        return EdmOnDeleteAction.Cascade;
                    default:
                        this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidOperation, Edm.Strings.CsdlParser_InvalidBoolean(attr.Value));
                        break;
                }
            }

            return EdmOnDeleteAction.None;
        }

        protected bool? OptionalBoolean(string attributeName)
        {
            XmlAttributeInfo attr = this.GetOptionalAttribute(this.currentElement, attributeName);
            if (!attr.IsMissing)
            {
                bool? value = attr.ToBoolean();
                if (value == null)
                {
                    this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidBoolean, Edm.Strings.CsdlParser_InvalidDeleteAction(attr.Value));
                }

                return value;
            }

            return null;
        }

        protected string Optional(string attributeName)
        {
            XmlAttributeInfo attr = this.GetOptionalAttribute(this.currentElement, attributeName);
            return !attr.IsMissing ? attr.Value : null;
        }

        protected string Required(string attributeName)
        {
            XmlAttributeInfo attr = this.GetRequiredAttribute(this.currentElement, attributeName);
            return !attr.IsMissing ? attr.Value : string.Empty;
        }
        #endregion
    }
}

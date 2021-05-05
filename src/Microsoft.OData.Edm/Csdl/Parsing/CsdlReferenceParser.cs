//---------------------------------------------------------------------
// <copyright file="CsdlReferenceParser.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Csdl.Parsing.Common;

namespace Microsoft.OData.Edm.Csdl.Parsing
{
    /// <summary>
    /// CSDL document (Edm reference) parser.
    /// </summary>
    internal class CsdlReferenceParser : CsdlDocumentParserBase<CsdlReference>
    {
        internal CsdlReferenceParser(string documentPath, XmlReader reader)
            : base(documentPath, reader)
        {
        }

        protected override bool CheckAnnotationNamespace => false;

        internal override IEnumerable<KeyValuePair<Version, string>> SupportedVersions
        {
            get { return CsdlConstants.SupportedEdmxVersions; }
        }

        protected override bool TryGetDocumentElementParser(Version csdlArtifactVersion, XmlElementInfo rootElement, out XmlElementParser<CsdlReference> parser)
        {
            EdmUtil.CheckArgumentNull(rootElement, "rootElement");
            if (string.Equals(rootElement.Name, CsdlConstants.Element_Reference, StringComparison.Ordinal))
            {
                parser = this.CreateRootElementParser();

                return true;
            }

            parser = null;
            return false;
        }

        private XmlElementParser<CsdlReference> CreateRootElementParser()
        {
            XmlElementParser<CsdlAnnotation> annotationParser = CreateAnnotationParser();

            var rootElementParser =
                //// <Reference>
                CsdlElement<CsdlReference>(CsdlConstants.Element_Reference, this.OnReferenceElement,
                    //// <Include>
                    CsdlElement<CsdlInclude>(CsdlConstants.Element_Include, this.OnIncludeElement,
                        //// <Annotation/>
                        annotationParser),
                    //// </Include>

                    //// <InlcudeAnnotations>
                    CsdlElement<CsdlIncludeAnnotations>(CsdlConstants.Element_IncludeAnnotations, this.OnIncludeAnnotationsElement),
                    //// </InlcudeAnnotations>

                    //// <Annotation/>
                    annotationParser);
                //// </Reference>

            return rootElementParser;
        }

        private CsdlReference OnReferenceElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            // read 'Uri' attribute
            string uri = Required(CsdlConstants.Attribute_Uri);

            return new CsdlReference(uri,
                childValues.ValuesOfType<CsdlInclude>(),
                childValues.ValuesOfType<CsdlIncludeAnnotations>(),
                element.Location);
        }

        private CsdlInclude OnIncludeElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string nameSp = Required(CsdlConstants.Attribute_Namespace);
            string alias = Optional(CsdlConstants.Attribute_Alias);
            return new CsdlInclude(alias, nameSp, element.Location);
        }

        private CsdlIncludeAnnotations OnIncludeAnnotationsElement(XmlElementInfo element, XmlElementValueCollection childValues)
        {
            string termNamespace = Required(CsdlConstants.Attribute_TermNamespace);
            string qualifier = Optional(CsdlConstants.Attribute_Qualifier);
            string targetNamespace = Optional(CsdlConstants.Attribute_TargetNamespace);
            return new CsdlIncludeAnnotations(termNamespace, qualifier, targetNamespace, element.Location);
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="VocabularyDefinitionCsdlGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.Test.OData.Utils.Metadata;

namespace EdmLibTests.FunctionalUtilities
{
    /// <summary>
    /// Generates the vocabulary definition Csdl (EntityType and ValueTerm)
    /// </summary>
    public class VocabularyDefinitionCsdlGenerator
    {
        private Dictionary<string, XElement> namespaceToContents;

        protected Dictionary<EdmVersion, Version> toProductVersionlookup = new Dictionary<EdmVersion, Version>()
        {
            { EdmVersion.V40, Microsoft.OData.Edm.EdmConstants.EdmVersion4 }
        };

        public IEnumerable<XElement> GenerateDefinitionCsdl(EdmVersion edmVersion, IEdmModel definitionModel)
        {
            this.namespaceToContents = new Dictionary<string, XElement>();

            this.GenerateContentsWithoutDefinition(edmVersion, definitionModel);

            XNamespace ns = this.namespaceToContents.First().Value.Name.Namespace;
            foreach (var valueTerm in definitionModel.SchemaElements.OfType<IEdmTerm>())
            {
                XElement schema = this.FindOrCreateCorrespondingSchema(valueTerm.Namespace, ns);

                schema.Add(new XElement(ns + "Term",
                                        new XAttribute("Name", valueTerm.Name),
                                        this.GenerateTypeAttributes(valueTerm.Type)));
            }

            return this.namespaceToContents.Values.Where(fc => fc.HasElements);
        }

        private void GenerateContentsWithoutDefinition(EdmVersion edmVersion, IEdmModel definitionModel)
        {
            IEnumerable<EdmError> errors;
            var contentsWithoutDefinition = this.GetSerializerResult(definitionModel, edmVersion, out errors).Select(XElement.Parse);

            var stripTheseElements = new[] { "Term", "Annotations", "Annotation"};

            foreach (var contents in contentsWithoutDefinition)
            {
                contents.Descendants().Where(e => stripTheseElements.Contains(e.Name.LocalName)).Remove();

                string namespaceName = contents.Attribute("Namespace").Value;
                this.namespaceToContents.Add(namespaceName, contents);
            }
        }

        private XElement FindOrCreateCorrespondingSchema(string namespaceName, XNamespace ns)
        {
            XElement schema;
            if (this.namespaceToContents.ContainsKey(namespaceName))
            {
                schema = this.namespaceToContents[namespaceName];
            }
            else
            {
                schema = new XElement(ns + "Schema", new XAttribute("Namespace", namespaceName));
                this.namespaceToContents.Add(namespaceName, schema);
            }

            return schema;
        }

        private IEnumerable<XObject> GenerateTypeAttributes(IEdmTypeReference typeReference)
        {
            var typeAttributes = new List<XObject>();
            if (typeReference.IsCollection())
            {
                var elementType = ((IEdmCollectionTypeReference)typeReference).ElementType();
                typeAttributes.Add(new XAttribute("Type", "Collection(" + elementType.FullName() + ")" )); 
                AddFacetAttributes(elementType, typeAttributes);
            }
            else
            {
                typeAttributes.Add(new XAttribute("Type", typeReference.FullName()));
                AddFacetAttributes(typeReference, typeAttributes);
            }

            return typeAttributes;
        }

        private static void AddFacetAttributes(IEdmTypeReference typeReference, IList<XObject> typeAttributes)
        {
            if (!typeReference.IsCollection() && !typeReference.IsNullable)
            {
                typeAttributes.Add(new XAttribute("Nullable", false));
            }

            if (typeReference.IsBinary())
            {
                var binaryTypeReference = (IEdmBinaryTypeReference)typeReference;
                if (binaryTypeReference.MaxLength.HasValue)
                {
                    typeAttributes.Add(new XAttribute("MaxLength", binaryTypeReference.MaxLength.Value));
                }
            }

            if (typeReference.IsString())
            {
                var stringTypeReference = (IEdmStringTypeReference)typeReference;
                if (stringTypeReference.MaxLength.HasValue)
                {
                    typeAttributes.Add(new XAttribute("MaxLength", stringTypeReference.MaxLength.Value));
                }

                if (stringTypeReference.IsUnicode.HasValue && !stringTypeReference.IsUnicode.Value)
                {
                    typeAttributes.Add(new XAttribute("Unicode", false));
                }
            }

            if (typeReference.IsTemporal())
            {
                var temporaralTypeReference = (IEdmTemporalTypeReference)typeReference;
                if (temporaralTypeReference.Precision.HasValue && temporaralTypeReference.Precision != 0)
                {
                    typeAttributes.Add(new XAttribute("Precision", temporaralTypeReference.Precision.Value));
                }
            }

            if (typeReference.IsDecimal())
            {
                var decimalTypeReference = (IEdmDecimalTypeReference)typeReference;
                if (decimalTypeReference.Precision.HasValue)
                {
                    typeAttributes.Add(new XAttribute("Precision", decimalTypeReference.Precision));
                }

                if (decimalTypeReference.Scale.HasValue && decimalTypeReference.Scale != 0)
                {
                    typeAttributes.Add(new XAttribute("Scale", decimalTypeReference.Scale.Value));
                }
            }

            if (typeReference.IsSpatial())
            {
                var spatialTypeReference = (IEdmSpatialTypeReference)typeReference;
                if (spatialTypeReference.SpatialReferenceIdentifier.HasValue)
                {
                    typeAttributes.Add(new XAttribute("SRID", spatialTypeReference.SpatialReferenceIdentifier.Value));
                }
            }
        }

        protected IEnumerable<string> GetSerializerResult(IEdmModel edmModel, EdmVersion edmVersion, out IEnumerable<EdmError> errors)
        {
            // TODO: figure out the best way for multiple schemas
            List<StringBuilder> stringBuilders = new List<StringBuilder>();
            List<XmlWriter> xmlWriters = new List<XmlWriter>();
            edmModel.SetEdmVersion(this.toProductVersionlookup[edmVersion]);
            edmModel.TryWriteSchema(
                s =>
                {
                    stringBuilders.Add(new StringBuilder());
                    xmlWriters.Add(XmlWriter.Create(stringBuilders.Last()));

                    return xmlWriters.Last();
                }, out errors);

            for (int i = 0; i < stringBuilders.Count; i++)
            {
                xmlWriters[i].Close();
            }

            List<string> strings = new List<string>();
            foreach (var sb in stringBuilders)
            {
                strings.Add(sb.ToString());
            }
            return strings;
        }
    }
}

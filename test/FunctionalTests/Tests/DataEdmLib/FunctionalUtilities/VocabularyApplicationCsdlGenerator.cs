//---------------------------------------------------------------------
// <copyright file="VocabularyApplicationCsdlGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.OData.Utils.Metadata;

    /// <summary>
    /// Generates the vocabulary annotations (application) in a model into a separate Csdl
    /// </summary>
    public class VocabularyApplicationCsdlGenerator
    {
        public XElement GenerateApplicationCsdl(EdmVersion edmVersion, IEdmModel applicationModel)
        {
            XNamespace ns = this.DetermineXmlNamespace(edmVersion);
            var schema = new XElement(ns + "Schema", new XAttribute("Namespace", "Application.NS1"));

            IEnumerable<IEdmVocabularyAnnotatable> possiblyAnnotated = applicationModel.SchemaElements.OfType<IEdmEntityType>().Cast<IEdmVocabularyAnnotatable>()
                .Concat(applicationModel.SchemaElements.OfType<IEdmEntityContainer>().Cast<IEdmVocabularyAnnotatable>())
                .Concat(applicationModel.SchemaElements.OfType<IEdmEntityContainer>().SelectMany(c => c.EntitySets()).Cast<IEdmVocabularyAnnotatable>());

            foreach (var annotated in possiblyAnnotated.Where(e => e.VocabularyAnnotations(applicationModel).Any()))
            {
                var valueAnnotations = annotated.VocabularyAnnotations(applicationModel);
                schema.Add(new XElement(
                                ns + "Annotations",
                                new XAttribute("Target", this.GetTargetPathFor(annotated, applicationModel)),
                                this.GenerateVocabularyAnnotations(ns, valueAnnotations)));
            }

            return schema;
        }

        private string GetTargetPathFor(IEdmVocabularyAnnotatable annotated, IEdmModel model)
        {
            var withFullName = annotated as IEdmSchemaElement;
            var containerElement = annotated as IEdmEntityContainerElement;
            if (withFullName != null)
            {
                return withFullName.FullName();
            }
            else if (containerElement != null)
            {
                var parentContainer = model.EntityContainer;
                return parentContainer.FullName() + "/" + containerElement.Name;
            }

            throw new System.InvalidOperationException(string.Format("unexpected element (not SchemaElement, Container, or ContainerElement): {0}", annotated));
        }

        private IEnumerable<XElement> GenerateVocabularyAnnotations(XNamespace ns, IEnumerable<IEdmVocabularyAnnotation> valueAnnotations)
        {
            foreach (var va in valueAnnotations)
            {
                yield return new XElement(
                                    ns + "Annotation",
                                    new XAttribute("Term", ((IEdmSchemaElement)va.Term).Namespace + "." + va.Term.Name),
                                    va.Qualifier != null ? new XAttribute("Qualifier", va.Qualifier) : null,
                                    this.GenerateValueExpression(ns, va.Value));
            }
        }

        private XObject GenerateValueExpression(XNamespace ns, IEdmExpression expression)
        {
            // TODO: handle other than constant, other DataTypes
            XObject returnXObject = null;
            switch (expression.ExpressionKind)
            {
                case EdmExpressionKind.Null:
                    returnXObject = new XElement(ns.GetName("Null"));
                    break;
                case EdmExpressionKind.IntegerConstant:
                    var integerConstantExpression = (IEdmIntegerConstantExpression)expression;
                    returnXObject = new XAttribute("Int", integerConstantExpression.Value);
                    break;
                case EdmExpressionKind.StringConstant:
                    var stringConstantExpression = (IEdmStringConstantExpression)expression;
                    returnXObject = new XAttribute("String", stringConstantExpression.Value);
                    break;
                case EdmExpressionKind.Collection:
                    returnXObject = this.GenerateCollectionExpression(ns, (IEdmCollectionExpression)expression);
                    break;
                case EdmExpressionKind.Record:
                    returnXObject = this.GenerateRecordExpression(ns, (IEdmRecordExpression)expression);
                    break;
                default:
                    throw new NotSupportedException();
            }
            return returnXObject;
        }

        private XObject GenerateRecordExpression(XNamespace ns, IEdmRecordExpression record)
        {
            var recordElement = new XElement(ns.GetName("Record"));
            foreach (var property in record.Properties)
            {
                var propertyValueElement = new XElement(ns.GetName("PropertyValue"));
                propertyValueElement.Add(new XAttribute("Property", property.Name));
                propertyValueElement.Add(GenerateValueExpression(ns, property.Value));
                recordElement.Add(propertyValueElement);
            }
            return recordElement;
        }

        private XObject GenerateCollectionExpression(XNamespace ns, IEdmCollectionExpression collection)
        {
            var collectionElements = new XElement(ns.GetName("Collection"));
            foreach (var element in collection.Elements)
            {
                var elementValue = GenerateValueExpression(ns, element);

                if (elementValue is XAttribute)
                {
                    var collectionElement = new XElement(ns.GetName(((XAttribute)elementValue).Name.ToString()));
                    collectionElement.Add(((XAttribute)elementValue).Value);
                    collectionElements.Add(collectionElement);
                }
                else
                {
                    collectionElements.Add(elementValue);
                }
            }
            return collectionElements;
        }

        // TODO: get it from CsdlContentGenerator (or some common place)
        private XNamespace DetermineXmlNamespace(EdmVersion csdlVersion)
        {
            if (csdlVersion == EdmVersion.V40)
            {
                return Microsoft.Test.OData.Utils.Metadata.EdmConstants.EdmOasisNamespace;
            }
            else
            {
                throw new NotSupportedException("CSDL Schema Version is not supported: " + csdlVersion.ToString());
            }
        }

    }
}

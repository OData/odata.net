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

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;

namespace Microsoft.Data.Edm.Csdl.Internal.Parsing
{
    internal static class AnnotationsParser
    {
        public static bool TryParse(IEnumerable<XmlReader> annotations, out IDictionary<string, ExternalAnnotations> annotationsDictionary)
        {
            annotationsDictionary = new Dictionary<string, ExternalAnnotations>();
            foreach (var annotationsReader in annotations)
            {
                if (TryParse(annotationsReader, annotationsDictionary))
                {
                    continue;
                }

                annotationsDictionary = null;
                return false;
            }

            return true;
        }

        private static bool TryParse(XmlReader annotationsReader, IDictionary<string, ExternalAnnotations> annotationsDictionary)
        {
            var xmlDoc = XDocument.Load(annotationsReader);

            const string annotationsNS = "http://schemas.microsoft.com/ado/2011/04/edm/annotations";
            var annotationsQuery = xmlDoc.Elements(XName.Get("Annotations", annotationsNS)).SelectMany(e => e.Elements(XName.Get("Annotation", annotationsNS)));

            foreach (XElement xmlAnnotation in annotationsQuery)
            {
                // Get name and value of the annotation
                string namespaceName;
                string localName;
                if (!TryParseTerm(xmlAnnotation, out namespaceName, out localName))
                {
                    return false;
                }

                IEdmValue value;
                if (!TryParseValue(xmlAnnotation, out value))
                {
                    return false;
                }

                var edmAnnotation = new EdmAnnotation(namespaceName, localName, value);

                // Get type full name edmAnnotation applies to.
                var targetEntityType = xmlAnnotation.Attribute("Target");

                if (targetEntityType == null || EdmUtil.IsNullOrWhiteSpaceInternal(targetEntityType.Value))
                {
                    return false;
                }

                ExternalAnnotations externalAnnotations;
                if (!annotationsDictionary.TryGetValue(targetEntityType.Value, out externalAnnotations))
                {
                    externalAnnotations = new ExternalAnnotations();
                    annotationsDictionary.Add(targetEntityType.Value, externalAnnotations);
                }

                // Get property name edmAnnotation applies to.
                var targetProperty = xmlAnnotation.Attribute("TargetProperty");

                if (targetProperty != null)
                {
                    if (string.IsNullOrEmpty(targetProperty.Value))
                    {
                        return false;
                    }

                    // Add annotation to the property.
                    List<IEdmAnnotation> propertyAnnotations;
                    if (externalAnnotations.PropertyAnnotations == null ||
                        !externalAnnotations.PropertyAnnotations.TryGetValue(targetProperty.Value, out propertyAnnotations))
                    {
                        if (externalAnnotations.PropertyAnnotations == null)
                        {
                            externalAnnotations.PropertyAnnotations = new Dictionary<string, List<IEdmAnnotation>>();
                        }

                        propertyAnnotations = new List<IEdmAnnotation>();
                        externalAnnotations.PropertyAnnotations.Add(targetProperty.Value, propertyAnnotations);
                    }

                    propertyAnnotations.Add(edmAnnotation);
                }
                else
                {
                    // Add annotation to the element.
                    if (externalAnnotations.Annotations == null)
                    {
                        externalAnnotations.Annotations = new List<IEdmAnnotation>();
                    }

                    externalAnnotations.Annotations.Add(edmAnnotation);
                }
            }

            return true;
        }

        private static bool TryParseTerm(XElement annotation, out string namespaceName, out string localName)
        {
            namespaceName = null;
            localName = null;

            var term = annotation.Attribute("Term");

            if (term == null || EdmUtil.IsNullOrWhiteSpaceInternal(term.Value))
            {
                return false;
            }

            var qname = term.Value.Split(':');
            if (qname.Length == 2)
            {
                var ns = annotation.GetNamespaceOfPrefix(qname[0]);
                if (ns == null)
                {
                    return false;
                }

                namespaceName = ns.NamespaceName;
                localName = qname[1];
                return true;
            }

            return false;
        }

        private static bool TryParseValue(XElement xmlAnnotation, out IEdmValue value)
        {
            value = null;

            var propertyValues = new List<TupleInternal<string, IEdmValue>>();
            var annotations = new List<IEdmImmediateValueAnnotation>();
            var collectionElementValues = new List<IEdmValue>();

            foreach (XElement element in xmlAnnotation.Elements())
            {
                if (element.Name.LocalName == "Item" &&
                    element.Name.Namespace == "http://schemas.microsoft.com/ado/2011/04/edm/annotations")
                {
                    IEdmValue elementValue;
                    if (!TryParseValue(element, out elementValue))
                    {
                        return false;
                    }

                    collectionElementValues.Add(elementValue);
                }
                else if (element.Name.Namespace == "http://schemas.microsoft.com/ado/2011/04/edm/annotations/properties")
                {
                    var propertyName = element.Name.LocalName;
                    IEdmValue propertyValue;
                    if (!TryParseValue(element, out propertyValue))
                    {
                        return false;
                    }

                    propertyValues.Add(TupleInternal.Create(propertyName, propertyValue));
                }
                else
                {
                    IEdmValue inlineAnnotationValue;
                    if (!TryParseValue(element, out inlineAnnotationValue))
                    {
                        return false;
                    }

                    annotations.Add(new EdmAnnotation(element.Name.Namespace.NamespaceName, element.Name.LocalName, inlineAnnotationValue));
                }
            }

            if (collectionElementValues.Count > 0)
            {
                var elementType = collectionElementValues.First().Type;
                value = new EdmCollectionValue(new EdmCollectionTypeReference(new EdmCollectionType(elementType, CsdlConstants.Default_IsAtomic), false), collectionElementValues);
            }
            else if (propertyValues.Count > 0)
            {
                var rowTypeDef = new EdmRowType();
                var rowPropValues = new List<IEdmPropertyValue>();
                foreach (var propVal in propertyValues)
                {
                    var rowProp = new EdmStructuralProperty(rowTypeDef, propVal.Item1, propVal.Item2.Type, null, EdmConcurrencyMode.None);
                    rowPropValues.Add(new EdmPropertyValue(rowProp, propVal.Item2));
                }

                value = new EdmRowValue(rowTypeDef.ApplyType(true), rowPropValues);
            }
            else
            {
                value = new EdmStringValue(new EdmStringTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false), xmlAnnotation.Value);
            }

            foreach (IEdmImmediateValueAnnotation annotation in annotations)
            {
                value.SetAnnotation(annotation.Namespace(), annotation.LocalName(), annotation.Value);
            }

            return true;
        }
    }
}

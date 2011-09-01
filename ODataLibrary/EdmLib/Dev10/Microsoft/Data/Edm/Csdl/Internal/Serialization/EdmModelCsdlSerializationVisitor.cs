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
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Library;

namespace Microsoft.Data.Edm.Csdl.Internal.Serialization
{
    internal sealed class EdmModelCsdlSerializationVisitor : EdmModelVisitor
    {
        private readonly Version edmVersion;
        private readonly EdmModelCsdlSchemaWriter schemaWriter;

        internal EdmModelCsdlSerializationVisitor(XmlWriter xmlWriter, Version edmVersion)
        {
            this.edmVersion = edmVersion;
            this.schemaWriter = new EdmModelCsdlSchemaWriter(xmlWriter, this.edmVersion);
        }

        internal void VisitEdmSchema(EdmSchema element, XmlNamespaceManager manager)
        {
            this.schemaWriter.WriteSchemaElementHeader(element.Namespace, manager);
            foreach (string usingNamespace in element.Usings)
            {
                if (usingNamespace != element.Namespace)
                {
                    this.schemaWriter.WriteUsingElement(usingNamespace);
                }
            }

            VisitSchemaElements(element.SchemaElements);
            VisitEntityContainers(element.EntityContainers);
            this.schemaWriter.WriteEndElement();
        }

        private void VisitAttributeAnnotations(IEnumerable<IEdmAnnotation> annotations)
        {
            foreach (IEdmAnnotation annotation in annotations)
            {
                IEdmImmediateValueAnnotation immediate = annotation as IEdmImmediateValueAnnotation;
                if (immediate != null)
                {
                    if (immediate.Namespace() != EdmConstants.InternalUri)
                    {
                        var edmValue = immediate.Value as IEdmValue;
                        if (edmValue != null)
                        {
                            if (edmValue.Type.TypeKind() == EdmTypeKind.Primitive)
                            {
                                this.VisitPrimitiveAnnotation(immediate);
                            }
                        }
                    }
                }
            }
        }

        static private void VisitElementAnnotations(IEnumerable<IEdmAnnotation> annotations)
        {
            foreach (IEdmAnnotation annotation in annotations)
            {
                IEdmImmediateValueAnnotation immediate = annotation as IEdmImmediateValueAnnotation;
                if (immediate != null)
                {
                    if (immediate.Namespace() != EdmConstants.InternalUri)
                    {
                        var edmValue = immediate.Value as IEdmValue;
                        if (edmValue != null)
                        {
                            if (edmValue.Type.TypeKind() != EdmTypeKind.Primitive)
                            {
                                // TODO: Do something.
                            }
                        }
                    }
                }
            }
        }

        private void VisitPrimitiveAnnotation(IEdmImmediateValueAnnotation annotation)
        {
            this.schemaWriter.WriteAnnotationStringAttribute(annotation);
        }

        private void BeginElement<TElement>(TElement element, Action<TElement> elementHeaderWriter, params Action<TElement>[] additionalAttributeWriters)
            where TElement: IEdmElement
        {
            elementHeaderWriter(element);
            foreach (Action<TElement> action in additionalAttributeWriters)
            {
                action(element);
            }

            this.VisitAttributeAnnotations(element.Annotations);
            IEdmDocumentation documentation = element.GetDocumentation();
            if (documentation != null)
            {
                this.ProcessEdmDocumentation(documentation);
            }
        }

        private void EndElement(IEdmElement element)
        {
            VisitElementAnnotations(element.Annotations);
            this.schemaWriter.WriteEndElement();
        }

        private void ProcessEdmDocumentation(IEdmDocumentation element)
        {
            this.schemaWriter.WriteDocumentationElement(element);
        }

        protected override void ProcessEntityContainer(IEdmEntityContainer element)
        {
            this.BeginElement<IEdmEntityContainer>(element, this.schemaWriter.WriteEntityContainerElementHeader);
            base.ProcessEntityContainer(element);
            this.EndElement(element);
        }

        protected override void ProcessAssociationSet(IEdmAssociationSet element)
        {
            this.BeginElement<IEdmAssociationSet>(element, this.schemaWriter.WriteAssociationSetElementHeader);
            base.ProcessAssociationSet(element);
            this.EndElement(element);
        }

        protected override void ProcessAssociationSetEnd(IEdmAssociationSetEnd element)
        {
            this.BeginElement<IEdmAssociationSetEnd>(element, this.schemaWriter.WriteAssociationSetEndElementHeader);
            base.ProcessAssociationSetEnd(element);
            this.EndElement(element);
        }

        protected override void ProcessEntitySet(IEdmEntitySet element)
        {
            this.BeginElement<IEdmEntitySet>(element, this.schemaWriter.WriteEntitySetElementHeader);
            base.ProcessEntitySet(element);
            this.EndElement(element);
        }

        protected override void ProcessEntityTypeDefinition(IEdmEntityType element)
        {
            this.BeginElement<IEdmEntityType>(element, this.schemaWriter.WriteEntityTypeElementHeader);
            if (element.DeclaredKey != null && element.DeclaredKey.Count() > 0 && element.BaseType == null)
            {
                this.VisitEntityTypeDeclaredKey(element.DeclaredKey);
            }

            this.VisitProperties(element.DeclaredStructuralProperties().Cast<IEdmProperty>());
            this.VisitProperties(element.DeclaredNavigationProperties().Cast<IEdmProperty>());
            this.EndElement(element);
        }

        private void VisitEntityTypeDeclaredKey(IEnumerable<IEdmStructuralProperty> keyProperties)
        {
            this.schemaWriter.WriteDelaredKeyPropertiesElementHeader();
            this.VisitPropertyRefs(keyProperties);
            this.schemaWriter.WriteEndElement();
        }

        private void VisitPropertyRefs(IEnumerable<IEdmStructuralProperty> properties)
        {
            foreach (IEdmStructuralProperty property in properties)
            {
                this.schemaWriter.WritePropertyRefElement(property);
            }
        }

        protected override void ProcessStructuralProperty(IEdmStructuralProperty element)
        {
            bool inlineType = IsInlineType(element.Type);
            Debug.Assert(element.DeclaringType.TypeKind == EdmTypeKind.Row || inlineType, "Only row properties can have nested types");
            this.BeginElement<IEdmStructuralProperty>(element, (IEdmStructuralProperty t) => { this.schemaWriter.WriteStructuralPropertyElementHeader(t, inlineType); }, e => { this.ProcessFacets(e.Type, inlineType); });
            if (!inlineType)
            {
                VisitTypeReference(element.Type);
            }

            this.EndElement(element);
        }

        private void ProcessFacets(IEdmTypeReference element, bool inlineType)
        {
            if (inlineType)
            {
                if (element.TypeKind() == EdmTypeKind.Collection)
                {
                    IEdmCollectionTypeReference collectionElement = element.AsCollection();
                    if (collectionElement.CollectionDefinition().ElementType.TypeKind() == EdmTypeKind.Primitive)
                    {
                        this.schemaWriter.WriteNullableAttribute(collectionElement.CollectionDefinition().ElementType);
                        VisitTypeReference(collectionElement.CollectionDefinition().ElementType);
                    }
                }
                else
                {
                    this.schemaWriter.WriteNullableAttribute(element);
                    VisitTypeReference(element);
                }
            }
        }

        protected override void ProcessBinaryTypeReference(IEdmBinaryTypeReference element)
        {
            this.schemaWriter.WriteBinaryTypeAttributes(element);
        }

        protected override void ProcessDecimalTypeReference(IEdmDecimalTypeReference element)
        {
            this.schemaWriter.WriteDecimalTypeAttributes(element);
        }

        protected override void ProcessSpatialTypeReference(IEdmSpatialTypeReference element)
        {
            this.schemaWriter.WriteSpatialTypeAttributes(element);
        }

        protected override void ProcessStringTypeReference(IEdmStringTypeReference element)
        {
            this.schemaWriter.WriteStringTypeAttributes(element);
        }

        protected override void ProcessTemporalTypeReference(IEdmTemporalTypeReference element)
        {
            this.schemaWriter.WriteTemporalTypeAttributes(element);
        }

        protected override void ProcessNavigationProperty(IEdmNavigationProperty element)
        {
            this.BeginElement<IEdmNavigationProperty>(element, this.schemaWriter.WriteNavigationPropertyElementHeader);
            this.EndElement(element);
        }

        protected override void ProcessComplexTypeDefinition(IEdmComplexType element)
        {
            this.BeginElement<IEdmComplexType>(element, this.schemaWriter.WriteComplexTypeElementHeader);
            base.ProcessComplexTypeDefinition(element);
            this.EndElement(element);
        }

        protected override void ProcessEnumTypeDefinition(IEdmEnumType element)
        {
            this.BeginElement<IEdmEnumType>(element, this.schemaWriter.WriteEnumTypeElementHeader);
            base.ProcessEnumTypeDefinition(element);
            this.EndElement(element);
        }

        protected override void ProcessEnumMember(IEdmEnumMember element)
        {
            this.BeginElement<IEdmEnumMember>(element, this.schemaWriter.WriteEnumMemberElementHeader);
            this.EndElement(element);
        }

        protected override void ProcessAssociation(IEdmAssociation element)
        {
            this.BeginElement<IEdmAssociation>(element, this.schemaWriter.WriteAssociationElementHeader);
            base.ProcessAssociation(element);
            this.EndElement(element);
        }

        protected override void ProcessAssociationEnd(IEdmAssociationEnd element)
        {
            this.BeginElement<IEdmAssociationEnd>(element, this.schemaWriter.WriteAssociationEndElementHeader);
            if (element.OnDelete != EdmOnDeleteAction.None)
            {
                this.schemaWriter.WriteOperationActionElement(CsdlConstants.Element_OnDelete, element.OnDelete);
            }

            this.EndElement(element);
        }

        protected override void ProcessReferentialConstraint(IEdmReferentialConstraint element)
        {
            // We do not use Begin/End Element functions for Constraints end because the documentation belongs to the Association Ends where they were declared, not to this reference
            this.BeginElement<IEdmReferentialConstraint>(element, this.schemaWriter.WriteReferentialConstraintElementHeader);
            this.schemaWriter.WriteReferentialConstraintPrincipleEndElementHeader(element.PrincipalEnd);
            this.VisitPropertyRefs(element.PrincipalEnd.EntityType.Key());
            this.schemaWriter.WriteEndElement();
            this.schemaWriter.WriteReferentialConstraintDependentEndElementHeader(element.DependentEnd());
            this.VisitPropertyRefs(element.DependentProperties);
            this.schemaWriter.WriteEndElement();
            this.EndElement(element);
        }

        protected override void ProcessFunction(IEdmFunction element)
        {
            if (element.ReturnType != null)
            {
                bool inlineReturnType = IsInlineType(element.ReturnType);
                this.BeginElement<IEdmFunction>(element, (IEdmFunction t) => { this.schemaWriter.WriteFunctionElementHeader(t, inlineReturnType); });
                if (!inlineReturnType)
                {
                    this.schemaWriter.WriteReturnTypeElementHeader();
                    this.VisitTypeReference(element.ReturnType);
                    this.schemaWriter.WriteEndElement();
                }
            }
            else
            {
                this.BeginElement<IEdmFunction>(element, (IEdmFunction t) => { this.schemaWriter.WriteFunctionElementHeader(t, false /*Inline ReturnType*/); });
            }

            if (element.DefiningExpression != null)
            {
                this.schemaWriter.WriteDefiningExpressionElement(element.DefiningExpression);
            }

            this.VisitFunctionParameters(element.Parameters);
            this.EndElement(element);
        }

        protected override void ProcessFunctionParameter(IEdmFunctionParameter element)
        {
            bool inlineType = IsInlineType(element.Type);
            this.BeginElement<IEdmFunctionParameter>(
                element, 
                (IEdmFunctionParameter t) => { this.schemaWriter.WriteFunctionParameterElementHeader(t, inlineType); },
                e => { this.ProcessFacets(e.Type, inlineType); });
            if (!inlineType)
            {
                VisitTypeReference(element.Type);
            }

            this.EndElement(element);
        }

        protected override void ProcessCollectionTypeDefinition(IEdmCollectionType element)
        {
            bool inlineType = IsInlineType(element.ElementType);
            this.BeginElement<IEdmCollectionType>(
                element,
                (IEdmCollectionType t) => this.schemaWriter.WriteCollectionTypeElementHeader(t, inlineType),
                e => this.ProcessFacets(e.ElementType, inlineType));
            if (!inlineType)
            {
                VisitTypeReference(element.ElementType);
            }

            this.EndElement(element);
        }

        protected override void ProcessRowTypeDefinition(IEdmRowType element)
        {
            this.schemaWriter.WriteRowTypeElementHeader();
            base.ProcessRowTypeDefinition(element);
            this.schemaWriter.WriteEndElement();
        }

        protected override void ProcessEntityReferenceTypeDefinition(IEdmEntityReferenceType element)
        {
            this.BeginElement<IEdmEntityReferenceType>(element, this.schemaWriter.WriteEntityReferenceTypeElementHeader);
            this.EndElement(element);
        }

        protected override void ProcessFunctionImport(IEdmFunctionImport functionImport)
        {
            this.BeginElement<IEdmFunctionImport>(functionImport, this.schemaWriter.WriteFunctionImportElementHeader);
            this.VisitFunctionParameters(functionImport.Parameters);
            this.EndElement(functionImport);
        }

        private static bool IsInlineType(IEdmTypeReference reference)
        {
            if (reference.Definition is IEdmSchemaElement)
            {
                return true;
            }

            IEdmCollectionTypeReference collectionReference = reference as IEdmCollectionTypeReference;
            return collectionReference != null && collectionReference.CollectionDefinition().ElementType.Definition is IEdmSchemaElement;
        }
    }
}

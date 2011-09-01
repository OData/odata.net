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
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Library;

namespace Microsoft.Data.Edm
{
    internal abstract class EdmModelVisitor
    {
        protected EdmModelVisitor()
        {
        }

        public void VisitEdmModel(IEdmModel model)
        {
            this.ProcessModel(model);
        }

        #region Process Methods

        protected virtual void ProcessModel(IEdmModel model)
        {
            this.ProcessElement(model);
            this.VisitSchemaElements(model.SchemaElements);
            this.VisitEntityContainers(model.EntityContainers);
        }

        #region Base Element Types

        protected virtual void ProcessElement(IEdmElement element)
        {
            this.ProcessAnnotatable(element);
        }

        protected virtual void ProcessAnnotatable(IEdmAnnotatable element)
        {
            this.VisitAnnotations(element.Annotations);
        }

        protected virtual void ProcessNamedElement(IEdmNamedElement element)
        {
            this.ProcessElement(element);
        }

        protected virtual void ProcessSchemaElement(IEdmSchemaElement element)
        {
            this.ProcessNamedElement(element);
        }

        #endregion

        #region Type References

        protected virtual void ProcessComplexTypeReference(IEdmComplexTypeReference reference)
        {
            this.ProcessStructuredTypeReference(reference);
        }

        protected virtual void ProcessEntityTypeReference(IEdmEntityTypeReference reference)
        {
            this.ProcessStructuredTypeReference(reference);
        }

        protected virtual void ProcessEntityReferenceTypeReference(IEdmEntityReferenceTypeReference reference)
        {
            this.ProcessTypeReference(reference);
            this.ProcessEntityReferenceTypeDefinition(reference.EntityReferenceDefinition());
        }

        protected virtual void ProcessRowTypeReference(IEdmRowTypeReference reference) 
        {
            this.ProcessStructuredTypeReference(reference);
            this.ProcessRowTypeDefinition(reference.RowDefinition());
        }

        protected virtual void ProcessCollectionTypeReference(IEdmCollectionTypeReference reference)
        {
            this.ProcessTypeReference(reference);
            this.ProcessCollectionTypeDefinition(reference.CollectionDefinition());
        }

        protected virtual void ProcessEnumTypeReference(IEdmEnumTypeReference reference)
        {
            this.ProcessTypeReference(reference);
        }

        protected virtual void ProcessBinaryTypeReference(IEdmBinaryTypeReference reference)
        {
            this.ProcessPrimitiveTypeReference(reference);
        }

        protected virtual void ProcessDecimalTypeReference(IEdmDecimalTypeReference reference)
        {
            this.ProcessPrimitiveTypeReference(reference);
        }

        protected virtual void ProcessSpatialTypeReference(IEdmSpatialTypeReference reference)
        {
            this.ProcessPrimitiveTypeReference(reference);
        }

        protected virtual void ProcessStringTypeReference(IEdmStringTypeReference reference)
        {
            this.ProcessPrimitiveTypeReference(reference);
        }

        protected virtual void ProcessTemporalTypeReference(IEdmTemporalTypeReference reference)
        {
            this.ProcessPrimitiveTypeReference(reference);
        }

        protected virtual void ProcessPrimitiveTypeReference(IEdmPrimitiveTypeReference reference)
        {
            this.ProcessTypeReference(reference);
        }

        protected virtual void ProcessStructuredTypeReference(IEdmStructuredTypeReference reference)
        {
            this.ProcessTypeReference(reference);
        }

        protected virtual void ProcessTypeReference(IEdmTypeReference element)
        {
            this.ProcessElement(element);
        }

        #endregion

        #region Type Definitions

        protected virtual void ProcessAssociation(IEdmAssociation definition)
        {
            this.ProcessSchemaElement(definition);
            this.ProcessAssociationEnd(definition.End1);
            this.ProcessAssociationEnd(definition.End2);
            if (definition.ReferentialConstraint != null)
            {
                this.ProcessReferentialConstraint(definition.ReferentialConstraint);
            }
        }

        protected virtual void ProcessComplexTypeDefinition(IEdmComplexType definition)
        {
            this.ProcessSchemaElement(definition);
            this.ProcessStructuredTypeDefinition(definition);
        }

        protected virtual void ProcessEntityTypeDefinition(IEdmEntityType definition)
        {
            this.ProcessSchemaElement(definition);
            this.ProcessStructuredTypeDefinition(definition);
        }

        protected virtual void ProcessRowTypeDefinition(IEdmRowType definition)
        {
            this.ProcessElement(definition);
            this.ProcessStructuredTypeDefinition(definition);
        }

        protected virtual void ProcessCollectionTypeDefinition(IEdmCollectionType definition)
        {
            this.ProcessElement(definition);
            this.ProcessTypeDefinition(definition);
            this.VisitTypeReference(definition.ElementType);
        }

        protected virtual void ProcessEnumTypeDefinition(IEdmEnumType definition)
        {
            this.ProcessSchemaElement(definition);
            this.ProcessTypeDefinition(definition);
            this.VisitEnumMembers(definition.Members);
        }

        protected virtual void ProcessEntityReferenceTypeDefinition(IEdmEntityReferenceType definition)
        {
            this.ProcessElement(definition);
            this.ProcessTypeDefinition(definition);
        }

        protected virtual void ProcessStructuredTypeDefinition(IEdmStructuredType definition)
        {
            this.ProcessTypeDefinition(definition);
            this.VisitProperties(definition.DeclaredProperties);
        }

        protected virtual void ProcessTypeDefinition(IEdmType definition)
        {
        }

        #endregion

        #region Definition Components

        protected virtual void ProcessAssociationEnd(IEdmAssociationEnd end)
        {
            this.ProcessNamedElement(end);
        }

        protected virtual void ProcessReferentialConstraint(IEdmReferentialConstraint constraint)
        {
            this.ProcessElement(constraint);
        }

        protected virtual void ProcessNavigationProperty(IEdmNavigationProperty property)
        {
            this.ProcessProperty(property);
        }

        protected virtual void ProcessStructuralProperty(IEdmStructuralProperty property)
        {
            this.ProcessProperty(property);
        }

        protected virtual void ProcessProperty(IEdmProperty property)
        {
            this.ProcessNamedElement(property);
            this.VisitTypeReference(property.Type);
        }

        protected virtual void ProcessEnumMember(IEdmEnumMember enumMember)
        {
            this.ProcessNamedElement(enumMember);
        }

        #endregion

        #region Annotations

        protected virtual void ProcessAnnotation(IEdmAnnotation annotation)
        {
        }

        #endregion

        #region Data Model

        protected virtual void ProcessEntityContainer(IEdmEntityContainer container)
        {
            this.ProcessNamedElement(container);
            this.VisitEntityContainerElements(container.Elements);
        }

        protected virtual void ProcessEntityContainerElement(IEdmEntityContainerElement element)
        {
            this.ProcessNamedElement(element);
        }

        protected virtual void ProcessEntitySet(IEdmEntitySet set)
        {
            this.ProcessEntityContainerElement(set);
        }

        protected virtual void ProcessAssociationSet(IEdmAssociationSet set)
        {
            this.ProcessEntityContainerElement(set);
            if (set.End1 != null)
            {
                this.ProcessAssociationSetEnd(set.End1);
            }

            if (set.End2 != null)
            {
                this.ProcessAssociationSetEnd(set.End2);
            }
        }

        protected virtual void ProcessAssociationSetEnd(IEdmAssociationSetEnd end)
        {
            this.ProcessElement(end);
        }

        #endregion

        #region Function Related
        protected virtual void ProcessFunction(IEdmFunction function)
        {
            this.ProcessSchemaElement(function);
            this.ProcessFunctionBase(function);
        }

        protected virtual void ProcessFunctionImport(IEdmFunctionImport functionImport)
        {
            this.ProcessEntityContainerElement(functionImport);
            this.ProcessFunctionBase(functionImport);
        }

        protected virtual void ProcessFunctionBase(IEdmFunctionBase functionBase)
        {
            if (functionBase.ReturnType != null)
            {
                this.VisitTypeReference(functionBase.ReturnType);
            }

            this.VisitFunctionParameters(functionBase.Parameters);
        }

        protected virtual void ProcessFunctionParameter(IEdmFunctionParameter parameter)
        {
            this.ProcessNamedElement(parameter);
            this.VisitTypeReference(parameter.Type);
        }

        #endregion

        #endregion

        #region Visit Methods

        public static void VisitCollection<T>(IEnumerable<T> collection, Action<T> visitMethod)
        {
            foreach (T element in collection)
            {
                visitMethod(element);
            }
        }

        public void VisitSchemaElements(IEnumerable<IEdmSchemaElement> elements)
        {
            VisitCollection(elements, this.VisitSchemaElement);
        }

        public void VisitSchemaElement(IEdmSchemaElement element)
        {
            switch (element.SchemaElementKind)
            {
                case EdmSchemaElementKind.TypeDefinition:
                    this.VisitTypeDefinition((IEdmType)element);
                    break;
                case EdmSchemaElementKind.Function:
                    this.ProcessFunction((IEdmFunction)element);
                    break;
                case EdmSchemaElementKind.Association:
                    this.ProcessAssociation((IEdmAssociation)element);
                    break;
                case EdmSchemaElementKind.None:
                    this.ProcessSchemaElement(element);
                    break;
                default:
                    throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_SchemaElementKind(element.SchemaElementKind));
            }
        }

        #region Annotations

        public void VisitAnnotations(IEnumerable<IEdmAnnotation> annotations)
        {
            VisitCollection(annotations, this.VisitAnnotation);
        }

        public void VisitAnnotation(IEdmAnnotation annotation)
        {
            this.ProcessAnnotation(annotation);
        }

        #endregion

        #region Data Model

        public void VisitEntityContainers(IEnumerable<IEdmEntityContainer> entityContainers)
        {
            VisitCollection(entityContainers, this.ProcessEntityContainer);
        }

        public void VisitEntityContainerElements(IEnumerable<IEdmEntityContainerElement> elements)
        {
            foreach (IEdmEntityContainerElement element in elements)
            {
                switch (element.ContainerElementKind)
                {
                    case EdmContainerElementKind.EntitySet:
                        this.ProcessEntitySet((IEdmEntitySet)element);
                        break;
                    case EdmContainerElementKind.AssociationSet:
                        this.ProcessAssociationSet((IEdmAssociationSet)element);
                        break;
                    case EdmContainerElementKind.FunctionImport:
                        this.ProcessFunctionImport((IEdmFunctionImport)element);
                        break;
                    case EdmContainerElementKind.None:
                        this.ProcessEntityContainerElement(element);
                        break;
                    default:
                        throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_ContainerElementKind(element.ContainerElementKind.ToString()));
                }
            }
        }

        #endregion

        #region References

        public void VisitTypeReference(IEdmTypeReference reference)
        {
            switch (reference.TypeKind())
            {
                case EdmTypeKind.Collection:
                    this.ProcessCollectionTypeReference(reference.AsCollection());
                    break;
                case EdmTypeKind.Complex:
                    this.ProcessComplexTypeReference(reference.AsComplex());
                    break;
                case EdmTypeKind.Entity:
                    this.ProcessEntityTypeReference(reference.AsEntity());
                    break;
                case EdmTypeKind.EntityReference:
                    this.ProcessEntityReferenceTypeReference(reference.AsEntityReference());
                    break;
                case EdmTypeKind.Enum:
                    this.ProcessEnumTypeReference(reference.AsEnum());
                    break;
                case EdmTypeKind.Primitive:
                    this.VisitPrimitiveTypeReference(reference.AsPrimitive());
                    break;
                case EdmTypeKind.Row:
                    this.ProcessRowTypeReference(reference.AsRow());
                    break;
                case EdmTypeKind.None:
                    this.ProcessTypeReference(reference);
                    break;
                default:
                    throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_TypeKind(reference.TypeKind().ToString()));
            }
        }

        public void VisitPrimitiveTypeReference(IEdmPrimitiveTypeReference reference)
        {
            switch (reference.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Binary:
                    this.ProcessBinaryTypeReference(reference.AsBinary());
                    break;
                case EdmPrimitiveTypeKind.Decimal:
                    this.ProcessDecimalTypeReference(reference.AsDecimal());
                    break;
                case EdmPrimitiveTypeKind.String:
                    this.ProcessStringTypeReference(reference.AsString());
                    break;
                case EdmPrimitiveTypeKind.DateTime:
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Time:
                    this.ProcessTemporalTypeReference(reference.AsTemporal());
                    break;
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.Point:
                case EdmPrimitiveTypeKind.LineString:
                case EdmPrimitiveTypeKind.Polygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.MultiPolygon:
                case EdmPrimitiveTypeKind.MultiLineString:
                case EdmPrimitiveTypeKind.MultiPoint:
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometricPoint:
                case EdmPrimitiveTypeKind.GeometricLineString:
                case EdmPrimitiveTypeKind.GeometricPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometricMultiPolygon:
                case EdmPrimitiveTypeKind.GeometricMultiLineString:
                case EdmPrimitiveTypeKind.GeometricMultiPoint:
                    this.ProcessSpatialTypeReference(reference.AsSpatial());
                    break;
                case EdmPrimitiveTypeKind.Boolean:
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.Double:
                case EdmPrimitiveTypeKind.Guid:
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.Stream:
                case EdmPrimitiveTypeKind.None:
                    this.ProcessPrimitiveTypeReference(reference);
                    break;
                default:
                    throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_PrimitiveKind(reference.PrimitiveKind().ToString()));
            }
        }

        #endregion

        #region Definitions

        public void VisitTypeDefinition(IEdmType definition)
        {
            switch (definition.TypeKind)
            {
                case EdmTypeKind.Complex:
                    this.ProcessComplexTypeDefinition((IEdmComplexType)definition);
                    break;
                case EdmTypeKind.Entity:
                    this.ProcessEntityTypeDefinition((IEdmEntityType)definition);
                    break;
                case EdmTypeKind.Collection:
                    this.ProcessCollectionTypeDefinition((IEdmCollectionType)definition);
                    break;
                case EdmTypeKind.Row:
                    this.ProcessRowTypeDefinition((IEdmRowType)definition);
                    break;
                case EdmTypeKind.EntityReference:
                    this.ProcessEntityReferenceTypeDefinition((IEdmEntityReferenceType)definition);
                    break;
                case EdmTypeKind.Enum:
                    this.ProcessEnumTypeDefinition((IEdmEnumType)definition);
                    break;
                case EdmTypeKind.None:
                    this.VisitTypeDefinition(definition);
                    break;
                default:
                    throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_TypeKind(definition.TypeKind));
            }
        }

        public void VisitProperties(IEnumerable<IEdmProperty> properties)
        {
            VisitCollection(properties, this.VisitProperty);
        }

        public void VisitProperty(IEdmProperty property)
        {
            switch (property.PropertyKind)
            {
                case EdmPropertyKind.Navigation:
                    this.ProcessNavigationProperty((IEdmNavigationProperty)property);
                    break;
                case EdmPropertyKind.Structural:
                    this.ProcessStructuralProperty((IEdmStructuralProperty)property);
                    break;
                case EdmPropertyKind.None:
                    this.ProcessProperty(property);
                    break;
                default:
                    throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_PropertyKind(property.PropertyKind.ToString()));
            }
        }

        public void VisitEnumMembers(IEnumerable<IEdmEnumMember> enumMembers)
        {
            VisitCollection(enumMembers, this.VisitEnumMember);
        }

        public void VisitEnumMember(IEdmEnumMember enumMember)
        {
            this.ProcessEnumMember(enumMember);
        }

        #endregion

        #region Function Related

        public void VisitFunctionParameters(IEnumerable<IEdmFunctionParameter> parameters)
        {
            VisitCollection(parameters, this.ProcessFunctionParameter);
        }

        #endregion
        #endregion
    }
}

//---------------------------------------------------------------------
// <copyright file="EdmModelVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    internal abstract class EdmModelVisitor
    {
        protected readonly IEdmModel Model;

        protected EdmModelVisitor(IEdmModel model)
        {
            this.Model = model;
        }

        public void VisitEdmModel()
        {
            this.ProcessModel(this.Model);
        }

        #region Visit Methods

        #region Elements

        public void VisitSchemaElements(IEnumerable<IEdmSchemaElement> elements)
        {
            VisitCollection(elements, this.VisitSchemaElement);
        }

        public void VisitSchemaElement(IEdmSchemaElement element)
        {
            switch (element.SchemaElementKind)
            {
                case EdmSchemaElementKind.Action:
                    this.ProcessAction((IEdmAction)element);
                    break;
                case EdmSchemaElementKind.Function:
                    this.ProcessFunction((IEdmFunction)element);
                    break;
                case EdmSchemaElementKind.TypeDefinition:
                    this.VisitSchemaType((IEdmType)element);
                    break;
                case EdmSchemaElementKind.Term:
                    this.ProcessTerm((IEdmTerm)element);
                    break;
                case EdmSchemaElementKind.EntityContainer:
                    this.ProcessEntityContainer((IEdmEntityContainer)element);
                    break;
                case EdmSchemaElementKind.None:
                    this.ProcessSchemaElement(element);
                    break;
                default:
                    throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_SchemaElementKind(element.SchemaElementKind));
            }
        }

        #endregion

        #region Annotations

        public void VisitAnnotations(IEnumerable<IEdmDirectValueAnnotation> annotations)
        {
            VisitCollection(annotations, this.VisitAnnotation);
        }

        public void VisitVocabularyAnnotations(IEnumerable<IEdmVocabularyAnnotation> annotations)
        {
            VisitCollection(annotations, this.VisitVocabularyAnnotation);
        }

        public void VisitAnnotation(IEdmDirectValueAnnotation annotation)
        {
            this.ProcessImmediateValueAnnotation((IEdmDirectValueAnnotation)annotation);
        }

        public void VisitVocabularyAnnotation(IEdmVocabularyAnnotation annotation)
        {
            if (annotation.Term != null)
            {
                this.ProcessAnnotation(annotation);
            }
            else
            {
                this.ProcessVocabularyAnnotation(annotation);
            }
        }

        public void VisitPropertyValueBindings(IEnumerable<IEdmPropertyValueBinding> bindings)
        {
            VisitCollection(bindings, this.ProcessPropertyValueBinding);
        }

        #endregion

        #region Expressions

        public void VisitExpressions(IEnumerable<IEdmExpression> expressions)
        {
            VisitCollection(expressions, this.VisitExpression);
        }

        public void VisitExpression(IEdmExpression expression)
        {
            switch (expression.ExpressionKind)
            {
                case EdmExpressionKind.Cast:
                    this.ProcessCastExpression((IEdmCastExpression)expression);
                    break;
                case EdmExpressionKind.BinaryConstant:
                    this.ProcessBinaryConstantExpression((IEdmBinaryConstantExpression)expression);
                    break;
                case EdmExpressionKind.BooleanConstant:
                    this.ProcessBooleanConstantExpression((IEdmBooleanConstantExpression)expression);
                    break;
                case EdmExpressionKind.Collection:
                    this.ProcessCollectionExpression((IEdmCollectionExpression)expression);
                    break;
                case EdmExpressionKind.DateConstant:
                    this.ProcessDateConstantExpression((IEdmDateConstantExpression)expression);
                    break;
                case EdmExpressionKind.DateTimeOffsetConstant:
                    this.ProcessDateTimeOffsetConstantExpression((IEdmDateTimeOffsetConstantExpression)expression);
                    break;
                case EdmExpressionKind.DecimalConstant:
                    this.ProcessDecimalConstantExpression((IEdmDecimalConstantExpression)expression);
                    break;
                case EdmExpressionKind.EnumMember:
                    this.ProcessEnumMemberExpression((IEdmEnumMemberExpression)expression);
                    break;
                case EdmExpressionKind.FloatingConstant:
                    this.ProcessFloatingConstantExpression((IEdmFloatingConstantExpression)expression);
                    break;
                case EdmExpressionKind.FunctionApplication:
                    this.ProcessFunctionApplicationExpression((IEdmApplyExpression)expression);
                    break;
                case EdmExpressionKind.GuidConstant:
                    this.ProcessGuidConstantExpression((IEdmGuidConstantExpression)expression);
                    break;
                case EdmExpressionKind.If:
                    this.ProcessIfExpression((IEdmIfExpression)expression);
                    break;
                case EdmExpressionKind.IntegerConstant:
                    this.ProcessIntegerConstantExpression((IEdmIntegerConstantExpression)expression);
                    break;
                case EdmExpressionKind.IsType:
                    this.ProcessIsTypeExpression((IEdmIsTypeExpression)expression);
                    break;
                case EdmExpressionKind.LabeledExpressionReference:
                    this.ProcessLabeledExpressionReferenceExpression((IEdmLabeledExpressionReferenceExpression)expression);
                    break;
                case EdmExpressionKind.Labeled:
                    this.ProcessLabeledExpression((IEdmLabeledExpression)expression);
                    break;
                case EdmExpressionKind.Null:
                    this.ProcessNullConstantExpression((IEdmNullExpression)expression);
                    break;
                case EdmExpressionKind.Path:
                    this.ProcessPathExpression((IEdmPathExpression)expression);
                    break;
                case EdmExpressionKind.PropertyPath:
                    this.ProcessPropertyPathExpression((IEdmPathExpression)expression);
                    break;
                case EdmExpressionKind.NavigationPropertyPath:
                    this.ProcessNavigationPropertyPathExpression((IEdmPathExpression)expression);
                    break;
                case EdmExpressionKind.AnnotationPath:
                    this.ProcessAnnotationPathExpression((IEdmPathExpression)expression);
                    break;
                case EdmExpressionKind.Record:
                    this.ProcessRecordExpression((IEdmRecordExpression)expression);
                    break;
                case EdmExpressionKind.StringConstant:
                    this.ProcessStringConstantExpression((IEdmStringConstantExpression)expression);
                    break;
                case EdmExpressionKind.TimeOfDayConstant:
                    this.ProcessTimeOfDayConstantExpression((IEdmTimeOfDayConstantExpression)expression);
                    break;
                case EdmExpressionKind.DurationConstant:
                    this.ProcessDurationConstantExpression((IEdmDurationConstantExpression)expression);
                    break;
                case EdmExpressionKind.None:
                    this.ProcessExpression(expression);
                    break;
                default:
                    throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_ExpressionKind(expression.ExpressionKind));
            }
        }

        public void VisitPropertyConstructors(IEnumerable<IEdmPropertyConstructor> constructor)
        {
            VisitCollection(constructor, this.ProcessPropertyConstructor);
        }

        #endregion

        #region Data Model

        public virtual void VisitEntityContainerElements(IEnumerable<IEdmEntityContainerElement> elements)
        {
            foreach (IEdmEntityContainerElement element in elements)
            {
                switch (element.ContainerElementKind)
                {
                    case EdmContainerElementKind.EntitySet:
                        this.ProcessEntitySet((IEdmEntitySet)element);
                        break;
                    case EdmContainerElementKind.Singleton:
                        this.ProcessSingleton((IEdmSingleton)element);
                        break;
                    case EdmContainerElementKind.ActionImport:
                        this.ProcessActionImport((IEdmActionImport)element);
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

        public virtual async Task VisitEntityContainerElementsAsync(IEnumerable<IEdmEntityContainerElement> elements)
        {
            foreach (IEdmEntityContainerElement element in elements)
            {
                switch (element.ContainerElementKind)
                {
                    case EdmContainerElementKind.EntitySet:
                        await this.ProcessEntitySetAsync((IEdmEntitySet)element);
                        break;
                    case EdmContainerElementKind.Singleton:
                        await this.ProcessSingletonAsync((IEdmSingleton)element);
                        break;
                    case EdmContainerElementKind.ActionImport:
                        await this.ProcessActionImportAsync((IEdmActionImport)element);
                        break;
                    case EdmContainerElementKind.FunctionImport:
                        await this.ProcessFunctionImportAsync((IEdmFunctionImport)element);
                        break;
                    case EdmContainerElementKind.None:
                        await this.ProcessEntityContainerElementAsync(element);
                        break;
                    default:
                        throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_ContainerElementKind(element.ContainerElementKind.ToString()));
                }
            }
        }

        #endregion

        #region Type References

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
                case EdmTypeKind.TypeDefinition:
                    this.ProcessTypeDefinitionReference(reference.AsTypeDefinition());
                    break;
                case EdmTypeKind.None:
                    this.ProcessTypeReference(reference);
                    break;
                case EdmTypeKind.Path:
                    this.ProcessPathTypeReference(reference.AsPath());
                    break;
                case EdmTypeKind.Untyped:
                    this.ProcessUntypedTypeReference(reference as IEdmUntypedTypeReference);
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
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Duration:
                case EdmPrimitiveTypeKind.TimeOfDay:
                    this.ProcessTemporalTypeReference(reference.AsTemporal());
                    break;
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.GeographyPoint:
                case EdmPrimitiveTypeKind.GeographyLineString:
                case EdmPrimitiveTypeKind.GeographyPolygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometryPoint:
                case EdmPrimitiveTypeKind.GeometryLineString:
                case EdmPrimitiveTypeKind.GeometryPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
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
                case EdmPrimitiveTypeKind.Date:
                case EdmPrimitiveTypeKind.PrimitiveType:
                case EdmPrimitiveTypeKind.None:
                    this.ProcessPrimitiveTypeReference(reference);
                    break;
                default:
                    throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_PrimitiveKind(reference.PrimitiveKind().ToString()));
            }
        }

        #endregion

        #region Type Definitions

        public void VisitSchemaType(IEdmType definition)
        {
            switch (definition.TypeKind)
            {
                case EdmTypeKind.Complex:
                    this.ProcessComplexType((IEdmComplexType)definition);
                    break;
                case EdmTypeKind.Entity:
                    this.ProcessEntityType((IEdmEntityType)definition);
                    break;
                case EdmTypeKind.Enum:
                    this.ProcessEnumType((IEdmEnumType)definition);
                    break;
                case EdmTypeKind.TypeDefinition:
                    this.ProcessTypeDefinition((IEdmTypeDefinition)definition);
                    break;
                case EdmTypeKind.None:
                    this.VisitSchemaType(definition);
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

        #region Operation Related

        public void VisitOperationParameters(IEnumerable<IEdmOperationParameter> parameters)
        {
            VisitCollection(parameters, this.ProcessOperationParameter);
        }

        #endregion

        protected static void VisitCollection<T>(IEnumerable<T> collection, Action<T> visitMethod)
        {
            foreach (T element in collection)
            {
                visitMethod(element);
            }
        }
        #endregion

        #region Process Methods

        protected virtual void ProcessModel(IEdmModel model)
        {
            this.ProcessElement(model);

            // TODO: also visit referenced models?
            this.VisitSchemaElements(model.SchemaElements);
            this.VisitVocabularyAnnotations(model.VocabularyAnnotations);
        }

        #region Base Element Types

        protected virtual void ProcessElement(IEdmElement element)
        {
            // TODO: DirectValueAnnotationsInMainSchema (not including those in referenced schemas)
            this.VisitAnnotations(this.Model.DirectValueAnnotations(element));
        }

        protected virtual Task ProcessElementAsync(IEdmElement element)
        {
            // TODO: DirectValueAnnotationsInMainSchema (not including those in referenced schemas)
            this.VisitAnnotations(this.Model.DirectValueAnnotations(element));

            return Task.CompletedTask;
        }

        protected virtual void ProcessNamedElement(IEdmNamedElement element)
        {
            this.ProcessElement(element);
        }

        protected virtual async Task ProcessNamedElementAsync(IEdmNamedElement element)
        {
            await this.ProcessElementAsync(element);
        }

        protected virtual void ProcessSchemaElement(IEdmSchemaElement element)
        {
            this.ProcessVocabularyAnnotatable(element);
            this.ProcessNamedElement(element);
        }

        protected virtual async Task ProcessSchemaElementAsync(IEdmSchemaElement element)
        {
            await this.ProcessVocabularyAnnotatableAsync(element);
            await this.ProcessNamedElementAsync(element);
        }

        protected virtual void ProcessVocabularyAnnotatable(IEdmVocabularyAnnotatable annotatable)
        {
        }

        protected virtual Task ProcessVocabularyAnnotatableAsync(IEdmVocabularyAnnotatable annotatable)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Type References

        protected virtual void ProcessComplexTypeReference(IEdmComplexTypeReference reference)
        {
            this.ProcessStructuredTypeReference(reference);
        }

        protected virtual async Task ProcessComplexTypeReferenceAsync(IEdmComplexTypeReference reference)
        {
            await this.ProcessStructuredTypeReferenceAsync(reference);
        }

        protected virtual void ProcessEntityTypeReference(IEdmEntityTypeReference reference)
        {
            this.ProcessStructuredTypeReference(reference);
        }

        protected virtual async Task ProcessEntityTypeReferenceAsync(IEdmEntityTypeReference reference)
        {
            await this.ProcessStructuredTypeReferenceAsync(reference);
        }

        protected virtual void ProcessEntityReferenceTypeReference(IEdmEntityReferenceTypeReference reference)
        {
            this.ProcessTypeReference(reference);
            this.ProcessEntityReferenceType(reference.EntityReferenceDefinition());
        }

        protected virtual async Task ProcessEntityReferenceTypeReferenceAsync(IEdmEntityReferenceTypeReference reference)
        {
            await this.ProcessTypeReferenceAsync(reference);
            await this.ProcessEntityReferenceTypeAsync(reference.EntityReferenceDefinition());
        }

        protected virtual void ProcessCollectionTypeReference(IEdmCollectionTypeReference reference)
        {
            this.ProcessTypeReference(reference);
            this.ProcessCollectionType(reference.CollectionDefinition());
        }

        protected virtual async Task ProcessCollectionTypeReferenceAsync(IEdmCollectionTypeReference reference)
        {
            await this.ProcessTypeReferenceAsync(reference);
            await this.ProcessCollectionTypeAsync(reference.CollectionDefinition());
        }

        protected virtual void ProcessEnumTypeReference(IEdmEnumTypeReference reference)
        {
            this.ProcessTypeReference(reference);
        }

        protected virtual async Task ProcessEnumTypeReferenceAsync(IEdmEnumTypeReference reference)
        {
            await this.ProcessTypeReferenceAsync(reference);
        }

        protected virtual void ProcessTypeDefinitionReference(IEdmTypeDefinitionReference reference)
        {
            this.ProcessTypeReference(reference);
        }

        protected virtual async Task ProcessTypeDefinitionReferenceAsync(IEdmTypeDefinitionReference reference)
        {
            await this.ProcessTypeReferenceAsync(reference);
        }

        protected virtual void ProcessBinaryTypeReference(IEdmBinaryTypeReference reference)
        {
            this.ProcessPrimitiveTypeReference(reference);
        }

        protected virtual async Task ProcessBinaryTypeReferenceAsync(IEdmBinaryTypeReference reference)
        {
            await this.ProcessPrimitiveTypeReferenceAsync(reference);
        }

        protected virtual void ProcessDecimalTypeReference(IEdmDecimalTypeReference reference)
        {
            this.ProcessPrimitiveTypeReference(reference);
        }

        protected virtual async Task ProcessDecimalTypeReferenceAsync(IEdmDecimalTypeReference reference)
        {
            await this.ProcessPrimitiveTypeReferenceAsync(reference);
        }

        protected virtual void ProcessSpatialTypeReference(IEdmSpatialTypeReference reference)
        {
            this.ProcessPrimitiveTypeReference(reference);
        }

        protected virtual async Task ProcessSpatialTypeReferenceAsync(IEdmSpatialTypeReference reference)
        {
            await this.ProcessPrimitiveTypeReferenceAsync(reference);
        }

        protected virtual void ProcessStringTypeReference(IEdmStringTypeReference reference)
        {
            this.ProcessPrimitiveTypeReference(reference);
        }

        protected virtual async Task ProcessStringTypeReferenceAsync(IEdmStringTypeReference reference)
        {
            await this.ProcessPrimitiveTypeReferenceAsync(reference);
        }

        protected virtual void ProcessTemporalTypeReference(IEdmTemporalTypeReference reference)
        {
            this.ProcessPrimitiveTypeReference(reference);
        }

        protected virtual async Task ProcessTemporalTypeReferenceAsync(IEdmTemporalTypeReference reference)
        {
            await this.ProcessPrimitiveTypeReferenceAsync(reference);
        }

        protected virtual void ProcessPrimitiveTypeReference(IEdmPrimitiveTypeReference reference)
        {
            this.ProcessTypeReference(reference);
        }

        protected virtual async Task ProcessPrimitiveTypeReferenceAsync(IEdmPrimitiveTypeReference reference)
        {
            await this.ProcessTypeReferenceAsync(reference);
        }

        protected virtual void ProcessStructuredTypeReference(IEdmStructuredTypeReference reference)
        {
            this.ProcessTypeReference(reference);
        }

        protected virtual async Task ProcessStructuredTypeReferenceAsync(IEdmStructuredTypeReference reference)
        {
            await this.ProcessTypeReferenceAsync(reference);
        }

        protected virtual void ProcessTypeReference(IEdmTypeReference element)
        {
            this.ProcessElement(element);
        }

        protected virtual async Task ProcessTypeReferenceAsync(IEdmTypeReference element)
        {
            await this.ProcessElementAsync(element);
        }

        protected virtual void ProcessPathTypeReference(IEdmPathTypeReference reference)
        {
            this.ProcessTypeReference(reference);
        }

        protected virtual async Task ProcessPathTypeReferenceAsync(IEdmPathTypeReference reference)
        {
            await this.ProcessTypeReferenceAsync(reference);
        }

        protected virtual void ProcessUntypedTypeReference(IEdmUntypedTypeReference reference)
        {
            this.ProcessTypeReference(reference);
        }

        protected virtual async Task ProcessUntypedTypeReferenceAsync(IEdmUntypedTypeReference reference)
        {
            await this.ProcessTypeReferenceAsync(reference);
        }

        #endregion

        #region Terms

        protected virtual void ProcessTerm(IEdmTerm term)
        {
            this.ProcessSchemaElement(term);
            this.VisitTypeReference(term.Type);
        }

        protected virtual async Task ProcessTermAsync(IEdmTerm term)
        {
            await this.ProcessSchemaElementAsync(term);
            this.VisitTypeReference(term.Type);
        }

        #endregion

        #region Type Definitions

        protected virtual void ProcessComplexType(IEdmComplexType definition)
        {
            this.ProcessSchemaElement(definition);
            this.ProcessStructuredType(definition);
            this.ProcessSchemaType(definition);
        }

        protected virtual async Task ProcessComplexTypeAsync(IEdmComplexType definition)
        {
            await this.ProcessSchemaElementAsync(definition);
            await this.ProcessStructuredTypeAsync(definition);
            await this.ProcessSchemaTypeAsync(definition);
        }

        protected virtual void ProcessEntityType(IEdmEntityType definition)
        {
            this.ProcessSchemaElement(definition);
            this.ProcessStructuredType(definition);
            this.ProcessSchemaType(definition);
        }

        protected virtual async Task ProcessEntityTypeAsync(IEdmEntityType definition)
        {
            await this.ProcessSchemaElementAsync(definition);
            await this.ProcessStructuredTypeAsync(definition);
            await this.ProcessSchemaTypeAsync(definition);
        }

        protected virtual void ProcessCollectionType(IEdmCollectionType definition)
        {
            this.ProcessElement(definition);
            this.ProcessType(definition);
            this.VisitTypeReference(definition.ElementType);
        }

        protected virtual async Task ProcessCollectionTypeAsync(IEdmCollectionType definition)
        {
            await this.ProcessElementAsync(definition);
            await this.ProcessTypeAsync(definition);
            this.VisitTypeReference(definition.ElementType);
        }

        protected virtual void ProcessEnumType(IEdmEnumType definition)
        {
            this.ProcessSchemaElement(definition);
            this.ProcessType(definition);
            this.ProcessSchemaType(definition);
            this.VisitEnumMembers(definition.Members);
        }

        protected virtual async Task ProcessEnumTypeAsync(IEdmEnumType definition)
        {
            await this.ProcessSchemaElementAsync(definition);
            await this.ProcessTypeAsync(definition);
            await this.ProcessSchemaTypeAsync(definition);
            this.VisitEnumMembers(definition.Members);
        }

        protected virtual void ProcessTypeDefinition(IEdmTypeDefinition definition)
        {
            this.ProcessSchemaElement(definition);
            this.ProcessType(definition);
            this.ProcessSchemaType(definition);
        }

        protected virtual async Task ProcessTypeDefinitionAsync(IEdmTypeDefinition definition)
        {
            await this.ProcessSchemaElementAsync(definition);
            await this.ProcessTypeAsync(definition);
            await this.ProcessSchemaTypeAsync(definition);
        }

        protected virtual void ProcessEntityReferenceType(IEdmEntityReferenceType definition)
        {
            this.ProcessElement(definition);
            this.ProcessType(definition);
        }

        protected virtual async Task ProcessEntityReferenceTypeAsync(IEdmEntityReferenceType definition)
        {
            await this.ProcessElementAsync(definition);
            await this.ProcessTypeAsync(definition);
        }

        protected virtual void ProcessStructuredType(IEdmStructuredType definition)
        {
            this.ProcessType(definition);
            this.VisitProperties(definition.DeclaredProperties);
        }

        protected virtual async Task ProcessStructuredTypeAsync(IEdmStructuredType definition)
        {
            await this.ProcessTypeAsync(definition);
            this.VisitProperties(definition.DeclaredProperties);
        }

        protected virtual void ProcessSchemaType(IEdmSchemaType type)
        {
            // Do not visit type or schema element, because all types will do that on their own.
        }

        protected virtual Task ProcessSchemaTypeAsync(IEdmSchemaType type)
        {
            // Do not visit type or schema element, because all types will do that on their own.
            return Task.CompletedTask;
        }

        protected virtual void ProcessType(IEdmType definition)
        {
        }

        protected virtual Task ProcessTypeAsync(IEdmType definition)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Definition Components

        protected virtual void ProcessNavigationProperty(IEdmNavigationProperty property)
        {
            this.ProcessProperty(property);
        }

        protected virtual async Task ProcessNavigationPropertyAsync(IEdmNavigationProperty property)
        {
            await this.ProcessPropertyAsync(property);
        }

        protected virtual void ProcessStructuralProperty(IEdmStructuralProperty property)
        {
            this.ProcessProperty(property);
        }

        protected virtual async Task ProcessStructuralPropertyAsync(IEdmStructuralProperty property)
        {
            await this.ProcessPropertyAsync(property);
        }

        protected virtual void ProcessProperty(IEdmProperty property)
        {
            this.ProcessVocabularyAnnotatable(property);
            this.ProcessNamedElement(property);
            this.VisitTypeReference(property.Type);
        }

        protected virtual async Task ProcessPropertyAsync(IEdmProperty property)
        {
            await this.ProcessVocabularyAnnotatableAsync(property);
            await this.ProcessNamedElementAsync(property);
            this.VisitTypeReference(property.Type);
        }

        protected virtual void ProcessEnumMember(IEdmEnumMember enumMember)
        {
            this.ProcessNamedElement(enumMember);
        }

        protected virtual async Task ProcessEnumMemberAsync(IEdmEnumMember enumMember)
        {
            await this.ProcessNamedElementAsync(enumMember);
        }

        #endregion

        #region Annotations

        protected virtual void ProcessVocabularyAnnotation(IEdmVocabularyAnnotation annotation)
        {
            this.ProcessElement(annotation);
        }

        protected virtual async Task ProcessVocabularyAnnotationAsync(IEdmVocabularyAnnotation annotation)
        {
            await this.ProcessElementAsync(annotation);
        }

        protected virtual void ProcessImmediateValueAnnotation(IEdmDirectValueAnnotation annotation)
        {
            this.ProcessNamedElement(annotation);
        }

        protected virtual async Task ProcessImmediateValueAnnotationAsync(IEdmDirectValueAnnotation annotation)
        {
            await this.ProcessNamedElementAsync(annotation);
        }

        protected virtual void ProcessAnnotation(IEdmVocabularyAnnotation annotation)
        {
            this.ProcessVocabularyAnnotation(annotation);
            this.VisitExpression(annotation.Value);
        }

        protected virtual async Task ProcessAnnotationAsync(IEdmVocabularyAnnotation annotation)
        {
            await this.ProcessVocabularyAnnotationAsync(annotation);
            this.VisitExpression(annotation.Value);
        }

        protected virtual void ProcessPropertyValueBinding(IEdmPropertyValueBinding binding)
        {
            this.VisitExpression(binding.Value);
        }

        #endregion

        #region Expressions

        protected virtual void ProcessExpression(IEdmExpression expression)
        {
        }

        protected virtual Task ProcessExpressionAsync(IEdmExpression expression)
        {
            return Task.CompletedTask;
        }

        protected virtual void ProcessStringConstantExpression(IEdmStringConstantExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual async Task ProcessStringConstantExpressionAsync(IEdmStringConstantExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessBinaryConstantExpression(IEdmBinaryConstantExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual async Task ProcessBinaryConstantExpressionAsync(IEdmBinaryConstantExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessRecordExpression(IEdmRecordExpression expression)
        {
            this.ProcessExpression(expression);
            if (expression.DeclaredType != null)
            {
                this.VisitTypeReference(expression.DeclaredType);
            }

            this.VisitPropertyConstructors(expression.Properties);
        }

        protected virtual async Task ProcessRecordExpressionAsync(IEdmRecordExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
            if (expression.DeclaredType != null)
            {
                this.VisitTypeReference(expression.DeclaredType);
            }

            this.VisitPropertyConstructors(expression.Properties);
        }

        protected virtual void ProcessPathExpression(IEdmPathExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual async Task ProcessPathExpressionAsync(IEdmPathExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessPropertyPathExpression(IEdmPathExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual async Task ProcessPropertyPathExpressionAsync(IEdmPathExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessNavigationPropertyPathExpression(IEdmPathExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual async Task ProcessNavigationPropertyPathExpressionAsync(IEdmPathExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessAnnotationPathExpression(IEdmPathExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual async Task ProcessAnnotationPathExpressionAsync(IEdmPathExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessCollectionExpression(IEdmCollectionExpression expression)
        {
            this.ProcessExpression(expression);
            this.VisitExpressions(expression.Elements);
        }

        protected virtual async Task ProcessCollectionExpressionAsync(IEdmCollectionExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
            this.VisitExpressions(expression.Elements);
        }

        protected virtual void ProcessLabeledExpressionReferenceExpression(IEdmLabeledExpressionReferenceExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual async Task ProcessLabeledExpressionReferenceExpressionAsync(IEdmLabeledExpressionReferenceExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessIsTypeExpression(IEdmIsTypeExpression expression)
        {
            this.ProcessExpression(expression);
            this.VisitTypeReference(expression.Type);
            this.VisitExpression(expression.Operand);
        }

        protected virtual async Task ProcessIsTypeExpressionAsync(IEdmIsTypeExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
            this.VisitTypeReference(expression.Type);
            this.VisitExpression(expression.Operand);
        }

        protected virtual void ProcessIntegerConstantExpression(IEdmIntegerConstantExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual async Task ProcessIntegerConstantExpressionAsync(IEdmIntegerConstantExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessIfExpression(IEdmIfExpression expression)
        {
            this.ProcessExpression(expression);
            this.VisitExpression(expression.TestExpression);
            this.VisitExpression(expression.TrueExpression);
            this.VisitExpression(expression.FalseExpression);
        }

        protected virtual async Task ProcessIfExpressionAsync(IEdmIfExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
            this.VisitExpression(expression.TestExpression);
            this.VisitExpression(expression.TrueExpression);
            this.VisitExpression(expression.FalseExpression);
        }

        protected virtual void ProcessFunctionApplicationExpression(IEdmApplyExpression expression)
        {
            this.ProcessExpression(expression);
            this.VisitExpressions(expression.Arguments);
        }

        protected virtual async Task ProcessFunctionApplicationExpressionAsync(IEdmApplyExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
            this.VisitExpressions(expression.Arguments);
        }

        protected virtual void ProcessFloatingConstantExpression(IEdmFloatingConstantExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual async Task ProcessFloatingConstantExpressionAsync(IEdmFloatingConstantExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessGuidConstantExpression(IEdmGuidConstantExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual async Task ProcessGuidConstantExpressionAsync(IEdmGuidConstantExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessEnumMemberExpression(IEdmEnumMemberExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual async Task ProcessEnumMemberExpressionAsync(IEdmEnumMemberExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessDecimalConstantExpression(IEdmDecimalConstantExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual async Task ProcessDecimalConstantExpressionAsync(IEdmDecimalConstantExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessDateConstantExpression(IEdmDateConstantExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual async Task ProcessDateConstantExpressionAsync(IEdmDateConstantExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessTimeOfDayConstantExpression(IEdmTimeOfDayConstantExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual async Task ProcessTimeOfDayConstantExpressionAsync(IEdmTimeOfDayConstantExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessDateTimeOffsetConstantExpression(IEdmDateTimeOffsetConstantExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual async Task ProcessDateTimeOffsetConstantExpressionAsync(IEdmDateTimeOffsetConstantExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessDurationConstantExpression(IEdmDurationConstantExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual async Task ProcessDurationConstantExpressionAsync(IEdmDurationConstantExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessBooleanConstantExpression(IEdmBooleanConstantExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual async Task ProcessBooleanConstantExpressionAsync(IEdmBooleanConstantExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessCastExpression(IEdmCastExpression expression)
        {
            this.ProcessExpression(expression);
            this.VisitTypeReference(expression.Type);
            this.VisitExpression(expression.Operand);
        }

        protected virtual async Task ProcessCastExpressionAsync(IEdmCastExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
            this.VisitTypeReference(expression.Type);
            this.VisitExpression(expression.Operand);
        }


        protected virtual void ProcessLabeledExpression(IEdmLabeledExpression element)
        {
            this.VisitExpression(element.Expression);
        }

        protected virtual Task ProcessLabeledExpressionAsync(IEdmLabeledExpression element)
        {
            this.VisitExpression(element.Expression);

            return Task.CompletedTask;
        }

        protected virtual void ProcessPropertyConstructor(IEdmPropertyConstructor constructor)
        {
            this.VisitExpression(constructor.Value);
        }

        protected virtual Task ProcessPropertyConstructorAsync(IEdmPropertyConstructor constructor)
        {
            this.VisitExpression(constructor.Value);

            return Task.CompletedTask;
        }

        protected virtual void ProcessNullConstantExpression(IEdmNullExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual async Task ProcessNullConstantExpressionAsync(IEdmNullExpression expression)
        {
            await this.ProcessExpressionAsync(expression);
        }

        #endregion

        #region Data Model

        protected virtual void ProcessEntityContainer(IEdmEntityContainer container)
        {
            this.ProcessVocabularyAnnotatable(container);
            this.ProcessNamedElement(container);
            this.VisitEntityContainerElements(container.Elements);
        }

        protected virtual async Task ProcessEntityContainerAsync(IEdmEntityContainer container)
        {
            await this.ProcessVocabularyAnnotatableAsync(container);
            await this.ProcessNamedElementAsync(container);
            await this.VisitEntityContainerElementsAsync(container.Elements);
        }

        protected virtual void ProcessEntityContainerElement(IEdmEntityContainerElement element)
        {
            this.ProcessNamedElement(element);
        }

        protected virtual async Task ProcessEntityContainerElementAsync(IEdmEntityContainerElement element)
        {
            await this.ProcessNamedElementAsync(element);
        }

        protected virtual void ProcessEntitySet(IEdmEntitySet set)
        {
            this.ProcessEntityContainerElement(set);
        }

        protected virtual async Task ProcessEntitySetAsync(IEdmEntitySet set)
        {
            await this.ProcessEntityContainerElementAsync(set);
        }

        protected virtual void ProcessSingleton(IEdmSingleton singleton)
        {
            this.ProcessEntityContainerElement(singleton);
        }

        protected virtual async Task ProcessSingletonAsync(IEdmSingleton singleton)
        {
            await this.ProcessEntityContainerElementAsync(singleton);
        }

        #endregion

        #region Operation Related

        protected virtual void ProcessAction(IEdmAction action)
        {
            this.ProcessSchemaElement(action);
            this.ProcessOperation(action);
        }

        protected virtual async Task ProcessActionAsync(IEdmAction action)
        {
            await this.ProcessSchemaElementAsync(action);
            await this.ProcessOperationAsync(action);
        }

        protected virtual void ProcessFunction(IEdmFunction function)
        {
            this.ProcessSchemaElement(function);
            this.ProcessOperation(function);
        }

        protected virtual async Task ProcessFunctionAsync(IEdmFunction function)
        {
            await this.ProcessSchemaElementAsync(function);
            await this.ProcessOperationAsync(function);
        }

        protected virtual void ProcessActionImport(IEdmActionImport actionImport)
        {
            this.ProcessEntityContainerElement(actionImport);
        }

        protected virtual async Task ProcessActionImportAsync(IEdmActionImport actionImport)
        {
            await this.ProcessEntityContainerElementAsync(actionImport);
        }

        protected virtual void ProcessFunctionImport(IEdmFunctionImport functionImport)
        {
            this.ProcessEntityContainerElement(functionImport);
        }

        protected virtual async Task ProcessFunctionImportAsync(IEdmFunctionImport functionImport)
        {
            await this.ProcessEntityContainerElementAsync(functionImport);
        }

        protected virtual void ProcessOperation(IEdmOperation operation)
        {
            // Do not visit vocabularyAnnotatable because functions and operation imports are always going to be either a schema element or a container element and will be visited through those paths.
            this.VisitOperationParameters(operation.Parameters);

            IEdmOperationReturn operationReturn = operation.GetReturn();
            this.ProcessOperationReturn(operationReturn);
        }

        protected virtual async Task ProcessOperationAsync(IEdmOperation operation)
        {
            // Do not visit vocabularyAnnotatable because functions and operation imports are always going to be either a schema element or a container element and will be visited through those paths.
            this.VisitOperationParameters(operation.Parameters);

            IEdmOperationReturn operationReturn = operation.GetReturn();
            await this.ProcessOperationReturnAsync(operationReturn);
        }

        protected virtual void ProcessOperationParameter(IEdmOperationParameter parameter)
        {
            this.ProcessVocabularyAnnotatable(parameter);
            this.ProcessNamedElement(parameter);
            this.VisitTypeReference(parameter.Type);
        }

        protected virtual async Task ProcessOperationParameterAsync(IEdmOperationParameter parameter)
        {
            await this.ProcessVocabularyAnnotatableAsync(parameter);
            await this.ProcessNamedElementAsync(parameter);
            this.VisitTypeReference(parameter.Type);
        }

        protected virtual void ProcessOperationReturn(IEdmOperationReturn operationReturn)
        {
            if (operationReturn == null)
            {
                return;
            }

            this.ProcessVocabularyAnnotatable(operationReturn);
            this.VisitTypeReference(operationReturn.Type);
        }

        protected virtual async Task ProcessOperationReturnAsync(IEdmOperationReturn operationReturn)
        {
            if (operationReturn == null)
            {
                return;
            }

            await this.ProcessVocabularyAnnotatableAsync(operationReturn);
            this.VisitTypeReference(operationReturn.Type);
        }
        #endregion

        #endregion
    }
}

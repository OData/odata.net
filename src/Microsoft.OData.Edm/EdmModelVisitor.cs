//---------------------------------------------------------------------
// <copyright file="EdmModelVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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

        public Task VisitEdmModelAsync()
        {
            return this.ProcessModelAsync(this.Model);
        }

        #region Visit Methods

        #region Elements

        public void VisitSchemaElements(IEnumerable<IEdmSchemaElement> elements)
        {
            VisitCollection(elements, this.VisitSchemaElement);
        }

        public Task VisitSchemaElementsAsync(IEnumerable<IEdmSchemaElement> elements)
        {
            return VisitCollectionAsync(elements, this.VisitSchemaElementAsync);
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

        public Task VisitSchemaElementAsync(IEdmSchemaElement element)
        {
            switch (element.SchemaElementKind)
            {
                case EdmSchemaElementKind.Action:
                    return this.ProcessActionAsync((IEdmAction)element);
                case EdmSchemaElementKind.Function:
                    return this.ProcessFunctionAsync((IEdmFunction)element);
                case EdmSchemaElementKind.TypeDefinition:
                    return this.VisitSchemaTypeAsync((IEdmType)element);
                case EdmSchemaElementKind.Term:
                    return this.ProcessTermAsync((IEdmTerm)element);
                case EdmSchemaElementKind.EntityContainer:
                    return this.ProcessEntityContainerAsync((IEdmEntityContainer)element);
                case EdmSchemaElementKind.None:
                    return this.ProcessSchemaElementAsync(element);
                default:
                    Contract.Assert(false, Edm.Strings.UnknownEnumVal_SchemaElementKind(element.SchemaElementKind));
                    return Task.FromException<InvalidOperationException>(new InvalidOperationException(Edm.Strings.UnknownEnumVal_SchemaElementKind(element.SchemaElementKind)));
            }
        }

        #endregion

        #region Annotations

        public void VisitAnnotations(IEnumerable<IEdmDirectValueAnnotation> annotations)
        {
            VisitCollection(annotations, this.VisitAnnotation);
        }

        public Task VisitAnnotationsAsync(IEnumerable<IEdmDirectValueAnnotation> annotations)
        {
            return VisitCollectionAsync(annotations, this.VisitAnnotationAsync);
        }

        public void VisitVocabularyAnnotations(IEnumerable<IEdmVocabularyAnnotation> annotations)
        {
            VisitCollection(annotations, this.VisitVocabularyAnnotation);
        }

        public Task VisitVocabularyAnnotationsAsync(IEnumerable<IEdmVocabularyAnnotation> annotations)
        {
            return VisitCollectionAsync(annotations, this.VisitVocabularyAnnotationAsync);
        }

        public void VisitAnnotation(IEdmDirectValueAnnotation annotation)
        {
            this.ProcessImmediateValueAnnotation((IEdmDirectValueAnnotation)annotation);
        }

        public Task VisitAnnotationAsync(IEdmDirectValueAnnotation annotation)
        {
            return this.ProcessImmediateValueAnnotationAsync((IEdmDirectValueAnnotation)annotation);
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

        public Task VisitVocabularyAnnotationAsync(IEdmVocabularyAnnotation annotation)
        {
            if (annotation.Term != null)
            {
                return this.ProcessAnnotationAsync(annotation);
            }
            else
            {
                return this.ProcessVocabularyAnnotationAsync(annotation);
            }
        }

        public void VisitPropertyValueBindings(IEnumerable<IEdmPropertyValueBinding> bindings)
        {
            VisitCollection(bindings, this.ProcessPropertyValueBinding);
        }

        public Task VisitPropertyValueBindingsAsync(IEnumerable<IEdmPropertyValueBinding> bindings)
        {
            return VisitCollectionAsync(bindings, this.ProcessPropertyValueBindingAsync);
        }

        #endregion

        #region Expressions

        public void VisitExpressions(IEnumerable<IEdmExpression> expressions)
        {
            VisitCollection(expressions, this.VisitExpression);
        }

        public Task VisitExpressionsAsync(IEnumerable<IEdmExpression> expressions)
        {
            return VisitCollectionAsync(expressions, this.VisitExpressionAsync);
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
                case EdmExpressionKind.IsOf:
                    this.ProcessIsOfExpression((IEdmIsOfExpression)expression);
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

        public Task VisitExpressionAsync(IEdmExpression expression)
        {
            switch (expression.ExpressionKind)
            {
                case EdmExpressionKind.Cast:
                    return this.ProcessCastExpressionAsync((IEdmCastExpression)expression);
                case EdmExpressionKind.BinaryConstant:
                    return this.ProcessBinaryConstantExpressionAsync((IEdmBinaryConstantExpression)expression);
                case EdmExpressionKind.BooleanConstant:
                    return this.ProcessBooleanConstantExpressionAsync((IEdmBooleanConstantExpression)expression);
                case EdmExpressionKind.Collection:
                    return this.ProcessCollectionExpressionAsync((IEdmCollectionExpression)expression);
                case EdmExpressionKind.DateConstant:
                    return this.ProcessDateConstantExpressionAsync((IEdmDateConstantExpression)expression);
                case EdmExpressionKind.DateTimeOffsetConstant:
                    return this.ProcessDateTimeOffsetConstantExpressionAsync((IEdmDateTimeOffsetConstantExpression)expression);
                case EdmExpressionKind.DecimalConstant:
                    return this.ProcessDecimalConstantExpressionAsync((IEdmDecimalConstantExpression)expression);
                case EdmExpressionKind.EnumMember:
                    return this.ProcessEnumMemberExpressionAsync((IEdmEnumMemberExpression)expression);
                case EdmExpressionKind.FloatingConstant:
                    return this.ProcessFloatingConstantExpressionAsync((IEdmFloatingConstantExpression)expression);
                case EdmExpressionKind.FunctionApplication:
                    return this.ProcessFunctionApplicationExpressionAsync((IEdmApplyExpression)expression);
                case EdmExpressionKind.GuidConstant:
                    return this.ProcessGuidConstantExpressionAsync((IEdmGuidConstantExpression)expression);
                case EdmExpressionKind.If:
                    return this.ProcessIfExpressionAsync((IEdmIfExpression)expression);
                case EdmExpressionKind.IntegerConstant:
                    return this.ProcessIntegerConstantExpressionAsync((IEdmIntegerConstantExpression)expression);
                case EdmExpressionKind.IsOf:
                    return this.ProcessIsOfExpressionAsync((IEdmIsOfExpression)expression);
                case EdmExpressionKind.LabeledExpressionReference:
                    return this.ProcessLabeledExpressionReferenceExpressionAsync((IEdmLabeledExpressionReferenceExpression)expression);
                case EdmExpressionKind.Labeled:
                    return this.ProcessLabeledExpressionAsync((IEdmLabeledExpression)expression);
                case EdmExpressionKind.Null:
                    return this.ProcessNullConstantExpressionAsync((IEdmNullExpression)expression);
                case EdmExpressionKind.Path:
                    return this.ProcessPathExpressionAsync((IEdmPathExpression)expression);
                case EdmExpressionKind.PropertyPath:
                    return this.ProcessPropertyPathExpressionAsync((IEdmPathExpression)expression);
                case EdmExpressionKind.NavigationPropertyPath:
                    return this.ProcessNavigationPropertyPathExpressionAsync((IEdmPathExpression)expression);
                case EdmExpressionKind.AnnotationPath:
                    return this.ProcessAnnotationPathExpressionAsync((IEdmPathExpression)expression);
                case EdmExpressionKind.Record:
                    return this.ProcessRecordExpressionAsync((IEdmRecordExpression)expression);
                case EdmExpressionKind.StringConstant:
                    return this.ProcessStringConstantExpressionAsync((IEdmStringConstantExpression)expression);
                case EdmExpressionKind.TimeOfDayConstant:
                    return this.ProcessTimeOfDayConstantExpressionAsync((IEdmTimeOfDayConstantExpression)expression);
                case EdmExpressionKind.DurationConstant:
                    return this.ProcessDurationConstantExpressionAsync((IEdmDurationConstantExpression)expression);
                case EdmExpressionKind.None:
                    return this.ProcessExpressionAsync(expression);
                default:
                    Contract.Assert(false, Edm.Strings.UnknownEnumVal_ExpressionKind(expression.ExpressionKind));
                    return Task.FromException<InvalidOperationException>(new InvalidOperationException(Edm.Strings.UnknownEnumVal_ExpressionKind(expression.ExpressionKind)));
            }
        }

        public void VisitPropertyConstructors(IEnumerable<IEdmPropertyConstructor> constructor)
        {
            VisitCollection(constructor, this.ProcessPropertyConstructor);
        }

        public Task VisitPropertyConstructorsAsync(IEnumerable<IEdmPropertyConstructor> constructor)
        {
            return VisitCollectionAsync(constructor, this.ProcessPropertyConstructorAsync);
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

        public virtual Task VisitEntityContainerElementsAsync(IEnumerable<IEdmEntityContainerElement> elements)
        {
            foreach (IEdmEntityContainerElement element in elements)
            {
                switch (element.ContainerElementKind)
                {
                    case EdmContainerElementKind.EntitySet:
                        return this.ProcessEntitySetAsync((IEdmEntitySet)element);
                    case EdmContainerElementKind.Singleton:
                        return this.ProcessSingletonAsync((IEdmSingleton)element);
                    case EdmContainerElementKind.ActionImport:
                        return this.ProcessActionImportAsync((IEdmActionImport)element);
                    case EdmContainerElementKind.FunctionImport:
                        return this.ProcessFunctionImportAsync((IEdmFunctionImport)element);
                    case EdmContainerElementKind.None:
                        return this.ProcessEntityContainerElementAsync(element);
                    default:
                        Contract.Assert(false, Edm.Strings.UnknownEnumVal_ContainerElementKind(element.ContainerElementKind.ToString()));
                        return Task.FromException<InvalidOperationException>(new InvalidOperationException(Edm.Strings.UnknownEnumVal_ContainerElementKind(element.ContainerElementKind.ToString())));
                }
            }

            return Task.CompletedTask;
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

        public Task VisitTypeReferenceAsync(IEdmTypeReference reference)
        {
            switch (reference.TypeKind())
            {
                case EdmTypeKind.Collection:
                    return this.ProcessCollectionTypeReferenceAsync(reference.AsCollection());
                case EdmTypeKind.Complex:
                    return this.ProcessComplexTypeReferenceAsync(reference.AsComplex());
                case EdmTypeKind.Entity:
                    return this.ProcessEntityTypeReferenceAsync(reference.AsEntity());
                case EdmTypeKind.EntityReference:
                    return this.ProcessEntityReferenceTypeReferenceAsync(reference.AsEntityReference());
                case EdmTypeKind.Enum:
                    return this.ProcessEnumTypeReferenceAsync(reference.AsEnum());
                case EdmTypeKind.Primitive:
                    return this.VisitPrimitiveTypeReferenceAsync(reference.AsPrimitive());
                case EdmTypeKind.TypeDefinition:
                    return this.ProcessTypeDefinitionReferenceAsync(reference.AsTypeDefinition());
                case EdmTypeKind.None:
                    return this.ProcessTypeReferenceAsync(reference);
                case EdmTypeKind.Path:
                    return this.ProcessPathTypeReferenceAsync(reference.AsPath());
                case EdmTypeKind.Untyped:
                    return this.ProcessUntypedTypeReferenceAsync(reference as IEdmUntypedTypeReference);
                default:
                    Contract.Assert(false, Edm.Strings.UnknownEnumVal_TypeKind(reference.TypeKind().ToString()));
                    return Task.FromException<InvalidOperationException>(new InvalidOperationException(Edm.Strings.UnknownEnumVal_TypeKind(reference.TypeKind().ToString())));
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

        public Task VisitPrimitiveTypeReferenceAsync(IEdmPrimitiveTypeReference reference)
        {
            switch (reference.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Binary:
                    return this.ProcessBinaryTypeReferenceAsync(reference.AsBinary());
                case EdmPrimitiveTypeKind.Decimal:
                    return this.ProcessDecimalTypeReferenceAsync(reference.AsDecimal());
                case EdmPrimitiveTypeKind.String:
                    return this.ProcessStringTypeReferenceAsync(reference.AsString());
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Duration:
                case EdmPrimitiveTypeKind.TimeOfDay:
                    return this.ProcessTemporalTypeReferenceAsync(reference.AsTemporal());
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
                    return this.ProcessSpatialTypeReferenceAsync(reference.AsSpatial());
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
                    return this.ProcessPrimitiveTypeReferenceAsync(reference);
                default:
                    Contract.Assert(false, Edm.Strings.UnknownEnumVal_PrimitiveKind(reference.PrimitiveKind().ToString()));
                    return Task.FromException<InvalidOperationException>(new InvalidOperationException(Edm.Strings.UnknownEnumVal_PrimitiveKind(reference.PrimitiveKind().ToString())));
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

        public Task VisitSchemaTypeAsync(IEdmType definition)
        {
            switch (definition.TypeKind)
            {
                case EdmTypeKind.Complex:
                    return this.ProcessComplexTypeAsync((IEdmComplexType)definition);
                case EdmTypeKind.Entity:
                    return this.ProcessEntityTypeAsync((IEdmEntityType)definition);
                case EdmTypeKind.Enum:
                    return this.ProcessEnumTypeAsync((IEdmEnumType)definition);
                case EdmTypeKind.TypeDefinition:
                    return this.ProcessTypeDefinitionAsync((IEdmTypeDefinition)definition);
                case EdmTypeKind.None:
                    return this.VisitSchemaTypeAsync(definition);
                default:
                    Contract.Assert(false, Edm.Strings.UnknownEnumVal_TypeKind(definition.TypeKind));
                    return Task.FromException<InvalidOperationException>(new InvalidOperationException(Edm.Strings.UnknownEnumVal_TypeKind(definition.TypeKind)));
            }
        }

        public void VisitProperties(IEnumerable<IEdmProperty> properties)
        {
            VisitCollection(properties, this.VisitProperty);
        }

        public Task VisitPropertiesAsync(IEnumerable<IEdmProperty> properties)
        {
            return VisitCollectionAsync(properties, this.VisitPropertyAsync);
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

        public Task VisitPropertyAsync(IEdmProperty property)
        {
            switch (property.PropertyKind)
            {
                case EdmPropertyKind.Navigation:
                    return this.ProcessNavigationPropertyAsync((IEdmNavigationProperty)property);
                case EdmPropertyKind.Structural:
                    return this.ProcessStructuralPropertyAsync((IEdmStructuralProperty)property);
                case EdmPropertyKind.None:
                    return this.ProcessPropertyAsync(property);
                default:
                    Contract.Assert(false, Edm.Strings.UnknownEnumVal_PropertyKind(property.PropertyKind.ToString()));
                    return Task.FromException<InvalidOperationException>(new InvalidOperationException(Edm.Strings.UnknownEnumVal_PropertyKind(property.PropertyKind.ToString())));
            }
        }

        public void VisitEnumMembers(IEnumerable<IEdmEnumMember> enumMembers)
        {
            VisitCollection(enumMembers, this.VisitEnumMember);
        }

        public Task VisitEnumMembersAsync(IEnumerable<IEdmEnumMember> enumMembers)
        {
            return VisitCollectionAsync(enumMembers, this.VisitEnumMemberAsync);
        }

        public void VisitEnumMember(IEdmEnumMember enumMember)
        {
            this.ProcessEnumMember(enumMember);
        }

        public Task VisitEnumMemberAsync(IEdmEnumMember enumMember)
        {
            return this.ProcessEnumMemberAsync(enumMember);
        }

        #endregion

        #region Operation Related

        public void VisitOperationParameters(IEnumerable<IEdmOperationParameter> parameters)
        {
            VisitCollection(parameters, this.ProcessOperationParameter);
        }

        public Task VisitOperationParametersAsync(IEnumerable<IEdmOperationParameter> parameters)
        {
            return VisitCollectionAsync(parameters, this.ProcessOperationParameterAsync);
        }

        #endregion

        protected static void VisitCollection<T>(IEnumerable<T> collection, Action<T> visitMethod)
        {
            foreach (T element in collection)
            {
                visitMethod(element);
            }
        }

        protected static async Task VisitCollectionAsync<T>(IEnumerable<T> collection, Func<T, Task> visitMethod)
        {
            foreach (T element in collection)
            {
                await visitMethod(element).ConfigureAwait(false);
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

        protected virtual async Task ProcessModelAsync(IEdmModel model)
        {
            await this.ProcessElementAsync(model).ConfigureAwait(false);

            await this.VisitSchemaElementsAsync(model.SchemaElements).ConfigureAwait(false);
            await this.VisitVocabularyAnnotationsAsync(model.VocabularyAnnotations).ConfigureAwait(false);
        }

        #region Base Element Types

        protected virtual void ProcessElement(IEdmElement element)
        {
            // TODO: DirectValueAnnotationsInMainSchema (not including those in referenced schemas)
            this.VisitAnnotations(this.Model.DirectValueAnnotations(element));
        }

        protected virtual Task ProcessElementAsync(IEdmElement element)
        {
            return this.VisitAnnotationsAsync(this.Model.DirectValueAnnotations(element));
        }

        protected virtual void ProcessNamedElement(IEdmNamedElement element)
        {
            this.ProcessElement(element);
        }

        protected virtual Task ProcessNamedElementAsync(IEdmNamedElement element)
        {
            return this.ProcessElementAsync(element);
        }

        protected virtual void ProcessSchemaElement(IEdmSchemaElement element)
        {
            this.ProcessVocabularyAnnotatable(element);
            this.ProcessNamedElement(element);
        }

        protected virtual async Task ProcessSchemaElementAsync(IEdmSchemaElement element)
        {
            await this.ProcessVocabularyAnnotatableAsync(element).ConfigureAwait(false);
            await this.ProcessNamedElementAsync(element).ConfigureAwait(false);
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

        protected virtual Task ProcessComplexTypeReferenceAsync(IEdmComplexTypeReference reference)
        {
            return this.ProcessStructuredTypeReferenceAsync(reference);
        }

        protected virtual void ProcessEntityTypeReference(IEdmEntityTypeReference reference)
        {
            this.ProcessStructuredTypeReference(reference);
        }

        protected virtual Task ProcessEntityTypeReferenceAsync(IEdmEntityTypeReference reference)
        {
            return this.ProcessStructuredTypeReferenceAsync(reference);
        }

        protected virtual void ProcessEntityReferenceTypeReference(IEdmEntityReferenceTypeReference reference)
        {
            this.ProcessTypeReference(reference);
            this.ProcessEntityReferenceType(reference.EntityReferenceDefinition());
        }

        protected virtual async Task ProcessEntityReferenceTypeReferenceAsync(IEdmEntityReferenceTypeReference reference)
        {
            await this.ProcessTypeReferenceAsync(reference).ConfigureAwait(false);
            await this.ProcessEntityReferenceTypeAsync(reference.EntityReferenceDefinition()).ConfigureAwait(false);
        }

        protected virtual void ProcessCollectionTypeReference(IEdmCollectionTypeReference reference)
        {
            this.ProcessTypeReference(reference);
            this.ProcessCollectionType(reference.CollectionDefinition());
        }

        protected virtual async Task ProcessCollectionTypeReferenceAsync(IEdmCollectionTypeReference reference)
        {
            await this.ProcessTypeReferenceAsync(reference).ConfigureAwait(false);
            await this.ProcessCollectionTypeAsync(reference.CollectionDefinition()).ConfigureAwait(false);
        }

        protected virtual void ProcessEnumTypeReference(IEdmEnumTypeReference reference)
        {
            this.ProcessTypeReference(reference);
        }

        protected virtual Task ProcessEnumTypeReferenceAsync(IEdmEnumTypeReference reference)
        {
            return this.ProcessTypeReferenceAsync(reference);
        }

        protected virtual void ProcessTypeDefinitionReference(IEdmTypeDefinitionReference reference)
        {
            this.ProcessTypeReference(reference);
        }

        protected virtual Task ProcessTypeDefinitionReferenceAsync(IEdmTypeDefinitionReference reference)
        {
            return this.ProcessTypeReferenceAsync(reference);
        }

        protected virtual void ProcessBinaryTypeReference(IEdmBinaryTypeReference reference)
        {
            this.ProcessPrimitiveTypeReference(reference);
        }

        protected virtual Task ProcessBinaryTypeReferenceAsync(IEdmBinaryTypeReference reference)
        {
            return this.ProcessPrimitiveTypeReferenceAsync(reference);
        }

        protected virtual void ProcessDecimalTypeReference(IEdmDecimalTypeReference reference)
        {
            this.ProcessPrimitiveTypeReference(reference);
        }

        protected virtual Task ProcessDecimalTypeReferenceAsync(IEdmDecimalTypeReference reference)
        {
            return this.ProcessPrimitiveTypeReferenceAsync(reference);
        }

        protected virtual void ProcessSpatialTypeReference(IEdmSpatialTypeReference reference)
        {
            this.ProcessPrimitiveTypeReference(reference);
        }

        protected virtual Task ProcessSpatialTypeReferenceAsync(IEdmSpatialTypeReference reference)
        {
            return this.ProcessPrimitiveTypeReferenceAsync(reference);
        }

        protected virtual void ProcessStringTypeReference(IEdmStringTypeReference reference)
        {
            this.ProcessPrimitiveTypeReference(reference);
        }

        protected virtual Task ProcessStringTypeReferenceAsync(IEdmStringTypeReference reference)
        {
            return this.ProcessPrimitiveTypeReferenceAsync(reference);
        }

        protected virtual void ProcessTemporalTypeReference(IEdmTemporalTypeReference reference)
        {
            this.ProcessPrimitiveTypeReference(reference);
        }

        protected virtual Task ProcessTemporalTypeReferenceAsync(IEdmTemporalTypeReference reference)
        {
            return this.ProcessPrimitiveTypeReferenceAsync(reference);
        }

        protected virtual void ProcessPrimitiveTypeReference(IEdmPrimitiveTypeReference reference)
        {
            this.ProcessTypeReference(reference);
        }

        protected virtual Task ProcessPrimitiveTypeReferenceAsync(IEdmPrimitiveTypeReference reference)
        {
            return this.ProcessTypeReferenceAsync(reference);
        }

        protected virtual void ProcessStructuredTypeReference(IEdmStructuredTypeReference reference)
        {
            this.ProcessTypeReference(reference);
        }

        protected virtual Task ProcessStructuredTypeReferenceAsync(IEdmStructuredTypeReference reference)
        {
            return this.ProcessTypeReferenceAsync(reference);
        }

        protected virtual void ProcessTypeReference(IEdmTypeReference element)
        {
            this.ProcessElement(element);
        }

        protected virtual Task ProcessTypeReferenceAsync(IEdmTypeReference element)
        {
            return this.ProcessElementAsync(element);
        }

        protected virtual void ProcessPathTypeReference(IEdmPathTypeReference reference)
        {
            this.ProcessTypeReference(reference);
        }

        protected virtual Task ProcessPathTypeReferenceAsync(IEdmPathTypeReference reference)
        {
            return this.ProcessTypeReferenceAsync(reference);
        }

        protected virtual void ProcessUntypedTypeReference(IEdmUntypedTypeReference reference)
        {
            this.ProcessTypeReference(reference);
        }

        protected virtual Task ProcessUntypedTypeReferenceAsync(IEdmUntypedTypeReference reference)
        {
            return this.ProcessTypeReferenceAsync(reference);
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
            await this.ProcessSchemaElementAsync(term).ConfigureAwait(false);
            await this.VisitTypeReferenceAsync(term.Type).ConfigureAwait(false);
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
            await this.ProcessSchemaElementAsync(definition).ConfigureAwait(false);
            await this.ProcessStructuredTypeAsync(definition).ConfigureAwait(false);
            await this.ProcessSchemaTypeAsync(definition).ConfigureAwait(false);
        }

        protected virtual void ProcessEntityType(IEdmEntityType definition)
        {
            this.ProcessSchemaElement(definition);
            this.ProcessStructuredType(definition);
            this.ProcessSchemaType(definition);
        }

        protected virtual async Task ProcessEntityTypeAsync(IEdmEntityType definition)
        {
            await this.ProcessSchemaElementAsync(definition).ConfigureAwait(false);
            await this.ProcessStructuredTypeAsync(definition).ConfigureAwait(false);
            await this.ProcessSchemaTypeAsync(definition).ConfigureAwait(false);
        }

        protected virtual void ProcessCollectionType(IEdmCollectionType definition)
        {
            this.ProcessElement(definition);
            this.ProcessType(definition);
            this.VisitTypeReference(definition.ElementType);
        }

        protected virtual async Task ProcessCollectionTypeAsync(IEdmCollectionType definition)
        {
            await this.ProcessElementAsync(definition).ConfigureAwait(false);
            await this.ProcessTypeAsync(definition).ConfigureAwait(false);
            await this.VisitTypeReferenceAsync(definition.ElementType).ConfigureAwait(false);
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
            await this.ProcessSchemaElementAsync(definition).ConfigureAwait(false);
            await this.ProcessTypeAsync(definition).ConfigureAwait(false);
            await this.ProcessSchemaTypeAsync(definition).ConfigureAwait(false);
            await this.VisitEnumMembersAsync(definition.Members).ConfigureAwait(false);
        }

        protected virtual void ProcessTypeDefinition(IEdmTypeDefinition definition)
        {
            this.ProcessSchemaElement(definition);
            this.ProcessType(definition);
            this.ProcessSchemaType(definition);
        }

        protected virtual async Task ProcessTypeDefinitionAsync(IEdmTypeDefinition definition)
        {
            await this.ProcessSchemaElementAsync(definition).ConfigureAwait(false);
            await this.ProcessTypeAsync(definition).ConfigureAwait(false);
            await this.ProcessSchemaTypeAsync(definition).ConfigureAwait(false);
        }

        protected virtual void ProcessEntityReferenceType(IEdmEntityReferenceType definition)
        {
            this.ProcessElement(definition);
            this.ProcessType(definition);
        }

        protected virtual async Task ProcessEntityReferenceTypeAsync(IEdmEntityReferenceType definition)
        {
            await this.ProcessElementAsync(definition).ConfigureAwait(false);
            await this.ProcessTypeAsync(definition).ConfigureAwait(false);
        }

        protected virtual void ProcessStructuredType(IEdmStructuredType definition)
        {
            this.ProcessType(definition);
            this.VisitProperties(definition.DeclaredProperties);
        }

        protected virtual async Task ProcessStructuredTypeAsync(IEdmStructuredType definition)
        {
            await this.ProcessTypeAsync(definition).ConfigureAwait(false);
            await this.VisitPropertiesAsync(definition.DeclaredProperties).ConfigureAwait(false);
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

        protected virtual Task ProcessNavigationPropertyAsync(IEdmNavigationProperty property)
        {
            return this.ProcessPropertyAsync(property);
        }

        protected virtual void ProcessStructuralProperty(IEdmStructuralProperty property)
        {
            this.ProcessProperty(property);
        }

        protected virtual Task ProcessStructuralPropertyAsync(IEdmStructuralProperty property)
        {
            return this.ProcessPropertyAsync(property);
        }

        protected virtual void ProcessProperty(IEdmProperty property)
        {
            this.ProcessVocabularyAnnotatable(property);
            this.ProcessNamedElement(property);
            this.VisitTypeReference(property.Type);
        }

        protected virtual async Task ProcessPropertyAsync(IEdmProperty property)
        {
            await this.ProcessVocabularyAnnotatableAsync(property).ConfigureAwait(false);
            await this.ProcessNamedElementAsync(property).ConfigureAwait(false);
            await this.VisitTypeReferenceAsync(property.Type).ConfigureAwait(false);
        }

        protected virtual void ProcessEnumMember(IEdmEnumMember enumMember)
        {
            this.ProcessNamedElement(enumMember);
        }

        protected virtual Task ProcessEnumMemberAsync(IEdmEnumMember enumMember)
        {
            return this.ProcessNamedElementAsync(enumMember);
        }

        #endregion

        #region Annotations

        protected virtual void ProcessVocabularyAnnotation(IEdmVocabularyAnnotation annotation)
        {
            this.ProcessElement(annotation);
        }

        protected virtual Task ProcessVocabularyAnnotationAsync(IEdmVocabularyAnnotation annotation)
        {
            return this.ProcessElementAsync(annotation);
        }

        protected virtual void ProcessImmediateValueAnnotation(IEdmDirectValueAnnotation annotation)
        {
            this.ProcessNamedElement(annotation);
        }

        protected virtual Task ProcessImmediateValueAnnotationAsync(IEdmDirectValueAnnotation annotation)
        {
            return this.ProcessNamedElementAsync(annotation);
        }

        protected virtual void ProcessAnnotation(IEdmVocabularyAnnotation annotation)
        {
            this.ProcessVocabularyAnnotation(annotation);
            this.VisitExpression(annotation.Value);
        }

        protected virtual async Task ProcessAnnotationAsync(IEdmVocabularyAnnotation annotation)
        {
            await this.ProcessVocabularyAnnotationAsync(annotation).ConfigureAwait(false);
            await this.VisitExpressionAsync(annotation.Value).ConfigureAwait(false);
        }

        protected virtual void ProcessPropertyValueBinding(IEdmPropertyValueBinding binding)
        {
            this.VisitExpression(binding.Value);
        }

        protected virtual Task ProcessPropertyValueBindingAsync(IEdmPropertyValueBinding binding)
        {
            return this.VisitExpressionAsync(binding.Value);
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

        protected virtual Task ProcessStringConstantExpressionAsync(IEdmStringConstantExpression expression)
        {
            return this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessBinaryConstantExpression(IEdmBinaryConstantExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual Task ProcessBinaryConstantExpressionAsync(IEdmBinaryConstantExpression expression)
        {
            return this.ProcessExpressionAsync(expression);
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
            await this.ProcessExpressionAsync(expression).ConfigureAwait(false);
            if (expression.DeclaredType != null)
            {
                await this.VisitTypeReferenceAsync(expression.DeclaredType).ConfigureAwait(false);
            }

            await this.VisitPropertyConstructorsAsync(expression.Properties).ConfigureAwait(false);
        }

        protected virtual void ProcessPathExpression(IEdmPathExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual Task ProcessPathExpressionAsync(IEdmPathExpression expression)
        {
            return this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessPropertyPathExpression(IEdmPathExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual Task ProcessPropertyPathExpressionAsync(IEdmPathExpression expression)
        {
            return this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessNavigationPropertyPathExpression(IEdmPathExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual Task ProcessNavigationPropertyPathExpressionAsync(IEdmPathExpression expression)
        {
            return this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessAnnotationPathExpression(IEdmPathExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual Task ProcessAnnotationPathExpressionAsync(IEdmPathExpression expression)
        {
            return this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessCollectionExpression(IEdmCollectionExpression expression)
        {
            this.ProcessExpression(expression);
            this.VisitExpressions(expression.Elements);
        }

        protected virtual async Task ProcessCollectionExpressionAsync(IEdmCollectionExpression expression)
        {
            await this.ProcessExpressionAsync(expression).ConfigureAwait(false);
            await this.VisitExpressionsAsync(expression.Elements).ConfigureAwait(false);
        }

        protected virtual void ProcessLabeledExpressionReferenceExpression(IEdmLabeledExpressionReferenceExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual Task ProcessLabeledExpressionReferenceExpressionAsync(IEdmLabeledExpressionReferenceExpression expression)
        {
            return this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessIsOfExpression(IEdmIsOfExpression expression)
        {
            this.ProcessExpression(expression);
            this.VisitTypeReference(expression.Type);
            this.VisitExpression(expression.Operand);
        }

        protected virtual async Task ProcessIsOfExpressionAsync(IEdmIsOfExpression expression)
        {
            await this.ProcessExpressionAsync(expression).ConfigureAwait(false);
            await this.VisitTypeReferenceAsync(expression.Type).ConfigureAwait(false);
            await this.VisitExpressionAsync(expression.Operand).ConfigureAwait(false);
        }

        protected virtual void ProcessIntegerConstantExpression(IEdmIntegerConstantExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual Task ProcessIntegerConstantExpressionAsync(IEdmIntegerConstantExpression expression)
        {
            return this.ProcessExpressionAsync(expression);
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
            await this.ProcessExpressionAsync(expression).ConfigureAwait(false);
            await this.VisitExpressionAsync(expression.TestExpression).ConfigureAwait(false);
            await this.VisitExpressionAsync(expression.TrueExpression).ConfigureAwait(false);
            await this.VisitExpressionAsync(expression.FalseExpression).ConfigureAwait(false);
        }

        protected virtual void ProcessFunctionApplicationExpression(IEdmApplyExpression expression)
        {
            this.ProcessExpression(expression);
            this.VisitExpressions(expression.Arguments);
        }

        protected virtual async Task ProcessFunctionApplicationExpressionAsync(IEdmApplyExpression expression)
        {
            await this.ProcessExpressionAsync(expression).ConfigureAwait(false);
            await this.VisitExpressionsAsync(expression.Arguments).ConfigureAwait(false);
        }

        protected virtual void ProcessFloatingConstantExpression(IEdmFloatingConstantExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual Task ProcessFloatingConstantExpressionAsync(IEdmFloatingConstantExpression expression)
        {
            return this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessGuidConstantExpression(IEdmGuidConstantExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual Task ProcessGuidConstantExpressionAsync(IEdmGuidConstantExpression expression)
        {
            return this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessEnumMemberExpression(IEdmEnumMemberExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual Task ProcessEnumMemberExpressionAsync(IEdmEnumMemberExpression expression)
        {
            return this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessDecimalConstantExpression(IEdmDecimalConstantExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual Task ProcessDecimalConstantExpressionAsync(IEdmDecimalConstantExpression expression)
        {
            return this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessDateConstantExpression(IEdmDateConstantExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual Task ProcessDateConstantExpressionAsync(IEdmDateConstantExpression expression)
        {
            return this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessTimeOfDayConstantExpression(IEdmTimeOfDayConstantExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual Task ProcessTimeOfDayConstantExpressionAsync(IEdmTimeOfDayConstantExpression expression)
        {
            return this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessDateTimeOffsetConstantExpression(IEdmDateTimeOffsetConstantExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual Task ProcessDateTimeOffsetConstantExpressionAsync(IEdmDateTimeOffsetConstantExpression expression)
        {
            return this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessDurationConstantExpression(IEdmDurationConstantExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual Task ProcessDurationConstantExpressionAsync(IEdmDurationConstantExpression expression)
        {
            return this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessBooleanConstantExpression(IEdmBooleanConstantExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual Task ProcessBooleanConstantExpressionAsync(IEdmBooleanConstantExpression expression)
        {
            return this.ProcessExpressionAsync(expression);
        }

        protected virtual void ProcessCastExpression(IEdmCastExpression expression)
        {
            this.ProcessExpression(expression);
            this.VisitTypeReference(expression.Type);
            this.VisitExpression(expression.Operand);
        }

        protected virtual async Task ProcessCastExpressionAsync(IEdmCastExpression expression)
        {
            await this.ProcessExpressionAsync(expression).ConfigureAwait(false);
            await this.VisitTypeReferenceAsync(expression.Type).ConfigureAwait(false);
            await this.VisitExpressionAsync(expression.Operand).ConfigureAwait(false);
        }


        protected virtual void ProcessLabeledExpression(IEdmLabeledExpression element)
        {
            this.VisitExpression(element.Expression);
        }

        protected virtual async Task ProcessLabeledExpressionAsync(IEdmLabeledExpression element)
        {
            await this.VisitExpressionAsync(element.Expression).ConfigureAwait(false);
        }

        protected virtual void ProcessPropertyConstructor(IEdmPropertyConstructor constructor)
        {
            this.VisitExpression(constructor.Value);
        }

        protected virtual async Task ProcessPropertyConstructorAsync(IEdmPropertyConstructor constructor)
        {
            await this.VisitExpressionAsync(constructor.Value).ConfigureAwait(false);
        }

        protected virtual void ProcessNullConstantExpression(IEdmNullExpression expression)
        {
            this.ProcessExpression(expression);
        }

        protected virtual Task ProcessNullConstantExpressionAsync(IEdmNullExpression expression)
        {
            return this.ProcessExpressionAsync(expression);
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
            await this.ProcessVocabularyAnnotatableAsync(container).ConfigureAwait(false);
            await this.ProcessNamedElementAsync(container).ConfigureAwait(false);
            await this.VisitEntityContainerElementsAsync(container.Elements).ConfigureAwait(false);
        }

        protected virtual void ProcessEntityContainerElement(IEdmEntityContainerElement element)
        {
            this.ProcessNamedElement(element);
        }

        protected virtual Task ProcessEntityContainerElementAsync(IEdmEntityContainerElement element)
        {
            return this.ProcessNamedElementAsync(element);
        }

        protected virtual void ProcessEntitySet(IEdmEntitySet set)
        {
            this.ProcessEntityContainerElement(set);
        }

        protected virtual Task ProcessEntitySetAsync(IEdmEntitySet set)
        {
            return this.ProcessEntityContainerElementAsync(set);
        }

        protected virtual void ProcessSingleton(IEdmSingleton singleton)
        {
            this.ProcessEntityContainerElement(singleton);
        }

        protected virtual Task ProcessSingletonAsync(IEdmSingleton singleton)
        {
            return this.ProcessEntityContainerElementAsync(singleton);
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
            await this.ProcessSchemaElementAsync(action).ConfigureAwait(false);
            await this.ProcessOperationAsync(action).ConfigureAwait(false);
        }

        protected virtual void ProcessFunction(IEdmFunction function)
        {
            this.ProcessSchemaElement(function);
            this.ProcessOperation(function);
        }

        protected virtual async Task ProcessFunctionAsync(IEdmFunction function)
        {
            await this.ProcessSchemaElementAsync(function).ConfigureAwait(false);
            await this.ProcessOperationAsync(function).ConfigureAwait(false);
        }

        protected virtual void ProcessActionImport(IEdmActionImport actionImport)
        {
            this.ProcessEntityContainerElement(actionImport);
        }

        protected virtual Task ProcessActionImportAsync(IEdmActionImport actionImport)
        {
            return this.ProcessEntityContainerElementAsync(actionImport);
        }

        protected virtual void ProcessFunctionImport(IEdmFunctionImport functionImport)
        {
            this.ProcessEntityContainerElement(functionImport);
        }

        protected virtual Task ProcessFunctionImportAsync(IEdmFunctionImport functionImport)
        {
            return this.ProcessEntityContainerElementAsync(functionImport);
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
            await this.VisitOperationParametersAsync(operation.Parameters).ConfigureAwait(false);

            IEdmOperationReturn operationReturn = operation.GetReturn();
            await this.ProcessOperationReturnAsync(operationReturn).ConfigureAwait(false);
        }

        protected virtual void ProcessOperationParameter(IEdmOperationParameter parameter)
        {
            this.ProcessVocabularyAnnotatable(parameter);
            this.ProcessNamedElement(parameter);
            this.VisitTypeReference(parameter.Type);
        }

        protected virtual async Task ProcessOperationParameterAsync(IEdmOperationParameter parameter)
        {
            await this.ProcessVocabularyAnnotatableAsync(parameter).ConfigureAwait(false);
            await this.ProcessNamedElementAsync(parameter).ConfigureAwait(false);
            await this.VisitTypeReferenceAsync(parameter.Type).ConfigureAwait(false);
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

            await this.ProcessVocabularyAnnotatableAsync(operationReturn).ConfigureAwait(false);
            await this.VisitTypeReferenceAsync(operationReturn.Type).ConfigureAwait(false);
        }
        #endregion

        #endregion
    }
}

//---------------------------------------------------------------------
// <copyright file="EdmModelCsdlSerializationVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
    internal sealed class EdmModelCsdlSerializationVisitor : EdmModelVisitor
    {
        private readonly Version edmVersion;
        private readonly EdmModelCsdlSchemaWriter schemaWriter;
        private readonly List<IEdmNavigationProperty> navigationProperties = new List<IEdmNavigationProperty>();
        private readonly VersioningDictionary<string, string> namespaceAliasMappings;

        internal EdmModelCsdlSerializationVisitor(IEdmModel model, XmlWriter xmlWriter, Version edmVersion)
            : base(model)
        {
            this.edmVersion = edmVersion;
            this.namespaceAliasMappings = model.GetNamespaceAliases();
            this.schemaWriter = new EdmModelCsdlSchemaWriter(model, this.namespaceAliasMappings, xmlWriter, this.edmVersion);
        }

        public override void VisitEntityContainerElements(IEnumerable<IEdmEntityContainerElement> elements)
        {
            HashSet<string> functionImportsWritten = new HashSet<string>();
            HashSet<string> actionImportsWritten = new HashSet<string>();

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
                        // Skip actionImports that have the same name for same overloads of a function.
                        IEdmActionImport actionImport = (IEdmActionImport)element;
                        string uniqueActionName = actionImport.Name + "_" + actionImport.Action.FullName() + GetEntitySetString(actionImport);
                        if (!actionImportsWritten.Contains(uniqueActionName))
                        {
                            actionImportsWritten.Add(uniqueActionName);
                            this.ProcessActionImport(actionImport);
                        }

                        break;
                    case EdmContainerElementKind.FunctionImport:
                        // Skip functionImports that have the same name for same overloads of a function.
                        IEdmFunctionImport functionImport = (IEdmFunctionImport)element;
                        string uniqueFunctionName = functionImport.Name + "_" + functionImport.Function.FullName() + GetEntitySetString(functionImport);
                        if (!functionImportsWritten.Contains(uniqueFunctionName))
                        {
                            functionImportsWritten.Add(uniqueFunctionName);
                            this.ProcessFunctionImport(functionImport);
                        }

                        break;
                    case EdmContainerElementKind.None:
                        this.ProcessEntityContainerElement(element);
                        break;
                    default:
                        throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_ContainerElementKind(element.ContainerElementKind.ToString()));
                }
            }
        }

        internal void VisitEdmSchema(EdmSchema element, IEnumerable<KeyValuePair<string, string>> mappings)
        {
            string alias = null;
            if (this.namespaceAliasMappings != null)
            {
                this.namespaceAliasMappings.TryGetValue(element.Namespace, out alias);
            }

            this.schemaWriter.WriteSchemaElementHeader(element, alias, mappings);

            VisitSchemaElements(element.SchemaElements);

            // EntityContainers are excluded from the EdmSchema.SchemaElements property so they can be forced to the end.
            VisitCollection(element.EntityContainers, this.ProcessEntityContainer);
            foreach (KeyValuePair<string, List<IEdmVocabularyAnnotation>> annotationsForTarget in element.OutOfLineAnnotations)
            {
                this.schemaWriter.WriteAnnotationsElementHeader(annotationsForTarget.Key);
                VisitVocabularyAnnotations(annotationsForTarget.Value);
                this.schemaWriter.WriteEndElement();
            }

            this.schemaWriter.WriteEndElement();
        }

        protected override void ProcessEntityContainer(IEdmEntityContainer element)
        {
            this.BeginElement(element, this.schemaWriter.WriteEntityContainerElementHeader);
            base.ProcessEntityContainer(element);
            this.EndElement(element);
        }

        protected override void ProcessEntitySet(IEdmEntitySet element)
        {
            this.BeginElement(element, this.schemaWriter.WriteEntitySetElementHeader);

            base.ProcessEntitySet(element);

            foreach (IEdmNavigationPropertyBinding binding in element.NavigationPropertyBindings)
            {
                this.schemaWriter.WriteNavigationPropertyBinding(element, binding);
            }

            this.EndElement(element);
        }

        protected override void ProcessSingleton(IEdmSingleton element)
        {
            this.BeginElement(element, this.schemaWriter.WriteSingletonElementHeader);

            base.ProcessSingleton(element);

            foreach (IEdmNavigationPropertyBinding binding in element.NavigationPropertyBindings)
            {
                this.schemaWriter.WriteNavigationPropertyBinding(element, binding);
            }

            this.EndElement(element);
        }

        protected override void ProcessEntityType(IEdmEntityType element)
        {
            this.BeginElement(element, this.schemaWriter.WriteEntityTypeElementHeader);
            if (element.DeclaredKey != null && element.DeclaredKey.Any())
            {
                this.VisitEntityTypeDeclaredKey(element.DeclaredKey);
            }

            this.VisitProperties(element.DeclaredStructuralProperties().Cast<IEdmProperty>());
            this.VisitProperties(element.DeclaredNavigationProperties().Cast<IEdmProperty>());
            this.EndElement(element);
        }

        protected override void ProcessStructuralProperty(IEdmStructuralProperty element)
        {
            bool inlineType = IsInlineType(element.Type);
            this.BeginElement(element, (IEdmStructuralProperty t) => { this.schemaWriter.WriteStructuralPropertyElementHeader(t, inlineType); }, e => { this.ProcessFacets(e.Type, inlineType); });
            if (!inlineType)
            {
                VisitTypeReference(element.Type);
            }

            this.EndElement(element);
        }

        protected override void ProcessTypeDefinitionReference(IEdmTypeDefinitionReference element)
        {
            this.schemaWriter.WriteTypeDefinitionAttributes(element);
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
            this.BeginElement(element, this.schemaWriter.WriteNavigationPropertyElementHeader);

            if (element.OnDelete != EdmOnDeleteAction.None)
            {
                this.schemaWriter.WriteOperationActionElement(CsdlConstants.Element_OnDelete, element.OnDelete);
            }

            this.ProcessReferentialConstraint(element.ReferentialConstraint);

            this.EndElement(element);
            this.navigationProperties.Add(element);
        }

        protected override void ProcessComplexType(IEdmComplexType element)
        {
            this.BeginElement(element, this.schemaWriter.WriteComplexTypeElementHeader);
            base.ProcessComplexType(element);
            this.EndElement(element);
        }

        protected override void ProcessEnumType(IEdmEnumType element)
        {
            this.BeginElement(element, this.schemaWriter.WriteEnumTypeElementHeader);
            base.ProcessEnumType(element);
            this.EndElement(element);
        }

        protected override void ProcessEnumMember(IEdmEnumMember element)
        {
            this.BeginElement(element, this.schemaWriter.WriteEnumMemberElementHeader);
            this.EndElement(element);
        }

        protected override void ProcessTypeDefinition(IEdmTypeDefinition element)
        {
            this.BeginElement(element, this.schemaWriter.WriteTypeDefinitionElementHeader);
            base.ProcessTypeDefinition(element);
            this.EndElement(element);
        }

        protected override void ProcessTerm(IEdmTerm term)
        {
            bool inlineType = term.Type != null && IsInlineType(term.Type);
            this.BeginElement(term, (IEdmTerm t) => { this.schemaWriter.WriteTermElementHeader(t, inlineType); }, e => { this.ProcessFacets(e.Type, inlineType); });
            if (!inlineType)
            {
                if (term.Type != null)
                {
                    VisitTypeReference(term.Type);
                }
            }

            this.EndElement(term);
        }

        protected override void ProcessAction(IEdmAction action)
        {
            this.ProcessOperation(action, this.schemaWriter.WriteActionElementHeader);
        }

        protected override void ProcessFunction(IEdmFunction function)
        {
            this.ProcessOperation(function, this.schemaWriter.WriteFunctionElementHeader);
        }

        protected override void ProcessOperationParameter(IEdmOperationParameter element)
        {
            bool inlineType = IsInlineType(element.Type);
            this.BeginElement(
                element,
                (IEdmOperationParameter t) => { this.schemaWriter.WriteOperationParameterElementHeader(t, inlineType); },
                e => { this.ProcessFacets(e.Type, inlineType); });
            if (!inlineType)
            {
                VisitTypeReference(element.Type);
            }

            this.VisitPrimitiveElementAnnotations(this.Model.DirectValueAnnotations(element));
            IEdmVocabularyAnnotatable vocabularyAnnotatableElement = element as IEdmVocabularyAnnotatable;
            if (vocabularyAnnotatableElement != null)
            {
                this.VisitElementVocabularyAnnotations(this.Model.FindDeclaredVocabularyAnnotations(vocabularyAnnotatableElement).Where(a => a.IsInline(this.Model)));
            }

            this.schemaWriter.WriteOperationParameterEndElement(element);
        }

        protected override void ProcessCollectionType(IEdmCollectionType element)
        {
            bool inlineType = IsInlineType(element.ElementType);
            this.BeginElement(
                element,
                (IEdmCollectionType t) => this.schemaWriter.WriteCollectionTypeElementHeader(t, inlineType),
                e => this.ProcessFacets(e.ElementType, inlineType));
            if (!inlineType)
            {
                VisitTypeReference(element.ElementType);
            }

            this.EndElement(element);
        }

        protected override void ProcessActionImport(IEdmActionImport actionImport)
        {
            this.BeginElement(actionImport, this.schemaWriter.WriteActionImportElementHeader);
            this.EndElement(actionImport);
        }

        protected override void ProcessFunctionImport(IEdmFunctionImport functionImport)
        {
            this.BeginElement(functionImport, this.schemaWriter.WriteFunctionImportElementHeader);
            this.EndElement(functionImport);
        }

        #region Vocabulary Annotations

        protected override void ProcessAnnotation(IEdmVocabularyAnnotation annotation)
        {
            bool isInline = IsInlineExpression(annotation.Value);
            this.BeginElement(annotation, t => this.schemaWriter.WriteVocabularyAnnotationElementHeader(t, isInline));
            if (!isInline)
            {
                base.ProcessAnnotation(annotation);
            }

            this.EndElement(annotation);
        }

        #endregion

        #region Expressions

        protected override void ProcessStringConstantExpression(IEdmStringConstantExpression expression)
        {
            this.schemaWriter.WriteStringConstantExpressionElement(expression);
        }

        protected override void ProcessBinaryConstantExpression(IEdmBinaryConstantExpression expression)
        {
            this.schemaWriter.WriteBinaryConstantExpressionElement(expression);
        }

        protected override void ProcessRecordExpression(IEdmRecordExpression expression)
        {
            this.BeginElement(expression, this.schemaWriter.WriteRecordExpressionElementHeader);
            this.VisitPropertyConstructors(expression.Properties);
            this.EndElement(expression);
        }

        protected override void ProcessLabeledExpression(IEdmLabeledExpression element)
        {
            if (element.Name == null)
            {
                base.ProcessLabeledExpression(element);
            }
            else
            {
                this.BeginElement(element, this.schemaWriter.WriteLabeledElementHeader);
                base.ProcessLabeledExpression(element);
                this.EndElement(element);
            }
        }

        protected override void ProcessPropertyConstructor(IEdmPropertyConstructor constructor)
        {
            bool isInline = IsInlineExpression(constructor.Value);
            this.BeginElement(constructor, t => this.schemaWriter.WritePropertyConstructorElementHeader(t, isInline));
            if (!isInline)
            {
                base.ProcessPropertyConstructor(constructor);
            }

            this.EndElement(constructor);
        }

        protected override void ProcessPathExpression(IEdmPathExpression expression)
        {
            this.schemaWriter.WritePathExpressionElement(expression);
        }

        protected override void ProcessPropertyPathExpression(IEdmPathExpression expression)
        {
            this.schemaWriter.WritePropertyPathExpressionElement(expression);
        }

        protected override void ProcessNavigationPropertyPathExpression(IEdmPathExpression expression)
        {
            this.schemaWriter.WriteNavigationPropertyPathExpressionElement(expression);
        }

        protected override void ProcessCollectionExpression(IEdmCollectionExpression expression)
        {
            this.BeginElement(expression, this.schemaWriter.WriteCollectionExpressionElementHeader);
            this.VisitExpressions(expression.Elements);
            this.EndElement(expression);
        }

        protected override void ProcessIsTypeExpression(IEdmIsTypeExpression expression)
        {
            bool inlineType = IsInlineType(expression.Type);
            this.BeginElement(expression, (IEdmIsTypeExpression t) => { this.schemaWriter.WriteIsTypeExpressionElementHeader(t, inlineType); }, e => { this.ProcessFacets(e.Type, inlineType); });
            if (!inlineType)
            {
                VisitTypeReference(expression.Type);
            }

            this.VisitExpression(expression.Operand);
            this.EndElement(expression);
        }

        protected override void ProcessIntegerConstantExpression(IEdmIntegerConstantExpression expression)
        {
            this.schemaWriter.WriteIntegerConstantExpressionElement(expression);
        }

        protected override void ProcessIfExpression(IEdmIfExpression expression)
        {
            this.BeginElement(expression, this.schemaWriter.WriteIfExpressionElementHeader);
            base.ProcessIfExpression(expression);
            this.EndElement(expression);
        }

        protected override void ProcessFunctionApplicationExpression(IEdmApplyExpression expression)
        {
            this.BeginElement(expression, e => this.schemaWriter.WriteFunctionApplicationElementHeader(e));
            this.VisitExpressions(expression.Arguments);
            this.EndElement(expression);
        }

        protected override void ProcessFloatingConstantExpression(IEdmFloatingConstantExpression expression)
        {
            this.schemaWriter.WriteFloatingConstantExpressionElement(expression);
        }

        protected override void ProcessGuidConstantExpression(IEdmGuidConstantExpression expression)
        {
            this.schemaWriter.WriteGuidConstantExpressionElement(expression);
        }

        protected override void ProcessEnumMemberExpression(IEdmEnumMemberExpression expression)
        {
            this.schemaWriter.WriteEnumMemberExpressionElement(expression);
        }

        protected override void ProcessDecimalConstantExpression(IEdmDecimalConstantExpression expression)
        {
            this.schemaWriter.WriteDecimalConstantExpressionElement(expression);
        }

        protected override void ProcessDateConstantExpression(IEdmDateConstantExpression expression)
        {
            this.schemaWriter.WriteDateConstantExpressionElement(expression);
        }

        protected override void ProcessDateTimeOffsetConstantExpression(IEdmDateTimeOffsetConstantExpression expression)
        {
            this.schemaWriter.WriteDateTimeOffsetConstantExpressionElement(expression);
        }

        protected override void ProcessDurationConstantExpression(IEdmDurationConstantExpression expression)
        {
            this.schemaWriter.WriteDurationConstantExpressionElement(expression);
        }

        protected override void ProcessTimeOfDayConstantExpression(IEdmTimeOfDayConstantExpression expression)
        {
            this.schemaWriter.WriteTimeOfDayConstantExpressionElement(expression);
        }

        protected override void ProcessBooleanConstantExpression(IEdmBooleanConstantExpression expression)
        {
            this.schemaWriter.WriteBooleanConstantExpressionElement(expression);
        }

        protected override void ProcessNullConstantExpression(IEdmNullExpression expression)
        {
            this.schemaWriter.WriteNullConstantExpressionElement(expression);
        }

        protected override void ProcessCastExpression(IEdmCastExpression expression)
        {
            bool inlineType = IsInlineType(expression.Type);
            this.BeginElement(expression, (IEdmCastExpression t) => { this.schemaWriter.WriteCastExpressionElementHeader(t, inlineType); }, e => { this.ProcessFacets(e.Type, inlineType); });
            if (!inlineType)
            {
                VisitTypeReference(expression.Type);
            }

            this.VisitExpression(expression.Operand);
            this.EndElement(expression);
        }

        #endregion

        private static bool IsInlineType(IEdmTypeReference reference)
        {
            if (reference.Definition is IEdmSchemaElement || reference.IsEntityReference())
            {
                return true;
            }
            else if (reference.IsCollection())
            {
                return reference.AsCollection().CollectionDefinition().ElementType.Definition is IEdmSchemaElement;
            }

            return false;
        }

        private static bool IsInlineExpression(IEdmExpression expression)
        {
            switch (expression.ExpressionKind)
            {
                case EdmExpressionKind.BinaryConstant:
                case EdmExpressionKind.BooleanConstant:
                case EdmExpressionKind.DateConstant:
                case EdmExpressionKind.DateTimeOffsetConstant:
                case EdmExpressionKind.DecimalConstant:
                case EdmExpressionKind.DurationConstant:
                case EdmExpressionKind.FloatingConstant:
                case EdmExpressionKind.GuidConstant:
                case EdmExpressionKind.IntegerConstant:
                case EdmExpressionKind.Path:
                case EdmExpressionKind.PropertyPath:
                case EdmExpressionKind.NavigationPropertyPath:
                case EdmExpressionKind.StringConstant:
                case EdmExpressionKind.TimeOfDayConstant:
                    return true;
            }

            return false;
        }

        private static string GetEntitySetString(IEdmOperationImport operationImport)
        {
            if (operationImport.EntitySet != null)
            {
                var pathExpression = operationImport.EntitySet as IEdmPathExpression;
                if (pathExpression != null)
                {
                    return EdmModelCsdlSchemaWriter.PathAsXml(pathExpression.PathSegments);
                }
            }

            return null;
        }

        private void ProcessOperation<TOperation>(TOperation operation, Action<TOperation> writeElementAction) where TOperation : IEdmOperation
        {
            this.BeginElement(operation, writeElementAction);
            this.VisitOperationParameters(operation.Parameters);
            this.ProcessOperationReturnType(operation.ReturnType);
            this.EndElement(operation);
        }

        private void ProcessOperationReturnType(IEdmTypeReference operationReturnType)
        {
            if (operationReturnType == null)
            {
                return;
            }

            bool inlineType = IsInlineType(operationReturnType);
            this.BeginElement(
                operationReturnType,
                type => this.schemaWriter.WriteReturnTypeElementHeader(),
                type =>
                {
                    if (inlineType)
                    {
                        this.schemaWriter.WriteTypeAttribute(type);
                        this.ProcessFacets(type, true /*inlineType*/);
                    }
                    else
                    {
                        this.VisitTypeReference(type);
                    }
                });
            this.EndElement(operationReturnType);
        }

        private void ProcessReferentialConstraint(IEdmReferentialConstraint element)
        {
            if (element != null)
            {
                foreach (var pair in element.PropertyPairs)
                {
                    this.schemaWriter.WriteReferentialConstraintPair(pair);
                }
            }
        }

        private void ProcessFacets(IEdmTypeReference element, bool inlineType)
        {
            if (element != null)
            {
                if (element.IsEntityReference())
                {
                    // No facets get serialized for an entity reference.
                    return;
                }

                if (inlineType)
                {
                    if (element.TypeKind() == EdmTypeKind.Collection)
                    {
                        IEdmCollectionTypeReference collectionElement = element.AsCollection();
                        this.schemaWriter.WriteNullableAttribute(collectionElement.CollectionDefinition().ElementType);
                        VisitTypeReference(collectionElement.CollectionDefinition().ElementType);
                    }
                    else
                    {
                        this.schemaWriter.WriteNullableAttribute(element);
                        VisitTypeReference(element);
                    }
                }
            }
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

        private void VisitAttributeAnnotations(IEnumerable<IEdmDirectValueAnnotation> annotations)
        {
            foreach (IEdmDirectValueAnnotation annotation in annotations)
            {
                if (annotation.NamespaceUri != EdmConstants.InternalUri)
                {
                    var edmValue = annotation.Value as IEdmValue;
                    if (edmValue != null)
                    {
                        if (!edmValue.IsSerializedAsElement(this.Model))
                        {
                            if (edmValue.Type.TypeKind() == EdmTypeKind.Primitive)
                            {
                                this.ProcessAttributeAnnotation(annotation);
                            }
                        }
                    }
                }
            }
        }

        private void VisitPrimitiveElementAnnotations(IEnumerable<IEdmDirectValueAnnotation> annotations)
        {
            foreach (IEdmDirectValueAnnotation annotation in annotations)
            {
                if (annotation.NamespaceUri != EdmConstants.InternalUri)
                {
                    var edmValue = annotation.Value as IEdmValue;
                    if (edmValue != null)
                    {
                        if (edmValue.IsSerializedAsElement(this.Model))
                        {
                            if (edmValue.Type.TypeKind() == EdmTypeKind.Primitive)
                            {
                                this.ProcessElementAnnotation(annotation);
                            }
                        }
                    }
                }
            }
        }

        private void ProcessAttributeAnnotation(IEdmDirectValueAnnotation annotation)
        {
            this.schemaWriter.WriteAnnotationStringAttribute(annotation);
        }

        private void ProcessElementAnnotation(IEdmDirectValueAnnotation annotation)
        {
            this.schemaWriter.WriteAnnotationStringElement(annotation);
        }

        private void VisitElementVocabularyAnnotations(IEnumerable<IEdmVocabularyAnnotation> annotations)
        {
            foreach (IEdmVocabularyAnnotation annotation in annotations)
            {
                this.ProcessAnnotation(annotation);
            }
        }

        private void BeginElement<TElement>(TElement element, Action<TElement> elementHeaderWriter, params Action<TElement>[] additionalAttributeWriters)
            where TElement : IEdmElement
        {
            elementHeaderWriter(element);
            if (additionalAttributeWriters != null)
            {
                foreach (Action<TElement> action in additionalAttributeWriters)
                {
                    action(element);
                }
            }

            this.VisitAttributeAnnotations(this.Model.DirectValueAnnotations(element));
        }

        private void EndElement(IEdmElement element)
        {
            this.VisitPrimitiveElementAnnotations(this.Model.DirectValueAnnotations(element));
            IEdmVocabularyAnnotatable vocabularyAnnotatableElement = element as IEdmVocabularyAnnotatable;
            if (vocabularyAnnotatableElement != null)
            {
                this.VisitElementVocabularyAnnotations(this.Model.FindDeclaredVocabularyAnnotations(vocabularyAnnotatableElement).Where(a => a.IsInline(this.Model)));
            }

            this.schemaWriter.WriteEndElement();
        }
    }
}

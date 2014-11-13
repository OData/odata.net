//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Values;

namespace Microsoft.Data.Edm.Csdl.Internal.Serialization
{
    internal sealed class EdmModelCsdlSerializationVisitor : EdmModelVisitor
    {
        private readonly Version edmVersion;
        private readonly EdmModelCsdlSchemaWriter schemaWriter;
        private readonly List<IEdmNavigationProperty> navigationProperties = new List<IEdmNavigationProperty>();
        private readonly Dictionary<string, List<IEdmNavigationProperty>> associations = new Dictionary<string, List<IEdmNavigationProperty>>();
        private readonly Dictionary<string, List<TupleInternal<IEdmEntitySet, IEdmNavigationProperty>>> associationSets = new Dictionary<string, List<TupleInternal<IEdmEntitySet, IEdmNavigationProperty>>>();
        private readonly VersioningDictionary<string, string> namespaceAliasMappings;

        internal EdmModelCsdlSerializationVisitor(IEdmModel model, XmlWriter xmlWriter, Version edmVersion)
            : base(model)
        {
            this.edmVersion = edmVersion;
            this.namespaceAliasMappings = model.GetNamespaceAliases();
            this.schemaWriter = new EdmModelCsdlSchemaWriter(model, this.namespaceAliasMappings, xmlWriter, this.edmVersion);
        }

        internal void VisitEdmSchema(EdmSchema element, IEnumerable<KeyValuePair<string, string>> mappings)
        {
            string alias = null;
            if (this.namespaceAliasMappings != null) 
            {
                this.namespaceAliasMappings.TryGetValue(element.Namespace, out alias);
            }

            this.schemaWriter.WriteSchemaElementHeader(element, alias, mappings);
            foreach (string usingNamespace in element.NamespaceUsings)
            {
                if (usingNamespace != element.Namespace)
                {
                    if (this.namespaceAliasMappings != null && this.namespaceAliasMappings.TryGetValue(usingNamespace, out alias))
                    {
                        this.schemaWriter.WriteNamespaceUsingElement(usingNamespace, alias);
                    }
                }
            }

            VisitSchemaElements(element.SchemaElements);
            foreach (IEdmNavigationProperty navigationProperty in element.AssociationNavigationProperties)
            {
                string associationName = this.Model.GetAssociationFullName(navigationProperty);
                List<IEdmNavigationProperty> handledNavigationProperties;
                if (!this.associations.TryGetValue(associationName, out handledNavigationProperties))
                {
                    handledNavigationProperties = new List<IEdmNavigationProperty>();
                    this.associations.Add(associationName, handledNavigationProperties);
                }

                // This prevents us from losing associations if they share the same name.
                if (!handledNavigationProperties.Any(np => this.SharesAssociation(np, navigationProperty)))
                {
                    handledNavigationProperties.Add(navigationProperty);
                    handledNavigationProperties.Add(navigationProperty.Partner);
                    this.ProcessAssociation(navigationProperty);
                }
            }

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

            foreach (IEdmEntitySet entitySet in element.EntitySets())
            {
                foreach (IEdmNavigationTargetMapping mapping in entitySet.NavigationTargets)
                {
                    string associationSetName = this.Model.GetAssociationFullName(mapping.NavigationProperty);
                    List<TupleInternal<IEdmEntitySet, IEdmNavigationProperty>> handledAssociationSets;
                    if (!this.associationSets.TryGetValue(associationSetName, out handledAssociationSets))
                    {
                        handledAssociationSets = new List<TupleInternal<IEdmEntitySet, IEdmNavigationProperty>>();
                        this.associationSets[associationSetName] = handledAssociationSets;
                    }

                    // This prevents us from losing association sets if they share the same name.
                    if (!handledAssociationSets.Any(set => this.SharesAssociationSet(set.Item1, set.Item2, entitySet, mapping.NavigationProperty)))
                    {
                        handledAssociationSets.Add(new TupleInternal<IEdmEntitySet, IEdmNavigationProperty>(entitySet, mapping.NavigationProperty));
                        handledAssociationSets.Add(new TupleInternal<IEdmEntitySet, IEdmNavigationProperty>(mapping.TargetEntitySet, mapping.NavigationProperty.Partner));

                        this.ProcessAssociationSet(entitySet, mapping.NavigationProperty);
                    }
                }
            }

            this.associationSets.Clear();

            this.EndElement(element);
        }

        protected override void ProcessEntitySet(IEdmEntitySet element)
        {
            this.BeginElement(element, this.schemaWriter.WriteEntitySetElementHeader);
            base.ProcessEntitySet(element);
            this.EndElement(element);
        }

        protected override void ProcessEntityType(IEdmEntityType element)
        {
            this.BeginElement(element, this.schemaWriter.WriteEntityTypeElementHeader);
            if (element.DeclaredKey != null && element.DeclaredKey.Count() > 0 && element.BaseType == null)
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

        protected override void ProcessValueTerm(IEdmValueTerm term)
        {
            bool inlineType = term.Type != null && IsInlineType(term.Type);
            this.BeginElement(term, (IEdmValueTerm t) => { this.schemaWriter.WriteValueTermElementHeader(t, inlineType); }, e => { this.ProcessFacets(e.Type, inlineType); });
            if (!inlineType)
            {
                if (term.Type != null)
                {
                    VisitTypeReference(term.Type);
                }
            }

            this.EndElement(term);
        }

        protected override void ProcessFunction(IEdmFunction element)
        {
            if (element.ReturnType != null)
            {
                bool inlineReturnType = IsInlineType(element.ReturnType);
                this.BeginElement(element, (IEdmFunction f) => { this.schemaWriter.WriteFunctionElementHeader(f, inlineReturnType); }, f => { this.ProcessFacets(f.ReturnType, inlineReturnType); });
                if (!inlineReturnType)
                {
                    this.schemaWriter.WriteReturnTypeElementHeader();
                    this.VisitTypeReference(element.ReturnType);
                    this.schemaWriter.WriteEndElement();
                }
            }
            else
            {
                this.BeginElement(element, (IEdmFunction t) => { this.schemaWriter.WriteFunctionElementHeader(t, false /*Inline ReturnType*/); });
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
            this.BeginElement(
                element, 
                (IEdmFunctionParameter t) => { this.schemaWriter.WriteFunctionParameterElementHeader(t, inlineType); },
                e => { this.ProcessFacets(e.Type, inlineType); });
            if (!inlineType)
            {
                VisitTypeReference(element.Type);
            }

            this.EndElement(element);
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

        protected override void ProcessRowType(IEdmRowType element)
        {
            this.schemaWriter.WriteRowTypeElementHeader();
            base.ProcessRowType(element);
            this.schemaWriter.WriteEndElement();
        }

        protected override void ProcessFunctionImport(IEdmFunctionImport functionImport)
        {
            if (functionImport.ReturnType != null && !IsInlineType(functionImport.ReturnType))
            {
                throw new InvalidOperationException(Edm.Strings.Serializer_NonInlineFunctionImportReturnType(functionImport.Container.FullName() + "/" + functionImport.Name));
            }

            this.BeginElement(functionImport, this.schemaWriter.WriteFunctionImportElementHeader);
            this.VisitFunctionParameters(functionImport.Parameters);
            this.EndElement(functionImport);
        }

        #region Vocabulary Annotations

        protected override void ProcessValueAnnotation(IEdmValueAnnotation annotation)
        {
            bool isInline = IsInlineExpression(annotation.Value);
            this.BeginElement(annotation, t => this.schemaWriter.WriteValueAnnotationElementHeader(t, isInline));
            if (!isInline)
            {
                base.ProcessValueAnnotation(annotation);
            }

            this.EndElement(annotation);
        }

        protected override void ProcessTypeAnnotation(IEdmTypeAnnotation annotation)
        {
            this.BeginElement(annotation, this.schemaWriter.WriteTypeAnnotationElementHeader);
            base.ProcessTypeAnnotation(annotation);
            this.EndElement(annotation);
        }

        protected override void ProcessPropertyValueBinding(IEdmPropertyValueBinding binding)
        {
            bool isInline = IsInlineExpression(binding.Value);
            this.BeginElement(binding, t => this.schemaWriter.WritePropertyValueElementHeader(t, isInline));
            if (!isInline)
            {
                base.ProcessPropertyValueBinding(binding);
            }

            this.EndElement(binding);
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

        protected override void ProcessPropertyReferenceExpression(IEdmPropertyReferenceExpression expression)
        {
            this.BeginElement(expression, this.schemaWriter.WritePropertyReferenceExpressionElementHeader);
            if (expression.Base != null)
            {
                this.VisitExpression(expression.Base);
            }

            this.EndElement(expression);
        }

        protected override void ProcessPathExpression(IEdmPathExpression expression)
        {
            this.schemaWriter.WritePathExpressionElement(expression);
        }

        protected override void ProcessParameterReferenceExpression(IEdmParameterReferenceExpression expression)
        {
            this.schemaWriter.WriteParameterReferenceExpressionElement(expression);
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

        protected override void ProcessFunctionReferenceExpression(IEdmFunctionReferenceExpression expression)
        {
            this.schemaWriter.WriteFunctionReferenceExpressionElement(expression);
        }

        protected override void ProcessFunctionApplicationExpression(IEdmApplyExpression expression)
        {
            bool isFunction = expression.AppliedFunction.ExpressionKind == EdmExpressionKind.FunctionReference;
            this.BeginElement(expression, e => this.schemaWriter.WriteFunctionApplicationElementHeader(e, isFunction));
            if (!isFunction)
            {
                this.VisitExpression(expression.AppliedFunction);
            }

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

        protected override void ProcessEnumMemberReferenceExpression(IEdmEnumMemberReferenceExpression expression)
        {
            this.schemaWriter.WriteEnumMemberReferenceExpressionElement(expression);
        }

        protected override void ProcessEntitySetReferenceExpression(IEdmEntitySetReferenceExpression expression)
        {
            this.schemaWriter.WriteEntitySetReferenceExpressionElement(expression);
        }

        protected override void ProcessDecimalConstantExpression(IEdmDecimalConstantExpression expression)
        {
            this.schemaWriter.WriteDecimalConstantExpressionElement(expression);
        }

        protected override void ProcessDateTimeConstantExpression(IEdmDateTimeConstantExpression expression)
        {
            this.schemaWriter.WriteDateTimeConstantExpressionElement(expression);
        }

        protected override void ProcessDateTimeOffsetConstantExpression(IEdmDateTimeOffsetConstantExpression expression)
        {
            this.schemaWriter.WriteDateTimeOffsetConstantExpressionElement(expression);
        }

        protected override void ProcessBooleanConstantExpression(IEdmBooleanConstantExpression expression)
        {
            this.schemaWriter.WriteBooleanConstantExpressionElement(expression);
        }

        protected override void ProcessNullConstantExpression(IEdmNullExpression expression)
        {
            this.schemaWriter.WriteNullConstantExpressionElement(expression);
        }

        protected override void ProcessAssertTypeExpression(IEdmAssertTypeExpression expression)
        {
            bool inlineType = IsInlineType(expression.Type);
            this.BeginElement(expression, (IEdmAssertTypeExpression t) => { this.schemaWriter.WriteAssertTypeExpressionElementHeader(t, inlineType); }, e => { this.ProcessFacets(e.Type, inlineType); });
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
                case EdmExpressionKind.DateTimeConstant:
                case EdmExpressionKind.DateTimeOffsetConstant:
                case EdmExpressionKind.DecimalConstant:
                case EdmExpressionKind.FloatingConstant:
                case EdmExpressionKind.GuidConstant:
                case EdmExpressionKind.IntegerConstant:
                case EdmExpressionKind.Path:
                case EdmExpressionKind.StringConstant:
                case EdmExpressionKind.TimeConstant:
                    return true;
            }

            return false;
        }

        private void ProcessAnnotations(IEnumerable<IEdmDirectValueAnnotation> annotations)
        {
            this.VisitAttributeAnnotations(annotations);
            foreach (IEdmDirectValueAnnotation annotation in annotations)
            {
                if (annotation.NamespaceUri == EdmConstants.DocumentationUri && annotation.Name == EdmConstants.DocumentationAnnotation)
                {
                    this.ProcessEdmDocumentation((IEdmDocumentation)annotation.Value);
                }
            }
        }

        private void ProcessAssociation(IEdmNavigationProperty element)
        {
            IEdmNavigationProperty end1 = element.GetPrimary();
            IEdmNavigationProperty end2 = end1.Partner;

            IEnumerable<IEdmDirectValueAnnotation> associationAnnotations;
            IEnumerable<IEdmDirectValueAnnotation> end1Annotations;
            IEnumerable<IEdmDirectValueAnnotation> end2Annotations;
            IEnumerable<IEdmDirectValueAnnotation> constraintAnnotations;
            this.Model.GetAssociationAnnotations(element, out associationAnnotations, out end1Annotations, out end2Annotations, out constraintAnnotations);

            this.schemaWriter.WriteAssociationElementHeader(end1);
            this.ProcessAnnotations(associationAnnotations);
            
            this.ProcessAssociationEnd(end1, end1 == element ? end1Annotations : end2Annotations);
            this.ProcessAssociationEnd(end2, end1 == element ? end2Annotations : end1Annotations);
            this.ProcessReferentialConstraint(end1, constraintAnnotations);

            this.VisitPrimitiveElementAnnotations(associationAnnotations);
            this.schemaWriter.WriteEndElement();
        }

        private void ProcessAssociationEnd(IEdmNavigationProperty element, IEnumerable<IEdmDirectValueAnnotation> annotations)
        {
            this.schemaWriter.WriteAssociationEndElementHeader(element);
            this.ProcessAnnotations(annotations);

            if (element.OnDelete != EdmOnDeleteAction.None)
            {
                this.schemaWriter.WriteOperationActionElement(CsdlConstants.Element_OnDelete, element.OnDelete);
            }

            this.VisitPrimitiveElementAnnotations(annotations);
            this.schemaWriter.WriteEndElement();
        }

        private void ProcessReferentialConstraint(IEdmNavigationProperty element, IEnumerable<IEdmDirectValueAnnotation> annotations)
        {
            IEdmNavigationProperty principalElement;
            if (element.DependentProperties != null)
            {
                principalElement = element.Partner;
            }
            else if (element.Partner.DependentProperties != null)
            {
                principalElement = element;
            }
            else
            {
                return;
            }

            this.schemaWriter.WriteReferentialConstraintElementHeader(principalElement);
            this.ProcessAnnotations(annotations);
            this.schemaWriter.WriteReferentialConstraintPrincipalEndElementHeader(principalElement);
            this.VisitPropertyRefs(((IEdmEntityType)principalElement.DeclaringType).Key());
            this.schemaWriter.WriteEndElement();
            this.schemaWriter.WriteReferentialConstraintDependentEndElementHeader(principalElement.Partner);
            this.VisitPropertyRefs(principalElement.Partner.DependentProperties);
            this.schemaWriter.WriteEndElement();
            this.VisitPrimitiveElementAnnotations(annotations);
            this.schemaWriter.WriteEndElement();
        }

        private void ProcessAssociationSet(IEdmEntitySet entitySet, IEdmNavigationProperty property)
        {
            IEnumerable<IEdmDirectValueAnnotation> associationSetAnnotations;
            IEnumerable<IEdmDirectValueAnnotation> end1Annotations;
            IEnumerable<IEdmDirectValueAnnotation> end2Annotations;
            this.Model.GetAssociationSetAnnotations(entitySet, property, out associationSetAnnotations, out end1Annotations, out end2Annotations);

            this.schemaWriter.WriteAssociationSetElementHeader(entitySet, property);
            this.ProcessAnnotations(associationSetAnnotations);

            this.ProcessAssociationSetEnd(entitySet, property, end1Annotations);

            IEdmEntitySet otherEntitySet = entitySet.FindNavigationTarget(property);
            if (otherEntitySet != null)
            {
                this.ProcessAssociationSetEnd(otherEntitySet, property.Partner, end2Annotations);
            }

            this.VisitPrimitiveElementAnnotations(associationSetAnnotations);
            this.schemaWriter.WriteEndElement();
        }

        private void ProcessAssociationSetEnd(IEdmEntitySet entitySet, IEdmNavigationProperty property, IEnumerable<IEdmDirectValueAnnotation> annotations)
        {
            this.schemaWriter.WriteAssociationSetEndElementHeader(entitySet, property);
            this.ProcessAnnotations(annotations);
            this.VisitPrimitiveElementAnnotations(annotations);
            this.schemaWriter.WriteEndElement();
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
                switch (annotation.Term.TermKind)
                {
                    case EdmTermKind.Type:
                        this.ProcessTypeAnnotation((IEdmTypeAnnotation)annotation);
                        break;
                    case EdmTermKind.Value:
                        this.ProcessValueAnnotation((IEdmValueAnnotation)annotation);
                        break;
                    case EdmTermKind.None:
                        this.ProcessVocabularyAnnotation(annotation);
                        break;
                    default:
                        throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_TermKind(annotation.Term.TermKind));
                }
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
            IEdmDocumentation documentation = this.Model.GetDocumentation(element);
            if (documentation != null)
            {
                this.ProcessEdmDocumentation(documentation);
            }
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

        private void ProcessEdmDocumentation(IEdmDocumentation element)
        {
            this.schemaWriter.WriteDocumentationElement(element);
        }

        private bool SharesAssociation(IEdmNavigationProperty thisNavprop, IEdmNavigationProperty thatNavprop)
        {
            if (thisNavprop == thatNavprop)
            {
                return true;
            }

            if (this.Model.GetAssociationName(thisNavprop) != this.Model.GetAssociationName(thatNavprop))
            {
                return false;
            }

            IEdmNavigationProperty thisPrimary = thisNavprop.GetPrimary();
            IEdmNavigationProperty thatPrimary = thatNavprop.GetPrimary();
            if (!this.SharesEnd(thisPrimary, thatPrimary))
            {
                return false;
            }
            
            IEdmNavigationProperty thisDependent = thisPrimary.Partner;
            IEdmNavigationProperty thatDependent = thatPrimary.Partner;
            if (!this.SharesEnd(thisDependent, thatDependent))
            {
                return false;
            }

            IEnumerable<IEdmStructuralProperty> thisPrincipalProperties = ((IEdmEntityType)thisPrimary.DeclaringType).Key();
            IEnumerable<IEdmStructuralProperty> thatPrincipalProperties = ((IEdmEntityType)thatPrimary.DeclaringType).Key();
            if (!this.SharesReferentialConstraintEnd(thisPrincipalProperties, thatPrincipalProperties))
            {
                return false;
            }

            IEnumerable<IEdmStructuralProperty> thisDependentProperties = thisDependent.DependentProperties;
            IEnumerable<IEdmStructuralProperty> thatDependentProperties = thisDependent.DependentProperties;
            if (thisDependentProperties != null &&
                thatDependentProperties != null &&
                !this.SharesReferentialConstraintEnd(thisDependentProperties, thatDependentProperties))
            {
                return false;
            }

            IEnumerable<IEdmDirectValueAnnotation> thisAssociationAnnotations;
            IEnumerable<IEdmDirectValueAnnotation> thisEnd1Annotations;
            IEnumerable<IEdmDirectValueAnnotation> thisEnd2Annotations;
            IEnumerable<IEdmDirectValueAnnotation> thisConstraintAnnotations;
            this.Model.GetAssociationAnnotations(thisPrimary, out thisAssociationAnnotations, out thisEnd1Annotations, out thisEnd2Annotations, out thisConstraintAnnotations);

            IEnumerable<IEdmDirectValueAnnotation> thatAssociationAnnotations;
            IEnumerable<IEdmDirectValueAnnotation> thatEnd1Annotations;
            IEnumerable<IEdmDirectValueAnnotation> thatEnd2Annotations;
            IEnumerable<IEdmDirectValueAnnotation> thatConstraintAnnotations;
            this.Model.GetAssociationAnnotations(thatPrimary, out thatAssociationAnnotations, out thatEnd1Annotations, out thatEnd2Annotations, out thatConstraintAnnotations);

            if (!(thisAssociationAnnotations == thatAssociationAnnotations &&
                thisEnd1Annotations == thatEnd1Annotations &&
                thisEnd2Annotations == thatEnd2Annotations &&
                thisConstraintAnnotations == thatConstraintAnnotations))
            {
                return false;
            }

            return true;
        }

        private bool SharesEnd(IEdmNavigationProperty end1, IEdmNavigationProperty end2)
        {
            if (!(((IEdmEntityType)end1.DeclaringType).FullName() == ((IEdmEntityType)end2.DeclaringType).FullName() &&
                this.Model.GetAssociationEndName(end1) == this.Model.GetAssociationEndName(end2) &&
                end1.Multiplicity() == end2.Multiplicity() &&
                end1.OnDelete == end2.OnDelete))
            {
                return false;
            }

            return true;
        }

        private bool SharesReferentialConstraintEnd(IEnumerable<IEdmStructuralProperty> theseProperties, IEnumerable<IEdmStructuralProperty> thoseProperties)
        {
            if (theseProperties.Count() != thoseProperties.Count())
            {
                return false;
            }

            IEnumerator<IEdmStructuralProperty> thesePropertiesEnumerator = theseProperties.GetEnumerator();
            foreach (IEdmStructuralProperty thatProperty in thoseProperties)
            {
                thesePropertiesEnumerator.MoveNext();
                if (!(thesePropertiesEnumerator.Current.Name == thatProperty.Name))
                {
                    return false;
                }
            }

            return true;
        }

        private bool SharesAssociationSet(IEdmEntitySet thisEntitySet, IEdmNavigationProperty thisNavprop, IEdmEntitySet thatEntitySet, IEdmNavigationProperty thatNavprop)
        {
            if (thisEntitySet == thatEntitySet && thisNavprop == thatNavprop)
            {
                return true;
            }

            // Association Set
            if (!(this.Model.GetAssociationSetName(thisEntitySet, thisNavprop) == this.Model.GetAssociationSetName(thatEntitySet, thatNavprop) &&
                this.Model.GetAssociationFullName(thisNavprop) == this.Model.GetAssociationFullName(thatNavprop)))
            {
                return false;
            }

            // End 1
            if (!(this.Model.GetAssociationEndName(thisNavprop) == this.Model.GetAssociationEndName(thatNavprop) &&
                thisEntitySet.Name == thatEntitySet.Name))
            {
                return false;
            }

            // End 2
            IEdmEntitySet thisOtherEntitySet = thisEntitySet.FindNavigationTarget(thisNavprop);
            IEdmEntitySet thatOtherEntitySet = thatEntitySet.FindNavigationTarget(thatNavprop);

            if (thisOtherEntitySet == null)
            {
                if (thatOtherEntitySet != null)
                {
                    return false;
                }
            }
            else
            {
                if (thatOtherEntitySet == null)
                {
                    return false;
                }

                if (!(this.Model.GetAssociationEndName(thisNavprop.Partner) == this.Model.GetAssociationEndName(thatNavprop.Partner) &&
                    thisOtherEntitySet.Name == thatOtherEntitySet.Name))
                {
                    return false;
                }
            }

            // Annotations
            IEnumerable<IEdmDirectValueAnnotation> thisAssociationSetAnnotations;
            IEnumerable<IEdmDirectValueAnnotation> thisEnd1Annotations;
            IEnumerable<IEdmDirectValueAnnotation> thisEnd2Annotations;
            this.Model.GetAssociationSetAnnotations(thisEntitySet, thisNavprop, out thisAssociationSetAnnotations, out thisEnd1Annotations, out thisEnd2Annotations);

            IEnumerable<IEdmDirectValueAnnotation> thatAssociationSetAnnotations;
            IEnumerable<IEdmDirectValueAnnotation> thatEnd1Annotations;
            IEnumerable<IEdmDirectValueAnnotation> thatEnd2Annotations;
            this.Model.GetAssociationSetAnnotations(thatEntitySet, thatNavprop, out thatAssociationSetAnnotations, out thatEnd1Annotations, out thatEnd2Annotations);

            if (!(thisAssociationSetAnnotations == thatAssociationSetAnnotations &&
                thisEnd1Annotations == thatEnd1Annotations &&
                thisEnd2Annotations == thatEnd2Annotations))
            {
                return false;
            }

            return true;
        }
    }
}

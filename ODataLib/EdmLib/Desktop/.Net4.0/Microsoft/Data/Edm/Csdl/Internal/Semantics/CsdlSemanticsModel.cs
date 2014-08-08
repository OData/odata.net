//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Library.Annotations;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlMetadataModel.
    /// </summary>
    internal class CsdlSemanticsModel : EdmModelBase, IEdmCheckable
    {
        private readonly CsdlModel astModel;
        private readonly List<CsdlSemanticsSchema> schemata = new List<CsdlSemanticsSchema>();
        private readonly Dictionary<string, List<CsdlSemanticsAnnotations>> outOfLineAnnotations = new Dictionary<string, List<CsdlSemanticsAnnotations>>();
        private readonly Dictionary<CsdlVocabularyAnnotationBase, CsdlSemanticsVocabularyAnnotation> wrappedAnnotations = new Dictionary<CsdlVocabularyAnnotationBase, CsdlSemanticsVocabularyAnnotation>();
        private readonly Dictionary<string, IEdmAssociation> associationDictionary = new Dictionary<string, IEdmAssociation>();
        private readonly Dictionary<string, List<IEdmStructuredType>> derivedTypeMappings = new Dictionary<string, List<IEdmStructuredType>>();

        public CsdlSemanticsModel(CsdlModel astModel, EdmDirectValueAnnotationsManager annotationsManager, IEnumerable<IEdmModel> referencedModels)
            : base(referencedModels, annotationsManager)
        {
            this.astModel = astModel;

            foreach (CsdlSchema schema in this.astModel.Schemata)
            {
                this.AddSchema(schema);
            }
        }

        public override IEnumerable<IEdmSchemaElement> SchemaElements
        {
            get
            {
                foreach (CsdlSemanticsSchema schema in this.schemata)
                {
                    foreach (IEdmSchemaType type in schema.Types)
                    {
                        yield return type;
                    }

                    foreach (IEdmSchemaElement function in schema.Functions)
                    {
                        yield return function;
                    }

                    foreach (IEdmSchemaElement valueTerm in schema.ValueTerms)
                    {
                        yield return valueTerm;
                    }

                    foreach (IEdmEntityContainer entityContainer in schema.EntityContainers)
                    {
                        yield return entityContainer;
                    }
                }
            }
        }

        public override IEnumerable<IEdmVocabularyAnnotation> VocabularyAnnotations
        {
            get
            {
                List<IEdmVocabularyAnnotation> result = new List<IEdmVocabularyAnnotation>();

                foreach (CsdlSemanticsSchema schema in this.schemata)
                {
                    foreach (CsdlAnnotations sourceAnnotations in ((CsdlSchema)schema.Element).OutOfLineAnnotations)
                    {
                        CsdlSemanticsAnnotations annotations = new CsdlSemanticsAnnotations(schema, sourceAnnotations);
                        foreach (CsdlVocabularyAnnotationBase sourceAnnotation in sourceAnnotations.Annotations)
                        {
                            IEdmVocabularyAnnotation vocabAnnotation = this.WrapVocabularyAnnotation(sourceAnnotation, schema, null, annotations, sourceAnnotations.Qualifier);
                            vocabAnnotation.SetSerializationLocation(this, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
                            vocabAnnotation.SetSchemaNamespace(this, schema.Namespace);
                            result.Add(vocabAnnotation);
                        }
                    }
                }

                foreach (IEdmSchemaElement element in this.SchemaElements)
                {
                    result.AddRange(((CsdlSemanticsElement)element).InlineVocabularyAnnotations);

                    CsdlSemanticsStructuredTypeDefinition type = element as CsdlSemanticsStructuredTypeDefinition;
                    if (type != null)
                    {
                        foreach (IEdmProperty property in type.DeclaredProperties)
                        {
                            result.AddRange(((CsdlSemanticsElement)property).InlineVocabularyAnnotations);
                        }
                    }

                    CsdlSemanticsFunction function = element as CsdlSemanticsFunction;
                    if (function != null)
                    {
                        foreach (IEdmFunctionParameter parameter in function.Parameters)
                        {
                            result.AddRange(((CsdlSemanticsElement)parameter).InlineVocabularyAnnotations);
                        }
                    }

                    CsdlSemanticsEntityContainer container = element as CsdlSemanticsEntityContainer;
                    if (container != null)
                    {
                        foreach (IEdmEntityContainerElement containerElement in container.Elements)
                        {
                            result.AddRange(((CsdlSemanticsElement)containerElement).InlineVocabularyAnnotations);
                            CsdlSemanticsFunctionImport functionImport = containerElement as CsdlSemanticsFunctionImport;
                            if (functionImport != null)
                            {
                                foreach (IEdmFunctionParameter parameter in functionImport.Parameters)
                                {
                                    result.AddRange(((CsdlSemanticsElement)parameter).InlineVocabularyAnnotations);
                                }
                            }
                        }
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets an error if one exists with the current object.
        /// </summary>
        public IEnumerable<EdmError> Errors
        {
            get
            {
                List<EdmError> errors = new List<EdmError>();

                foreach (IEdmAssociation association in this.associationDictionary.Values)
                {
                    string associationFullName = association.Namespace + "." + association.Name;

                    if (association.IsBad())
                    {
                        AmbiguousAssociationBinding ambiguous = association as AmbiguousAssociationBinding;
                        if (ambiguous != null)
                        {
                            // Ambiguous bindings don't have usable locations, so get locations from the individual bindings.
                            bool skipFirst = true;
                            foreach (IEdmAssociation duplicate in ambiguous.Bindings)
                            {
                                if (skipFirst)
                                {
                                    skipFirst = false;
                                }
                                else
                                {
                                    errors.Add(new EdmError(duplicate.Location(), EdmErrorCode.AlreadyDefined, Strings.EdmModel_Validator_Semantic_SchemaElementNameAlreadyDefined(associationFullName)));
                                }
                            }
                        }
                        else
                        {
                            errors.AddRange(association.Errors());
                        }
                    }
                    else
                    {
                        if (this.FindDeclaredType(associationFullName) != null ||
                            this.FindDeclaredValueTerm(associationFullName) != null ||
                            this.FindDeclaredFunctions(associationFullName).Count() != 0)
                        {
                            errors.Add(new EdmError(association.Location(), EdmErrorCode.AlreadyDefined, Strings.EdmModel_Validator_Semantic_SchemaElementNameAlreadyDefined(associationFullName)));
                        }

                        errors.AddRange(association.End1.Errors());
                        errors.AddRange(association.End2.Errors());

                        if (association.ReferentialConstraint != null)
                        {
                            errors.AddRange(association.ReferentialConstraint.Errors());
                        }
                    }
                }

                foreach (CsdlSemanticsSchema schema in this.schemata)
                {
                    errors.AddRange(schema.Errors());
                }

                return errors;
            }
        }

        /// <summary>
        /// Searches for an association with the given name in this model and returns null if no such association exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the type being found.</param>
        /// <returns>The requested association, or null if no such type exists.</returns>
        public IEdmAssociation FindAssociation(string qualifiedName)
        {
            IEdmAssociation result;
            this.associationDictionary.TryGetValue(qualifiedName, out result);
            return result;
        }

        /// <summary>
        /// Searches for vocabulary annotations specified by this model.
        /// </summary>
        /// <param name="element">The annotated element.</param>
        /// <returns>The vocabulary annotations for the element.</returns>
        public override IEnumerable<IEdmVocabularyAnnotation> FindDeclaredVocabularyAnnotations(IEdmVocabularyAnnotatable element)
        {
            // Include the inline annotations only if this model is the one that defined them.
            CsdlSemanticsElement semanticsElement = element as CsdlSemanticsElement;
            IEnumerable<IEdmVocabularyAnnotation> inlineAnnotations = semanticsElement != null && semanticsElement.Model == this ? semanticsElement.InlineVocabularyAnnotations : Enumerable.Empty<IEdmVocabularyAnnotation>();

            List<CsdlSemanticsAnnotations> elementAnnotations;
            string fullName = EdmUtil.FullyQualifiedName(element);
            
            if (fullName != null && this.outOfLineAnnotations.TryGetValue(fullName, out elementAnnotations))
            {
                List<IEdmVocabularyAnnotation> result = new List<IEdmVocabularyAnnotation>();

                foreach (CsdlSemanticsAnnotations annotations in elementAnnotations)
                {
                    foreach (CsdlVocabularyAnnotationBase annotation in annotations.Annotations.Annotations)
                    {
                        result.Add(this.WrapVocabularyAnnotation(annotation, annotations.Context, null, annotations, annotations.Annotations.Qualifier));
                    }
                }

                return inlineAnnotations.Concat(result);
            }

            return inlineAnnotations;
        }

        public override IEnumerable<IEdmStructuredType> FindDirectlyDerivedTypes(IEdmStructuredType baseType)
        {
            List<IEdmStructuredType> candidates;
            if (this.derivedTypeMappings.TryGetValue(((IEdmSchemaType)baseType).Name, out candidates))
            {
                return candidates.Where(t => t.BaseType == baseType);
            }

            return Enumerable.Empty<IEdmStructuredType>();
        }

        internal static IEdmExpression WrapExpression(CsdlExpressionBase expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
        {
            if (expression != null)
            {
                switch (expression.ExpressionKind)
                {
                    case EdmExpressionKind.AssertType:
                        return new CsdlSemanticsAssertTypeExpression((CsdlAssertTypeExpression)expression, bindingContext, schema);
                    case EdmExpressionKind.BinaryConstant:
                        return new CsdlSemanticsBinaryConstantExpression((CsdlConstantExpression)expression, schema);
                    case EdmExpressionKind.BooleanConstant:
                        return new CsdlSemanticsBooleanConstantExpression((CsdlConstantExpression)expression, schema);
                    case EdmExpressionKind.Collection:
                        return new CsdlSemanticsCollectionExpression((CsdlCollectionExpression)expression, bindingContext, schema);
                    case EdmExpressionKind.DateTimeConstant:
                        return new CsdlSemanticsDateTimeConstantExpression((CsdlConstantExpression)expression, schema);
                    case EdmExpressionKind.DateTimeOffsetConstant:
                        return new CsdlSemanticsDateTimeOffsetConstantExpression((CsdlConstantExpression)expression, schema);
                    case EdmExpressionKind.DecimalConstant:
                        return new CsdlSemanticsDecimalConstantExpression((CsdlConstantExpression)expression, schema);
                    case EdmExpressionKind.EntitySetReference:
                        return new CsdlSemanticsEntitySetReferenceExpression((CsdlEntitySetReferenceExpression)expression, bindingContext, schema);
                    case EdmExpressionKind.EnumMemberReference:
                        return new CsdlSemanticsEnumMemberReferenceExpression((CsdlEnumMemberReferenceExpression)expression, bindingContext, schema);
                    case EdmExpressionKind.FloatingConstant:
                        return new CsdlSemanticsFloatingConstantExpression((CsdlConstantExpression)expression, schema);
                    case EdmExpressionKind.Null:
                        return new CsdlSemanticsNullExpression((CsdlConstantExpression)expression, schema);
                    case EdmExpressionKind.FunctionApplication:
                        return new CsdlSemanticsApplyExpression((CsdlApplyExpression)expression, bindingContext, schema);
                    case EdmExpressionKind.FunctionReference:
                        return new CsdlSemanticsFunctionReferenceExpression((CsdlFunctionReferenceExpression)expression, bindingContext, schema);
                    case EdmExpressionKind.GuidConstant:
                        return new CsdlSemanticsGuidConstantExpression((CsdlConstantExpression)expression, schema);
                    case EdmExpressionKind.If:
                        return new CsdlSemanticsIfExpression((CsdlIfExpression)expression, bindingContext, schema);
                    case EdmExpressionKind.IntegerConstant:
                        return new CsdlSemanticsIntConstantExpression((CsdlConstantExpression)expression, schema);
                    case EdmExpressionKind.IsType:
                        return new CsdlSemanticsIsTypeExpression((CsdlIsTypeExpression)expression, bindingContext, schema);
                    case EdmExpressionKind.LabeledExpressionReference:
                        return new CsdlSemanticsLabeledExpressionReferenceExpression((CsdlLabeledExpressionReferenceExpression)expression, bindingContext, schema);
                    case EdmExpressionKind.Labeled:
                        return schema.WrapLabeledElement((CsdlLabeledExpression)expression, bindingContext);
                    case EdmExpressionKind.ParameterReference:
                        return new CsdlSemanticsParameterReferenceExpression((CsdlParameterReferenceExpression)expression, bindingContext, schema);
                    case EdmExpressionKind.Path:
                        return new CsdlSemanticsPathExpression((CsdlPathExpression)expression, bindingContext, schema);
                    case EdmExpressionKind.PropertyReference:
                        return new CsdlSemanticsPropertyReferenceExpression((CsdlPropertyReferenceExpression)expression, bindingContext, schema);
                    case EdmExpressionKind.Record:
                        return new CsdlSemanticsRecordExpression((CsdlRecordExpression)expression, bindingContext, schema);
                    case EdmExpressionKind.StringConstant:
                        return new CsdlSemanticsStringConstantExpression((CsdlConstantExpression)expression, schema);
                    case EdmExpressionKind.TimeConstant:
                        return new CsdlSemanticsTimeConstantExpression((CsdlConstantExpression)expression, schema);
                }
            }

            return null;
        }

        internal static IEdmTypeReference WrapTypeReference(CsdlSemanticsSchema schema, CsdlTypeReference type)
        {
            var typeReference = type as CsdlNamedTypeReference;
            if (typeReference != null)
            {
                var primitiveReference = typeReference as CsdlPrimitiveTypeReference;
                if (primitiveReference != null)
                {
                    switch (primitiveReference.Kind)
                    {
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
                            return new CsdlSemanticsPrimitiveTypeReference(schema, primitiveReference);

                        case EdmPrimitiveTypeKind.Binary:
                            return new CsdlSemanticsBinaryTypeReference(schema, (CsdlBinaryTypeReference)primitiveReference);

                        case EdmPrimitiveTypeKind.DateTime:
                        case EdmPrimitiveTypeKind.DateTimeOffset:
                        case EdmPrimitiveTypeKind.Time:
                            return new CsdlSemanticsTemporalTypeReference(schema, (CsdlTemporalTypeReference)primitiveReference);

                        case EdmPrimitiveTypeKind.Decimal:
                            return new CsdlSemanticsDecimalTypeReference(schema, (CsdlDecimalTypeReference)primitiveReference);

                        case EdmPrimitiveTypeKind.String:
                            return new CsdlSemanticsStringTypeReference(schema, (CsdlStringTypeReference)primitiveReference);

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
                            return new CsdlSemanticsSpatialTypeReference(schema, (CsdlSpatialTypeReference)primitiveReference);
                    }
                }

                return new CsdlSemanticsNamedTypeReference(schema, typeReference);
            }

            var typeExpression = type as CsdlExpressionTypeReference;
            if (typeExpression != null)
            {
                var rowType = typeExpression.TypeExpression as CsdlRowType;
                if (rowType != null)
                {
                    return new CsdlSemanticsRowTypeExpression(typeExpression, new CsdlSemanticsRowTypeDefinition(schema, rowType));
                }

                var collectionType = typeExpression.TypeExpression as CsdlCollectionType;
                if (collectionType != null)
                {
                    return new CsdlSemanticsCollectionTypeExpression(typeExpression, new CsdlSemanticsCollectionTypeDefinition(schema, collectionType));
                }

                var entityReferenceType = typeExpression.TypeExpression as CsdlEntityReferenceType;
                if (entityReferenceType != null)
                {
                    return new CsdlSemanticsEntityReferenceTypeExpression(typeExpression, new CsdlSemanticsEntityReferenceTypeDefinition(schema, entityReferenceType));
                }
            }

            return null;
        }

        internal static IEdmAssociation CreateAmbiguousAssociationBinding(IEdmAssociation first, IEdmAssociation second)
        {
            var ambiguous = first as AmbiguousAssociationBinding;
            if (ambiguous != null)
            {
                ambiguous.AddBinding(second);
                return ambiguous;
            }

            return new AmbiguousAssociationBinding(first, second);
        }

        internal IEnumerable<IEdmVocabularyAnnotation> WrapInlineVocabularyAnnotations(CsdlSemanticsElement element, CsdlSemanticsSchema schema)
        {
            IEdmVocabularyAnnotatable vocabularyAnnotatableElement = element as IEdmVocabularyAnnotatable;
            if (vocabularyAnnotatableElement != null)
            {
                IEnumerable<CsdlVocabularyAnnotationBase> vocabularyAnnotations = element.Element.VocabularyAnnotations;
                if (vocabularyAnnotations.FirstOrDefault() != null)
                {
                    List<IEdmVocabularyAnnotation> wrappedAnnotations = new List<IEdmVocabularyAnnotation>();
                    foreach (CsdlVocabularyAnnotationBase vocabularyAnnotation in vocabularyAnnotations)
                    {
                        IEdmVocabularyAnnotation vocabAnnotation = this.WrapVocabularyAnnotation(vocabularyAnnotation, schema, vocabularyAnnotatableElement, null, vocabularyAnnotation.Qualifier);
                        vocabAnnotation.SetSerializationLocation(this, EdmVocabularyAnnotationSerializationLocation.Inline);
                        wrappedAnnotations.Add(vocabAnnotation);
                    }

                    return wrappedAnnotations;
                }
            }

            return Enumerable.Empty<IEdmVocabularyAnnotation>();
        }

        private IEdmVocabularyAnnotation WrapVocabularyAnnotation(CsdlVocabularyAnnotationBase annotation, CsdlSemanticsSchema schema, IEdmVocabularyAnnotatable targetContext, CsdlSemanticsAnnotations annotationsContext, string qualifier)
        {
            CsdlSemanticsVocabularyAnnotation result;

            // Guarantee that multiple calls to wrap a given annotation all return the same object.
            if (this.wrappedAnnotations.TryGetValue(annotation, out result))
            {
                return result;
            }

            CsdlValueAnnotation valueAnnotation = annotation as CsdlValueAnnotation;
            result =
                valueAnnotation != null
                ? (CsdlSemanticsVocabularyAnnotation)new CsdlSemanticsValueAnnotation(schema, targetContext, annotationsContext, valueAnnotation, qualifier)
                : (CsdlSemanticsVocabularyAnnotation)new CsdlSemanticsTypeAnnotation(schema, targetContext, annotationsContext, (CsdlTypeAnnotation)annotation, qualifier);

            this.wrappedAnnotations[annotation] = result;
            return result;
        }

        private void AddSchema(CsdlSchema schema)
        {
            CsdlSemanticsSchema schemaWrapper = new CsdlSemanticsSchema(this, schema);
            this.schemata.Add(schemaWrapper);

            foreach (IEdmSchemaType type in schemaWrapper.Types)
            {
                CsdlSemanticsStructuredTypeDefinition structuredType = type as CsdlSemanticsStructuredTypeDefinition;
                if (structuredType != null)
                {
                    string baseTypeNamespace;
                    string baseTypeName;
                    string baseTypeFullName = ((CsdlNamedStructuredType)structuredType.Element).BaseTypeName;
                    if (baseTypeFullName != null)
                    {
                        EdmUtil.TryGetNamespaceNameFromQualifiedName(baseTypeFullName, out baseTypeNamespace, out baseTypeName);
                        if (baseTypeName != null)
                        {
                            List<IEdmStructuredType> derivedTypes;
                            if (!this.derivedTypeMappings.TryGetValue(baseTypeName, out derivedTypes))
                            {
                                derivedTypes = new List<IEdmStructuredType>();
                                this.derivedTypeMappings[baseTypeName] = derivedTypes;
                            }

                            derivedTypes.Add(structuredType);
                        }
                    }
                }

                RegisterElement(type);
            }

            foreach (CsdlSemanticsAssociation association in schemaWrapper.Associations)
            {
                RegistrationHelper.AddElement(association, association.Namespace + "." + association.Name, this.associationDictionary, CreateAmbiguousAssociationBinding);
            }

            foreach (IEdmFunction function in schemaWrapper.Functions)
            {
                RegisterElement(function);
            }

            foreach (IEdmValueTerm valueTerm in schemaWrapper.ValueTerms)
            {
                RegisterElement(valueTerm);
            }

            foreach (IEdmEntityContainer container in schemaWrapper.EntityContainers)
            {
                RegisterElement(container);
            }

            foreach (CsdlAnnotations schemaOutOfLineAnnotations in schema.OutOfLineAnnotations)
            {
                string target = schemaOutOfLineAnnotations.Target;
                string replaced = schemaWrapper.ReplaceAlias(target);
                if (replaced != null)
                {
                    target = replaced;
                }

                List<CsdlSemanticsAnnotations> annotations;
                if (!this.outOfLineAnnotations.TryGetValue(target, out annotations))
                {
                    annotations = new List<CsdlSemanticsAnnotations>();
                    this.outOfLineAnnotations[target] = annotations;
                }

                annotations.Add(new CsdlSemanticsAnnotations(schemaWrapper, schemaOutOfLineAnnotations));
            }

            foreach (CsdlUsing used in schema.Usings)
            {
                this.SetNamespaceAlias(used.Namespace, used.Alias);
            }

            var edmVersion = this.GetEdmVersion();
            if (edmVersion == null || edmVersion < schema.Version)
            {
                this.SetEdmVersion(schema.Version);
            }
        }
    }
}
 

//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for one CsdlModel and its referenced CsdlModels.
    /// </summary>
    [DebuggerDisplay("CsdlSemanticsModel({string.Join(\",\", DeclaredNamespaces)})")]
    internal class CsdlSemanticsModel : EdmModelBase, IEdmCheckable
    {
        private readonly CsdlSemanticsModel mainEdmModel;    // parent IEdmModel
        private readonly CsdlModel astModel;        // current internal CsdlModel
        private readonly List<CsdlSemanticsSchema> schemata = new List<CsdlSemanticsSchema>();
        private readonly Dictionary<string, List<CsdlSemanticsAnnotations>> outOfLineAnnotations = new Dictionary<string, List<CsdlSemanticsAnnotations>>();
        private readonly ConcurrentDictionary<CsdlAnnotation, CsdlSemanticsVocabularyAnnotation> wrappedAnnotations = new ConcurrentDictionary<CsdlAnnotation, CsdlSemanticsVocabularyAnnotation>();
        private readonly Dictionary<string, List<IEdmStructuredType>> derivedTypeMappings = new Dictionary<string, List<IEdmStructuredType>>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="astModel">The raw CsdlModel.</param>
        /// <param name="annotationsManager">The IEdmDirectValueAnnotationsManager.</param>
        /// <param name="referencedModels">The IEdmModels to be referenced. if any element or namespace is not supposed to be include, you should have removed it before passing to this constructor.</param>
        /// <param name="includeDefaultVocabularies">A value indicating enable/disable the built-in vocabulary supporting.</param>
        public CsdlSemanticsModel(CsdlModel astModel, IEdmDirectValueAnnotationsManager annotationsManager, IEnumerable<IEdmModel> referencedModels, bool includeDefaultVocabularies = true)
            : base(referencedModels, annotationsManager, includeDefaultVocabularies)
        {
            this.astModel = astModel;
            this.SetEdmReferences(astModel.CurrentModelReferences);
            foreach (CsdlSchema schema in this.astModel.Schemata)
            {
                this.AddSchema(schema);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mainCsdlModel">The main raw CsdlModel.</param>
        /// <param name="annotationsManager">The IEdmDirectValueAnnotationsManager.</param>
        /// <param name="referencedCsdlModels">The referenced raw CsdlModels.</param>
        /// <param name="includeDefaultVocabularies">A value indicating enable/disable the built-in vocabulary supporting.</param>
        public CsdlSemanticsModel(CsdlModel mainCsdlModel, IEdmDirectValueAnnotationsManager annotationsManager, IEnumerable<CsdlModel> referencedCsdlModels, bool includeDefaultVocabularies)
            : base(Enumerable.Empty<IEdmModel>(), annotationsManager, includeDefaultVocabularies)
        {
            this.astModel = mainCsdlModel;
            this.SetEdmReferences(astModel.CurrentModelReferences);

            // 1. build semantics for referenced models
            foreach (var tmp in referencedCsdlModels)
            {
                var refModel = new CsdlSemanticsModel(tmp, this.DirectValueAnnotationsManager, this, includeDefaultVocabularies);
                this.AddReferencedModel(refModel);
            }

            // 2. build semantics for current model
            foreach (var include in mainCsdlModel.CurrentModelReferences.SelectMany(s => s.Includes))
            {
                this.SetNamespaceAlias(include.Namespace, include.Alias);
            }

            foreach (CsdlSchema schema in this.astModel.Schemata)
            {
                this.AddSchema(schema);
            }
        }

        /// <summary>
        /// Constructor for creating a referenced model, is private and only called by the above constructor.
        /// </summary>
        /// <param name="referencedCsdlModel">The referenced raw CsdlModel.</param>
        /// <param name="annotationsManager">The IEdmDirectValueAnnotationsManager.</param>
        /// <param name="mainCsdlSemanticsModel">The CsdlSemanticsModel that will reference this new CsdlSemanticsModel. </param>
        /// <param name="includeDefaultVocabularies">A value indicating enable/disable the built-in vocabulary supporting.</param>
        private CsdlSemanticsModel(CsdlModel referencedCsdlModel, IEdmDirectValueAnnotationsManager annotationsManager, CsdlSemanticsModel mainCsdlSemanticsModel, bool includeDefaultVocabularies)
            : base(Enumerable.Empty<IEdmModel>(), annotationsManager, includeDefaultVocabularies)
        {
            this.mainEdmModel = mainCsdlSemanticsModel;
            Debug.Assert(referencedCsdlModel.ParentModelReferences.Any(), "referencedCsdlModel.ParentModelReferences.Any()");
            this.astModel = referencedCsdlModel;
            this.SetEdmReferences(referencedCsdlModel.CurrentModelReferences);

            foreach (var tmp in referencedCsdlModel.ParentModelReferences.SelectMany(s => s.Includes))
            {
                string includeNs = tmp.Namespace;
                if (!referencedCsdlModel.Schemata.Any(s => s.Namespace == includeNs))
                {
                    // edmx:include must be an existing namespace
                    // TODO: REF throw exception: should include a namespace that exists in referenced model.
                }
            }

            foreach (var tmp in referencedCsdlModel.CurrentModelReferences.SelectMany(s => s.Includes))
            {
                // in any referenced model, alias may point to a further referenced model, now make alias available:
                this.SetNamespaceAlias(tmp.Namespace, tmp.Alias);
            }

            foreach (var schema in referencedCsdlModel.Schemata)
            {
                this.AddSchemaIfReferenced(schema, referencedCsdlModel.ParentModelReferences);
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

                    foreach (IEdmSchemaElement function in schema.Operations)
                    {
                        yield return function;
                    }

                    foreach (IEdmSchemaElement valueTerm in schema.Terms)
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

        public override IEnumerable<string> DeclaredNamespaces
        {
            get { return this.schemata.Select(s => s.Namespace); }
        }

        public IDictionary<string, List<CsdlSemanticsAnnotations>> OutOfLineAnnotations
        {
            get
            {
                return outOfLineAnnotations;
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
                        foreach (CsdlAnnotation sourceAnnotation in sourceAnnotations.Annotations)
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

                    CsdlSemanticsOperation operation = element as CsdlSemanticsOperation;
                    if (operation != null)
                    {
                        foreach (IEdmOperationParameter parameter in operation.Parameters)
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
                        }
                    }

                    CsdlSemanticsEnumTypeDefinition enumType = element as CsdlSemanticsEnumTypeDefinition;
                    if (enumType != null)
                    {
                        foreach (IEdmEnumMember member in enumType.Members)
                        {
                            result.AddRange(((CsdlSemanticsElement)member).InlineVocabularyAnnotations);
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

                HashSetInternal<string> usedAlias = new HashSetInternal<string>();
                var usedNamespaces = this.GetUsedNamespacesHavingAlias();
                var mappings = this.GetNamespaceAliases();

                if (usedNamespaces != null && mappings != null)
                {
                    foreach (var ns in usedNamespaces)
                    {
                        string alias;
                        if (mappings.TryGetValue(ns, out alias) && !usedAlias.Add(alias))
                        {
                            errors.Add(new EdmError(this.Location(), EdmErrorCode.DuplicateAlias, Strings.CsdlSemantics_DuplicateAlias(ns, alias)));
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
        /// Gets the main model that is referencing this model. The value may be null.
        /// </summary>
        internal CsdlSemanticsModel MainModel
        {
            get { return this.mainEdmModel; }
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
                    foreach (CsdlAnnotation annotation in annotations.Annotations.Annotations)
                    {
                        IEdmVocabularyAnnotation vocabAnnotation = this.WrapVocabularyAnnotation(annotation, annotations.Context, null, annotations, annotations.Annotations.Qualifier);
                        vocabAnnotation.SetSerializationLocation(this, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
                        result.Add(vocabAnnotation);
                    }
                }

                return inlineAnnotations.Concat(result);
            }

            return inlineAnnotations;

            // TODO: REF
            // find annotation in referenced models
        }

        public override IEnumerable<IEdmStructuredType> FindDirectlyDerivedTypes(IEdmStructuredType baseType)
        {
            List<IEdmStructuredType> ret = new List<IEdmStructuredType>();
            List<IEdmStructuredType> candidates;
            if (this.derivedTypeMappings.TryGetValue(((IEdmSchemaType)baseType).Name, out candidates))
            {
                ret.AddRange(candidates.Where(t => t.BaseType == baseType));
            }

            // find derived type in referenced models
            foreach (var tmp in this.ReferencedModels)
            {
                ret.AddRange(tmp.FindDirectlyDerivedTypes(baseType));
            }

            return ret;
        }

        internal void AddToReferencedModels(IEnumerable<IEdmModel> models)
        {
            foreach (var edmModel in models)
            {
                this.AddReferencedModel(edmModel);
            }
        }

        internal static IEdmExpression WrapExpression(CsdlExpressionBase expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
        {
            if (expression != null)
            {
                switch (expression.ExpressionKind)
                {
                    case EdmExpressionKind.Cast:
                        return new CsdlSemanticsCastExpression((CsdlCastExpression)expression, bindingContext, schema);
                    case EdmExpressionKind.BinaryConstant:
                        return new CsdlSemanticsBinaryConstantExpression((CsdlConstantExpression)expression, schema);
                    case EdmExpressionKind.BooleanConstant:
                        return new CsdlSemanticsBooleanConstantExpression((CsdlConstantExpression)expression, schema);
                    case EdmExpressionKind.Collection:
                        return new CsdlSemanticsCollectionExpression((CsdlCollectionExpression)expression, bindingContext, schema);
                    case EdmExpressionKind.DateTimeOffsetConstant:
                        return new CsdlSemanticsDateTimeOffsetConstantExpression((CsdlConstantExpression)expression, schema);
                    case EdmExpressionKind.DecimalConstant:
                        return new CsdlSemanticsDecimalConstantExpression((CsdlConstantExpression)expression, schema);
                    case EdmExpressionKind.EnumMember:
                        return new CsdlSemanticsEnumMemberExpression((CsdlEnumMemberExpression)expression, bindingContext, schema);
                    case EdmExpressionKind.FloatingConstant:
                        return new CsdlSemanticsFloatingConstantExpression((CsdlConstantExpression)expression, schema);
                    case EdmExpressionKind.Null:
                        return new CsdlSemanticsNullExpression((CsdlConstantExpression)expression, schema);
                    case EdmExpressionKind.FunctionApplication:
                        return new CsdlSemanticsApplyExpression((CsdlApplyExpression)expression, bindingContext, schema);
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
                    case EdmExpressionKind.Path:
                        return new CsdlSemanticsPathExpression((CsdlPathExpression)expression, bindingContext, schema);
                    case EdmExpressionKind.PropertyPath:
                        return new CsdlSemanticsPropertyPathExpression((CsdlPropertyPathExpression)expression, bindingContext, schema);
                    case EdmExpressionKind.NavigationPropertyPath:
                        return new CsdlSemanticsNavigationPropertyPathExpression((CsdlNavigationPropertyPathExpression)expression, bindingContext, schema);
                    case EdmExpressionKind.Record:
                        return new CsdlSemanticsRecordExpression((CsdlRecordExpression)expression, bindingContext, schema);
                    case EdmExpressionKind.StringConstant:
                        return new CsdlSemanticsStringConstantExpression((CsdlConstantExpression)expression, schema);
                    case EdmExpressionKind.DurationConstant:
                        return new CsdlSemanticsDurationConstantExpression((CsdlConstantExpression)expression, schema);
                    case EdmExpressionKind.DateConstant:
                        return new CsdlSemanticsDateConstantExpression((CsdlConstantExpression)expression, schema);
                    case EdmExpressionKind.TimeOfDayConstant:
                        return new CsdlSemanticsTimeOfDayConstantExpression((CsdlConstantExpression)expression, schema);
                    case EdmExpressionKind.AnnotationPath:
                        return new CsdlSemanticsAnnotationPathExpression((CsdlAnnotationPathExpression)expression, bindingContext, schema);
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
                        case EdmPrimitiveTypeKind.Date:
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

                        case EdmPrimitiveTypeKind.DateTimeOffset:
                        case EdmPrimitiveTypeKind.Duration:
                        case EdmPrimitiveTypeKind.TimeOfDay:
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
                else
                {
                    CsdlUntypedTypeReference csdlUntypedTypeReference = typeReference as CsdlUntypedTypeReference;
                    if (csdlUntypedTypeReference != null)
                    {
                        return new CsdlSemanticsUntypedTypeReference(schema, csdlUntypedTypeReference);
                    }

                    if (schema.FindType(typeReference.FullName) is IEdmTypeDefinition)
                    {
                        return new CsdlSemanticsTypeDefinitionReference(schema, typeReference);
                    }
                }

                return new CsdlSemanticsNamedTypeReference(schema, typeReference);
            }

            var typeExpression = type as CsdlExpressionTypeReference;
            if (typeExpression != null)
            {
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

        internal IEnumerable<IEdmVocabularyAnnotation> WrapInlineVocabularyAnnotations(CsdlSemanticsElement element, CsdlSemanticsSchema schema)
        {
            IEdmVocabularyAnnotatable vocabularyAnnotatableElement = element as IEdmVocabularyAnnotatable;
            if (vocabularyAnnotatableElement != null)
            {
                IEnumerable<CsdlAnnotation> vocabularyAnnotations = element.Element.VocabularyAnnotations;
                if (vocabularyAnnotations.FirstOrDefault() != null)
                {
                    List<IEdmVocabularyAnnotation> wrappedAnnotations = new List<IEdmVocabularyAnnotation>();
                    foreach (CsdlAnnotation vocabularyAnnotation in vocabularyAnnotations)
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

        private IEdmVocabularyAnnotation WrapVocabularyAnnotation(CsdlAnnotation annotation, CsdlSemanticsSchema schema, IEdmVocabularyAnnotatable targetContext, CsdlSemanticsAnnotations annotationsContext, string qualifier)
        {
            return EdmUtil.DictionaryGetOrUpdate(
                this.wrappedAnnotations,
                annotation,
                ann => new CsdlSemanticsVocabularyAnnotation(schema, targetContext, annotationsContext, ann, qualifier));
        }

        private void AddSchema(CsdlSchema schema)
        {
            this.AddSchema(schema, true);
        }

        private void AddSchema(CsdlSchema schema, bool addAnnotations)
        {
            CsdlSemanticsSchema schemaWrapper = new CsdlSemanticsSchema(this, schema);
            this.schemata.Add(schemaWrapper);

            AddSchemaElements(schemaWrapper);

            if (!string.IsNullOrEmpty(schema.Alias))
            {
                this.SetNamespaceAlias(schema.Namespace, schema.Alias);
            }

            if (addAnnotations)
            {
                // this adds all out-of-line annotations in the schema regardless of edmx:IncludeAnnotations references in the model
                AddOutOfLineAnnotationsFromSchema(schema, schemaWrapper, /* includeAnnotationsIndex */ null);
            }

            var edmVersion = this.GetEdmVersion();
            if (edmVersion == null || edmVersion < schema.Version)
            {
                this.SetEdmVersion(schema.Version);
            }
        }

        /// <summary>
        /// Adds the specified <paramref name="schema"/> to this model if 
        /// the schema's namespace or annotations are referenced by the parent model
        /// </summary>
        /// <param name="schema">The schema to add</param>
        /// <param name="parentReferences">The <see cref="IEdmReference"/>s of the parent model.</param>
        private void AddSchemaIfReferenced(CsdlSchema schema, IEnumerable<IEdmReference> parentReferences)
        {
            CsdlSemanticsSchema schemaWrapper = new CsdlSemanticsSchema(this, schema);

            bool shouldAddSchemaElements;
            Dictionary<string, List<IEdmIncludeAnnotations>> includeAnnotationsIndex;
            ProcessSchemaParentReferences(schema, schemaWrapper, parentReferences, out shouldAddSchemaElements, out includeAnnotationsIndex);

            if (!string.IsNullOrEmpty(schema.Alias))
            {
                this.SetNamespaceAlias(schema.Namespace, schema.Alias);
            }

            if (shouldAddSchemaElements)
            {
                AddSchemaElements(schemaWrapper);
            }

            if (includeAnnotationsIndex.Count > 0)
            {
                AddOutOfLineAnnotationsFromSchema(schema, schemaWrapper, includeAnnotationsIndex);
            }

            if (shouldAddSchemaElements || includeAnnotationsIndex.Count > 0)
            {
                this.schemata.Add(schemaWrapper);

                var edmVersion = this.GetEdmVersion();
                if (edmVersion == null || edmVersion < schema.Version)
                {
                    this.SetEdmVersion(schema.Version);
                }
            }
        }

        /// <summary>
        /// Process the <see cref="IEdmReference"/>s of the parent model to
        /// figure out whether the schema's types (entity types, enums, containers, etc.) and annotations should
        /// be imported
        /// </summary>
        /// <param name="schema">The schema to process</param>
        /// <param name="schemaWrapper">The <see cref="CsdlSemanticsSchema"/> wrapper of <paramref name="schema"/></param>
        /// <param name="parentReferences">The references in the model that contains the <paramref name="schema"/></param>
        /// <param name="shouldAddSchemaElements">Whether schema types such entity types, operations, enums, etc. should be added</param>
        /// <param name="includeAnnotationsIndex">Cache of the schema's out-of-line <see cref="IEdmIncludeAnnotations"/>s indexed by the term namespace</param>
        private static void ProcessSchemaParentReferences(
            CsdlSchema schema,
            CsdlSemanticsSchema schemaWrapper,
            IEnumerable<IEdmReference> parentReferences,
            out bool shouldAddSchemaElements,
            out Dictionary<string, List<IEdmIncludeAnnotations>> includeAnnotationsIndex)
        {
            string schemaNamespace = schema.Namespace;

            shouldAddSchemaElements = false;
            includeAnnotationsIndex = new Dictionary<string, List<IEdmIncludeAnnotations>>();
            

            foreach (IEdmReference reference in parentReferences)
            {
                if (!shouldAddSchemaElements)
                {
                    // we should add types from this schema if there's at least
                    // one edmx:Include that references this schema's namespace
                    foreach (IEdmInclude include in reference.Includes)
                    {
                        if (include.Namespace == schemaNamespace)
                        {
                            shouldAddSchemaElements = true;
                            break;
                        }
                    }
                }

                foreach(IEdmIncludeAnnotations includeAnnotations in reference.IncludeAnnotations)
                {
                    // index the edmx:IncludeAnnotations by namespace to make it
                    // easier to filter which annotations to import with the schema
                    string termNamespace = includeAnnotations.TermNamespace;
                    List<IEdmIncludeAnnotations> cachedAnnotations;
                    if (!includeAnnotationsIndex.TryGetValue(termNamespace, out cachedAnnotations))
                    {
                        cachedAnnotations = new List<IEdmIncludeAnnotations>();
                        includeAnnotationsIndex[termNamespace] = cachedAnnotations;
                    }

                    cachedAnnotations.Add(includeAnnotations);
                }
            }
        }

        /// <summary>
        /// Registers schema elements (entity types, operations, entity containers, etc.) from the
        /// specified schema into the model.
        /// </summary>
        /// <param name="schemaWrapper">The <see cref="CsdlSemanticsSchema"/> to add elements from</param>
        private void AddSchemaElements(CsdlSemanticsSchema schemaWrapper)
        {
            foreach (IEdmSchemaType type in schemaWrapper.Types)
            {
                CsdlSemanticsStructuredTypeDefinition structuredType = type as CsdlSemanticsStructuredTypeDefinition;
                if (structuredType != null)
                {
                    string baseTypeName;
                    string baseTypeFullName = ((CsdlNamedStructuredType)structuredType.Element).BaseTypeName;
                    if (baseTypeFullName != null)
                    {
                        EdmUtil.TryGetNamespaceNameFromQualifiedName(baseTypeFullName, out _, out baseTypeName);
                        if (baseTypeName != null)
                        {
                            List<IEdmStructuredType> derivedTypes;
                            if (!this.derivedTypeMappings.TryGetValue(baseTypeName, out derivedTypes))
                            {
                                derivedTypes = new List<IEdmStructuredType>();
                                this.derivedTypeMappings[baseTypeName] = derivedTypes;
                            }

                            // TODO: REF referenced derived types
                            derivedTypes.Add(structuredType);
                        }
                    }
                }

                RegisterElement(type);
            }

            foreach (IEdmOperation function in schemaWrapper.Operations)
            {
                RegisterElement(function);
            }

            foreach (IEdmTerm valueTerm in schemaWrapper.Terms)
            {
                RegisterElement(valueTerm);
            }

            foreach (IEdmEntityContainer container in schemaWrapper.EntityContainers)
            {
                RegisterElement(container);
            }
        }

        /// <summary>
        /// Adds out-of-line annotations from the specified schema
        /// if they are referenced in the <paramref name="includeAnnotationsIndex"/> cache.
        /// </summary>
        /// <param name="schema">The schema to add annotations from.</param>
        /// <param name="schemaWrapper">The <see cref="CsdlSemanticsSchema"/> wrapper for the provided schema</param>
        /// <param name="includeAnnotationsIndex">Cache of the schema's out-of-line <see cref="IEdmIncludeAnnotations"/>s indexed by the term namespace.
        /// If this dictionary is non-empty, then only annotations that match
        /// the references will be added. If it's empty, no annotations will be added. If it's null, all annotations
        /// will be added unconditionally.</param>
        private void AddOutOfLineAnnotationsFromSchema(
            CsdlSchema schema,
            CsdlSemanticsSchema schemaWrapper,
            Dictionary<string, List<IEdmIncludeAnnotations>> includeAnnotationsIndex)
        {
            if (includeAnnotationsIndex != null && includeAnnotationsIndex.Count == 0)
            {
                return;
            }

            foreach (CsdlAnnotations schemaOutOfLineAnnotations in schema.OutOfLineAnnotations)
            {
                string target = this.ReplaceAlias(schemaOutOfLineAnnotations.Target);
                
                List<CsdlSemanticsAnnotations> annotations;
                if (!this.outOfLineAnnotations.TryGetValue(target, out annotations))
                {
                    annotations = new List<CsdlSemanticsAnnotations>();
                    this.outOfLineAnnotations[target] = annotations;
                }

                CsdlAnnotations filteredAnnotations = FilterIncludedAnnotations(schemaOutOfLineAnnotations, target, includeAnnotationsIndex);
                annotations.Add(new CsdlSemanticsAnnotations(schemaWrapper, filteredAnnotations));
            }
        }

        /// <summary>
        /// Filters the annotations of the specified <paramref name="csdlAnnotations"/>
        /// and returns a copy containing only the annotations that match the
        /// specified <see cref="IEdmIncludeAnnotations"/>
        /// </summary>
        /// <param name="csdlAnnotations"><see cref="CsdlAnnotations"/> containing the out-of-line annotations to filter</param>
        /// <param name="target">The target of the out-of-line annotations with the alias namespace already resolved.</param>
        /// <param name="includeAnnotationsIndex">Cache of the schema's out-of-line <see cref="IEdmIncludeAnnotations"/>s indexed by the term namespace.
        /// If the dictionary is non-null, only annotations that match the references will be included.
        /// If it's null, all annotations will be added unconditionally.</param>
        /// <returns>A <see cref="CsdlAnnotations"/> object with the same target and qualifier as the original, but including
        /// only the annotations that match the references in the <paramref name="includeAnnotationsIndex"/>.</returns>
        private CsdlAnnotations FilterIncludedAnnotations(
            CsdlAnnotations csdlAnnotations,
            string target,
            Dictionary<string, List<IEdmIncludeAnnotations>> includeAnnotationsIndex)
        {
            if (includeAnnotationsIndex == null)
            {
                return csdlAnnotations;
            }

            var filteredAnnotations = csdlAnnotations.Annotations.Where(annotation =>
                ShouldIncludeAnnotation(annotation, target, includeAnnotationsIndex));
            return new CsdlAnnotations(filteredAnnotations, csdlAnnotations.Target, csdlAnnotations.Qualifier);
        }

        /// <summary>
        /// Checks whether the specified annoation should be included in the model based
        /// on whether it matches any of the provided <see cref="IEdmIncludeAnnotations"/> references.
        /// </summary>
        /// <param name="annotation">The annotation to test</param>
        /// <param name="target">The target of the annotation, with the alias already resolved if any</param>
        /// <param name="includeAnnotationsIndex">Cache of the schema's out-of-line <see cref="IEdmIncludeAnnotations"/>s indexed by the term namespace.
        /// The annotation should be included if it matches at least one of references in this cache.</param>
        /// <returns>Whether or not the annotation should be included in the model.</returns>
        private bool ShouldIncludeAnnotation(
            CsdlAnnotation annotation,
            string target,
            Dictionary<string, List<IEdmIncludeAnnotations>> includeAnnotationsIndex)
        {
            // include annotation if it matches one of the edmx:IncludeAnnotation references
            // we check if the namespace matches, and (if applicable) also the target namespace and the qualifier
            // see: http://docs.oasis-open.org/odata/odata-csdl-xml/v4.01/odata-csdl-xml-v4.01.html#sec_IncludedAnnotations

            string qualifiedTerm = this.ReplaceAlias(annotation.Term);
            EdmUtil.TryGetNamespaceNameFromQualifiedName(target, out string targetNamespace, out _);
            if (EdmUtil.TryGetNamespaceNameFromQualifiedName(qualifiedTerm, out string termNamespace, out _))
            {
                if (includeAnnotationsIndex.TryGetValue(termNamespace, out List<IEdmIncludeAnnotations> includeAnnotations))
                {

                    return includeAnnotations.Any(include =>
                        (string.IsNullOrEmpty(include.Qualifier) || annotation.Qualifier == include.Qualifier)
                        && (string.IsNullOrEmpty(include.TargetNamespace) || targetNamespace == include.TargetNamespace));
                }
                   
            }

            return false;
        }
    }
}

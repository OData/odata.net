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
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Csdl.Internal.Parsing;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Library;
using System.Linq;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlMetadataModel.
    /// </summary>
    internal class CsdlSemanticsModel : EdmModelBase
    {
        private readonly CsdlModel astModel;
        private readonly IDictionary<string, ExternalAnnotations> externalAnnotationsDictionary;
        private readonly List<CsdlSemanticsSchema> schemata = new List<CsdlSemanticsSchema>();
        private readonly Dictionary<string, List<CsdlSemanticsAnnotations>> outOfLineAnnotations = new Dictionary<string, List<CsdlSemanticsAnnotations>>();
        private readonly Dictionary<IEdmEntityContainerElement, CsdlSemanticsEntityContainer> declaringContainers = new Dictionary<IEdmEntityContainerElement, CsdlSemanticsEntityContainer>();
        private readonly AnnotationsManager annotationsManager;

        public CsdlSemanticsModel(CsdlModel astModel, IDictionary<string, ExternalAnnotations> externalAnnotationsDictionary, AnnotationsManager annotationsManager)
        {
            this.astModel = astModel;
            this.externalAnnotationsDictionary = externalAnnotationsDictionary;
            this.annotationsManager = annotationsManager;

            foreach (CsdlSchema schema in this.astModel.Schemata)
            {
                this.AddSchema(schema);
            }
        }

        internal AnnotationsManager AnnotationsManager
        {
            get { return this.annotationsManager; }
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

                    foreach (IEdmSchemaElement association in schema.Associations)
                    {
                        yield return association;
                    }

                    foreach (IEdmSchemaElement valueTerm in schema.ValueTerms)
                    {
                        yield return valueTerm;
                    }
                }
            }
        }

        private void AddSchema(CsdlSchema schema)
        {
            CsdlSemanticsSchema schemaWrapper = new CsdlSemanticsSchema(this, schema);
            this.schemata.Add(schemaWrapper);

            foreach (IEdmSchemaType type in schemaWrapper.Types)
            {
                RegisterElement(type);
            }

            foreach (IEdmAssociation association in schemaWrapper.Associations)
            {
                RegisterElement(association);
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
                AddEntityContainer(container);
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

            var edmVersion = this.GetEdmVersion();
            if (edmVersion == null || edmVersion < schema.Version)
            {
                this.SetEdmVersion(schema.Version);
            }
        }

        private static IEdmVocabularyAnnotation WrapVocabularyAnnotation(CsdlVocabularyAnnotationBase annotation, CsdlSemanticsSchema schema, IEdmElement targetContext, CsdlSemanticsAnnotations annotationsContext, string qualifier)
        {
            CsdlValueAnnotation valueAnnotation = annotation as CsdlValueAnnotation;
            if (valueAnnotation != null)
            {
                return new CsdlSemanticsValueAnnotation(schema, targetContext, annotationsContext, valueAnnotation, qualifier);
            }

            return new CsdlSemanticsTypeAnnotation(schema, targetContext, annotationsContext, (CsdlTypeAnnotation)annotation, qualifier);
        }

        private static IEnumerable<IEdmAnnotation> WrapInlineAnnotations(CsdlSemanticsElement element, CsdlSemanticsSchema schema)
        {
            IEnumerable<CsdlImmediateValueAnnotation> annotations = element.Element.ImmediateValueAnnotations;
            IEnumerable<CsdlVocabularyAnnotationBase> vocabularyAnnotations = element.Element.VocabularyAnnotations;
            CsdlElementWithDocumentation elementWithDocumentation = element.Element as CsdlElementWithDocumentation;
            CsdlDocumentation documentation = (elementWithDocumentation != null) ? elementWithDocumentation.Documentation : null;

            if (documentation != null || annotations.FirstOrDefault() != null || vocabularyAnnotations.FirstOrDefault() != null)
            {
                List<IEdmAnnotation> wrappedAnnotations = new List<IEdmAnnotation>();
                
                foreach (CsdlImmediateValueAnnotation annotation in annotations)
                {
                    wrappedAnnotations.Add(new CsdlSemanticsImmediateValueAnnotation(annotation));
                }

                foreach (CsdlVocabularyAnnotationBase vocabularyAnnotation in vocabularyAnnotations)
                {
                    wrappedAnnotations.Add(WrapVocabularyAnnotation(vocabularyAnnotation, schema, element, null, vocabularyAnnotation.Qualifier));
                }

                if (documentation != null)
                {
                    wrappedAnnotations.Add(new CsdlSemanticsDocumentation(documentation));
                }

                return wrappedAnnotations;
            }

            return Enumerable.Empty<IEdmAnnotation>();
        }

        internal IEnumerable<IEdmAnnotation> WrapPropertyAnnotations(CsdlSemanticsProperty property, CsdlSemanticsSchema schema)
        {
            ExternalAnnotations typeAnnotations;
            List<IEdmAnnotation> propertyAnnotations;
            if (this.TryGetStandaloneAnnotations(property.DeclaringType as IEdmSchemaElement, out typeAnnotations) &&
                typeAnnotations.PropertyAnnotations != null && typeAnnotations.PropertyAnnotations.TryGetValue(property.Name, out propertyAnnotations))
            {
                var wrappedAnnotations = new List<IEdmAnnotation>(this.WrapAnnotations(property, schema));
                wrappedAnnotations.AddRange(propertyAnnotations);
                return wrappedAnnotations;
            }

            return this.WrapAnnotations(property, schema);
        }

        internal IEnumerable<IEdmAnnotation> WrapAnnotations(CsdlSemanticsElement element, CsdlSemanticsSchema schema)
        {
            ExternalAnnotations typeAnnotations;
            IEdmNamedElement namedElement = element as IEdmNamedElement;
            if (namedElement != null && this.TryGetStandaloneAnnotations(namedElement, out typeAnnotations))
            {
                var wrappedAnnotations = new List<IEdmAnnotation>(WrapInlineAnnotations(element, schema));
                wrappedAnnotations.AddRange(typeAnnotations.Annotations);
                return wrappedAnnotations;
            }
         
            return WrapInlineAnnotations(element, schema);
        }

        internal static IEdmExpression WrapExpression(CsdlExpressionBase expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
        {
            CsdlConstantExpression constant = expression as CsdlConstantExpression;
            if (constant != null)
            {
                switch (constant.Kind)
                {
                    case CsdlConstantExpressionKind.Int:
                        return new CsdlSemanticsIntConstantExpression(constant);
                    case CsdlConstantExpressionKind.String:
                        return new CsdlSemanticsStringConstantExpression(constant);
                    case CsdlConstantExpressionKind.Float:
                        return new CsdlSemanticsFloatingConstantExpression(constant);
                    case CsdlConstantExpressionKind.Decimal:
                        return new CsdlSemanticsDecimalConstantExpression(constant);
                    case CsdlConstantExpressionKind.Bool:
                        return new CsdlSemanticsBooleanConstantExpression(constant);
                    case CsdlConstantExpressionKind.DateTime:
                        return new CsdlSemanticsDateTimeConstantExpression(constant);
                }
            }

            CsdlRecordExpression record = expression as CsdlRecordExpression;
            if (record != null)
            {
                return new CsdlSemanticsRecordExpression(record, bindingContext, schema);
            }

            CsdlPathExpression path = expression as CsdlPathExpression;
            return path != null ? new CsdlSemanticsPathExpression(path, bindingContext) : null;
        }

        private bool TryGetStandaloneAnnotations(IEdmNamedElement element, out ExternalAnnotations typeAnnotations)
        {
            typeAnnotations = null;
            if (this.externalAnnotationsDictionary != null)
            {
                string nameToTry = null;
                IEdmSchemaElement schemaElement = element as IEdmSchemaElement;
                if (schemaElement != null)
                {
                    nameToTry = schemaElement.FullName();
                }
                else
                {
                    IEdmEntityContainer container = element as IEdmEntityContainer;
                    if (container != null)
                    {
                        nameToTry = container.Name;
                    }
                    else
                    {
                        CsdlSemanticsEntitySet set = element as CsdlSemanticsEntitySet;
                        if (set != null)
                        {
                            nameToTry = set.Container.Name + "." + set.Name;
                        }
                    }
                }

                return nameToTry != null && this.externalAnnotationsDictionary.TryGetValue(nameToTry, out typeAnnotations);
            }

            return false;
        }

        internal static IEdmTypeReference WrapTypeReference(CsdlSemanticsSchema schema, CsdlTypeReference type)
        {
            var typeReference = type as CsdlNamedTypeReference;
            if (typeReference != null)
            {
                var primitiveReference = typeReference as CsdlPrimitiveTypeReference;
                if (primitiveReference != null)
                {
                    switch(primitiveReference.Kind)
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

        /// <summary>
        /// Searches for vocabulary annotations specified by this model or a referenced model for a given element.
        /// </summary>
        /// <param name="element">The annotated element.</param>
        /// <returns>The vocabulary annotations for the element.</returns>
        public override IEnumerable<IEdmVocabularyAnnotation> FindVocabularyAnnotations(IEdmAnnotatable element)
        {
            List<CsdlSemanticsAnnotations> elementAnnotations;
            string fullName = null;
            IEdmSchemaElement schemaElement = element as IEdmSchemaElement;
            if (schemaElement != null)
            {
                fullName = schemaElement.FullName();
            }
            else
            {
                IEdmEntityContainerElement containerElement = element as IEdmEntityContainerElement;
                if (containerElement != null)
                {
                    CsdlSemanticsEntityContainer container;
                    if (this.declaringContainers.TryGetValue(containerElement, out container))
                    {
                        fullName = container.Name + "." + containerElement.Name;
                    }
                }
            }

            if (fullName != null && this.outOfLineAnnotations.TryGetValue(fullName, out elementAnnotations))
            {
                List<IEdmVocabularyAnnotation> result = new List<IEdmVocabularyAnnotation>();

                foreach (CsdlSemanticsAnnotations annotations in elementAnnotations)
                {
                    foreach (CsdlVocabularyAnnotationBase annotation in annotations.Annotations.Annotations)
                    {
                        result.Add(WrapVocabularyAnnotation(annotation, annotations.Context, annotations.Context.FindElement(annotations.Annotations.Target), annotations, annotations.Annotations.Qualifier));
                    }
                }

                return result;
            }

            return Enumerable.Empty<IEdmVocabularyAnnotation>();
        }

        public void RegisterContainerElement(CsdlSemanticsEntityContainer container, IEdmEntityContainerElement element)
        {
            this.declaringContainers[element] = container;
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsSchema.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for CsdlSchema.
    /// </summary>
    internal class CsdlSemanticsSchema : CsdlSemanticsElement, IEdmCheckable
    {
        private readonly CsdlSemanticsModel model;
        private readonly CsdlSchema schema;

        private readonly Cache<CsdlSemanticsSchema, IEnumerable<IEdmSchemaType>> typesCache = new Cache<CsdlSemanticsSchema, IEnumerable<IEdmSchemaType>>();
        private static readonly Func<CsdlSemanticsSchema, IEnumerable<IEdmSchemaType>> ComputeTypesFunc = (me) => me.ComputeTypes();

        private readonly Cache<CsdlSemanticsSchema, IEnumerable<IEdmOperation>> operationsCache = new Cache<CsdlSemanticsSchema, IEnumerable<IEdmOperation>>();
        private static readonly Func<CsdlSemanticsSchema, IEnumerable<IEdmOperation>> ComputeFunctionsFunc = (me) => me.ComputeOperations();

        private readonly Cache<CsdlSemanticsSchema, IEnumerable<IEdmEntityContainer>> entityContainersCache = new Cache<CsdlSemanticsSchema, IEnumerable<IEdmEntityContainer>>();
        private static readonly Func<CsdlSemanticsSchema, IEnumerable<IEdmEntityContainer>> ComputeEntityContainersFunc = (me) => me.ComputeEntityContainers();

        private readonly Cache<CsdlSemanticsSchema, IEnumerable<IEdmTerm>> termsCache = new Cache<CsdlSemanticsSchema, IEnumerable<IEdmTerm>>();
        private static readonly Func<CsdlSemanticsSchema, IEnumerable<IEdmTerm>> ComputeTermsFunc = (me) => me.ComputeTerms();

        private readonly Cache<CsdlSemanticsSchema, Dictionary<string, object>> labeledExpressionsCache = new Cache<CsdlSemanticsSchema, Dictionary<string, object>>();
        private static readonly Func<CsdlSemanticsSchema, Dictionary<string, object>> ComputeLabeledExpressionsFunc = (me) => me.ComputeLabeledExpressions();

        private readonly Dictionary<CsdlLabeledExpression, IEdmLabeledExpression> semanticsLabeledElements = new Dictionary<CsdlLabeledExpression, IEdmLabeledExpression>();
        private readonly Dictionary<List<CsdlLabeledExpression>, IEdmLabeledExpression> ambiguousLabeledExpressions = new Dictionary<List<CsdlLabeledExpression>, IEdmLabeledExpression>();

        public CsdlSemanticsSchema(CsdlSemanticsModel model, CsdlSchema schema)
            : base(schema)
        {
            this.model = model;
            this.schema = schema;
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.model; }
        }

        public override CsdlElement Element
        {
            get { return this.schema; }
        }

        public IEnumerable<IEdmSchemaType> Types
        {
            get { return this.typesCache.GetValue(this, ComputeTypesFunc, null); }
        }

        public IEnumerable<IEdmOperation> Operations
        {
            get { return this.operationsCache.GetValue(this, ComputeFunctionsFunc, null); }
        }

        public IEnumerable<IEdmTerm> Terms
        {
            get { return this.termsCache.GetValue(this, ComputeTermsFunc, null); }
        }

        public IEnumerable<IEdmEntityContainer> EntityContainers
        {
            get { return this.entityContainersCache.GetValue(this, ComputeEntityContainersFunc, null); }
        }

        public string Namespace
        {
            get { return this.schema.Namespace; }
        }

        public IEnumerable<EdmError> Errors
        {
            get
            {
                return Enumerable.Empty<EdmError>();
            }
        }

        /// <summary>
        /// Gets the labeled element expressions dictionary.
        /// Each value in the dictionary is either a <see cref="CsdlLabeledExpression"/> or a list of same.
        /// </summary>
        private Dictionary<string, object> LabeledExpressions
        {
            get { return this.labeledExpressionsCache.GetValue(this, ComputeLabeledExpressionsFunc, null); }
        }

        public IEnumerable<IEdmOperation> FindOperations(string name)
        {
            return FindSchemaElement<IEnumerable<IEdmOperation>>(name, ExtensionMethods.FindOperationsInModelTree);
        }

        public IEdmSchemaType FindType(string name)
        {
            return FindSchemaElement<IEdmSchemaType>(name, ExtensionMethods.FindTypeInModelTree);
        }

        public IEdmTerm FindTerm(string name)
        {
            return FindSchemaElement<IEdmTerm>(name, FindTerm);
        }

        public IEdmEntityContainer FindEntityContainer(string name)
        {
            return FindSchemaElement<IEdmEntityContainer>(name, FindEntityContainer);
        }

        public T FindSchemaElement<T>(string name, Func<CsdlSemanticsModel, string, T> modelFinder)
        {
            string candidateName = ReplaceAlias(name);
            if (candidateName == null)
            {
                candidateName = name;
            }

            return modelFinder(this.model, candidateName);
        }

        public string UnresolvedName(string qualifiedName)
        {
            if (qualifiedName == null)
            {
                return null;
            }

            return ReplaceAlias(qualifiedName) ?? qualifiedName;
        }

        public IEdmLabeledExpression FindLabeledElement(string label, IEdmEntityType bindingContext)
        {
            object labeledElement;
            if (this.LabeledExpressions.TryGetValue(label, out labeledElement))
            {
                CsdlLabeledExpression labeledElementExpression = labeledElement as CsdlLabeledExpression;
                if (labeledElementExpression != null)
                {
                    return this.WrapLabeledElement(labeledElementExpression, bindingContext);
                }

                return this.WrapLabeledElementList((List<CsdlLabeledExpression>)labeledElement, bindingContext);
            }

            return null;
        }

        public IEdmLabeledExpression WrapLabeledElement(CsdlLabeledExpression labeledElement, IEdmEntityType bindingContext)
        {
            IEdmLabeledExpression result;

            // Guarantee that multiple requests to wrap a given labeled element all return the same object.
            if (!this.semanticsLabeledElements.TryGetValue(labeledElement, out result))
            {
                result = new CsdlSemanticsLabeledExpression(labeledElement.Label, labeledElement.Element, bindingContext, this);
                this.semanticsLabeledElements[labeledElement] = result;
            }

            return result;
        }

        internal string ReplaceAlias(string name)
        {
            return this.model.ReplaceAlias(name);
        }

        private static IEdmTerm FindTerm(IEdmModel model, string name)
        {
            return model.FindTerm(name);
        }

        private static IEdmEntityContainer FindEntityContainer(IEdmModel model, string name)
        {
            return model.FindEntityContainer(name);
        }

        private static void AddLabeledExpressions(CsdlExpressionBase expression, Dictionary<string, object> result)
        {
            if (expression == null)
            {
                return;
            }

            switch (expression.ExpressionKind)
            {
                case EdmExpressionKind.Labeled:
                    {
                        CsdlLabeledExpression labeledElement = (CsdlLabeledExpression)expression;
                        string label = labeledElement.Label;
                        object duplicateLabeledElement;
                        if (result.TryGetValue(label, out duplicateLabeledElement))
                        {
                            // If the label has multiple definitions, store the duplicates as a list of labeled elements.
                            List<CsdlLabeledExpression> duplicates = duplicateLabeledElement as List<CsdlLabeledExpression>;
                            if (duplicates == null)
                            {
                                duplicates = new List<CsdlLabeledExpression>();
                                duplicates.Add((CsdlLabeledExpression)duplicateLabeledElement);
                                result[label] = duplicates;
                            }

                            duplicates.Add(labeledElement);
                        }
                        else
                        {
                            result[label] = labeledElement;
                        }

                        AddLabeledExpressions(labeledElement.Element, result);
                        break;
                    }

                case EdmExpressionKind.Collection:
                    foreach (CsdlExpressionBase element in ((CsdlCollectionExpression)expression).ElementValues)
                    {
                        AddLabeledExpressions(element, result);
                    }

                    break;
                case EdmExpressionKind.FunctionApplication:
                    foreach (CsdlExpressionBase argument in ((CsdlApplyExpression)expression).Arguments)
                    {
                        AddLabeledExpressions(argument, result);
                    }

                    break;
                case EdmExpressionKind.Record:
                    foreach (CsdlPropertyValue property in ((CsdlRecordExpression)expression).PropertyValues)
                    {
                        AddLabeledExpressions(property.Expression, result);
                    }

                    break;
                case EdmExpressionKind.If:
                    {
                        CsdlIfExpression ifExpression = (CsdlIfExpression)expression;
                        AddLabeledExpressions(ifExpression.Test, result);
                        AddLabeledExpressions(ifExpression.IfTrue, result);
                        AddLabeledExpressions(ifExpression.IfFalse, result);

                        break;
                    }

                case EdmExpressionKind.IsType:
                    AddLabeledExpressions(((CsdlIsTypeExpression)expression).Operand, result);
                    break;
                case EdmExpressionKind.Cast:
                    AddLabeledExpressions(((CsdlCastExpression)expression).Operand, result);
                    break;
                default:
                    break;
            }
        }

        private static void AddLabeledExpressions(IEnumerable<CsdlAnnotation> annotations, Dictionary<string, object> result)
        {
            foreach (CsdlAnnotation annotation in annotations)
            {
                if (annotation != null)
                {
                    AddLabeledExpressions(annotation.Expression, result);
                }
            }
        }

        private IEdmLabeledExpression WrapLabeledElementList(List<CsdlLabeledExpression> labeledExpressions, IEdmEntityType bindingContext)
        {
            IEdmLabeledExpression result;

            // Guarantee that multiple requests to wrap a given labeled element all return the same object.
            if (!this.ambiguousLabeledExpressions.TryGetValue(labeledExpressions, out result))
            {
                foreach (CsdlLabeledExpression labeledExpression in labeledExpressions)
                {
                    IEdmLabeledExpression wrappedExpression = this.WrapLabeledElement(labeledExpression, bindingContext);
                    result =
                        result == null
                            ? wrappedExpression
                            : new AmbiguousLabeledExpressionBinding(result, wrappedExpression);
                }

                this.ambiguousLabeledExpressions[labeledExpressions] = result;
            }

            return result;
        }

        private IEnumerable<IEdmTerm> ComputeTerms()
        {
            List<IEdmTerm> terms = new List<IEdmTerm>();
            foreach (CsdlTerm valueTerm in this.schema.Terms)
            {
                terms.Add(new CsdlSemanticsTerm(this, valueTerm));
            }

            return terms;
        }

        private IEnumerable<IEdmEntityContainer> ComputeEntityContainers()
        {
            List<IEdmEntityContainer> entityContainers = new List<IEdmEntityContainer>();
            foreach (CsdlEntityContainer entityContainer in this.schema.EntityContainers)
            {
                entityContainers.Add(new CsdlSemanticsEntityContainer(this, entityContainer));
            }

            return entityContainers;
        }

        private IEnumerable<IEdmOperation> ComputeOperations()
        {
            List<IEdmOperation> operations = new List<IEdmOperation>();
            foreach (CsdlOperation operation in this.schema.Operations)
            {
                CsdlAction action = operation as CsdlAction;
                if (action != null)
                {
                    operations.Add(new CsdlSemanticsAction(this, action));
                }
                else
                {
                    CsdlFunction function = operation as CsdlFunction;
                    Debug.Assert(function != null, "function != null");
                    operations.Add(new CsdlSemanticsFunction(this, function));
                }
            }

            return operations;
        }

        private IEnumerable<IEdmSchemaType> ComputeTypes()
        {
            List<IEdmSchemaType> types = new List<IEdmSchemaType>();

            foreach (var typeDefinition in schema.TypeDefinitions)
            {
                CsdlSemanticsTypeDefinitionDefinition edmTypeDefinition =
                    new CsdlSemanticsTypeDefinitionDefinition(this, typeDefinition);
                this.AttachDefaultPrimitiveValueConverter(typeDefinition, edmTypeDefinition);
                types.Add(edmTypeDefinition);
            }

            foreach (var structuredType in this.schema.StructuredTypes)
            {
                CsdlEntityType entity = structuredType as CsdlEntityType;
                if (entity != null)
                {
                    types.Add(new CsdlSemanticsEntityTypeDefinition(this, entity));
                }
                else
                {
                    CsdlComplexType complex = structuredType as CsdlComplexType;
                    if (complex != null)
                    {
                        types.Add(new CsdlSemanticsComplexTypeDefinition(this, complex));
                    }
                }
            }

            foreach (var enumType in this.schema.EnumTypes)
            {
                types.Add(new CsdlSemanticsEnumTypeDefinition(this, enumType));
            }

            return types;
        }

        /// <summary>
        /// Attach DefaultPrimitiveValueConverter to the model if the name and the underlying type of the given type definition
        /// matches the default unsigned int type definitions defined in <see cref="PrimitiveValueConverterConstants"/>.
        /// </summary>
        /// <param name="typeDefinition">The type definition to be added to the schema.</param>
        /// <param name="edmTypeDefinition">The EDM type definition to be added to the model.</param>
        private void AttachDefaultPrimitiveValueConverter(CsdlTypeDefinition typeDefinition, IEdmTypeDefinition edmTypeDefinition)
        {
            Debug.Assert(typeDefinition != null, "typeDefinition != null");

            string defaultUnderlyingType;
            switch (typeDefinition.Name)
            {
                case PrimitiveValueConverterConstants.UInt16TypeName:
                    defaultUnderlyingType = PrimitiveValueConverterConstants.DefaultUInt16UnderlyingType;
                    break;
                case PrimitiveValueConverterConstants.UInt32TypeName:
                    defaultUnderlyingType = PrimitiveValueConverterConstants.DefaultUInt32UnderlyingType;
                    break;
                case PrimitiveValueConverterConstants.UInt64TypeName:
                    defaultUnderlyingType = PrimitiveValueConverterConstants.DefaultUInt64UnderlyingType;
                    break;
                default:
                    // Not unsigned int type definition.
                    return;
            }

            if (String.CompareOrdinal(defaultUnderlyingType, typeDefinition.UnderlyingTypeName) != 0)
            {
                // Not default underlying type for unsigned int.
                return;
            }

            this.Model.SetPrimitiveValueConverter(edmTypeDefinition, DefaultPrimitiveValueConverter.Instance);
        }

        /// <summary>
        /// All of the labeled expressions in a schema are collected into a dictionary so that references to them can be bound.
        /// The elements of the dictionary are Csdl objects and not CsdlSemantics objects because the semantics objects are not created
        /// until and unless necessary.
        /// </summary>
        /// <returns>A dictionary containing entries for all labeled expressions in the schema.</returns>
        private Dictionary<string, object> ComputeLabeledExpressions()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            foreach (CsdlAnnotations sourceAnnotations in this.schema.OutOfLineAnnotations)
            {
                AddLabeledExpressions(sourceAnnotations.Annotations, result);
            }

            foreach (CsdlStructuredType schemaType in this.schema.StructuredTypes)
            {
                AddLabeledExpressions(schemaType.VocabularyAnnotations, result);
                foreach (CsdlProperty property in schemaType.StructuralProperties)
                {
                    AddLabeledExpressions(property.VocabularyAnnotations, result);
                }
            }

            foreach (CsdlOperation operation in this.schema.Operations)
            {
                AddLabeledExpressions(operation.VocabularyAnnotations, result);
                foreach (CsdlOperationParameter parameter in operation.Parameters)
                {
                    AddLabeledExpressions(parameter.VocabularyAnnotations, result);
                }
            }

            foreach (CsdlTerm terms in this.schema.Terms)
            {
                AddLabeledExpressions(terms.VocabularyAnnotations, result);
            }

            foreach (CsdlEntityContainer container in this.schema.EntityContainers)
            {
                AddLabeledExpressions(container.VocabularyAnnotations, result);
                foreach (CsdlEntitySet set in container.EntitySets)
                {
                    AddLabeledExpressions(set.VocabularyAnnotations, result);
                }

                foreach (CsdlOperationImport import in container.OperationImports)
                {
                    AddLabeledExpressions(import.VocabularyAnnotations, result);
                    foreach (CsdlOperationParameter parameter in import.Parameters)
                    {
                        AddLabeledExpressions(parameter.VocabularyAnnotations, result);
                    }
                }
            }

            return result;
        }
    }
}

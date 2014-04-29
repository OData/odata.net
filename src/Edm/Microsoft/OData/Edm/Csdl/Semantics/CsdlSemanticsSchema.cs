//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Validation;

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

        private readonly Cache<CsdlSemanticsSchema, IEnumerable<IEdmValueTerm>> valueTermsCache = new Cache<CsdlSemanticsSchema, IEnumerable<IEdmValueTerm>>();
        private static readonly Func<CsdlSemanticsSchema, IEnumerable<IEdmValueTerm>> ComputeValueTermsFunc = (me) => me.ComputeValueTerms();

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

        public IEnumerable<IEdmValueTerm> ValueTerms
        {
            get { return this.valueTermsCache.GetValue(this, ComputeValueTermsFunc, null); }
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

        public IEdmValueTerm FindValueTerm(string name)
        {
            return FindSchemaElement<IEdmValueTerm>(name, FindValueTerm);
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

        private static IEdmValueTerm FindValueTerm(IEdmModel model, string name)
        {
            return model.FindValueTerm(name);
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
                case Expressions.EdmExpressionKind.Labeled:
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

                case Expressions.EdmExpressionKind.Collection:
                    foreach (CsdlExpressionBase element in ((CsdlCollectionExpression)expression).ElementValues)
                    {
                        AddLabeledExpressions(element, result);
                    }

                    break;
                case Expressions.EdmExpressionKind.OperationApplication:
                    foreach (CsdlExpressionBase argument in ((CsdlApplyExpression)expression).Arguments)
                    {
                        AddLabeledExpressions(argument, result);
                    }

                    break;
                case Expressions.EdmExpressionKind.Record:
                    foreach (CsdlPropertyValue property in ((CsdlRecordExpression)expression).PropertyValues)
                    {
                        AddLabeledExpressions(property.Expression, result);
                    }

                    break;
                case Expressions.EdmExpressionKind.If:
                    {
                        CsdlIfExpression ifExpression = (CsdlIfExpression)expression;
                        AddLabeledExpressions(ifExpression.Test, result);
                        AddLabeledExpressions(ifExpression.IfTrue, result);
                        AddLabeledExpressions(ifExpression.IfFalse, result);

                        break;
                    }

                case Expressions.EdmExpressionKind.IsType:
                    AddLabeledExpressions(((CsdlIsTypeExpression)expression).Operand, result);
                    break;
                case Expressions.EdmExpressionKind.Cast:
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

        private IEnumerable<IEdmValueTerm> ComputeValueTerms()
        {
            List<IEdmValueTerm> terms = new List<IEdmValueTerm>();
            foreach (CsdlTerm valueTerm in this.schema.Terms)
            {
                terms.Add(new CsdlSemanticsValueTerm(this, valueTerm));
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
                if (operation is CsdlAction)
                {
                    operations.Add(new CsdlSemanticsAction(this, (CsdlAction)operation));
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
                foreach (CsdlProperty property in schemaType.Properties)
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

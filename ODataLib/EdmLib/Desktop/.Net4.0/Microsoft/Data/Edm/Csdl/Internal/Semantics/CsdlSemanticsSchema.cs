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
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library.Internal;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
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

        private readonly Cache<CsdlSemanticsSchema, IEnumerable<CsdlSemanticsAssociation>> associationsCache = new Cache<CsdlSemanticsSchema, IEnumerable<CsdlSemanticsAssociation>>();
        private static readonly Func<CsdlSemanticsSchema, IEnumerable<CsdlSemanticsAssociation>> ComputeAssociationsFunc = (me) => me.ComputeAssociations();

        private readonly Cache<CsdlSemanticsSchema, IEnumerable<IEdmFunction>> functionsCache = new Cache<CsdlSemanticsSchema, IEnumerable<IEdmFunction>>();
        private static readonly Func<CsdlSemanticsSchema, IEnumerable<IEdmFunction>> ComputeFunctionsFunc = (me) => me.ComputeFunctions();

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

        public IEnumerable<CsdlSemanticsAssociation> Associations
        {
            get { return this.associationsCache.GetValue(this, ComputeAssociationsFunc, null); }
        }

        public IEnumerable<IEdmFunction> Functions
        {
            get { return this.functionsCache.GetValue(this, ComputeFunctionsFunc, null); }
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
                HashSetInternal<string> usedAlias = new HashSetInternal<string>();
                if (this.schema.Alias != null)
                {
                    usedAlias.Add(this.schema.Alias);
                }

                foreach (CsdlUsing usingStatement in this.schema.Usings)
                {
                    if (!usedAlias.Add(usingStatement.Alias))
                    {
                        return new EdmError[] { new EdmError(this.Location, EdmErrorCode.DuplicateAlias, Strings.CsdlSemantics_DuplicateAlias(this.Namespace, usingStatement.Alias)) };
                    }
                }

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

        public IEdmAssociation FindAssociation(string name)
        {
            return FindSchemaElement<IEdmAssociation>(name, FindAssociation);
        }

        public IEnumerable<IEdmFunction> FindFunctions(string name)
        {
            return FindSchemaElement<IEnumerable<IEdmFunction>>(name, FindFunctions);
        }

        public IEdmSchemaType FindType(string name)
        {
            return FindSchemaElement<IEdmSchemaType>(name, FindType);
        }

        public IEdmValueTerm FindValueTerm(string name)
        {
            return FindSchemaElement<IEdmValueTerm>(name, FindValueTerm);
        }

        public IEdmEntityContainer FindEntityContainer(string name)
        {
            return FindSchemaElement<IEdmEntityContainer>(name, FindEntityContainer);
        }

        public T FindSchemaElement<T>(string name, Func<IEdmModel, string, T> modelFinder)
        {
            string candidateName = ReplaceAlias(name);
            if (candidateName == null)
            {
                candidateName = name;
            }

            return modelFinder(this.model, candidateName);
        }

        public string ReplaceAlias(string name)
        {
            string replaced = ReplaceAlias(this.Namespace, this.schema.Alias, name);
            if (replaced == null)
            {
                foreach (CsdlUsing schemaUsing in this.schema.Usings)
                {
                    replaced = ReplaceAlias(schemaUsing.Namespace, schemaUsing.Alias, name);
                    if (replaced != null)
                    {
                        break;
                    }
                }
            }

            return replaced;
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

        private static string ReplaceAlias(string namespaceName, string namespaceAlias, string name)
        {
            if (namespaceAlias != null)
            {
                if (name.Length > namespaceAlias.Length && name.StartsWith(namespaceAlias, StringComparison.Ordinal) && name[namespaceAlias.Length] == '.')
                {
                    return (namespaceName ?? string.Empty) + name.Substring(namespaceAlias.Length);
                }
            }

            return null;
        }

        private static IEdmAssociation FindAssociation(IEdmModel model, string name)
        {
            return ((CsdlSemanticsModel)model).FindAssociation(name);
        }

        private static IEnumerable<IEdmFunction> FindFunctions(IEdmModel model, string name)
        {
            return model.FindFunctions(name);
        }

        private static IEdmSchemaType FindType(IEdmModel model, string name)
        {
            return model.FindType(name);
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
                case Expressions.EdmExpressionKind.FunctionApplication:
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
                case Expressions.EdmExpressionKind.AssertType:
                    AddLabeledExpressions(((CsdlAssertTypeExpression)expression).Operand, result);
                    break;
                default:
                    break;
            }
        }

        private static void AddLabeledExpressions(IEnumerable<CsdlVocabularyAnnotationBase> annotations, Dictionary<string, object> result)
        {
            foreach (CsdlVocabularyAnnotationBase sourceAnnotation in annotations)
            {
                CsdlValueAnnotation valueAnnotation = sourceAnnotation as CsdlValueAnnotation;
                if (valueAnnotation != null)
                {
                    AddLabeledExpressions(valueAnnotation.Expression, result);
                }
                else
                {
                    CsdlTypeAnnotation typeAnnotation = sourceAnnotation as CsdlTypeAnnotation;
                    if (typeAnnotation != null)
                    {
                        foreach (CsdlPropertyValue property in typeAnnotation.Properties)
                        {
                            AddLabeledExpressions(property.Expression, result);
                        }
                    }
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
            List<IEdmValueTerm> valueTerms = new List<IEdmValueTerm>();
            foreach (CsdlValueTerm valueTerm in this.schema.ValueTerms)
            {
                valueTerms.Add(new CsdlSemanticsValueTerm(this, valueTerm));
            }

            return valueTerms;
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

        private IEnumerable<CsdlSemanticsAssociation> ComputeAssociations()
        {
            List<CsdlSemanticsAssociation> associations = new List<CsdlSemanticsAssociation>();
            foreach (CsdlAssociation association in this.schema.Associations)
            {
                associations.Add(new CsdlSemanticsAssociation(this, association));
            }

            return associations;
        }

        private IEnumerable<IEdmFunction> ComputeFunctions()
        {
            List<IEdmFunction> functions = new List<IEdmFunction>();
            foreach (CsdlFunction function in this.schema.Functions)
            {
                functions.Add(new CsdlSemanticsFunction(this, function));
            }

            return functions;
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

            foreach (CsdlFunction function in this.schema.Functions)
            {
                AddLabeledExpressions(function.VocabularyAnnotations, result);
                foreach (CsdlFunctionParameter parameter in function.Parameters)
                {
                    AddLabeledExpressions(parameter.VocabularyAnnotations, result);
                }
            }

            foreach (CsdlValueTerm valueTerm in this.schema.ValueTerms)
            {
                AddLabeledExpressions(valueTerm.VocabularyAnnotations, result);
            }

            foreach (CsdlEntityContainer container in this.schema.EntityContainers)
            {
                AddLabeledExpressions(container.VocabularyAnnotations, result);
                foreach (CsdlEntitySet set in container.EntitySets)
                {
                    AddLabeledExpressions(set.VocabularyAnnotations, result);
                }

                foreach (CsdlFunctionImport import in container.FunctionImports)
                {
                    AddLabeledExpressions(import.VocabularyAnnotations, result);
                    foreach (CsdlFunctionParameter parameter in import.Parameters)
                    {
                        AddLabeledExpressions(parameter.VocabularyAnnotations, result);
                    }
                }
            }

            return result;
        }
    }
}

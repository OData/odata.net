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
using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.OData.Edm.Internal;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Internal;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Common base class for CsdlSemanticsTypeAnnotation and CsdlSemanticsValueAnnotation.
    /// </summary>
    internal abstract class CsdlSemanticsVocabularyAnnotation : CsdlSemanticsElement, IEdmVocabularyAnnotation, IEdmCheckable
    {
        protected readonly CsdlVocabularyAnnotationBase Annotation;
        private readonly CsdlSemanticsSchema schema;
        private readonly string qualifier;
        private readonly IEdmVocabularyAnnotatable targetContext;
        private readonly CsdlSemanticsAnnotations annotationsContext;

        private readonly Cache<CsdlSemanticsVocabularyAnnotation, IEdmTerm> termCache = new Cache<CsdlSemanticsVocabularyAnnotation, IEdmTerm>();
        private static readonly Func<CsdlSemanticsVocabularyAnnotation, IEdmTerm> ComputeTermFunc = (me) => me.ComputeTerm();

        // Target cache.
        private readonly Cache<CsdlSemanticsVocabularyAnnotation, IEdmVocabularyAnnotatable> targetCache = new Cache<CsdlSemanticsVocabularyAnnotation, IEdmVocabularyAnnotatable>();
        private static readonly Func<CsdlSemanticsVocabularyAnnotation, IEdmVocabularyAnnotatable> ComputeTargetFunc = (me) => me.ComputeTarget();

        protected CsdlSemanticsVocabularyAnnotation(CsdlSemanticsSchema schema, IEdmVocabularyAnnotatable targetContext, CsdlSemanticsAnnotations annotationsContext, CsdlVocabularyAnnotationBase annotation, string qualifier)
            : base(annotation)
        {
            this.schema = schema;
            this.Annotation = annotation;
            this.qualifier = qualifier ?? annotation.Qualifier;
            this.targetContext = targetContext;
            this.annotationsContext = annotationsContext;
        }

        public CsdlSemanticsSchema Schema
        {
            get { return this.schema; }
        }

        public override CsdlElement Element
        {
            get { return this.Annotation; }
        }

        public string Qualifier
        {
            get { return this.qualifier; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.schema.Model; }
        }

        public IEdmTerm Term
        {
            get { return this.termCache.GetValue(this, ComputeTermFunc, null); }
        }

        public IEdmVocabularyAnnotatable Target
        {
            get { return this.targetCache.GetValue(this, ComputeTargetFunc, null); }
        }

        public IEnumerable<EdmError> Errors
        {
            get
            {
                if (this.Term is IUnresolvedElement)
                {
                    return this.Term.Errors();
                }

                if (this.Target is IUnresolvedElement)
                {
                    return this.Target.Errors();
                }

                return Enumerable.Empty<EdmError>();
            }
        }

        /// <summary>
        /// Gets the type to use as a binding context for expressions in the annotation. If the target of the annotation
        /// is an entity type, that is the binding context. If the target is an entity set, the binding context is the
        /// element type of the set.
        /// </summary>
        public IEdmEntityType TargetBindingContext
        {
            get
            {
                IEdmVocabularyAnnotatable bindingTarget = this.Target;
                IEdmEntityType bindingContext = bindingTarget as IEdmEntityType;
                if (bindingContext == null)
                {
                    IEdmEntitySet entitySet = bindingTarget as IEdmEntitySet;
                    if (entitySet != null)
                    {
                        bindingContext = entitySet.ElementType;
                    }
                }

                return bindingContext;
            }
        }

        protected abstract IEdmTerm ComputeTerm();

        private IEdmVocabularyAnnotatable ComputeTarget()
        {
            if (this.targetContext != null)
            {
                return this.targetContext;
            }
            else
            {
                Debug.Assert(this.annotationsContext != null, "Annotation must either have a target context or annotations context");
                string target = this.annotationsContext.Annotations.Target;
                string[] targetSegments = target.Split('/');
                int targetSegmentsCount = targetSegments.Count();
                IEdmEntityContainer container;

                if (targetSegmentsCount == 1)
                {
                    string elementName = targetSegments[0];
                    IEdmSchemaType type = this.schema.FindType(elementName);
                    if (type != null)
                    {
                        return type;
                    }

                    IEdmValueTerm term = this.schema.FindValueTerm(elementName);
                    if (term != null)
                    {
                        return term;
                    }

                    IEdmOperation operation = this.FindParameterizedFunction(elementName, this.Schema.FindOperations, this.CreateAmbiguousOperation);
                    if (operation != null)
                    {
                        return operation;
                    }

                    container = this.schema.FindEntityContainer(elementName);
                    if (container != null)
                    {
                        return container;
                    }

                    return new UnresolvedType(this.Schema.UnresolvedName(targetSegments[0]), this.Location);
                }

                if (targetSegmentsCount == 2)
                {
                    container = this.schema.FindEntityContainer(targetSegments[0]);
                    if (container != null)
                    {
                        IEdmEntityContainerElement containerElement = container.FindEntitySet(targetSegments[1]);
                        if (containerElement != null)
                        {
                            return containerElement;
                        }

                        IEdmOperationImport operationImport = this.FindParameterizedFunction(targetSegments[1], container.FindOperationImports, this.CreateAmbiguousOperationImport);
                        if (operationImport != null)
                        {
                            return operationImport;
                        }

                        return new UnresolvedEntitySet(targetSegments[1], container, this.Location);
                    }

                    IEdmStructuredType type = this.schema.FindType(targetSegments[0]) as IEdmStructuredType;
                    if (type != null)
                    {
                        IEdmProperty property = type.FindProperty(targetSegments[1]);
                        if (property != null)
                        {
                            return property;
                        }

                        return new UnresolvedProperty(type, targetSegments[1], this.Location);
                    }

                    IEdmOperation operation = this.FindParameterizedFunction(targetSegments[0], this.Schema.FindOperations, this.CreateAmbiguousOperation);
                    if (operation != null)
                    {
                        IEdmOperationParameter parameter = operation.FindParameter(targetSegments[1]);
                        if (parameter != null)
                        {
                            return parameter;
                        }

                        return new UnresolvedParameter(operation, targetSegments[1], this.Location);
                    }

                    return new UnresolvedProperty(new UnresolvedEntityType(this.Schema.UnresolvedName(targetSegments[0]), this.Location), targetSegments[1], this.Location);
                }

                if (targetSegmentsCount == 3)
                {
                    // The only valid target with three segments is a function parameter.
                    string containerName = targetSegments[0];
                    string operationName = targetSegments[1];
                    string parameterName = targetSegments[2];

                    container = this.Model.FindEntityContainer(containerName);
                    if (container != null)
                    {
                        IEdmOperationImport operationImport = this.FindParameterizedFunction(operationName, container.FindOperationImports, this.CreateAmbiguousOperationImport);
                        if (operationImport != null)
                        {
                            IEdmOperationParameter parameter = operationImport.FindParameter(parameterName);
                            if (parameter != null)
                            {
                                return parameter;
                            }

                            return new UnresolvedParameter(operationImport, parameterName, this.Location);
                        }
                    }

                    string qualifiedOperationName = containerName + "/" + operationName;
                    UnresolvedOperation unresolvedOperation = new UnresolvedOperation(qualifiedOperationName, Edm.Strings.Bad_UnresolvedOperation(qualifiedOperationName), this.Location);
                    return new UnresolvedParameter(unresolvedOperation, parameterName, this.Location);
                }

                return new BadElement(new EdmError[] { new EdmError(this.Location, EdmErrorCode.ImpossibleAnnotationsTarget, Edm.Strings.CsdlSemantics_ImpossibleAnnotationsTarget(target)) });
            }
        }

        private T FindParameterizedFunction<T>(string parameterizedName, Func<string, IEnumerable<T>> findFunctions, Func<IEnumerable<T>, T> ambiguityCreator)
             where T : class, IEdmFunctionBase
        {
            int openParen = parameterizedName.IndexOf('(');
            int closeParen = parameterizedName.LastIndexOf(')');
            if (openParen < 0)
            {
                return null;
            }

            string name = parameterizedName.Substring(0, openParen);
            string[] parameters = parameterizedName.Substring(openParen + 1, closeParen - (openParen + 1)).Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<T> matchingFunctions = this.FindParameterizedFunctionFromList(findFunctions(name).Cast<IEdmFunctionBase>(), parameters).Cast<T>();
            if (matchingFunctions.Count() == 0)
            {
                return null;
            }

            if (matchingFunctions.Count() == 1)
            {
                return matchingFunctions.First();
            }
            else
            {
                T ambiguous = ambiguityCreator(matchingFunctions);
                return ambiguous;
            }
        }

        private IEdmOperationImport CreateAmbiguousOperationImport(IEnumerable<IEdmOperationImport> operations)
        {
            Debug.Assert(operations.Count() > 1, "Should not make an ambiguous thing with fewer than two items");
            IEnumerator<IEdmOperationImport> operationEnumerator = operations.GetEnumerator();
            operationEnumerator.MoveNext();
            IEdmOperationImport first = operationEnumerator.Current;
            operationEnumerator.MoveNext();
            IEdmOperationImport second = operationEnumerator.Current;
            AmbiguousOperationImportBinding ambiguous = new AmbiguousOperationImportBinding(first, second);
            while (operationEnumerator.MoveNext())
            {
                ambiguous.AddBinding(operationEnumerator.Current);
            }

            return ambiguous;
        }

        private IEdmOperation CreateAmbiguousOperation(IEnumerable<IEdmOperation> operations)
        {
            Debug.Assert(operations.Count() > 1, "Should not make an ambiguous thing with fewer than two items");
            IEnumerator<IEdmOperation> operationEnumerator = operations.GetEnumerator();
            operationEnumerator.MoveNext();
            IEdmOperation first = operationEnumerator.Current;
            operationEnumerator.MoveNext();
            IEdmOperation second = operationEnumerator.Current;
            AmbiguousOperationBinding ambiguous = new AmbiguousOperationBinding(first, second);
            while (operationEnumerator.MoveNext())
            {
                ambiguous.AddBinding(operationEnumerator.Current);
            }

            return ambiguous;
        }

        private IEnumerable<IEdmFunctionBase> FindParameterizedFunctionFromList(IEnumerable<IEdmFunctionBase> functions, string[] parameters)
        {
            List<IEdmFunctionBase> matchingFunctions = new List<IEdmFunctionBase>();
            foreach (IEdmFunctionBase function in functions)
            {
                if (function.Parameters.Count() != parameters.Count())
                {
                    continue;
                }

                bool isMatch = true;
                IEnumerator<string> parameterTypeNameEnumerator = ((IEnumerable<string>)parameters).GetEnumerator();
                foreach (IEdmOperationParameter parameter in function.Parameters)
                {
                    parameterTypeNameEnumerator.MoveNext();
                    string[] typeInformation = parameterTypeNameEnumerator.Current.Split(new char[] { '(', ')' });
                    switch (typeInformation[0])
                    {
                        case CsdlConstants.Value_Collection:
                            isMatch = parameter.Type.IsCollection() && this.Schema.FindType(typeInformation[1]).IsEquivalentTo(parameter.Type.AsCollection().ElementType().Definition);
                            break;
                        case CsdlConstants.Value_Ref:
                            isMatch = parameter.Type.IsEntityReference() && this.Schema.FindType(typeInformation[1]).IsEquivalentTo(parameter.Type.AsEntityReference().EntityType());
                            break;
                        default:
                            isMatch = EdmCoreModel.Instance.FindDeclaredType(parameterTypeNameEnumerator.Current).IsEquivalentTo(parameter.Type.Definition) || this.Schema.FindType(parameterTypeNameEnumerator.Current).IsEquivalentTo(parameter.Type.Definition);
                            break;
                    }

                    if (!isMatch)
                    {
                        break;
                    }
                }

                if (isMatch)
                {
                    matchingFunctions.Add(function);
                }
            }

            return matchingFunctions;
        }
    }
}

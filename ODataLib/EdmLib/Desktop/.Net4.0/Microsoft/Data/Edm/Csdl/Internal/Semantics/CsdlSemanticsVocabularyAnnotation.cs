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
using System.Diagnostics;
using System.Linq;
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Library.Internal;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
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

                    IEdmFunction function = this.FindParameterizedFunction(elementName, this.Schema.FindFunctions, this.CreateAmbiguousFunction);
                    if (function != null)
                    {
                        return function;
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

                        IEdmFunctionImport functionImport = this.FindParameterizedFunction(targetSegments[1], container.FindFunctionImports, this.CreateAmbiguousFunctionImport);
                        if (functionImport != null)
                        {
                            return functionImport;
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

                    IEdmFunction function = this.FindParameterizedFunction(targetSegments[0], this.Schema.FindFunctions, this.CreateAmbiguousFunction);
                    if (function != null)
                    {
                        IEdmFunctionParameter parameter = function.FindParameter(targetSegments[1]);
                        if (parameter != null)
                        {
                            return parameter;
                        }

                        return new UnresolvedParameter(function, targetSegments[1], this.Location);
                    }

                    return new UnresolvedProperty(new UnresolvedEntityType(this.Schema.UnresolvedName(targetSegments[0]), this.Location), targetSegments[1], this.Location);
                }

                if (targetSegmentsCount == 3)
                {
                    // The only valid target with three segments is a function parameter.
                    string containerName = targetSegments[0];
                    string functionName = targetSegments[1];
                    string parameterName = targetSegments[2];

                    container = this.Model.FindEntityContainer(containerName);
                    if (container != null)
                    {
                        IEdmFunctionImport functionImport = this.FindParameterizedFunction(functionName, container.FindFunctionImports, this.CreateAmbiguousFunctionImport);
                        if (functionImport != null)
                        {
                            IEdmFunctionParameter parameter = functionImport.FindParameter(parameterName);
                            if (parameter != null)
                            {
                                return parameter;
                            }

                            return new UnresolvedParameter(functionImport, parameterName, this.Location);
                        }
                    }

                    string qualifiedFunctionName = containerName + "/" + functionName;
                    UnresolvedFunction unresolvedFunction = new UnresolvedFunction(qualifiedFunctionName, Edm.Strings.Bad_UnresolvedFunction(qualifiedFunctionName), this.Location);
                    return new UnresolvedParameter(unresolvedFunction, parameterName, this.Location);
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

        private IEdmFunctionImport CreateAmbiguousFunctionImport(IEnumerable<IEdmFunctionImport> functions)
        {
            Debug.Assert(functions.Count() > 1, "Should not make an ambiguous thing with fewer than two items");
            IEnumerator<IEdmFunctionImport> functionEnumerator = functions.GetEnumerator();
            functionEnumerator.MoveNext();
            IEdmFunctionImport first = functionEnumerator.Current;
            functionEnumerator.MoveNext();
            IEdmFunctionImport second = functionEnumerator.Current;
            AmbiguousFunctionImportBinding ambiguous = new AmbiguousFunctionImportBinding(first, second);
            while (functionEnumerator.MoveNext())
            {
                ambiguous.AddBinding(functionEnumerator.Current);
            }

            return ambiguous;
        }

        private IEdmFunction CreateAmbiguousFunction(IEnumerable<IEdmFunction> functions)
        {
            Debug.Assert(functions.Count() > 1, "Should not make an ambiguous thing with fewer than two items");
            IEnumerator<IEdmFunction> functionEnumerator = functions.GetEnumerator();
            functionEnumerator.MoveNext();
            IEdmFunction first = functionEnumerator.Current;
            functionEnumerator.MoveNext();
            IEdmFunction second = functionEnumerator.Current;
            AmbiguousFunctionBinding ambiguous = new AmbiguousFunctionBinding(first, second);
            while (functionEnumerator.MoveNext())
            {
                ambiguous.AddBinding(functionEnumerator.Current);
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
                foreach (IEdmFunctionParameter functionParameter in function.Parameters)
                {
                    parameterTypeNameEnumerator.MoveNext();
                    string[] typeInformation = parameterTypeNameEnumerator.Current.Split(new char[] { '(', ')' });
                    switch (typeInformation[0])
                    {
                        case CsdlConstants.Value_Collection:
                            isMatch = functionParameter.Type.IsCollection() && this.Schema.FindType(typeInformation[1]).IsEquivalentTo(functionParameter.Type.AsCollection().ElementType().Definition);
                            break;
                        case CsdlConstants.Value_Ref:
                            isMatch = functionParameter.Type.IsEntityReference() && this.Schema.FindType(typeInformation[1]).IsEquivalentTo(functionParameter.Type.AsEntityReference().EntityType());
                            break;
                        default:
                            isMatch = EdmCoreModel.Instance.FindDeclaredType(parameterTypeNameEnumerator.Current).IsEquivalentTo(functionParameter.Type.Definition) || this.Schema.FindType(parameterTypeNameEnumerator.Current).IsEquivalentTo(functionParameter.Type.Definition);
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

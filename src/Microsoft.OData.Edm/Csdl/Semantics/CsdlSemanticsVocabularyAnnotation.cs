//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsVocabularyAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlAnnotation.
    /// </summary>
    internal class CsdlSemanticsVocabularyAnnotation : CsdlSemanticsElement, IEdmVocabularyAnnotation, IEdmCheckable
    {
        protected readonly CsdlAnnotation Annotation;
        private readonly CsdlSemanticsModel model;
        private readonly string qualifier;
        private readonly IEdmVocabularyAnnotatable targetContext;
        private readonly CsdlSemanticsAnnotations annotationsContext;
        private readonly Cache<CsdlSemanticsVocabularyAnnotation, IEdmExpression> valueCache = new Cache<CsdlSemanticsVocabularyAnnotation, IEdmExpression>();
        private static readonly Func<CsdlSemanticsVocabularyAnnotation, IEdmExpression> ComputeValueFunc = (me) => me.ComputeValue();

        private readonly Cache<CsdlSemanticsVocabularyAnnotation, IEdmTerm> termCache = new Cache<CsdlSemanticsVocabularyAnnotation, IEdmTerm>();
        private static readonly Func<CsdlSemanticsVocabularyAnnotation, IEdmTerm> ComputeTermFunc = (me) => me.ComputeTerm();

        // Target cache.
        private readonly Cache<CsdlSemanticsVocabularyAnnotation, IEdmVocabularyAnnotatable> targetCache = new Cache<CsdlSemanticsVocabularyAnnotation, IEdmVocabularyAnnotatable>();
        private static readonly Func<CsdlSemanticsVocabularyAnnotation, IEdmVocabularyAnnotatable> ComputeTargetFunc = (me) => me.ComputeTarget();

        public CsdlSemanticsVocabularyAnnotation(CsdlSemanticsModel model, CsdlSemanticsSchema schema, IEdmVocabularyAnnotatable targetContext, CsdlSemanticsAnnotations annotationsContext, CsdlAnnotation annotation, string qualifier)
            : base(annotation)
        {
            this.model = model;
            Schema = schema;
            this.Annotation = annotation;
            this.qualifier = qualifier ?? annotation.Qualifier;
            this.targetContext = targetContext;
            this.annotationsContext = annotationsContext;
        }

        public CsdlSemanticsSchema Schema { get; }

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
            get { return this.model; }
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
        /// is an entity type, that is the binding context. If the target is an entity set or singleton, the binding context is the
        /// element type of the set or singleton.
        /// </summary>
        public IEdmEntityType TargetBindingContext
        {
            get
            {
                IEdmVocabularyAnnotatable bindingTarget = this.Target;
                IEdmEntityType bindingContext = bindingTarget as IEdmEntityType;
                if (bindingContext == null)
                {
                    IEdmNavigationSource navigationSource = bindingTarget as IEdmNavigationSource;
                    if (navigationSource != null)
                    {
                        bindingContext = navigationSource.EntityType();
                    }
                }

                return bindingContext;
            }
        }

        public IEdmExpression Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        /// <summary>
        /// Gets whether the annotation uses a default value
        /// </summary>
        internal bool UseDefault
        {
            get { return this.Annotation.Expression == null; }
        }

        protected IEdmTerm ComputeTerm()
        {
            return this.model.FindTerm(this.Annotation.Term) ?? new UnresolvedVocabularyTerm(this.model.ReplaceAlias(this.Annotation.Term));
        }

        private IEdmExpression ComputeValue()
        {
            if (this.UseDefault)
            {
                return Term.GetDefaultValueExpression();
            }

            IEdmTypeReference termType = Term is UnresolvedVocabularyTerm ? null : Term.Type;
            CsdlExpressionBase adjustedExpression = AdjustStringConstantUsingTermType((this.Annotation).Expression, termType);

            return CsdlSemanticsModel.WrapExpression(adjustedExpression, TargetBindingContext, this.Schema);
        }

        private static CsdlExpressionBase AdjustStringConstantUsingTermType(CsdlExpressionBase expression, IEdmTypeReference termType)
        {
            if (expression == null || termType == null)
            {
                return expression;
            }

            switch (expression.ExpressionKind)
            {
                case EdmExpressionKind.Collection:
                    if (termType.IsCollection())
                    {
                        IEdmTypeReference elementType = termType.AsCollection().ElementType();
                        IList<CsdlExpressionBase> newElements = new List<CsdlExpressionBase>();
                        CsdlCollectionExpression collectionExp = (CsdlCollectionExpression)expression;

                        foreach (CsdlExpressionBase exp in collectionExp.ElementValues)
                        {
                            if (exp != null && exp.ExpressionKind == EdmExpressionKind.StringConstant)
                            {
                                newElements.Add(AdjustStringConstantUsingTermType(exp, elementType));
                            }
                            else
                            {
                                newElements.Add(exp);
                            }
                        }

                        return new CsdlCollectionExpression(collectionExp.Type, newElements, collectionExp.Location as CsdlLocation);
                    }

                    break;

                case EdmExpressionKind.StringConstant:
                    CsdlConstantExpression constantExp = (CsdlConstantExpression)expression;
                    switch (termType.TypeKind())
                    {
                        case EdmTypeKind.Primitive:
                            IEdmPrimitiveTypeReference primitiveTypeReference = (IEdmPrimitiveTypeReference)termType;
                            return BuildPrimitiveExpression(primitiveTypeReference, constantExp);

                        case EdmTypeKind.Path:
                            IEdmPathType pathType = (IEdmPathType)termType.Definition;
                            return BuildPathExpression(pathType, constantExp);

                        case EdmTypeKind.Enum:
                            IEdmEnumType enumType = (IEdmEnumType)termType.Definition;
                            return BuildEnumExpression(enumType, constantExp);
                    }

                    break;
            }

            return expression;
        }

        private static CsdlExpressionBase BuildEnumExpression(IEdmEnumType enumType, CsdlConstantExpression expression)
        {
            return new CsdlEnumMemberExpression(expression.Value, enumType, expression.Location as CsdlLocation);
        }

        private static CsdlConstantExpression BuildPrimitiveExpression(IEdmPrimitiveTypeReference primitiveTypeReference, CsdlConstantExpression expression)
        {
            Debug.Assert(expression.ExpressionKind == EdmExpressionKind.StringConstant);
            CsdlLocation location = expression.Location as CsdlLocation;

            switch (primitiveTypeReference.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Binary:
                    return new CsdlConstantExpression(EdmValueKind.Binary, expression.Value, location);

                case EdmPrimitiveTypeKind.Date:
                    return new CsdlConstantExpression(EdmValueKind.Date, expression.Value, location);

                case EdmPrimitiveTypeKind.DateTimeOffset:
                    return new CsdlConstantExpression(EdmValueKind.DateTimeOffset, expression.Value, location);

                case EdmPrimitiveTypeKind.Decimal:
                    // it maybe use the string for decimal
                    // The IEEE754Compatible=true parameter indicates that the service MUST serialize Edm.Decimal numbers as strings.
                    // The special values INF, -INF, or NaN are represented as strings.
                    return new CsdlConstantExpression(EdmValueKind.Decimal, expression.Value, location);

                case EdmPrimitiveTypeKind.Int64:
                    // The IEEE754Compatible=true parameter indicates that the service MUST serialize Edm.Int64 numbers as strings.
                    return new CsdlConstantExpression(EdmValueKind.Integer, expression.Value, location);

                case EdmPrimitiveTypeKind.Duration:
                    return new CsdlConstantExpression(EdmValueKind.Duration, expression.Value, location);

                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.Double:
                    // may have a string containing one of the special values INF, -INF, or NaN.
                    return new CsdlConstantExpression(EdmValueKind.Floating, expression.Value, location);

                case EdmPrimitiveTypeKind.Guid:
                    return new CsdlConstantExpression(EdmValueKind.Guid, expression.Value, location);

                case EdmPrimitiveTypeKind.TimeOfDay:
                    return new CsdlConstantExpression(EdmValueKind.TimeOfDay, expression.Value, location);

                case EdmPrimitiveTypeKind.String:
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Stream:
                case EdmPrimitiveTypeKind.PrimitiveType:
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
                case EdmPrimitiveTypeKind.None:
                default:
                    return expression;
            }
        }

        private static CsdlExpressionBase BuildPathExpression(IEdmPathType pathType, CsdlConstantExpression expression)
        {
            Debug.Assert(expression.ExpressionKind == EdmExpressionKind.StringConstant);
            CsdlLocation location = expression.Location as CsdlLocation;

            switch (pathType.PathKind)
            {
                case EdmPathTypeKind.AnnotationPath:
                    return new CsdlAnnotationPathExpression(expression.Value, location);

                case EdmPathTypeKind.PropertyPath:
                    return new CsdlPropertyPathExpression(expression.Value, location);

                case EdmPathTypeKind.NavigationPropertyPath:
                    return new CsdlNavigationPropertyPathExpression(expression.Value, location);

                case EdmPathTypeKind.None:
                default:
                    return expression;
            }
        }

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
                int targetSegmentsCount = targetSegments.Length;
                IEdmEntityContainer container;

                if (targetSegmentsCount == 1)
                {
                    string elementName = targetSegments[0];
                    IEdmSchemaType type = this.model.FindType(elementName);
                    if (type != null)
                    {
                        return type;
                    }

                    IEdmTerm term = this.model.FindTerm(elementName);
                    if (term != null)
                    {
                        return term;
                    }

                    IEdmOperation operation = this.FindParameterizedOperation(elementName, this.model.FindOperations, this.CreateAmbiguousOperation);
                    if (operation != null)
                    {
                        return operation;
                    }

                    container = this.model.FindEntityContainer(elementName);
                    if (container != null)
                    {
                        return container;
                    }

                    return new UnresolvedType(this.model.ReplaceAlias(targetSegments[0]), this.Location);
                }

                if (targetSegmentsCount == 2)
                {
                    container = this.model.FindEntityContainer(targetSegments[0]);
                    if (container != null)
                    {
                        // Using the methods here results in a much faster lookup as it uses a dictionary instead of using the list of container elements.
                        IEdmEntityContainerElement containerElement = container.FindEntitySetExtended(targetSegments[1])
                                                                      ?? container.FindSingletonExtended(targetSegments[1]) as IEdmEntityContainerElement;
                        if (containerElement != null)
                        {
                            return containerElement;
                        }

                        IEdmOperationImport operationImport = FindParameterizedOperationImport(targetSegments[1], container.FindOperationImportsExtended, this.CreateAmbiguousOperationImport);
                        if (operationImport != null)
                        {
                            return operationImport;
                        }

                        return new UnresolvedEntitySet(targetSegments[1], container, this.Location);
                    }

                    IEdmSchemaType type = this.model.FindType(targetSegments[0]);
                    if (type != null)
                    {
                        IEdmStructuredType structuredType;
                        IEdmEnumType enumType;
                        if ((structuredType = type as IEdmStructuredType) != null)
                        {
                            IEdmProperty property = structuredType.FindProperty(targetSegments[1]);
                            if (property != null)
                            {
                                return property;
                            }

                            return new UnresolvedProperty(structuredType, targetSegments[1], this.Location);
                        }
                        else if ((enumType = type as IEdmEnumType) != null)
                        {
                            foreach (IEdmEnumMember member in enumType.Members)
                            {
                                if (String.Equals(member.Name, targetSegments[1], StringComparison.OrdinalIgnoreCase))
                                {
                                    return member;
                                }
                            }

                            return new UnresolvedEnumMember(targetSegments[1], enumType, this.Location);
                        }
                    }

                    IEdmOperation operation = this.FindParameterizedOperation(targetSegments[0], this.model.FindOperations, this.CreateAmbiguousOperation);
                    if (operation != null)
                    {
                        // $ReturnType
                        if (targetSegments[1] == CsdlConstants.OperationReturnExternalTarget)
                        {
                            if (operation.ReturnType != null)
                            {
                                return operation.GetReturn();
                            }

                            return new UnresolvedReturn(operation, this.Location);
                        }

                        IEdmOperationParameter parameter = operation.FindParameter(targetSegments[1]);
                        if (parameter != null)
                        {
                            return parameter;
                        }

                        return new UnresolvedParameter(operation, targetSegments[1], this.Location);
                    }

                    return new UnresolvedProperty(new UnresolvedEntityType(this.model.ReplaceAlias(targetSegments[0]), this.Location), targetSegments[1], this.Location);
                }

                if (targetSegmentsCount == 3)
                {
                    // The only valid target with three segments is a function parameter, or an operation return.
                    string containerName = targetSegments[0];
                    string operationName = targetSegments[1];
                    string parameterName = targetSegments[2];

                    container = this.Model.FindEntityContainer(containerName);
                    if (container != null)
                    {
                        IEdmOperationImport operationImport = FindParameterizedOperationImport(operationName, container.FindOperationImportsExtended, this.CreateAmbiguousOperationImport);
                        if (operationImport != null)
                        {
                            // $ReturnType
                            if (parameterName == CsdlConstants.OperationReturnExternalTarget)
                            {
                                if (operationImport.Operation.ReturnType != null)
                                {
                                    return operationImport.Operation.GetReturn();
                                }

                                return new UnresolvedReturn(operationImport.Operation, this.Location);
                            }

                            IEdmOperationParameter parameter = operationImport.Operation.FindParameter(parameterName);
                            if (parameter != null)
                            {
                                return parameter;
                            }

                            return new UnresolvedParameter(operationImport.Operation, parameterName, this.Location);
                        }
                    }

                    string qualifiedOperationName = containerName + "/" + operationName;
                    UnresolvedOperation unresolvedOperation = new UnresolvedOperation(qualifiedOperationName, Edm.Strings.Bad_UnresolvedOperation(qualifiedOperationName), this.Location);
                    if (parameterName == CsdlConstants.OperationReturnExternalTarget)
                    {
                        return new UnresolvedReturn(unresolvedOperation, this.Location);
                    }
                    else
                    {
                        return new UnresolvedParameter(unresolvedOperation, parameterName, this.Location);
                    }
                }

                return new BadElement(new EdmError[] { new EdmError(this.Location, EdmErrorCode.ImpossibleAnnotationsTarget, Edm.Strings.CsdlSemantics_ImpossibleAnnotationsTarget(target)) });
            }
        }

        private static IEdmOperationImport FindParameterizedOperationImport(string parameterizedName, Func<string, IEnumerable<IEdmOperationImport>> findFunctions, Func<IEnumerable<IEdmOperationImport>, IEdmOperationImport> ambiguityCreator)
        {
            IEnumerable<IEdmOperationImport> matchingFunctions = findFunctions(parameterizedName);
            if (!matchingFunctions.Any())
            {
                return null;
            }

            if (matchingFunctions.Count() == 1)
            {
                return matchingFunctions.First();
            }
            else
            {
                IEdmOperationImport ambiguous = ambiguityCreator(matchingFunctions);
                return ambiguous;
            }
        }

        private IEdmOperation FindParameterizedOperation(string parameterizedName, Func<string, IEnumerable<IEdmOperation>> findFunctions, Func<IEnumerable<IEdmOperation>, IEdmOperation> ambiguityCreator)
        {
            int openParen = parameterizedName.IndexOf('(');
            int closeParen = parameterizedName.LastIndexOf(')');
            if (openParen < 0)
            {
                return null;
            }

            string name = parameterizedName.Substring(0, openParen);
            string[] parameters = parameterizedName.Substring(openParen + 1, closeParen - (openParen + 1)).Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<IEdmOperation> matchingFunctions = this.FindParameterizedOperationFromList(findFunctions(name).Cast<IEdmOperation>(), parameters);
            if (!matchingFunctions.Any())
            {
                return null;
            }

            if (matchingFunctions.Count() == 1)
            {
                return matchingFunctions.First();
            }
            else
            {
                IEdmOperation ambiguous = ambiguityCreator(matchingFunctions);
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

        private IEnumerable<IEdmOperation> FindParameterizedOperationFromList(IEnumerable<IEdmOperation> operations, string[] parameters)
        {
            List<IEdmOperation> matchingOperations = new List<IEdmOperation>();
            foreach (IEdmOperation function in operations)
            {
                if (function.Parameters.Count() != parameters.Length)
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
                            isMatch = parameter.Type.IsCollection() && this.model.FindType(typeInformation[1]).IsEquivalentTo(parameter.Type.AsCollection().ElementType().Definition);
                            break;
                        case CsdlConstants.Value_Ref:
                            isMatch = parameter.Type.IsEntityReference() && this.model.FindType(typeInformation[1]).IsEquivalentTo(parameter.Type.AsEntityReference().EntityType());
                            break;
                        default:
                            isMatch = EdmCoreModel.Instance.FindDeclaredType(parameterTypeNameEnumerator.Current).IsEquivalentTo(parameter.Type.Definition) || this.model.FindType(parameterTypeNameEnumerator.Current).IsEquivalentTo(parameter.Type.Definition);
                            break;
                    }

                    if (!isMatch)
                    {
                        break;
                    }
                }

                if (isMatch)
                {
                    matchingOperations.Add(function);
                }
            }

            return matchingOperations;
        }
    }
}

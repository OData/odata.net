//---------------------------------------------------------------------
// <copyright file="EdmExpressionEvaluator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Expression evaluator.
    /// </summary>
    public class EdmExpressionEvaluator
    {
        private readonly IDictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>> builtInFunctions;
        private readonly Dictionary<IEdmLabeledExpression, DelayedValue> labeledValues = new Dictionary<IEdmLabeledExpression, DelayedValue>();
        private readonly Func<string, IEdmValue[], IEdmValue> lastChanceOperationApplier;
        private readonly Func<IEdmModel, IEdmType, string, string, IEdmExpression> getAnnotationExpressionForType;
        private readonly Func<IEdmModel, IEdmType, string, string, string, IEdmExpression> getAnnotationExpressionForProperty;

        private readonly IEdmModel edmModel;
        private Func<string, IEdmModel, IEdmType> resolveTypeFromName = (typeName, edmModel) => FindEdmType(typeName, edmModel);

        /// <summary>
        /// Initializes a new instance of the EdmExpressionEvaluator class.
        /// </summary>
        /// <param name="builtInFunctions">Builtin functions dictionary to the evaluators for the functions.</param>
        public EdmExpressionEvaluator(IDictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>> builtInFunctions)
        {
            this.builtInFunctions = builtInFunctions;
        }

        /// <summary>
        /// Initializes a new instance of the EdmExpressionEvaluator class.
        /// </summary>
        /// <param name="builtInFunctions">Builtin functions dictionary to the evaluators for the functions.</param>
        /// <param name="lastChanceOperationApplier">Function to call to evaluate an application of a function with no static binding.</param>
        public EdmExpressionEvaluator(IDictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>> builtInFunctions, Func<string, IEdmValue[], IEdmValue> lastChanceOperationApplier)
            : this(builtInFunctions)
        {
            this.lastChanceOperationApplier = lastChanceOperationApplier;
        }

        /// <summary>
        /// Initializes a new instance of the EdmExpressionEvaluator class.
        /// </summary>
        /// <param name="builtInFunctions">Builtin functions dictionary to the evaluators for the functions.</param>
        /// <param name="lastChanceOperationApplier">Function to call to evaluate an application of a function with no static binding.</param>
        /// <param name="getAnnotationExpressionForType">Function to get the <see cref="IEdmExpression"/> of an annotation of an <see cref="IEdmType"/>.</param>
        /// <param name="getAnnotationExpressionForProperty">Function to get the <see cref="IEdmExpression"/> of an annotation of a property or navigation property in <see cref="IEdmType"/>.</param>
        /// <param name="edmModel">The edm model.</param>
        public EdmExpressionEvaluator(
            IDictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>> builtInFunctions,
            Func<string, IEdmValue[], IEdmValue> lastChanceOperationApplier,
            Func<IEdmModel, IEdmType, string, string, IEdmExpression> getAnnotationExpressionForType,
            Func<IEdmModel, IEdmType, string, string, string, IEdmExpression> getAnnotationExpressionForProperty,
            IEdmModel edmModel)
            : this(builtInFunctions, lastChanceOperationApplier)
        {
            this.getAnnotationExpressionForType = getAnnotationExpressionForType;
            this.getAnnotationExpressionForProperty = getAnnotationExpressionForProperty;
            this.edmModel = edmModel;
        }

        /// <summary>
        /// Function used to get edm type based on <see cref="IEdmModel"/> and the type name.
        /// </summary>
        protected Func<string, IEdmModel, IEdmType> ResolveTypeFromName
        {
            get { return this.resolveTypeFromName; }
            set { this.resolveTypeFromName = value; }
        }

        /// <summary>
        /// Evaluates an expression with no value context.
        /// </summary>
        /// <param name="expression">Expression to evaluate. The expression must not contain paths, because no context for evaluating a path is supplied.</param>
        /// <returns>The value that results from evaluating the expression in the context of the supplied value.</returns>
        public IEdmValue Evaluate(IEdmExpression expression)
        {
            EdmUtil.CheckArgumentNull(expression, "expression");

            return this.Eval(expression, null);
        }

        /// <summary>
        /// Evaluates an expression in the context of a value.
        /// </summary>
        /// <param name="expression">Expression to evaluate.</param>
        /// <param name="context">Value to use as context in evaluating the expression. Cannot be null if the expression contains paths.</param>
        /// <returns>The value that results from evaluating the expression in the context of the supplied value.</returns>
        public IEdmValue Evaluate(IEdmExpression expression, IEdmStructuredValue context)
        {
            EdmUtil.CheckArgumentNull(expression, "expression");

            return this.Eval(expression, context);
        }

        /// <summary>
        /// Evaluates an expression in the context of a value and a target type.
        /// </summary>
        /// <param name="expression">Expression to evaluate.</param>
        /// <param name="context">Value to use as context in evaluating the expression. Cannot be null if the expression contains paths.</param>
        /// <param name="targetType">Type to which the result value is expected to conform.</param>
        /// <returns>The value that results from evaluating the expression in the context of the supplied value, asserted to be of the target type.</returns>
        public IEdmValue Evaluate(IEdmExpression expression, IEdmStructuredValue context, IEdmTypeReference targetType)
        {
            EdmUtil.CheckArgumentNull(expression, "expression");
            EdmUtil.CheckArgumentNull(targetType, "targetType");

            return Cast(targetType, this.Eval(expression, context));
        }

        /// <summary>
        /// Get the <see cref=" IEdmType"/> of a specified edm type name from an <see cref="IEdmModel"/>.
        /// </summary>
        /// <param name="edmTypeName">The specified edm type name.</param>
        /// <param name="edmModel">The edm model.</param>
        /// <returns>The requested type, or null if no such type exists.</returns>
        protected static IEdmType FindEdmType(string edmTypeName, IEdmModel edmModel)
        {
            return edmModel.FindDeclaredType(edmTypeName);
        }

        private static bool InRange(Int64 value, Int64 min, Int64 max)
        {
            return value >= min && value <= max;
        }

        private static bool FitsInSingle(double value)
        {
            return value >= Single.MinValue && value <= Single.MaxValue;
        }

        private static bool MatchesType(IEdmTypeReference targetType, IEdmValue operand)
        {
            return MatchesType(targetType, operand, true);
        }

        private static bool MatchesType(IEdmTypeReference targetType, IEdmValue operand, bool testPropertyTypes)
        {
            IEdmTypeReference operandType = operand.Type;
            EdmValueKind operandKind = operand.ValueKind;

            if (operandType != null && operandKind != EdmValueKind.Null && operandType.Definition.IsOrInheritsFrom(targetType.Definition))
            {
                return true;
            }

            switch (operandKind)
            {
                case EdmValueKind.Binary:
                    if (targetType.IsBinary())
                    {
                        IEdmBinaryTypeReference targetBinary = targetType.AsBinary();
                        return targetBinary.IsUnbounded || !targetBinary.MaxLength.HasValue || targetBinary.MaxLength.Value >= ((IEdmBinaryValue)operand).Value.Length;
                    }

                    break;
                case EdmValueKind.Boolean:
                    return targetType.IsBoolean();
                case EdmValueKind.Date:
                    return targetType.IsDateOnly();
                case EdmValueKind.DateTimeOffset:
                    return targetType.IsDateTimeOffset();
                case EdmValueKind.Decimal:
                    return targetType.IsDecimal();
                case EdmValueKind.Guid:
                    return targetType.IsGuid();
                case EdmValueKind.Null:
                    return targetType.IsNullable;
                case EdmValueKind.Duration:
                    return targetType.IsDuration();
                case EdmValueKind.String:
                    if (targetType.IsString())
                    {
                        IEdmStringTypeReference targetString = targetType.AsString();
                        return targetString.IsUnbounded || !targetString.MaxLength.HasValue || targetString.MaxLength.Value >= ((IEdmStringValue)operand).Value.Length;
                    }

                    break;
                case EdmValueKind.TimeOfDay:
                    return targetType.IsTimeOnly();
                case EdmValueKind.Floating:
                    return targetType.IsDouble() || (targetType.IsSingle() && FitsInSingle(((IEdmFloatingValue)operand).Value));
                case EdmValueKind.Integer:
                    if (targetType.TypeKind() == EdmTypeKind.Primitive)
                    {
                        switch (targetType.AsPrimitive().PrimitiveKind())
                        {
                            case EdmPrimitiveTypeKind.Int16:
                                return InRange(((IEdmIntegerValue)operand).Value, Int16.MinValue, Int16.MaxValue);
                            case EdmPrimitiveTypeKind.Int32:
                                return InRange(((IEdmIntegerValue)operand).Value, Int32.MinValue, Int32.MaxValue);
                            case EdmPrimitiveTypeKind.SByte:
                                return InRange(((IEdmIntegerValue)operand).Value, SByte.MinValue, SByte.MaxValue);
                            case EdmPrimitiveTypeKind.Byte:
                                return InRange(((IEdmIntegerValue)operand).Value, Byte.MinValue, Byte.MaxValue);
                            case EdmPrimitiveTypeKind.Int64:
                            case EdmPrimitiveTypeKind.Single:
                            case EdmPrimitiveTypeKind.Double:
                                return true;
                        }
                    }

                    break;
                case EdmValueKind.Collection:
                    if (targetType.IsCollection())
                    {
                        IEdmTypeReference targetElementType = targetType.AsCollection().ElementType();

                        // This enumerates the entire collection, which is unfortunate.
                        foreach (IEdmDelayedValue elementValue in ((IEdmCollectionValue)operand).Elements)
                        {
                            if (!MatchesType(targetElementType, elementValue.Value))
                            {
                                return false;
                            }
                        }

                        return true;
                    }

                    break;
                case EdmValueKind.Enum:
                    return ((IEdmEnumValue)operand).Type.Definition.IsEquivalentTo(targetType.Definition);
                case EdmValueKind.Structured:
                    if (targetType.IsStructured())
                    {
                        return AssertOrMatchStructuredType(targetType.AsStructured(), (IEdmStructuredValue)operand, testPropertyTypes, null);
                    }

                    break;
            }

            return false;
        }

        private static IEdmValue Cast(IEdmTypeReference targetType, IEdmValue operand)
        {
            IEdmTypeReference operandType = operand.Type;
            EdmValueKind operandKind = operand.ValueKind;

            if ((operandType != null && operandKind != EdmValueKind.Null && operandType.Definition.IsOrInheritsFrom(targetType.Definition)) || targetType.TypeKind() == EdmTypeKind.None)
            {
                return operand;
            }

            bool matches = true;

            switch (operandKind)
            {
                case EdmValueKind.Collection:
                    if (targetType.IsCollection())
                    {
                        // Avoid enumerating the collection at this point.
                        return new CastCollectionValue(targetType.AsCollection(), (IEdmCollectionValue)operand);
                    }
                    else
                    {
                        matches = false;
                    }

                    break;

                case EdmValueKind.Structured:
                    if (targetType.IsStructured())
                    {
                        IEdmStructuredTypeReference structuredTargetType = targetType.AsStructured();
                        List<IEdmPropertyValue> newProperties = new List<IEdmPropertyValue>();
                        matches = AssertOrMatchStructuredType(structuredTargetType, (IEdmStructuredValue)operand, true, newProperties);
                        if (matches)
                        {
                            return new EdmStructuredValue(structuredTargetType, newProperties);
                        }
                    }
                    else
                    {
                        matches = false;
                    }

                    break;
                default:
                    matches = MatchesType(targetType, operand);
                    break;
            }

            if (!matches)
            {
                throw new InvalidOperationException(Error.Format(SRResources.Edm_Evaluator_FailedTypeAssertion, targetType.ToTraceString()));
            }

            return operand;
        }

        private static bool AssertOrMatchStructuredType(IEdmStructuredTypeReference structuredTargetType, IEdmStructuredValue structuredValue, bool testPropertyTypes, List<IEdmPropertyValue> newProperties)
        {
            // If the value has a nominal type, the target type must be derived from the nominal type for a type match to be possible.
            IEdmTypeReference operandType = structuredValue.Type;
            if (operandType != null && !structuredTargetType.StructuredDefinition().InheritsFrom(operandType.AsStructured().StructuredDefinition()))
            {
                return false;
            }

            HashSetInternal<IEdmPropertyValue> visitedProperties = new HashSetInternal<IEdmPropertyValue>();

            foreach (IEdmProperty property in structuredTargetType.StructuralProperties())
            {
                IEdmPropertyValue propertyValue = structuredValue.FindPropertyValue(property.Name);
                if (propertyValue == null)
                {
                    return false;
                }

                visitedProperties.Add(propertyValue);
                if (testPropertyTypes)
                {
                    if (newProperties != null)
                    {
                        newProperties.Add(new EdmPropertyValue(propertyValue.Name, Cast(property.Type, propertyValue.Value)));
                    }
                    else if (!MatchesType(property.Type, propertyValue.Value))
                    {
                        return false;
                    }
                }
            }

            if (structuredTargetType.IsEntity())
            {
                foreach (IEdmNavigationProperty property in structuredTargetType.AsEntity().NavigationProperties())
                {
                    IEdmPropertyValue propertyValue = structuredValue.FindPropertyValue(property.Name);
                    if (propertyValue == null)
                    {
                        return false;
                    }

                    // Make a superficial test of the navigation property value--check that it has a valid set of properties,
                    // but don't test their types.
                    if (testPropertyTypes && !MatchesType(property.Type, propertyValue.Value, false))
                    {
                        return false;
                    }

                    visitedProperties.Add(propertyValue);
                    if (newProperties != null)
                    {
                        newProperties.Add(propertyValue);
                    }
                }
            }

            //// Allow property values not mentioned in the target type, whether or not the target type is open.

            if (newProperties != null)
            {
                foreach (IEdmPropertyValue propertyValue in structuredValue.PropertyValues)
                {
                    if (!visitedProperties.Contains(propertyValue))
                    {
                        newProperties.Add(propertyValue);
                    }
                }
            }

            return true;
        }

        private IEdmValue Eval(IEdmExpression expression, IEdmStructuredValue context)
        {
            switch (expression.ExpressionKind)
            {
                case EdmExpressionKind.IntegerConstant:
                    return (IEdmIntegerConstantExpression)expression;
                case EdmExpressionKind.StringConstant:
                    return (IEdmStringConstantExpression)expression;
                case EdmExpressionKind.BinaryConstant:
                    return (IEdmBinaryConstantExpression)expression;
                case EdmExpressionKind.BooleanConstant:
                    return (IEdmBooleanConstantExpression)expression;
                case EdmExpressionKind.DateTimeOffsetConstant:
                    return (IEdmDateTimeOffsetConstantExpression)expression;
                case EdmExpressionKind.DecimalConstant:
                    return (IEdmDecimalConstantExpression)expression;
                case EdmExpressionKind.FloatingConstant:
                    return (IEdmFloatingConstantExpression)expression;
                case EdmExpressionKind.GuidConstant:
                    return (IEdmGuidConstantExpression)expression;
                case EdmExpressionKind.DurationConstant:
                    return (IEdmDurationConstantExpression)expression;
                case EdmExpressionKind.DateConstant:
                    return (IEdmDateConstantExpression)expression;
                case EdmExpressionKind.TimeOfDayConstant:
                    return (IEdmTimeOfDayConstantExpression)expression;
                case EdmExpressionKind.Null:
                    return (IEdmNullExpression)expression;
                case EdmExpressionKind.Path:
                case EdmExpressionKind.AnnotationPath:
                    {
                        if (context == null)
                        {
                            throw new InvalidOperationException(SRResources.Edm_Evaluator_NoContextPath);
                        }

                        IEdmPathExpression pathExpression = (IEdmPathExpression)expression;
                        IEdmValue result = context;
                        // Only Support Annotation in EntityType or ComplexType or Property or NavigationProperty.
                        // Empty Path is not supported.
                        foreach (string hop in pathExpression.PathSegments)
                        {
                            if (hop.Contains("@", StringComparison.Ordinal))
                            {
                                var currentPathSegmentInfos = hop.Split('@');
                                var propertyName = currentPathSegmentInfos[0];
                                var termInfo = currentPathSegmentInfos[1];
                                IEdmExpression termCastExpression = null;

                                if (!string.IsNullOrWhiteSpace(termInfo))
                                {
                                    var termInfos = termInfo.Split('#');
                                    if (termInfos.Length <= 2)
                                    {
                                        string termName = termInfos[0];
                                        string qualifier = termInfos.Length == 2 ? termInfos[1] : null;

                                        if (string.IsNullOrWhiteSpace(propertyName) && this.getAnnotationExpressionForType != null)
                                        {
                                            termCastExpression = this.getAnnotationExpressionForType(this.edmModel, context.Type.Definition, termName, qualifier);
                                        }
                                        else if (!string.IsNullOrWhiteSpace(propertyName) && this.getAnnotationExpressionForProperty != null)
                                        {
                                            termCastExpression = this.getAnnotationExpressionForProperty(this.edmModel, context.Type.Definition, propertyName, termName, qualifier);
                                        }
                                    }
                                }

                                if (termCastExpression == null)
                                {
                                    result = null;
                                    break;
                                }

                                result = this.Eval(termCastExpression, context);
                            }
                            else if (hop == "$count")
                            {
                                var edmCollectionValue = result as IEdmCollectionValue;
                                if (edmCollectionValue != null)
                                {
                                    result = new EdmIntegerConstant(edmCollectionValue.Elements.Count());
                                }
                                else
                                {
                                    result = null;
                                    break;
                                }
                            }
                            else if (hop.Contains(".", StringComparison.Ordinal))
                            {
                                if (this.edmModel == null)
                                {
                                    throw new InvalidOperationException(SRResources.Edm_Evaluator_TypeCastNeedsEdmModel);
                                }

                                IEdmType typeSegmentClientType = this.resolveTypeFromName(hop, this.edmModel);
                                if (typeSegmentClientType == null)
                                {
                                    result = null;
                                    break;
                                }

                                IEdmTypeReference operandType = result.Type;
                                EdmValueKind operandKind = result.ValueKind;

                                if (operandKind == EdmValueKind.Collection)
                                {
                                    List<IEdmDelayedValue> elementValues = new List<IEdmDelayedValue>();
                                    var collection = result as IEdmCollectionValue;
                                    foreach (IEdmDelayedValue element in collection.Elements)
                                    {
                                        if (element.Value.Type.Definition.IsOrInheritsFrom(typeSegmentClientType))
                                        {
                                            elementValues.Add(element);
                                        }
                                    }

                                    result = new EdmCollectionValue(
                                        new EdmCollectionTypeReference(new EdmCollectionType(typeSegmentClientType.GetTypeReference(false))),
                                        elementValues);
                                }
                                else if (operandKind != EdmValueKind.Structured
                                    || (operandKind == EdmValueKind.Structured
                                        && !operandType.Definition.IsOrInheritsFrom(typeSegmentClientType)))
                                {
                                    result = null;
                                    break;
                                }
                            }
                            else
                            {
                                result = FindProperty(hop, result);
                                if (result == null)
                                {
                                    throw new InvalidOperationException(Error.Format(SRResources.Edm_Evaluator_UnboundPath, hop));
                                }
                            }
                        }

                        return result;
                    }

                case EdmExpressionKind.PropertyPath:
                case EdmExpressionKind.NavigationPropertyPath:
                    {
                        EdmUtil.CheckArgumentNull(context, "context");

                        IEdmPathExpression pathExpression = (IEdmPathExpression)expression;
                        IEdmValue result = context;

                        // [EdmLib] Need to handle paths that bind to things other than properties.
                        foreach (string hop in pathExpression.PathSegments)
                        {
                            result = FindProperty(hop, result);

                            if (result == null)
                            {
                                throw new InvalidOperationException(Error.Format(SRResources.Edm_Evaluator_UnboundPath, hop));
                            }
                        }

                        return result;
                    }

                case EdmExpressionKind.FunctionApplication:
                    {
                        IEdmApplyExpression apply = (IEdmApplyExpression)expression;
                        IEdmFunction target = apply.AppliedFunction;
                        if (target != null)
                        {
                            IList<IEdmExpression> argumentExpressions = apply.Arguments.ToList();
                            IEdmValue[] arguments = new IEdmValue[argumentExpressions.Count];

                            {
                                int argumentIndex = 0;
                                foreach (IEdmExpression argument in argumentExpressions)
                                {
                                    arguments[argumentIndex++] = this.Eval(argument, context);
                                }
                            }

                            //// Static validation will have checked that the number and types of arguments are correct,
                            //// so those checks are not performed dynamically.

                            Func<IEdmValue[], IEdmValue> operationEvaluator;
                            if (this.builtInFunctions.TryGetValue(target, out operationEvaluator))
                            {
                                return operationEvaluator(arguments);
                            }

                            if (this.lastChanceOperationApplier != null)
                            {
                                return this.lastChanceOperationApplier(target.FullName(), arguments);
                            }
                        }

                        throw new InvalidOperationException(Error.Format(SRResources.Edm_Evaluator_UnboundFunction, target != null ? target.ToTraceString() : string.Empty));
                    }

                case EdmExpressionKind.If:
                    {
                        IEdmIfExpression ifExpression = (IEdmIfExpression)expression;

                        if (((IEdmBooleanValue)this.Eval(ifExpression.TestExpression, context)).Value)
                        {
                            return this.Eval(ifExpression.TrueExpression, context);
                        }

                        return this.Eval(ifExpression.FalseExpression, context);
                    }

                case EdmExpressionKind.IsOf:
                    {
                        IEdmIsOfExpression isOf = (IEdmIsOfExpression)expression;

                        IEdmValue operand = this.Eval(isOf.Operand, context);
                        IEdmTypeReference targetType = isOf.Type;

                        return new EdmBooleanConstant(MatchesType(targetType, operand));
                    }

                case EdmExpressionKind.Cast:
                    {
                        IEdmCastExpression castType = (IEdmCastExpression)expression;

                        IEdmValue operand = this.Eval(castType.Operand, context);
                        IEdmTypeReference targetType = castType.Type;

                        return Cast(targetType, operand);
                    }

                case EdmExpressionKind.Record:
                    {
                        IEdmRecordExpression record = (IEdmRecordExpression)expression;
                        DelayedExpressionContext recordContext = new DelayedExpressionContext(this, context);

                        List<IEdmPropertyValue> propertyValues = new List<IEdmPropertyValue>();

                        //// Static validation will have checked that the set of supplied properties are appropriate
                        //// for the supplied type and have no duplicates, so those checks are not performed dynamically.

                        foreach (IEdmPropertyConstructor propertyConstructor in record.Properties)
                        {
                            propertyValues.Add(new DelayedRecordProperty(recordContext, propertyConstructor));
                        }

                        EdmStructuredValue result = new EdmStructuredValue(record.DeclaredType != null ? record.DeclaredType.AsStructured() : null, propertyValues);
                        return result;
                    }

                case EdmExpressionKind.Collection:
                    {
                        IEdmCollectionExpression collection = (IEdmCollectionExpression)expression;
                        DelayedExpressionContext collectionContext = new DelayedExpressionContext(this, context);
                        List<IEdmDelayedValue> elementValues = new List<IEdmDelayedValue>();

                        //// Static validation will have checked that the result types of the element expressions are
                        //// appropriate and so these checks are not performed dynamically.

                        foreach (IEdmExpression element in collection.Elements)
                        {
                            elementValues.Add(this.MapLabeledExpressionToDelayedValue(element, collectionContext, context));
                        }

                        EdmCollectionValue result = new EdmCollectionValue(collection.DeclaredType != null ? collection.DeclaredType.AsCollection() : null, elementValues);
                        return result;
                    }

                case EdmExpressionKind.LabeledExpressionReference:
                    {
                        return this.MapLabeledExpressionToDelayedValue(((IEdmLabeledExpressionReferenceExpression)expression).ReferencedLabeledExpression, null, context).Value;
                    }

                case EdmExpressionKind.Labeled:
                    return this.MapLabeledExpressionToDelayedValue(expression, new DelayedExpressionContext(this, context), context).Value;

                case EdmExpressionKind.UnaryOperator:
                    return EvalUnaryOperator((IEdmUnaryOperatorExpression)expression, context);

                case EdmExpressionKind.BinaryOperator:
                    return EvalBinaryOperator((IEdmBinaryOperatorExpression)expression, context);

                case EdmExpressionKind.EnumMember:
                    IEdmEnumMemberExpression enumMemberExpression = (IEdmEnumMemberExpression)expression;
                    var enumMembers = enumMemberExpression.EnumMembers.ToList();
                    IEdmEnumType enumType = enumMembers.First().DeclaringType;
                    IEdmEnumTypeReference enumTypeReference = new EdmEnumTypeReference(enumType, false);
                    if (enumMembers.Count == 1)
                    {
                        return new EdmEnumValue(enumTypeReference, enumMemberExpression.EnumMembers.Single());
                    }
                    else
                    {
                        if (!enumType.IsFlags || !EdmEnumValueParser.IsEnumIntegerType(enumType))
                        {
                            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Type {0} cannot be assigned with multi-values.", enumType.FullName()));
                        }

                        long result = 0;
                        foreach (var enumMember in enumMembers)
                        {
                            long value = enumMember.Value.Value;
                            result |= value;
                        }

                        return new EdmEnumValue(enumTypeReference, new EdmEnumMemberValue(result));
                    }

                default:
                    throw new InvalidOperationException(Error.Format(SRResources.Edm_Evaluator_UnrecognizedExpressionKind, ((int)expression.ExpressionKind).ToString(CultureInfo.InvariantCulture)));
            }
        }

        private IEdmValue EvalUnaryOperator(IEdmUnaryOperatorExpression unaryExpression, IEdmStructuredValue context)
        {
            IEdmValue operand = this.Eval(unaryExpression.Operand, context);

            switch (unaryExpression.Kind)
            {
                case EdmUnaryOperatorKind.Negate:
                    {
                        IEdmIntegerValue intOperand = operand as IEdmIntegerValue;
                        if (intOperand != null)
                        {
                            return new EdmIntegerConstant(-intOperand.Value);
                        }

                        IEdmDecimalValue decimalOperand = operand as IEdmDecimalValue;
                        if (decimalOperand != null)
                        {
                            return new EdmDecimalConstant(-decimalOperand.Value);
                        }

                        IEdmFloatingValue floatingOperand = operand as IEdmFloatingValue;
                        if (floatingOperand != null)
                        {
                            return new EdmFloatingConstant(-floatingOperand.Value);
                        }

                        throw new InvalidOperationException(Error.Format(SRResources.Edm_Evaluator_InvalidUnaryOperand, unaryExpression.Kind, operand.ValueKind));
                    }

                case EdmUnaryOperatorKind.Not:
                    {
                        IEdmBooleanValue boolOperand = operand as IEdmBooleanValue;
                        if (boolOperand != null)
                        {
                            return new EdmBooleanConstant(!boolOperand.Value);
                        }

                        throw new InvalidOperationException(Error.Format(SRResources.Edm_Evaluator_InvalidUnaryOperand, unaryExpression.Kind, operand.ValueKind));
                    }

                default:
                    throw new InvalidOperationException(Error.Format(SRResources.Edm_Evaluator_UnrecognizedUnaryOperatorKind, ((int)unaryExpression.Kind).ToString(CultureInfo.InvariantCulture)));
            }
        }

        private IEdmValue EvalBinaryOperator(IEdmBinaryOperatorExpression binaryExpression, IEdmStructuredValue context)
        {
            IEdmValue left = this.Eval(binaryExpression.Left, context);
            IEdmValue right = this.Eval(binaryExpression.Right, context);

            if (binaryExpression.Kind == EdmBinaryOperatorKind.Or)
            {
                IEdmBooleanValue leftBool = left as IEdmBooleanValue;
                IEdmBooleanValue rightBool = right as IEdmBooleanValue;
                if (leftBool != null && rightBool != null)
                {
                    return new EdmBooleanConstant(leftBool.Value || rightBool.Value);
                }

                throw new InvalidOperationException(Error.Format(SRResources.Edm_Evaluator_InvalidBinaryOperand, binaryExpression.Kind, left.ValueKind, right.ValueKind));
            }
            else if (binaryExpression.Kind == EdmBinaryOperatorKind.And)
            {
                IEdmBooleanValue leftBool = left as IEdmBooleanValue;
                IEdmBooleanValue rightBool = right as IEdmBooleanValue;
                if (leftBool != null && rightBool != null)
                {
                    return new EdmBooleanConstant(leftBool.Value && rightBool.Value);
                }

                throw new InvalidOperationException(Error.Format(SRResources.Edm_Evaluator_InvalidBinaryOperand, binaryExpression.Kind, left.ValueKind, right.ValueKind));
            }

            return EvalArithmeticBinaryOperatorOperator(left, right, binaryExpression.Kind, context);
        }

        private static IEdmValue EvalArithmeticBinaryOperatorOperator(IEdmValue left, IEdmValue right, EdmBinaryOperatorKind kind, IEdmStructuredValue context)
        {
            if (left is IEdmIntegerValue leftInt)
            {
                if (right is IEdmIntegerValue rightInt)
                {
                    switch (kind)
                    {
                        case EdmBinaryOperatorKind.Add:
                            return new EdmIntegerConstant(leftInt.Value + rightInt.Value);
                        case EdmBinaryOperatorKind.Sub:
                            return new EdmIntegerConstant(leftInt.Value - rightInt.Value);
                        case EdmBinaryOperatorKind.Mul:
                            return new EdmIntegerConstant(leftInt.Value * rightInt.Value);
                        case EdmBinaryOperatorKind.Div:
                            return new EdmIntegerConstant(leftInt.Value / rightInt.Value);
                        case EdmBinaryOperatorKind.DivBy:
                            return new EdmFloatingConstant((double)leftInt.Value / (double)rightInt.Value);
                        case EdmBinaryOperatorKind.Mod:
                            return new EdmIntegerConstant(leftInt.Value % rightInt.Value);

                        case EdmBinaryOperatorKind.Eq:
                            return new EdmBooleanConstant(leftInt.Value == rightInt.Value);
                        case EdmBinaryOperatorKind.Ne:
                            return new EdmBooleanConstant(leftInt.Value != rightInt.Value);
                        case EdmBinaryOperatorKind.Lt:
                            return new EdmBooleanConstant(leftInt.Value < rightInt.Value);
                        case EdmBinaryOperatorKind.Gt:
                            return new EdmBooleanConstant(leftInt.Value > rightInt.Value);
                        case EdmBinaryOperatorKind.Le:
                            return new EdmBooleanConstant(leftInt.Value <= rightInt.Value);
                        case EdmBinaryOperatorKind.Ge:
                            return new EdmBooleanConstant(leftInt.Value >= rightInt.Value);
                    }
                }
                else if (right is IEdmFloatingValue rightFloat)
                {
                    switch (kind)
                    {
                        case EdmBinaryOperatorKind.Add:
                            return new EdmFloatingConstant(leftInt.Value + rightFloat.Value);
                        case EdmBinaryOperatorKind.Sub:
                            return new EdmFloatingConstant(leftInt.Value - rightFloat.Value);
                        case EdmBinaryOperatorKind.Mul:
                            return new EdmFloatingConstant(leftInt.Value * rightFloat.Value);
                        case EdmBinaryOperatorKind.Div:
                            return new EdmFloatingConstant(leftInt.Value / rightFloat.Value);
                        case EdmBinaryOperatorKind.DivBy:
                            return new EdmFloatingConstant((double)leftInt.Value / (double)rightFloat.Value);

                        case EdmBinaryOperatorKind.Eq:
                            return new EdmBooleanConstant(leftInt.Value == rightFloat.Value); // Allow comparison between int and float?
                        case EdmBinaryOperatorKind.Ne:
                            return new EdmBooleanConstant(leftInt.Value != rightFloat.Value);
                        case EdmBinaryOperatorKind.Lt:
                            return new EdmBooleanConstant(leftInt.Value < rightFloat.Value);
                        case EdmBinaryOperatorKind.Gt:
                            return new EdmBooleanConstant(leftInt.Value > rightFloat.Value);
                        case EdmBinaryOperatorKind.Le:
                            return new EdmBooleanConstant(leftInt.Value <= rightFloat.Value);
                        case EdmBinaryOperatorKind.Ge:
                            return new EdmBooleanConstant(leftInt.Value >= rightFloat.Value);
                        case EdmBinaryOperatorKind.Mod:
                            break;
                    }
                }
                else if (right is IEdmDecimalValue rightDecimal)
                {
                    switch (kind)
                    {
                        case EdmBinaryOperatorKind.Add:
                            return new EdmDecimalConstant(leftInt.Value + rightDecimal.Value);
                        case EdmBinaryOperatorKind.Sub:
                            return new EdmDecimalConstant(leftInt.Value - rightDecimal.Value);
                        case EdmBinaryOperatorKind.Mul:
                            return new EdmDecimalConstant(leftInt.Value * rightDecimal.Value);
                        case EdmBinaryOperatorKind.Div:
                            return new EdmDecimalConstant(leftInt.Value / rightDecimal.Value);
                        case EdmBinaryOperatorKind.DivBy:
                            return new EdmDecimalConstant(leftInt.Value / rightDecimal.Value);

                        case EdmBinaryOperatorKind.Eq:
                            return new EdmBooleanConstant(leftInt.Value == rightDecimal.Value); // Allow comparison between int and decimal?
                        case EdmBinaryOperatorKind.Ne:
                            return new EdmBooleanConstant(leftInt.Value != rightDecimal.Value);
                        case EdmBinaryOperatorKind.Lt:
                            return new EdmBooleanConstant(leftInt.Value < rightDecimal.Value);
                        case EdmBinaryOperatorKind.Gt:
                            return new EdmBooleanConstant(leftInt.Value > rightDecimal.Value);
                        case EdmBinaryOperatorKind.Le:
                            return new EdmBooleanConstant(leftInt.Value <= rightDecimal.Value);
                        case EdmBinaryOperatorKind.Ge:
                            return new EdmBooleanConstant(leftInt.Value >= rightDecimal.Value);

                        case EdmBinaryOperatorKind.Mod: // Not valid for decimal??
                            break;
                    }
                }
            }
            else if (left is IEdmFloatingValue leftFloat)
            {
                if (right is IEdmIntegerValue rightInt)
                {
                    switch (kind)
                    {
                        case EdmBinaryOperatorKind.Add:
                            return new EdmFloatingConstant(leftFloat.Value + rightInt.Value);
                        case EdmBinaryOperatorKind.Sub:
                            return new EdmFloatingConstant(leftFloat.Value - rightInt.Value);
                        case EdmBinaryOperatorKind.Mul:
                            return new EdmFloatingConstant(leftFloat.Value * rightInt.Value);
                        case EdmBinaryOperatorKind.Div:
                            return new EdmFloatingConstant(leftFloat.Value / rightInt.Value);
                        case EdmBinaryOperatorKind.DivBy:
                            return new EdmFloatingConstant((double)leftFloat.Value / (double)rightInt.Value);

                        case EdmBinaryOperatorKind.Eq:
                            return new EdmBooleanConstant(leftFloat.Value == rightInt.Value);
                        case EdmBinaryOperatorKind.Ne:
                            return new EdmBooleanConstant(leftFloat.Value != rightInt.Value);
                        case EdmBinaryOperatorKind.Lt:
                            return new EdmBooleanConstant(leftFloat.Value < rightInt.Value);
                        case EdmBinaryOperatorKind.Gt:
                            return new EdmBooleanConstant(leftFloat.Value > rightInt.Value);
                        case EdmBinaryOperatorKind.Le:
                            return new EdmBooleanConstant(leftFloat.Value <= rightInt.Value);
                        case EdmBinaryOperatorKind.Ge:
                            return new EdmBooleanConstant(leftFloat.Value >= rightInt.Value);

                        case EdmBinaryOperatorKind.Mod: // not valid for floating
                            break;
                    }
                }
                else if (right is IEdmFloatingValue rightFloat)
                {
                    switch (kind)
                    {
                        case EdmBinaryOperatorKind.Add:
                            return new EdmFloatingConstant(leftFloat.Value + rightFloat.Value);
                        case EdmBinaryOperatorKind.Sub:
                            return new EdmFloatingConstant(leftFloat.Value - rightFloat.Value);
                        case EdmBinaryOperatorKind.Mul:
                            return new EdmFloatingConstant(leftFloat.Value * rightFloat.Value);
                        case EdmBinaryOperatorKind.Div:
                            return new EdmFloatingConstant(leftFloat.Value / rightFloat.Value);
                        case EdmBinaryOperatorKind.DivBy:
                            return new EdmFloatingConstant((double)leftFloat.Value / (double)rightFloat.Value);

                        case EdmBinaryOperatorKind.Eq:
                            return new EdmBooleanConstant(leftFloat.Value == rightFloat.Value); // Allow comparison between float and float?
                        case EdmBinaryOperatorKind.Ne:
                            return new EdmBooleanConstant(leftFloat.Value != rightFloat.Value);
                        case EdmBinaryOperatorKind.Lt:
                            return new EdmBooleanConstant(leftFloat.Value < rightFloat.Value);
                        case EdmBinaryOperatorKind.Gt:
                            return new EdmBooleanConstant(leftFloat.Value > rightFloat.Value);
                        case EdmBinaryOperatorKind.Le:
                            return new EdmBooleanConstant(leftFloat.Value <= rightFloat.Value);
                        case EdmBinaryOperatorKind.Ge:
                            return new EdmBooleanConstant(leftFloat.Value >= rightFloat.Value);
                        case EdmBinaryOperatorKind.Mod: // Not valid for floating
                            break;
                    }
                }
                else if (right is IEdmDecimalValue rightDecimal)
                {
                    switch (kind)
                    {
                        case EdmBinaryOperatorKind.Add:
                            return new EdmDecimalConstant((decimal)leftFloat.Value + rightDecimal.Value);
                        case EdmBinaryOperatorKind.Sub:
                            return new EdmDecimalConstant((decimal)leftFloat.Value - rightDecimal.Value);
                        case EdmBinaryOperatorKind.Mul:
                            return new EdmDecimalConstant((decimal)leftFloat.Value * rightDecimal.Value);
                        case EdmBinaryOperatorKind.Div:
                            return new EdmDecimalConstant((decimal)leftFloat.Value / rightDecimal.Value);
                        case EdmBinaryOperatorKind.DivBy:
                            return new EdmDecimalConstant((decimal)leftFloat.Value / rightDecimal.Value);
                        case EdmBinaryOperatorKind.Eq:
                            return new EdmBooleanConstant((decimal)leftFloat.Value == rightDecimal.Value); // Allow comparison between double and decimal?
                        case EdmBinaryOperatorKind.Ne:
                            return new EdmBooleanConstant((decimal)leftFloat.Value != rightDecimal.Value);
                        case EdmBinaryOperatorKind.Lt:
                            return new EdmBooleanConstant((decimal)leftFloat.Value < rightDecimal.Value);
                        case EdmBinaryOperatorKind.Gt:
                            return new EdmBooleanConstant((decimal)leftFloat.Value > rightDecimal.Value);
                        case EdmBinaryOperatorKind.Le:
                            return new EdmBooleanConstant((decimal)leftFloat.Value <= rightDecimal.Value);
                        case EdmBinaryOperatorKind.Ge:
                            return new EdmBooleanConstant((decimal)leftFloat.Value >= rightDecimal.Value);
                        case EdmBinaryOperatorKind.Mod: // Not valid for decimal??
                            break;
                    }
                }
            }
            else if (left is IEdmDecimalValue leftDecimal)
            {
                if (right is IEdmIntegerValue rightInt)
                {
                    switch (kind)
                    {
                        case EdmBinaryOperatorKind.Add:
                            return new EdmDecimalConstant(leftDecimal.Value + rightInt.Value);
                        case EdmBinaryOperatorKind.Sub:
                            return new EdmDecimalConstant(leftDecimal.Value - rightInt.Value);
                        case EdmBinaryOperatorKind.Mul:
                            return new EdmDecimalConstant(leftDecimal.Value * rightInt.Value);
                        case EdmBinaryOperatorKind.Div:
                            return new EdmDecimalConstant(leftDecimal.Value / rightInt.Value);
                        case EdmBinaryOperatorKind.DivBy:
                            return new EdmDecimalConstant(leftDecimal.Value / rightInt.Value);
                        case EdmBinaryOperatorKind.Eq:
                            return new EdmBooleanConstant(leftDecimal.Value == rightInt.Value);
                        case EdmBinaryOperatorKind.Ne:
                            return new EdmBooleanConstant(leftDecimal.Value != rightInt.Value);
                        case EdmBinaryOperatorKind.Lt:
                            return new EdmBooleanConstant(leftDecimal.Value < rightInt.Value);
                        case EdmBinaryOperatorKind.Gt:
                            return new EdmBooleanConstant(leftDecimal.Value > rightInt.Value);
                        case EdmBinaryOperatorKind.Le:
                            return new EdmBooleanConstant(leftDecimal.Value <= rightInt.Value);
                        case EdmBinaryOperatorKind.Ge:
                            return new EdmBooleanConstant(leftDecimal.Value >= rightInt.Value);

                        case EdmBinaryOperatorKind.Mod: // not valid for floating
                            break;
                    }
                }
                else if (right is IEdmFloatingValue rightFloat)
                {
                    switch (kind)
                    {
                        case EdmBinaryOperatorKind.Add:
                            return new EdmDecimalConstant(leftDecimal.Value + (decimal)rightFloat.Value);
                        case EdmBinaryOperatorKind.Sub:
                            return new EdmDecimalConstant(leftDecimal.Value - (decimal)rightFloat.Value);
                        case EdmBinaryOperatorKind.Mul:
                            return new EdmDecimalConstant(leftDecimal.Value * (decimal)rightFloat.Value);
                        case EdmBinaryOperatorKind.Div:
                            return new EdmDecimalConstant(leftDecimal.Value / (decimal)rightFloat.Value);
                        case EdmBinaryOperatorKind.DivBy:
                            return new EdmDecimalConstant(leftDecimal.Value / (decimal)rightFloat.Value);
                        case EdmBinaryOperatorKind.Eq:
                            return new EdmBooleanConstant(leftDecimal.Value == (decimal)rightFloat.Value); // Allow comparison between float and float?
                        case EdmBinaryOperatorKind.Ne:
                            return new EdmBooleanConstant(leftDecimal.Value != (decimal)rightFloat.Value);
                        case EdmBinaryOperatorKind.Lt:
                            return new EdmBooleanConstant(leftDecimal.Value < (decimal)rightFloat.Value);
                        case EdmBinaryOperatorKind.Gt:
                            return new EdmBooleanConstant(leftDecimal.Value > (decimal)rightFloat.Value);
                        case EdmBinaryOperatorKind.Le:
                            return new EdmBooleanConstant(leftDecimal.Value <= (decimal)rightFloat.Value);
                        case EdmBinaryOperatorKind.Ge:
                            return new EdmBooleanConstant(leftDecimal.Value >= (decimal)rightFloat.Value);
                        case EdmBinaryOperatorKind.Mod: // Not valid for floating
                            break;
                    }
                }
                else if (right is IEdmDecimalValue rightDecimal)
                {
                    switch (kind)
                    {
                        case EdmBinaryOperatorKind.Add:
                            return new EdmDecimalConstant(leftDecimal.Value + rightDecimal.Value);
                        case EdmBinaryOperatorKind.Sub:
                            return new EdmDecimalConstant(leftDecimal.Value - rightDecimal.Value);
                        case EdmBinaryOperatorKind.Mul:
                            return new EdmDecimalConstant(leftDecimal.Value * rightDecimal.Value);
                        case EdmBinaryOperatorKind.Div:
                            return new EdmDecimalConstant(leftDecimal.Value / rightDecimal.Value);
                        case EdmBinaryOperatorKind.DivBy:
                            return new EdmDecimalConstant(leftDecimal.Value / rightDecimal.Value);
                        case EdmBinaryOperatorKind.Eq:
                            return new EdmBooleanConstant(leftDecimal.Value == rightDecimal.Value); // Allow comparison between double and decimal?
                        case EdmBinaryOperatorKind.Ne:
                            return new EdmBooleanConstant(leftDecimal.Value != rightDecimal.Value);
                        case EdmBinaryOperatorKind.Lt:
                            return new EdmBooleanConstant(leftDecimal.Value < rightDecimal.Value);
                        case EdmBinaryOperatorKind.Gt:
                            return new EdmBooleanConstant(leftDecimal.Value > rightDecimal.Value);
                        case EdmBinaryOperatorKind.Le:
                            return new EdmBooleanConstant(leftDecimal.Value <= rightDecimal.Value);
                        case EdmBinaryOperatorKind.Ge:
                            return new EdmBooleanConstant(leftDecimal.Value >= rightDecimal.Value);
                        case EdmBinaryOperatorKind.Mod: // Not valid for decimal??
                            break;
                    }
                }
            }
            else if (left is IEdmDateValue leftDate)
            {
                if (right is IEdmDateValue rightDate)
                {
                    switch (kind)
                    {
                        // The DateOnly struct supports comparison operators.
                        case EdmBinaryOperatorKind.Eq:
                            return new EdmBooleanConstant(leftDate.Value == rightDate.Value);
                        case EdmBinaryOperatorKind.Ne:
                            return new EdmBooleanConstant(leftDate.Value != rightDate.Value);
                        case EdmBinaryOperatorKind.Lt:
                            return new EdmBooleanConstant(leftDate.Value < rightDate.Value);
                        case EdmBinaryOperatorKind.Gt:
                            return new EdmBooleanConstant(leftDate.Value > rightDate.Value);
                        case EdmBinaryOperatorKind.Le:
                            return new EdmBooleanConstant(leftDate.Value <= rightDate.Value);
                        case EdmBinaryOperatorKind.Ge:
                            return new EdmBooleanConstant(leftDate.Value >= rightDate.Value);
                    }
                }
                else if (right is IEdmDurationValue rightDuration)
                {
                    switch (kind)
                    {
                        // Weird but valid operations on date and duration values - the DateOnly struct supports addition and subtraction with TimeSpan.
                        //case EdmBinaryOperatorKind.Add:
                        //    return new EdmDateConstant(leftDate.Value + rightDuration.Value);
                        //case EdmBinaryOperatorKind.Subtract:
                        //    return new EdmDateConstant(leftDate.Value - rightDuration.Value);
                        default:
                            break;
                    }
                }
                else if (right is IEdmDateTimeOffsetValue rightDateTimeOffset)
                {
                    switch (kind)
                    {
                        // Weird but valid operations on date and datetimeoffset values - the DateOnly struct supports addition and subtraction with DateTimeOffset.
                        //case EdmBinaryOperatorKind.Add:
                        //    return new EdmDateTimeOffsetConstant(leftDate.Value + rightDateTimeOffset.Value);
                        //case EdmBinaryOperatorKind.Subtract:
                        //    return new EdmDateTimeOffsetConstant(leftDate.Value - rightDateTimeOffset.Value);
                        default:
                            break;
                    }
                }
            }
            else if (left is IEdmDurationValue leftDuration)
            {
                if (right is IEdmDurationValue rightDuration)
                {
                    switch (kind)
                    {
                        // The TimeSpan struct supports comparison operators.
                        case EdmBinaryOperatorKind.Add:
                            return new EdmDurationConstant(leftDuration.Value + rightDuration.Value);
                        case EdmBinaryOperatorKind.Sub:
                            return new EdmDurationConstant(leftDuration.Value - rightDuration.Value);
                        case EdmBinaryOperatorKind.Eq:
                            return new EdmBooleanConstant(leftDuration.Value == rightDuration.Value);
                        case EdmBinaryOperatorKind.Ne:
                            return new EdmBooleanConstant(leftDuration.Value != rightDuration.Value);
                        case EdmBinaryOperatorKind.Lt:
                            return new EdmBooleanConstant(leftDuration.Value < rightDuration.Value);
                        case EdmBinaryOperatorKind.Gt:
                            return new EdmBooleanConstant(leftDuration.Value > rightDuration.Value);
                        case EdmBinaryOperatorKind.Le:
                            return new EdmBooleanConstant(leftDuration.Value <= rightDuration.Value);
                        case EdmBinaryOperatorKind.Ge:
                            return new EdmBooleanConstant(leftDuration.Value >= rightDuration.Value);
                    }
                }
            }
            else if (left is IEdmDateTimeOffsetValue leftDateTimeOffset)
            {
                if (right is IEdmDateValue rightDate)
                {
                    //switch (kind)
                    //{
                    //    case EdmBinaryOperatorKind.Add:
                    //        return new EdmDateTimeOffsetConstant(leftDateTimeOffset.Value + rightDate.Value);
                    //}
                }
                else if (right is IEdmDurationValue rightDuration)
                {
                    switch (kind)
                    {
                        case EdmBinaryOperatorKind.Add:
                            return new EdmDateTimeOffsetConstant(leftDateTimeOffset.Value + rightDuration.Value);
                        case EdmBinaryOperatorKind.Sub:
                            return new EdmDateTimeOffsetConstant(leftDateTimeOffset.Value - rightDuration.Value);
                    }
                }
                else if (right is IEdmDateTimeOffsetValue rightDateTimeOffset)
                {
                    switch (kind)
                    {
                        // The DateTimeOffset struct supports comparison operators.
                        case EdmBinaryOperatorKind.Sub:
                            return new EdmDurationConstant(leftDateTimeOffset.Value - rightDateTimeOffset.Value);
                        case EdmBinaryOperatorKind.Eq:
                            return new EdmBooleanConstant(leftDateTimeOffset.Value == rightDateTimeOffset.Value);
                        case EdmBinaryOperatorKind.Ne:
                            return new EdmBooleanConstant(leftDateTimeOffset.Value != rightDateTimeOffset.Value);
                        case EdmBinaryOperatorKind.Lt:
                            return new EdmBooleanConstant(leftDateTimeOffset.Value < rightDateTimeOffset.Value);
                        case EdmBinaryOperatorKind.Gt:
                            return new EdmBooleanConstant(leftDateTimeOffset.Value > rightDateTimeOffset.Value);
                        case EdmBinaryOperatorKind.Le:
                            return new EdmBooleanConstant(leftDateTimeOffset.Value <= rightDateTimeOffset.Value);
                        case EdmBinaryOperatorKind.Ge:
                            return new EdmBooleanConstant(leftDateTimeOffset.Value >= rightDateTimeOffset.Value);
                    }
                }
            }

            throw new InvalidOperationException(Error.Format(SRResources.Edm_Evaluator_InvalidBinaryOperand, kind, left.ValueKind, right.ValueKind));
        }

        private IEdmDelayedValue MapLabeledExpressionToDelayedValue(IEdmExpression expression, DelayedExpressionContext delayedContext, IEdmStructuredValue context)
        {
            //// Labeled expressions map to delayed values either at the point of definition or at a point of reference
            //// (evaluation of a LabeledExpressionReference that refers to the expression). All of these must map to
            //// a single delayed value (so that the value itself is evaluated only once).

            IEdmLabeledExpression labeledExpression = expression as IEdmLabeledExpression;
            if (labeledExpression == null)
            {
                //// If an expression has no label, there can be no references to it and so only the point of definition needs a mapping to a delayed value,
                //// and so there is no need to cache the delayed value. The point of definition always supplies a context.

                System.Diagnostics.Debug.Assert(delayedContext != null, "Labeled element definition failed to supply an evaluation context.");
                return new DelayedCollectionElement(delayedContext, expression);
            }

            DelayedValue expressionValue;
            if (this.labeledValues.TryGetValue(labeledExpression, out expressionValue))
            {
                return expressionValue;
            }

            expressionValue = new DelayedCollectionElement(delayedContext ?? new DelayedExpressionContext(this, context), labeledExpression.Expression);
            this.labeledValues[labeledExpression] = expressionValue;
            return expressionValue;
        }

        private static IEdmValue FindProperty(string name, IEdmValue context)
        {
            IEdmValue result = null;

            IEdmStructuredValue structuredContext = context as IEdmStructuredValue;
            if (structuredContext != null)
            {
                IEdmPropertyValue propertyValue = structuredContext.FindPropertyValue(name);
                if (propertyValue != null)
                {
                    result = propertyValue.Value;
                }
            }

            return result;
        }

        private class DelayedExpressionContext
        {
            private readonly EdmExpressionEvaluator expressionEvaluator;
            private readonly IEdmStructuredValue context;

            public DelayedExpressionContext(EdmExpressionEvaluator expressionEvaluator, IEdmStructuredValue context)
            {
                this.expressionEvaluator = expressionEvaluator;
                this.context = context;
            }

            public IEdmValue Eval(IEdmExpression expression)
            {
                return this.expressionEvaluator.Eval(expression, this.context);
            }
        }

        private abstract class DelayedValue : IEdmDelayedValue
        {
            private readonly DelayedExpressionContext context;
            private IEdmValue value;

            public DelayedValue(DelayedExpressionContext context)
            {
                this.context = context;
            }

            public abstract IEdmExpression Expression { get; }

            public IEdmValue Value
            {
                get
                {
                    if (this.value == null)
                    {
                        this.value = this.context.Eval(this.Expression);
                    }

                    return this.value;
                }
            }
        }

        private class DelayedRecordProperty : DelayedValue, IEdmPropertyValue
        {
            private readonly IEdmPropertyConstructor constructor;

            public DelayedRecordProperty(DelayedExpressionContext context, IEdmPropertyConstructor constructor)
                : base(context)
            {
                this.constructor = constructor;
            }

            public string Name
            {
                get { return this.constructor.Name; }
            }

            public override IEdmExpression Expression
            {
                get { return this.constructor.Value; }
            }
        }

        private class DelayedCollectionElement : DelayedValue
        {
            private readonly IEdmExpression expression;

            public DelayedCollectionElement(DelayedExpressionContext context, IEdmExpression expression)
                : base(context)
            {
                this.expression = expression;
            }

            public override IEdmExpression Expression
            {
                get { return this.expression; }
            }
        }

        private class CastCollectionValue : EdmElement, IEdmCollectionValue, IEnumerable<IEdmDelayedValue>
        {
            private readonly IEdmCollectionTypeReference targetCollectionType;
            private readonly IEdmCollectionValue collectionValue;

            public CastCollectionValue(IEdmCollectionTypeReference targetCollectionType, IEdmCollectionValue collectionValue)
            {
                this.targetCollectionType = targetCollectionType;
                this.collectionValue = collectionValue;
            }

            IEnumerable<IEdmDelayedValue> IEdmCollectionValue.Elements
            {
                get { return this; }
            }

            IEdmTypeReference IEdmValue.Type
            {
                get { return this.targetCollectionType; }
            }

            EdmValueKind IEdmValue.ValueKind
            {
                get { return EdmValueKind.Collection; }
            }

            IEnumerator<IEdmDelayedValue> IEnumerable<IEdmDelayedValue>.GetEnumerator()
            {
                return new CastCollectionValueEnumerator(this);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return new CastCollectionValueEnumerator(this);
            }

            private class CastCollectionValueEnumerator : IEnumerator<IEdmDelayedValue>
            {
                private readonly CastCollectionValue value;
                private readonly IEnumerator<IEdmDelayedValue> enumerator;

                public CastCollectionValueEnumerator(CastCollectionValue value)
                {
                    this.value = value;
                    this.enumerator = value.collectionValue.Elements.GetEnumerator();
                }

                public IEdmDelayedValue Current
                {
                    get { return new DelayedCast(this.value.targetCollectionType.ElementType(), this.enumerator.Current); }
                }

                object System.Collections.IEnumerator.Current
                {
                    get { return this.Current; }
                }

                bool System.Collections.IEnumerator.MoveNext()
                {
                    return this.enumerator.MoveNext();
                }

                void System.Collections.IEnumerator.Reset()
                {
                    this.enumerator.Reset();
                }

                void IDisposable.Dispose()
                {
                    this.enumerator.Dispose();
                }

                private class DelayedCast : IEdmDelayedValue
                {
                    private readonly IEdmDelayedValue delayedValue;
                    private readonly IEdmTypeReference targetType;
                    private IEdmValue value;

                    public DelayedCast(IEdmTypeReference targetType, IEdmDelayedValue value)
                    {
                        this.delayedValue = value;
                        this.targetType = targetType;
                    }

                    public IEdmValue Value
                    {
                        get
                        {
                            if (this.value == null)
                            {
                                this.value = EdmExpressionEvaluator.Cast(this.targetType, this.delayedValue.Value);
                            }

                            return this.value;
                        }
                    }
                }
            }
        }
    }
}

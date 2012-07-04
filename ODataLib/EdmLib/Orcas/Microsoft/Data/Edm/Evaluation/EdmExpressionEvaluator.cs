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

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Library.Values;
using Microsoft.Data.Edm.Validation;
using Microsoft.Data.Edm.Values;

namespace Microsoft.Data.Edm.Evaluation
{
    /// <summary>
    /// Expression evaluator.
    /// </summary>
    public class EdmExpressionEvaluator
    {
        private readonly IDictionary<IEdmFunction, Func<IEdmValue[], IEdmValue>> builtInFunctions;
        private readonly Dictionary<IEdmLabeledExpression, DelayedValue> labeledValues = new Dictionary<IEdmLabeledExpression, DelayedValue>();
        private readonly Func<string, IEdmValue[], IEdmValue> lastChanceFunctionApplier;

        /// <summary>
        /// Initializes a new instance of the EdmExpressionEvaluator class.
        /// </summary>
        /// <param name="builtInFunctions">Builtin functions dictionary to the evaluators for the functions.</param>
        public EdmExpressionEvaluator(IDictionary<IEdmFunction, Func<IEdmValue[], IEdmValue>> builtInFunctions)
        {
            this.builtInFunctions = builtInFunctions;
        }

        /// <summary>
        /// Initializes a new instance of the EdmExpressionEvaluator class.
        /// </summary>
        /// <param name="builtInFunctions">Builtin functions dictionary to the evaluators for the functions.</param>
        /// <param name="lastChanceFunctionApplier">Function to call to evaluate an application of a function with no static binding.</param>
        public EdmExpressionEvaluator(IDictionary<IEdmFunction, Func<IEdmValue[], IEdmValue>> builtInFunctions, Func<string, IEdmValue[], IEdmValue> lastChanceFunctionApplier)
            : this(builtInFunctions)
        {
            this.lastChanceFunctionApplier = lastChanceFunctionApplier;
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

            return AssertType(targetType, this.Eval(expression, context));
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
                case EdmValueKind.DateTime:
                    return targetType.IsDateTime();
                case EdmValueKind.DateTimeOffset:
                    return targetType.IsDateTimeOffset();
                case EdmValueKind.Decimal:
                    return targetType.IsDecimal();
                case EdmValueKind.Guid:
                    return targetType.IsGuid();
                case EdmValueKind.Null:
                    return targetType.IsNullable;
                case EdmValueKind.Time:
                    return targetType.IsTime();
                case EdmValueKind.String:
                    if (targetType.IsString())
                    {
                        IEdmStringTypeReference targetString = targetType.AsString();
                        return targetString.IsUnbounded || !targetString.MaxLength.HasValue || targetString.MaxLength.Value >= ((IEdmStringValue)operand).Value.Length;
                    }

                    break;
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

        private static IEdmValue AssertType(IEdmTypeReference targetType, IEdmValue operand)
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
                        return new AssertTypeCollectionValue(targetType.AsCollection(), (IEdmCollectionValue)operand);
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
                throw new InvalidOperationException(Edm.Strings.Edm_Evaluator_FailedTypeAssertion(targetType.ToTraceString()));
            }

            return operand;
        }

        private static bool AssertOrMatchStructuredType(IEdmStructuredTypeReference structuredTargetType, IEdmStructuredValue structuredValue, bool testPropertyTypes, List<IEdmPropertyValue> newProperties)
        {
            // If the value has a nominal type, the target type must be derived from the nominal type for a type match to be possible.
            IEdmTypeReference operandType = structuredValue.Type;
            if (operandType != null && operandType.TypeKind() != EdmTypeKind.Row && !structuredTargetType.StructuredDefinition().InheritsFrom(operandType.AsStructured().StructuredDefinition()))
            {
                return false;
            }

            Internal.HashSetInternal<IEdmPropertyValue> visitedProperties = new Internal.HashSetInternal<IEdmPropertyValue>();

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
                        newProperties.Add(new EdmPropertyValue(propertyValue.Name, AssertType(property.Type, propertyValue.Value)));
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
                case EdmExpressionKind.DateTimeConstant:
                    return (IEdmDateTimeConstantExpression)expression;
                case EdmExpressionKind.DateTimeOffsetConstant:
                    return (IEdmDateTimeOffsetConstantExpression)expression;
                case EdmExpressionKind.DecimalConstant:
                    return (IEdmDecimalConstantExpression)expression;
                case EdmExpressionKind.FloatingConstant:
                    return (IEdmFloatingConstantExpression)expression;
                case EdmExpressionKind.GuidConstant:
                    return (IEdmGuidConstantExpression)expression;
                case EdmExpressionKind.TimeConstant:
                    return (IEdmTimeConstantExpression)expression;
                case EdmExpressionKind.Null:
                    return (IEdmNullExpression)expression;
                case EdmExpressionKind.Path:
                    {
                        EdmUtil.CheckArgumentNull(context, "context");

                        IEdmPathExpression pathExpression = (IEdmPathExpression)expression;
                        IEdmValue result = context;

                        foreach (string hop in pathExpression.Path)
                        {
                            result = this.FindProperty(hop, result);

                            if (result == null)
                            {
                                throw new InvalidOperationException(Edm.Strings.Edm_Evaluator_UnboundPath(hop));
                            }
                        }

                        return result;
                    }

                case EdmExpressionKind.FunctionApplication:
                    {
                        IEdmApplyExpression apply = (IEdmApplyExpression)expression;
                        IEdmExpression targetReference = apply.AppliedFunction;
                        IEdmFunctionReferenceExpression targetFunctionReference = targetReference as IEdmFunctionReferenceExpression;
                        if (targetFunctionReference != null)
                        {
                            IList<IEdmExpression> argumentExpressions = apply.Arguments.ToList();
                            IEdmValue[] arguments = new IEdmValue[argumentExpressions.Count()];

                            {
                                int argumentIndex = 0;
                                foreach (IEdmExpression argument in argumentExpressions)
                                {
                                    arguments[argumentIndex++] = this.Eval(argument, context);
                                }
                            }

                            IEdmFunction target = targetFunctionReference.ReferencedFunction;
                            if (!target.IsBad())
                            {
                                //// Static validation will have checked that the number and types of arguments are correct,
                                //// so those checks are not performed dynamically.

                                Func<IEdmValue[], IEdmValue> functionEvaluator;
                                if (this.builtInFunctions.TryGetValue(target, out functionEvaluator))
                                {
                                    return functionEvaluator(arguments);
                                }
                            }

                            if (this.lastChanceFunctionApplier != null)
                            {
                                return this.lastChanceFunctionApplier(target.FullName(), arguments);
                            }
                        }

                        throw new InvalidOperationException(Edm.Strings.Edm_Evaluator_UnboundFunction(targetFunctionReference != null ? targetFunctionReference.ReferencedFunction.ToTraceString() : string.Empty));
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

                case EdmExpressionKind.IsType:
                    {
                        IEdmIsTypeExpression isType = (IEdmIsTypeExpression)expression;

                        IEdmValue operand = this.Eval(isType.Operand, context);
                        IEdmTypeReference targetType = isType.Type;

                        return new EdmBooleanConstant(MatchesType(targetType, operand));
                    }

                case EdmExpressionKind.AssertType:
                    {
                        IEdmAssertTypeExpression assertType = (IEdmAssertTypeExpression)expression;

                        IEdmValue operand = this.Eval(assertType.Operand, context);
                        IEdmTypeReference targetType = assertType.Type;

                        return AssertType(targetType, operand);
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

                case EdmExpressionKind.ParameterReference:
                case EdmExpressionKind.FunctionReference:
                case EdmExpressionKind.PropertyReference:
                case EdmExpressionKind.ValueTermReference:
                case EdmExpressionKind.EntitySetReference:
                case EdmExpressionKind.EnumMemberReference:
                    throw new InvalidOperationException("Not yet implemented: evaluation of " + expression.ExpressionKind.ToString() + " expressions.");
                default:
                    throw new InvalidOperationException(Edm.Strings.Edm_Evaluator_UnrecognizedExpressionKind(((int)expression.ExpressionKind).ToString(System.Globalization.CultureInfo.InvariantCulture)));
            }
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

        private IEdmValue FindProperty(string name, IEdmValue context)
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

        private class AssertTypeCollectionValue : Library.EdmElement, IEdmCollectionValue, IEnumerable<IEdmDelayedValue>
        {
            private readonly IEdmCollectionTypeReference targetCollectionType;
            private readonly IEdmCollectionValue collectionValue;

            public AssertTypeCollectionValue(IEdmCollectionTypeReference targetCollectionType, IEdmCollectionValue collectionValue)
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
                return new AssertTypeCollectionValueEnumerator(this);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return new AssertTypeCollectionValueEnumerator(this);
            }

            private class AssertTypeCollectionValueEnumerator : IEnumerator<IEdmDelayedValue>
            {
                private readonly AssertTypeCollectionValue value;
                private readonly IEnumerator<IEdmDelayedValue> enumerator;

                public AssertTypeCollectionValueEnumerator(AssertTypeCollectionValue value)
                {
                    this.value = value;
                    this.enumerator = value.collectionValue.Elements.GetEnumerator();
                }

                public IEdmDelayedValue Current
                {
                    get { return new DelayedAssertType(this.value.targetCollectionType.ElementType(), this.enumerator.Current); }
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

                private class DelayedAssertType : IEdmDelayedValue
                {
                    private readonly IEdmDelayedValue delayedValue;
                    private readonly IEdmTypeReference targetType;
                    private IEdmValue value;

                    public DelayedAssertType(IEdmTypeReference targetType, IEdmDelayedValue value)
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
                                this.value = EdmExpressionEvaluator.AssertType(this.targetType, this.delayedValue.Value);
                            }

                            return this.value;
                        }
                    }
                }
            }
        }
    }
}

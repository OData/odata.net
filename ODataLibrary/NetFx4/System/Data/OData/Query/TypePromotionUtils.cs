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

namespace System.Data.OData.Query
{
    #region Namespaces.
    using System.Collections.Generic;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Linq;
    #endregion Namespaces.

    /// <summary>
    /// Helper methods for promoting argument types of operators and function calls.
    /// </summary>
    internal static class TypePromotionUtils
    {
        #region Operator signatures
        //// NOTE: the order of the operator signatures matters if more than one signature matches equally
        ////       well than another. In that case the first signature in the list is used.

        /// <summary>Function signatures for logical operators (and, or).</summary>
        private static readonly FunctionSignature[] logicalSignatures = new FunctionSignature[]
        {
            new FunctionSignature(PrimitiveTypeUtils.BoolResourceType, PrimitiveTypeUtils.BoolResourceType),
            new FunctionSignature(PrimitiveTypeUtils.NullableBoolResourceType, PrimitiveTypeUtils.NullableBoolResourceType),
        };

        /// <summary>Function signatures for the 'not' operator.</summary>
        private static readonly FunctionSignature[] notSignatures = new FunctionSignature[]
        {
            new FunctionSignature(PrimitiveTypeUtils.BoolResourceType),
            new FunctionSignature(PrimitiveTypeUtils.NullableBoolResourceType),
        };

        /// <summary>Function signatures for arithmetic operators (add, sub, mul, div, mod).</summary>
        private static readonly FunctionSignature[] arithmeticSignatures = new FunctionSignature[]
        {
            new FunctionSignature(PrimitiveTypeUtils.Int32ResourceType, PrimitiveTypeUtils.Int32ResourceType),
            new FunctionSignature(PrimitiveTypeUtils.NullableInt32ResourceType, PrimitiveTypeUtils.NullableInt32ResourceType),
            new FunctionSignature(PrimitiveTypeUtils.Int64ResourceType, PrimitiveTypeUtils.Int64ResourceType),
            new FunctionSignature(PrimitiveTypeUtils.NullableInt64ResourceType, PrimitiveTypeUtils.NullableInt64ResourceType),
            new FunctionSignature(PrimitiveTypeUtils.FloatResourceType, PrimitiveTypeUtils.FloatResourceType),
            new FunctionSignature(PrimitiveTypeUtils.NullableFloatResourceType, PrimitiveTypeUtils.NullableFloatResourceType),
            new FunctionSignature(PrimitiveTypeUtils.DoubleResourceType, PrimitiveTypeUtils.DoubleResourceType),
            new FunctionSignature(PrimitiveTypeUtils.NullableDoubleResourceType, PrimitiveTypeUtils.NullableDoubleResourceType),
            new FunctionSignature(PrimitiveTypeUtils.DecimalResourceType, PrimitiveTypeUtils.DecimalResourceType),
            new FunctionSignature(PrimitiveTypeUtils.NullableDecimalResourceType, PrimitiveTypeUtils.NullableDecimalResourceType),
        };

        /// <summary>Function signatures for relational operators (eq, ne, lt, le, gt, ge).</summary>
        private static readonly FunctionSignature[] relationalSignatures = new FunctionSignature[]
        {
            new FunctionSignature(PrimitiveTypeUtils.Int32ResourceType, PrimitiveTypeUtils.Int32ResourceType),
            new FunctionSignature(PrimitiveTypeUtils.NullableInt32ResourceType, PrimitiveTypeUtils.NullableInt32ResourceType),
            new FunctionSignature(PrimitiveTypeUtils.Int64ResourceType, PrimitiveTypeUtils.Int64ResourceType),
            new FunctionSignature(PrimitiveTypeUtils.NullableInt64ResourceType, PrimitiveTypeUtils.NullableInt64ResourceType),
            new FunctionSignature(PrimitiveTypeUtils.FloatResourceType, PrimitiveTypeUtils.FloatResourceType),
            new FunctionSignature(PrimitiveTypeUtils.NullableFloatResourceType, PrimitiveTypeUtils.NullableFloatResourceType),
            new FunctionSignature(PrimitiveTypeUtils.DoubleResourceType, PrimitiveTypeUtils.DoubleResourceType),
            new FunctionSignature(PrimitiveTypeUtils.NullableDoubleResourceType, PrimitiveTypeUtils.NullableDoubleResourceType),
            new FunctionSignature(PrimitiveTypeUtils.DecimalResourceType, PrimitiveTypeUtils.DecimalResourceType),
            new FunctionSignature(PrimitiveTypeUtils.NullableDecimalResourceType, PrimitiveTypeUtils.NullableDecimalResourceType),

            new FunctionSignature(PrimitiveTypeUtils.StringResourceType, PrimitiveTypeUtils.StringResourceType),
            new FunctionSignature(PrimitiveTypeUtils.BinaryResourceType, PrimitiveTypeUtils.BinaryResourceType),
            new FunctionSignature(PrimitiveTypeUtils.BoolResourceType, PrimitiveTypeUtils.BoolResourceType),
            new FunctionSignature(PrimitiveTypeUtils.NullableBoolResourceType, PrimitiveTypeUtils.NullableBoolResourceType),
            new FunctionSignature(PrimitiveTypeUtils.GuidResourceType, PrimitiveTypeUtils.GuidResourceType),
            new FunctionSignature(PrimitiveTypeUtils.NullableGuidResourceType, PrimitiveTypeUtils.NullableGuidResourceType),
            new FunctionSignature(PrimitiveTypeUtils.DateTimeResourceType, PrimitiveTypeUtils.DateTimeResourceType),
            new FunctionSignature(PrimitiveTypeUtils.NullableDateTimeResourceType, PrimitiveTypeUtils.NullableDateTimeResourceType),
        };

        /// <summary>Function signatures for the 'negate' operator.</summary>
        private static readonly FunctionSignature[] negationSignatures = new FunctionSignature[]
        {
            new FunctionSignature(PrimitiveTypeUtils.Int32ResourceType),
            new FunctionSignature(PrimitiveTypeUtils.NullableInt32ResourceType),
            new FunctionSignature(PrimitiveTypeUtils.Int64ResourceType),
            new FunctionSignature(PrimitiveTypeUtils.NullableInt64ResourceType),
            new FunctionSignature(PrimitiveTypeUtils.FloatResourceType),
            new FunctionSignature(PrimitiveTypeUtils.NullableFloatResourceType),
            new FunctionSignature(PrimitiveTypeUtils.DoubleResourceType),
            new FunctionSignature(PrimitiveTypeUtils.NullableDoubleResourceType),
            new FunctionSignature(PrimitiveTypeUtils.DecimalResourceType),
            new FunctionSignature(PrimitiveTypeUtils.NullableDecimalResourceType),
        };
        #endregion Operator signatures

        /// <summary>
        /// Numeric type kinds.
        /// </summary>
        private enum NumericTypeKind
        {
            /// <summary>A type that is not numeric.</summary>
            NotNumeric = 0,

            /// <summary>A type that is a char, single, double or decimal.</summary>
            NotIntegral = 1,

            /// <summary>A type that is a signed integral.</summary>
            SignedIntegral = 2,

            /// <summary>A type that is an unsigned integral.</summary>
            UnsignedIntegral = 3,
        }

        /// <summary>Checks that the operands (possibly promoted) are valid for the specified operation.</summary>
        /// <param name="operatorKind">The operator kind to promote the operand types for.</param>
        /// <param name="left">Resource type of left operand.</param>
        /// <param name="right">Resource type of right operand.</param>
        /// <returns>True if all argument types could be promoted; otherwise false.</returns>
        internal static bool PromoteOperandTypes(BinaryOperatorKind operatorKind, ref ResourceType left, ref ResourceType right)
        {
            DebugUtils.CheckNoExternalCallers();

            // The resource types for the operands can be null
            // if they (a) represent the null literal or (b) represent an open type/property.
            // TODO: We currently do not support open properties/types so if both argument types are null 
            //       we have null literals on both sides and cannot promote arguments;
            //       Review when we support open properties/types.
            if (left == null && right == null)
            {
                // if we find null literals on both sides we cannot promote; the result type will also be null
                return true;
            }

            FunctionSignature[] signatures = GetFunctionSignatures(operatorKind);

            ResourceType[] argumentTypes = new ResourceType[] { left, right };
            bool success = FindBestSignature(signatures, argumentTypes) == 1;

            if (success)
            {
                left = argumentTypes[0];
                right = argumentTypes[1];

                if (left == null)
                {
                    left = right;
                }
                else if (right == null)
                {
                    right = left;
                }
            }

            return success;
        }

        /// <summary>Checks that the operands (possibly promoted) are valid for the specified operation.</summary>
        /// <param name="operatorKind">The operator kind to promote the operand types for.</param>
        /// <param name="resourceType">Resource type of the operand.</param>
        /// <returns>True if the resource type could be promoted; otherwise false.</returns>
        internal static bool PromoteOperandType(UnaryOperatorKind operatorKind, ref ResourceType resourceType)
        {
            DebugUtils.CheckNoExternalCallers();

            // The resource type for the operands can be null
            // if it (a) represents the null literal or (b) represents an open type/property.
            // TODO: We currently do not support open properties/types so if argument type is null 
            //       we have a null literal and cannot promote the argument type;
            //       Review when we support open properties/types.
            if (resourceType == null)
            {
                // if we find a null literal we cannot promote; the result type will also be null
                return true;
            }

            FunctionSignature[] signatures = GetFunctionSignatures(operatorKind);

            ResourceType[] argumentTypes = new ResourceType[] { resourceType };
            bool success = FindBestSignature(signatures, argumentTypes) == 1;

            if (success)
            {
                resourceType = argumentTypes[0];
            }

            return success;
        }

        /// <summary>Finds the best fitting function for the specified arguments.</summary>
        /// <param name="functions">Functions to consider.</param>
        /// <param name="argumentTypes">Types of the arguments for the function.</param>
        /// <returns>The best fitting function; null if none found or ambiguous.</returns>
        internal static FunctionSignature FindBestFunctionSignature(FunctionSignature[] functions, ResourceType[] argumentTypes)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(functions != null, "functions != null");
            Debug.Assert(argumentTypes != null, "argumentTypes != null");
            List<FunctionSignature> applicableFunctions = new List<FunctionSignature>(functions.Length);

            // Build a list of applicable functions (and cache their promoted arguments).
            foreach (FunctionSignature candidate in functions)
            {
                if (candidate.ArgumentTypes.Length != argumentTypes.Length)
                {
                    continue;
                }

                bool argumentsMatch = true;
                for (int i = 0; i < candidate.ArgumentTypes.Length; i++)
                {
                    if (!CanPromoteTo(argumentTypes[i], candidate.ArgumentTypes[i]))
                    {
                        argumentsMatch = false;
                        break;
                    }
                }

                if (argumentsMatch)
                {
                    applicableFunctions.Add(candidate);
                }
            }

            // Return the best applicable function.
            if (applicableFunctions.Count == 0)
            {
                // No matching function.
                return null;
            }
            else if (applicableFunctions.Count == 1)
            {
                return applicableFunctions[0];
            }
            else
            {
                // Find a single function which is better than all others.
                int bestFunctionIndex = -1;
                for (int i = 0; i < applicableFunctions.Count; i++)
                {
                    bool betterThanAllOthers = true;
                    for (int j = 0; j < applicableFunctions.Count; j++)
                    {
                        if (i != j && MatchesArgumentTypesBetterThan(argumentTypes, applicableFunctions[j].ArgumentTypes, applicableFunctions[i].ArgumentTypes))
                        {
                            betterThanAllOthers = false;
                            break;
                        }
                    }

                    if (betterThanAllOthers)
                    {
                        if (bestFunctionIndex == -1)
                        {
                            bestFunctionIndex = i;
                        }
                        else
                        {
                            // Ambiguous.
                            return null;
                        }
                    }
                }

                if (bestFunctionIndex == -1)
                {
                    return null;
                }

                return applicableFunctions[bestFunctionIndex];
            }
        }

        /// <summary>Finds the exact fitting function for the specified arguments.</summary>
        /// <param name="functions">Functions to consider.</param>
        /// <param name="argumentTypes">Types of the arguments for the function.</param>
        /// <returns>The exact fitting function; null if no exact match was found.</returns>
        internal static FunctionSignature FindExactFunctionSignature(FunctionSignature[] functions, ResourceType[] argumentTypes)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(functions != null, "functions != null");
            Debug.Assert(argumentTypes != null, "argumentTypes != null");

            for (int functionIndex = 0; functionIndex < functions.Length; functionIndex++)
            {
                FunctionSignature functionSignature = functions[functionIndex];
                bool matchFound = true;

                if (functionSignature.ArgumentTypes.Length != argumentTypes.Length)
                {
                    continue;
                }

                for (int argumentIndex = 0; argumentIndex < argumentTypes.Length; argumentIndex++)
                {
                    ResourceType functionSignatureArgumentType = functionSignature.ArgumentTypes[argumentIndex];
                    ResourceType argumentType = argumentTypes[argumentIndex];
                    Debug.Assert(functionSignatureArgumentType.ResourceTypeKind == ResourceTypeKind.Primitive, "Only primitive arguments are supported for functions.");

                    if (argumentType.ResourceTypeKind != ResourceTypeKind.Primitive)
                    {
                        matchFound = false;
                        break;
                    }

                    // Since we're working on primitive types only we can compare just references since primitive resource types are atomized.
                    if (!object.ReferenceEquals(argumentType, functionSignatureArgumentType))
                    {
                        matchFound = false;
                        break;
                    }
                }

                if (matchFound)
                {
                    return functionSignature;
                }
            }

            return null;
        }

        /// <summary>Checks whether the source type is compatible with the target type.</summary>
        /// <param name="source">Source type.</param>
        /// <param name="target">Target type.</param>
        /// <returns>true if source can be used in place of target; false otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "One method to describe all rules around converts.")]
        internal static bool CanConvertTo(Type source, Type target)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(source != null, "source != null");
            Debug.Assert(target != null, "target != null");

            //// Copy of the ResourceQueryParser.ExpressionParser.IsCompatibleWith method.

            // NOTE reference comparison (instead of IsEquivalentTo) is ok here because we use these for primitive only.
            if (source == target)
            {
                return true;
            }

            if (!target.IsValueType)
            {
                return target.IsAssignableFrom(source);
            }

            Type sourceType = Nullable.GetUnderlyingType(source) ?? source;
            Type targetType = Nullable.GetUnderlyingType(target) ?? target;

            //// This rule stops the parser from considering nullable types as incompatible
            //// with non-nullable types. We have however implemented this rule because we just
            //// access underlying rules. C# requires an explicit .Value access, and EDM has no
            //// nullablity on types and (at the model level) implements null propagation.
            ////
            //// if (sourceType != source && targetType == target)
            //// {
            ////     return false;
            //// }

            TypeCode sourceCode = sourceType.IsEnum ? TypeCode.Object : Type.GetTypeCode(sourceType);
            TypeCode targetCode = targetType.IsEnum ? TypeCode.Object : Type.GetTypeCode(targetType);
            switch (sourceCode)
            {
                case TypeCode.SByte:
                    switch (targetCode)
                    {
                        case TypeCode.SByte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Byte:
                    switch (targetCode)
                    {
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Int16:
                    switch (targetCode)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Int32:
                    switch (targetCode)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Int64:
                    switch (targetCode)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }

                    break;
                case TypeCode.Single:
                    switch (targetCode)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                            return true;
                    }

                    break;
                default:
                    if (sourceType == targetType)
                    {
                        return true;
                    }

                    break;
            }

            // NOTE: this is to deal with open types that are not yet supported.
            //
            // Anything can be converted to something that's *exactly* an object.
            if (target == typeof(object))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the correct set of function signatures for type promotion for a given binary operator.
        /// </summary>
        /// <param name="operatorKind">The operator kind to get the signatures for.</param>
        /// <returns>The set of signatures for the specified <paramref name="operatorKind"/>.</returns>
        private static FunctionSignature[] GetFunctionSignatures(BinaryOperatorKind operatorKind)
        {
            switch (operatorKind)
            {
                case BinaryOperatorKind.Or:     // fall through
                case BinaryOperatorKind.And:
                    return logicalSignatures;

                case BinaryOperatorKind.Equal:              // fall through
                case BinaryOperatorKind.NotEqual:           // fall through
                case BinaryOperatorKind.GreaterThan:        // fall through
                case BinaryOperatorKind.GreaterThanOrEqual: // fall through
                case BinaryOperatorKind.LessThan:           // fall through
                case BinaryOperatorKind.LessThanOrEqual:
                    return relationalSignatures;

                case BinaryOperatorKind.Add:                // fall through
                case BinaryOperatorKind.Subtract:           // fall through
                case BinaryOperatorKind.Multiply:           // fall through
                case BinaryOperatorKind.Divide:             // fall through
                case BinaryOperatorKind.Modulo:             // fall through
                    return arithmeticSignatures;

                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.TypePromotionUtils_GetFunctionSignatures_Binary_UnreachableCodepath));
            }
        }

        /// <summary>
        /// Gets the correct set of function signatures for type promotion for a given binary operator.
        /// </summary>
        /// <param name="operatorKind">The operator kind to get the signatures for.</param>
        /// <returns>The set of signatures for the specified <paramref name="operatorKind"/>.</returns>
        private static FunctionSignature[] GetFunctionSignatures(UnaryOperatorKind operatorKind)
        {
            switch (operatorKind)
            {
                case UnaryOperatorKind.Negate:
                    return negationSignatures;

                case UnaryOperatorKind.Not:
                    return notSignatures;
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.TypePromotionUtils_GetFunctionSignatures_Unary_UnreachableCodepath));
            }
        }

        /// <summary>Finds the best methods for the specified arguments given a candidate method enumeration.</summary>
        /// <param name="signatures">The candidate function signatures.</param>
        /// <param name="argumentTypes">The argument types to match.</param>
        /// <returns>The number of "best match" methods.</returns>
        private static int FindBestSignature(FunctionSignature[] signatures, ResourceType[] argumentTypes)
        {
            Debug.Assert(signatures != null, "signatures != null");
            Debug.Assert(argumentTypes != null, "argumentTypes != null");
            Debug.Assert(argumentTypes.All(t => t == null || t.ResourceTypeKind == ResourceTypeKind.Primitive), "All argument types must be primitive or null.");
            Debug.Assert(signatures.All(s => s.ArgumentTypes != null && s.ArgumentTypes.All(t => t.ResourceTypeKind == ResourceTypeKind.Primitive)), "All signatures must have only primitive argument types.");

            List<FunctionSignature> applicableSignatures = signatures.Where(signature => IsApplicable(signature, argumentTypes)).ToList();
            if (applicableSignatures.Count > 1)
            {
                applicableSignatures = FindBestApplicableSignatures(applicableSignatures, argumentTypes);
            }

            int result = applicableSignatures.Count;
            if (result == 1)
            {
                // TODO: deal with the situation that we started off with all non-open types
                //       and end up with all open types; see RequestQueryParser.FindBestMethod. 
                //       Ignored for now since we don't support open types yet.
                FunctionSignature signature = applicableSignatures[0];
                for (int i = 0; i < argumentTypes.Length; i++)
                {
                    argumentTypes[i] = signature.ArgumentTypes[i];
                }

                result = 1;
            }
            else if (result > 1)
            {
                // We may have the case for operators (which C# doesn't) in which we have a nullable operand
                // and a non-nullable operand. We choose to convert the one non-null operand to nullable in that
                // case (the binary expression will lift to null).
                if (argumentTypes.Length == 2 && result == 2 &&
                    TypeUtils.GetNonNullableType(applicableSignatures[0].ArgumentTypes[0].InstanceType) ==
                    TypeUtils.GetNonNullableType(applicableSignatures[1].ArgumentTypes[0].InstanceType))
                {
                    FunctionSignature nullableMethod =
                        TypeUtils.TypeAllowsNull(applicableSignatures[0].ArgumentTypes[0].InstanceType) ?
                        applicableSignatures[0] :
                        applicableSignatures[1];
                    argumentTypes[0] = nullableMethod.ArgumentTypes[0];
                    argumentTypes[1] = nullableMethod.ArgumentTypes[1];

                    // TODO: why is this necessary? We keep it here for now since the product has it but assert
                    //       that nothing new was found.
                    int signatureCount = FindBestSignature(signatures, argumentTypes);
                    Debug.Assert(signatureCount == 1, "signatureCount == 1");
                    Debug.Assert(argumentTypes[0] == nullableMethod.ArgumentTypes[0], "argumentTypes[0] == nullableMethod.ArgumentTypes[0]");
                    Debug.Assert(argumentTypes[1] == nullableMethod.ArgumentTypes[1], "argumentTypes[1] == nullableMethod.ArgumentTypes[1]");
                    return signatureCount;
                }
            }

            return result;
        }

        /// <summary>Checks whether the specified method is applicable given the argument expressions.</summary>
        /// <param name="signature">The candidate function signature to check.</param>
        /// <param name="argumentTypes">The argument types to match.</param>
        /// <returns>An applicable function signature if all argument types can be promoted; 'null' otherwise.</returns>
        private static bool IsApplicable(FunctionSignature signature, ResourceType[] argumentTypes)
        {
            Debug.Assert(signature != null, "signature != null");
            Debug.Assert(argumentTypes != null, "argumentTypes != null");

            if (signature.ArgumentTypes.Length != argumentTypes.Length)
            {
                return false;
            }

            for (int i = 0; i < argumentTypes.Length; ++i)
            {
                if (!CanPromoteTo(argumentTypes[i], signature.ArgumentTypes[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Promotes the specified expression to the given type if necessary.</summary>
        /// <param name="sourceType">The actual argument type.</param>
        /// <param name="targetType">The required type to promote to.</param>
        /// <returns>True if the <paramref name="sourceType"/> could be promoted; otherwise false.</returns>
        private static bool CanPromoteTo(ResourceType sourceType, ResourceType targetType)
        {
            Debug.Assert(targetType != null, "targetType != null");
            Debug.Assert(sourceType == null || sourceType.ResourceTypeKind == ResourceTypeKind.Primitive, "Type promotion only supported for primitive types.");
            Debug.Assert(targetType.ResourceTypeKind == ResourceTypeKind.Primitive, "Type promotion only supported for primitive types.");

            // Using refernence equality here is ok since both types have to primitive (assert above)
            // (or the source type is null)
            if (object.ReferenceEquals(sourceType, targetType))
            {
                return true;
            }

            if (sourceType == null)
            {
                // This indicates that a null literal or an open type has been specified.
                // For null literals we can promote to the required target type if it is nullable
                // TODO: review this once open types are supported.
                return TypeUtils.TypeAllowsNull(targetType.InstanceType);
            }

            if (CanConvertTo(sourceType.InstanceType, targetType.InstanceType))
            {
                return true;
            }

            // Allow promotion from nullable to non-nullable by directly accessing underlying value.
            if (TypeUtils.IsNullableType(sourceType.InstanceType) && targetType.InstanceType.IsValueType)
            {
                Type underlyingType = TypeUtils.GetNonNullableType(sourceType.InstanceType);
                if (CanConvertTo(underlyingType, targetType.InstanceType))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Finds the best applicable methods from the specified array that match the arguments.</summary>
        /// <param name="signatures">The candidate function signatures.</param>
        /// <param name="argumentTypes">The argument types to match.</param>
        /// <returns>Best applicable methods.</returns>
        private static List<FunctionSignature> FindBestApplicableSignatures(List<FunctionSignature> signatures, ResourceType[] argumentTypes)
        {
            Debug.Assert(signatures != null, "signatures != null");

            List<FunctionSignature> result = new List<FunctionSignature>();
            foreach (FunctionSignature method in signatures)
            {
                bool betterThanAllOthers = true;
                foreach (FunctionSignature otherMethod in signatures)
                {
                    if (otherMethod != method && MatchesArgumentTypesBetterThan(argumentTypes, otherMethod.ArgumentTypes, method.ArgumentTypes))
                    {
                        betterThanAllOthers = false;
                        break;
                    }
                }

                if (betterThanAllOthers)
                {
                    result.Add(method);
                }
            }

            return result;
        }

        /// <summary>
        /// Checks whether the <paramref name="firstCandidate"/> type list has better argument matching against the <paramref name="argumentTypes"/>
        /// than the <paramref name="secondCandidate"/> type list.
        /// </summary>
        /// <param name="argumentTypes">Actual arguments types.</param>
        /// <param name="firstCandidate">First type list to check.</param>
        /// <param name="secondCandidate">Second type list to check.</param>
        /// <returns>
        /// True if <paramref name="firstCandidate"/> has better parameter matching than <paramref name="secondCandidate"/>; otherwise false.
        /// </returns>
        private static bool MatchesArgumentTypesBetterThan(ResourceType[] argumentTypes, ResourceType[] firstCandidate, ResourceType[] secondCandidate)
        {
            Debug.Assert(argumentTypes != null, "argumentTypes != null");
            Debug.Assert(firstCandidate != null, "firstCandidate != null");
            Debug.Assert(argumentTypes.Length == firstCandidate.Length, "argumentTypes.Length == firstCandidate.Length");
            Debug.Assert(secondCandidate != null, "secondCandidate != null");
            Debug.Assert(argumentTypes.Length == secondCandidate.Length, "argumentTypes.Length == secondCandidate.Length");

            bool better = false;

            for (int i = 0; i < argumentTypes.Length; ++i)
            {
                if (argumentTypes[i] == null)
                {
                    // we don't support typed nulls; instead we have no argument type for null literals.
                    // since null literals can be converted to any type don't include them in the comparison
                    // TODO: revisit this once we support open types (where me might have null types for open properties as well)
                    continue;
                }

                int c = CompareConversions(argumentTypes[i].InstanceType, firstCandidate[i].InstanceType, secondCandidate[i].InstanceType);
                if (c < 0)
                {
                    return false;
                }
                else if (c > 0)
                {
                    better = true;
                }
            }

            return better;
        }

        /// <summary>Checks which conversion is better.</summary>
        /// <param name="source">Source type.</param>
        /// <param name="targetA">First candidate type to convert to.</param>
        /// <param name="targetB">Second candidate type to convert to.</param>
        /// <returns>
        /// Return 1 if s -> t1 is a better conversion than s -> t2
        /// Return -1 if s -> t2 is a better conversion than s -> t1
        /// Return 0 if neither conversion is better
        /// </returns>
        private static int CompareConversions(Type source, Type targetA, Type targetB)
        {
            // If both types are exactly the same, there is no preference.
            if (targetA == targetB)
            {
                return 0;
            }

            // Look for exact matches.
            if (source == targetA)
            {
                return 1;
            }
            else if (source == targetB)
            {
                return -1;
            }

            // If one is compatible and the other is not, choose the compatible type.
            bool compatibleT1AndT2 = CanConvertTo(targetA, targetB);
            bool compatibleT2AndT1 = CanConvertTo(targetB, targetA);
            if (compatibleT1AndT2 && !compatibleT2AndT1)
            {
                return 1;
            }
            else if (compatibleT2AndT1 && !compatibleT1AndT2)
            {
                return -1;
            }

            // Prefer to keep the original nullability.
            bool sourceNullable = TypeUtils.IsNullableType(source);
            bool typeNullableA = TypeUtils.IsNullableType(targetA);
            bool typeNullableB = TypeUtils.IsNullableType(targetB);
            if (sourceNullable == typeNullableA && sourceNullable != typeNullableB)
            {
                return 1;
            }
            else if (sourceNullable != typeNullableA && sourceNullable == typeNullableB)
            {
                return -1;
            }

            // Prefer signed to unsigned.
            if (IsSignedIntegralType(targetA) && IsUnsignedIntegralType(targetB))
            {
                return 1;
            }
            else if (IsSignedIntegralType(targetB) && IsUnsignedIntegralType(targetA))
            {
                return -1;
            }

            // If both decimal and double are possible or if both decimal and single are possible, then prefer decimal
            // since double and single should only be targets for single and double source if decimal is available.
            // And since neither single not double convert to decimal, we don't need to handle that case here.
            if (IsDecimalType(targetA) && IsDoubleOrSingle(targetB))
            {
                return 1;
            }
            else if (IsDecimalType(targetB) && IsDoubleOrSingle(targetA))
            {
                return -1;
            }

            // Prefer non-object to object.
            // TODO: this is how the product treats open properties (as object-typed). Left here for now
            //       but likely to have to change when we start supporting open properties (since our open
            //       properties are not object-typed).
            if (targetA != typeof(object) && targetB == typeof(object))
            {
                return 1;
            }
            else if (targetB != typeof(object) && targetA == typeof(object))
            {
                return -1;
            }

            return 0;
        }

        /// <summary>Checks whether the specified type is a signed integral type.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>true if <paramref name="type"/> is a signed integral type; false otherwise.</returns>
        private static bool IsSignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == NumericTypeKind.SignedIntegral;
        }

        /// <summary>Checks whether the specified type is an unsigned integral type.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>true if <paramref name="type"/> is an unsigned integral type; false otherwise.</returns>
        private static bool IsUnsignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == NumericTypeKind.UnsignedIntegral;
        }

        /// <summary>Checks if the specified type is a decimal or nullable decimal type.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>true if <paramref name="type"/> is either decimal or nullable decimal type; false otherwise.</returns>
        private static bool IsDecimalType(Type type)
        {
            return Type.GetTypeCode(TypeUtils.GetNonNullableType(type)) == TypeCode.Decimal;
        }

        /// <summary>Checks if the specified type is either double or single or the nullable variants.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>true if <paramref name="type"/> is double, single or nullable double or single; false otherwise.</returns>
        private static bool IsDoubleOrSingle(Type type)
        {
            TypeCode typeCode = Type.GetTypeCode(TypeUtils.GetNonNullableType(type));
            return (typeCode == TypeCode.Double || typeCode == TypeCode.Single);
        }

        /// <summary>Gets a flag for the numeric kind of type.</summary>
        /// <param name="type">Type to get numeric kind for.</param>
        /// <returns>The <see cref="NumericTypeKind"/> of the <paramref name="type"/> argument.</returns>
        private static NumericTypeKind GetNumericTypeKind(Type type)
        {
            type = TypeUtils.GetNonNullableType(type);
            Debug.Assert(!type.IsEnum, "!type.IsEnum");
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return NumericTypeKind.NotIntegral;
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return NumericTypeKind.SignedIntegral;
                case TypeCode.Byte:
                    return NumericTypeKind.UnsignedIntegral;
                default:
                    return NumericTypeKind.NotNumeric;
            }
        }
    }
}

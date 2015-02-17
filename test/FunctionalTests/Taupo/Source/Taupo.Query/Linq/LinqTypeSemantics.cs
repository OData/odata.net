//---------------------------------------------------------------------
// <copyright file="LinqTypeSemantics.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Handles type semantics for Linq types.
    /// </summary>
    public static class LinqTypeSemantics
    {
        private static Dictionary<Type, ICollection<Type>> numericPromotabilityTable = new Dictionary<Type, ICollection<Type>>();
        private static Dictionary<Type, Dictionary<Type, Type>> commonTypes = new Dictionary<Type, Dictionary<Type, Type>>();
        private static Dictionary<Type, bool> integralTypes = new Dictionary<Type, bool>();

        /// <summary>
        /// Initializes static members of the LinqTypeSemantics class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Not possible to initialize inline.")]
        static LinqTypeSemantics()
        {
            // implicit numeric type conversions in C#
            // taken from ECMA-334: 13.1.2 Implicit numeric conversions
            AddPromotability(typeof(sbyte), typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal));
            AddPromotability(typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal));
            AddPromotability(typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal));
            AddPromotability(typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal));
            AddPromotability(typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal));
            AddPromotability(typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal));
            AddPromotability(typeof(long), typeof(float), typeof(double), typeof(decimal));
            AddPromotability(typeof(ulong), typeof(float), typeof(double), typeof(decimal));
            AddPromotability(typeof(char), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal));
            AddPromotability(typeof(float), typeof(double));
            AddPromotability(typeof(double));
            AddPromotability(typeof(decimal));

            PrecomputeCommonTypes();

            RegisterIntegralTypes();
        }

        /// <summary>
        /// Gets the common type to which both types can be promoted.
        /// </summary>
        /// <param name="leftType">First type.</param>
        /// <param name="rightType">Second type.</param>
        /// <returns>Common type to which both types can be promoted. Throws if unable to find a common type.</returns>
        public static Type GetCommonType(Type leftType, Type rightType)
        {
            Type commonType;

            if (!TryGetCommonType(leftType, rightType, out commonType))
            {
                throw new TaupoInvalidOperationException("Cannot determine common type for " + leftType + " and " + rightType);
            }

            return commonType;
        }

        /// <summary>
        /// Try gets the common type to which both types can be promoted.
        /// </summary>
        /// <param name="leftType">First type.</param>
        /// <param name="rightType">Second type.</param>
        /// <param name="commonType">outputs the common type</param>
        /// <returns>true if common type can be found, false otherwise</returns>
        public static bool TryGetCommonType(Type leftType, Type rightType, out Type commonType)
        {
            ExceptionUtilities.CheckArgumentNotNull(leftType, "leftType");
            ExceptionUtilities.CheckArgumentNotNull(rightType, "rightType");

            Type leftBaseType = Nullable.GetUnderlyingType(leftType) ?? leftType;
            Type rightBaseType = Nullable.GetUnderlyingType(rightType) ?? rightType;

            Dictionary<Type, Type> dict;
            commonType = null;
            
            if (!commonTypes.TryGetValue(leftBaseType, out dict))
            {
                if (leftBaseType != rightBaseType)
                {
                    return false;
                }
                else
                {
                    commonType = leftBaseType;
                }
            }
            else
            {
                if (!dict.TryGetValue(rightBaseType, out commonType))
                {
                    return false;
                }
            }

            // if any argument is nullable, the result is nullable
            if (leftBaseType != leftType || rightBaseType != rightType)
            {
                commonType = typeof(Nullable<>).MakeGenericType(commonType);
            }

            return true;
        }

        /// <summary>
        /// Determines whether one type is promotable to another.
        /// </summary>
        /// <param name="fromType">Type from which the promotion should occur.</param>
        /// <param name="toType">Type to which the promotion should ocur.</param>
        /// <returns>True if types are promotable, false otherwise.</returns>
        public static bool IsPromotable(Type fromType, Type toType)
        {
            ExceptionUtilities.CheckArgumentNotNull(fromType, "fromType");
            ExceptionUtilities.CheckArgumentNotNull(toType, "toType");

            if (fromType != toType)
            {
                Type baseFromType = Nullable.GetUnderlyingType(fromType) ?? fromType;
                Type baseToType = Nullable.GetUnderlyingType(toType) ?? toType;

                ICollection<Type> promotableTo;

                if (!numericPromotabilityTable.TryGetValue(baseFromType, out promotableTo))
                {
                    return false;
                }

                if (!promotableTo.Contains(baseToType))
                {
                    return false;
                }
            }

            bool fromNullable = Nullable.GetUnderlyingType(fromType) != null;
            bool toNullable = Nullable.GetUnderlyingType(toType) != null;

            if (fromNullable && !toNullable)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether one type is integral
        /// </summary>
        /// <param name="type">the type to be checked</param>
        /// <returns>true if it's integral, false otherwise</returns>
        public static bool IsIntegralType(Type type)
        {
            Type realType = Nullable.GetUnderlyingType(type) ?? type;
            return integralTypes.ContainsKey(realType);
        }

        /// <summary>
        /// Determines whether one type is numeric
        /// </summary>
        /// <param name="type">the type to be checked</param>
        /// <returns>true if it's numeric, false otherwise</returns>
        public static bool IsNumeric(Type type)
        {
            Type realType = Nullable.GetUnderlyingType(type) ?? type;
            return numericPromotabilityTable.ContainsKey(realType);
        }

        private static void PrecomputeCommonTypes()
        {
            Type[] numericTypes = numericPromotabilityTable.Keys.ToArray();
            foreach (Type t1 in numericTypes)
            {
                foreach (Type t2 in numericTypes)
                {
                    Type commonType;

                    if (ComputeCommonType(t1, t2, out commonType))
                    {
                        AddCommonType(t1, t2, commonType);
                        AddCommonType(t2, t1, commonType);
                    }
                }
            }
        }

        private static bool ComputeCommonType(Type t1, Type t2, out Type commonType)
        {
            ICollection<Type> promotableTypes1 = numericPromotabilityTable[t1];
            ICollection<Type> promotableTypes2 = numericPromotabilityTable[t2];

            Type[] possibleTypes = 
            { 
               typeof(int), 
               typeof(uint), 
               typeof(long), 
               typeof(ulong), 
               typeof(decimal), 
               typeof(float),
               typeof(double)
            };

            foreach (Type candidate in possibleTypes)
            {
                if (promotableTypes1.Contains(candidate) && promotableTypes2.Contains(candidate))
                {
                    // exception: the result is decimal only if one of the arguments is decimal
                    if (candidate == typeof(decimal) && t1 != typeof(decimal) && t2 != typeof(decimal))
                    {
                        continue;
                    }

                    // exception: the result is float only if one of the arguments is float
                    if (candidate == typeof(float) && t1 != typeof(float) && t2 != typeof(float))
                    {
                        continue;
                    }

                    // exception: the result is double only if one of the arguments is double
                    if (candidate == typeof(double) && t1 != typeof(double) && t2 != typeof(double))
                    {
                        continue;
                    }

                    commonType = candidate;
                    return true;
                }
            }

            commonType = null;
            return false;
        }

        private static void AddPromotability(Type fromType, params Type[] toTypes)
        {
            var promotableTo = new List<Type>();
            promotableTo.Add(fromType);
            promotableTo.AddRange(toTypes);

            numericPromotabilityTable.Add(fromType, promotableTo);
        }

        private static void AddCommonType(Type leftType, Type rightType, Type commonType)
        {
            Dictionary<Type, Type> dict;

            if (!commonTypes.TryGetValue(leftType, out dict))
            {
                dict = new Dictionary<Type, Type>();
                commonTypes.Add(leftType, dict);
            }

            dict[rightType] = commonType;
        }

        private static void RegisterIntegralTypes()
        {
            foreach (Type t in new Type[] { typeof(sbyte), typeof(byte), typeof(char), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong) })
            {
                integralTypes[t] = true;
            }
        }
    }
}

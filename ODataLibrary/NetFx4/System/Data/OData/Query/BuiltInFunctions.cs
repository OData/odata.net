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
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    #endregion Namespaces.

    /// <summary>
    /// Class containing definitions of all the built-in functions.
    /// </summary>
    internal static class BuiltInFunctions
    {
        /// <summary>
        /// Method info for the string.Contains method.
        /// </summary>
        private static readonly MethodInfo StringContainsMethodInfo = typeof(string).GetMethod(
                "Contains", 
                BindingFlags.Public | BindingFlags.Instance, 
                null, 
                new Type[] { typeof(string) }, 
                null);

        /// <summary>
        /// Dictionary of the name of the built-in function and all the signatures.
        /// </summary>
        private static Dictionary<string, BuiltInFunctionSignature[]> builtInFunctions;

        /// <summary>
        /// Returns a list of signatures for a function name.
        /// </summary>
        /// <param name="name">The name of the function to look for.</param>
        /// <param name="signatures">The list of signatures available for the function name.</param>
        /// <returns>true if the function was found, or false otherwise.</returns>
        internal static bool TryGetBuiltInFunction(string name, out BuiltInFunctionSignature[] signatures)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(name != null, "name != null");

            InitializeBuiltInFunctions();
            return builtInFunctions.TryGetValue(name, out signatures);
        }

        /// <summary>Builds a description of a list of function signatures.</summary>
        /// <param name="name">Function name.</param>
        /// <param name="signatures">Function signatures.</param>
        /// <returns>A string with ';'-separated list of function signatures.</returns>
        internal static string BuildFunctionSignatureListDescription(string name, IEnumerable<BuiltInFunctionSignature> signatures)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(name != null, "name != null");
            Debug.Assert(signatures != null, "signatures != null");

            StringBuilder builder = new StringBuilder();
            string descriptionSeparator = "";
            foreach (BuiltInFunctionSignature signature in signatures)
            {
                builder.Append(descriptionSeparator);
                descriptionSeparator = "; ";

                string parameterSeparator = "";
                builder.Append(name);
                builder.Append('(');
                foreach (ResourceType type in signature.ArgumentTypes)
                {
                    builder.Append(parameterSeparator);
                    parameterSeparator = ", ";

                    if (type.ResourceTypeKind == ResourceTypeKind.Primitive && TypeUtils.IsNullableType(type.InstanceType))
                    {
                        builder.Append(type.FullName);
                        builder.Append(" Nullable=true");
                    }
                    else
                    {
                        builder.Append(type.FullName);
                    }
                }

                builder.Append(')');
            }

            return builder.ToString();
        }

        /// <summary>
        /// Builds the list of all built-in functions.
        /// </summary>
        private static void InitializeBuiltInFunctions()
        {
            if (builtInFunctions == null)
            {
                builtInFunctions = new Dictionary<string, BuiltInFunctionSignature[]>(StringComparer.Ordinal);
                CreateStringFunctions();
                CreateDateTimeFunctions();
                CreateMathFunctions();
            }
        }

        /// <summary>
        /// Creates all string functions.
        /// </summary>
        private static void CreateStringFunctions()
        {
            BuiltInFunctionSignature signature;
            BuiltInFunctionSignature[] signatures;
            
            // bool endswith(string, string)
            signature = BuiltInFunctionSignature.CreateFromInstanceMethodCall(
                "EndsWith",
                PrimitiveTypeUtils.BoolResourceType,
                PrimitiveTypeUtils.StringResourceType,
                PrimitiveTypeUtils.StringResourceType);
            builtInFunctions.Add("endswith", new BuiltInFunctionSignature[] { signature });

            // int indexof(string, string)
            signature = BuiltInFunctionSignature.CreateFromInstanceMethodCall(
                "IndexOf",
                PrimitiveTypeUtils.Int32ResourceType,
                PrimitiveTypeUtils.StringResourceType,
                PrimitiveTypeUtils.StringResourceType);
            builtInFunctions.Add("indexof", new BuiltInFunctionSignature[] { signature });

            // string replace(string, string, string)
            signature = BuiltInFunctionSignature.CreateFromInstanceMethodCall(
                "Replace",
                PrimitiveTypeUtils.StringResourceType,
                PrimitiveTypeUtils.StringResourceType,
                PrimitiveTypeUtils.StringResourceType,
                PrimitiveTypeUtils.StringResourceType);
            builtInFunctions.Add("replace", new BuiltInFunctionSignature[] { signature });

            // bool startswith(string, string)
            signature = BuiltInFunctionSignature.CreateFromInstanceMethodCall(
                "StartsWith",
                PrimitiveTypeUtils.BoolResourceType,
                PrimitiveTypeUtils.StringResourceType,
                PrimitiveTypeUtils.StringResourceType);
            builtInFunctions.Add("startswith", new BuiltInFunctionSignature[] { signature });

            // string tolower(string)
            signature = BuiltInFunctionSignature.CreateFromInstanceMethodCall(
                "ToLower",
                PrimitiveTypeUtils.StringResourceType,
                PrimitiveTypeUtils.StringResourceType);
            builtInFunctions.Add("tolower", new BuiltInFunctionSignature[] { signature });

            // string toupper(string)
            signature = BuiltInFunctionSignature.CreateFromInstanceMethodCall(
                "ToUpper",
                PrimitiveTypeUtils.StringResourceType,
                PrimitiveTypeUtils.StringResourceType);
            builtInFunctions.Add("toupper", new BuiltInFunctionSignature[] { signature });

            // string trim(string)
            signature = BuiltInFunctionSignature.CreateFromInstanceMethodCall(
                "Trim",
                PrimitiveTypeUtils.StringResourceType,
                PrimitiveTypeUtils.StringResourceType);
            builtInFunctions.Add("trim", new BuiltInFunctionSignature[] { signature });

            signatures = new BuiltInFunctionSignature[] 
            {
                // string substring(string, int)
                BuiltInFunctionSignature.CreateFromInstanceMethodCall(
                    "Substring",
                    PrimitiveTypeUtils.StringResourceType,
                    PrimitiveTypeUtils.StringResourceType,
                    PrimitiveTypeUtils.Int32ResourceType),

                // string substring(string, int?)
                BuiltInFunctionSignature.CreateFromInstanceMethodCall(
                    "Substring",
                    PrimitiveTypeUtils.StringResourceType,
                    PrimitiveTypeUtils.StringResourceType,
                    PrimitiveTypeUtils.NullableInt32ResourceType),

                // string substring(string, int, int)
                BuiltInFunctionSignature.CreateFromInstanceMethodCall(
                    "Substring",
                    PrimitiveTypeUtils.StringResourceType,
                    PrimitiveTypeUtils.StringResourceType,
                    PrimitiveTypeUtils.Int32ResourceType,
                    PrimitiveTypeUtils.Int32ResourceType),

                // string substring(string, int?, int)
                BuiltInFunctionSignature.CreateFromInstanceMethodCall(
                    "Substring",
                    PrimitiveTypeUtils.StringResourceType,
                    PrimitiveTypeUtils.StringResourceType,
                    PrimitiveTypeUtils.NullableInt32ResourceType,
                    PrimitiveTypeUtils.Int32ResourceType),

                // string substring(string, int, int?)
                BuiltInFunctionSignature.CreateFromInstanceMethodCall(
                    "Substring",
                    PrimitiveTypeUtils.StringResourceType,
                    PrimitiveTypeUtils.StringResourceType,
                    PrimitiveTypeUtils.Int32ResourceType,
                    PrimitiveTypeUtils.NullableInt32ResourceType),

                // string substring(string, int?, int?)
                BuiltInFunctionSignature.CreateFromInstanceMethodCall(
                    "Substring",
                    PrimitiveTypeUtils.StringResourceType,
                    PrimitiveTypeUtils.StringResourceType,
                    PrimitiveTypeUtils.NullableInt32ResourceType,
                    PrimitiveTypeUtils.NullableInt32ResourceType)
            };
            builtInFunctions.Add("substring", signatures);

            // bool substringof(string, string)
            signature = new BuiltInFunctionSignature(
                BuildSubstringOfExpression,
                PrimitiveTypeUtils.BoolResourceType,
                PrimitiveTypeUtils.StringResourceType,
                PrimitiveTypeUtils.StringResourceType);
            builtInFunctions.Add("substringof", new BuiltInFunctionSignature[] { signature });

            // string concat(string, string)
            signature = BuiltInFunctionSignature.CreateFromStaticMethodCall(
                typeof(string),
                "Concat",
                PrimitiveTypeUtils.StringResourceType,
                PrimitiveTypeUtils.StringResourceType,
                PrimitiveTypeUtils.StringResourceType);
            builtInFunctions.Add("concat", new BuiltInFunctionSignature[] { signature });

            // int length(string)
            signature = BuiltInFunctionSignature.CreateFromPropertyAccess(
                "Length",
                PrimitiveTypeUtils.Int32ResourceType,
                PrimitiveTypeUtils.StringResourceType);
            builtInFunctions.Add("length", new BuiltInFunctionSignature[] { signature });
        }

        /// <summary>
        /// Builds an expression for the substringof function.
        /// </summary>
        /// <param name="argumentExpressions">The argument expressions for the function call.</param>
        /// <returns>The expression which evaluates to the substringof result.</returns>
        private static Expression BuildSubstringOfExpression(Expression[] argumentExpressions)
        {
            Debug.Assert(argumentExpressions != null, "argumentExpressions != null");
            Debug.Assert(argumentExpressions.Length == 2, "substringof should take exactly two arguments.");
            Debug.Assert(StringContainsMethodInfo != null, "method != null -- otherwise couldn't find string.Contains(string)");
            return Expression.Call(argumentExpressions[1], StringContainsMethodInfo, argumentExpressions[0]);
        }

        /// <summary>
        /// Creates all date time functions.
        /// </summary>
        private static void CreateDateTimeFunctions()
        {
            // int year(DateTime)
            // int year(DateTime?)
            builtInFunctions.Add("year", CreateDateTimeFunctionSignatureArray("Year"));

            // int month(DateTime)
            // int month(DateTime?)
            builtInFunctions.Add("month", CreateDateTimeFunctionSignatureArray("Month"));

            // int day(DateTime)
            // int day(DateTime?)
            builtInFunctions.Add("day", CreateDateTimeFunctionSignatureArray("Day"));

            // int hour(DateTime)
            // int hour(DateTime?)
            builtInFunctions.Add("hour", CreateDateTimeFunctionSignatureArray("Hour"));

            // int minute(DateTime)
            // int minute(DateTime?)
            builtInFunctions.Add("minute", CreateDateTimeFunctionSignatureArray("Minute"));

            // int second(DateTime)
            // int second(DateTime?)
            builtInFunctions.Add("second", CreateDateTimeFunctionSignatureArray("Second"));
        }

        /// <summary>
        /// Builds an array of signatures for date time functions.
        /// </summary>
        /// <param name="propertyName">The name of the property on DateTime to access.</param>
        /// <returns>The array of signatures for a date time functions.</returns>
        private static BuiltInFunctionSignature[] CreateDateTimeFunctionSignatureArray(string propertyName)
        {
            BuiltInFunctionSignature signatureNonNullable = BuiltInFunctionSignature.CreateFromPropertyAccess(
                propertyName,
                PrimitiveTypeUtils.Int32ResourceType,
                PrimitiveTypeUtils.DateTimeResourceType);

            BuiltInFunctionSignature signatureNullable = BuiltInFunctionSignature.CreateFromPropertyAccess(
                propertyName,
                PrimitiveTypeUtils.Int32ResourceType,
                PrimitiveTypeUtils.NullableDateTimeResourceType);

            return new BuiltInFunctionSignature[] { signatureNonNullable, signatureNullable };
        }

        /// <summary>
        /// Creates all math functions.
        /// </summary>
        private static void CreateMathFunctions()
        {
            // double round(double)
            // decimal round(decimal)
            // double round(double?)
            // decimal round(decimal?)
            builtInFunctions.Add("round", CreateMathFunctionSignatureArray("Round"));

            // double floor(double)
            // decimal floor(decimal)
            // double floor(double?)
            // decimal floor(decimal?)
            builtInFunctions.Add("floor", CreateMathFunctionSignatureArray("Floor"));

            // double ceiling(double)
            // decimal ceiling(decimal)
            // double ceiling(double?)
            // decimal ceiling(decimal?)
            builtInFunctions.Add("ceiling", CreateMathFunctionSignatureArray("Ceiling"));
        }

        /// <summary>
        /// Builds an array of signatures for math functions.
        /// </summary>
        /// <param name="methodName">The name of the method to call on the Math class.</param>
        /// <returns>The array of signatures for math functions.</returns>
        private static BuiltInFunctionSignature[] CreateMathFunctionSignatureArray(string methodName)
        {
            BuiltInFunctionSignature doubleSignature = BuiltInFunctionSignature.CreateFromStaticMethodCall(
                typeof(Math),
                methodName,
                PrimitiveTypeUtils.DoubleResourceType,
                PrimitiveTypeUtils.DoubleResourceType);
            BuiltInFunctionSignature nullableDoubleSignature = BuiltInFunctionSignature.CreateFromStaticMethodCall(
                typeof(Math),
                methodName,
                PrimitiveTypeUtils.DoubleResourceType,
                PrimitiveTypeUtils.NullableDoubleResourceType);
            BuiltInFunctionSignature decimalSignature = BuiltInFunctionSignature.CreateFromStaticMethodCall(
                typeof(Math),
                methodName,
                PrimitiveTypeUtils.DecimalResourceType,
                PrimitiveTypeUtils.DecimalResourceType);
            BuiltInFunctionSignature nullableDecimalSignature = BuiltInFunctionSignature.CreateFromStaticMethodCall(
                typeof(Math),
                methodName,
                PrimitiveTypeUtils.DecimalResourceType,
                PrimitiveTypeUtils.NullableDecimalResourceType);

            return new BuiltInFunctionSignature[] { doubleSignature, decimalSignature, nullableDoubleSignature, nullableDecimalSignature };
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="CombinatorialEngineExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.CombinatorialEngine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Extension methods for combinatorial engines
    /// </summary>
    public static class CombinatorialEngineExtensionMethods
    {
        /// <summary>
        /// Runs combinations using specified engine provider.
        /// </summary>
        /// <typeparam name="T">Type of an item in the dimension.</typeparam>
        /// <param name="provider">The combinatorial engine provider to use.</param>
        /// <param name="dimension">Dimension to get combinations for.</param>
        /// <param name="action">The action to run for all combinations.</param>
        /// <param name="skipTo">If specified determines which combination to skip to. Must be null when running outside the debugger.</param>
        public static void RunCombinations<T>(
            this ICombinatorialEngineProvider provider,
            IEnumerable<T> dimension,
            Action<T> action,
            int? skipTo = null)
        {
            provider.RunCombinations(action, skipTo, dimension);
        }

        /// <summary>
        /// Runs combinations using specified engine provider.
        /// </summary>
        /// <typeparam name="T1">Type of an item in the dimension.</typeparam>
        /// <typeparam name="T2">Type of an item in the dimension.</typeparam>
        /// <param name="provider">The combinatorial engine provider to use.</param>
        /// <param name="dimension1">Dimension to get combinations for.</param>
        /// <param name="dimension2">Dimension to get combinations for.</param>
        /// <param name="action">The action to run for all combinations.</param>
        /// <param name="skipTo">If specified determines which combination to skip to. Must be null when running outside the debugger.</param>
        public static void RunCombinations<T1, T2>(
            this ICombinatorialEngineProvider provider,
            IEnumerable<T1> dimension1,
            IEnumerable<T2> dimension2,
            Action<T1, T2> action,
            int? skipTo = null)
        {
            provider.RunCombinations(action, skipTo, dimension1, dimension2);
        }

        /// <summary>
        /// Runs combinations using specified engine provider.
        /// </summary>
        /// <typeparam name="T1">Type of an item in the dimension.</typeparam>
        /// <typeparam name="T2">Type of an item in the dimension.</typeparam>
        /// <typeparam name="T3">Type of an item in the dimension.</typeparam>
        /// <param name="provider">The combinatorial engine provider to use.</param>
        /// <param name="dimension1">Dimension to get combinations for.</param>
        /// <param name="dimension2">Dimension to get combinations for.</param>
        /// <param name="dimension3">Dimension to get combinations for.</param>
        /// <param name="action">The action to run for all combinations.</param>
        /// <param name="skipTo">If specified determines which combination to skip to. Must be null when running outside the debugger.</param>
        public static void RunCombinations<T1, T2, T3>(
            this ICombinatorialEngineProvider provider,
            IEnumerable<T1> dimension1,
            IEnumerable<T2> dimension2,
            IEnumerable<T3> dimension3,
            Action<T1, T2, T3> action,
            int? skipTo = null)
        {
            provider.RunCombinations(action, skipTo, dimension1, dimension2, dimension3);
        }

        /// <summary>
        /// Runs combinations using specified engine provider.
        /// </summary>
        /// <typeparam name="T1">Type of an item in the dimension.</typeparam>
        /// <typeparam name="T2">Type of an item in the dimension.</typeparam>
        /// <typeparam name="T3">Type of an item in the dimension.</typeparam>
        /// <typeparam name="T4">Type of an item in the dimension.</typeparam>
        /// <param name="provider">The combinatorial engine provider to use.</param>
        /// <param name="dimension1">Dimension to get combinations for.</param>
        /// <param name="dimension2">Dimension to get combinations for.</param>
        /// <param name="dimension3">Dimension to get combinations for.</param>
        /// <param name="dimension4">Dimension to get combinations for.</param>
        /// <param name="action">The action to run for all combinations.</param>
        /// <param name="skipTo">If specified determines which combination to skip to. Must be null when running outside the debugger.</param>
        public static void RunCombinations<T1, T2, T3, T4>(
            this ICombinatorialEngineProvider provider,
            IEnumerable<T1> dimension1,
            IEnumerable<T2> dimension2,
            IEnumerable<T3> dimension3,
            IEnumerable<T4> dimension4,
            Action<T1, T2, T3, T4> action,
            int? skipTo = null)
        {
            provider.RunCombinations(action, skipTo, dimension1, dimension2, dimension3, dimension4);
        }

        /// <summary>
        /// Runs combinations using specified engine provider.
        /// </summary>
        /// <typeparam name="T1">Type of an item in the dimension.</typeparam>
        /// <typeparam name="T2">Type of an item in the dimension.</typeparam>
        /// <typeparam name="T3">Type of an item in the dimension.</typeparam>
        /// <typeparam name="T4">Type of an item in the dimension.</typeparam>
        /// <typeparam name="T5">Type of an item in the dimension.</typeparam>
        /// <param name="provider">The combinatorial engine provider to use.</param>
        /// <param name="dimension1">Dimension to get combinations for.</param>
        /// <param name="dimension2">Dimension to get combinations for.</param>
        /// <param name="dimension3">Dimension to get combinations for.</param>
        /// <param name="dimension4">Dimension to get combinations for.</param>
        /// <param name="dimension5">Dimension to get combinations for.</param>
        /// <param name="action">The action to run for all combinations.</param>
        /// <param name="skipTo">If specified determines which combination to skip to. Must be null when running outside the debugger.</param>
        public static void RunCombinations<T1, T2, T3, T4, T5>(
            this ICombinatorialEngineProvider provider,
            IEnumerable<T1> dimension1,
            IEnumerable<T2> dimension2,
            IEnumerable<T3> dimension3,
            IEnumerable<T4> dimension4,
            IEnumerable<T5> dimension5,
            Action<T1, T2, T3, T4, T5> action,
            int? skipTo = null)
        {
            provider.RunCombinations(action, skipTo, dimension1, dimension2, dimension3, dimension4, dimension5);
        }

        /// <summary>
        /// Runs combinations using specified engine provider.
        /// </summary>
        /// <param name="provider">The engine provider to use to create the combinatorial engine.</param>
        /// <param name="action">The action to run for each combination.</param>
        /// <param name="skipTo">If specified determines which combination to skip to. Must be null when running outside the debugger.</param>
        /// <param name="dimensions">Array of enumerations which represent the dimensions, these are matched by order to the parameters of the action.</param>
        public static void RunCombinations(this ICombinatorialEngineProvider provider, Delegate action, int? skipTo, params IEnumerable[] dimensions)
        {
            ParameterInfo[] parameters = action.Method.GetParameters();
            CombinatorialDimension[] dimensionInstances = dimensions.Select((dimensionEnumerable, dimensionIndex) =>
                new CombinatorialDimension(parameters[dimensionIndex].Name, dimensionEnumerable)).ToArray();

            ICombinatorialEngine engine = provider.CreateEngine(dimensionInstances);
            engine.RunCombinations((dimensionValues) =>
            {
                action.DynamicInvoke(parameters.Select(parameter => dimensionValues[parameter.Name]).ToArray());
            }, skipTo);
        }

        /// <summary>
        /// Runs the specified action for all combinations of the specified combinatorial engine.
        /// </summary>
        /// <param name="engine">The engine to use.</param>
        /// <param name="action">Action to call for each combination.</param>
        /// <param name="skipTo">If specified determines which combination to skip to. Must be null when running outside the debugger.</param>
        public static void RunCombinations(this ICombinatorialEngine engine, Action<Dictionary<string, object>> action, int? skipTo = null)
        {
            if (skipTo.HasValue && !Debugger.IsAttached)
            {
                throw new ArgumentException("The skipTo parameter was used in RunCombinations and the code is not running under debugger. " +
                    "The skipTo is only intended for usage during debugging and MUST NOT be present in checked-in code.");
            }

            int combinationNumber = 0;
            while (engine.NextCombination())
            {
                combinationNumber++;
                if (skipTo.HasValue && skipTo > combinationNumber)
                {
                    continue;
                }

                try
                {
                    engine.LogCombinationNumber(combinationNumber);
                    action(engine.CurrentDimensionValues);
                }
                catch (Exception e)
                {
                    string message = string.Format(
                        "Failure in combination #{1} for dimension values:{0}{2}{0}",
                        Environment.NewLine,
                        combinationNumber.ToString(),
                        engine.DescribeDimensions());
                    throw new Exception(message, e);
                }
            }
        }

        /// <summary>
        /// Writes the current dimensions and their values of the specified engine to a string for logging purposes.
        /// </summary>
        /// <param name="engine">The combinatorial engine to write values for.</param>
        /// <returns>String representation suitable for logging.</returns>
        public static string DescribeDimensions(this ICombinatorialEngine engine)
        {
            if (engine.CurrentDimensionValues == null)
            {
                return "[none]";
            }

            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (var dimension in engine.Dimensions)
            {
                if (!first)
                {
                    sb.AppendLine();
                }

                first = false;
                sb.Append(dimension.Name);
                sb.Append(": [");
                object dimensionValue = engine.CurrentDimensionValues[dimension.Name];
                if (dimensionValue == null)
                {
                    dimensionValue = "*null*";
                }
                else if (dimensionValue is string[])
                {
                    dimensionValue = "string[]:" + string.Join(",", (string[])dimensionValue);
                }
                sb.Append(dimensionValue);
                sb.Append("]");
            }

            return sb.ToString();
        }
    }
}

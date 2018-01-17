//---------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;
    using System.Globalization;
    using System.IO;

    /// <summary>
    ///   Extension methods for TextWriter
    /// </summary>
    internal static class ExtensionMethods
    {
        /// <summary>
        ///   Write a double to a TextWriter ensuring that the value will be roundtrippable thorugh double.parse
        /// </summary>
        /// <param name = "writer">the writer</param>
        /// <param name = "d">the double value to be written</param>
        public static void WriteRoundtrippable(this TextWriter writer, double d)
        {
            writer.Write(d.ToString("R", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// If the arg is non-null, evaluate the op. Otherwise, propogate the null.
        /// </summary>
        /// <typeparam name="TArg">The type of the arg.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="arg">The arg.</param>
        /// <param name="op">The op.</param>
        /// <returns>op(arg) if arg is non-null; null if arg is null.</returns>
        internal static TResult? IfValidReturningNullable<TArg, TResult>(this TArg arg, Func<TArg, TResult> op)
            where TArg : class
            where TResult : struct
        {
            return arg == null ? (TResult?)null : op(arg);
        }

        /// <summary>
        /// If the arg is non-null, evaluate the op. Otherwise, propogate the null.
        /// </summary>
        /// <typeparam name="TArg">The type of the arg.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="arg">The arg.</param>
        /// <param name="op">The op.</param>
        /// <returns>op(arg) if arg is non-null; null if arg is null.</returns>
        internal static TResult IfValid<TArg, TResult>(this TArg arg, Func<TArg, TResult> op)
            where TArg : class
            where TResult : class
        {
            return arg == null ? default(TResult) : op(arg);
        }
    }
}
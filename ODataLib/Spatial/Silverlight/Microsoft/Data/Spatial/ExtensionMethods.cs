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

namespace Microsoft.Data.Spatial
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

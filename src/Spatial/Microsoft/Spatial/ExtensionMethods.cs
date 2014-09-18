//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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

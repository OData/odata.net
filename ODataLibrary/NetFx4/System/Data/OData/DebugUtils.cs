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

namespace System.Data.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    #endregion Namespaces

    /// <summary>
    /// Class with utility methods to help with debug only code.
    /// </summary>
    internal static class DebugUtils
    {
#if DEBUG && !SILVERLIGHT && !WINDOWS_PHONE
        /// <summary>
        /// The assembly this code resides in.
        /// </summary>
        private static readonly Assembly ThisAssembly = typeof(DebugUtils).Assembly;

        /// <summary>
        /// The assembly which reflection resides in, so that we can skip over private reflection callstack.
        /// </summary>
        private static readonly Assembly ReflectionAssembly = typeof(RuntimeMethodHandle).Assembly;

        /// <summary>
        /// The assembly which LINQ resides in, so that we can skip over it to allow for usage of LINQ in our internal calling code.
        /// </summary>
        private static readonly Assembly LinqAssembly = typeof(Enumerable).Assembly;

        /// <summary>
        /// Private knob to turn of the external calls check - used by unit tests.
        /// </summary>
        private static bool turnOffNoExternalCallersCheck = false;
#endif

        /// <summary>
        /// Checks that the method which called this helper method
        /// was not called by any code outside of the System.Data.OData.dll.
        /// </summary>
        /// <remarks>
        /// The method is only called when in DEBUG builds, since it's quite expensive and we expect
        /// all our external callers to run DEBUG builds at least sometimes.
        /// The method is also called when compiling for FxCop analysis, as we run our official FxCop
        /// pass on RET build and we need to check that all internal methods call this one to check for accessibility
        /// even though only in debug builds.
        /// </remarks>
#if !CODE_ANALYSIS
        [Conditional("DEBUG")]
#endif
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0015:CheckNonInternalCallersRule", Justification = "The method can't call itself, would cause endless recursion.")]
        internal static void CheckNoExternalCallers()
        {
#if DEBUG && !SILVERLIGHT && !WINDOWS_PHONE
            if (turnOffNoExternalCallersCheck)
            {
                return;
            }

            // Skip this method in the callstack since it's not really interesting.
            StackTrace stackTrace = new StackTrace(1);
            StackFrame callerFrame = null;
            Assembly methodAssembly = null;

            // Try to skip over private reflection and LINQ and see the true caller
            for (int i = 1; i < stackTrace.FrameCount; i++)
            {
                callerFrame = stackTrace.GetFrame(i);
                methodAssembly = callerFrame.GetMethod().Module.Assembly;
                if (methodAssembly != ReflectionAssembly && methodAssembly != LinqAssembly)
                {
                    break;
                }
            }

            // Validate that the caller is this assembly
            if (methodAssembly != ThisAssembly)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Internal method of '{0}' was called, this is not allowed.{1}Use only public methods.{1}Callstack:{1}{2}",
                    ThisAssembly.FullName,
                    Environment.NewLine,
                    stackTrace.ToString());
                Debug.Assert(false, message);
            }
#endif
        }
    }
}

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

#if !INTERNAL_DROP 

#if ODATALIB_QUERY
namespace Microsoft.Data.Experimental.OData
#else
namespace Microsoft.Data.OData
#endif
{
    #region Namespaces
#if !CODE_ANALYSIS
    using System.Diagnostics;
#endif
    using System.Diagnostics.CodeAnalysis;
    #endregion Namespaces

    /// <summary>
    /// Class with utility methods to help with debug only code.
    /// </summary>
    internal static class DebugUtils
    {
#if DEBUG && !SILVERLIGHT && !WINDOWS_PHONE && INTERNAL_DROP && !ASTORIA_LIGHT
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
        /// was not called by any code outside of the Microsoft.Data.OData.dll.
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
        [SuppressMessage("DataWeb.Usage", "AC0016:CheckNonInternalCallersRule", Justification = "The method can't call itself, would cause endless recursion.")]
        internal static void CheckNoExternalCallers()
        {
#if DEBUG && !SILVERLIGHT && !WINDOWS_PHONE && INTERNAL_DROP && !ASTORIA_LIGHT
            CheckNoExternalCallersImplementation(false);
#endif
        }

#if !ASTORIA_LIGHT
        /// <summary>
        /// Checks that the method which called this helper method
        /// was not called by any code outside of the Microsoft.Data.OData.dll.
        /// </summary>
        /// <param name="checkPublicMethods">Set to true if this check is called from a public method which should also be checked for non-external callers.
        /// In that case, make sure that the calling method will not get inlined!</param>
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
        [SuppressMessage("DataWeb.Usage", "AC0016:CheckNonInternalCallersRule", Justification = "The method can't call itself, would cause endless recursion.")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "checkPublicMethods", Justification = "Intended to only be used in debug builds.")]
        internal static void CheckNoExternalCallers(bool checkPublicMethods)
        {
#if DEBUG && !SILVERLIGHT && !WINDOWS_PHONE && INTERNAL_DROP && !ASTORIA_LIGHT
            CheckNoExternalCallersImplementation(checkPublicMethods);
#endif
        }
#endif

#if DEBUG && !SILVERLIGHT && !WINDOWS_PHONE && INTERNAL_DROP && !ASTORIA_LIGHT
        /// <summary>
        /// Checks that the method which called this helper method
        /// was not called by any code outside of the Microsoft.Data.OData.dll.
        /// </summary>
        /// <param name="checkPublicMethods">Set to true if this check is called from a public method which should also be checked for non-external callers.
        /// In that case, make sure that the calling method will not get inlined!</param>
        /// <remarks>
        /// The method is only called when in DEBUG builds, since it's quite expensive and we expect
        /// all our external callers to run DEBUG builds at least sometimes.
        /// The method is also called when compiling for FxCop analysis, as we run our official FxCop
        /// pass on RET build and we need to check that all internal methods call this one to check for accessibility
        /// even though only in debug builds.
        /// </remarks>
        private static void CheckNoExternalCallersImplementation(bool checkPublicMethods)
        {
            if (turnOffNoExternalCallersCheck)
            {
                return;
            }

            // Skip this method in the callstack since it's not really interesting.
            StackTrace stackTrace = new StackTrace(1);
            StackFrame callerFrame = null;
            Assembly methodAssembly = null;

            int methodIndex = 0;

            // Skip over the CheckNoExternalCallers method as well since it's the internal head for this method.
            // (Note that these methods get inlined quite often, so that's why we need a name check).
            MethodBase firstMethod = stackTrace.GetFrame(methodIndex).GetMethod();
            if (firstMethod.Name == "CheckNoExternalCallers")
            {
                methodIndex++;
            }

            // The first method on the stack should be the one which needs to be protected.
            // And thus if it's not from our assembly or it's not internal, allow the call anyway.
            // That can happen if the method which needs to be protected was inlined in which case the caller
            // of the check can be anything, and we should not check.
            if (!checkPublicMethods)
            {
                firstMethod = stackTrace.GetFrame(methodIndex).GetMethod();
                if (firstMethod.Module.Assembly != ThisAssembly || !firstMethod.IsAssembly)
                {
                    return;
                }
            }

            methodIndex++;

            // Try to skip over private reflection and LINQ and see the true caller
            for (int i = methodIndex; i < stackTrace.FrameCount; i++)
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
        }
#endif
    }
}

#endif

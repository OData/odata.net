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

#if ASTORIA_CLIENT
namespace Microsoft.OData.Client
#else
namespace Microsoft.OData.Service
#endif
{
    using System;
    using System.Threading;
    
    /// <summary>
    /// Common defintions and functions for ALL product assemblies
    /// </summary>
    internal static partial class CommonUtil
    {
        // Only StackOverflowException & ThreadAbortException are sealed classes.

        /// <summary>Type of OutOfMemoryException.</summary>
        private static readonly Type OutOfMemoryType = typeof(OutOfMemoryException);

#if !PORTABLELIB
        /// <summary>Type of StackOverflowException.</summary>
        private static readonly Type StackOverflowType = typeof(StackOverflowException);

        /// <summary>Type of ThreadAbortException.</summary>
        private static readonly Type ThreadAbortType = typeof(ThreadAbortException);
#endif

        /// <summary>
        /// Determines whether the specified exception can be caught and 
        /// handled, or whether it should be allowed to continue unwinding.
        /// </summary>
        /// <param name="e"><see cref="Exception"/> to test.</param>
        /// <returns>
        /// true if the specified exception can be caught and handled; 
        /// false otherwise.
        /// </returns>
        internal static bool IsCatchableExceptionType(Exception e)
        {
            if (e == null)
            {
                return true;
            }

            // a 'catchable' exception is defined by what it is not.
            Type type = e.GetType();
            return (
#if !PORTABLELIB
                    (type != ThreadAbortType) &&
                    (type != StackOverflowType) &&
#endif
                    (type != OutOfMemoryType));
        }
    }
}

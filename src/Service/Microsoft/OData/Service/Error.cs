//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service
{
    using System;

    /// <summary>
    /// Strongly-typed and parameterized exception factory.
    /// </summary>
    internal static partial class Error
    {
        /// <summary>
        /// create and trace a HttpHeaderFailure
        /// </summary>
        /// <param name="errorCode">error code</param>
        /// <param name="message">error message</param>
        /// <returns>DataServiceException</returns>
        internal static DataServiceException HttpHeaderFailure(int errorCode, string message)
        {
            return Trace(new DataServiceException(errorCode, message));
        }

        /// <summary>
        /// Trace the exception
        /// </summary>
        /// <typeparam name="T">type of the exception</typeparam>
        /// <param name="exception">exception object to trace</param>
        /// <returns>the exception parameter</returns>
        private static T Trace<T>(T exception) where T : Exception
        {
            return exception;
        }
    }
}

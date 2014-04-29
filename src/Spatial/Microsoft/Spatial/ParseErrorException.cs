//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.Spatial
{
    using System;
#if ORCAS
    using System.Runtime.Serialization;
#endif

    /// <summary>The exception that is thrown on an unsuccessful parsing of the serialized format.</summary>
#if ORCAS
    [Serializable]
#endif
    public class ParseErrorException : Exception
    {
        /// <summary>Creates a new instance of the <see cref="T:Microsoft.Spatial.ParseErrorException" /> class.</summary>
        public ParseErrorException()
        {
        }

        /// <summary>Creates a new instance of the <see cref="T:Microsoft.Spatial.ParseErrorException" /> class from a message and previous exception.</summary>
        /// <param name="message">The message about the exception.</param>
        /// <param name="innerException">The exception that preceeded this one.</param>
        public ParseErrorException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        /// <summary>Creates a new instance of the <see cref="T:Microsoft.Spatial.ParseErrorException" /> class from a message.</summary>
        /// <param name="message">The message about the exception.</param>
        public ParseErrorException(string message) 
            : base(message)
        {
        }

#if ORCAS
        /// <summary>Creates a new instance of the <see cref="T:Microsoft.Spatial.ParseErrorException" /> class from a serialized data.</summary>
        /// <param name="info">The instance that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The instance that contains contextual information about the source or destination.</param>
        protected ParseErrorException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
#endif
    }
}

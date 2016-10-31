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

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData;
    #endregion Namespaces

    /// <summary>
    /// Class representing a function signature using EDM types.
    /// </summary>
    internal class FunctionSignature
    {
        /// <summary>The argument types for this function signature.</summary>
        private readonly IEdmTypeReference[] argumentTypes;

        /// <summary>
        /// Constructor taking all the argument types.
        /// </summary>
        /// <param name="argumentTypes">The argument types for this function signature.</param>
        internal FunctionSignature(params IEdmTypeReference[] argumentTypes)
        {
            DebugUtils.CheckNoExternalCallers();

            this.argumentTypes = argumentTypes;
        }

        /// <summary>
        /// The argument types for this function signature.
        /// </summary>
        internal IEdmTypeReference[] ArgumentTypes 
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.argumentTypes;
            }
        }
    }

    /// <summary>
    /// Class representing a function signature using EDM types.
    /// </summary>
    internal sealed class FunctionSignatureWithReturnType : FunctionSignature
    {
        /// <summary>
        /// The return type of this function signature.
        /// </summary>
        private readonly IEdmTypeReference returnType;

        /// <summary>
        /// Constructor taking all the argument types.
        /// </summary>
        /// <param name="returnType">The return type of this function signature.</param>
        /// <param name="argumentTypes">The argument types for this function signature.</param>
        internal FunctionSignatureWithReturnType(IEdmTypeReference returnType, params IEdmTypeReference[] argumentTypes)
            : base(argumentTypes)
        {
            DebugUtils.CheckNoExternalCallers();

            this.returnType = returnType;
        }

        /// <summary>
        /// The return type of this function signature.
        /// </summary>
        internal IEdmTypeReference ReturnType 
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.returnType;
            }
        }
    }
}

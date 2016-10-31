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

namespace Microsoft.Data.Edm
{
    /// <summary>
    /// Enumerates the modes of parameters of EDM functions.
    /// </summary>
    public enum EdmFunctionParameterMode
    {
        /// <summary>
        /// Denotes that a parameter with an unknown or error directionality.
        /// </summary>
        None,

        /// <summary>
        /// Denotes that a parameter is used for input.
        /// </summary>
        In,

        /// <summary>
        /// Denotes that a parameter is used for output.
        /// </summary>
        Out,

        /// <summary>
        /// Denotes that a parameter is used for input and output.
        /// </summary>
        InOut,
    }

    /// <summary>
    /// Represents a parameter of an EDM function.
    /// </summary>
    public interface IEdmFunctionParameter : IEdmNamedElement, IEdmVocabularyAnnotatable
    {
        /// <summary>
        /// Gets the type of this function parameter.
        /// </summary>
        IEdmTypeReference Type { get; }

        /// <summary>
        /// Gets the function or function import that declared this parameter.
        /// </summary>
        IEdmFunctionBase DeclaringFunction { get; }

        /// <summary>
        /// Gets the mode of this function parameter.
        /// </summary>
        EdmFunctionParameterMode Mode { get; }
    }
}

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

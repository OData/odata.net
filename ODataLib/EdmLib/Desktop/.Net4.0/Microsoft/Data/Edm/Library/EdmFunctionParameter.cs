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

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents an EDM function parameter.
    /// </summary>
    public class EdmFunctionParameter : EdmNamedElement, IEdmFunctionParameter
    {
        private readonly IEdmTypeReference type;
        private readonly EdmFunctionParameterMode mode;
        private readonly IEdmFunctionBase declaringFunction;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmFunctionParameter"/> class.
        /// </summary>
        /// <param name="declaringFunction">Declaring function of the parameter.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="type">Type of the parameter.</param>
        public EdmFunctionParameter(IEdmFunctionBase declaringFunction, string name, IEdmTypeReference type)
            : this(declaringFunction, name, type, EdmFunctionParameterMode.In)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmFunctionParameter"/> class.
        /// </summary>
        /// <param name="declaringFunction">Declaring function of the parameter.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="type">Type of the parameter.</param>
        /// <param name="mode">Mode of the parameter.</param>
        public EdmFunctionParameter(IEdmFunctionBase declaringFunction, string name, IEdmTypeReference type, EdmFunctionParameterMode mode)
            : base(name)
        {
            EdmUtil.CheckArgumentNull(declaringFunction, "declaringFunction");
            EdmUtil.CheckArgumentNull(name, "name");
            EdmUtil.CheckArgumentNull(type, "type");

            this.type = type;
            this.mode = mode;
            this.declaringFunction = declaringFunction;
        }

        /// <summary>
        /// Gets the type of this parameter.
        /// </summary>
        public IEdmTypeReference Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the function or function import that declared this parameter.
        /// </summary>
        public IEdmFunctionBase DeclaringFunction
        {
            get { return this.declaringFunction; }
        }

        /// <summary>
        /// Gets the mode of this parameter.
        /// </summary>
        public EdmFunctionParameterMode Mode
        {
            get { return this.mode; }
        }
    }
}

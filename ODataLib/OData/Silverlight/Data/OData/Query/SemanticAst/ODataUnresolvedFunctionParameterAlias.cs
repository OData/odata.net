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

namespace Microsoft.Data.OData.Query.SemanticAst
{
    using Microsoft.Data.Edm;

    /// <summary>
    /// Represents an aliased parameter in a function call that has not yet been resolved to a specific value.
    /// </summary>
    public class ODataUnresolvedFunctionParameterAlias : ODataValue
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ODataUnresolvedFunctionParameterAlias"/>.
        /// </summary>
        /// <param name="alias">The alias provided as the parameter value.</param>
        /// <param name="type">The EDM type of the parameter represented by this alias.</param>
        public ODataUnresolvedFunctionParameterAlias(string alias, IEdmTypeReference type) 
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(alias, "alias");
            this.Alias = alias;
            this.Type = type;
        }

        /// <summary>
        /// The EDM type of the parameter represented by this alias.
        /// </summary>
        public IEdmTypeReference Type { get; private set; }

        /// <summary>
        /// The alias provided as the parameter value.
        /// </summary>
        public string Alias { get; private set; }
    }
}

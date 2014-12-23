//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Core.UriParser.Semantic
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides syntactic information of the paramenter aliases in query uri, and caches the semantics information of the aliases' values.
    /// </summary>
    internal sealed class ParameterAliasValueAccessor
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameterAliasValueExpressions">The parameter alias value expressions from uri.</param>
        public ParameterAliasValueAccessor(IDictionary<string, string> parameterAliasValueExpressions)
        {
            ExceptionUtils.CheckArgumentNotNull(parameterAliasValueExpressions, "parameterAliasValueExpressions");
            this.ParameterAliasValueExpressions = new Dictionary<string, string>(parameterAliasValueExpressions, StringComparer.Ordinal);
            this.ParameterAliasValueNodesCached = new Dictionary<string, SingleValueNode>(StringComparer.Ordinal);
        }

        /// <summary>
        /// Gets the up-to-now cached semantics nodes of parameter alias value expressions (StringComparer.Ordinal)
        /// Only referenced parameter alias will have their value nodes cached.
        /// </summary>
        public IDictionary<string, SingleValueNode> ParameterAliasValueNodesCached { get; private set; }

        /// <summary>
        /// Gets the parameter alias's value expressions like @p1=... (StringComparer.Ordinal)
        /// </summary>
        internal IDictionary<string, string> ParameterAliasValueExpressions { get; private set; }

        /// <summary>
        /// Gets a parameter alias's value expression (e.g. the string content of @p1=...).
        /// </summary>
        /// <param name="alias">The parameter alias.</param>
        /// <returns>The alias value expression text.</returns>
        public string GetAliasValueExpression(string alias)
        {
            string ret = null;
            if (this.ParameterAliasValueExpressions.TryGetValue(alias, out ret))
            {
                return ret;
            }

            return null;
        }
    }
}

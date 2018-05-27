//---------------------------------------------------------------------
// <copyright file="ParameterAliasValueAccessor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
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

//   OData .NET Libraries ver. 6.8.1
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

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;

    #endregion Namespaces

    /// <summary>
    /// An resource specific expression representing a projection query option.
    /// </summary>
    internal class ProjectionQueryOptionExpression : QueryOptionExpression
    {
        #region Private fields

        /// <summary>projection expression to evaluate on client on results from server to materialize type</summary>
        private readonly LambdaExpression lambda;

        /// <summary>projection paths to send to the server</summary>
        private readonly List<string> paths;

        #endregion Private fields

        /// <summary>
        /// Creates a ProjectionQueryOption expression
        /// </summary>
        /// <param name="type">the return type of the expression</param>
        /// <param name="lambda">projection expression</param>
        /// <param name="paths">Projection paths for the query option</param>
        internal ProjectionQueryOptionExpression(Type type, LambdaExpression lambda, List<string> paths)
            : base(type)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(lambda != null, "lambda != null");
            Debug.Assert(paths != null, "paths != null");

            this.lambda = lambda;
            this.paths = paths;
        }

        /// <summary>
        /// The <see cref="ExpressionType"/> of the <see cref="Expression"/>.
        /// </summary>
        public override ExpressionType NodeType
        {
            get { return (ExpressionType)ResourceExpressionType.ProjectionQueryOption; }
        }

        #region Internal properties

        /// <summary>
        /// expression for the projection
        /// </summary>
        internal LambdaExpression Selector
        {
            get
            {
                return this.lambda;
            }
        }

        /// <summary>
        /// expression for the projection
        /// </summary>
        internal List<string> Paths
        {
            get
            {
                return this.paths;
            }
        }

        #endregion Internal properties
    }
}

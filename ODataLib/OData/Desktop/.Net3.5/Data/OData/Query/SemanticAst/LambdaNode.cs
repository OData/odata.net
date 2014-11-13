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
    #region Namespaces

    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    #endregion Namespaces

    /// <summary>
    /// Node representing an Any/All query.
    /// </summary>
    public abstract class LambdaNode : SingleValueNode
    {
        /// <summary>
        /// The collection of rangeVariables in scope for this Any or All.
        /// </summary>
        private readonly Collection<RangeVariable> rangeVariables;

        /// <summary>
        /// The newest range variable added for by this Any or All.
        /// </summary>
        private readonly RangeVariable currentRangeVariable;

        /// <summary>
        /// Create a LambdaNode
        /// </summary>
        /// <param name="rangeVariables">The collection of rangeVariables in scope for this Any or All.</param>
        protected LambdaNode(Collection<RangeVariable> rangeVariables) : this(rangeVariables, null)
        {
            Debug.Assert(false, "Don't ever call this, its for backcompat");
        }

        /// <summary>
        /// Create a LambdaNode
        /// </summary>
        /// <param name="rangeVariables">The collection of rangeVariables in scope for this Any or All.</param>
        /// <param name="currentRangeVariable">The newest range variable added for by this Any or All.</param>
        protected LambdaNode(Collection<RangeVariable> rangeVariables, RangeVariable currentRangeVariable)
        {
            this.rangeVariables = rangeVariables;
            this.currentRangeVariable = currentRangeVariable;
        }

        /// <summary>
        /// Gets the collection of rangeVariables in scope for this Any or All.
        /// </summary>
        public Collection<RangeVariable> RangeVariables
        {
            get { return this.rangeVariables; }
        }

        /// <summary>
        /// Gets the newest range variable added for by this Any or All.
        /// </summary>
        public RangeVariable CurrentRangeVariable
        {
            get { return this.currentRangeVariable; }
        }

        /// <summary>
        /// Gets or Sets the associated boolean expression
        /// </summary>
        public SingleValueNode Body
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the parent entity set or navigation property
        /// </summary>
        public CollectionNode Source
        {
            get;
            set;
        }
    }
}

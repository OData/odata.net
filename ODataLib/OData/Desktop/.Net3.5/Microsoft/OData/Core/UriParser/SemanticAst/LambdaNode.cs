//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
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

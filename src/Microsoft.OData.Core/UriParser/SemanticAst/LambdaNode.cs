//---------------------------------------------------------------------
// <copyright file="LambdaNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

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
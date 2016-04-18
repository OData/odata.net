//---------------------------------------------------------------------
// <copyright file="ODataResourceSet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Describes a collection of entities.
    /// </summary>
    public sealed class ODataResourceSet : ODataResourceSetBase
    {
        /// <summary>The feed actions provided by the user or seen on the wire (never computed).</summary>
        private List<ODataAction> actions = new List<ODataAction>();

        /// <summary>The feed functions provided by the user or seen on the wire (never computed).</summary>
        private List<ODataFunction> functions = new List<ODataFunction>();

        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataWriter"/> for this <see cref="ODataResourceSet"/>.
        /// </summary>
        private ODataResourceSerializationInfo serializationInfo;

        /// <summary>Gets the feed actions.</summary>
        /// <returns>The feed actions.</returns>
        public IEnumerable<ODataAction> Actions
        {
            get { return this.actions; }
        }

        /// <summary>Gets the feed functions.</summary>
        /// <returns>The feed functions.</returns>
        public IEnumerable<ODataFunction> Functions
        {
            get { return this.functions; }
        }

        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataWriter"/> for this <see cref="ODataResourceSet"/>.
        /// </summary>
        internal ODataResourceSerializationInfo SerializationInfo
        {
            get
            {
                return this.serializationInfo;
            }

            set
            {
                this.serializationInfo = ODataResourceSerializationInfo.Validate(value);
            }
        }

        /// <summary>
        /// Add action to feed.
        /// </summary>
        /// <param name="action">The action to add.</param>
        public void AddAction(ODataAction action)
        {
            ExceptionUtils.CheckArgumentNotNull(action, "action");
            if (!this.actions.Contains(action))
            {
                this.actions.Add(action);
            }
        }

        /// <summary>
        /// Add function to feed.
        /// </summary>
        /// <param name="function">The function to add.</param>
        public void AddFunction(ODataFunction function)
        {
            ExceptionUtils.CheckArgumentNotNull(function, "function");
            if (!this.functions.Contains(function))
            {
                this.functions.Add(function);
            }
        }
    }
}

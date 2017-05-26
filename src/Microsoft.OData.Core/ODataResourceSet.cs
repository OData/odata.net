//---------------------------------------------------------------------
// <copyright file="ODataResourceSet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System.Collections.Generic;
    using Microsoft.OData.Metadata;

    /// <summary>
    /// Describes a collection of entities.
    /// </summary>
    public sealed class ODataResourceSet : ODataResourceSetBase
    {
        /// <summary>The resource set actions provided by the user or seen on the wire (never computed).</summary>
        private List<ODataAction> actions = new List<ODataAction>();

        /// <summary>The resource set functions provided by the user or seen on the wire (never computed).</summary>
        private List<ODataFunction> functions = new List<ODataFunction>();

        /// <summary>
        /// Provides additional serialization information to the <see cref="ODataWriter"/> for this <see cref="ODataResourceSet"/>.
        /// </summary>
        private ODataResourceSerializationInfo serializationInfo;

        /// <summary>
        /// The type name of the resource set.
        /// </summary>
        private string typeName;

        /// <summary>Gets the resource set actions.</summary>
        /// <returns>The resource set actions.</returns>
        public IEnumerable<ODataAction> Actions
        {
            get { return this.actions; }
        }

        /// <summary>Gets the resource set functions.</summary>
        /// <returns>The resource set functions.</returns>
        public IEnumerable<ODataFunction> Functions
        {
            get { return this.functions; }
        }

        /// <summary>Gets the resource set type name.</summary>
        /// <returns>The resource set type name.</returns>
        public string TypeName
        {
            get
            {
                if (typeName == null && this.SerializationInfo != null && this.SerializationInfo.ExpectedTypeName != null)
                {
                    typeName = EdmLibraryExtensions.GetCollectionTypeName(this.serializationInfo.ExpectedTypeName);
                }

                return typeName;
            }

            set
            {
                this.typeName = value;
            }
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
        /// Add action to resource set.
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
        /// Add function to resource set.
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

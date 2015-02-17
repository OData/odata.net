//---------------------------------------------------------------------
// <copyright file="DescriptorData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using Microsoft.Test.Taupo.Astoria.Contracts.Product;

    /// <summary>
    /// Abstract class that represents data for the Descriptor (from Microsoft.OData.Client namespace).
    /// </summary>
    public abstract class DescriptorData
    {
        /// <summary>
        /// Initializes a new instance of the DescriptorData class
        /// </summary>
        protected DescriptorData()
        {
            this.State = EntityStates.Detached;
            this.ChangeOrder = uint.MaxValue;
        }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public EntityStates State { get; set; }

        /// <summary>
        /// Gets the change order.
        /// </summary>
        /// <value>The change order.</value>
        public long ChangeOrder { get; internal set; }
    }
}
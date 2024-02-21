//---------------------------------------------------------------------
// <copyright file="ODataSimplifiedOptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData
{
    /// <summary>
    /// Options which used to control the behaviour related odata simplified.
    /// </summary>
    internal sealed class ODataSimplifiedOptionsDeleted
    {
        

        /// <summary>
        /// Constructor of ODataSimplifiedOptions
        /// </summary>
        private ODataSimplifiedOptionsDeleted() : this(null /*version*/)
        {
        }

        /// <summary>
        /// Constructor of ODataSimplifiedOptions
        /// </summary>
        /// <param name="version">The ODataVersion to create Default Options for.</param>
        private ODataSimplifiedOptionsDeleted(ODataVersion? version)
        {
        }
        

        /// <summary>
        /// Creates a shallow copy of this <see cref="ODataSimplifiedOptions"/>.
        /// </summary>
        /// <returns>A shallow copy of this <see cref="ODataSimplifiedOptions"/>.</returns>
        private ODataSimplifiedOptionsDeleted Clone()
        {
            var copy = new ODataSimplifiedOptionsDeleted();
            copy.CopyFrom(this);
            return copy;
        }
        

        /// <summary>
        /// Return the instance of ODataSimplifiedOptions from container if it container not null.
        /// Otherwise return the static instance of ODataSimplifiedOptions.
        /// </summary>
        /// <param name="container">Container</param>
        /// <param name="version">OData Version</param>
        /// <returns>Instance of GetODataSimplifiedOptions</returns>
        private static ODataSimplifiedOptionsDeleted GetODataSimplifiedOptions(IServiceProvider container, ODataVersion? version = null)
        {
            if (container == null)
            {
                return new ODataSimplifiedOptionsDeleted(version);
            }

            return container.GetRequiredService<ODataSimplifiedOptionsDeleted>();
        }

        private void CopyFrom(ODataSimplifiedOptionsDeleted other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");
        }
    }
}

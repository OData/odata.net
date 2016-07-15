//---------------------------------------------------------------------
// <copyright file="ODataAnnotatable.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Microsoft.OData
{
    /// <summary>
    /// Base class for all annotatable types in OData library.
    /// </summary>
#if ORCAS
    [Serializable]
#endif
    public abstract class ODataAnnotatable
    {
        /// <summary>
        /// Collection of custom instance annotations.
        /// </summary>
#if ORCAS
        [NonSerialized]
#endif
        private ICollection<ODataInstanceAnnotation> instanceAnnotations = new Collection<ODataInstanceAnnotation>();

        /// <summary>
        /// The annotation for storing @odata.type.
        /// </summary>
        public ODataTypeAnnotation TypeAnnotation { get; set; }

        /// <summary>
        /// Gets the custom instance annotations.
        /// </summary>
        /// <returns>The custom instance annotations.</returns>
        internal ICollection<ODataInstanceAnnotation> GetInstanceAnnotations()
        {
            Debug.Assert(this.instanceAnnotations != null, "this.instanceAnnotations != null");
            return this.instanceAnnotations;
        }

        /// <summary>
        /// Sets the custom instance annotations.
        /// </summary>
        /// <param name="value">The new value to set.</param>
        internal void SetInstanceAnnotations(ICollection<ODataInstanceAnnotation> value)
        {
            ExceptionUtils.CheckArgumentNotNull(value, "value");
            this.instanceAnnotations = value;
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="ODataJsonLightRawAnnotationSet.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    #endregion Namespaces

    /// <summary>
    /// Class representing a property's raw annotations in the JSON Light format.
    /// </summary>
    internal sealed class ODataJsonLightRawAnnotationSet
    {
        /// <summary>The (instance and property) annotations included in this annotation group.</summary>
        private IDictionary<string, string> annotations;

        /// <summary>
        /// The (instance and property) annotations included in this annotation group.
        /// </summary>
        /// <remarks>The keys in the dictionary are the names of the annotations, the values are their values.</remarks>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "We allow setting of all properties on public ODataLib OM classes.")]
        public IDictionary<string, string> Annotations
        {
            get
            {
                return this.annotations;
            }

            set
            {
                this.annotations = value;
            }
        }
    }
}

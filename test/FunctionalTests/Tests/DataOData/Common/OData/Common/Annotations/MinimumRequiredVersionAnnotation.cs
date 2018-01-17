//---------------------------------------------------------------------
// <copyright file="MinimumRequiredVersionAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    #endregion Namespaces

    /// <summary>
    /// An annotation to associate a minimum version to an element.
    /// </summary>
    public sealed class MinimumRequiredVersionAnnotation : Annotation
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="model">The model to store the annotation on.</param>
        public MinimumRequiredVersionAnnotation(ODataVersion minimumVersion)
        {
            this.MinimumVersion = minimumVersion;
        }

        /// <summary>
        /// The minimum version to store on the annotation.
        /// </summary>
        public ODataVersion MinimumVersion { get; set; }
    }
}

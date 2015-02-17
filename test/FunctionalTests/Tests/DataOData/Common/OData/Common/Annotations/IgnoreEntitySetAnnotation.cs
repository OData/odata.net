//---------------------------------------------------------------------
// <copyright file="IgnoreEntitySetAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    #endregion Namespaces

    /// <summary>
    /// An annotation to indicate that entity set information (if available) should be ignored during reading.
    /// </summary>
    public sealed class IgnoreEntitySetAnnotation : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get { return "Entity set information will be ignored during reading."; }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>A clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new IgnoreEntitySetAnnotation();
        }
    }
}

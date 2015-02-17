//---------------------------------------------------------------------
// <copyright file="MemberInSpatialTypeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Annotations for members in a spatial type.
    /// </summary>
    public class MemberInSpatialTypeAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the MemberInSpatialTypeAnnotation class.
        /// </summary>
        /// <param name="isStatic">whether it is a static function.</param>
        public MemberInSpatialTypeAnnotation(bool isStatic)
        {
            ExceptionUtilities.CheckArgumentNotNull(isStatic, "isStatic");
            this.IsStaticMember = isStatic;
        }

        /// <summary>
        /// Gets a value indicating whether the function is static.
        /// </summary>
        public bool IsStaticMember { get; private set; }
    }
}

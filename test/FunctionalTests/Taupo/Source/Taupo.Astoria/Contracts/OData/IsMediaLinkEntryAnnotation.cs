//---------------------------------------------------------------------
// <copyright file="IsMediaLinkEntryAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// An annotation indicating that an entity is a media link entry
    /// </summary>
    public class IsMediaLinkEntryAnnotation : ODataPayloadElementEquatableAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the IsMediaLinkEntryAnnotation class
        /// </summary>
        internal IsMediaLinkEntryAnnotation()
        {
        }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return "MLE";
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new IsMediaLinkEntryAnnotation();
        }

        /// <summary>
        /// Returns whether or not the given annotation is equal to the current annotation
        /// </summary>
        /// <param name="other">The annotation to compare to</param>
        /// <returns>True if the annotations are equivalent, false otherwise</returns>
        public override bool Equals(ODataPayloadElementEquatableAnnotation other)
        {
            return this.CastAndCheckEquality<IsMediaLinkEntryAnnotation>(other, o => true);
        }
    }
}

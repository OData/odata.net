//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementEquatableAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using System.Diagnostics;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents a custom annotation for an OData payload element which can be compared.
    /// Annotations deriving from this class will become part of the comparison of the ODataPayloadElement trees.
    /// </summary>
    public abstract class ODataPayloadElementEquatableAnnotation : ODataPayloadElementAnnotation, IEquatable<ODataPayloadElementEquatableAnnotation>
    {
        /// <summary>
        /// Gets a value indicating whether or not to ignore this annotation when comparing payloads.
        /// Should only be true for annotations which are used to store extra test context about the
        /// payload and have no impact on serialization/deserialization.
        /// </summary>
        public virtual bool IgnoreDuringPayloadComparison
        {
            get { return false; }
        }

        /// <summary>
        /// Returns whether or not the given annotation is equal to the current annotation
        /// </summary>
        /// <param name="other">The annotation to compare to</param>
        /// <returns>True if the annotations are equivalent, false otherwise</returns>
        public abstract bool Equals(ODataPayloadElementEquatableAnnotation other);

        /// <summary>
        /// Helper for derived types to implement equality comparison
        /// </summary>
        /// <typeparam name="TAnnotation">The derived annotation type</typeparam>
        /// <param name="other">The other annotation to cast and pass to the comparison function</param>
        /// <param name="areEqualFunc">Callback for equality check</param>
        /// <returns>False if the annotation is of the wrong type, otherwise the result of the callback</returns>
        protected bool CastAndCheckEquality<TAnnotation>(ODataPayloadElementEquatableAnnotation other, Func<TAnnotation, bool> areEqualFunc) where TAnnotation : ODataPayloadElementEquatableAnnotation
        {
            ExceptionUtilities.Assert(typeof(TAnnotation) == this.GetType(), "Unexpected generic argument '{0}'. Expected '{1}'", typeof(TAnnotation), this.GetType());

            var afterCast = other as TAnnotation;
            if (afterCast == null)
            {
                // this also handles cases where other is null
                return false;
            }

            ExceptionUtilities.CheckArgumentNotNull(areEqualFunc, "areEqualFunc");
            return areEqualFunc(afterCast);
        }
    }
}
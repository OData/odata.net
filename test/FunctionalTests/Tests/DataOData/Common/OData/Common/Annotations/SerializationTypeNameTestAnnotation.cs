//---------------------------------------------------------------------
// <copyright file="SerializationTypeNameTestAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Annotations
{
    #region Namespaces
    using System.Globalization;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    #endregion Namespaces

    /// <summary>
    /// Represents the type name in the payload, as opposed to the type name from the model (if they differ).
    /// This annotation can be used on EntityInstance, ComplexInstance, ComplexMultiValue and PrimitiveMultiValue
    /// </summary>
    public class SerializationTypeNameTestAnnotation : ODataPayloadElementEquatableAnnotation
    {
        /// <summary>
        /// The type name in the payload.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Returns whether or not the given annotation is equal to the current annotation.
        /// </summary>
        /// <param name="other">The annotation to compare to.</param>
        /// <returns>True if the annotations are equivalent, false otherwise.</returns>
        public override bool Equals(ODataPayloadElementEquatableAnnotation other)
        {
            return this.CastAndCheckEquality<SerializationTypeNameTestAnnotation>(
                other,
                (a) => a.TypeName == this.TypeName);
        }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier.
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "Serialization type name: {0}",
                    this.TypeName ?? "<null>");
            }
        }

        /// <summary>
        /// Creates a clone of the annotation.
        /// </summary>
        /// <returns>A clone of the annotation.</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new SerializationTypeNameTestAnnotation
            {
                TypeName = this.TypeName,
            };
        }
    }
}

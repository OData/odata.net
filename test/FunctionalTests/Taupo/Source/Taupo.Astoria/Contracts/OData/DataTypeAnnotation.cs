//---------------------------------------------------------------------
// <copyright file="DataTypeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Globalization;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Payload annotation containing a data type
    /// </summary>
    public class DataTypeAnnotation : ODataPayloadElementEquatableAnnotation
    {
        /// <summary>
        /// Gets or sets the data type
        /// </summary>
        public DataType DataType { get; set; }

        /// <summary>
        /// Gets or sets the IEdmType
        /// </summary>
        public IEdmType EdmDataType { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                if (this.DataType != null)
                {
                    return string.Format(CultureInfo.InvariantCulture, "DataType:{0}", this.DataType);
                }

                if (this.EdmDataType != null)
                {
                    return string.Format(CultureInfo.InvariantCulture, "DataType:{0}", this.EdmDataType);
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not to ignore this annotation when comparing payloads.
        /// Should only be true for annotations which are used to store extra test context about the
        /// payload and have no impact on serialization/deserialization.
        /// </summary>
        public override bool IgnoreDuringPayloadComparison
        {
            get { return true; }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new DataTypeAnnotation
            {
                DataType = this.DataType,
                EdmDataType = this.EdmDataType
            };
        }

        /// <summary>
        /// Returns whether or not the given annotation is equal to the current annotation
        /// </summary>
        /// <param name="other">The annotation to compare to</param>
        /// <returns>True if the annotations are equivalent, false otherwise</returns>
        public override bool Equals(ODataPayloadElementEquatableAnnotation other)
        {
            return this.CastAndCheckEquality<DataTypeAnnotation>(other, o => o.DataType.Equals(this.DataType) && o.EdmDataType.Equals(this.EdmDataType));
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="PayloadOrderODataPayloadElementAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    #endregion Namespaces

    /// <summary>
    /// Annotation which holds list of items as they were read from the payload in the payload order for the annotated element.
    /// </summary>
    public class PayloadOrderODataPayloadElementAnnotation : ODataPayloadElementEquatableAnnotation
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public PayloadOrderODataPayloadElementAnnotation()
        {
            this.PayloadItems = new List<string>();
        }

        /// <summary>
        /// List of names of payload items in the payload order.
        /// </summary>
        public List<string> PayloadItems { get; private set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Payload order items: ");
                if (this.PayloadItems != null)
                {
                    sb.Append(string.Join(", ", this.PayloadItems));
                }

                if (this.PayloadItems == null || this.PayloadItems.Count == 0)
                {
                    sb.Append("<none>");
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>A clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new PayloadOrderODataPayloadElementAnnotation()
            {
                PayloadItems = new List<string>(this.PayloadItems)
            };
        }

        /// <summary>
        /// Returns whether or not the given annotation is equal to the current annotation
        /// </summary>
        /// <param name="other">The annotation to compare to</param>
        /// <returns>True if the annotations are equivalent, false otherwise</returns>
        public override bool Equals(ODataPayloadElementEquatableAnnotation other)
        {
            return this.CastAndCheckEquality<PayloadOrderODataPayloadElementAnnotation>(other, (o) =>
                {
                    return o.StringRepresentation == this.StringRepresentation;
                });
        }
    }
}
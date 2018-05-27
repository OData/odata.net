//---------------------------------------------------------------------
// <copyright file="FunctionAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Globalization;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Payload annotation containing a function call
    /// </summary>
    public class FunctionAnnotation : ODataPayloadElementEquatableAnnotation
    {
        /// <summary>
        /// Gets or sets the function
        /// </summary>
        public Function Function { get; set; }

        /// <summary>
        /// Gets or sets the function import.
        /// </summary>
        public EdmOperationImport FunctionImport { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get 
            {
                if (Function != null)
                {
                    return string.Format(CultureInfo.InvariantCulture, "Function:{0}", this.Function.Name);
                }

                if (FunctionImport != null)
                {
                    return string.Format(CultureInfo.InvariantCulture, "Function:{0}", this.FunctionImport.Name);
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new FunctionAnnotation { Function = this.Function, FunctionImport = this.FunctionImport };
        }

        /// <summary>
        /// Returns whether or not the given annotation is equal to the current annotation
        /// </summary>
        /// <param name="other">The annotation to compare to</param>
        /// <returns>True if the annotations are equivalent, false otherwise</returns>
        public override bool Equals(ODataPayloadElementEquatableAnnotation other)
        {
            return this.CastAndCheckEquality<FunctionAnnotation>(other, o => o.Function == this.Function && o.FunctionImport == this.FunctionImport);
        }
    }
}

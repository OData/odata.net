//---------------------------------------------------------------------
// <copyright file="JsonCollectionResultWrapperAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Indicates that a json array should/should-not or had/did-not-have the results wrapper.
    /// </summary>
    public class JsonCollectionResultWrapperAnnotation : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonCollectionResultWrapperAnnotation"/> class.
        /// </summary>
        /// <param name="value">The value of the annotation.</param>
        public JsonCollectionResultWrapperAnnotation(bool value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return "JsonResultWrapper:" + this.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the result wrapper was/should-be present
        /// </summary>
        public bool Value { get; private set; }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new JsonCollectionResultWrapperAnnotation(this.Value);
        }
    }
}

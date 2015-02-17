//---------------------------------------------------------------------
// <copyright file="JsonPayloadElementRepresentationAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts.Json
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    #endregion Namespaces

    /// <summary>
    /// An annotation which stores the exact JSON representation of an ODataPayloadElement.
    /// </summary>
    public class JsonPayloadElementRepresentationAnnotation : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// The JsonValue representing the payload element the annotation is on.
        /// </summary>
        public JsonValue Json { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return this.Json == null ? "JSON representation missing." : ("JSON representation: " + this.Json.ToString());
            }
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new JsonPayloadElementRepresentationAnnotation { Json = (this.Json == null) ? null : this.Json.Clone() };
        }
    }
}

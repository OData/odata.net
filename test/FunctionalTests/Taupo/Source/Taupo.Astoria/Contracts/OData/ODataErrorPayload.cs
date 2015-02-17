//---------------------------------------------------------------------
// <copyright file="ODataErrorPayload.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents error payloads 
    /// </summary>
    public class ODataErrorPayload : ODataPayloadElement
    {
        /// <summary>
        /// Gets or sets the code value
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the message value
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the inner error
        /// </summary>
        public ODataInternalExceptionPayload InnerError { get; set; }

        /// <summary>
        /// Gets a string representation of the payload element to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "Error:{0}", this.Message);
            }
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this element.</param>
        /// <returns>The result of visiting this expression.</returns>
        public override TResult Accept<TResult>(IODataPayloadElementVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that does not return a result.
        /// </summary>
        /// <param name="visitor">The visitor that is visiting this element.</param>
        public override void Accept(IODataPayloadElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="ErrorMessageNormalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    #endregion Namespaces

    /// <summary>
    /// Visitor which replaces all 'null' error messages with empty error messages since in ATOM we always get an empty error message if the error element is preset.
    /// </summary>
    /// <remarks>Only use for ATOM payloads.</remarks>
    public class ErrorMessageNormalizer : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// Visit the payload element and replace null error messages.
        /// </summary>
        /// <param name="payloadElement">The root of the payload element tree to visit.</param>
        /// <returns>The <paramref name="payloadElement"/> after it has been visited.</returns>
        public static ODataPayloadElement Visit(ODataPayloadElement payloadElement)
        {
            new ErrorMessageNormalizer().Recurse(payloadElement);
            return payloadElement;
        }

        /// <summary>
        /// Visits a payload element whose root is an ODataErrorPayload.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        public override void Visit(ODataErrorPayload payloadElement)
        {
            base.Visit(payloadElement);
        }
    }
}
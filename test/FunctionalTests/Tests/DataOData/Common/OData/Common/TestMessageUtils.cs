//---------------------------------------------------------------------
// <copyright file="TestMessageUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Utility methods for test messages.
    /// </summary>
    public static class TestMessageUtils
    {
        /// <summary>
        /// Set the default content type header for a message of the given payload kind.
        /// </summary>
        /// <param name="testMessage">The test message to set the Content-Type header on.</param>
        /// <param name="format">The format of the message.</param>
        /// <param name="payloadKind">The payload kind for which to determine and set the content type.</param>
        public static void SetContentType(this TestMessage testMessage, ODataFormat format, ODataPayloadKind payloadKind)
        {
            string contentType = TestMediaTypeUtils.GetDefaultContentType(payloadKind, format);
            testMessage.SetHeader(ODataConstants.ContentTypeHeader, contentType);
        }
    }
}

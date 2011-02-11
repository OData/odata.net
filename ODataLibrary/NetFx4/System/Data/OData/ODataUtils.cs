//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.OData
{
    #region Namespaces.
    #endregion Namespaces.

    /// <summary>
    /// Utility methods used with the OData library.
    /// </summary>
#if INTERNAL_DROP
    internal static class ODataUtils
#else
    public static class ODataUtils
#endif
    {
        /// <summary>
        /// Sets the content-type and data service version headers on the message used by the message writer.
        /// This method can be called if it is important to set all the message headers before calling any of the
        /// write methods on the <paramref name="messageWriter"/>.
        /// If it is sufficient to set the headers when the write methods on the <paramref name="messageWriter"/> 
        /// are called, you don't have to call this method and setting the headers will happen automatically.
        /// </summary>
        /// <param name="messageWriter">The message writer to set the headers for.</param>
        /// <param name="payloadKind">The kind of payload to be written with the message writer.</param>
        public static void SetHeadersForPayload(ODataMessageWriter messageWriter, ODataPayloadKind payloadKind)
        {
            ExceptionUtils.CheckArgumentNotNull(messageWriter, "messageWriter");

            if (payloadKind == ODataPayloadKind.Unsupported)
            {
                throw new ArgumentException(Strings.ODataMessageWriter_CannotSetHeadersWithInvalidPayloadKind(payloadKind), "payloadKind");
            }

            messageWriter.SetHeaders(payloadKind);
        }
    }
}

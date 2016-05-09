//---------------------------------------------------------------------
// <copyright file="ODataMessageInfoExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System.Diagnostics;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif

    /// <summary>
    /// Class provides context informatation of certain <see cref="IODataRequestMessage"/>
    /// or <see cref="IODataResponseMessage"/>
    /// </summary>
    internal static class ODataMessageInfoExtensions
    {
        public static ODataMessageInfo ComputeStreamFunc(this ODataMessageInfo messageInfo)
        {
            Debug.Assert(messageInfo != null, "messageInfo != null");

            messageInfo.MessageStream = messageInfo.GetMessageStream();
            messageInfo.TextReader = null;
            return messageInfo;
        }

#if PORTABLELIB
        public static Task<ODataMessageInfo> ComputeStreamFuncAsync(this ODataMessageInfo messageInfo)
        {
            Debug.Assert(messageInfo != null, "messageInfo != null");

            return messageInfo.GetMessageStreamAsync().FollowOnSuccessWith(
                streamTask =>
                {
                    messageInfo.MessageStream = streamTask.Result;
                    messageInfo.TextReader = null;
                    return messageInfo;
                });
        }
#endif
    }
}

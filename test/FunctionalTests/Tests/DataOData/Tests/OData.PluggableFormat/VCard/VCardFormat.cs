//---------------------------------------------------------------------
// <copyright file="VCardFormat.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.PluggableFormat.VCard
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.OData.Core;

    public class VCardFormat : ODataFormat
    {
        public override IEnumerable<ODataPayloadKind> DetectPayloadKind(ODataMessageInfo messageInfo, ODataMessageReaderSettings settings)
        {
            throw new System.NotImplementedException();
        }

        public override ODataInputContext CreateInputContext(ODataMessageInfo messageInfo, ODataMessageReaderSettings messageReaderSettings)
        {
            return new VCardInputContext(this, messageInfo.GetMessageStream(), messageInfo.MediaType, messageInfo.Encoding, messageReaderSettings, messageInfo.IsResponse, true, messageInfo.Model, messageInfo.UrlResolver);
        }

        public override ODataOutputContext CreateOutputContext(ODataMessageInfo messageInfo, ODataMessageWriterSettings messageWriterSettings)
        {
            return new VCardOutputContext(this, messageInfo.GetMessageStream(), messageInfo.Encoding, messageWriterSettings, messageInfo.IsResponse, true, messageInfo.Model, messageInfo.UrlResolver);
        }

        public override System.Threading.Tasks.Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(ODataMessageInfo messageInfo, ODataMessageReaderSettings settings)
        {
            throw new System.NotImplementedException();
        }

        public override System.Threading.Tasks.Task<ODataInputContext> CreateInputContextAsync(ODataMessageInfo messageInfo, ODataMessageReaderSettings messageReaderSettings)
        {
            return messageInfo.GetMessageStreamAsync()
                .ContinueWith(
                    (streamTask) => (ODataInputContext) new VCardInputContext(
                        this,
                        streamTask.Result,
                        messageInfo.MediaType,
                        messageInfo.Encoding,
                        messageReaderSettings,
                        messageInfo.IsResponse,
                        /*sync*/ false,
                        messageInfo.Model,
                        messageInfo.UrlResolver),
                    TaskContinuationOptions.NotOnFaulted);
        }

        public override System.Threading.Tasks.Task<ODataOutputContext> CreateOutputContextAsync(ODataMessageInfo messageInfo, ODataMessageWriterSettings messageWriterSettings)
        {
            throw new System.NotImplementedException();
        }
    }
}

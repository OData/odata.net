//---------------------------------------------------------------------
// <copyright file="AvroFormat.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ENABLE_AVRO
namespace Microsoft.Test.OData.PluggableFormat.Avro
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.OData;

    public class AvroFormat : ODataFormat
    {
        private static readonly AvroFormat avroFormat = new AvroFormat();

        public static ODataFormat Avro { get { return avroFormat; } }

        private AvroFormat() { }

        public override IEnumerable<ODataPayloadKind> DetectPayloadKind(ODataMessageInfo messageInfo, ODataMessageReaderSettings settings)
        {
            throw new System.NotImplementedException();
        }

        public override ODataInputContext CreateInputContext(ODataMessageInfo messageInfo, ODataMessageReaderSettings messageReaderSettings)
        {
            return new ODataAvroInputContext(this, messageInfo, messageReaderSettings);
        }

        public override ODataOutputContext CreateOutputContext(ODataMessageInfo messageInfo, ODataMessageWriterSettings messageWriterSettings)
        {
            return new ODataAvroOutputContext(this, messageInfo, messageWriterSettings);
        }

        public override Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(ODataMessageInfo messageInfo, ODataMessageReaderSettings settings)
        {
            throw new System.NotImplementedException();
        }

        public override Task<ODataInputContext> CreateInputContextAsync(ODataMessageInfo messageInfo, ODataMessageReaderSettings messageReaderSettings)
        {
            throw new System.NotImplementedException();
        }

        public override Task<ODataOutputContext> CreateOutputContextAsync(ODataMessageInfo messageInfo, ODataMessageWriterSettings messageWriterSettings)
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif
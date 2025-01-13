namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _IPv4addressTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._IPv4address>
    {
        private _IPv4addressTranscriber()
        {
        }
        
        public static _IPv4addressTranscriber Instance { get; } = new _IPv4addressTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._IPv4address value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._decⲻoctetTranscriber.Instance.Transcribe(value._decⲻoctet_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._decⲻoctetTranscriber.Instance.Transcribe(value._decⲻoctet_2, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_2, builder);
__GeneratedOdata.Trancsribers.Rules._decⲻoctetTranscriber.Instance.Transcribe(value._decⲻoctet_3, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_3, builder);
__GeneratedOdata.Trancsribers.Rules._decⲻoctetTranscriber.Instance.Transcribe(value._decⲻoctet_4, builder);

        }
    }
    
}

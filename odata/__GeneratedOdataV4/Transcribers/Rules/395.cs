namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _IPv4addressTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._IPv4address>
    {
        private _IPv4addressTranscriber()
        {
        }
        
        public static _IPv4addressTranscriber Instance { get; } = new _IPv4addressTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._IPv4address value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._decⲻoctetTranscriber.Instance.Transcribe(value._decⲻoctet_1, builder);
__GeneratedOdataV4.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._decⲻoctetTranscriber.Instance.Transcribe(value._decⲻoctet_2, builder);
__GeneratedOdataV4.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._decⲻoctetTranscriber.Instance.Transcribe(value._decⲻoctet_3, builder);
__GeneratedOdataV4.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_3, builder);
__GeneratedOdataV4.Trancsribers.Rules._decⲻoctetTranscriber.Instance.Transcribe(value._decⲻoctet_4, builder);

        }
    }
    
}

namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _metadataOptionsTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._metadataOptions>
    {
        private _metadataOptionsTranscriber()
        {
        }
        
        public static _metadataOptionsTranscriber Instance { get; } = new _metadataOptionsTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._metadataOptions value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._metadataOptionTranscriber.Instance.Transcribe(value._metadataOption_1, builder);
foreach (var _Ⲥʺx26ʺ_metadataOptionↃ_1 in value._Ⲥʺx26ʺ_metadataOptionↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._Ⲥʺx26ʺ_metadataOptionↃTranscriber.Instance.Transcribe(_Ⲥʺx26ʺ_metadataOptionↃ_1, builder);
}

        }
    }
    
}

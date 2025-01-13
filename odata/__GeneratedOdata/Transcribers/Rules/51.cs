namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _batchOptionsTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._batchOptions>
    {
        private _batchOptionsTranscriber()
        {
        }
        
        public static _batchOptionsTranscriber Instance { get; } = new _batchOptionsTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._batchOptions value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._batchOptionTranscriber.Instance.Transcribe(value._batchOption_1, builder);
foreach (var _Ⲥʺx26ʺ_batchOptionↃ_1 in value._Ⲥʺx26ʺ_batchOptionↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._Ⲥʺx26ʺ_batchOptionↃTranscriber.Instance.Transcribe(_Ⲥʺx26ʺ_batchOptionↃ_1, builder);
}

        }
    }
    
}

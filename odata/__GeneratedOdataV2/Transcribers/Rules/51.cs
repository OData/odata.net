namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _batchOptionsTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._batchOptions>
    {
        private _batchOptionsTranscriber()
        {
        }
        
        public static _batchOptionsTranscriber Instance { get; } = new _batchOptionsTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._batchOptions value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._batchOptionTranscriber.Instance.Transcribe(value._batchOption_1, builder);
foreach (var _Ⲥʺx26ʺ_batchOptionↃ_1 in value._Ⲥʺx26ʺ_batchOptionↃ_1)
{
Inners._Ⲥʺx26ʺ_batchOptionↃTranscriber.Instance.Transcribe(_Ⲥʺx26ʺ_batchOptionↃ_1, builder);
}

        }
    }
    
}

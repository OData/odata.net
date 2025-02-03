namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _preferTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._prefer>
    {
        private _preferTranscriber()
        {
        }
        
        public static _preferTranscriber Instance { get; } = new _preferTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._prefer value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Inners._ʺx50x72x65x66x65x72ʺTranscriber.Instance.Transcribe(value._ʺx50x72x65x66x65x72ʺ_1, builder);
__GeneratedOdataV3.Trancsribers.Inners._ʺx3AʺTranscriber.Instance.Transcribe(value._ʺx3Aʺ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._OWSTranscriber.Instance.Transcribe(value._OWS_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._preferenceTranscriber.Instance.Transcribe(value._preference_1, builder);
foreach (var _ⲤCOMMA_preferenceↃ_1 in value._ⲤCOMMA_preferenceↃ_1)
{
Inners._ⲤCOMMA_preferenceↃTranscriber.Instance.Transcribe(_ⲤCOMMA_preferenceↃ_1, builder);
}

        }
    }
    
}

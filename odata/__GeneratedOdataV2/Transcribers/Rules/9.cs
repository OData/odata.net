namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _compoundKeyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._compoundKey>
    {
        private _compoundKeyTranscriber()
        {
        }
        
        public static _compoundKeyTranscriber Instance { get; } = new _compoundKeyTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._compoundKey value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._keyValuePairTranscriber.Instance.Transcribe(value._keyValuePair_1, builder);
foreach (var _ⲤCOMMA_keyValuePairↃ_1 in value._ⲤCOMMA_keyValuePairↃ_1)
{
Inners._ⲤCOMMA_keyValuePairↃTranscriber.Instance.Transcribe(_ⲤCOMMA_keyValuePairↃ_1, builder);
}
__GeneratedOdataV2.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

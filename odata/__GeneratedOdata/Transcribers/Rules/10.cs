namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _keyValuePairTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._keyValuePair>
    {
        private _keyValuePairTranscriber()
        {
        }
        
        public static _keyValuePairTranscriber Instance { get; } = new _keyValuePairTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._keyValuePair value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ⲤprimitiveKeyPropertyⳆkeyPropertyAliasↃTranscriber.Instance.Transcribe(value._ⲤprimitiveKeyPropertyⳆkeyPropertyAliasↃ_1, builder);
__GeneratedOdata.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
__GeneratedOdata.Trancsribers.Inners._ⲤparameterAliasⳆkeyPropertyValueↃTranscriber.Instance.Transcribe(value._ⲤparameterAliasⳆkeyPropertyValueↃ_1, builder);

        }
    }
    
}

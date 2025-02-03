namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _parameterNamesTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._parameterNames>
    {
        private _parameterNamesTranscriber()
        {
        }
        
        public static _parameterNamesTranscriber Instance { get; } = new _parameterNamesTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._parameterNames value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._parameterNameTranscriber.Instance.Transcribe(value._parameterName_1, builder);
foreach (var _ⲤCOMMA_parameterNameↃ_1 in value._ⲤCOMMA_parameterNameↃ_1)
{
Inners._ⲤCOMMA_parameterNameↃTranscriber.Instance.Transcribe(_ⲤCOMMA_parameterNameↃ_1, builder);
}

        }
    }
    
}

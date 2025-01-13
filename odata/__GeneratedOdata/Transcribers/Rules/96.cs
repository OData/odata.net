namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _parameterNamesTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._parameterNames>
    {
        private _parameterNamesTranscriber()
        {
        }
        
        public static _parameterNamesTranscriber Instance { get; } = new _parameterNamesTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._parameterNames value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._parameterNameTranscriber.Instance.Transcribe(value._parameterName_1, builder);
foreach (var _ⲤCOMMA_parameterNameↃ_1 in value._ⲤCOMMA_parameterNameↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._ⲤCOMMA_parameterNameↃTranscriber.Instance.Transcribe(_ⲤCOMMA_parameterNameↃ_1, builder);
}

        }
    }
    
}

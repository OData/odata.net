namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _functionParameter_ЖⲤCOMMA_functionParameterↃTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._functionParameter_ЖⲤCOMMA_functionParameterↃ>
    {
        private _functionParameter_ЖⲤCOMMA_functionParameterↃTranscriber()
        {
        }
        
        public static _functionParameter_ЖⲤCOMMA_functionParameterↃTranscriber Instance { get; } = new _functionParameter_ЖⲤCOMMA_functionParameterↃTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._functionParameter_ЖⲤCOMMA_functionParameterↃ value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._functionParameterTranscriber.Instance.Transcribe(value._functionParameter_1, builder);
foreach (var _ⲤCOMMA_functionParameterↃ_1 in value._ⲤCOMMA_functionParameterↃ_1)
{
Inners._ⲤCOMMA_functionParameterↃTranscriber.Instance.Transcribe(_ⲤCOMMA_functionParameterↃ_1, builder);
}

        }
    }
    
}

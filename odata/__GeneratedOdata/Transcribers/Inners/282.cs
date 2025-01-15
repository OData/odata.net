namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _functionExprParameter_ЖⲤCOMMA_functionExprParameterↃTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ>
    {
        private _functionExprParameter_ЖⲤCOMMA_functionExprParameterↃTranscriber()
        {
        }
        
        public static _functionExprParameter_ЖⲤCOMMA_functionExprParameterↃTranscriber Instance { get; } = new _functionExprParameter_ЖⲤCOMMA_functionExprParameterↃTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._functionExprParameterTranscriber.Instance.Transcribe(value._functionExprParameter_1, builder);
foreach (var _ⲤCOMMA_functionExprParameterↃ_1 in value._ⲤCOMMA_functionExprParameterↃ_1)
{
Inners._ⲤCOMMA_functionExprParameterↃTranscriber.Instance.Transcribe(_ⲤCOMMA_functionExprParameterↃ_1, builder);
}

        }
    }
    
}

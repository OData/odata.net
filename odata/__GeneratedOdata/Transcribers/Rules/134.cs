namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _functionExprParametersTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._functionExprParameters>
    {
        private _functionExprParametersTranscriber()
        {
        }
        
        public static _functionExprParametersTranscriber Instance { get; } = new _functionExprParametersTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._functionExprParameters value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
if (value._functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._functionExprParameter_ЖⲤCOMMA_functionExprParameterↃTranscriber.Instance.Transcribe(value._functionExprParameter_ЖⲤCOMMA_functionExprParameterↃ_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _functionParametersTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._functionParameters>
    {
        private _functionParametersTranscriber()
        {
        }
        
        public static _functionParametersTranscriber Instance { get; } = new _functionParametersTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._functionParameters value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
if (value._functionParameter_ЖⲤCOMMA_functionParameterↃ_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._functionParameter_ЖⲤCOMMA_functionParameterↃTranscriber.Instance.Transcribe(value._functionParameter_ЖⲤCOMMA_functionParameterↃ_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

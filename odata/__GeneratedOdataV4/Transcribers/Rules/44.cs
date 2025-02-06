namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _functionParametersTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._functionParameters>
    {
        private _functionParametersTranscriber()
        {
        }
        
        public static _functionParametersTranscriber Instance { get; } = new _functionParametersTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._functionParameters value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
if (value._functionParameter_ЖⲤCOMMA_functionParameterↃ_1 != null)
{
__GeneratedOdataV4.Trancsribers.Inners._functionParameter_ЖⲤCOMMA_functionParameterↃTranscriber.Instance.Transcribe(value._functionParameter_ЖⲤCOMMA_functionParameterↃ_1, builder);
}
__GeneratedOdataV4.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _COMMA_functionParameterTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._COMMA_functionParameter>
    {
        private _COMMA_functionParameterTranscriber()
        {
        }
        
        public static _COMMA_functionParameterTranscriber Instance { get; } = new _COMMA_functionParameterTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._COMMA_functionParameter value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._functionParameterTranscriber.Instance.Transcribe(value._functionParameter_1, builder);

        }
    }
    
}

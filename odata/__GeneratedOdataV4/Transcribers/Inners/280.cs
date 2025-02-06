namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _COMMA_functionExprParameterTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._COMMA_functionExprParameter>
    {
        private _COMMA_functionExprParameterTranscriber()
        {
        }
        
        public static _COMMA_functionExprParameterTranscriber Instance { get; } = new _COMMA_functionExprParameterTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._COMMA_functionExprParameter value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._functionExprParameterTranscriber.Instance.Transcribe(value._functionExprParameter_1, builder);

        }
    }
    
}

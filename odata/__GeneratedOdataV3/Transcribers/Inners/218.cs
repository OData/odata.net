namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _COMMA_parameterNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._COMMA_parameterName>
    {
        private _COMMA_parameterNameTranscriber()
        {
        }
        
        public static _COMMA_parameterNameTranscriber Instance { get; } = new _COMMA_parameterNameTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._COMMA_parameterName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._parameterNameTranscriber.Instance.Transcribe(value._parameterName_1, builder);

        }
    }
    
}

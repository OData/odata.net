namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _valueⲻseparatorTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._valueⲻseparator>
    {
        private _valueⲻseparatorTranscriber()
        {
        }
        
        public static _valueⲻseparatorTranscriber Instance { get; } = new _valueⲻseparatorTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._valueⲻseparator value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);

        }
    }
    
}

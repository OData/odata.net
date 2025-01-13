namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _valueⲻseparatorTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._valueⲻseparator>
    {
        private _valueⲻseparatorTranscriber()
        {
        }
        
        public static _valueⲻseparatorTranscriber Instance { get; } = new _valueⲻseparatorTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._valueⲻseparator value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdata.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdata.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);

        }
    }
    
}

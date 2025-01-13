namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _COMMA_keyValuePairTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._COMMA_keyValuePair>
    {
        private _COMMA_keyValuePairTranscriber()
        {
        }
        
        public static _COMMA_keyValuePairTranscriber Instance { get; } = new _COMMA_keyValuePairTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._COMMA_keyValuePair value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdata.Trancsribers.Rules._keyValuePairTranscriber.Instance.Transcribe(value._keyValuePair_1, builder);

        }
    }
    
}

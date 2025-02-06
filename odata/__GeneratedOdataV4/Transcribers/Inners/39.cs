namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _COMMA_keyValuePairTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._COMMA_keyValuePair>
    {
        private _COMMA_keyValuePairTranscriber()
        {
        }
        
        public static _COMMA_keyValuePairTranscriber Instance { get; } = new _COMMA_keyValuePairTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._COMMA_keyValuePair value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._keyValuePairTranscriber.Instance.Transcribe(value._keyValuePair_1, builder);

        }
    }
    
}

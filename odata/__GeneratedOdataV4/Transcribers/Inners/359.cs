namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _valueⲻseparator_primitiveLiteralInJSONTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._valueⲻseparator_primitiveLiteralInJSON>
    {
        private _valueⲻseparator_primitiveLiteralInJSONTranscriber()
        {
        }
        
        public static _valueⲻseparator_primitiveLiteralInJSONTranscriber Instance { get; } = new _valueⲻseparator_primitiveLiteralInJSONTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._valueⲻseparator_primitiveLiteralInJSON value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._valueⲻseparatorTranscriber.Instance.Transcribe(value._valueⲻseparator_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._primitiveLiteralInJSONTranscriber.Instance.Transcribe(value._primitiveLiteralInJSON_1, builder);

        }
    }
    
}

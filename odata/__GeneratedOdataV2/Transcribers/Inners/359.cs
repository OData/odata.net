namespace __GeneratedOdataV2.Trancsribers.Inners
{
    public sealed class _valueⲻseparator_primitiveLiteralInJSONTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Inners._valueⲻseparator_primitiveLiteralInJSON>
    {
        private _valueⲻseparator_primitiveLiteralInJSONTranscriber()
        {
        }
        
        public static _valueⲻseparator_primitiveLiteralInJSONTranscriber Instance { get; } = new _valueⲻseparator_primitiveLiteralInJSONTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Inners._valueⲻseparator_primitiveLiteralInJSON value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._valueⲻseparatorTranscriber.Instance.Transcribe(value._valueⲻseparator_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._primitiveLiteralInJSONTranscriber.Instance.Transcribe(value._primitiveLiteralInJSON_1, builder);

        }
    }
    
}

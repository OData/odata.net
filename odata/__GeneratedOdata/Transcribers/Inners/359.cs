namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _valueⲻseparator_primitiveLiteralInJSONTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._valueⲻseparator_primitiveLiteralInJSON>
    {
        private _valueⲻseparator_primitiveLiteralInJSONTranscriber()
        {
        }
        
        public static _valueⲻseparator_primitiveLiteralInJSONTranscriber Instance { get; } = new _valueⲻseparator_primitiveLiteralInJSONTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._valueⲻseparator_primitiveLiteralInJSON value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._valueⲻseparatorTranscriber.Instance.Transcribe(value._valueⲻseparator_1, builder);
__GeneratedOdata.Trancsribers.Rules._primitiveLiteralInJSONTranscriber.Instance.Transcribe(value._primitiveLiteralInJSON_1, builder);

        }
    }
    
}

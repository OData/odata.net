namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _primitiveColInUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._primitiveColInUri>
    {
        private _primitiveColInUriTranscriber()
        {
        }
        
        public static _primitiveColInUriTranscriber Instance { get; } = new _primitiveColInUriTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._primitiveColInUri value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._beginⲻarrayTranscriber.Instance.Transcribe(value._beginⲻarray_1, builder);
if (value._primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ_1 != null)
{
__GeneratedOdataV4.Trancsribers.Inners._primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃTranscriber.Instance.Transcribe(value._primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ_1, builder);
}
__GeneratedOdataV4.Trancsribers.Rules._endⲻarrayTranscriber.Instance.Transcribe(value._endⲻarray_1, builder);

        }
    }
    
}

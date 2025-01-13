namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _primitiveColInUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._primitiveColInUri>
    {
        private _primitiveColInUriTranscriber()
        {
        }
        
        public static _primitiveColInUriTranscriber Instance { get; } = new _primitiveColInUriTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._primitiveColInUri value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._beginⲻarrayTranscriber.Instance.Transcribe(value._beginⲻarray_1, builder);
if (value._primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃTranscriber.Instance.Transcribe(value._primitiveLiteralInJSON_ЖⲤvalueⲻseparator_primitiveLiteralInJSONↃ_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._endⲻarrayTranscriber.Instance.Transcribe(value._endⲻarray_1, builder);

        }
    }
    
}

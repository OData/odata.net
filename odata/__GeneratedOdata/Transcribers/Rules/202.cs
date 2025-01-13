namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _primitivePropertyInUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._primitivePropertyInUri>
    {
        private _primitivePropertyInUriTranscriber()
        {
        }
        
        public static _primitivePropertyInUriTranscriber Instance { get; } = new _primitivePropertyInUriTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._primitivePropertyInUri value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_1, builder);
__GeneratedOdata.Trancsribers.Rules._primitivePropertyTranscriber.Instance.Transcribe(value._primitiveProperty_1, builder);
__GeneratedOdata.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_2, builder);
__GeneratedOdata.Trancsribers.Rules._nameⲻseparatorTranscriber.Instance.Transcribe(value._nameⲻseparator_1, builder);
__GeneratedOdata.Trancsribers.Rules._primitiveLiteralInJSONTranscriber.Instance.Transcribe(value._primitiveLiteralInJSON_1, builder);

        }
    }
    
}

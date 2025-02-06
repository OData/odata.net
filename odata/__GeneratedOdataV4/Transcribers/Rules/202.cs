namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _primitivePropertyInUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._primitivePropertyInUri>
    {
        private _primitivePropertyInUriTranscriber()
        {
        }
        
        public static _primitivePropertyInUriTranscriber Instance { get; } = new _primitivePropertyInUriTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._primitivePropertyInUri value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._primitivePropertyTranscriber.Instance.Transcribe(value._primitiveProperty_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._nameⲻseparatorTranscriber.Instance.Transcribe(value._nameⲻseparator_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._primitiveLiteralInJSONTranscriber.Instance.Transcribe(value._primitiveLiteralInJSON_1, builder);

        }
    }
    
}

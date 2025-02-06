namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUri>
    {
        private _quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriTranscriber()
        {
        }
        
        public static _quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriTranscriber Instance { get; } = new _quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUri value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._primitiveColPropertyTranscriber.Instance.Transcribe(value._primitiveColProperty_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._nameⲻseparatorTranscriber.Instance.Transcribe(value._nameⲻseparator_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._primitiveColInUriTranscriber.Instance.Transcribe(value._primitiveColInUri_1, builder);

        }
    }
    
}

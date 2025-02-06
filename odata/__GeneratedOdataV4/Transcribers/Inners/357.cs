namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUri>
    {
        private _quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriTranscriber()
        {
        }
        
        public static _quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriTranscriber Instance { get; } = new _quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUri value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._complexColPropertyTranscriber.Instance.Transcribe(value._complexColProperty_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._nameⲻseparatorTranscriber.Instance.Transcribe(value._nameⲻseparator_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._complexColInUriTranscriber.Instance.Transcribe(value._complexColInUri_1, builder);

        }
    }
    
}

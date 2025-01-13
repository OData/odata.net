namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUri>
    {
        private _quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriTranscriber()
        {
        }
        
        public static _quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriTranscriber Instance { get; } = new _quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUri value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_1, builder);
__GeneratedOdata.Trancsribers.Rules._complexColPropertyTranscriber.Instance.Transcribe(value._complexColProperty_1, builder);
__GeneratedOdata.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_2, builder);
__GeneratedOdata.Trancsribers.Rules._nameⲻseparatorTranscriber.Instance.Transcribe(value._nameⲻseparator_1, builder);
__GeneratedOdata.Trancsribers.Rules._complexColInUriTranscriber.Instance.Transcribe(value._complexColInUri_1, builder);

        }
    }
    
}

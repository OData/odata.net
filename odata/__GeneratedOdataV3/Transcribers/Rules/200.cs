namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _complexPropertyInUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._complexPropertyInUri>
    {
        private _complexPropertyInUriTranscriber()
        {
        }
        
        public static _complexPropertyInUriTranscriber Instance { get; } = new _complexPropertyInUriTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._complexPropertyInUri value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._complexPropertyTranscriber.Instance.Transcribe(value._complexProperty_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_2, builder);
__GeneratedOdataV3.Trancsribers.Rules._nameⲻseparatorTranscriber.Instance.Transcribe(value._nameⲻseparator_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._complexInUriTranscriber.Instance.Transcribe(value._complexInUri_1, builder);

        }
    }
    
}

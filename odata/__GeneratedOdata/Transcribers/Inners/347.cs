namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _valueⲻseparator_complexInUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._valueⲻseparator_complexInUri>
    {
        private _valueⲻseparator_complexInUriTranscriber()
        {
        }
        
        public static _valueⲻseparator_complexInUriTranscriber Instance { get; } = new _valueⲻseparator_complexInUriTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._valueⲻseparator_complexInUri value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._valueⲻseparatorTranscriber.Instance.Transcribe(value._valueⲻseparator_1, builder);
__GeneratedOdata.Trancsribers.Rules._complexInUriTranscriber.Instance.Transcribe(value._complexInUri_1, builder);

        }
    }
    
}

namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _valueⲻseparator_complexInUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._valueⲻseparator_complexInUri>
    {
        private _valueⲻseparator_complexInUriTranscriber()
        {
        }
        
        public static _valueⲻseparator_complexInUriTranscriber Instance { get; } = new _valueⲻseparator_complexInUriTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._valueⲻseparator_complexInUri value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._valueⲻseparatorTranscriber.Instance.Transcribe(value._valueⲻseparator_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._complexInUriTranscriber.Instance.Transcribe(value._complexInUri_1, builder);

        }
    }
    
}

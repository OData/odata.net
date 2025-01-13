namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _complexColInUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._complexColInUri>
    {
        private _complexColInUriTranscriber()
        {
        }
        
        public static _complexColInUriTranscriber Instance { get; } = new _complexColInUriTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._complexColInUri value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._beginⲻarrayTranscriber.Instance.Transcribe(value._beginⲻarray_1, builder);
if (value._complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._complexInUri_ЖⲤvalueⲻseparator_complexInUriↃTranscriber.Instance.Transcribe(value._complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._endⲻarrayTranscriber.Instance.Transcribe(value._endⲻarray_1, builder);

        }
    }
    
}

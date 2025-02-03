namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _complexInUri_ЖⲤvalueⲻseparator_complexInUriↃTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ>
    {
        private _complexInUri_ЖⲤvalueⲻseparator_complexInUriↃTranscriber()
        {
        }
        
        public static _complexInUri_ЖⲤvalueⲻseparator_complexInUriↃTranscriber Instance { get; } = new _complexInUri_ЖⲤvalueⲻseparator_complexInUriↃTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._complexInUri_ЖⲤvalueⲻseparator_complexInUriↃ value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._complexInUriTranscriber.Instance.Transcribe(value._complexInUri_1, builder);
foreach (var _Ⲥvalueⲻseparator_complexInUriↃ_1 in value._Ⲥvalueⲻseparator_complexInUriↃ_1)
{
Inners._Ⲥvalueⲻseparator_complexInUriↃTranscriber.Instance.Transcribe(_Ⲥvalueⲻseparator_complexInUriↃ_1, builder);
}

        }
    }
    
}

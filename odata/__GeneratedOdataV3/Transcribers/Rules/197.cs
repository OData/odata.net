namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _complexInUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._complexInUri>
    {
        private _complexInUriTranscriber()
        {
        }
        
        public static _complexInUriTranscriber Instance { get; } = new _complexInUriTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._complexInUri value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._beginⲻobjectTranscriber.Instance.Transcribe(value._beginⲻobject_1, builder);
if (value._ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃ_ЖⲤvalueⲻseparator_ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃↃ_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃ_ЖⲤvalueⲻseparator_ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃↃTranscriber.Instance.Transcribe(value._ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃ_ЖⲤvalueⲻseparator_ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃↃ_1, builder);
}
__GeneratedOdataV3.Trancsribers.Rules._endⲻobjectTranscriber.Instance.Transcribe(value._endⲻobject_1, builder);

        }
    }
    
}

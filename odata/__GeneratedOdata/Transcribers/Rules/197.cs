namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _complexInUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._complexInUri>
    {
        private _complexInUriTranscriber()
        {
        }
        
        public static _complexInUriTranscriber Instance { get; } = new _complexInUriTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._complexInUri value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._beginⲻobjectTranscriber.Instance.Transcribe(value._beginⲻobject_1, builder);
if (value._ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃ_ЖⲤvalueⲻseparator_ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃↃ_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃ_ЖⲤvalueⲻseparator_ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃↃTranscriber.Instance.Transcribe(value._ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃ_ЖⲤvalueⲻseparator_ⲤannotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriↃↃ_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._endⲻobjectTranscriber.Instance.Transcribe(value._endⲻobject_1, builder);

        }
    }
    
}

namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _annotationInUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._annotationInUri>
    {
        private _annotationInUriTranscriber()
        {
        }
        
        public static _annotationInUriTranscriber Instance { get; } = new _annotationInUriTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._annotationInUri value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_1, builder);
__GeneratedOdata.Trancsribers.Rules._ATTranscriber.Instance.Transcribe(value._AT_1, builder);
__GeneratedOdata.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._termNameTranscriber.Instance.Transcribe(value._termName_1, builder);
__GeneratedOdata.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_2, builder);
__GeneratedOdata.Trancsribers.Rules._nameⲻseparatorTranscriber.Instance.Transcribe(value._nameⲻseparator_1, builder);
__GeneratedOdata.Trancsribers.Inners._ⲤcomplexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriↃTranscriber.Instance.Transcribe(value._ⲤcomplexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriↃ_1, builder);

        }
    }
    
}

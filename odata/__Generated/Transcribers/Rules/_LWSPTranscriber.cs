namespace __Generated.Trancsribers.Rules
{
    public sealed class _LWSPTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._LWSP>
    {
        private _LWSPTranscriber()
        {
        }
        
        public static _LWSPTranscriber Instance { get; } = new _LWSPTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._LWSP value, System.Text.StringBuilder builder)
        {
            foreach (var _ⲤWSPⳆCRLF_WSPↃ_1 in value._ⲤWSPⳆCRLF_WSPↃ_1)
{
Inners._ⲤWSPⳆCRLF_WSPↃTranscriber.Instance.Transcribe(_ⲤWSPⳆCRLF_WSPↃ_1, builder);
}

        }
    }
    
}

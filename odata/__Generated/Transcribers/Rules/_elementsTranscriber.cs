namespace __Generated.Trancsribers.Rules
{
    public sealed class _elementsTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._elements>
    {
        private _elementsTranscriber()
        {
        }
        
        public static _elementsTranscriber Instance { get; } = new _elementsTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._elements value, System.Text.StringBuilder builder)
        {
            __Generated.Trancsribers.Rules._alternationTranscriber.Instance.Transcribe(value._alternation_1, builder);
foreach (var _cⲻwsp_1 in value._cⲻwsp_1)
{
__Generated.Trancsribers.Rules._cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp_1, builder);
}

        }
    }
    
}

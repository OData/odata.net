namespace __Generated.Trancsribers.Rules
{
    public sealed class _groupTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._group>
    {
        private _groupTranscriber()
        {
        }
        
        public static _groupTranscriber Instance { get; } = new _groupTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._group value, System.Text.StringBuilder builder)
        {
            __Generated.Trancsribers.Inners._ʺx28ʺTranscriber.Instance.Transcribe(value._ʺx28ʺ_1, builder);
foreach (var _cⲻwsp_1 in value._cⲻwsp_1)
{
__Generated.Trancsribers.Rules._cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp_1, builder);
}
__Generated.Trancsribers.Rules._alternationTranscriber.Instance.Transcribe(value._alternation_1, builder);
foreach (var _cⲻwsp_2 in value._cⲻwsp_2)
{
__Generated.Trancsribers.Rules._cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp_2, builder);
}
__Generated.Trancsribers.Inners._ʺx29ʺTranscriber.Instance.Transcribe(value._ʺx29ʺ_1, builder);

        }
    }
    
}

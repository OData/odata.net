namespace __Generated.Trancsribers.Rules
{
    public sealed class _optionTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._option>
    {
        private _optionTranscriber()
        {
        }
        
        public static _optionTranscriber Instance { get; } = new _optionTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._option value, System.Text.StringBuilder builder)
        {
            __Generated.Trancsribers.Inners._ʺx5BʺTranscriber.Instance.Transcribe(value._ʺx5Bʺ_1, builder);
foreach (var _cⲻwsp_1 in value._cⲻwsp_1)
{
__Generated.Trancsribers.Rules._cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp_1, builder);
}
__Generated.Trancsribers.Rules._alternationTranscriber.Instance.Transcribe(value._alternation_1, builder);
foreach (var _cⲻwsp_2 in value._cⲻwsp_2)
{
__Generated.Trancsribers.Rules._cⲻwspTranscriber.Instance.Transcribe(_cⲻwsp_2, builder);
}
__Generated.Trancsribers.Inners._ʺx5DʺTranscriber.Instance.Transcribe(value._ʺx5Dʺ_1, builder);

        }
    }
    
}

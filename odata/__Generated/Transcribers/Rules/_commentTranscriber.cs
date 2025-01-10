namespace __Generated.Trancsribers.Rules
{
    public sealed class _commentTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._comment>
    {
        private _commentTranscriber()
        {
        }
        
        public static _commentTranscriber Instance { get; } = new _commentTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._comment value, System.Text.StringBuilder builder)
        {
            __Generated.Trancsribers.Inners._ʺx3BʺTranscriber.Instance.Transcribe(value._ʺx3Bʺ_1, builder);
foreach (var _ⲤWSPⳆVCHARↃ_1 in value._ⲤWSPⳆVCHARↃ_1)
{
__Generated.Trancsribers.Inners._ⲤWSPⳆVCHARↃTranscriber.Instance.Transcribe(_ⲤWSPⳆVCHARↃ_1, builder);
}
__Generated.Trancsribers.Rules._CRLFTranscriber.Instance.Transcribe(value._CRLF_1, builder);

        }
    }
    
}

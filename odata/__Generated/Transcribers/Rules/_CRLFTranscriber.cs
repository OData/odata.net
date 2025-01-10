namespace __Generated.Trancsribers.Rules
{
    public sealed class _CRLFTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._CRLF>
    {
        private _CRLFTranscriber()
        {
        }
        
        public static _CRLFTranscriber Instance { get; } = new _CRLFTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._CRLF value, System.Text.StringBuilder builder)
        {
            __Generated.Trancsribers.Rules._CRTranscriber.Instance.Transcribe(value._CR_1, builder);
__Generated.Trancsribers.Rules._LFTranscriber.Instance.Transcribe(value._LF_1, builder);

        }
    }
    
}

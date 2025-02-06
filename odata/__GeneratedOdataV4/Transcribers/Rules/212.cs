namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _nameⲻseparatorTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._nameⲻseparator>
    {
        private _nameⲻseparatorTranscriber()
        {
        }
        
        public static _nameⲻseparatorTranscriber Instance { get; } = new _nameⲻseparatorTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._nameⲻseparator value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._COLONTranscriber.Instance.Transcribe(value._COLON_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_2, builder);

        }
    }
    
}

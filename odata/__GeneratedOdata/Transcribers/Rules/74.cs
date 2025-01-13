namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _skipTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._skip>
    {
        private _skipTranscriber()
        {
        }
        
        public static _skipTranscriber Instance { get; } = new _skipTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._skip value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._Ⲥʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺↃTranscriber.Instance.Transcribe(value._Ⲥʺx24x73x6Bx69x70ʺⳆʺx73x6Bx69x70ʺↃ_1, builder);
__GeneratedOdata.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdata.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}

        }
    }
    
}

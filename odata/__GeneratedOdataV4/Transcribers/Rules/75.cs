namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _topTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._top>
    {
        private _topTranscriber()
        {
        }
        
        public static _topTranscriber Instance { get; } = new _topTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._top value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Inners._Ⲥʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺↃTranscriber.Instance.Transcribe(value._Ⲥʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺↃ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdataV4.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}

        }
    }
    
}

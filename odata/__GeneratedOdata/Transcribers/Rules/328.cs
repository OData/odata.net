namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _sridLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._sridLiteral>
    {
        private _sridLiteralTranscriber()
        {
        }
        
        public static _sridLiteralTranscriber Instance { get; } = new _sridLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._sridLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx53x52x49x44ʺTranscriber.Instance.Transcribe(value._ʺx53x52x49x44ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdata.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._SEMITranscriber.Instance.Transcribe(value._SEMI_1, builder);

        }
    }
    
}

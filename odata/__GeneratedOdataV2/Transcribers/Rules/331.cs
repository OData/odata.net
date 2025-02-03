namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _positionLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._positionLiteral>
    {
        private _positionLiteralTranscriber()
        {
        }
        
        public static _positionLiteralTranscriber Instance { get; } = new _positionLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._positionLiteral value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._doubleValueTranscriber.Instance.Transcribe(value._doubleValue_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._SPTranscriber.Instance.Transcribe(value._SP_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._doubleValueTranscriber.Instance.Transcribe(value._doubleValue_2, builder);

        }
    }
    
}

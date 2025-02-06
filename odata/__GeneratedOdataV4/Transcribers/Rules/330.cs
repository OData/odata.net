namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _pointDataTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._pointData>
    {
        private _pointDataTranscriber()
        {
        }
        
        public static _pointDataTranscriber Instance { get; } = new _pointDataTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._pointData value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._positionLiteralTranscriber.Instance.Transcribe(value._positionLiteral_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

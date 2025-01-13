namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _pointDataTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._pointData>
    {
        private _pointDataTranscriber()
        {
        }
        
        public static _pointDataTranscriber Instance { get; } = new _pointDataTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._pointData value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdata.Trancsribers.Rules._positionLiteralTranscriber.Instance.Transcribe(value._positionLiteral_1, builder);
__GeneratedOdata.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

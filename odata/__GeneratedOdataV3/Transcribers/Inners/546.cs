namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _COMMA_pointDataTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._COMMA_pointData>
    {
        private _COMMA_pointDataTranscriber()
        {
        }
        
        public static _COMMA_pointDataTranscriber Instance { get; } = new _COMMA_pointDataTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._COMMA_pointData value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._pointDataTranscriber.Instance.Transcribe(value._pointData_1, builder);

        }
    }
    
}

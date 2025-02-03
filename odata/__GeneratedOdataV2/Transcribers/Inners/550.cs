namespace __GeneratedOdataV2.Trancsribers.Inners
{
    public sealed class _COMMA_polygonDataTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Inners._COMMA_polygonData>
    {
        private _COMMA_polygonDataTranscriber()
        {
        }
        
        public static _COMMA_polygonDataTranscriber Instance { get; } = new _COMMA_polygonDataTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Inners._COMMA_polygonData value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._polygonDataTranscriber.Instance.Transcribe(value._polygonData_1, builder);

        }
    }
    
}

namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _COMMA_polygonDataTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._COMMA_polygonData>
    {
        private _COMMA_polygonDataTranscriber()
        {
        }
        
        public static _COMMA_polygonDataTranscriber Instance { get; } = new _COMMA_polygonDataTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._COMMA_polygonData value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdata.Trancsribers.Rules._polygonDataTranscriber.Instance.Transcribe(value._polygonData_1, builder);

        }
    }
    
}

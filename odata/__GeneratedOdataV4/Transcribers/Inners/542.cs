namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _COMMA_lineStringDataTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._COMMA_lineStringData>
    {
        private _COMMA_lineStringDataTranscriber()
        {
        }
        
        public static _COMMA_lineStringDataTranscriber Instance { get; } = new _COMMA_lineStringDataTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._COMMA_lineStringData value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._lineStringDataTranscriber.Instance.Transcribe(value._lineStringData_1, builder);

        }
    }
    
}

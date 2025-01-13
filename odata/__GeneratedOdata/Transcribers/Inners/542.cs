namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _COMMA_lineStringDataTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._COMMA_lineStringData>
    {
        private _COMMA_lineStringDataTranscriber()
        {
        }
        
        public static _COMMA_lineStringDataTranscriber Instance { get; } = new _COMMA_lineStringDataTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._COMMA_lineStringData value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdata.Trancsribers.Rules._lineStringDataTranscriber.Instance.Transcribe(value._lineStringData_1, builder);

        }
    }
    
}

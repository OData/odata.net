namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _COMMA_expandItemTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._COMMA_expandItem>
    {
        private _COMMA_expandItemTranscriber()
        {
        }
        
        public static _COMMA_expandItemTranscriber Instance { get; } = new _COMMA_expandItemTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._COMMA_expandItem value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._expandItemTranscriber.Instance.Transcribe(value._expandItem_1, builder);

        }
    }
    
}

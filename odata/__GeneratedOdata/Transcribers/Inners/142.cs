namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _COMMA_orderbyItemTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._COMMA_orderbyItem>
    {
        private _COMMA_orderbyItemTranscriber()
        {
        }
        
        public static _COMMA_orderbyItemTranscriber Instance { get; } = new _COMMA_orderbyItemTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._COMMA_orderbyItem value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdata.Trancsribers.Rules._orderbyItemTranscriber.Instance.Transcribe(value._orderbyItem_1, builder);

        }
    }
    
}

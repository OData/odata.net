namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _COMMA_orderbyItemTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._COMMA_orderbyItem>
    {
        private _COMMA_orderbyItemTranscriber()
        {
        }
        
        public static _COMMA_orderbyItemTranscriber Instance { get; } = new _COMMA_orderbyItemTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._COMMA_orderbyItem value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._orderbyItemTranscriber.Instance.Transcribe(value._orderbyItem_1, builder);

        }
    }
    
}

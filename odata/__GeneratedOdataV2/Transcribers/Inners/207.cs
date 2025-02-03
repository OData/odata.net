namespace __GeneratedOdataV2.Trancsribers.Inners
{
    public sealed class _COMMA_selectItemTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Inners._COMMA_selectItem>
    {
        private _COMMA_selectItemTranscriber()
        {
        }
        
        public static _COMMA_selectItemTranscriber Instance { get; } = new _COMMA_selectItemTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Inners._COMMA_selectItem value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._selectItemTranscriber.Instance.Transcribe(value._selectItem_1, builder);

        }
    }
    
}

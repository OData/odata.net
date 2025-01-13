namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _COMMA_selectItemTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._COMMA_selectItem>
    {
        private _COMMA_selectItemTranscriber()
        {
        }
        
        public static _COMMA_selectItemTranscriber Instance { get; } = new _COMMA_selectItemTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._COMMA_selectItem value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdata.Trancsribers.Rules._selectItemTranscriber.Instance.Transcribe(value._selectItem_1, builder);

        }
    }
    
}

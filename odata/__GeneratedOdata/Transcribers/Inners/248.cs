namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _COMMA_selectListItemTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._COMMA_selectListItem>
    {
        private _COMMA_selectListItemTranscriber()
        {
        }
        
        public static _COMMA_selectListItemTranscriber Instance { get; } = new _COMMA_selectListItemTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._COMMA_selectListItem value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdata.Trancsribers.Rules._selectListItemTranscriber.Instance.Transcribe(value._selectListItem_1, builder);

        }
    }
    
}

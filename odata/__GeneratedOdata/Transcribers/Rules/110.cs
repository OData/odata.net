namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _selectListTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._selectList>
    {
        private _selectListTranscriber()
        {
        }
        
        public static _selectListTranscriber Instance { get; } = new _selectListTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._selectList value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdata.Trancsribers.Rules._selectListItemTranscriber.Instance.Transcribe(value._selectListItem_1, builder);
foreach (var _ⲤCOMMA_selectListItemↃ_1 in value._ⲤCOMMA_selectListItemↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._ⲤCOMMA_selectListItemↃTranscriber.Instance.Transcribe(_ⲤCOMMA_selectListItemↃ_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}
namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _selectTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._select>
    {
        private _selectTranscriber()
        {
        }
        
        public static _selectTranscriber Instance { get; } = new _selectTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._select value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._Ⲥʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺↃTranscriber.Instance.Transcribe(value._Ⲥʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺↃ_1, builder);
__GeneratedOdata.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
__GeneratedOdata.Trancsribers.Rules._selectItemTranscriber.Instance.Transcribe(value._selectItem_1, builder);
foreach (var _ⲤCOMMA_selectItemↃ_1 in value._ⲤCOMMA_selectItemↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._ⲤCOMMA_selectItemↃTranscriber.Instance.Transcribe(_ⲤCOMMA_selectItemↃ_1, builder);
}

        }
    }
    
}

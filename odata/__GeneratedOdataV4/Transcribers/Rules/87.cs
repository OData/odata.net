namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _selectTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._select>
    {
        private _selectTranscriber()
        {
        }
        
        public static _selectTranscriber Instance { get; } = new _selectTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._select value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Inners._Ⲥʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺↃTranscriber.Instance.Transcribe(value._Ⲥʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺↃ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._selectItemTranscriber.Instance.Transcribe(value._selectItem_1, builder);
foreach (var _ⲤCOMMA_selectItemↃ_1 in value._ⲤCOMMA_selectItemↃ_1)
{
Inners._ⲤCOMMA_selectItemↃTranscriber.Instance.Transcribe(_ⲤCOMMA_selectItemↃ_1, builder);
}

        }
    }
    
}

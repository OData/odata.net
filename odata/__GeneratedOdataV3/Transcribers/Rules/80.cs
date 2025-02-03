namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _searchTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._search>
    {
        private _searchTranscriber()
        {
        }
        
        public static _searchTranscriber Instance { get; } = new _searchTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._search value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Inners._Ⲥʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺↃTranscriber.Instance.Transcribe(value._Ⲥʺx24x73x65x61x72x63x68ʺⳆʺx73x65x61x72x63x68ʺↃ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._BWSTranscriber.Instance.Transcribe(value._BWS_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._searchExprTranscriber.Instance.Transcribe(value._searchExpr_1, builder);

        }
    }
    
}

namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _addExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._addExpr>
    {
        private _addExprTranscriber()
        {
        }
        
        public static _addExprTranscriber Instance { get; } = new _addExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._addExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdataV2.Trancsribers.Inners._ʺx61x64x64ʺTranscriber.Instance.Transcribe(value._ʺx61x64x64ʺ_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdataV2.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}

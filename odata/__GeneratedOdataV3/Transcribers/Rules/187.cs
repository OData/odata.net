namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _mulExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._mulExpr>
    {
        private _mulExprTranscriber()
        {
        }
        
        public static _mulExprTranscriber Instance { get; } = new _mulExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._mulExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdataV3.Trancsribers.Inners._ʺx6Dx75x6CʺTranscriber.Instance.Transcribe(value._ʺx6Dx75x6Cʺ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdataV3.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}

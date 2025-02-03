namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _gtExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._gtExpr>
    {
        private _gtExprTranscriber()
        {
        }
        
        public static _gtExprTranscriber Instance { get; } = new _gtExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._gtExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdataV3.Trancsribers.Inners._ʺx67x74ʺTranscriber.Instance.Transcribe(value._ʺx67x74ʺ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdataV3.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}

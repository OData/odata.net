namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _subExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._subExpr>
    {
        private _subExprTranscriber()
        {
        }
        
        public static _subExprTranscriber Instance { get; } = new _subExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._subExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_1, builder);
__GeneratedOdataV4.Trancsribers.Inners._ʺx73x75x62ʺTranscriber.Instance.Transcribe(value._ʺx73x75x62ʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._RWSTranscriber.Instance.Transcribe(value._RWS_2, builder);
__GeneratedOdataV4.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}

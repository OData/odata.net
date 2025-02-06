namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _boolCommonExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._boolCommonExpr>
    {
        private _boolCommonExprTranscriber()
        {
        }
        
        public static _boolCommonExprTranscriber Instance { get; } = new _boolCommonExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._boolCommonExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}

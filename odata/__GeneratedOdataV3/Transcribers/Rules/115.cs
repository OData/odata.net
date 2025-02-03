namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _boolCommonExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._boolCommonExpr>
    {
        private _boolCommonExprTranscriber()
        {
        }
        
        public static _boolCommonExprTranscriber Instance { get; } = new _boolCommonExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._boolCommonExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._commonExprTranscriber.Instance.Transcribe(value._commonExpr_1, builder);

        }
    }
    
}

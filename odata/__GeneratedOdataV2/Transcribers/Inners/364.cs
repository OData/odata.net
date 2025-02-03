namespace __GeneratedOdataV2.Trancsribers.Inners
{
    public sealed class _valueⲻseparator_rootExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Inners._valueⲻseparator_rootExpr>
    {
        private _valueⲻseparator_rootExprTranscriber()
        {
        }
        
        public static _valueⲻseparator_rootExprTranscriber Instance { get; } = new _valueⲻseparator_rootExprTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Inners._valueⲻseparator_rootExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._valueⲻseparatorTranscriber.Instance.Transcribe(value._valueⲻseparator_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._rootExprTranscriber.Instance.Transcribe(value._rootExpr_1, builder);

        }
    }
    
}

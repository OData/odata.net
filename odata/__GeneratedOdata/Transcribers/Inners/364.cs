namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _valueⲻseparator_rootExprTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._valueⲻseparator_rootExpr>
    {
        private _valueⲻseparator_rootExprTranscriber()
        {
        }
        
        public static _valueⲻseparator_rootExprTranscriber Instance { get; } = new _valueⲻseparator_rootExprTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._valueⲻseparator_rootExpr value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._valueⲻseparatorTranscriber.Instance.Transcribe(value._valueⲻseparator_1, builder);
__GeneratedOdata.Trancsribers.Rules._rootExprTranscriber.Instance.Transcribe(value._rootExpr_1, builder);

        }
    }
    
}

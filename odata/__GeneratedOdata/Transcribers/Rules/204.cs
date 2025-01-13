namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _singleNavPropInJSONTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._singleNavPropInJSON>
    {
        private _singleNavPropInJSONTranscriber()
        {
        }
        
        public static _singleNavPropInJSONTranscriber Instance { get; } = new _singleNavPropInJSONTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._singleNavPropInJSON value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_1, builder);
__GeneratedOdata.Trancsribers.Rules._entityNavigationPropertyTranscriber.Instance.Transcribe(value._entityNavigationProperty_1, builder);
__GeneratedOdata.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_2, builder);
__GeneratedOdata.Trancsribers.Rules._nameⲻseparatorTranscriber.Instance.Transcribe(value._nameⲻseparator_1, builder);
__GeneratedOdata.Trancsribers.Rules._rootExprTranscriber.Instance.Transcribe(value._rootExpr_1, builder);

        }
    }
    
}

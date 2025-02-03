namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _collectionNavPropInJSONTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._collectionNavPropInJSON>
    {
        private _collectionNavPropInJSONTranscriber()
        {
        }
        
        public static _collectionNavPropInJSONTranscriber Instance { get; } = new _collectionNavPropInJSONTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._collectionNavPropInJSON value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._entityColNavigationPropertyTranscriber.Instance.Transcribe(value._entityColNavigationProperty_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_2, builder);
__GeneratedOdataV3.Trancsribers.Rules._nameⲻseparatorTranscriber.Instance.Transcribe(value._nameⲻseparator_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._rootExprColTranscriber.Instance.Transcribe(value._rootExprCol_1, builder);

        }
    }
    
}

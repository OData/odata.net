namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _entityColNavigationPropertyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._entityColNavigationProperty>
    {
        private _entityColNavigationPropertyTranscriber()
        {
        }
        
        public static _entityColNavigationPropertyTranscriber Instance { get; } = new _entityColNavigationPropertyTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._entityColNavigationProperty value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}

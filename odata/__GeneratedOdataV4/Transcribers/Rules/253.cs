namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _entityNavigationPropertyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._entityNavigationProperty>
    {
        private _entityNavigationPropertyTranscriber()
        {
        }
        
        public static _entityNavigationPropertyTranscriber Instance { get; } = new _entityNavigationPropertyTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._entityNavigationProperty value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}

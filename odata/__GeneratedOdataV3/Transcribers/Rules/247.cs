namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _primitiveNonKeyPropertyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._primitiveNonKeyProperty>
    {
        private _primitiveNonKeyPropertyTranscriber()
        {
        }
        
        public static _primitiveNonKeyPropertyTranscriber Instance { get; } = new _primitiveNonKeyPropertyTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._primitiveNonKeyProperty value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}

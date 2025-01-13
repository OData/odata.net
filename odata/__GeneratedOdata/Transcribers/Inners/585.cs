namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _ʺx23ʺ_odataIdentifierTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._ʺx23ʺ_odataIdentifier>
    {
        private _ʺx23ʺ_odataIdentifierTranscriber()
        {
        }
        
        public static _ʺx23ʺ_odataIdentifierTranscriber Instance { get; } = new _ʺx23ʺ_odataIdentifierTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._ʺx23ʺ_odataIdentifier value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx23ʺTranscriber.Instance.Transcribe(value._ʺx23ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._odataIdentifierTranscriber.Instance.Transcribe(value._odataIdentifier_1, builder);

        }
    }
    
}

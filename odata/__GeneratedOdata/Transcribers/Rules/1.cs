namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _odataUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._odataUri>
    {
        private _odataUriTranscriber()
        {
        }
        
        public static _odataUriTranscriber Instance { get; } = new _odataUriTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._odataUri value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._serviceRootTranscriber.Instance.Transcribe(value._serviceRoot_1, builder);
if (value._odataRelativeUri_1 != null)
{
__GeneratedOdata.Trancsribers.Rules._odataRelativeUriTranscriber.Instance.Transcribe(value._odataRelativeUri_1, builder);
}

        }
    }
    
}

namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _odataUriTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._odataUri>
    {
        private _odataUriTranscriber()
        {
        }
        
        public static _odataUriTranscriber Instance { get; } = new _odataUriTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._odataUri value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._serviceRootTranscriber.Instance.Transcribe(value._serviceRoot_1, builder);
if (value._odataRelativeUri_1 != null)
{
__GeneratedOdataV3.Trancsribers.Rules._odataRelativeUriTranscriber.Instance.Transcribe(value._odataRelativeUri_1, builder);
}

        }
    }
    
}

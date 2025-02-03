namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _requestⲻidTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._requestⲻid>
    {
        private _requestⲻidTranscriber()
        {
        }
        
        public static _requestⲻidTranscriber Instance { get; } = new _requestⲻidTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._requestⲻid value, System.Text.StringBuilder builder)
        {
            foreach (var _unreserved_1 in value._unreserved_1)
{
__GeneratedOdataV3.Trancsribers.Rules._unreservedTranscriber.Instance.Transcribe(_unreserved_1, builder);
}

        }
    }
    
}

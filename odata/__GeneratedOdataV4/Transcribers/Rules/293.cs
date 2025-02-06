namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _durationTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._duration>
    {
        private _durationTranscriber()
        {
        }
        
        public static _durationTranscriber Instance { get; } = new _durationTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._duration value, System.Text.StringBuilder builder)
        {
            if (value._ʺx64x75x72x61x74x69x6Fx6Eʺ_1 != null)
{
__GeneratedOdataV4.Trancsribers.Inners._ʺx64x75x72x61x74x69x6Fx6EʺTranscriber.Instance.Transcribe(value._ʺx64x75x72x61x74x69x6Fx6Eʺ_1, builder);
}
__GeneratedOdataV4.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._durationValueTranscriber.Instance.Transcribe(value._durationValue_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

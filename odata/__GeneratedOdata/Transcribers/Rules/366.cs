namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _waitPreferenceTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._waitPreference>
    {
        private _waitPreferenceTranscriber()
        {
        }
        
        public static _waitPreferenceTranscriber Instance { get; } = new _waitPreferenceTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._waitPreference value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx77x61x69x74ʺTranscriber.Instance.Transcribe(value._ʺx77x61x69x74ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._EQⲻhTranscriber.Instance.Transcribe(value._EQⲻh_1, builder);
foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdata.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}

        }
    }
    
}

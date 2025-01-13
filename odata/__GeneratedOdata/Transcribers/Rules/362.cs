namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _maxpagesizePreferenceTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._maxpagesizePreference>
    {
        private _maxpagesizePreferenceTranscriber()
        {
        }
        
        public static _maxpagesizePreferenceTranscriber Instance { get; } = new _maxpagesizePreferenceTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._maxpagesizePreference value, System.Text.StringBuilder builder)
        {
            if (value._ʺx6Fx64x61x74x61x2Eʺ_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx6Fx64x61x74x61x2EʺTranscriber.Instance.Transcribe(value._ʺx6Fx64x61x74x61x2Eʺ_1, builder);
}
__GeneratedOdata.Trancsribers.Inners._ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺTranscriber.Instance.Transcribe(value._ʺx6Dx61x78x70x61x67x65x73x69x7Ax65ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._EQⲻhTranscriber.Instance.Transcribe(value._EQⲻh_1, builder);
__GeneratedOdata.Trancsribers.Rules._oneToNineTranscriber.Instance.Transcribe(value._oneToNine_1, builder);
foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdata.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}

        }
    }
    
}

namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _COMMA_preferenceTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._COMMA_preference>
    {
        private _COMMA_preferenceTranscriber()
        {
        }
        
        public static _COMMA_preferenceTranscriber Instance { get; } = new _COMMA_preferenceTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._COMMA_preference value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._COMMATranscriber.Instance.Transcribe(value._COMMA_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._preferenceTranscriber.Instance.Transcribe(value._preference_1, builder);

        }
    }
    
}

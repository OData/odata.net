namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _SEMI_expandRefOptionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._SEMI_expandRefOption>
    {
        private _SEMI_expandRefOptionTranscriber()
        {
        }
        
        public static _SEMI_expandRefOptionTranscriber Instance { get; } = new _SEMI_expandRefOptionTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._SEMI_expandRefOption value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._SEMITranscriber.Instance.Transcribe(value._SEMI_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._expandRefOptionTranscriber.Instance.Transcribe(value._expandRefOption_1, builder);

        }
    }
    
}

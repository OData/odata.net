namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _SEMI_expandRefOptionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._SEMI_expandRefOption>
    {
        private _SEMI_expandRefOptionTranscriber()
        {
        }
        
        public static _SEMI_expandRefOptionTranscriber Instance { get; } = new _SEMI_expandRefOptionTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._SEMI_expandRefOption value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._SEMITranscriber.Instance.Transcribe(value._SEMI_1, builder);
__GeneratedOdata.Trancsribers.Rules._expandRefOptionTranscriber.Instance.Transcribe(value._expandRefOption_1, builder);

        }
    }
    
}

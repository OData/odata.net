namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSETranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE>
    {
        private _OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSETranscriber()
        {
        }
        
        public static _OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSETranscriber Instance { get; } = new _OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSETranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdata.Trancsribers.Rules._expandRefOptionTranscriber.Instance.Transcribe(value._expandRefOption_1, builder);
foreach (var _ⲤSEMI_expandRefOptionↃ_1 in value._ⲤSEMI_expandRefOptionↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._ⲤSEMI_expandRefOptionↃTranscriber.Instance.Transcribe(_ⲤSEMI_expandRefOptionↃ_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}
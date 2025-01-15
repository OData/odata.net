namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSETranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE>
    {
        private _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSETranscriber()
        {
        }
        
        public static _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSETranscriber Instance { get; } = new _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSETranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdata.Trancsribers.Rules._expandCountOptionTranscriber.Instance.Transcribe(value._expandCountOption_1, builder);
foreach (var _ⲤSEMI_expandCountOptionↃ_1 in value._ⲤSEMI_expandCountOptionↃ_1)
{
Inners._ⲤSEMI_expandCountOptionↃTranscriber.Instance.Transcribe(_ⲤSEMI_expandCountOptionↃ_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSETranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE>
    {
        private _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSETranscriber()
        {
        }
        
        public static _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSETranscriber Instance { get; } = new _OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSETranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._expandCountOptionTranscriber.Instance.Transcribe(value._expandCountOption_1, builder);
foreach (var _ⲤSEMI_expandCountOptionↃ_1 in value._ⲤSEMI_expandCountOptionↃ_1)
{
Inners._ⲤSEMI_expandCountOptionↃTranscriber.Instance.Transcribe(_ⲤSEMI_expandCountOptionↃ_1, builder);
}
__GeneratedOdataV4.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

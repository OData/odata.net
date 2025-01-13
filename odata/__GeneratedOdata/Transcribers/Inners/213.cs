namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSETranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE>
    {
        private _OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSETranscriber()
        {
        }
        
        public static _OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSETranscriber Instance { get; } = new _OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSETranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._OPEN_selectOptionPC_ЖⲤSEMI_selectOptionPCↃ_CLOSE value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdata.Trancsribers.Rules._selectOptionPCTranscriber.Instance.Transcribe(value._selectOptionPC_1, builder);
foreach (var _ⲤSEMI_selectOptionPCↃ_1 in value._ⲤSEMI_selectOptionPCↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._ⲤSEMI_selectOptionPCↃTranscriber.Instance.Transcribe(_ⲤSEMI_selectOptionPCↃ_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}

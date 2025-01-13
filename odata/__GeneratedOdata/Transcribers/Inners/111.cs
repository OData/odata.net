namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _SEMI_expandCountOptionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._SEMI_expandCountOption>
    {
        private _SEMI_expandCountOptionTranscriber()
        {
        }
        
        public static _SEMI_expandCountOptionTranscriber Instance { get; } = new _SEMI_expandCountOptionTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._SEMI_expandCountOption value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._SEMITranscriber.Instance.Transcribe(value._SEMI_1, builder);
__GeneratedOdata.Trancsribers.Rules._expandCountOptionTranscriber.Instance.Transcribe(value._expandCountOption_1, builder);

        }
    }
    
}

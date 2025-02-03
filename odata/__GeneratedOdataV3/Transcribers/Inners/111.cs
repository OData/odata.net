namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _SEMI_expandCountOptionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._SEMI_expandCountOption>
    {
        private _SEMI_expandCountOptionTranscriber()
        {
        }
        
        public static _SEMI_expandCountOptionTranscriber Instance { get; } = new _SEMI_expandCountOptionTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._SEMI_expandCountOption value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._SEMITranscriber.Instance.Transcribe(value._SEMI_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._expandCountOptionTranscriber.Instance.Transcribe(value._expandCountOption_1, builder);

        }
    }
    
}

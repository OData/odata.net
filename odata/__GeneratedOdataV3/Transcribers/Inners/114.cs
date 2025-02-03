namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _SEMI_expandOptionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._SEMI_expandOption>
    {
        private _SEMI_expandOptionTranscriber()
        {
        }
        
        public static _SEMI_expandOptionTranscriber Instance { get; } = new _SEMI_expandOptionTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._SEMI_expandOption value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._SEMITranscriber.Instance.Transcribe(value._SEMI_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._expandOptionTranscriber.Instance.Transcribe(value._expandOption_1, builder);

        }
    }
    
}

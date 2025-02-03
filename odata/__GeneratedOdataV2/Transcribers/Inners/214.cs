namespace __GeneratedOdataV2.Trancsribers.Inners
{
    public sealed class _SEMI_selectOptionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Inners._SEMI_selectOption>
    {
        private _SEMI_selectOptionTranscriber()
        {
        }
        
        public static _SEMI_selectOptionTranscriber Instance { get; } = new _SEMI_selectOptionTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Inners._SEMI_selectOption value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._SEMITranscriber.Instance.Transcribe(value._SEMI_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._selectOptionTranscriber.Instance.Transcribe(value._selectOption_1, builder);

        }
    }
    
}

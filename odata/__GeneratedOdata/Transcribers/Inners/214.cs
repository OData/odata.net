namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _SEMI_selectOptionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._SEMI_selectOption>
    {
        private _SEMI_selectOptionTranscriber()
        {
        }
        
        public static _SEMI_selectOptionTranscriber Instance { get; } = new _SEMI_selectOptionTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._SEMI_selectOption value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._SEMITranscriber.Instance.Transcribe(value._SEMI_1, builder);
__GeneratedOdata.Trancsribers.Rules._selectOptionTranscriber.Instance.Transcribe(value._selectOption_1, builder);

        }
    }
    
}

namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _SEMI_selectOptionPCTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._SEMI_selectOptionPC>
    {
        private _SEMI_selectOptionPCTranscriber()
        {
        }
        
        public static _SEMI_selectOptionPCTranscriber Instance { get; } = new _SEMI_selectOptionPCTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._SEMI_selectOptionPC value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._SEMITranscriber.Instance.Transcribe(value._SEMI_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._selectOptionPCTranscriber.Instance.Transcribe(value._selectOptionPC_1, builder);

        }
    }
    
}

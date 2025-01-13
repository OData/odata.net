namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _1ЖDIGIT_ʺx4DʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._1ЖDIGIT_ʺx4Dʺ>
    {
        private _1ЖDIGIT_ʺx4DʺTranscriber()
        {
        }
        
        public static _1ЖDIGIT_ʺx4DʺTranscriber Instance { get; } = new _1ЖDIGIT_ʺx4DʺTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._1ЖDIGIT_ʺx4Dʺ value, System.Text.StringBuilder builder)
        {
            foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdata.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}
__GeneratedOdata.Trancsribers.Inners._ʺx4DʺTranscriber.Instance.Transcribe(value._ʺx4Dʺ_1, builder);

        }
    }
    
}

namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _1ЖDIGIT_ʺx48ʺTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._1ЖDIGIT_ʺx48ʺ>
    {
        private _1ЖDIGIT_ʺx48ʺTranscriber()
        {
        }
        
        public static _1ЖDIGIT_ʺx48ʺTranscriber Instance { get; } = new _1ЖDIGIT_ʺx48ʺTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._1ЖDIGIT_ʺx48ʺ value, System.Text.StringBuilder builder)
        {
            foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdata.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}
__GeneratedOdata.Trancsribers.Inners._ʺx48ʺTranscriber.Instance.Transcribe(value._ʺx48ʺ_1, builder);

        }
    }
    
}

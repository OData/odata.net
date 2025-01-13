namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _ordinalIndexTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._ordinalIndex>
    {
        private _ordinalIndexTranscriber()
        {
        }
        
        public static _ordinalIndexTranscriber Instance { get; } = new _ordinalIndexTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._ordinalIndex value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(value._ʺx2Fʺ_1, builder);
foreach (var _DIGIT_1 in value._DIGIT_1)
{
__GeneratedOdata.Trancsribers.Rules._DIGITTranscriber.Instance.Transcribe(_DIGIT_1, builder);
}

        }
    }
    
}

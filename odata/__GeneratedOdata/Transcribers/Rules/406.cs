namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _pctⲻencodedTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._pctⲻencoded>
    {
        private _pctⲻencodedTranscriber()
        {
        }
        
        public static _pctⲻencodedTranscriber Instance { get; } = new _pctⲻencodedTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._pctⲻencoded value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx25ʺTranscriber.Instance.Transcribe(value._ʺx25ʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._HEXDIGTranscriber.Instance.Transcribe(value._HEXDIG_1, builder);
__GeneratedOdata.Trancsribers.Rules._HEXDIGTranscriber.Instance.Transcribe(value._HEXDIG_2, builder);

        }
    }
    
}

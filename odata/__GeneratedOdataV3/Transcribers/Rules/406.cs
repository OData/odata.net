namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _pctⲻencodedTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._pctⲻencoded>
    {
        private _pctⲻencodedTranscriber()
        {
        }
        
        public static _pctⲻencodedTranscriber Instance { get; } = new _pctⲻencodedTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._pctⲻencoded value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Inners._ʺx25ʺTranscriber.Instance.Transcribe(value._ʺx25ʺ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._HEXDIGTranscriber.Instance.Transcribe(value._HEXDIG_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._HEXDIGTranscriber.Instance.Transcribe(value._HEXDIG_2, builder);

        }
    }
    
}

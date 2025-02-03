namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _binaryTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._binary>
    {
        private _binaryTranscriber()
        {
        }
        
        public static _binaryTranscriber Instance { get; } = new _binaryTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._binary value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Inners._ʺx62x69x6Ex61x72x79ʺTranscriber.Instance.Transcribe(value._ʺx62x69x6Ex61x72x79ʺ_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._binaryValueTranscriber.Instance.Transcribe(value._binaryValue_1, builder);
__GeneratedOdataV2.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

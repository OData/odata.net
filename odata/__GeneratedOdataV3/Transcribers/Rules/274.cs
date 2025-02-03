namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _binaryValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._binaryValue>
    {
        private _binaryValueTranscriber()
        {
        }
        
        public static _binaryValueTranscriber Instance { get; } = new _binaryValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._binaryValue value, System.Text.StringBuilder builder)
        {
            foreach (var _Ⲥ4base64charↃ_1 in value._Ⲥ4base64charↃ_1)
{
Inners._Ⲥ4base64charↃTranscriber.Instance.Transcribe(_Ⲥ4base64charↃ_1, builder);
}
if (value._base64b16Ⳇbase64b8_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._base64b16Ⳇbase64b8Transcriber.Instance.Transcribe(value._base64b16Ⳇbase64b8_1, builder);
}

        }
    }
    
}

namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _keyPropertyValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._keyPropertyValue>
    {
        private _keyPropertyValueTranscriber()
        {
        }
        
        public static _keyPropertyValueTranscriber Instance { get; } = new _keyPropertyValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._keyPropertyValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._primitiveLiteralTranscriber.Instance.Transcribe(value._primitiveLiteral_1, builder);

        }
    }
    
}

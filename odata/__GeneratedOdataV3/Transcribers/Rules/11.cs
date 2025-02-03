namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _keyPropertyValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._keyPropertyValue>
    {
        private _keyPropertyValueTranscriber()
        {
        }
        
        public static _keyPropertyValueTranscriber Instance { get; } = new _keyPropertyValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._keyPropertyValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._primitiveLiteralTranscriber.Instance.Transcribe(value._primitiveLiteral_1, builder);

        }
    }
    
}

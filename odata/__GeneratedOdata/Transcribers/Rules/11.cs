namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _keyPropertyValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._keyPropertyValue>
    {
        private _keyPropertyValueTranscriber()
        {
        }
        
        public static _keyPropertyValueTranscriber Instance { get; } = new _keyPropertyValueTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._keyPropertyValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._primitiveLiteralTranscriber.Instance.Transcribe(value._primitiveLiteral_1, builder);

        }
    }
    
}
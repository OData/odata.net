namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _enumMemberValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._enumMemberValue>
    {
        private _enumMemberValueTranscriber()
        {
        }
        
        public static _enumMemberValueTranscriber Instance { get; } = new _enumMemberValueTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._enumMemberValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._int64ValueTranscriber.Instance.Transcribe(value._int64Value_1, builder);

        }
    }
    
}

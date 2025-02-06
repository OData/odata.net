namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _enumMemberValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._enumMemberValue>
    {
        private _enumMemberValueTranscriber()
        {
        }
        
        public static _enumMemberValueTranscriber Instance { get; } = new _enumMemberValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._enumMemberValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._int64ValueTranscriber.Instance.Transcribe(value._int64Value_1, builder);

        }
    }
    
}

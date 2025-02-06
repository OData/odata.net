namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _EQ_customValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._EQ_customValue>
    {
        private _EQ_customValueTranscriber()
        {
        }
        
        public static _EQ_customValueTranscriber Instance { get; } = new _EQ_customValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._EQ_customValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._customValueTranscriber.Instance.Transcribe(value._customValue_1, builder);

        }
    }
    
}

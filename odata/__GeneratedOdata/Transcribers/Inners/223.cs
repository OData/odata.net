namespace __GeneratedOdata.Trancsribers.Inners
{
    public sealed class _EQ_customValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Inners._EQ_customValue>
    {
        private _EQ_customValueTranscriber()
        {
        }
        
        public static _EQ_customValueTranscriber Instance { get; } = new _EQ_customValueTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Inners._EQ_customValue value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
__GeneratedOdata.Trancsribers.Rules._customValueTranscriber.Instance.Transcribe(value._customValue_1, builder);

        }
    }
    
}

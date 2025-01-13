namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _customQueryOptionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._customQueryOption>
    {
        private _customQueryOptionTranscriber()
        {
        }
        
        public static _customQueryOptionTranscriber Instance { get; } = new _customQueryOptionTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._customQueryOption value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._customNameTranscriber.Instance.Transcribe(value._customName_1, builder);
if (value._EQ_customValue_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._EQ_customValueTranscriber.Instance.Transcribe(value._EQ_customValue_1, builder);
}

        }
    }
    
}

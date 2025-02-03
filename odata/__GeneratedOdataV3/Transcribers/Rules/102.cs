namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _customQueryOptionTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._customQueryOption>
    {
        private _customQueryOptionTranscriber()
        {
        }
        
        public static _customQueryOptionTranscriber Instance { get; } = new _customQueryOptionTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._customQueryOption value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._customNameTranscriber.Instance.Transcribe(value._customName_1, builder);
if (value._EQ_customValue_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._EQ_customValueTranscriber.Instance.Transcribe(value._EQ_customValue_1, builder);
}

        }
    }
    
}

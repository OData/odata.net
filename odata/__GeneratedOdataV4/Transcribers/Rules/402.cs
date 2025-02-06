namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _segmentⲻnzTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._segmentⲻnz>
    {
        private _segmentⲻnzTranscriber()
        {
        }
        
        public static _segmentⲻnzTranscriber Instance { get; } = new _segmentⲻnzTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._segmentⲻnz value, System.Text.StringBuilder builder)
        {
            foreach (var _pchar_1 in value._pchar_1)
{
__GeneratedOdataV4.Trancsribers.Rules._pcharTranscriber.Instance.Transcribe(_pchar_1, builder);
}

        }
    }
    
}

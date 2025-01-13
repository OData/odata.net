namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _segmentⲻnzTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._segmentⲻnz>
    {
        private _segmentⲻnzTranscriber()
        {
        }
        
        public static _segmentⲻnzTranscriber Instance { get; } = new _segmentⲻnzTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._segmentⲻnz value, System.Text.StringBuilder builder)
        {
            foreach (var _pchar_1 in value._pchar_1)
{
__GeneratedOdata.Trancsribers.Rules._pcharTranscriber.Instance.Transcribe(_pchar_1, builder);
}

        }
    }
    
}

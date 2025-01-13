namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _segmentTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._segment>
    {
        private _segmentTranscriber()
        {
        }
        
        public static _segmentTranscriber Instance { get; } = new _segmentTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._segment value, System.Text.StringBuilder builder)
        {
            foreach (var _pchar_1 in value._pchar_1)
{
__GeneratedOdata.Trancsribers.Rules._pcharTranscriber.Instance.Transcribe(_pchar_1, builder);
}

        }
    }
    
}

namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _segmentTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._segment>
    {
        private _segmentTranscriber()
        {
        }
        
        public static _segmentTranscriber Instance { get; } = new _segmentTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._segment value, System.Text.StringBuilder builder)
        {
            foreach (var _pchar_1 in value._pchar_1)
{
__GeneratedOdataV4.Trancsribers.Rules._pcharTranscriber.Instance.Transcribe(_pchar_1, builder);
}

        }
    }
    
}

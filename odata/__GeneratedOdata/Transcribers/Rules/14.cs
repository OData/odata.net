namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _keyPathLiteralTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._keyPathLiteral>
    {
        private _keyPathLiteralTranscriber()
        {
        }
        
        public static _keyPathLiteralTranscriber Instance { get; } = new _keyPathLiteralTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._keyPathLiteral value, System.Text.StringBuilder builder)
        {
            foreach (var _pchar_1 in value._pchar_1)
{
__GeneratedOdata.Trancsribers.Rules._pcharTranscriber.Instance.Transcribe(_pchar_1, builder);
}

        }
    }
    
}

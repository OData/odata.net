namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _h16Transcriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._h16>
    {
        private _h16Transcriber()
        {
        }
        
        public static _h16Transcriber Instance { get; } = new _h16Transcriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._h16 value, System.Text.StringBuilder builder)
        {
            foreach (var _HEXDIG_1 in value._HEXDIG_1)
{
__GeneratedOdata.Trancsribers.Rules._HEXDIGTranscriber.Instance.Transcribe(_HEXDIG_1, builder);
}

        }
    }
    
}

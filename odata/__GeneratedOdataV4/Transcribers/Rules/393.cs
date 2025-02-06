namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _h16Transcriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._h16>
    {
        private _h16Transcriber()
        {
        }
        
        public static _h16Transcriber Instance { get; } = new _h16Transcriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._h16 value, System.Text.StringBuilder builder)
        {
            foreach (var _HEXDIG_1 in value._HEXDIG_1)
{
__GeneratedOdataV4.Trancsribers.Rules._HEXDIGTranscriber.Instance.Transcribe(_HEXDIG_1, builder);
}

        }
    }
    
}

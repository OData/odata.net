namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _schemeTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._scheme>
    {
        private _schemeTranscriber()
        {
        }
        
        public static _schemeTranscriber Instance { get; } = new _schemeTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._scheme value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._ALPHATranscriber.Instance.Transcribe(value._ALPHA_1, builder);
foreach (var _ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃ_1 in value._ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃTranscriber.Instance.Transcribe(_ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃ_1, builder);
}

        }
    }
    
}

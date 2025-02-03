namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _schemeTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._scheme>
    {
        private _schemeTranscriber()
        {
        }
        
        public static _schemeTranscriber Instance { get; } = new _schemeTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._scheme value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV2.Trancsribers.Rules._ALPHATranscriber.Instance.Transcribe(value._ALPHA_1, builder);
foreach (var _ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃ_1 in value._ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃ_1)
{
Inners._ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃTranscriber.Instance.Transcribe(_ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃ_1, builder);
}

        }
    }
    
}

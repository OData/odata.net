namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _serviceRootTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._serviceRoot>
    {
        private _serviceRootTranscriber()
        {
        }
        
        public static _serviceRootTranscriber Instance { get; } = new _serviceRootTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._serviceRoot value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._Ⲥʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺↃTranscriber.Instance.Transcribe(value._Ⲥʺx68x74x74x70x73ʺⳆʺx68x74x74x70ʺↃ_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx3Ax2Fx2FʺTranscriber.Instance.Transcribe(value._ʺx3Ax2Fx2Fʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._hostTranscriber.Instance.Transcribe(value._host_1, builder);
if (value._ʺx3Aʺ_port_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx3Aʺ_portTranscriber.Instance.Transcribe(value._ʺx3Aʺ_port_1, builder);
}
__GeneratedOdata.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(value._ʺx2Fʺ_1, builder);
foreach (var _Ⲥsegmentⲻnz_ʺx2FʺↃ_1 in value._Ⲥsegmentⲻnz_ʺx2FʺↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._Ⲥsegmentⲻnz_ʺx2FʺↃTranscriber.Instance.Transcribe(_Ⲥsegmentⲻnz_ʺx2FʺↃ_1, builder);
}

        }
    }
    
}
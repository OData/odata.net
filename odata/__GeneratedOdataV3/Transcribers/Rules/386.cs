namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _authorityTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._authority>
    {
        private _authorityTranscriber()
        {
        }
        
        public static _authorityTranscriber Instance { get; } = new _authorityTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._authority value, System.Text.StringBuilder builder)
        {
            if (value._userinfo_ʺx40ʺ_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._userinfo_ʺx40ʺTranscriber.Instance.Transcribe(value._userinfo_ʺx40ʺ_1, builder);
}
__GeneratedOdataV3.Trancsribers.Rules._hostTranscriber.Instance.Transcribe(value._host_1, builder);
if (value._ʺx3Aʺ_port_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._ʺx3Aʺ_portTranscriber.Instance.Transcribe(value._ʺx3Aʺ_port_1, builder);
}

        }
    }
    
}

namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _authorityTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._authority>
    {
        private _authorityTranscriber()
        {
        }
        
        public static _authorityTranscriber Instance { get; } = new _authorityTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._authority value, System.Text.StringBuilder builder)
        {
            if (value._userinfo_ʺx40ʺ_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._userinfo_ʺx40ʺTranscriber.Instance.Transcribe(value._userinfo_ʺx40ʺ_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._hostTranscriber.Instance.Transcribe(value._host_1, builder);
if (value._ʺx3Aʺ_port_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx3Aʺ_portTranscriber.Instance.Transcribe(value._ʺx3Aʺ_port_1, builder);
}

        }
    }
    
}

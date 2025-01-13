namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _deltatokenTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._deltatoken>
    {
        private _deltatokenTranscriber()
        {
        }
        
        public static _deltatokenTranscriber Instance { get; } = new _deltatokenTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._deltatoken value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Inners._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6EʺTranscriber.Instance.Transcribe(value._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
foreach (var _ⲤqcharⲻnoⲻAMPↃ_1 in value._ⲤqcharⲻnoⲻAMPↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._ⲤqcharⲻnoⲻAMPↃTranscriber.Instance.Transcribe(_ⲤqcharⲻnoⲻAMPↃ_1, builder);
}

        }
    }
    
}

namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _deltatokenTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._deltatoken>
    {
        private _deltatokenTranscriber()
        {
        }
        
        public static _deltatokenTranscriber Instance { get; } = new _deltatokenTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._deltatoken value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Inners._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6EʺTranscriber.Instance.Transcribe(value._ʺx24x64x65x6Cx74x61x74x6Fx6Bx65x6Eʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
foreach (var _ⲤqcharⲻnoⲻAMPↃ_1 in value._ⲤqcharⲻnoⲻAMPↃ_1)
{
__GeneratedOdataV4.Trancsribers.Inners._ⲤqcharⲻnoⲻAMPↃTranscriber.Instance.Transcribe(_ⲤqcharⲻnoⲻAMPↃ_1, builder);
}

        }
    }
    
}

namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _skiptokenTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._skiptoken>
    {
        private _skiptokenTranscriber()
        {
        }
        
        public static _skiptokenTranscriber Instance { get; } = new _skiptokenTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._skiptoken value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Inners._ʺx24x73x6Bx69x70x74x6Fx6Bx65x6EʺTranscriber.Instance.Transcribe(value._ʺx24x73x6Bx69x70x74x6Fx6Bx65x6Eʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
foreach (var _ⲤqcharⲻnoⲻAMPↃ_1 in value._ⲤqcharⲻnoⲻAMPↃ_1)
{
__GeneratedOdataV4.Trancsribers.Inners._ⲤqcharⲻnoⲻAMPↃTranscriber.Instance.Transcribe(_ⲤqcharⲻnoⲻAMPↃ_1, builder);
}

        }
    }
    
}

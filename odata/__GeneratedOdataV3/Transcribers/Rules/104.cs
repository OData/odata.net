namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _customValueTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._customValue>
    {
        private _customValueTranscriber()
        {
        }
        
        public static _customValueTranscriber Instance { get; } = new _customValueTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._customValue value, System.Text.StringBuilder builder)
        {
            foreach (var _ⲤqcharⲻnoⲻAMPↃ_1 in value._ⲤqcharⲻnoⲻAMPↃ_1)
{
Inners._ⲤqcharⲻnoⲻAMPↃTranscriber.Instance.Transcribe(_ⲤqcharⲻnoⲻAMPↃ_1, builder);
}

        }
    }
    
}

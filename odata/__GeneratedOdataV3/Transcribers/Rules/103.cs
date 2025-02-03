namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _customNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._customName>
    {
        private _customNameTranscriber()
        {
        }
        
        public static _customNameTranscriber Instance { get; } = new _customNameTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._customName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLARTranscriber.Instance.Transcribe(value._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR_1, builder);
foreach (var _ⲤqcharⲻnoⲻAMPⲻEQↃ_1 in value._ⲤqcharⲻnoⲻAMPⲻEQↃ_1)
{
Inners._ⲤqcharⲻnoⲻAMPⲻEQↃTranscriber.Instance.Transcribe(_ⲤqcharⲻnoⲻAMPⲻEQↃ_1, builder);
}

        }
    }
    
}

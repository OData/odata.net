namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _EQⲻhTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._EQⲻh>
    {
        private _EQⲻhTranscriber()
        {
        }
        
        public static _EQⲻhTranscriber Instance { get; } = new _EQⲻhTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._EQⲻh value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._BWSⲻhTranscriber.Instance.Transcribe(value._BWSⲻh_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._BWSⲻhTranscriber.Instance.Transcribe(value._BWSⲻh_2, builder);

        }
    }
    
}

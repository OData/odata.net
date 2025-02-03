namespace __GeneratedOdataV2.Trancsribers.Rules
{
    public sealed class _IRIⲻinⲻqueryTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV2.CstNodes.Rules._IRIⲻinⲻquery>
    {
        private _IRIⲻinⲻqueryTranscriber()
        {
        }
        
        public static _IRIⲻinⲻqueryTranscriber Instance { get; } = new _IRIⲻinⲻqueryTranscriber();
        
        public void Transcribe(__GeneratedOdataV2.CstNodes.Rules._IRIⲻinⲻquery value, System.Text.StringBuilder builder)
        {
            foreach (var _qcharⲻnoⲻAMP_1 in value._qcharⲻnoⲻAMP_1)
{
__GeneratedOdataV2.Trancsribers.Rules._qcharⲻnoⲻAMPTranscriber.Instance.Transcribe(_qcharⲻnoⲻAMP_1, builder);
}

        }
    }
    
}

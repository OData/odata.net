namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _searchPhraseTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._searchPhrase>
    {
        private _searchPhraseTranscriber()
        {
        }
        
        public static _searchPhraseTranscriber Instance { get; } = new _searchPhraseTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._searchPhrase value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_1, builder);
foreach (var _qcharⲻnoⲻAMPⲻDQUOTE_1 in value._qcharⲻnoⲻAMPⲻDQUOTE_1)
{
__GeneratedOdataV4.Trancsribers.Rules._qcharⲻnoⲻAMPⲻDQUOTETranscriber.Instance.Transcribe(_qcharⲻnoⲻAMPⲻDQUOTE_1, builder);
}
__GeneratedOdataV4.Trancsribers.Rules._quotationⲻmarkTranscriber.Instance.Transcribe(value._quotationⲻmark_2, builder);

        }
    }
    
}

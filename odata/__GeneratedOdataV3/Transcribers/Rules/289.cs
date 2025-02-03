namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _stringTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._string>
    {
        private _stringTranscriber()
        {
        }
        
        public static _stringTranscriber Instance { get; } = new _stringTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._string value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
foreach (var _ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ_1 in value._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ_1)
{
Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃTranscriber.Instance.Transcribe(_ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ_1, builder);
}
__GeneratedOdataV3.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

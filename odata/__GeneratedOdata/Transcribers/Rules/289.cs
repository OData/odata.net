namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _stringTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._string>
    {
        private _stringTranscriber()
        {
        }
        
        public static _stringTranscriber Instance { get; } = new _stringTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._string value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_1, builder);
foreach (var _ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ_1 in value._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃTranscriber.Instance.Transcribe(_ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._SQUOTETranscriber.Instance.Transcribe(value._SQUOTE_2, builder);

        }
    }
    
}

namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _queryOptionsTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._queryOptions>
    {
        private _queryOptionsTranscriber()
        {
        }
        
        public static _queryOptionsTranscriber Instance { get; } = new _queryOptionsTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._queryOptions value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._queryOptionTranscriber.Instance.Transcribe(value._queryOption_1, builder);
foreach (var _Ⲥʺx26ʺ_queryOptionↃ_1 in value._Ⲥʺx26ʺ_queryOptionↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._Ⲥʺx26ʺ_queryOptionↃTranscriber.Instance.Transcribe(_Ⲥʺx26ʺ_queryOptionↃ_1, builder);
}

        }
    }
    
}
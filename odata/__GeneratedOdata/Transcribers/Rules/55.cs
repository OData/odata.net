namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _entityOptionsTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._entityOptions>
    {
        private _entityOptionsTranscriber()
        {
        }
        
        public static _entityOptionsTranscriber Instance { get; } = new _entityOptionsTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._entityOptions value, System.Text.StringBuilder builder)
        {
            foreach (var _ⲤentityIdOption_ʺx26ʺↃ_1 in value._ⲤentityIdOption_ʺx26ʺↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._ⲤentityIdOption_ʺx26ʺↃTranscriber.Instance.Transcribe(_ⲤentityIdOption_ʺx26ʺↃ_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._idTranscriber.Instance.Transcribe(value._id_1, builder);
foreach (var _Ⲥʺx26ʺ_entityIdOptionↃ_1 in value._Ⲥʺx26ʺ_entityIdOptionↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._Ⲥʺx26ʺ_entityIdOptionↃTranscriber.Instance.Transcribe(_Ⲥʺx26ʺ_entityIdOptionↃ_1, builder);
}

        }
    }
    
}
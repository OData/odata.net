namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _entityCastOptionsTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._entityCastOptions>
    {
        private _entityCastOptionsTranscriber()
        {
        }
        
        public static _entityCastOptionsTranscriber Instance { get; } = new _entityCastOptionsTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._entityCastOptions value, System.Text.StringBuilder builder)
        {
            foreach (var _ⲤentityCastOption_ʺx26ʺↃ_1 in value._ⲤentityCastOption_ʺx26ʺↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._ⲤentityCastOption_ʺx26ʺↃTranscriber.Instance.Transcribe(_ⲤentityCastOption_ʺx26ʺↃ_1, builder);
}
__GeneratedOdata.Trancsribers.Rules._idTranscriber.Instance.Transcribe(value._id_1, builder);
foreach (var _Ⲥʺx26ʺ_entityCastOptionↃ_1 in value._Ⲥʺx26ʺ_entityCastOptionↃ_1)
{
__GeneratedOdata.Trancsribers.Inners._Ⲥʺx26ʺ_entityCastOptionↃTranscriber.Instance.Transcribe(_Ⲥʺx26ʺ_entityCastOptionↃ_1, builder);
}

        }
    }
    
}

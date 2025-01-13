namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _qualifiedEnumTypeNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._qualifiedEnumTypeName>
    {
        private _qualifiedEnumTypeNameTranscriber()
        {
        }
        
        public static _qualifiedEnumTypeNameTranscriber Instance { get; } = new _qualifiedEnumTypeNameTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._qualifiedEnumTypeName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._enumerationTypeNameTranscriber.Instance.Transcribe(value._enumerationTypeName_1, builder);

        }
    }
    
}

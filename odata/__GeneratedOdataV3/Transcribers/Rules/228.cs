namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _qualifiedEnumTypeNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._qualifiedEnumTypeName>
    {
        private _qualifiedEnumTypeNameTranscriber()
        {
        }
        
        public static _qualifiedEnumTypeNameTranscriber Instance { get; } = new _qualifiedEnumTypeNameTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._qualifiedEnumTypeName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdataV3.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._enumerationTypeNameTranscriber.Instance.Transcribe(value._enumerationTypeName_1, builder);

        }
    }
    
}

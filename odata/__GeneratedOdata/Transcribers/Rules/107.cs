namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _entitySetTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._entitySet>
    {
        private _entitySetTranscriber()
        {
        }
        
        public static _entitySetTranscriber Instance { get; } = new _entitySetTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._entitySet value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._entitySetNameTranscriber.Instance.Transcribe(value._entitySetName_1, builder);
foreach (var _ⲤcontainmentNavigationↃ_1 in value._ⲤcontainmentNavigationↃ_1)
{
Inners._ⲤcontainmentNavigationↃTranscriber.Instance.Transcribe(_ⲤcontainmentNavigationↃ_1, builder);
}
if (value._ʺx2Fʺ_qualifiedEntityTypeName_1 != null)
{
__GeneratedOdata.Trancsribers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameTranscriber.Instance.Transcribe(value._ʺx2Fʺ_qualifiedEntityTypeName_1, builder);
}

        }
    }
    
}

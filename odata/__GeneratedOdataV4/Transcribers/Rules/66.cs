namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _expandPathTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._expandPath>
    {
        private _expandPathTranscriber()
        {
        }
        
        public static _expandPathTranscriber Instance { get; } = new _expandPathTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._expandPath value, System.Text.StringBuilder builder)
        {
            if (value._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ_1 != null)
{
__GeneratedOdataV4.Trancsribers.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2FʺTranscriber.Instance.Transcribe(value._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ_1, builder);
}
foreach (var _ⲤⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡Ↄ_1 in value._ⲤⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡Ↄ_1)
{
Inners._ⲤⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡ↃTranscriber.Instance.Transcribe(_ⲤⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡Ↄ_1, builder);
}
__GeneratedOdataV4.Trancsribers.Inners._ⲤSTARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡ↃTranscriber.Instance.Transcribe(value._ⲤSTARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Ↄ_1, builder);

        }
    }
    
}

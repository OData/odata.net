namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _expandPathParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._expandPath> Instance { get; } = from _ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2FʺParser.Instance.Optional()
from _ⲤⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡Ↄ_1 in Inners._ⲤⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡ↃParser.Instance.Many()
from _ⲤSTARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Ↄ_1 in __GeneratedOdata.Parsers.Inners._ⲤSTARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡ↃParser.Instance
select new __GeneratedOdata.CstNodes.Rules._expandPath(_ⲤqualifiedEntityTypeNameⳆqualifiedComplexTypeNameↃ_ʺx2Fʺ_1.GetOrElse(null), _ⲤⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡Ↄ_1, _ⲤSTARⳆstreamPropertyⳆnavigationProperty_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Ↄ_1);
    }
    
}

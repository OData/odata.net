namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _ⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡Parser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡> Instance { get; } = from _ⲤcomplexPropertyⳆcomplexColPropertyↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃParser.Instance
from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _qualifiedComplexTypeName_ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._qualifiedComplexTypeName_ʺx2FʺParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡(_ⲤcomplexPropertyⳆcomplexColPropertyↃ_1, _ʺx2Fʺ_1, _qualifiedComplexTypeName_ʺx2Fʺ_1.GetOrElse(null));
    }
    
}

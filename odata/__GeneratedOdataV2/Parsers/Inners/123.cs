namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡Parser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡> Instance { get; } = from _ⲤcomplexPropertyⳆcomplexColPropertyↃ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃParser.Instance
from _ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2FʺParser.Instance
from _qualifiedComplexTypeName_ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._qualifiedComplexTypeName_ʺx2FʺParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃ_ʺx2Fʺ_꘡qualifiedComplexTypeName_ʺx2Fʺ꘡(_ⲤcomplexPropertyⳆcomplexColPropertyↃ_1, _ʺx2Fʺ_1, _qualifiedComplexTypeName_ʺx2Fʺ_1.GetOrElse(null));
    }
    
}

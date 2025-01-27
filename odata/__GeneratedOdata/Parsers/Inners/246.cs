namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡Parser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _complexProperty_1 in __GeneratedOdata.Parsers.Rules._complexPropertyParser.Instance
from _ʺx2Fʺ_qualifiedComplexTypeName_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡(_ʺx2Fʺ_1, _complexProperty_1, _ʺx2Fʺ_qualifiedComplexTypeName_1.GetOrElse(null));
    }
    
}

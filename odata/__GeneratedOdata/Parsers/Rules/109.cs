namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _navigationParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._navigation> Instance { get; } = from _Ⲥʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡Ↄ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡ↃParser.Instance.Many()
from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _navigationProperty_1 in __GeneratedOdata.Parsers.Rules._navigationPropertyParser.Instance
select new __GeneratedOdata.CstNodes.Rules._navigation(_Ⲥʺx2Fʺ_complexProperty_꘡ʺx2Fʺ_qualifiedComplexTypeName꘡Ↄ_1, _ʺx2Fʺ_1, _navigationProperty_1);
    }
    
}

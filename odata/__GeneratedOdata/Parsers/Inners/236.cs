namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Parser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡> Instance { get; } = from _navigation_1 in __GeneratedOdata.Parsers.Rules._navigationParser.Instance
from _ⲤcontainmentNavigationↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤcontainmentNavigationↃParser.Instance.Many()
from _ʺx2Fʺ_qualifiedEntityTypeName_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Inners._navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡(_navigation_1, _ⲤcontainmentNavigationↃ_1, _ʺx2Fʺ_qualifiedEntityTypeName_1.GetOrElse(null));
    }
    
}

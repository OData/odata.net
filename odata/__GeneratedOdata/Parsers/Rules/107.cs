namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _entitySetParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._entitySet> Instance { get; } = from _entitySetName_1 in __GeneratedOdata.Parsers.Rules._entitySetNameParser.Instance
from _ⲤcontainmentNavigationↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤcontainmentNavigationↃParser.Instance.Many()
from _ʺx2Fʺ_qualifiedEntityTypeName_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._entitySet(_entitySetName_1, _ⲤcontainmentNavigationↃ_1, _ʺx2Fʺ_qualifiedEntityTypeName_1.GetOrElse(null));
    }
    
}

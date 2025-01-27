namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _containmentNavigationParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._containmentNavigation> Instance { get; } = from _keyPredicate_1 in __GeneratedOdata.Parsers.Rules._keyPredicateParser.Instance
from _ʺx2Fʺ_qualifiedEntityTypeName_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional()
from _navigation_1 in __GeneratedOdata.Parsers.Rules._navigationParser.Instance
select new __GeneratedOdata.CstNodes.Rules._containmentNavigation(_keyPredicate_1, _ʺx2Fʺ_qualifiedEntityTypeName_1.GetOrElse(null), _navigation_1);
    }
    
}

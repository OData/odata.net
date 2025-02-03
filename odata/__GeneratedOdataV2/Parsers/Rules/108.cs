namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _containmentNavigationParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._containmentNavigation> Instance { get; } = from _keyPredicate_1 in __GeneratedOdataV2.Parsers.Rules._keyPredicateParser.Instance
from _ʺx2Fʺ_qualifiedEntityTypeName_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional()
from _navigation_1 in __GeneratedOdataV2.Parsers.Rules._navigationParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._containmentNavigation(_keyPredicate_1, _ʺx2Fʺ_qualifiedEntityTypeName_1.GetOrElse(null), _navigation_1);
    }
    
}

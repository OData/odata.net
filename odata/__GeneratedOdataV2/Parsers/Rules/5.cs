namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _collectionNavigationParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._collectionNavigation> Instance { get; } = from _ʺx2Fʺ_qualifiedEntityTypeName_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional()
from _collectionNavPath_1 in __GeneratedOdataV2.Parsers.Rules._collectionNavPathParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._collectionNavigation(_ʺx2Fʺ_qualifiedEntityTypeName_1.GetOrElse(null), _collectionNavPath_1.GetOrElse(null));
    }
    
}

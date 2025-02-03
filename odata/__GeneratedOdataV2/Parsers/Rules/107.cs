namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _entitySetParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._entitySet> Instance { get; } = from _entitySetName_1 in __GeneratedOdataV2.Parsers.Rules._entitySetNameParser.Instance
from _ⲤcontainmentNavigationↃ_1 in Inners._ⲤcontainmentNavigationↃParser.Instance.Many()
from _ʺx2Fʺ_qualifiedEntityTypeName_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._entitySet(_entitySetName_1, _ⲤcontainmentNavigationↃ_1, _ʺx2Fʺ_qualifiedEntityTypeName_1.GetOrElse(null));
    }
    
}

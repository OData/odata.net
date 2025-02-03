namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _filterInPathParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._filterInPath> Instance { get; } = from _ʺx2Fx24x66x69x6Cx74x65x72ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2Fx24x66x69x6Cx74x65x72ʺParser.Instance
from _EQ_1 in __GeneratedOdataV2.Parsers.Rules._EQParser.Instance
from _parameterAlias_1 in __GeneratedOdataV2.Parsers.Rules._parameterAliasParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._filterInPath(_ʺx2Fx24x66x69x6Cx74x65x72ʺ_1, _EQ_1, _parameterAlias_1);
    }
    
}

namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _maxDateTimeMethodCallExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._maxDateTimeMethodCallExpr> Instance { get; } = from _ʺx6Dx61x78x64x61x74x65x74x69x6Dx65ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx6Dx61x78x64x61x74x65x74x69x6Dx65ʺParser.Instance
from _OPEN_1 in __GeneratedOdataV2.Parsers.Rules._OPENParser.Instance
from _BWS_1 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
from _CLOSE_1 in __GeneratedOdataV2.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._maxDateTimeMethodCallExpr(_ʺx6Dx61x78x64x61x74x65x74x69x6Dx65ʺ_1, _OPEN_1, _BWS_1, _CLOSE_1);
    }
    
}

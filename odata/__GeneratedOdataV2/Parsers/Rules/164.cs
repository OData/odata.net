namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _nowMethodCallExprParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._nowMethodCallExpr> Instance { get; } = from _ʺx6Ex6Fx77ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx6Ex6Fx77ʺParser.Instance
from _OPEN_1 in __GeneratedOdataV2.Parsers.Rules._OPENParser.Instance
from _BWS_1 in __GeneratedOdataV2.Parsers.Rules._BWSParser.Instance
from _CLOSE_1 in __GeneratedOdataV2.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._nowMethodCallExpr(_ʺx6Ex6Fx77ʺ_1, _OPEN_1, _BWS_1, _CLOSE_1);
    }
    
}

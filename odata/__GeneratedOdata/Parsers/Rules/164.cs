namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _nowMethodCallExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._nowMethodCallExpr> Instance { get; } = from _ʺx6Ex6Fx77ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx6Ex6Fx77ʺParser.Instance
from _OPEN_1 in __GeneratedOdata.Parsers.Rules._OPENParser.Instance
from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _CLOSE_1 in __GeneratedOdata.Parsers.Rules._CLOSEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._nowMethodCallExpr(_ʺx6Ex6Fx77ʺ_1, _OPEN_1, _BWS_1, _CLOSE_1);
    }
    
}

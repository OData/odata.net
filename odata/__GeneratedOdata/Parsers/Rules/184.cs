namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _hasExprParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._hasExpr> Instance { get; } = from _RWS_1 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _ʺx68x61x73ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx68x61x73ʺParser.Instance
from _RWS_2 in __GeneratedOdata.Parsers.Rules._RWSParser.Instance
from _enum_1 in __GeneratedOdata.Parsers.Rules._enumParser.Instance
select new __GeneratedOdata.CstNodes.Rules._hasExpr(_RWS_1, _ʺx68x61x73ʺ_1, _RWS_2, _enum_1);
    }
    
}

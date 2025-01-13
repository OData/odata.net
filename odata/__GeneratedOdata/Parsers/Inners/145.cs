namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx64x65x73x63ʺParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx64x65x73x63ʺ> Instance { get; } = from _x64_1 in __GeneratedOdata.Parsers.Inners._x64Parser.Instance
from _x65_1 in __GeneratedOdata.Parsers.Inners._x65Parser.Instance
from _x73_1 in __GeneratedOdata.Parsers.Inners._x73Parser.Instance
from _x63_1 in __GeneratedOdata.Parsers.Inners._x63Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx64x65x73x63ʺ(_x64_1, _x65_1, _x73_1, _x63_1);
    }
    
}

namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx63x61x73x74ʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx63x61x73x74ʺ> Instance { get; } = from _x63_1 in __GeneratedOdata.Parsers.Inners._x63Parser.Instance
from _x61_1 in __GeneratedOdata.Parsers.Inners._x61Parser.Instance
from _x73_1 in __GeneratedOdata.Parsers.Inners._x73Parser.Instance
from _x74_1 in __GeneratedOdata.Parsers.Inners._x74Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx63x61x73x74ʺ(_x63_1, _x61_1, _x73_1, _x74_1);
    }
    
}

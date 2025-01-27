namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx25x33x41ʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx25x33x41ʺ> Instance { get; } = from _x25_1 in __GeneratedOdata.Parsers.Inners._x25Parser.Instance
from _x33_1 in __GeneratedOdata.Parsers.Inners._x33Parser.Instance
from _x41_1 in __GeneratedOdata.Parsers.Inners._x41Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx25x33x41ʺ(_x25_1, _x33_1, _x41_1);
    }
    
}

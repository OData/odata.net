namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx25x35x42ʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx25x35x42ʺ> Instance { get; } = from _x25_1 in __GeneratedOdata.Parsers.Inners._x25Parser.Instance
from _x35_1 in __GeneratedOdata.Parsers.Inners._x35Parser.Instance
from _x42_1 in __GeneratedOdata.Parsers.Inners._x42Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx25x35x42ʺ(_x25_1, _x35_1, _x42_1);
    }
    
}

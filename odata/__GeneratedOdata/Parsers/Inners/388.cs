namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx25x35x44ʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx25x35x44ʺ> Instance { get; } = from _x25_1 in __GeneratedOdata.Parsers.Inners._x25Parser.Instance
from _x35_1 in __GeneratedOdata.Parsers.Inners._x35Parser.Instance
from _x44_1 in __GeneratedOdata.Parsers.Inners._x44Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx25x35x44ʺ(_x25_1, _x35_1, _x44_1);
    }
    
}

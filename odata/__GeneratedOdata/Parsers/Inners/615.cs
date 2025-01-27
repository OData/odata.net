namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx25x32x30ʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx25x32x30ʺ> Instance { get; } = from _x25_1 in __GeneratedOdata.Parsers.Inners._x25Parser.Instance
from _x32_1 in __GeneratedOdata.Parsers.Inners._x32Parser.Instance
from _x30_1 in __GeneratedOdata.Parsers.Inners._x30Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx25x32x30ʺ(_x25_1, _x32_1, _x30_1);
    }
    
}

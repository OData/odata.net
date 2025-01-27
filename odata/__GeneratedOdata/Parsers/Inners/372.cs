namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx25x37x42ʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx25x37x42ʺ> Instance { get; } = from _x25_1 in __GeneratedOdata.Parsers.Inners._x25Parser.Instance
from _x37_1 in __GeneratedOdata.Parsers.Inners._x37Parser.Instance
from _x42_1 in __GeneratedOdata.Parsers.Inners._x42Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx25x37x42ʺ(_x25_1, _x37_1, _x42_1);
    }
    
}

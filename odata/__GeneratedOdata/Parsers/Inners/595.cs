namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx77x61x69x74ʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx77x61x69x74ʺ> Instance { get; } = from _x77_1 in __GeneratedOdata.Parsers.Inners._x77Parser.Instance
from _x61_1 in __GeneratedOdata.Parsers.Inners._x61Parser.Instance
from _x69_1 in __GeneratedOdata.Parsers.Inners._x69Parser.Instance
from _x74_1 in __GeneratedOdata.Parsers.Inners._x74Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx77x61x69x74ʺ(_x77_1, _x61_1, _x69_1, _x74_1);
    }
    
}

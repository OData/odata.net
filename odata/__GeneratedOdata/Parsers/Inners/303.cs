namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx64x61x79ʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx64x61x79ʺ> Instance { get; } = from _x64_1 in __GeneratedOdata.Parsers.Inners._x64Parser.Instance
from _x61_1 in __GeneratedOdata.Parsers.Inners._x61Parser.Instance
from _x79_1 in __GeneratedOdata.Parsers.Inners._x79Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx64x61x79ʺ(_x64_1, _x61_1, _x79_1);
    }
    
}

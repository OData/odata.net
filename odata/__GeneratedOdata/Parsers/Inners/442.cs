namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx53x42x79x74x65ʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx53x42x79x74x65ʺ> Instance { get; } = from _x53_1 in __GeneratedOdata.Parsers.Inners._x53Parser.Instance
from _x42_1 in __GeneratedOdata.Parsers.Inners._x42Parser.Instance
from _x79_1 in __GeneratedOdata.Parsers.Inners._x79Parser.Instance
from _x74_1 in __GeneratedOdata.Parsers.Inners._x74Parser.Instance
from _x65_1 in __GeneratedOdata.Parsers.Inners._x65Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx53x42x79x74x65ʺ(_x53_1, _x42_1, _x79_1, _x74_1, _x65_1);
    }
    
}

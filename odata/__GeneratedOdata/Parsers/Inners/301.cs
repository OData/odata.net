namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx79x65x61x72ʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx79x65x61x72ʺ> Instance { get; } = from _x79_1 in __GeneratedOdata.Parsers.Inners._x79Parser.Instance
from _x65_1 in __GeneratedOdata.Parsers.Inners._x65Parser.Instance
from _x61_1 in __GeneratedOdata.Parsers.Inners._x61Parser.Instance
from _x72_1 in __GeneratedOdata.Parsers.Inners._x72Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx79x65x61x72ʺ(_x79_1, _x65_1, _x61_1, _x72_1);
    }
    
}

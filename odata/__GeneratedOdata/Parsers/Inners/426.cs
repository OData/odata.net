namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx44x61x74x65ʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx44x61x74x65ʺ> Instance { get; } = from _x44_1 in __GeneratedOdata.Parsers.Inners._x44Parser.Instance
from _x61_1 in __GeneratedOdata.Parsers.Inners._x61Parser.Instance
from _x74_1 in __GeneratedOdata.Parsers.Inners._x74Parser.Instance
from _x65_1 in __GeneratedOdata.Parsers.Inners._x65Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx44x61x74x65ʺ(_x44_1, _x61_1, _x74_1, _x65_1);
    }
    
}

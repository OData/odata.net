namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx74x72x75x65ʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx74x72x75x65ʺ> Instance { get; } = from _x74_1 in __GeneratedOdata.Parsers.Inners._x74Parser.Instance
from _x72_1 in __GeneratedOdata.Parsers.Inners._x72Parser.Instance
from _x75_1 in __GeneratedOdata.Parsers.Inners._x75Parser.Instance
from _x65_1 in __GeneratedOdata.Parsers.Inners._x65Parser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx74x72x75x65ʺ(_x74_1, _x72_1, _x75_1, _x65_1);
    }
    
}

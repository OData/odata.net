namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx74x72x69x6DʺParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx74x72x69x6Dʺ> Instance { get; } = from _x74_1 in __GeneratedOdata.Parsers.Inners._x74Parser.Instance
from _x72_1 in __GeneratedOdata.Parsers.Inners._x72Parser.Instance
from _x69_1 in __GeneratedOdata.Parsers.Inners._x69Parser.Instance
from _x6D_1 in __GeneratedOdata.Parsers.Inners._x6DParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx74x72x69x6Dʺ(_x74_1, _x72_1, _x69_1, _x6D_1);
    }
    
}
